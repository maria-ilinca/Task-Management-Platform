using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using Task_Management_Platform.Data;
using Task_Management_Platform.Models;
using Task = Task_Management_Platform.Models.Task;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ganss.Xss;
using System.Threading.Tasks;

namespace Task_Management_Platform.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ProjectsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // toti utilizatorii pot vedea echipele existente
        // fiecare utilizator vede echipele din care face parte
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            SetAccesRights();

            if (User.IsInRole("User") || User.IsInRole("Organizer"))
            {
                var projects = from project in db.Projects.Include("User")
                               .Where(p => p.UserId == _userManager.GetUserId(User))
                               select project;
                ViewBag.Projects = projects;

                return View();
            }

            else if (User.IsInRole("Admin"))
            {
                var projects = from project in db.Projects.Include("User")
                               select project;
                ViewBag.Projects = projects;
                return View();
            }

            else
            {
                TempData["message"] = "Nu aveti acest drept";
                return RedirectToAction("Index", "Tasks");
            }


        }

        //Afisare proiect individual

        [Authorize(Roles ="User,Organize,Admin")]
        public IActionResult Show(int id)
        {
            try
            {
                Project project = db.Projects.Include("User")
                                             .Include("Tasks")
                                             .Where(pr => pr.Id == id)
                                             .First();
                SetAccesRights();
                return View(project);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }


        }
        // Adaugarea unui task pentru un proiect
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult AddTask(int id)
        {
            TempData["ProjectId"] = id;
            return Redirect("/Tasks/New");
        }
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult New()
        {
            Project project = new Project();
            return View(project);
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult New(Project project)
        {
            project.UserId = _userManager.GetUserId(User);

            if(ModelState.IsValid)
            {
                db.Projects.Add(project);
                if (TempData["TeamId"] != null)
                {
                    int? teamId = (int?)TempData["TeamId"];
                    project.TeamId = teamId;
                    Team team = db.Teams.Find(teamId);
                    team.Projects.Add(project);
                    db.SaveChanges();
                    TempData["message"] = "Proiectul a fost creat";
                    // resetam TeamId, altfel ramane setat cu ultimul
                    TempData["TeamId"] = null;
                    return Redirect($"/Teams/Show/{teamId}");
                }
                db.SaveChanges();
                TempData["message"] = "Proiectul a fost creat cu succes";
                return RedirectToAction("Index");
            }
            else
            {
                return View(project);
            }
        }
        [Authorize(Roles = "Organizer, Admin")]
        public IActionResult Edit(int id)
        {
            Project project = db.Projects.Include("Tasks")
                                         .Where(prj => prj.Id == id)
                                         .First();
            //project.Tasks = db.Tasks.Where(t => t.ProjectId == id).ToList();
            if (project.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(project);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati acest proiect";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Organizer, Admin")]
        public IActionResult Edit(int id, Project requestProject)
        {
            var sanitizer = new HtmlSanitizer();

            var project = db.Projects.Find(id);

            if(ModelState.IsValid)
            {
                if (project.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    project.Name = requestProject.Name;
                    requestProject.Description = sanitizer.Sanitize(requestProject.Description);
                    project.Description = requestProject.Description;
                    TempData["message"] = "Proiectul a fost modificat";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati acest proiect";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestProject);
            }
        }

        [HttpPost]
        [Authorize(Roles ="Organizer,Admin")]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Include("Tasks")
                                         .Where(prj => prj.Id == id)
                                         .First();

            if(project.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Projects.Remove(project);
                db.SaveChanges();
                TempData["message"] = "Proiectul a fost sters";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti acest proiect";
                return RedirectToAction("Index");
            }
        }

        private void SetAccesRights()
        {
            ViewBag.AfisareButoane = false;

            if(User.IsInRole("Organizer") || User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = _userManager.GetUserId(User);

        }
    }
}
