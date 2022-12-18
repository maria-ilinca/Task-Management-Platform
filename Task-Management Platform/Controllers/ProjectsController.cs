using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using Task_Management_Platform.Data;
using Task_Management_Platform.Models;

namespace Task_Management_Platform.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ProjectsController(
            ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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

            if(User.IsInRole("User") || User.IsInRole("Organizer"))
            {
                var projects = from project in db.Projects.Include("User")
                               .Where(p => p.UserId == _userManager.GetUserId(User))
                               select project;
                ViewBag.Projects = projects;

                return View();
            }

            else if(User.IsInRole("Admin"))
            {
                var projects = from project in db.Projects.Include("User")
                               select project;
                ViewBag.Projects = projects;
                return View();
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi";
                return RedirectToAction("Index", "Tasks");
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
