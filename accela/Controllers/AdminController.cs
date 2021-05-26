using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accela.Models;
using accela.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Controllers
{
    
    [Authorize]
    public class AdminController : Controller 
    {
       // private readonly SignInManager<IdentityUser> _signInManager;

        public AdminController(/*SignInManager<IdentityUser> signInManager*/)
        {
        //    _signInManager = signInManager;
        }
    
        public IActionResult Index(){
            return View();
        }

        public IActionResult Managers(){
            Database db = new Database();
            ViewBag.ManagerList = db.GetManagers();
            return View();
        }
    }
}