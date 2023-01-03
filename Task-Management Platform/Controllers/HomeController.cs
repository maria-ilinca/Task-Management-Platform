using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Task_Management_Platform.Models;

namespace Task_Management_Platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;
            }

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareUser = true;
            }
           

            if(User.IsInRole("Organizer"))
            {
                ViewBag.AfisareOrganizer = true;
            }

            return View();


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}