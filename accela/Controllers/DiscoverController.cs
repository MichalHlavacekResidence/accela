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
            //ViewBag.Massage = discoverUrl;
            Database database = new Database();
            //ViewBag.newsList = database.GetNewByUrlWithTags(discoverUrl);
            ViewBag.NewsList = database.GetNewByUrl(discoverUrl);
            ViewBag.TagList = database.GetTagsByNews(ViewBag.NewsList.ID);
            ViewBag.NewsWithTags = database.GetNewByUrlWithTags(discoverUrl);
            return View();

        }
    }
}