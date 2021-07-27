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
    public class TechnologyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route ("/Technology/{technologyUrl}")]
        public IActionResult Detail( string technologyUrl)
        {
            //ViewBag.message = technologyUrl;
            //Database database = new Database();
            //ViewBag.CategoryList = database.GetCategoryByUrlVol(technologyUrl);
            //ViewBag.CategoryList = database.GetCategoryByUrlVol(technologyUrl);
            return View();

        }
    }
}