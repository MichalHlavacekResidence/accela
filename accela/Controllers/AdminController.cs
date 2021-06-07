using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accela.Models;
using accela.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using accela.Extensions;


namespace Controllers
{
    [Authorize]
    [ViewLayout("_AdminLayout")]
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
            ViewBag.ManagerList = db.GetAllManagers();
            return View();
        }

        public IActionResult Departments(){
            Database db = new Database();
            ViewBag.DepartmentList = db.GetAllDepartments();
            return View();
        }

        public IActionResult AddDepartment(){
            Department dep = new Department();
            return View(dep);
        }

        [HttpPost]
        public IActionResult AddDepartment(Department dep){
            //Generate URL from Name
            dep.GenerateUrl();
            //Check if all form inputs are filled
            if(dep.CheckDetails() == false){
                return View();
            }

            Database db = new Database();
            db.AddDepartment(dep);
            return RedirectToAction("Departments", "Admin");
        }

        public IActionResult AddManager(){
            Manager mng = new Manager();
            Database db = new Database();
            ViewBag.DepartmentList = db.GetVisibleDepartments();
            return View(mng);
        }

        [HttpPost]
        public IActionResult AddManager(Manager manager){
            Database db = new Database();
            if(manager.CheckDetails() == false)
            {
                return View();
            }
            //db.AddManager(manager);
            return RedirectToAction("Managers", "Admin");
        }

        public IActionResult Brands(){
            Database db = new Database();
            ViewBag.BrandList = db.GetAllBrands();
            return View();
        }

        public IActionResult AddBrand(){
            Brand brand = new Brand();
            Database db = new Database();
            ViewBag.ManagerList = db.GetAllManagers();
            return View(brand);
        }

        [HttpPost]
        public IActionResult AddBrand(Brand brand){
            brand.GenerateUrl();
            if(brand.CheckDetails() == false){
                return View(brand);
            }
            Database db = new Database();
            db.AddBrand(brand);
            return View(brand);
        }

        public IActionResult Products() {
            Database db = new Database();
            ViewBag.ProductList = db.GetAllProducts();
            return View();
        }

        public IActionResult AddProduct() {
            Database db = new Database();
            ViewBag.ProducerList = db.GetAllBrands();
            Product prod = new Product();
            return View(prod);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product){
            if(product.CheckDetails(false) == false){
                return View(product);
            }           
            Database db = new Database();
            SystemMessage smg = db.AddProduct(product);
            return RedirectToAction("Products", "Admin");
        }

        [HttpGet]
        public IActionResult Product(int prodID){
            Database db = new Database();
            Product prod = db.GetProduct(prodID);
            return View(prod);
        }
    }
}