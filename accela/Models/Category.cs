using System;
using System.Collections.Generic;

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
    }
}