using System;
using System.Collections.Generic;

namespace accela.Models
{
    public class Tags
    {
        private int _id;
        private string _name;
        private string _url;
        private int _position;
        private bool _visibility;

        public Tags()
        {
            _id = 0;
        }
        public Tags(int id, string name, string url, int position, bool visibility)
        {
            _id = id;
            _name = name;
            _url = url;
            _position = position;
            _visibility = visibility;
        }
        public int ID { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Url { get { return _url; } set { _url = value; } }
        public int Position { get { return _position; } set { _position = value; } }
        public bool Visibility { get { return _visibility; } set { _visibility = value; } }
    }
   
}
