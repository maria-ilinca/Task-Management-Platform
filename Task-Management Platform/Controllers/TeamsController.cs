using Task_Management_Platform.Data;
using Task_Management_Platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.AspNetCore.Identity;

namespace Task_Management_Platform.Controllers
{
 
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public TeamsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        [HttpGet]
        [Authorize(Roles = "User,Organizer,Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            SetAccesRights();

            //var teams = from team in db.Teams
            //                 orderby team.TeamName
            //                 select team;

            var teams = db.Teams.Include("UserTeams").OrderBy(t => t.TeamName);
            ViewBag.Teams = teams;


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult AddProject(int id)
        {
            TempData["TeamId"] = id;
            return Redirect("/Projects/New");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddUser([FromForm] UserTeam userTeam)
        {
            SetAccesRights();
            if (ModelState.IsValid)
            {
                if (db.UserTeams
                    .Where(ut => ut.UserId == userTeam.UserId)
                    .Where(ut => ut.TeamId == userTeam.TeamId)
                    .Count() > 0)
                {
                    TempData["message"] = "Acest user se afla deja in echipa";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    db.UserTeams.Add(userTeam);
                    db.SaveChanges();
                    Console.WriteLine($"\n\n\n\nTeamId1: {userTeam.TeamId}");
                    Team team = db.Teams.Include("UserTeams")
                                        .Where(t => t.Id == userTeam.TeamId).First();
                    team.UserTeams.Add(userTeam);
                    db.SaveChanges();
                    Console.WriteLine($"Count: {team.UserTeams.Count()}");

                    TempData["message"] = "Userul a fost adaugat cu succes";
                    TempData["messageType"] = "alert-succes";

                }
            }
            else
            {
                TempData["message"] = "Nu s-a putut adauga userul in echipa";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Teams/Show/" + userTeam.TeamId);
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public ActionResult Show(int id)
        {
            SetAccesRights();
            try
            {
                Team team = db.Teams.Include("Projects")
                                    .Where(t => t.Id == id).First();
                ViewBag.UserTeams = from user in db.Users
                                    orderby user.LastName
                                    select user;
                return View(team);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Team t)
        {
            if (ModelState.IsValid)
            {
                db.Teams.Add(t);
                db.SaveChanges();
                TempData["message"] = "Echipa a fost adaugata";
                return RedirectToAction("Index");
            }

            else
            {
                return View(t);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Team team = db.Teams.Find(id);
            return View(team);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Team requestTeam)
        {
            Team team = db.Teams.Find(id);

            if (ModelState.IsValid)
            {

                team.TeamName = requestTeam.TeamName;
                db.SaveChanges();
                TempData["message"] = "Ai modificat echipa!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestTeam);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Team category = db.Teams.Find(id);
            db.Teams.Remove(category);
            TempData["message"] = "Echipa a fost stearsa";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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