using System;
using System.Collections.Generic;
using System.Diagnostics;
using accela.Models;
using accela.Extensions;
using accela.Data;

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
        private string _imageRelativeURL;
        private List<Product> _brandProducts;

        public Brand()
        {
            _id = 0;
            _contact = new Manager();
            _brandProducts = new List<Product>();            
        }

        public Brand(int id, string name, string link, string url, string desc, string smallt, Manager contact, int position, bool vis, string img)
        {
            _id = id;
            _name = name;
            _link = link;
            _url = url;
            _description = desc;
            _smallText = smallt;
            _contact = contact;
            _position = position;
            _visibility = vis;
            _imageRelativeURL = img;
            _brandProducts = new List<Product>();
        }

        public int ID { get { return _id;} set { _id = value; } }
        public string Name { get { return _name;} set { _name = value; } }
        public string ReferenceLink { get{ return _link;} set { _link = value; } }
        public string URL { get { return _url;} set { _url = value; }}
        public string Description { get { return _description;} set { _description = value; }}
        public string SmallText { get { return _smallText;} set {_smallText = value; }}
        public Manager Contact { get { return _contact;} set { _contact = value; }}
        public int Position { get { return _position;} set { _position = value; }}
        public bool Visibility { get { return _visibility;} set { _visibility = value; }}
        public string ImageRelativeURL { get { return _imageRelativeURL;} set { _imageRelativeURL = value; }}
        public List<Product> Products { get { return _brandProducts;} set { _brandProducts = value;}}

        ///
        ///    <summary>
        ///         Funkce pro kontrolu vyplnění atributů Name, Link, URL a Contact.ID (Manažer).
        ///    </summary>
        ///    <returns>
        ///         Boolean hodnota.
        ///    </returns>
        ///
        public bool CheckDetails(){
            bool result = true;

            if(this._name == null || this._link == null || this._url == null){
                result = false;
            }
            if(this._contact.ID == 0){
                result = false;
            }

            return result;
        }

        ///
        ///<summary>
        ///     Funkce pro načtení produktů pro tuto firmu.     
        ///</summary>
        ///<returns>
        ///     Funkce nevrací nic, pouze načítá interně pole (List[Product]) do 
        ///     atributu Products.
        ///</returns>
        ///
        public void LoadProducts()
        {
            Database db = new Database();
            this._brandProducts = db.GetProductsForBrand(this._id);
        }

        ///
        ///    <summary>
        ///         Funkce pro vytvoření URL z atributu Name (pomocí funkce Slugify).
        ///    </summary>
        ///    <returns>
        ///         Funkce je typu Void a nevrací žádnou hodnotu. Výsledkem funkce je zapsání hodnoty do atributu URL.
        ///    </returns>
        ///
        public void GenerateUrl(){
            this._url = Slugify.URLFriendly(this._name);
        }
    } 
}