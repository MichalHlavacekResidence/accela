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
using System.Net.Mail;

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

            Database database = new Database();
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

        [Route("/Admin/Manager/{mid}")]
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
        [HttpGet]      
        public IActionResult AddProduct(int pid) {
            string checkID;
            if (pid == null)
            {
                checkID = "AddProduct";
            }
            else
            {
                checkID = "EditProduct";
            }
            ViewBag.Pid = checkID;
            Database db = new Database();
            ViewBag.ProducerList = db.GetAllBrands();
            ViewBag.ManagerList = db.GetAllManagers();
            ViewBag.CategoryList = db.GetAllCategories();
            Product prod = new Product();
            prod = db.GetProduct(pid);
            return View(prod);
        }

         [HttpGet]      
        public IActionResult AddNew(int nid) {
               
            Database db = new Database();
            ViewBag.ManagerList = db.GetVisibleManagers();
            ViewBag.BrandList = db.GetAllBrands();
            News news = new News();
            news = db.GetNew(nid);
            Console.WriteLine(nid);
            return View(news);
        }
         [HttpPost]
        public IActionResult AddNew(News news, IFormFile ImageFileBig, IFormFile ImageFile){
            //Zkontroluj, jestli jsou všechny mandatory údaje opravdu vyplněné
            /* if(manager.CheckDetails() == false)
             {
                 return View();
             }*/
            news.GenerateUrl();
            news.Created = DateTime.Today;
            string FileName = null;
            string FileBigName = null;
            //Nahraj obrázek, pokud je
            if(ImageFileBig != null)
            {
                //ZKontroluj koncovky souboru (obrázku)
                string imageExtension = Path.GetExtension(ImageFileBig.FileName).ToLower();
                switch(imageExtension){
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        Console.WriteLine("koncovka: "+imageExtension);
                        FileBigName = this._uploadFile(ImageFileBig, "newImageBig", news.URL);
                        news.ImageBig = FileBigName;
                        break;

                    //Neznámá koncovka obrázku
                    default:
                        return RedirectToAction("Managers", "Admin");

                }
            }
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
                        FileBigName = this._uploadFile(ImageFile, "newImage", news.URL);
                        news.ImageSmall = FileName;
                        break;

                    //Neznámá koncovka obrázku
                    default:
                        return RedirectToAction("Managers", "Admin");

                }
            }

            Database db = new Database();
             if(news.ID != 0)
            {           
                db.UpdateNew(news);
                //db.UpdateBrand(brand);
            }else
            {
                db.AddNew(news);
                //db.AddBrand(brand);
            }
           
            return RedirectToAction("News", "Admin");
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
        public IActionResult AddProductMultiImage()
        {
            Mails mail = new Mails();
            return View(mail);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product, IFormFile[] proImage, IFormFile ImageFile)
        {


            Console.WriteLine(proImage + "multiImage");
            Console.WriteLine(ImageFile + "oneImage");
            /*if (proImage != null)
            {
                Console.WriteLine("hafinky null");
            }
            else
            {
                Console.WriteLine("haf ne");
            }
                

            string FileName = null;
            if (proImage != null)
            {
                //ZKontroluj koncovky souboru (obrázku)
                string imageExtension = Path.GetExtension(proImage.FileName).ToLower();
                switch (imageExtension)
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        Console.WriteLine("koncovka: " + imageExtension);
                        //FileName = this._uploadFile(ImageFile);
                        //manager.ImageRelativeURL = FileName;
                        break;

                    //Neznámá koncovka obrázku
                    default:
                        return RedirectToAction("Managers", "Admin");
                        //break;
                }
            }*/
            //Generate URL
            /* product.GenerateUrl();

             if(product.CheckDetails(false) == false){
                 return View(product);
             }           
             Database db = new Database();
             Console.WriteLine(product.ID);
             if (product.ID == 0)
             {
                 SystemMessage smg = db.AddProduct(product);
             }
             else
             {
                 SystemMessage smg = db.EditProduct(product);
             }*/
            //return View();
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
        private string _uploadFile(IFormFile file, string imageDirection = null,string url = null)
        {
            Uploader uploader = new Uploader();
            //Generate file name
            string FileName = url+Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            Console.WriteLine("Vygenerováno jméno souboru");
            string SavePath = null;
            string Folder = null;

            Console.WriteLine("koncovka2: "+Path.GetExtension(file.FileName));

            if (imageDirection == null) {
                //Generate url to save
                switch (Path.GetExtension(file.FileName))
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
            }
            else
            {
                switch (imageDirection)
                {
                    case "productImg":
                        SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/product/", FileName);
                        Folder = "img";
                        break;
                    case "productPdf":
                        SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/product/", FileName);
                        Folder = "pdf";
                        break;
                    case "newImage":
                        SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/", FileName);
                        Folder = "new";
                        break;
                    case "newImageBig":
                        SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/", FileName);
                        Folder = "new";
                        break;
                    default:
                        SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/uploaded/unknown/", FileName);
                        Folder = "unknown";
                        break;
                }
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
        /*funkce pro posílání emailu SendMail*/
        [HttpPost]
        public IActionResult AddProductMultiImage(Mails mailInter)
        {

            MailMessage mail = new MailMessage();
            mail.To.Add("web@residencev.com");
            //mail.To.Add("Another Email ID where you wanna send same email");
            mail.From = new MailAddress("hlavacekmichal.1@gmail.com");
            mail.Subject = "Email using Gmail";

            string Body = "Hi, this mail is to test sending mail" +
                          "using Gmail in ASP.NET" + mailInter.Content;
            mail.Body = Body;

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
            smtp.Credentials = new System.Net.NetworkCredential ("hlavacekmichal.1@gmail.com", "sparta2381998");
            //Or your Smtp Email ID and Password
            smtp.EnableSsl = true;
            
            try
            {
                //client.Send(message);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
            }


            /* string to = "jane@contoso.com";
             string from = "ben@contoso.com";
             MailMessage message = new MailMessage(from, to);
             message.Subject = "Using the new SMTP client.";
             message.Body = @"Using this new feature, you can send an email message from an application very easily.";*/

            //SmtpClient client = new SmtpClient(server);
            // Credentials are necessary if the server requires the client
            // to authenticate before it will send email on the client's behalf.
            //client.UseDefaultCredentials = true;

            /* try
             {
                 //client.Send(message);
             }
             catch (Exception ex)
             {
                 Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                     ex.ToString());
             }*/

            /*Database db = new Database();
            Product prod = db.GetProduct(prodID);*/
            /* mailInter.From = "web@residencev.com";
             mailInter.To = "hlavacekmichal.1@gmail.com";

             SmtpClient smtpClient = new SmtpClient(mailInter.From, 25);

             smtpClient.Credentials = new System.Net.NetworkCredential(mailInter.From, "Martin789*");
             // smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
             smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
             smtpClient.EnableSsl = true;
             MailMessage mail = new MailMessage();

             //Setting From , To and CC
             mail.From = new MailAddress(mailInter.From, "MyWeb Site");
             mail.To.Add(new MailAddress(mailInter.To));
             //mail.CC.Add(new MailAddress("MyEmailID@gmail.com"));

             smtpClient.Send(mail);*/
            //Console.WriteLine("mail sendedt1" + mailInter.Content + "  2  " + mailInter.From + "   3   " + mailInter.To );
            Console.WriteLine(mail.Body);
            return View();
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