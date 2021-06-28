using accela.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using accela.Data;

namespace accela.Controllers
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

            return View();
        }

        [Route("/Mailing")]
        public IActionResult Mailing()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Route("/Brand")]
        public IActionResult Brand()
        {
            return View();
        }

        public IActionResult References()
        {
            return View();
        }
        public IActionResult Technologies()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("{*url}", Order = 999)]
        public IActionResult Page_404()
        {
            Response.StatusCode = 404;
            List<string> Messages = new List<string>();
            Messages.Add("Utekl jste nám na neexistující stránku.");
            Random randomizer = new Random();
            int key = randomizer.Next(0, Messages.Count);
            ViewBag.Message = Messages[key];
            return View();
        }
    }
}
