using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accela.Models;
using accela.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using accela.Extensions;

namespace accela.Controllers
{
    public class EmailController : Controller
    {

        public EmailController()
        {
            
        }

        [HttpPost]
        public SystemMessage RecieveMessage(string Name, string Email, string Text, string FormType)
        {
            Database db = new Database();
            SystemMessage smc = db.SaveCustomerMessage(Name, Email, Text, FormType);
            Console.WriteLine("Ahoj, spustil jsi mě přes aJax!");
            return smc;
        }

        public IActionResult SendEmail()
        {
            return View();
        }
    }
}