using System;
using System.Collections.Generic;
using accela.Extensions;

namespace accela.Models
{
    public class Category
    {
        private int _id;
        private string _name;
        private string _url;
        private string _description;
        private string _image;
        private string _text;
        private int _position;
        private bool _visibiliy;
        private Manager _contact;
        private Category _pool;

        public Category(int id, string name, string url, string desc, string img, string text, int position, bool visibility, Manager contact)
        {
            _id = id;
            _name = name;
            _url = url;
            _description = desc;
            _image = img;
            _text = text;
            _position = position;
            _visibiliy = visibility;
            _contact = contact;
        }

        public Category(int id, Category pid, string name, string url, string desc, string img, string text, int position, bool visibility, Manager contact)
        {
            _id = id;
            _name = name;
            _url = url;
            _description = desc;
            _image = img;
            _text = text;
            _position = position;
            _visibiliy = visibility;
            _contact = contact;
            _pool = pid;
        }

        public Category()
        {
            _id = 0;
            _contact = new Manager();
        }

        public int ID { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; }}
        public string URL { get { return _url; }set { _url = value; }}
        public string Description { get { return _description;} set { _description = value; }}
        public string Image { get { return _image;} set { _image = value;}}
        public string Text { get { return _text;} set { _text = value; }}
        public bool Visibility { get { return _visibiliy; } set { _visibiliy = value;}}
        public Manager Contact { get { return _contact; } set { _contact = value; }}
        public Category Pool { get { return _pool; } set { _pool = value; }}
        public int Position { get { return _position; } set { _position = value; }}

        public bool CheckDetails()
        {
            bool result = true;
            if(this._name == null || this._url == null)
            {
                result = false;
            }

            return result;
        }

        public void GenerateUrl()
        {
            this._url = Slugify.URLFriendly(this._name);
        }
    }
}