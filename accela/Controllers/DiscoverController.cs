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
    public class DiscoverController : Controller
    {
        public IActionResult Index()
        {
           
            return View();
        }
        [Route ("/Discover/{discoverUrl}")]
        public IActionResult Detail( string discoverUrl)
        {
            Database database = new Database();
            ViewBag.NewsWithTags = database.GetNewByUrlDetail(discoverUrl);
            News news = new News();
            news = ViewBag.NewsWithTags;
            if (news.Title == null) {
                return Redirect("/Discover/");
            }
            else
            {
                return View();
            }

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Route("/Discover")]
        public IActionResult Page_404()
        {
            Response.StatusCode = 404;
            List<string> Messages = new List<string>();
            Messages.Add("Tento èlánek neexistuje");
            Random randomizer = new Random();
            int key = randomizer.Next(0, Messages.Count);
            ViewBag.Message = Messages[key];
            return View();
        }
    }
}