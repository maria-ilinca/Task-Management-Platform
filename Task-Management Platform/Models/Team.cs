using Microsoft.AspNetCore.Mvc;

namespace Task_Management_Platform.Models
{
    public class Team : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
