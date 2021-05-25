using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using accela.Extensions;

namespace accela.Models
{
    public class User : IdentityUser
    {
        private int _id;
        private string _firstname;
        private string _lastname;
        private string _email;
        private string _password;
        private string _level;
        private string _message;
        private string _repassword;

        public User()
        {

        }

        public User(int id, string fname, string lname, string email, string level)
        {
            _id = id;
            _firstname = fname;
            _lastname = lname;
            _email = email;
            _level = level;
            
            //Je třeba specifikovat username pro framework
            this.UserName = email;

            if(_id == 0){
                this._message = "Nastala chyba při vytvoření instance uživatele";
            }
        }

        public int ID { get { return _id; } set{ _id = value; } }
        public string Firstname { get { return _firstname; } set { _firstname = value; } }
        public string Lastname { get { return _lastname; } set { _lastname = value; } }
        public override string Email { get { return _email; } set { _email = value; } }
        public string Password { get { return _password; } set { _password = Hasher.ComputeSHA256(value); } }
        public string Level { get { return _level; } set { _level = value; } }
        public string Message { get { return _message; } set { _message = value; } }
        public string Repassword { get { return _repassword; } set { _repassword = Hasher.ComputeSHA256(value); } }

    }
}
