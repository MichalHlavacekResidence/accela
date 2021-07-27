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
    public class BrandController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route ("/Brand/{brandUrl}")]
        public IActionResult Detail( string brandUrl)
        {
            Database database = new Database();
            ViewBag.BrandList = database.GetBrandByUrl(brandUrl);
            ViewBag.ProductList = database.GetProductsForBrand(ViewBag.BrandList.ID);
            //ViewBag.ManagerList = database.GetVisibleManagers();
            //ViewBag.Message = brandUrl;
            return View();

        }
    }
}