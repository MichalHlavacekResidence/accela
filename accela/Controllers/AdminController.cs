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

        public IActionResult Messages()
        {
            Database db = new Database();
            ViewBag.MessageList = db.GetAllMessages();
            return View();
        }

        public IActionResult Managers(){
            Database db = new Database();
            ViewBag.ManagerList = db.GetAllManagers();
            return View();
        }

        [HttpGet]
        public IActionResult Manager(int mid)
        {
            Database db = new Database();
            Manager mng = db.GetManagerByID(mid);
            if(mng.ID == 0)
            {
                return RedirectToAction("Managers", "Admin");
            }
            
            return View(mng);
        }

        public IActionResult Departments(){
            Database db = new Database();
            ViewBag.DepartmentList = db.GetAllDepartments();
            return View();
        }

        [HttpGet]
        public IActionResult Department(int did)
        {
            Database db = new Database();
            Department dep = db.GetDepartmentDetails(did);
            if(dep.ID == 0){
                return RedirectToAction("Departments", "Admin");
            }
            //Načti manažery
            dep.LoadManagers();

            return View(dep);
        }

        [HttpGet]
        public IActionResult AddDepartment(int did = 0){
            Department dep = new Department();
            if(did != 0)
            { 
                Database db = new Database();
                dep = db.GetDepartmentDetails(did);
            }
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
            if(dep.ID != 0)
            {
                db.UpdateDepartment(dep);
            }else{
                db.AddDepartment(dep);
            }
            return RedirectToAction("Departments", "Admin");
        }

        [HttpGet]
        public IActionResult AddManager(int uid = 0){
            Database db = new Database();
            Manager mng = new Manager();
            if(uid != 0){
                mng = db.GetManagerByID(uid);
            }
            ViewBag.DepartmentList = db.GetVisibleDepartments();
            return View(mng);
        }

        [HttpPost]
        public IActionResult AddManager(Manager manager, IFormFile ImageFile){
            //Zkontroluj, jestli jsou všechny mandatory údaje opravdu vyplněné
            if(manager.CheckDetails() == false)
            {
                return View();
            }
            string FileName = null;
            //Nahraj obrázek, pokud je
            if(ImageFile != null)
            {
                //ZKontroluj koncovky souboru (obrázku)
                string imageExtension = Path.GetExtension(ImageFile.FileName).ToLower();
                switch(imageExtension){
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        Console.WriteLine("koncovka: "+imageExtension);
                        FileName = this._uploadFile(ImageFile);
                        manager.ImageRelativeURL = FileName;
                        break;

                    //Neznámá koncovka obrázku
                    default:
                        return RedirectToAction("Managers", "Admin");

                }
            }
            

            //Připoj se k databázi a proveď požadovanou operaci
            Database db = new Database();
            if(manager.ID != 0)
            {
                db.UpdateManager(manager);
            }else{
                db.AddManager(manager);
            }
            return RedirectToAction("Managers", "Admin");
        }

        public IActionResult Brands(){
            Database db = new Database();
            ViewBag.BrandList = db.GetAllBrands();
            return View();
        }

        [HttpGet]
        public IActionResult AddBrand(int ID){
            Brand brand = new Brand();
            Database db = new Database();
            if(ID != 0)
            {
                brand = db.GetBrandByID(ID);
            }
            ViewBag.ManagerList = db.GetVisibleManagers();
            ViewBag.ProductList = db.GetVisibleProducts();
            return View(brand);
        }

        [HttpPost]
        public IActionResult AddBrand(Brand brand, IFormFile ImageFile){
            brand.GenerateUrl();
            if(brand.CheckDetails() == false){
                return RedirectToAction("AddBrand", "Admin");
            }

            string FileName = null;

            if(ImageFile != null)
            {
                //ZKontroluj koncovky souboru (obrázku)
                string imageExtension = Path.GetExtension(ImageFile.FileName).ToLower();
                switch(imageExtension){
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        Console.WriteLine("koncovka: "+imageExtension);
                        FileName = this._uploadFile(ImageFile);
                        brand.ImageRelativeURL = FileName;
                        break;

                    //Neznámá koncovka obrázku
                    default:
                        return RedirectToAction("Managers", "Admin");

                }
            }

            Database db = new Database();
            if(brand.ID != 0)
            {            
                db.UpdateBrand(brand);
            }else
            {
                db.AddBrand(brand);
            }
            return RedirectToAction("Brands", "Admin");
        }

        [HttpGet]
        public IActionResult Brand(int bid = 0)
        {
            if(bid == 0)
            {
                return RedirectToAction("Brands", "Admin");
            }
            Database db = new Database();
            Brand brand = db.GetBrandByID(bid);
            if(brand.ID == 0)
            {
                //Neexistující brand
                return RedirectToAction("Brands", "Admin");
            }

            brand.LoadProducts();
            return View(brand);
        }

        public IActionResult Products() {
            Database db = new Database();
            ViewBag.ProductList = db.GetAllProducts();
            return View();
        }

        public IActionResult News()
        {
            Database db = new Database();
            ViewBag.NewsList = db.GetAllNews();
            return View();
        }

        [HttpGet]
        public IActionResult New(int bid)
        {
            Database db = new Database();
            return View(db.GetNew(bid));
        }

        public IActionResult AddNews()
        {
            return View(new News());
        }

        public IActionResult AddProduct() {
            Database db = new Database();
            ViewBag.ProducerList = db.GetAllBrands();
            ViewBag.ManagerList = db.GetAllManagers();
            ViewBag.CategoryList = db.GetAllCategories();
            Product prod = new Product();
            return View(prod);
        }

        //public IActionResult AddProduct(Product product, )

        public IActionResult Categories()
        {
            Database db = new Database();
            ViewBag.CategoryList = db.GetAllCategories();
            return View();
        }

        public IActionResult AddCategory()
        {
            Database db = new Database();
            ViewBag.PoolList = db.GetVisiblePools();
            ViewBag.ManagerList = db.GetVisibleManagers();
            Category cg = new Category();
            return View(cg);
        }

        [HttpPost]
        public IActionResult AddCategory(Category cat)
        {
            //Vygeneruj URL
            cat.GenerateUrl();
            if(cat.CheckDetails() == false)
            {
                return View(cat);
            }
            
            Database db = new Database();
            SystemMessage smg = db.AddCategory(cat);
            return RedirectToAction("Categories", "Admin");
        }

        public IActionResult Pools()
        {
            Database db = new Database();
            ViewBag.PoolList = db.GetAllPools();
            return View();
        }

        public IActionResult AddPool()
        {
            Database db = new Database();
            ViewBag.ManagerList = db.GetVisibleManagers();
            return View(new Category());
        }

        [HttpPost]
        public IActionResult AddPool(Category pool)
        {
            pool.GenerateUrl();
            if(pool.CheckDetails() == false)
            {
                return View(pool);
            }

            Database db = new Database();
            db.AddPool(pool);
            return RedirectToAction("Pools", "Admin");
        }

        [HttpPost]
        public IActionResult AddProduct(Product product){
            //Generate URL
            product.GenerateUrl();

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

        /*
            #Funkce pro nahrání souboru na server (obsahuje i kontrolu provedení)
        */
        private string _uploadFile(IFormFile file)
        {
            Uploader uploader = new Uploader();
            //Generate file name
            string FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            Console.WriteLine("Vygenerováno jméno souboru");
            string SavePath = null;
            string Folder = null;

            Console.WriteLine("koncovka2: "+Path.GetExtension(file.FileName));

            //Generate url to save
            switch(Path.GetExtension(file.FileName))
            {
                case ".pdf":
                    SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/uploaded/pdfs/", FileName);
                    Folder = "pdfs";
                break;

                case ".jpg":
                case ".png":
                case ".gif":
                case ".jpeg":
                    SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/uploaded/images/", FileName);
                    Folder = "images";
                break;

                default: 
                    SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/uploaded/unknown/", FileName);
                    Folder = "unknown";
                break;
            }

            uploader.FilePath = SavePath;
            Console.WriteLine("Příprava na přesun");
            
            try{
                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("Přesun kompletní");

            //Check status of upload
            bool result = uploader.CheckFileExistence(FileName, Folder);
            return FileName;
        }

        /*
            #Funkce pro kontrolu existence složek na straně serveru (složky, kam se budou nahrávat soubory)
        */
        private bool _verifyUploadFolderExistence(string finalFolder)
        {
            finalFolder = finalFolder.ToLower();
            if(finalFolder != "images" || finalFolder != "pdfs"){
                return false;
            }

            string path = "wwwroot/file/uploaded/"+finalFolder;
            if(Directory.Exists(path) == true)
            {
                return true;
            }
            else
            {
                Directory.CreateDirectory(path);
                return true;
            }
        }

    }
}