using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace accela.Models
{
    public class User : IdentityUser
    {
        private int _id;
        private string _firstname;
        private string _lastname;
        private string _email;
        private string _password;

        public User()
        {

        }
        
        public int ID{get { return _id; } set{ _id = value; } }
        public string Firstname { get { return _firstname; } set { _firstname = value; } }
        public string Lastname { get { return _lastname; } set { _lastname = value; } }
        public override string Email { get { return _email; } set { _email = value; } }
        public string Password { get { return _password; } set { _password = value; } }


    }
}
