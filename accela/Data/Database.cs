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


        ///
        /// <summary>
        ///     Funkce provede kontrolu hesla a emailu. 
        /// </summary>
        /// <returns>
        ///     Funkce vrátí instanci třídy User, pokud nebyl uživatel přihlášen, nebo nalezen, vrátí instanci třídy s ID = 0.    
        /// </returns>     
        ///
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

        ///
        /// <summary>
        ///     Funkce vytvoří nový záznam uživatele v databázi. Před vytvořením záznamu
        ///     kontroluje existenci emailu v databázi. Email musí být unikátní. 
        /// </summary>
        /// <returns>
        ///     Notifikaci ve formátu SystemMessage obsahující informaci o výsledku operace.    
        /// </returns>     
        ///
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

        ///<summary>
        ///     Funkce pro získání všech zpráv, které byli zaslány skrze modální okna do systému.
        ///</summary>
        ///<returns>
        ///     List objektů třídy Email. Díky tomu je možné ihned připravit (doplnit) objekt a odeslat email.
        ///</returns>
        ///
        public List<Email> GetAllMessages()
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = db.Connection;
                        command.CommandText = "SELECT ID, Name, Email, Text, FormUrl, Created FROM CustomerMessages";
                        var reader = command.ExecuteReader();
                        if(reader.HasRows == false)
                        {
                            return new List<Email>();
                        }

                        int id = 0;
                        string name = null;
                        string email = null;
                        string text = null;
                        string formurl = null;
                        DateTime created = DateTime.Now;
                        List<Email> mails = new List<Email>();
                        while(reader.Read())
                        {
                            try{ id = reader.GetInt32(0);}catch(Exception){ id = 0;}
                            try{ name = reader.GetString(1);}catch(Exception){ name = null;}
                            try{ email = reader.GetString(2);}catch(Exception){ email = null;}
                            try{ text = reader.GetString(3);}catch(Exception){ text = null;}
                            try{ formurl = reader.GetString(4);}catch(Exception){ formurl = null;}
                            try{ created = reader.GetDateTime("Created");}catch(Exception){ created = DateTime.Now;}
                            mails.Add(new Email(id, created, text, name, email, formurl));
                        }
                        return mails;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Email>();
            }
        }

        ///
        ///<summary>
        ///     Funkce je určená k uložení formuláře (dotazníku) do databáze.
        ///</summary>
        ///<returns>
        ///     Funkce vrací výsledek pokusu o zápis do databáze ve formátu objektu SystemMessage.
        ///</returns>
        ///
        public SystemMessage SaveCustomerMessage(string username, string email, string text, string formtype)
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText ="INSERT INTO CustomerMessages (Name, Email, Text, FormURL) VALUES (@nam, @em, @tex, @url)";
                        cmd.Parameters.AddWithValue("@nam", username);
                        cmd.Parameters.AddWithValue("@em", email);
                        cmd.Parameters.AddWithValue("@tex", text);
                        cmd.Parameters.AddWithValue("@url", formtype);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Message successfully sent", "Your message was successfully sent to our system.", "ok");
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new SystemMessage("Message was not sent", ex.Message, "Error");
            }
        }

        ///
        /// <summary>
        ///     Funkce získá z databáze všechny manažery, viditelné i skryté. 
        /// </summary>
        /// <returns>
        ///     Pole ve formátu List[Manager].    
        /// </returns>     
        ///
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
        ///
        ///<summary>
        ///     Funkce pro získání produktů se specifickým BranID u produktu.
        ///</summary>
        ///<returns>
        ///     List produktů v datovém formátu List[Product].
        ///</returns>
        ///
        public List<Product> GetProductsForBrand(int brandID)
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Subtitle, Small_desc, Description, Link, Visibility, BrandID, CategoryID, VideoURL, ReferenceLink, ManagerID FROM Products WHERE BrandID = @bid";
                        cmd.Parameters.AddWithValue("@bid", brandID);
                        var reader = cmd.ExecuteReader();

                        int id = 0;
                        string name = null;
                        string url = null;
                        string subtitle = null;
                        string smalld = null;
                        string descr = null;
                        string link = null;
                        bool vis = false;
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
                            try { bID = reader.GetInt16(8);}catch(Exception){ bID = 0; }
                            try { catID = reader.GetInt16(9);}catch(Exception){catID = 0;}
                            try { vidUrl = reader.GetString(10);}catch(Exception){vidUrl = null;}
                            try { referenceLink = reader.GetString(11);}catch(Exception){referenceLink = null;}
                            try { managerID = reader.GetInt16(12); }catch(Exception){ managerID = 0;}
                            if(managerID != 0){
                                mng = this.GetManagerByID(managerID);
                            }
                            products.Add(new Product(id, name, url, descr, subtitle, smalld, referenceLink, mng, vidUrl, vis));
                        }
                        return products;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetProductsForBrand] "+ex.Message);
                return new List<Product>();
            }
        }

            
        ///
        /// <summary>
        ///     Funkce získá z databáze všechny viditelné manažery. 
        /// </summary>
        /// <returns>
        ///     Pole ve formátu List[Manager].    
        /// </returns>     
        ///
        public List<Manager> GetVisibleManagers()
        {
             try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Firstname, Lastname, Email, Phone, Img, Description, Visibility, Position, DepartmentID FROM Managers Where Visibility = 1";
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

        ///
        /// <summary>
        ///     Funkce získá z databáze všechny viditelné Pools (nejvyšší kategorie). 
        /// </summary>
        /// <returns>
        ///     Pole ve formátu List[Category].    
        /// </returns>     
        ///
        public List<Category> GetVisiblePools()
        {
            try
            {
                using (var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, Url, Description, Visibility, Position, Img, ContactID FROM Pools WHERE Visibility = 1";
                        var reader = cmd.ExecuteReader();
                        if(reader.HasRows == false)
                        {
                            return new List<Category>();
                        }

                        int id = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        bool vis = false;
                        int pos = 0;
                        string img = null;
                        int contId = 0;
                        List<Category> cats = new List<Category>();
                        while(reader.Read())
                        {
                            try{ id = reader.GetInt32(0);}catch(Exception){ id = 0;}
                            try{ name = reader.GetString(1);}catch(Exception){ name = "Neznámá kategorie"; }
                            try{ url = reader.GetString(2);}catch(Exception){ url = null; }
                            try{ desc = reader.GetString(3);}catch(Exception){ desc = null; }
                            try{ vis = reader.GetBoolean(4);}catch(Exception){ vis = false; }
                            try{ pos = reader.GetInt32(5);}catch(Exception){ pos = 0;}
                            try{ img = reader.GetString(6);}catch(Exception){ img = null; }
                            try{ contId = reader.GetInt32(7);}catch(Exception){ contId = 0; }
                            cats.Add(new Category(id, name, url, desc, img, null, pos, vis, this.GetManagerByID(contId)));
                        }
                        return cats;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetPools] " +ex.Message);
                return new List<Category>();
            }
        }

        ///
        /// <summary>
        ///     Funkce získá z databáze všechny Pools, viditelné i skryté. (nejvyšší kategorie). 
        /// </summary>
        /// <returns>
        ///     Pole ve formátu List[Category].    
        /// </returns>     
        ///
        public List<Category> GetAllPools()
        {
            try
            {
                using (var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, Url, Description, Visibility, Position, Img, ContactID FROM Pools";
                        var reader = cmd.ExecuteReader();
                        if(reader.HasRows == false)
                        {
                            return new List<Category>();
                        }

                        int id = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        bool vis = false;
                        int pos = 0;
                        string img = null;
                        int contId = 0;
                        List<Category> cats = new List<Category>();
                        while(reader.Read())
                        {
                            try{ id = reader.GetInt32(0);}catch(Exception){ id = 0;}
                            try{ name = reader.GetString(1);}catch(Exception){ name = "Neznámá kategorie"; }
                            try{ url = reader.GetString(2);}catch(Exception){ url = null; }
                            try{ desc = reader.GetString(3);}catch(Exception){ desc = null; }
                            try{ vis = reader.GetBoolean(4);}catch(Exception){ vis = false; }
                            try{ pos = reader.GetInt32(5);}catch(Exception){ pos = 0;}
                            try{ img = reader.GetString(6);}catch(Exception){ img = null; }
                            try{ contId = reader.GetInt32(7);}catch(Exception){ contId = 0; }
                            cats.Add(new Category(id, name, url, desc, img, null, pos, vis, this.GetManagerByID(contId)));
                        }
                        return cats;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetPools] " +ex.Message);
                return new List<Category>();
            }
        }

        ///
        /// <summary>
        ///     Funkce přidá do databáze záznam o novém Poolu (nejvyšší kategori). 
        /// </summary>
        /// <returns>
        ///     Notifikaci ve formátu SystemMessage obsahující informaci o výsledku operace.    
        /// </returns>     
        ///
        public SystemMessage AddPool(Category pool)
        {
            try
            {
                using (var db = new AppDb())
                {
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "INSERT INTO Pools (Name, Url, Description, Visibility, Position, Img, ContactID) VALUES (@name, @url, @desc, @vis, @pos, @img, @cId)";
                        cmd.Parameters.AddWithValue("@name", pool.Name);
                        cmd.Parameters.AddWithValue("@url", pool.URL);
                        cmd.Parameters.AddWithValue("@desc", pool.Description);
                        cmd.Parameters.AddWithValue("@vis", pool.Visibility);
                        cmd.Parameters.AddWithValue("@pos", pool.Position);
                        cmd.Parameters.AddWithValue("@img", pool.Image);
                        cmd.Parameters.AddWithValue("@cId", pool.Contact.ID);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Adding pool", "Pool was successfully added.", "ok");
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[AddPools] "+ex.Message);
                return new SystemMessage("Adding pool", "Fatal error: "+ex.Message, "error");
            }
        }

        ///
        /// <summary>
        ///     Funkce přidá do databáze záznam o nové kategorii. 
        /// </summary>
        /// <returns>
        ///     Notifikaci ve formátu SystemMessage obsahující informaci o výsledku operace.    
        /// </returns>     
        ///
        public SystemMessage AddCategory(Category cat)
        {
            try
            {
                using (var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "INSERT INTO Categories (PoolID, ContactID, Name, Url, Description, Img, Visibility, Position) VALUES (@pId, @cId, @name, @url, @desc, @img, @vis, @pos)";
                        cmd.Parameters.AddWithValue("@pId", cat.Pool.ID);
                        cmd.Parameters.AddWithValue("@cId", cat.Contact.ID);
                        cmd.Parameters.AddWithValue("@name", cat.Name);
                        cmd.Parameters.AddWithValue("@url", cat.URL);
                        cmd.Parameters.AddWithValue("@desc", cat.Description);
                        cmd.Parameters.AddWithValue("@img", cat.Image);
                        cmd.Parameters.AddWithValue("@vis", cat.Visibility);
                        cmd.Parameters.AddWithValue("@pos", cat.Position);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Added new category", "Successfully added new category called: "+cat.Name, "OK");
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[AddCategory] "+ ex.Message);
                return new SystemMessage("Trying to add category", "Error occured: "+ex.Message, "error");
            }
        }

        ///
        /// <summary>
        ///     Funkce získá z databáze detaily o žádaném oddělení (Department). 
        /// </summary>
        /// <returns>
        ///     Instance objektu Department, pokud nebyl nalezen žádný záznam se zadaným ID,
        ///     je vrácen prázdný objekt obsahující ID = 0;  
        /// </returns>     
        ///
        public Department GetDepartmentDetails(int Id){
            try{
                using(var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Visibility, Position FROM Departments WHERE ID = @id";
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
        ///
        /// <summary>
        ///     Funkce získá z databáze záznamy všech produktů, viditelných i skrytých. (Department). 
        /// </summary>
        /// <returns>
        ///     Funkce vrátí List[Product].  
        /// </returns>     
        ///
        public List<Product> GetAllProducts (){
            try{
                using (var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Subtitle, Small_desc, Description, Link, Visibility, BrandID, CategoryID, VideoURL, ReferenceLink, ManagerID FROM Products";
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
                        int bID = 0;
                        int catID = 0;
                        string vidUrl = null;
                        string referenceLink = null;
                        int managerID = 0;
                        string image = null;
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
                            try { bID = reader.GetInt16(8);}catch(Exception){ bID = 0; }
                            try { catID = reader.GetInt16(9);}catch(Exception){catID = 0;}
                            try { vidUrl = reader.GetString(10);}catch(Exception){vidUrl = null;}
                            try { referenceLink = reader.GetString(11);}catch(Exception){referenceLink = null;}
                            try { managerID = reader.GetInt16(12); }catch(Exception){ managerID = 0;}
                            try { image = reader.GetString("Image");}catch(Exception){ image = null;}
                            if(managerID != 0){
                                mng = this.GetManagerByID(managerID);
                            }
                            products.Add(new Product(id, name, url, descr, subtitle, smalld, referenceLink, this.GetBrandByID(bID), mng, vidUrl, vis, image));
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

        ///
        ///<summary>
        ///     Funkce pro získání všech novinek z databáze, skrytých i viditelných.
        ///</summary>
        ///<returns>
        ///     Funkce vrací pole ve formátu List[News].
        ///</returns>
        public List<News> GetAllNews()
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Title, BrandID, ContactID, ImageBig, ImageSmall, Content, ContentSmall, Created, VideoURL, imageNew FROM News";
                        var reader = cmd.ExecuteReader();

                        if(reader.HasRows == false)
                        {
                            return new List<News>();
                        }

                        int id = 0;
                        string title = null;
                        int bid = 0;
                        int cid = 0;
                        string imgbig = null;
                        string imgsml = null;
                        string content = null;
                        string contentsml = null;
                        DateTime Created = DateTime.Now;
                        string videourl = null;
                        List<News> news = new List<News>();
                        string imgnew = null;

                        while(reader.Read())
                        {
                            try{ id = reader.GetInt32(0);}catch(Exception ex){ id = 0;}
                            try{ title = reader.GetString(1);}catch(Exception ex){ title = null;}
                            try{ bid = reader.GetInt32(2);}catch(Exception ex){ bid = 0;}
                            try{ cid = reader.GetInt32(3);}catch(Exception){ cid = 0;}
                            try{ imgbig = reader.GetString(4);}catch(Exception){ imgbig = null;}
                            try{ imgsml = reader.GetString(5);}catch(Exception){ imgsml = null;}
                            try{ content = reader.GetString(6);}catch(Exception){ content = null;}
                            try{ contentsml = reader.GetString(7);}catch(Exception){ contentsml = null;}
                            try{ Created = reader.GetDateTime(8);}catch(Exception){ Created = DateTime.Now;}
                            try{ videourl = reader.GetString(9);}catch(Exception){ videourl = null;}
                            try { imgnew = reader.GetString(5); } catch (Exception) { imgnew = null; }
                            news.Add(new News(id, title, this.GetBrandByID(bid), this.GetManagerByID(cid), imgbig, imgsml, content, contentsml, Created, videourl, imgnew));
                        }
                        return news;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetAllNews] "+ex.Message);
                return new List<News>();
            }
        }

        ///
        ///<summary>
        ///     Funkce pro získání konkrétní novinky. Jako vstup je potřeba zadat ID novinky.
        ///</summary>
        ///<returns>
        ///     Funkce vrací objekt News. Pokud nebyla novinka nalezena, vrací se prázdný objekt s ID = 0.
        ///</returns>
        public News GetNew(int nid)
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Title, BrandID, ContactID, ImageBig, ImageSmall, Content, ContentSmall, Created, VideoURL, imageNew FROM News WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", nid);
                        var reader = cmd.ExecuteReader();

                        if(reader.HasRows == false)
                        {
                            return new News();
                        }

                        int id = 0;
                        string title = null;
                        int bid = 0;
                        int cid = 0;
                        string imgbig = null;
                        string imgsml = null;
                        string content = null;
                        string contentsml = null;
                        DateTime Created = DateTime.Now;
                        string videourl = null;
                        string imgnew = null;

                        while (reader.Read())
                        {
                            try{ id = reader.GetInt32(0);}catch(Exception ex){ id = 0;}
                            try{ title = reader.GetString(1);}catch(Exception ex){ title = null;}
                            try{ bid = reader.GetInt32(2);}catch(Exception ex){ bid = 0;}
                            try{ cid = reader.GetInt32(3);}catch(Exception){ cid = 0;}
                            try{ imgbig = reader.GetString(4);}catch(Exception){ imgbig = null;}
                            try{ imgsml = reader.GetString(5);}catch(Exception){ imgsml = null;}
                            try{ content = reader.GetString(6);}catch(Exception){ content = null;}
                            try{ contentsml = reader.GetString(7);}catch(Exception){ contentsml = null;}
                            try{ Created = reader.GetDateTime(8);}catch(Exception){ Created = DateTime.Now;}
                            try{ videourl = reader.GetString(9);}catch(Exception){ videourl = null;}
                            try { imgnew = reader.GetString(4); } catch (Exception) { imgnew = null; }
                        }
                        return new News(id, title, this.GetBrandByID(bid), this.GetManagerByID(cid), imgbig, imgsml, content, contentsml, Created, videourl, imgnew);
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetNew] "+ex.Message);
                return new News();
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

        ///
        ///<summary>
        ///     Funkce pro aktualizaci manažera. Jako vstupní parametr se očekává naplněný objekt Managera. 
        ///</summary>
        ///<returns>
        ///     Notifikaci ve formátu SystemMessage.
        ///</returns>
        ///
        public SystemMessage UpdateManager(Manager mng)
        {
            if(this.VerifyManagerExistence(mng) == false)
            {
                return new SystemMessage("Updating manager", "Manager with this ID was not found.", "Error");
            }

            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "UPDATE Managers SET Firstname = @fname, Lastname = @lname, Email = @mail, Phone = @phone, Img = @img, Description = @desc, Visibility = @vis, Position = @pos, DepartmentID = @dep WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", mng.ID);
                        cmd.Parameters.AddWithValue("@fname", mng.Firstname);
                        cmd.Parameters.AddWithValue("@lname", mng.Lastname);
                        cmd.Parameters.AddWithValue("@mail", mng.Email);
                        cmd.Parameters.AddWithValue("@phone", mng.Phone);
                        cmd.Parameters.AddWithValue("@img", mng.ImageRelativeURL);
                        cmd.Parameters.AddWithValue("@desc", mng.Description);
                        cmd.Parameters.AddWithValue("@vis", mng.Visibility);
                        cmd.Parameters.AddWithValue("@pos", mng.Position);
                        cmd.Parameters.AddWithValue("@dep", mng.Department.ID);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Updating Manager", "Manager "+mng.Firstname +" "+mng.Lastname+" was successfully updated.", "OK");    
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[UpdateManager] "+ex.Message);
                return new SystemMessage("Updating manager", "Critical failure: "+ex.Message, "Error");
            }
            
        }

        public SystemMessage UpdateBrand(Brand bt)
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "UPDATE Brands SET Name = @name, URL = @url, Visibility = @vis, Position = @pos, Image = @img WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@name", bt.Name);
                        cmd.Parameters.AddWithValue("@url", bt.URL);
                        cmd.Parameters.AddWithValue("@vis", bt.Visibility);
                        cmd.Parameters.AddWithValue("@pos", bt.Position);
                        cmd.Parameters.AddWithValue("@img", bt.ImageRelativeURL);
                        cmd.Parameters.AddWithValue("@id", bt.ID);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Updating Brand", "Brand was successfully updated", "OK");
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[UpdateBrand] "+ex.Message);
                return new SystemMessage("Updating Brand", ex.Message, "Error");   
            }
        }

        public SystemMessage UpdateDepartment(Department dp) 
        {
             if(this.VerifyDepartmentExistence(dp) == false)
            {
                return new SystemMessage("Updating department", "Department with this ID was not found.", "Error");
            }

            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "UPDATE Departments SET Name = @name, URL = @url, Visibility = @vis, Position = @pos WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", dp.ID);
                        cmd.Parameters.AddWithValue("@name", dp.Name);
                        cmd.Parameters.AddWithValue("@url", dp.URL);
                        cmd.Parameters.AddWithValue("@vis", dp.Visibility);
                        cmd.Parameters.AddWithValue("@pos", dp.Position);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Updating Department", "Department "+dp.Name +" was successfully updated.", "OK");    
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[UpdateDepartment] "+ex.Message);
                return new SystemMessage("Updating department", "Critical failure: "+ex.Message, "Error");
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
                        cmd.CommandText = "INSERT INTO Products (Name, URL, Subtitle, Small_desc, Description, Visibility, BrandID, CategoryID, VideoURL,ReferenceLink, ManagerID) VALUES (@name, @url, @sub, @smalld, @desc, @vis,@brandId, @catId, @videoUrl, @referLink, @manId)";
                        cmd.Parameters.AddWithValue("@name", product.Name);
                        cmd.Parameters.AddWithValue("@url", product.URL);
                        cmd.Parameters.AddWithValue("@sub", product.Subtitle);
                        cmd.Parameters.AddWithValue("@smalld", product.SmallDescription);
                        cmd.Parameters.AddWithValue("@desc", product.Description);
                        cmd.Parameters.AddWithValue("@vis", product.Visibility);
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
                        cmd.Parameters.AddWithValue("@link", brand.ReferenceLink);
                        cmd.ExecuteNonQuery();
                        return new SystemMessage("Adding new brand", "Brand was succesfully added", "OK");
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[AddProduct] " + ex.Message);
                return new SystemMessage("Adding new brand", ex.Message, "Error");
            }
        }

        public List<Category> GetAllCategories()
        {
            try{
                using (var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, PoolID, ContactID, Name, Url, Description, Img, Visibility, Position FROM Categories";
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new List<Category>();
                        }

                        int id = 0;
                        int pid = 0;
                        int cid = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        string img = null;
                        bool vis = false;
                        int pos = 0;
                        List<Category> cats = new List<Category>();

                        while(reader.Read())
                        {
                            try{id = reader.GetInt32(0);}catch(Exception){ id = 0;}
                            try{pid = reader.GetInt32(1);}catch(Exception){ pid = 0;}
                            try{cid = reader.GetInt32(2);}catch(Exception){ cid = 0;}
                            try{name = reader.GetString(3);}catch(Exception){ name = null;}
                            try{url = reader.GetString(4);}catch(Exception){ url = null;}
                            try{desc = reader.GetString(5);}catch(Exception){desc = null;}
                            try{img = reader.GetString(6);}catch(Exception){img = null;}
                            try{vis = reader.GetBoolean(7);}catch(Exception){vis = false;}
                            try{pos = reader.GetInt32(8);}catch(Exception){pos = 0;}
                            //Bez možnosti přidat pool (zatim)
                            cats.Add(new Category(id, this.GetPool(pid), name, url, desc, img, desc, pos, vis, this.GetManagerByID(cid)));
                        }
                        return cats;
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetAllCategories] "+ex.Message);
                return new List<Category>();
            }
        }

        public Category GetPool(int id)
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();

                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, Url, Description, Visibility, Position, Img, ContactID FROM Pools WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", id);
                        var reader = cmd.ExecuteReader();

                        if(reader.HasRows == false)
                        {
                            return new Category();
                        }

                        int ID = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        bool vis = false;
                        int pos = 0;
                        string img = null;
                        int conid = 0;

                        while(reader.Read())
                        {
                            try{ ID = reader.GetInt32(0);}catch(Exception){ ID = 0;}
                            try{ name = reader.GetString(1);}catch(Exception) { name = null;}
                            try{ url = reader.GetString(2);}catch(Exception){ url = null;}
                            try{ desc = reader.GetString(3);}catch(Exception){ desc= null;}
                            try{ vis = reader.GetBoolean(4);}catch(Exception){ vis = false;}
                            try{ pos = reader.GetInt32(5);}catch(Exception){ pos = 0;}
                            try{ img = reader.GetString(6);}catch(Exception){ img = null;}
                            try{ conid = reader.GetInt32(7);}catch(Exception){ conid = 0;}
                        }
                        return new Category(ID, name, url, desc, img, "", pos, vis, this.GetManagerByID(conid));
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetPool] "+ex.Message);
                return new Category();
            }
        }

        public Category GetCategory(int id)
        {
             try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();

                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, Url, Description, Visibility, Position, Img, ContactID, PoolID FROM Categories WHERE ID = @id";
                        cmd.Parameters.AddWithValue("@id", id);
                        var reader = cmd.ExecuteReader();

                        if(reader.HasRows == false)
                        {
                            return new Category();
                        }

                        int ID = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        bool vis = false;
                        int pos = 0;
                        string img = null;
                        int conid = 0;
                        int poolid = 0;

                        while(reader.Read())
                        {
                            try{ ID = reader.GetInt32(0);}catch(Exception){ ID = 0;}
                            try{ name = reader.GetString(1);}catch(Exception) { name = null;}
                            try{ url = reader.GetString(2);}catch(Exception){ url = null;}
                            try{ desc = reader.GetString(3);}catch(Exception){ desc= null;}
                            try{ vis = reader.GetBoolean(4);}catch(Exception){ vis = false;}
                            try{ pos = reader.GetInt32(5);}catch(Exception){ pos = 0;}
                            try{ img = reader.GetString(6);}catch(Exception){ img = null;}
                            try{ conid = reader.GetInt32(7);}catch(Exception){ conid = 0;}
                            try{ poolid = reader.GetInt32(8);}catch(Exception){ poolid = 0;}
                        }
                        return new Category(ID, this.GetPool(poolid), name, url, desc, img, "", pos, vis, this.GetManagerByID(conid));
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetCategory] "+ex.Message);
                return new Category();
            }
        }

        public List<Category> GetCategoriesForPool(int poolID)
        {
            try{
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand command= new MySqlCommand())
                    {
                        command.Connection = db.Connection;
                        command.CommandText = "SELECT ID, Name, Url, Description, Visibility, Position, Img, ContactID, PoolID FROM Categories WHERE PoolID = @id";
                        command.Parameters.AddWithValue("@id", poolID);
                        var reader = command.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new List<Category>();
                        }

                        int ID = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        bool vis = false;
                        int pos = 0;
                        string img = null;
                        int conid = 0;
                        int poolid = poolID;
                        List<Category> poolCategories = new List<Category>();
                        while(reader.Read())
                        {
                            try{ ID = reader.GetInt32(0);}catch(Exception){ ID = 0;}
                            try{ name = reader.GetString(1);}catch(Exception) { name = null;}
                            try{ url = reader.GetString(2);}catch(Exception){ url = null;}
                            try{ desc = reader.GetString(3);}catch(Exception){ desc= null;}
                            try{ vis = reader.GetBoolean(4);}catch(Exception){ vis = false;}
                            try{ pos = reader.GetInt32(5);}catch(Exception){ pos = 0;}
                            try{ img = reader.GetString(6);}catch(Exception){ img = null;}
                            try{ conid = reader.GetInt32(7);}catch(Exception){ conid = 0;}
                            try{ poolid = reader.GetInt32(8);}catch(Exception){ poolid = 0;}
                            poolCategories.Add(new Category(ID, this.GetPool(poolid), name, url, desc, img, "", pos, vis, this.GetManagerByID(conid)));
                        }
                        return poolCategories;
                   }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetCategoriesForPool] "+ex.Message);
                return new List<Category>();
            }
        }

        public List<Category> GetVisibleCategories()
        {
            try{
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand command= new MySqlCommand())
                    {
                        command.Connection = db.Connection;
                        command.CommandText = "SELECT ID, Name, Url, Description, Visibility, Position, Img, ContactID, PoolID FROM Categories WHERE Visibility = 1";
                        var reader = command.ExecuteReader();
                        if(!reader.HasRows)
                        {
                            return new List<Category>();
                        }

                        int ID = 0;
                        string name = null;
                        string url = null;
                        string desc = null;
                        bool vis = false;
                        int pos = 0;
                        string img = null;
                        int conid = 0;
                        List<Category> poolCategories = new List<Category>();
                        while(reader.Read())
                        {
                            try{ ID = reader.GetInt32(0);}catch(Exception){ ID = 0;}
                            try{ name = reader.GetString(1);}catch(Exception) { name = null;}
                            try{ url = reader.GetString(2);}catch(Exception){ url = null;}
                            try{ desc = reader.GetString(3);}catch(Exception){ desc= null;}
                            try{ vis = reader.GetBoolean(4);}catch(Exception){ vis = false;}
                            try{ pos = reader.GetInt32(5);}catch(Exception){ pos = 0;}
                            try{ img = reader.GetString(6);}catch(Exception){ img = null;}
                            try{ conid = reader.GetInt32(7);}catch(Exception){ conid = 0;}
                            poolCategories.Add(new Category(ID,name, url, desc, img, "", pos, vis, this.GetManagerByID(conid)));
                        }
                        return poolCategories;
                   }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetCategories] "+ex.Message);
                return new List<Category>();
            }
        }

        public List<Product> GetVisibleProducts(){
            try{
                using (var db = new AppDb()){
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand()){
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Subtitle, Small_desc, Description, Link, Visibility,BrandID, CategoryID, VideoURL, ReferenceLink, ManagerID, Image FROM Products WHERE Visibility = 1 ORDER BY Position ASC";
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
                        int bID = 0;
                        int catID = 0;
                        string vidUrl = null;
                        string referenceLink = null;
                        int managerID = 0;
                        string image = null;
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
                            try { bID = reader.GetInt16(8);}catch(Exception){ bID = 0; }
                            try { catID = reader.GetInt16(9);}catch(Exception){catID = 0;}
                            try { vidUrl = reader.GetString(10);}catch(Exception){vidUrl = null;}
                            try { referenceLink = reader.GetString(11);}catch(Exception){referenceLink = null;}
                            try { managerID = reader.GetInt16(12); }catch(Exception){ managerID = 0;}
                            try { image = reader.GetString("Image");}catch(Exception) { image = null; }
                            if(managerID != 0){
                                mng = this.GetManagerByID(managerID);
                            }
                            products.Add(new Product(id, name, url, descr, subtitle, smalld, referenceLink, this.GetBrandByID(bID), mng, vidUrl, vis, image));
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
                        cmd.CommandText = "SELECT ID, Name, URL, Subtitle, Small_desc, Description, Link, Visibility, BrandID, CategoryID, VideoURL, ReferenceLink, ManagerID FROM Products WHERE ID = @id";
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
                        int bID = 0;
                        int catID = 0;
                        string vidUrl = null;
                        string referenceLink = null;
                        int managerID = 0;
                        string image= null;
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
                            try { bID = reader.GetInt16(8);}catch(Exception){ bID = 0; }
                            try { catID = reader.GetInt16(9);}catch(Exception){catID = 0;}
                            try { vidUrl = reader.GetString(10);}catch(Exception){vidUrl = null;}
                            try { referenceLink = reader.GetString(11);}catch(Exception){referenceLink = null;}
                            try { managerID = reader.GetInt16(12); }catch(Exception){ managerID = 0;}
                            try { image = reader.GetString("Image");}catch(Exception) { image = null;}                            
                            if(managerID != 0){
                                mng = this.GetManagerByID(managerID);
                            }
                        }
                        return new Product(id, name, url, descr, subtitle, smalld, referenceLink, this.GetBrandByID(bID), mng, vidUrl, vis, image);
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetProduct] "+ex.Message);
                return new Product();
            }
        }
        ///
        ///<summary>
        ///     Funkce pro získání listu produktů se specifickým ManagerID
        ///</summary>
        ///<returns>
        ///     Vrací pole produktů (List[Product]).
        ///</returns>
        public List<Product> GetProductsForManager(int managerID)
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, URL, Subtitle, Small_desc, Description, Link, Visibility, BrandID, CategoryID, VideoURL, ReferenceLink FROM Products WHERE ManagerID = @bid";
                        cmd.Parameters.AddWithValue("@bid", managerID);
                        var reader = cmd.ExecuteReader();

                        int id = 0;
                        string name = null;
                        string url = null;
                        string subtitle = null;
                        string smalld = null;
                        string descr = null;
                        string link = null;
                        bool vis = false;
                        int bID = 0;
                        int catID = 0;
                        string vidUrl = null;
                        string referenceLink = null;
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
                            try { bID = reader.GetInt16(8);}catch(Exception){ bID = 0; }
                            try { catID = reader.GetInt16(9);}catch(Exception){catID = 0;}
                            try { vidUrl = reader.GetString(10);}catch(Exception){vidUrl = null;}
                            try { referenceLink = reader.GetString(11);}catch(Exception){referenceLink = null;}
                            try { managerID = reader.GetInt16(12); }catch(Exception){ managerID = 0;}

                            products.Add(new Product(id, name, url, descr, subtitle, smalld, referenceLink, mng, vidUrl, vis));
                        }
                        return products;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[GetProductsForManager] "+ex.Message);
                return new List<Product>();
            }  
        }

        public List<Manager> LoadManagersForDepartment(int DepartmentID)
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Firstname, Lastname, Email, Phone, Img, Description, Visibility, Position, DepartmentID FROM Managers WHERE DepartmentID = @id";
                        cmd.Parameters.AddWithValue("@id", DepartmentID);
                        var reader = cmd.ExecuteReader();
                        if(!reader.HasRows){
                            return new List<Manager>();
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
                        List<Manager> mngrs = new List<Manager>();

                        while(reader.Read()){
                            try { id = reader.GetInt16("ID"); }catch(Exception){ id = 0;}
                            try { fname = reader.GetString("Firstname");} catch(Exception){fname = null;}
                            try { lname = reader.GetString("Lastname");} catch(Exception){lname = null;}
                            try { email = reader.GetString("Email");} catch(Exception){email = null;}
                            try { phone = reader.GetString("Phone");} catch(Exception){phone = null;}
                            try { img = reader.GetString("Img");}catch(Exception){img = null;}
                            try { desc = reader.GetString("Description");}catch(Exception){desc = null;}
                            try { vis = reader.GetBoolean("Visibility");}catch(Exception){vis = false;}
                            try { pos = reader.GetInt16("Position");}catch(Exception){pos = 0;}
                            try { depID = reader.GetInt32("DepartmentID");}catch(Exception){depID = 0;}
                            mngrs.Add(new Manager(id, fname, lname, email, phone, desc, img, vis, pos));
                        }
                        return mngrs;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[LoadManagersForDepartment] "+ex.Message);
                return new List<Manager>();
            }
        }


        ///
        ///<summary>
        ///     Funkce pro načtení detailů manažera na základě zadaného ID.
        ///</summary>
        ///<returns>
        ///     Funkce vrací naplněnou instanci třídy Manager. V případě selhání, nebo nenalezení manažera
        ///     se vrací objekt s ID = 0;
        ///</returns>
        ///
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
                        cmd.CommandText = "SELECT ID, ContactID, Name, URL, Description, Small_desc, Link, Position, Visibility, Image FROM Brands WHERE ID = @id";
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
                        string img = null;

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
                            try { img = reader.GetString(9);}catch(Exception){ img = null;}
                        }
                        Manager contact = new Manager();
                        if(contID != 0){
                            contact = this.GetManagerByID(contID);
                        }
                        return new Brand(id, name, link ,url, desc, smalld, contact, pos, vis, img);
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
                        cmd.CommandText = "SELECT ID, ContactID, Name, URL, Description, Small_desc, Link, Position, Visibility, Image FROM Brands";
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
                        string img = null;
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
                            try { img = reader.GetString(9);}catch(Exception){ img = null;}
                            Manager contact = new Manager();
                            if(contID != 0){
                                contact = this.GetManagerByID(contID);
                            }
                            brands.Add(new Brand(id, name, link ,url, desc, smalld, contact, pos, vis, img));
                        }
                        return brands;
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("[GetBrands] "+ex.Message);
                return new List<Brand>();
            }
        }
        public List<Brand> GetVisibleBrands()
        {
            try
            {
                using (var db = new AppDb())
                {
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, ContactID, Name, URL, Description, Small_desc, Link, Position, Visibility, Image FROM Brands WHERE Visibility = 1";
                        var reader = cmd.ExecuteReader();
                        if (!reader.HasRows)
                        {
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
                        string img = null;
                        List<Brand> brands = new List<Brand>();
                        while (reader.Read())
                        {
                            try { id = reader.GetInt32(0); } catch (Exception) { id = 0; }
                            try { contID = reader.GetInt32(1); } catch (Exception) { contID = 0; }
                            try { name = reader.GetString(2); } catch (Exception) { name = null; }
                            try { url = reader.GetString(3); } catch (Exception) { url = null; }
                            try { desc = reader.GetString(4); } catch (Exception) { desc = null; }
                            try { smalld = reader.GetString(5); } catch (Exception) { smalld = null; }
                            try { link = reader.GetString(6); } catch (Exception) { link = null; }
                            try { pos = reader.GetInt32(7); } catch (Exception) { pos = 0; }
                            try { vis = reader.GetBoolean(8); } catch (Exception) { vis = false; }
                            try { img = reader.GetString(9); } catch (Exception) { img = null; }
                            Manager contact = new Manager();
                            if (contID != 0)
                            {
                                contact = this.GetManagerByID(contID);
                            }
                            brands.Add(new Brand(id, name, link, url, desc, smalld, contact, pos, vis, img));
                        }
                        return brands;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[GetVisibleBrands] " + ex.Message);
                return new List<Brand>();
            }
        }
        public List<References> GetAllReferences()
        {
            try
            {
                using (var db = new AppDb())
                {
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, Company,Img, Visibility,Position,Description FROM Ref";
                        var reader = cmd.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            return new List<References>();
                        }

                        int id = 0;
                        string name = null;
                        string company = null;
                        string img = null;
                        int pos = 0;
                        bool vis = false;
                        string desc = null;
                        List<References> referenced = new List<References>();
                        while (reader.Read())
                        {
                            try { id = reader.GetInt32(0); } catch (Exception) { id = 0; }
                            try { img = reader.GetString(9); } catch (Exception) { img = null; }
                            try { name = reader.GetString(2); } catch (Exception) { name = null; }
                            try { company = reader.GetString(2); } catch (Exception) { company = null; }
                            try { pos = reader.GetInt32(7); } catch (Exception) { pos = 0; }
                            try { vis = reader.GetBoolean(8); } catch (Exception) { vis = false; }
                            try { desc = reader.GetString(4); } catch (Exception) { desc = null; }

                           
                            referenced.Add(new References(id, name,  company,img,pos, vis, desc));
                        }
                        return referenced;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[GetReferences] " + ex.Message);
                return new List<References>();
            }
        }
        public List<References> GetVisibleRefeences()
        {
            try
            {
                using (var db = new AppDb())
                {
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SELECT ID, Name, Company,Img, Visibility,Position,Description FROM Ref WHERE Visibility = 1";
                        var reader = cmd.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            return new List<References>();
                        }

                        int id = 0;
                        string name = null;
                        string company = null;
                        string img = null;
                        int pos = 0;
                        bool vis = false;
                        string desc = null;
                        List<References> referenced = new List<References>();
                        while (reader.Read())
                        {
                            try { id = reader.GetInt32(0); } catch (Exception) { id = 0; }
                            try { name = reader.GetString(2); } catch (Exception) { name = null; }
                            try { company = reader.GetString(2); } catch (Exception) { company = null; }
                            try { img = reader.GetString(3); } catch (Exception) { img = null; }
                            try { pos = reader.GetInt32(7); } catch (Exception) { pos = 0; }
                            try { vis = reader.GetBoolean(8); } catch (Exception) { vis = false; }
                            try { desc = reader.GetString(4); } catch (Exception) { desc = null; }


                            referenced.Add(new References(id, name, company, img, pos, vis, desc));
                        }
                        return referenced;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[GetReferences] " + ex.Message);
                return new List<References>();
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

          /*
            #Funkce pro deaktivaci kontroly cizích klíčů.
            #Tato funkce by neměla být nikdy použita, pokud je použita, dochází k zapsání neexistujícího cizího klíče
        */
        private void _disableForeignChecks()
        {
            try
            {
                using(var db = new AppDb())
                {
                    db.Connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = db.Connection;
                        cmd.CommandText = "SET FOREIGN_KEY_CHECK=0";
                        cmd.ExecuteNonQuery();
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine("[_disableForeignChecks] "+ ex.Message);
            }
        }


        public string ResultMessage { get { return _resultMessage; } } 
    }
}
