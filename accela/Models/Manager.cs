using System;
using System.Collections.Generic;
using System.Diagnostics;
using accela.Models;

namespace Models
{
    public class Manager : User 
    {
        private string _description;
        private string _imageRelativeURL;
        private string _imageAbsoluteURL;
        private string _phone;
        private string _department;
        private int _position;
        private bool _visibility;

        public Manager()
        {
            
        }

        public string Description { get { return _description;} set { _description = value; } }
        public string ImageRelativeURL { get { return _imageRelativeURL; } set { _imageRelativeURL = value; }}
        public string ImageAbsoluteURL { get { return _imageAbsoluteURL; } set { _imageAbsoluteURL = value; }}
        public string Phone { get { return _phone; } set { _phone = value; }}
        public string Department { get { return _department; } set { _department = value; }}
        public int Position { get { return _position;} set { _position = value; } }
        public bool Visibility { get { return _visibility;} set { _visibility = value; }}

    }
}