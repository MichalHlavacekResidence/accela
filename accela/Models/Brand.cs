using System;
using System.Collections.Generic;
using System.Diagnostics;
using accela.Models;

namespace accela.Models 
{
    public class Brand 
    {

        private int _id;
        private string _name;
        private string _link;
        private string _url;
        private string _description;
        private string _smallText;
        private Manager _contact;
        private int _position;
        private bool _visibility;

        public Brand()
        {
            _id = 0;
        }

        public int ID { get { return _id;} set { _id = value; } }
        public string Name { get { return _name;} set { _name = value; } }
        public string Link { get{ return _link;} set { _link = value; } }
        public string URL { get { return _url;} set { _url = value; }}
        public string Description { get { return _description;} set { _description = value; }}
        public string SmallText { get { return _smallText;} set {_smallText = value; }}
        public Manager Contact { get { return _contact;} set { _contact = value; }}
        public int Position { get { return _position;} set { _position = value; }}
        public bool Visibility { get { return _visibility;} set { _visibility = value; }}
    }
}