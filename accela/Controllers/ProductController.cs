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
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route ("/Product/{productUrl}")]
        public IActionResult Detail( string productUrl)
        {
            //Console.WriteLine(productUrl);
            //ViewBag.message = technologyUrl;
            Database database = new Database();
            Product product = database.GetDetailProduct(productUrl); 


            //ViewBag.CategoryList = database.GetCategoryByUrlVol(technologyUrl);
            //ViewBag.CategoryList = database.GetCategoryByUrlVol(technologyUrl);
            return View(product);

        }
    }
}