using System;
using accela.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using accela.Data;

namespace Controllers 
{
    public class AccountController : Controller 
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signManager;
        public AccountController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signManager            
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signManager = signManager;
        }

        public IActionResult Login(){
            if(User.Identity.IsAuthenticated == true){
                return RedirectToAction("Admin", "Index");
            }
            User usr = new User();
            return View(usr);
        }

        [HttpPost]
        public async Task<IActionResult> Login(User usr){
            //Do stuff
            if((usr.Password == null || usr.Password == "") || (usr.Email == "" || usr.Email == null)){
                //Vrať chybovou hlášku
                return View(usr);
            }

            //zkontroluj a připrav role
            if(await _roleManager.RoleExistsAsync("Admin") == false){
                var adminRole = new IdentityRole();
                adminRole.Name = "Admin";
                await _roleManager.CreateAsync(adminRole);
            }

            if(await _roleManager.RoleExistsAsync("User") == false){
                var userRole = new IdentityRole();
                userRole.Name = "User";
                await _roleManager.CreateAsync(userRole);
            }

            //Připoj k uživateli roli
            var resultSettingRole = await _userManager.AddToRoleAsync(usr, "Admin");

            //proveď ověření uživatele (login funkce)

            Database db = new Database();
            User loggedUser = db.LoginUser(usr);
            
            if(loggedUser.ID == 0){
                Console.WriteLine("Chyba lognutí");
                return View(loggedUser);
            }
            
            //Přihlaš uživatele v rámci frameworku .NET
            await _signManager.SignInAsync(loggedUser, false, null);
            Console.WriteLine("Uživatel byl přihlášen");
 
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Registration(){
            User usr = new User();
            return View(usr);
        }

        [HttpPost]
        public IActionResult Registration(User usr){
            if(usr.Firstname == null || usr.Firstname == "" || usr.Lastname == null || usr.Lastname == "" || usr.Email == null || usr.Email == "" || usr.Password == null || usr.Password == ""){
                return View(usr);    
            }

            if(usr.Password != usr.Repassword){
                return View(usr);
            }
            usr.Level = "Admin";
            Database db = new Database();
            SystemMessage msg = db.CreateUser(usr);
            if( msg.Result == false){
                return View(usr);
            }
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public async Task<IActionResult> SignOut(){
            await _signManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}