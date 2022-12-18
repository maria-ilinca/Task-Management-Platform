using Microsoft.AspNetCore.Mvc;

namespace Task_Management_Platform.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
