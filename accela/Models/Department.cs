using System;
using System.Collections.Generic;
using System.Diagnostics;
using accela.Extensions;

namespace accela.Models
{
    public class Department
    {
        private int _id;
        private string _name;
        private string _url;
        private bool _visibility;
        private int _position;

        public Department()
        {
            _id = 0;
        }

        public Department(int id, string name, string url, bool visibility, int position){
            _id = id;
            _name = name;
            _url = url;
            _visibility = visibility;
            _position = position;
        }

        public void GenerateUrl(){
            this._url = Slugify.URLFriendly(this._name);
        }

        public bool CheckDetails(){
            if(this._name == null || this._name == "" || this._url == "" || this._url == null){
                return false;
            }
            return true;
        }

        public int ID { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value ;} }
        public string URL { get { return _url; } set { _url = value; } }
        public bool Visibility { get { return _visibility; } set { _visibility = value; } }
        public int Position { get { return _position; } set { _position = value; } }
    }
}