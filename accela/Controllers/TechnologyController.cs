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
        [Route ("/Technology/{poolUrl}/{technologyUrl}")]
        public IActionResult Detail(string poolUrl, string technologyUrl)
        {
            //ViewBag.message = technologyUrl;
            Database database = new Database();
            Category categ = database.GetCategoryByUrl(technologyUrl);

            return View(categ);

        }
        [Route("/Technology/{poolUrl}")]
        public IActionResult DetailPool(string poolUrl)
        {
            //ViewBag.message = technologyUrl;
            Database database = new Database();
            //Category categ = database.GetCategoryByUrl(technologyUrl);
            Category categ = database.GetPoolByUrl(poolUrl);
            
            List<string> CategoryIds = new List<string>();
            foreach(Category cat in categ.PoolCategories)
            {
                CategoryIds.Add(cat.ID.ToString());
            }
            string arrayCategoriesIDS = string.Join(",", CategoryIds);
            Console.WriteLine(arrayCategoriesIDS);

           
            ViewBag.PoolList = database.GetVisiblePools();


            //get 3 random products
            List<Product> prods = database.GetRandomVisibleProductsByCategorysID(arrayCategoriesIDS);
            //ViewBag.SelectedProduct = prods;
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
            ViewBag.SelectedProduct = selectedProds;
            //end 3 random Products
            List<string> ProducersIds = new List<string>();
            foreach (Product prod in prods)
            {
                ProducersIds.Add(prod.Producer.ID.ToString());
            }
            string arrayProducersIDS = string.Join(",", ProducersIds);
            
            ViewBag.BrandList = database.GetVisibleBrandsByIDS(arrayProducersIDS);

            return View(categ);

        }
    }
}