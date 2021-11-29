using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace accela.Models.EmailModels
{
    public class EmailTags
    {
        private int _id;
        private string _name;
        private bool _visibility;

        public EmailTags()
        {
            _id = 0;
        }
        public EmailTags(int id,string name, bool visibility)
        {
            _id = id;
            _name = name;
            _visibility = visibility;
        }
        public EmailTags(int id, string name)
        {
            _id = id;
            _name = name;
        }

        //Konstruktor pro načtení zpráv ze zpráv poslaných uživatelů (modal okna)

        public int ID { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public bool Visibility { get { return _visibility; } set { _visibility = value; } }

    }
}