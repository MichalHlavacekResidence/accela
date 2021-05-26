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

        public List<Manager> GetManagers(){
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                return new SystemMessage("Přidání manažera", ex.Message, "Error");
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
