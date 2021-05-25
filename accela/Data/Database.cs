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
