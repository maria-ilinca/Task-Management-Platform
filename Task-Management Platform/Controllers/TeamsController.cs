using Task_Management_Platform.Data;
using Task_Management_Platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace Task_Management_Platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext db;

        public TeamsController(ApplicationDbContext context)
        {
            db = context;
        }
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var teams = from team in db.Teams
                             orderby team.TeamName
                             select team;
            ViewBag.Teams = teams;


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult AddProject(int id)
        {
            TempData["TeamId"] = id;
            return Redirect("/Projects/New");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddUser([FromForm] UserTeam userTeam)
        {
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

        public ActionResult Show(int id)
        {
            Team team = db.Teams.Include("Projects")
                                .Where(t => t.Id == id).First();
            ViewBag.UserTeams = from user in db.Users
                                orderby user.LastName
                                select user;
            return View(team);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
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

        public ActionResult Edit(int id)
        {
            Team team = db.Teams.Find(id);
            return View(team);
        }

        [HttpPost]
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
        public ActionResult Delete(int id)
        {
            Team category = db.Teams.Find(id);
            db.Teams.Remove(category);
            TempData["message"] = "Echipa a fost stearsa";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}