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
            Database database = new Database();
            // ViewBag.BrandList = database.GetVisibleBrands();
            // ViewBag.DiscoverList = database.GetVisibleDIscover();
            /*List<Product> prods = database.GetVisibleProducts();
            Random rng = new Random();
            List<Product> selectedProds = new List<Product>();
            List<int> cisla = new List<int>();
            while (cisla.Count < 3)
            {
                int cislo = rng.Next(0, prods.Count);
                if (!cisla.Contains(cislo))
                {
                    cisla.Add(cislo);
                    selectedProds.Add(prods[cislo]);

                }
            }
            ViewBag.SelectedProduct = selectedProds;*/

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
                
        public IActionResult Brand()
        {
            Database database = new Database();
            ViewBag.BrandList = database.GetVisibleBrands();
            return View();
        }
        public IActionResult Discover()
        {
            Database database = new Database();
            // ViewBag.DiscoverList = database.GetVisibleDiscover();
            return View();
        }
        public IActionResult References()
        {
            Database database = new Database();
            ViewBag.ReferencesList = database.GetVisibleRefeences();
            return View();
        }
        public IActionResult Technologies()
        {
            Database database = new Database();
            //ViewBag.PoolList = database.GetVisiblePools();
            //ViewBag.CategoryList = database.GetVisibleCategories();

            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult TechnologiDetail()
        {
            return View();
        }
        public IActionResult DiscoverDetail()
        {
            return View();
        }
        public IActionResult ProductDetail()
        {
            return View();
        }
        public IActionResult Product()
        {
             Database database = new Database();
             ViewBag.ProductList = database.GetVisibleProducts();

            /*List<Product> ProductList = new List<Product>();
            ProductList.Add(new Product(0,"test", "test", "test", "test", "test", "imagestream_x_mkii_24.png", new Manager(), "test",true));
            ViewBag.ProductList = ProductList;*/
            return View();

        }
        public IActionResult Support()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Downloads()
        {
            return View();
        }
        public IActionResult MeetUs()
        {
            return View();
        }
        public IActionResult Career()
        {
            return View();
        }
        public IActionResult BrandDetail()
        {
            /* Database database = new Database();
             ViewBag.RelatedProductBrandList = database.GetRelatedProductBrand();*/

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