using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accela.Models;
using MySqlConnector;
using accela.Data;

namespace accela.Data 
{
    public class Database
    {
        private string _resultMessage;
        private bool _isConnected;

        public Database()
        {
            if(this._verifyConnection()){
                _isConnected = true;
            }else{
                _isConnected = false;
            }
        }

        public User LoginUser(User usr){
            if(usr.Email == null || usr.Password == null){
                //Uživatel nevyplnil heslo a email
                return new User();
            }
            try{
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Firstname, Lastname, Email, Level FROM Users WHERE Password = @pswd AND Email = @em";
                        cmd.Parameters.AddWithValue("@pswd", usr.Password);
                        cmd.Parameters.AddWithValue("@em", usr.Email);
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            //Špatné přihlášení
                            return new User(0, null, null, null, null);
                        }else{
                            //OK přihlášení (uživatel byl s kombinací nalezen)
                            int id = 0;
                            string fname = null;
                            string lname = null;
                            string email = null;
                            string level = null;
                            while(reader.Read()){
                                try{ id = reader.GetInt16(0); }catch(Exception){ id = 0;}
                                try{ fname = reader.GetString(1); }catch(Exception) { fname = "Nenalezeno";}
                                try{ lname = reader.GetString(2); }catch(Exception) { lname = "Nenalezeno";}
                                try{ email = reader.GetString(3); }catch(Exception) { email = "Nenalezeno";}
                                try{ level = reader.GetString(4); }catch(Exception) { level = "Nenalezeno";} 
                            }
                            //Vrať uživatele zpět do kontrolleru
                            return new User(id, fname, lname, email, level);
                        }
                    }
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                return new User();
            }
        }

        public SystemMessage CreateUser(User usr){
            if(this.VerifyUserExistence(usr)){
                return new SystemMessage("Registrace uživatele", "Tento uživatel již existuje", "Error");
            }

            try {
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "INSERT INTO Users (Firstname, Lastname, Email, Password, Level) VALUES (@fname, @lname, @em, @pass, @lev);";
                        cmd.Parameters.AddWithValue("@fname", usr.Firstname);
                        cmd.Parameters.AddWithValue("@lname", usr.Lastname);
                        cmd.Parameters.AddWithValue("@em", usr.Email);
                        cmd.Parameters.AddWithValue("@pass", usr.Password);
                        cmd.Parameters.AddWithValue("@lev", usr.Level);
                        cmd.ExecuteNonQuery();
                    }
                }
                return new SystemMessage("Registrace uživatele", "Byl jste úspěšně registrován, nyní se můžete přihlásit", "OK");
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                return new SystemMessage("Registrace uživatele", ex.Message, "Error");
            }
        }

        public List<Manager> GetAllManagers(){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Firstname, Lastname, Email, Phone, Img, Description, Visibility, Position, DepartmentID FROM Managers";
                        var reader = cmd.ExecuteReader();

                        if(!reader.HasRows){
                            //Vrátit prázdný výsledek
                            return new List<Manager>();
                        }
                        int id;
                        string fname;
                        string lname;
                        string email;
                        string phone;
                        string img;
                        string description;
                        bool visibility;
                        int position;
                        int departmentId;
                        List<Manager> managers = new List<Manager>();
                        while(reader.Read()){
                            try{id = reader.GetInt16(0);}catch(Exception){ id = 0;}
                            try{fname = reader.GetString(1);}catch(Exception){ fname = null; }
                            try{lname = reader.GetString(2);}catch(Exception){ lname = null; }
                            try{email = reader.GetString(3);}catch(Exception){ email = null; }
                            try{phone = reader.GetString(4);}catch(Exception){ phone = null; }
                            try{img = reader.GetString(5);}catch(Exception){ img = null; }
                            try{description = reader.GetString(6);}catch(Exception){ description = null; }
                            try{visibility = reader.GetBoolean(7);}catch(Exception){ visibility = false; }
                            try{position = reader.GetInt16(8);}catch(Exception){ position = 0; }
                            try{departmentId = reader.GetInt16(9);}catch(Exception){departmentId = 0;}
                            
                            managers.Add(new Manager(id, fname, lname, email, this.GetDepartmentDetails(departmentId), phone, description, img, visibility, position));
                        }
                        return managers;
                    }
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                return new List<Manager>();
            }
        }

        public Department GetDepartmentDetails(int Id){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Visibility, Position FROM Department WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", Id);
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            //Když není žádný výsledek
                            return new Department();
                        }
                        int id = 0;
                        string name = null;
                        string url = null;
                        bool visibility = false;
                        int position = 0;
                        while(reader.Read()){
                            try{ id = reader.GetInt16(0); }catch(Exception){ id = 0; }
                            try{ name = reader.GetString(1); }catch(Exception){ name = null; }
                            try{ url = reader.GetString(2); }catch(Exception){ url = null; }
                            try{ visibility = reader.GetBoolean(3); }catch(Exception){ visibility = false; }
                            try{ position = reader.GetInt16(4);}catch(Exception){position = 0;}
                        }
                        return new Department(id, name, url, visibility, position);
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetDepartmentDetails] "+ex.Message);
                return new Department();
            }
        }

        public SystemMessage CreateManager(Manager manager){
            if(this.VerifyManagerExistence(manager) == true){
                return new SystemMessage("Přidání nového manažera", "Manažer s tímto emailem již existuje", "Error");
            }

            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "INSERT INTO Managers (Firstname, Lastname, Email, DepartmentID, Phone, Img, Description, Visibility, Position) VALUES (@fname, @lname, @em, @department, @phone, @img, @desc, @vis, @pos)";
                        cmd.Parameters.AddWithValue("@fname", manager.Firstname);
                        cmd.Parameters.AddWithValue("@lname", manager.Lastname);
                        cmd.Parameters.AddWithValue("@em", manager.Email);
                        cmd.Parameters.AddWithValue("@phone", manager.Phone);
                        cmd.Parameters.AddWithValue("@img", manager.ImageAbsoluteURL);
                        cmd.Parameters.AddWithValue("@desc", manager.Description);
                        cmd.Parameters.AddWithValue("@department", manager.Department.ID);
                        cmd.Parameters.AddWithValue("@vis", manager.Visibility);
                        cmd.Parameters.AddWithValue("@pos", manager.Position);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Přidání manažera", "Manažer byl úspěšně přidán", "OK");
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[CreateManager] "+ex.Message);
                return new SystemMessage("Přidání manažera", ex.Message, "Error");
            }
        }

        public List<Product> GetAllProducts (){
            try{
                using (var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Subtitle, Small_desc, Description, Link, Visibility, ContactID, BrandID, CategoryID, VideoURL, ReferenceLink, ManagerID FROM Products";
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            return new List<Product>();
                        }
                        int id = 0;
                        string name = null;
                        string url = null;
                        string subtitle = null;
                        string smalld = null;
                        string descr = null;
                        string link = null;
                        bool vis = false;
                        int contID = 0;
                        int bID = 0;
                        int catID = 0;
                        string vidUrl = null;
                        string referenceLink = null;
                        int managerID = 0;
                        Manager mng = new Manager();
                        List<Product> products = new List<Product>();

                        while(reader.Read()){
                            try { id = reader.GetInt16(0); }catch(Exception){ id = 0;}
                            try { name = reader.GetString(1); }catch(Exception){name = null;}
                            try { url = reader.GetString(2); }catch(Exception){url = null;}
                            try { subtitle = reader.GetString(3); }catch(Exception){ subtitle = null; }
                            try { smalld = reader.GetString(4); }catch(Exception){ smalld = null; }
                            try { descr = reader.GetString(5); }catch(Exception){ descr = null; }
                            try { link = reader.GetString(6);}catch(Exception){link = null; }
                            try { vis = reader.GetBoolean(7);}catch(Exception){vis = false; }
                            try { contID = reader.GetInt16(8);} catch(Exception){ contID = 0; }
                            try { bID = reader.GetInt16(9);}catch(Exception){ bID = 0; }
                            try { catID = reader.GetInt16(10);}catch(Exception){catID = 0;}
                            try { vidUrl = reader.GetString(11);}catch(Exception){vidUrl = null;}
                            try { referenceLink = reader.GetString(12);}catch(Exception){referenceLink = null;}
                            try { managerID = reader.GetInt16(13); }catch(Exception){ managerID = 0;}
                            if(managerID != 0){
                                mng = this.GetManagerByID(managerID);
                            }
                            products.Add(new Product(id, name, url, descr, subtitle, smalld, referenceLink, this.GetBrandByID(bID), mng, vidUrl, vis));
                        }
                        return products;
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetAllProducts] "+ex.Message);
               return new List<Product>(); 
            }
        }

        public List<Department> GetAllDepartments(){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand command = new MySqlCommand()){
                        command.Connection = db.Connection;
                        command.CommandText = "SELECT ID, Name, URL, Visibility, Position FROM Departments";
                        var reader = command.ExecuteReader();
                        if(reader.HasRows == false){
                            return new List<Department>();
                        }

                        int id = 0;
                        string name = null;
                        string url = null;
                        bool vis = false;
                        int pos = 0;
                        List<Department> deps = new List<Department>();
                        while(reader.Read()){
                            try{ id = reader.GetInt32(0);}catch(Exception){ id = 0; }
                            try{ name = reader.GetString(1);}catch(Exception){ name = null; }
                            try{ url = reader.GetString(2);}catch(Exception){ url = null; }
                            try{ vis = reader.GetBoolean(3);}catch(Exception){ vis = false; }
                            try{ pos = reader.GetInt32(4);}catch(Exception){ pos = 0; } 
                            deps.Add(new Department(id, name, url, vis, pos));
                        }
                        return deps;
                    }
                }
            }catch(Exception ex ){
                Console.WriteLine("[GetAllDepartments] "+ex.Message);
                return new List<Department>(); 
            }
        }

        public List<Department> GetVisibleDepartments(){
            try {
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Visibility, Position FROM Departments WHERE Visibility = 1";
                        var reader = cmd.ExecuteReader();
                        if(reader.HasRows == false){
                            return new List<Department>();
                        }

                        int id = 0;
                        string name = null;
                        string url = null;
                        bool visibility = false;
                        int position = 0;
                        List<Department> deps = new List<Department>();
                        while(reader.Read())
                        {
                            try{ id = reader.GetInt32(0);}catch(Exception){ id = 0; }
                            try{ name = reader.GetString(1);}catch(Exception) {name = null;}
                            try{ url = reader.GetString(2);}catch(Exception){ url = null; }
                            try{ visibility = reader.GetBoolean(3); }catch(Exception){visibility = false;}
                            try{ position = reader.GetInt32(4);}catch(Exception){ position = 0;}
                            deps.Add(new Department(id, name, url, visibility, position));
                        }
                        return deps;
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetVisibleDepartments] "+ ex.Message);
                return new List<Department>();
            }
            
        }

        public SystemMessage AddManager(Manager mng)
        {
            try{
                if(this.VerifyManagerExistence(mng))
                {
                    return new SystemMessage("Adding new Manager", "Manager with this email already exists", "error");
                }

                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "INSERT INTO Managers(Firstname, Lastname, Email, Phone, Img, Description, Visibility, Position, DepartmentID) VALUES (@fname, @lname, @email, @phone, @img, @desc, @vis, @pos, @depid)";
                        cmd.Parameters.AddWithValue("@fname", mng.Firstname);
                        cmd.Parameters.AddWithValue("@lname", mng.Lastname);
                        cmd.Parameters.AddWithValue("@email", mng.Email);
                        cmd.Parameters.AddWithValue("@phone", mng.Phone);
                        cmd.Parameters.AddWithValue("@img", mng.ImageRelativeURL);
                        cmd.Parameters.AddWithValue("@desc", mng.Description);
                        cmd.Parameters.AddWithValue("@vis", mng.Visibility);
                        cmd.Parameters.AddWithValue("@pos", mng.Position);
                        cmd.Parameters.AddWithValue("@depid", mng.Department.ID);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Adding new Manager", "Manager was successfully added", "OK");
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[AddManager] "+ex.Message);
                return new SystemMessage("Adding new Manager", "There was critical error processing this request. Check server -- app console for more informations", "Error");
            }
            
        }

        public SystemMessage AddDepartment(Department dp){
            if(this.VerifyDepartmentExistence(dp)){
                return new SystemMessage("Adding new Department", "Department with this name alredy exists.", "Error");
            }

            using(var db = new AppDb()){
                db.Connection.Open();
                using(MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = db.Connection;
                    cmd.CommandText = "INSERT INTO Departments (Name, URL, Visibility, Position) VALUES (@nam, @url, @vis, @pos)";
                    cmd.Parameters.AddWithValue("@nam", dp.Name);
                    cmd.Parameters.AddWithValue("@url", dp.URL);
                    cmd.Parameters.AddWithValue("@vis", dp.Visibility);
                    cmd.Parameters.AddWithValue("@pos", dp.Position);
                    cmd.ExecuteNonQuery();
                    return new SystemMessage("Adding new Department", "Department was successfully added", "OK");
                }
            }
        }

        public SystemMessage AddProduct(Product product){
            try {
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "INSERT INTO Products (Name, URL, Subtitle, Small_desc, Description, Visibility, ContactID, BrandID, CategoryID, VideoURL,ReferenceLink, ManagerID) VALUES (@name, @url, @sub, @smalld, @desc, @vis, @contId, @brandId, @catId, @videoUrl, @referLink, @manId)";
                        cmd.Parameters.AddWithValue("@name", product.Name);
                        cmd.Parameters.AddWithValue("@url", product.URL);
                        cmd.Parameters.AddWithValue("@sub", product.Subtitle);
                        cmd.Parameters.AddWithValue("@smalld", product.SmallDescription);
                        cmd.Parameters.AddWithValue("@desc", product.Description);
                        cmd.Parameters.AddWithValue("@vis", product.Visibility);
                        if(product.Manager.ID == 0){
                            cmd.Parameters.AddWithValue("@contId", null);
                        }else{
                            cmd.Parameters.AddWithValue("@contId", product.Manager.ID);
                        }
                        cmd.Parameters.AddWithValue("@brandId", product.Producer.ID);
                        cmd.Parameters.AddWithValue("@catId", product.Category.ID);
                        cmd.Parameters.AddWithValue("@videoUrl", product.VideoURL);
                        cmd.Parameters.AddWithValue("@referLink", product.ReferenceLink);
                        if(product.Manager.ID == 0){
                            cmd.Parameters.AddWithValue("@manId", null);
                        }else{
                            cmd.Parameters.AddWithValue("@manId", product.Manager.ID);
                        }
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Přidání nového produktu", "Produkt byl úspěšně přidán", "OK");
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[AddProduct] " + ex.Message);
                return new SystemMessage("Přidání nového produktu", ex.Message, "Error");
            }
        }

        public SystemMessage AddBrand(Brand brand){
            try {
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "INSERT INTO Brands (ContactID, Name, URL, Description, Small_desc, Link, Position, Visibility) VALUES (@contId, @name, @url, @desc, @smalld, @link, @pos, @vis)";
                        cmd.Parameters.AddWithValue("@contId", brand.Contact.ID);
                        cmd.Parameters.AddWithValue("@name", brand.Name);
                        cmd.Parameters.AddWithValue("@url", brand.URL);
                        cmd.Parameters.AddWithValue("@smalld", brand.SmallText);
                        cmd.Parameters.AddWithValue("@desc", brand.Description);
                        cmd.Parameters.AddWithValue("@vis", brand.Visibility);
                        cmd.Parameters.AddWithValue("@pos", brand.Position);
                        cmd.Parameters.AddWithValue("@referLink", brand.ReferenceLink);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Adding new brand", "Brand was succesfully added", "OK");
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[AddProduct] " + ex.Message);
                return new SystemMessage("Adding new brand", ex.Message, "Error");
            }
        }

        public List<Product> GetVisibleProducts(){
            try{
                using (var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Subtitle, Small_desc, Description, Link, Visibility, ContactID, BrandID, CategoryID, VideoURL, ReferenceLink, ManagerID FROM Products WHERE Visibility = 1 ORDER BY Position ASC";
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            return new List<Product>();
                        }
                        int id = 0;
                        string name = null;
                        string url = null;
                        string subtitle = null;
                        string smalld = null;
                        string descr = null;
                        string link = null;
                        bool vis = false;
                        int contID = 0;
                        int bID = 0;
                        int catID = 0;
                        string vidUrl = null;
                        string referenceLink = null;
                        int managerID = 0;
                        Manager mng = new Manager();
                        List<Product> products = new List<Product>();

                        while(reader.Read()){
                            try { id = reader.GetInt16(0); }catch(Exception){ id = 0;}
                            try { name = reader.GetString(1); }catch(Exception){name = null;}
                            try { url = reader.GetString(2); }catch(Exception){url = null;}
                            try { subtitle = reader.GetString(3); }catch(Exception){ subtitle = null; }
                            try { smalld = reader.GetString(4); }catch(Exception){ smalld = null; }
                            try { descr = reader.GetString(5); }catch(Exception){ descr = null; }
                            try { link = reader.GetString(6);}catch(Exception){link = null; }
                            try { vis = reader.GetBoolean(7);}catch(Exception){vis = false; }
                            try { contID = reader.GetInt16(8);} catch(Exception){ contID = 0; }
                            try { bID = reader.GetInt16(9);}catch(Exception){ bID = 0; }
                            try { catID = reader.GetInt16(10);}catch(Exception){catID = 0;}
                            try { vidUrl = reader.GetString(11);}catch(Exception){vidUrl = null;}
                            try { referenceLink = reader.GetString(12);}catch(Exception){referenceLink = null;}
                            try { managerID = reader.GetInt16(13); }catch(Exception){ managerID = 0;}
                            if(managerID != 0){
                                mng = this.GetManagerByID(managerID);
                            }
                            products.Add(new Product(id, name, url, descr, subtitle, smalld, referenceLink, this.GetBrandByID(bID), mng, vidUrl, vis));
                        }
                        return products;
                    }
                }
             }catch(Exception ex){
                Console.WriteLine("[GetVisibleProducts] "+ex.Message);
                return new List<Product>();
            }
        }

        public Product GetProduct (int productID){
            try {
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Subtitle, Small_desc, Description, Link, Visibility, ContactId, BrandID, CategoryID, VideoURL, ReferenceLink, ManagerID FROM Products WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", productID);
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            return new Product();
                        }

                        int id = 0;
                        string name = null;
                        string url = null;
                        string subtitle = null;
                        string smalld = null;
                        string descr = null;
                        string link = null;
                        bool vis = false;
                        int contID = 0;
                        int bID = 0;
                        int catID = 0;
                        string vidUrl = null;
                        string referenceLink = null;
                        int managerID = 0;
                        Manager mng = new Manager();

                        while(reader.Read()){
                            try { id = reader.GetInt16(0); }catch(Exception){ id = 0;}
                            try { name = reader.GetString(1); }catch(Exception){name = null;}
                            try { url = reader.GetString(2); }catch(Exception){url = null;}
                            try { subtitle = reader.GetString(3); }catch(Exception){ subtitle = null; }
                            try { smalld = reader.GetString(4); }catch(Exception){ smalld = null; }
                            try { descr = reader.GetString(5); }catch(Exception){ descr = null; }
                            try { link = reader.GetString(6);}catch(Exception){link = null; }
                            try { vis = reader.GetBoolean(7);}catch(Exception){vis = false; }
                            try { contID = reader.GetInt16(8);} catch(Exception){ contID = 0; }
                            try { bID = reader.GetInt16(9);}catch(Exception){ bID = 0; }
                            try { catID = reader.GetInt16(10);}catch(Exception){catID = 0;}
                            try { vidUrl = reader.GetString(11);}catch(Exception){vidUrl = null;}
                            try { referenceLink = reader.GetString(12);}catch(Exception){referenceLink = null;}
                            try { managerID = reader.GetInt16(13); }catch(Exception){ managerID = 0;}
                            if(managerID != 0){
                                mng = this.GetManagerByID(managerID);
                            }
                        }
                        return new Product(id, name, url, descr, subtitle, smalld, referenceLink, this.GetBrandByID(bID), mng, vidUrl, vis);
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetProduct] "+ex.Message);
                return new Product();
            }
        }

        public Manager GetManagerByID(int managerID){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Firstname, Lastname, Email, Phone, Img, Description, Visibility, Position, DepartmentID FROM Managers WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", managerID);
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            return new Manager();
                        }

                        int id = 0;
                        string fname = null;
                        string lname = null;
                        string email = null;
                        string phone = null;
                        string img = null;
                        string desc = null;
                        bool vis = false;
                        int pos = 0;
                        int depID = 0;

                        while(reader.Read()){
                            try { id = reader.GetInt16(0); }catch(Exception){ id = 0;}
                            try { fname = reader.GetString(1);} catch(Exception){fname = null;}
                            try { lname = reader.GetString(2);} catch(Exception){lname = null;}
                            try { email = reader.GetString(3);} catch(Exception){email = null;}
                            try { phone = reader.GetString(4);} catch(Exception){phone = null;}
                            try { img = reader.GetString(5);}catch(Exception){img = null;}
                            try { desc = reader.GetString(6);}catch(Exception){desc = null;}
                            try { vis = reader.GetBoolean(7);}catch(Exception){vis = false;}
                            try { pos = reader.GetInt16(8);}catch(Exception){pos = 0;}
                            try { depID = reader.GetInt32(9);}catch(Exception){depID = 0;}
                        }
                        Department department = new Department();
                        if(depID != 0){
                            department = this.GetDepartmentDetails(depID);
                        }
                        return new Manager(id, fname, lname, email, department, phone, desc, img, vis, pos);
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetManagerByID] "+ex.Message);
                return new Manager();
            }
        }

        public Brand GetBrandByID(int brandID){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, ContactID, Name, URL, Description, Small_desc, Link, Position, Visibility FROM Brands WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", brandID);
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            return new Brand();
                        }

                        int id = 0;
                        int contID = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        string smalld = null;
                        string link = null;
                        int pos = 0;
                        bool vis = false;
                        while(reader.Read()){
                            try { id = reader.GetInt32(0); }catch(Exception){ id = 0;}
                            try { contID = reader.GetInt32(1); }catch(Exception){ contID = 0;}
                            try { name = reader.GetString(2); }catch(Exception){ name = null; }
                            try { url = reader.GetString(3);}catch(Exception){ url = null; }
                            try { desc = reader.GetString(4);}catch(Exception) { desc = null;}
                            try { smalld = reader.GetString(5);}catch(Exception){ smalld = null; }
                            try { link = reader.GetString(6);}catch(Exception){ link = null; }
                            try { pos = reader.GetInt32(7);}catch(Exception){ pos = 0;}
                            try { vis = reader.GetBoolean(8);}catch(Exception){ vis = false;}
                        }
                        Manager contact = new Manager();
                        if(contID != 0){
                            contact = this.GetManagerByID(contID);
                        }
                        return new Brand(id, name, link ,url, desc, smalld, contact, pos, vis);
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetBrandByID] "+ex.Message);
                return new Brand();
            }
        }

        public List<Brand> GetAllBrands(){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, ContactID, Name, URL, Description, Small_desc, Link, Position, Visibility FROM Brands";
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            return new List<Brand>();
                        }

                        int id = 0;
                        int contID = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        string smalld = null;
                        string link = null;
                        int pos = 0;
                        bool vis = false;
                        List<Brand> brands = new List<Brand>();
                        while(reader.Read()){
                            try { id = reader.GetInt32(0); }catch(Exception){ id = 0;}
                            try { contID = reader.GetInt32(1); }catch(Exception){ contID = 0;}
                            try { name = reader.GetString(2); }catch(Exception){ name = null; }
                            try { url = reader.GetString(3);}catch(Exception){ url = null; }
                            try { desc = reader.GetString(4);}catch(Exception) { desc = null;}
                            try { smalld = reader.GetString(5);}catch(Exception){ smalld = null; }
                            try { link = reader.GetString(6);}catch(Exception){ link = null; }
                            try { pos = reader.GetInt32(7);}catch(Exception){ pos = 0;}
                            try { vis = reader.GetBoolean(8);}catch(Exception){ vis = false;}
                            Manager contact = new Manager();
                            if(contID != 0){
                                contact = this.GetManagerByID(contID);
                            }
                            brands.Add(new Brand(id, name, link ,url, desc, smalld, contact, pos, vis));
                        }
                        return brands;
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetBrands] "+ex.Message);
                return new List<Brand>();
            }
        }

        public bool VerifyManagerExistence(Manager manager){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT 1 FROM Managers WHERE Email = @email OR ID = @id";
                        cmd.Parameters.AddWithValue("@email", manager.Email);
                        cmd.Parameters.AddWithValue("@id", manager.ID);
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            return false;
                        }else{
                            return true;
                        }
                    }
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool VerifyDepartmentExistence(Department dp){
            try{
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT 1 FROM Departments WHERE Name = @name OR ID = @id";
                        cmd.Parameters.AddWithValue("@id", dp.ID);
                        cmd.Parameters.AddWithValue("@name", dp.Name);
                        var reader = cmd.ExecuteReader();
                        if(reader.HasRows == true){
                            return true;
                        }else{
                            return false;
                        }
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[VerifyDepartmentExistence] "+ ex.Message);
                return false;
            }
        }

        public bool VerifyUserExistence(User usr){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT 1 FROM Users WHERE Email = @email OR ID = @id";
                        cmd.Parameters.AddWithValue("@email", usr.Email);
                        cmd.Parameters.AddWithValue("@id", usr.ID);
                        var reader = cmd.ExecuteReader();
                        if(reader.HasRows){
                            return true;
                        }else{
                            return false;
                        }
                    }
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private bool _verifyConnection(){
            
            try{
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SHOW TABLES";
                        var reader = cmd.ExecuteReader();
                        if(reader.HasRows){
                            return true;
                        }else{
                            return false;
                        }
                    } 
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                return false;
            }
            
        }

        public string ResultMessage { get { return _resultMessage; } } 
    }
}
