using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            var tasks = db.Tasks.OrderBy(a => a.DataStart);
            var search = "";

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
            Task task = db.Tasks.Include("Comments")
                .Where(tsk => tsk.TaskId == id)
                .First();
            SetAccesRights();
            return View(task);
        }

        //Adaugarea unui comentariu asociat unui task
        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]

        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if(ModelState.IsValid)
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

        // se afiseaza formularul pentru a completa datele unui articol
        public IActionResult New()
        {
            Task task = new Task();

            return View(task);
        }

        // se adauga articolul din formular in baza de date
        [HttpPost]
        public IActionResult New(Task task)
        {
            task.DataStart = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                TempData["message"] = "Taskul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                return View(task);
            }
        }

        // formular pentru editarea unui articol

        public IActionResult Edit(int id)
        {
            Task task = db.Tasks.First();

            if (User.IsInRole("Admin"))
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
        public ActionResult Delete(int id)
        {
            Task task = db.Tasks
                                .First();
            db.Tasks.Remove(task);
            db.SaveChanges();
            TempData["message"] = "Task-ul a fost sters";
            return RedirectToAction("Index");
        }

        // Conditii de afisare a butoanelore se editare/stergere
        private void SetAccesRights()
        {
            ViewBag.AfisareButoane = true;
            if (User.IsInRole("Organizator"))
            {
                ViewBag.AfisareButoane = true;
            }
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

    }


}
