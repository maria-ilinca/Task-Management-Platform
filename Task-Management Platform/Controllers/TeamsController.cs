using Task_Management_Platform.Data;
using Task_Management_Platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;


namespace Task_Management_Platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext context)
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

        public ActionResult Show(int id)
        {
            Team category = db.Teams.Find(id);
            return View(category);
        }

        public ActionResult New()
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