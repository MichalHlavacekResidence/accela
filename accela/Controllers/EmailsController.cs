using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accela.Models;
using accela.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using accela.Extensions;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Grpc.Core;
using accela.Models.EmailModels;


namespace Controllers
{
   
    [ViewLayout("_NoLayout")]
    public class EmailsController : Controller
    {

        public EmailsController()
        {
          
        }

        public IActionResult Index(int Message = 0) {

            if (Message == 1 || Message == 0)
            {
                ViewData["message"] = "We did not find your email.";
            }else if (Message == 2)
            {
                ViewData["message"] = "You are no longer a subscriber.";
            }
            else if (Message == 3)
            {
                ViewData["message"] = "You already are a unsubscriber.";
            }


            //Console.WriteLine(Message +"aa");
            Database database = new Database();
            return View();
        }
        [HttpGet]
        public IActionResult Unsubscribe(int? ID)
        {
            EmailUsers user = new EmailUsers();
            if (ID.GetValueOrDefault(0)==0)
            {
                return RedirectToAction("Index","Emails", new { Message = 1});
            }
            else
            {
                user.ID = ID ?? default(int);
                ViewBag.User = ID;
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult Unsubscribe(EmailUsers user)
        {
            if (user.ID != null || user.ID != 0)
            {
                Database db = new Database();
                user = db.GetAllEmailUser(user.ID);
                if (user.Status == "subscriber")
                {
                    Console.WriteLine("db unsc" + user.ID);
                    db.UnsubscriberUser(user);
                    return RedirectToAction("Index", "Emails", new { Message = 2 });
                }
                else
                {
                    Console.WriteLine("db viev unsc" + user.ID);
                    return RedirectToAction("Index", "Emails", new { Message = 3 });
                }
            }
            else
            {
                Console.WriteLine("no usr" + user.ID);
                return RedirectToAction("Index", "Emails", new { Message = 1 });
            }
        }
    }
}