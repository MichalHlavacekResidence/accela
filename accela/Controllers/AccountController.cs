using System;
using accela.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace Controllers 
{
    public class AccountController : Controller 
    {
        public AccountController()
        {
            
        }

        public IActionResult Login(){
            User usr = new User();
            return View(usr);
        }

        [HttpPost]
        public IActionResult Login(User usr){
            //Do stuff
            return View();
        }
    }
}