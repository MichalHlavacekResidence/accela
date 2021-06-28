using System;
using System.Collections.Generic;
using System.Diagnostics;
using accela.Models;
using accela.Data;

namespace accela.Models
{
    public class Manager : User 
    {
        private string _description;
        private string _imageRelativeURL;
        private string _imageAbsoluteURL;
        private string _phone;
        private int _position;
        private bool _visibility;
        private List<Product> _products;

        private Department _department;

        public Manager()
        {
            this.ID = 0;
            this._products = new List<Product>();
        }

        //Načtení do tabulky (admin/managers)
        public Manager(int id, string fname, string lname, string email, Department department, string phone, string description, string img, bool visibility, int position)
        {
            this.ID = id;
            this.Firstname = fname;
            this.Lastname = lname;
            this.Email = email;
            this._phone = phone;
            this._description = description;
            this._imageRelativeURL = img;
            this._visibility = visibility;
            this._position = position;
            this._department = department;
            this._products = new List<Product>();
        }
        
        //Načtení pro oddělení
        public Manager(int id, string fname, string lname, string email, string phone, string description, string img, bool visibility, int position)
        {
            this.ID = id;
            this.Firstname = fname;
            this.Lastname = lname;
            this.Email = email;
            this._phone = phone;
            this._description = description;
            this._imageRelativeURL = img;
            this._visibility = visibility;
            this._position = position;
            this._products = new List<Product>();
        }

        public string Description { get { return _description;} set { _description = value; } }
        public string ImageRelativeURL { get { return _imageRelativeURL; } set { _imageRelativeURL = value; }}
        public string ImageAbsoluteURL { get { return _imageAbsoluteURL; } set { _imageAbsoluteURL = value; }}
        public string Phone { get { return _phone; } set { _phone = value; }}
        public Department Department { get { return _department; } set { _department = value; }}
        public int Position { get { return _position;} set { _position = value; } }
        public bool Visibility { get { return _visibility;} set { _visibility = value; }}
        public List<Product> Products { get { return _products; } set { _products = value; }}

        public bool CheckDetails(){
            bool result = true;

            if(this.Firstname == null || this.Lastname == null || this.Email == null){
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
            this._products = db.GetProductsForManager(this.ID);
        }

    }
}