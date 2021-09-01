using accela.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using accela.Data;
using System.Text.Json;
using Newtonsoft.Json;

namespace accela.Controllers
{
    public class CustomController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public CustomController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public PartialViewResult Menu()
        {
            //var ChargeTypes = db.ChargeTypes.ToList();
            Product prod = new Product();
            prod.Name = "kakakak";
            return PartialView(prod);
        }


        public ActionResult RenderMenu()
        {
            Database database = new Database();
            List<Product> prod = new List<Product>();
            //prod.Name = "kak";
            prod = database.GetVisibleProducts();

            return View(prod);
        }
        public ActionResult RenderMenu(string name)
        {
            Database database = new Database();
            List<Product> prod = new List<Product>();
            //prod.Name = "kak";
            prod = database.GetVisibleProducts();
            string dd = "<p>dasdasd</p>";
            //return PartialView("_Layout", prod);
            return View();
        }

    }
}