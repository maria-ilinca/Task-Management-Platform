using Humanizer;
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

        public TasksController(ApplicationDbContext context)
        {
            db = context;
        }

   
        public IActionResult Index()
        {
            var tasks = db.Tasks;
            ViewBag.Tasks = tasks;

            if( TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        public IActionResult Show(int id)
        {
            Task task = db.Tasks.Include("Status").Include("Comments")
                .Where(tsk => tsk.TaskId == id)
                .First();
            return View(task);
        }

        public IActionResult New()
        {
            Task task = new Task();

            return View(task);
        }

        [HttpPost]
        public IActionResult New(Task task)
        {
            task.DataStart = DateTime.Now;
            if(ModelState.IsValid)
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

    }
}
