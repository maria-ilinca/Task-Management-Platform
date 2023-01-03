
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Task_Management_Platform.Data;
using Task_Management_Platform.Models;
using Task = Task_Management_Platform.Models.Task;

namespace Task_Management_Platform.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public TasksController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        // Se afiseaza fiecare task si proiectul din care
        // face parte si statusul
        [Authorize(Roles = "User, Organizer, Admin")]
        public IActionResult Index()
        {
            var tasks = db.Tasks.Include("UserTasks").OrderBy(a => a.DataStart);
            var search = "";

            SetAccesRights();
            //Motor de cautare

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                List<int> taskIds = db.Tasks.Where
                                        (
                                            tsk => tsk.Title.Contains(search)
                                            || tsk.Description.Contains(search)
                                        ).Select(a => a.TaskId).ToList();
                List<int> taskIdsOfCommentsWithSearchString = db.Comments
                            .Where
                            (
                                c => c.Content.Contains(search)
                            ).Select(c => (int)c.TaskId).ToList();
                List<int> mergedIds = taskIds.Union(taskIdsOfCommentsWithSearchString).ToList();

                tasks = db.Tasks.Where(task => mergedIds.Contains(task.TaskId))
                                .Include("Status")
                                .Include("UserTasks") // avem nevoie pentru a afisa pentru fiecare user task-urile in home
                                .Include("User")
                                .OrderBy(a => a.DataStart);
            }

            ViewBag.SearchString = search;


            //Afisare paginata

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            int _perPage = 3;

            if(TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            int totalItems = tasks.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;

            if(!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedTasks = tasks.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            ViewBag.Tasks = paginatedTasks;

            if(search != "")
            {
                ViewBag.PaginationBaseUrl = "/Tasks/Index/?search=" + search + "&page";
            }else
            {
                ViewBag.PaginationBaseUrl = "/Tasks/Index/?page";
            }

            

            return View();
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show(int id)
        {

            SetAccesRights();
            try
            {
                Task task = db.Tasks.Include("Comments")
                                    .Include("UserTasks")
                    .Where(tsk => tsk.TaskId == id)
                    .First();


                // pentru dropdown din show
                ViewBag.UserTasks = from user in db.Users
                                    orderby user.LastName
                                    select user;

                return View(task);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //Adaugarea unui comentariu asociat unui task
        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]

        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);
            // pentru dropdown din show
            ViewBag.UserTasks = from user in db.Users
                                orderby user.LastName
                                select user;
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + comment.TaskId);
            }

            else
            {
                Task tsk = db.Tasks.Include("Comments")
                    .Where(tsk => tsk.TaskId == comment.TaskId)
                    .First();

                SetAccesRights();
                return View(tsk);
            }


        }

        // Assign task to user (asociare task-user )
        [HttpPost]
        [Authorize(Roles = "Organizer, Admin")]
        public IActionResult AddUser([FromForm] UserTask userTask)
        {
            if (ModelState.IsValid)
            {
                if (db.UserTasks
                    .Where(tu => tu.UserId == userTask.UserId)
                    .Where(tu => tu.TaskId == userTask.TaskId)
                    .Count() > 0)
                {
                    
                    TempData["message"] = "Acest user are deja atribuit task-ul";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    db.UserTasks.Add(userTask);
                    db.SaveChanges();
                    Task task = db.Tasks.Include("UserTasks").Where(t => t.TaskId == userTask.TaskId).First();
                    //Task task = db.Tasks.Find(userTask.TaskId);
                    task.UserTasks.Add(userTask);

                    db.SaveChanges();

                    // gasim userul si task-ul din baza de date
                    // adaugam pentru User in Tasks task-ul corespunzator
                    //ApplicationUser user = db.Users.Where(t => t.Id == userTask.UserId).First();
                    //Task task = db.Tasks.Where(t => t.TaskId == userTask.TaskId).First();
                    //user.Tasks.Add(task);
                    //ViewBag.UserTasks = task;
                    
                    TempData["message"] = "Userului i-a fost atribuit acest task";
                    TempData["messageType"] = "alert-success";
                }
            }
            else
            {
                TempData["message"] = "Nu s-a putut atribui task-ul userului";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Tasks/Show/" + userTask.TaskId);
        }

        // se afiseaza formularul pentru a completa datele unui task
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult New()
        {
            Task task = new Task();

            return View(task);
        }



        // se adauga task-ul din formular in baza de date
        [Authorize(Roles ="Organizer,Admin")]
        [HttpPost]
        public IActionResult New(Task task)
        {
            
            task.DataStart = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                // adaugam task-ul in lista cu task-uri din project
                if (TempData["ProjectId"] != null)
                {
                    int? projectId = (int?)TempData["ProjectId"];
                    task.ProjectId = projectId;
                    Project project = db.Projects.Find(projectId);
                    project.Tasks.Add(task);
                    db.SaveChanges();
                    TempData["message"] = "Task-ul a fost adaugat";
                    // resetam ProjectId, altfel ramane setat cu ultimul
                    TempData["ProjectId"] = null;
                    return Redirect($"/Projects/Show/{projectId}");
                }
                db.SaveChanges();
                TempData["message"] = "Task-ul a fost adaugat";

                return RedirectToAction("Index");
            }
            else
            {
                return View(task);
            }

        }

       


        // formular pentru editarea unui articol
        [Authorize(Roles ="Organizer,Admin")]
        public IActionResult Edit(int id)
        {
            Task task = db.Tasks.First();

            if (task.OrganizerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(task);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati aces task";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Organizer, Admin")]
        public IActionResult Edit(int id, Task requestTask)
        {
            Task task = db.Tasks.Find(id);

            if(ModelState.IsValid)
            {
                if (task.OrganizerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    task.Title = requestTask.Title;
                    task.Description = requestTask.Description;
                    task.Status = requestTask.Status;
                    TempData["message"] = "Task-ul a fost modficat";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti aceste drepturi";
                    return RedirectToAction("Index");
                }

            }
            else
            {
                return View(requestTask);
            }
        }
        
        [HttpPost]
        [Authorize(Roles ="Organizer,Admin")]
        public ActionResult Delete(int id)
        {
            Task task = db.Tasks
                                .First();
            if (task.OrganizerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (task.Comments != null)
                {
                    db.Tasks.Remove((Task)task.Comments);
                }
                db.Tasks.Remove(task);
                db.SaveChanges();
                TempData["message"] = "Task-ul a fost sters";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti acest drept";
                return View(task);
            }
        }

        // Conditii de afisare a butoanelore de editare/stergere
        private void SetAccesRights()
        {
            ViewBag.AfisareButoane = false;
            if (User.IsInRole("Organizer"))
            {
                ViewBag.AfisareButoane = true;
            }
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

    }


}
