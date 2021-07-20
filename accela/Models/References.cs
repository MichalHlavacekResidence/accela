using accela.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace accela.Models
{
    public class References
    {

        private int _id;
        private string _name;
        private string _company;
        private string _img;
        private bool _visibility;
        private int _position;
        private string _description;
       

        public References()
        {
            _id = 0;
        }

        public References(int id, string name, string company, string img, int position, bool vis, string desc)
        {
            _id = id;
            _name = name;
            _company = company;
            _img = img;
            _position = position;
            _visibility = vis;
            _description = desc;
        }

        public int ID { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Company { get { return _company; } set { _company = value; } }
        public string Img { get { return _img; } set { _img = value; } }
        public int Position { get { return _position; } set { _position = value; } }
        public bool Visibility { get { return _visibility; } set { _visibility = value; } }
        public string Description { get { return _description; } set { _description = value; } }


        ///
        ///    <summary>
        ///         Funkce pro kontrolu vyplnění atributů Name, Link, URL a Contact.ID (Manažer).
        ///    </summary>
        ///    <returns>
        ///         Boolean hodnota.
        ///    </returns>
        ///
        public bool CheckDetails()
        {
            bool result = true;

            /*if (this._name == null || this._link == null || this._url == null)
            {
                result = false;
            }
            if (this._contact.ID == 0)
            {
                result = false;
            }*/

            return result;
        }
    }
}
