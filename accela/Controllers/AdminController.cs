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


using MailChimp.Net.Interfaces;
using MailChimp.Net;
using MailChimp.Net.Core;
using System.Net.Mime;

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

        public IActionResult Index() {

            Database database = new Database();
            return View();
        }

        public IActionResult Messages()
        {
            Database db = new Database();
            ViewBag.MessageList = db.GetAllMessages();
            return View();
        }

        public IActionResult Managers() {
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
            if (mng.ID == 0)
            {
                return RedirectToAction("Managers", "Admin");
            }

            return View(mng);
        }

        public IActionResult Departments() {
            Database db = new Database();
            ViewBag.DepartmentList = db.GetAllDepartments();
            return View();
        }

        [HttpGet]
        public IActionResult Department(int did)
        {
            Database db = new Database();
            Department dep = db.GetDepartmentDetails(did);
            if (dep.ID == 0) {
                return RedirectToAction("Departments", "Admin");
            }
            //Načti manažery
            dep.LoadManagers();

            return View(dep);
        }

        [HttpGet]
        public IActionResult AddDepartment(int did = 0) {
            Department dep = new Department();
            if (did != 0)
            {
                Database db = new Database();
                dep = db.GetDepartmentDetails(did);
            }
            return View(dep);
        }

        [HttpPost]
        public IActionResult AddDepartment(Department dep) {
            //Generate URL from Name
            dep.GenerateUrl();
            //Check if all form inputs are filled
            if (dep.CheckDetails() == false) {
                return View();
            }

            Database db = new Database();
            if (dep.ID != 0)
            {
                db.UpdateDepartment(dep);
            } else {
                db.AddDepartment(dep);
            }
            return RedirectToAction("Departments", "Admin");
        }

        [HttpGet]
        public IActionResult AddManager(int uid = 0) {
            Database db = new Database();
            Manager mng = new Manager();
            if (uid != 0) {
                mng = db.GetManagerByID(uid);
            }
            ViewBag.DepartmentList = db.GetVisibleDepartments();
            return View(mng);
        }

        [HttpPost]
        public IActionResult AddManager(Manager manager, IFormFile ImageFile) {
            //Zkontroluj, jestli jsou všechny mandatory údaje opravdu vyplněné
            if (manager.CheckDetails() == false)
            {
                return View();
            }
            string FileName = null;
            //Nahraj obrázek, pokud je
            if (ImageFile != null)
            {
                //ZKontroluj koncovky souboru (obrázku)
                string imageExtension = Path.GetExtension(ImageFile.FileName).ToLower();
                switch (imageExtension) {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        Console.WriteLine("koncovka: " + imageExtension);
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
            if (manager.ID != 0)
            {
                db.UpdateManager(manager);
            } else {
                db.AddManager(manager);
            }
            return RedirectToAction("Managers", "Admin");
        }

        public IActionResult Brands() {
            Database db = new Database();
            ViewBag.BrandList = db.GetAllBrands();
            return View();
        }

        [HttpGet]
        public IActionResult AddBrand(int ID) {
            Brand brand = new Brand();
            Database db = new Database();
            if (ID != 0)
            {
                brand = db.GetBrandByID(ID);
            }
            ViewBag.ManagerList = db.GetVisibleManagers();
            ViewBag.ProductList = db.GetVisibleProducts();

            return View(brand);
        }

        [HttpPost]
        public IActionResult AddBrand(Brand brand, IFormFile ImageFile) {
            brand.GenerateUrl();
            if (brand.CheckDetails() == false) {
                return RedirectToAction("AddBrand", "Admin");
            }

            string FileName = null;

            if (ImageFile != null)
            {
                //ZKontroluj koncovky souboru (obrázku)
                string imageExtension = Path.GetExtension(ImageFile.FileName).ToLower();
                switch (imageExtension) {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        Console.WriteLine("koncovka: " + imageExtension);
                        FileName = this._uploadFile(ImageFile);
                        brand.ImageRelativeURL = FileName;
                        break;

                    //Neznámá koncovka obrázku
                    default:
                        return RedirectToAction("Managers", "Admin");

                }
            }

            Database db = new Database();
            if (brand.ID != 0)
            {
                db.UpdateBrand(brand);
            } else
            {
                db.AddBrand(brand);
            }
            return RedirectToAction("Brands", "Admin");
        }

        [HttpGet]
        public IActionResult Brand(int bid = 0)
        {
            if (bid == 0)
            {
                return RedirectToAction("Brands", "Admin");
            }
            Database db = new Database();
            Brand brand = db.GetBrandByID(bid);
            if (brand.ID == 0)
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
        public IActionResult AddNew(News news, IFormFile ImageFileBig, IFormFile ImageFile) {
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
            if (ImageFileBig != null)
            {
                //ZKontroluj koncovky souboru (obrázku)
                string imageExtension = Path.GetExtension(ImageFileBig.FileName).ToLower();
                switch (imageExtension) {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        Console.WriteLine("koncovka: " + imageExtension);
                        FileBigName = this._uploadFile(ImageFileBig, "newImageBig", news.URL);
                        news.ImageBig = FileBigName;
                        break;

                    //Neznámá koncovka obrázku
                    default:
                        return RedirectToAction("Managers", "Admin");

                }
            }
            if (ImageFile != null)
            {
                //ZKontroluj koncovky souboru (obrázku)
                string imageExtension = Path.GetExtension(ImageFile.FileName).ToLower();
                switch (imageExtension) {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        Console.WriteLine("koncovka: " + imageExtension);
                        FileBigName = this._uploadFile(ImageFile, "newImage", news.URL);
                        news.ImageSmall = FileName;
                        break;

                    //Neznámá koncovka obrázku
                    default:
                        return RedirectToAction("Managers", "Admin");

                }
            }

            Database db = new Database();
            if (news.ID != 0)
            {
                db.UpdateNew(news);
                //db.UpdateBrand(brand);
            } else
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
            if (cat.CheckDetails() == false)
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
            if (pool.CheckDetails() == false)
            {
                return View(pool);
            }

            Database db = new Database();
            db.AddPool(pool);
            return RedirectToAction("Pools", "Admin");
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
        public IActionResult Product(int prodID) {
            Database db = new Database();
            Product prod = db.GetProduct(prodID);
            return View(prod);
        }

        /*
            #Funkce pro nahrání souboru na server (obsahuje i kontrolu provedení)
        */
        private string _uploadFile(IFormFile file, string imageDirection = null, string url = null)
        {
            Uploader uploader = new Uploader();
            //Generate file name
            string FileName = url + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            Console.WriteLine("Vygenerováno jméno souboru");
            string SavePath = null;
            string Folder = null;

            Console.WriteLine("koncovka2: " + Path.GetExtension(file.FileName));

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

            try {
                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("Přesun kompletní");

            //Check status of upload
            bool result = uploader.CheckFileExistence(FileName, Folder);
            return FileName;
        }
        //public IActionResult EmailPage()
        public async Task<IActionResult> EmailPage()
        {
            Mails mail = new Mails();
            Database db = new Database();
            /*Mailchimp test not work*/

            //string apiKey = "ce5878845d727a0e27f746736b88a667-us11";
            string apiKey = "3434759dd813342dbeafdead8330162e-us11";

            //IMailChimpManager manager = new MailChimpManager(apiKey);

            IMailChimpManager mailChimpManager = new MailChimpManager(apiKey);

            //var mailChimpListCollection = mailChimpManager.Lists.GetAllAsync().ConfigureAwait(false);
            var mailChimpListCollection = await mailChimpManager.Lists.GetAllAsync(new ListRequest { Limit = 5 }).ConfigureAwait(false);
            var template = await mailChimpManager.Templates.GetAllAsync().ConfigureAwait(false);
            
            var segment = await mailChimpManager.ListSegments.GetAllAsync("51c94162ad").ConfigureAwait(false);
            var campains = await mailChimpManager.Campaigns.GetAllAsync(new CampaignRequest { Limit = 10 }).ConfigureAwait(false);
            var contact = await mailChimpManager.Members.GetAllAsync("51c94162ad", new MemberRequest { Limit = 10 }).ConfigureAwait(false);
            var oneMember = await mailChimpManager.Members.GetAsync("51c94162ad", "web@residencev.com").ConfigureAwait(false);//me
            DateTime now = DateTime.Now;



            var campainsDate = await mailChimpManager.Campaigns.GetAllAsync(new CampaignRequest { Limit = 1000, BeforeCreateTime = now}).ConfigureAwait(false);

            string[] kraken = { "aaaa", "test" };
            // var campaignTest = mailChimpManager.Campaigns.TestAsync("263e98012b"); 
            //var testMail = mailChimpManager.Campaigns.TestAsync("263e98012b");
        
     
            
            
             //create campaign
            var addCampain = await mailChimpManager.Campaigns.AddAsync(new MailChimp.Net.Models.Campaign
            {
                Type = CampaignType.Plaintext,
                
                //ContentType = "template",
                Settings = new MailChimp.Net.Models.Setting
                {
                    Title = "test 13.12",
                    ReplyTo = "web@residencev.com",
                    SubjectLine = "Do not reply, test mail",
                    TemplateId = 193325,
                    
                     

                },
                Recipients = new MailChimp.Net.Models.Recipient
                {
                    ListId = "d37275c722"
                },

            }).ConfigureAwait(false);
               Console.WriteLine(addCampain.Id);
               Console.WriteLine(addCampain.Status);
            //end add campaign
            
           // var SendetCampaign = mailChimpManager.Campaigns.SendAsync(cpID);

            //await mailChimpManager.Campaigns.SendAsync("aa").ConfigureAwait(false);


             // creating member
            /* MailChimp.Net.Models.Member member = new MailChimp.Net.Models.Member()
            {
                EmailAddress = "web@residencev.com",
                 ListId = "51c94162ad"
            };
            var responce = await mailChimpManager.Members.AddOrUpdateAsync("51c94162ad", member).ConfigureAwait(false);*/

            /*var listId = "51c94162ad"; // contect List ID
            var aa = await mailChimpManager.Members.GetAllAsync(listId).ConfigureAwait(false);*/
            ViewBag.message = mailChimpListCollection;
            ViewBag.template = template;
            ViewBag.segment = segment;
            ViewBag.campains = campains;
            ViewBag.contact = contact;
            ViewBag.oneMember = oneMember;
            ViewBag.kk = campainsDate;
            
 
            List<EmailUsers> emailUser = db.GetAllEmailUsers();
            //ViewBag.message = "aa";
            ViewBag.emailUsers = emailUser;
            return View(mail);
        }
       
        /*funkce pro posílání emailu SendMail*/
        [HttpPost]
        public IActionResult EmailPage(Mails mailInter)
        {
            Mails mail = new Mails();
            Database db = new Database();

            string path = "./wwwroot/file/emails/htmlpage.html";
            string pathBig = "./wwwroot/file/testEmailHtml/big.html";
            //Server.MapPath(Url.Content("/~wwwroot/file/emails"));
            string readText = "";
             if (System.IO.File.Exists(pathBig))
             {
                 
                 readText = System.IO.File.ReadAllText(pathBig);
                //Console.WriteLine(readText);
                Console.WriteLine("subor jede");
            }
             else
             {
                Console.WriteLine("ne subor nejede");
                readText = mailInter.Content;
                return View();
             }


            //File.ReadAllText(path);

            /*neFunkcni mail neni odepreli mi opravneni sendinblue*/
            /*
            MailMessage msg = new MailMessage();
            System.Net.Mail.SmtpClient client = this._createSmtpConection();
            try
            {
               
                msg.Subject = "Add Subject";
                msg.Body = "Animal reserch" + mailInter.Content + readText;
                msg.From = new MailAddress("web@residencev.com");
               
                List<EmailUsers> users = db.GetEmailUsersByName("Test");
                foreach (EmailUsers usr in users)
                {
                    Console.WriteLine(usr.Email);
                    msg.To.Add(usr.Email);
                    msg.IsBodyHtml = true;
                    //db.EmailSend(usr.ID);
                   
                }
                client.Send(msg);

                Console.WriteLine("Email sendet");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}", ex.ToString());
            }*/
            /*konec neFunkcni mail dendinblue*/
           
            List<EmailUsers> emailUser = db.GetAllEmailUsers();
            ViewBag.message = "aa";
            ViewBag.emailUsers = emailUser;
            return View(mail);

        }
        public IActionResult TestPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TestPage(IEnumerable<IFormFile> myFiles)
        {
            Console.WriteLine("TestPage");
            foreach (var file in myFiles)
            {
                if (file != null && file.Length > 0)
                {
                    Console.WriteLine(file.FileName );
                    Console.WriteLine(file.Length);
                }
            }
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
        private bool _generateEmailHtmlFile()
        {
            //bude mit id(emailnovinky)_url(emailnovinky)
            string path = "./wwwroot/file/emails/htmlpage.html";
            string templateHead = System.IO.File.ReadAllText("./wwwroot/file/emailTemplate/emailHead.html");
            string templateFooter = System.IO.File.ReadAllText("./wwwroot/file/emailTemplate/emailFooter.html");
            //Server.MapPath(Url.Content("/~wwwroot/file/emails"));
            if (System.IO.File.Exists(path))
            {
                Console.WriteLine("subor jede" + path);
                string readText = System.IO.File.ReadAllText(path);
                //Console.WriteLine(readText);
            }
            else
            {
                //System.IO.File.Copy(templatePath,path);
                Console.WriteLine("ne subor nejede a byl vytvorenej" + path);
                string contentText = templateHead + templateFooter;
                System.IO.File.WriteAllText(path, contentText, Encoding.UTF8);
            }
            string appendText = "This is extra text" + Environment.NewLine;
            System.IO.File.AppendAllText(path, appendText, Encoding.UTF8);

            return true;
        }
        private SmtpClient _createSmtpConection()
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "smtp-relay.sendinblue.com";
            System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("web@residencev.com", "xsmtpsib-9f8e3626727b54d3e64b0e190dcb798d30cf6ee7542543b1ff26e3356fc93546-8tWrbYPpNOvmVQny");
            
            client.Port = int.Parse("587");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicauthenticationinfo;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            return client;
        }

    }
}