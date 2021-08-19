using System;
using System.Collections.Generic;
using accela.Extensions;
using accela.Data;

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
        private List<Category> _poolCategories;
        private int _poolID;
        private int _contactID;
        private List<Product> _products;

        //CTOR pro pooly
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

            this._getCategoriesForPool();
        }

        //CTOR pro kategorie
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

        //CTOR pro èásteènì naètené kategorie
        public Category(int id, int poolID, string name, string url, string desc, string img, string text, int position, bool visibility, int contactID)
        {
            _id = id;
            _poolID = poolID;
            _name = name;
            _url = url;
            _description = desc;
            _image = img;
            _text = text;
            _position = position;
            _visibiliy = visibility;
            _contactID = contactID;
        }
        public Category(int id, string name, string url, string desc, string img, string text, int position, bool visibility, Manager contact, List<Product> products, List<Category> categories)
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
            _products = products;
            _poolCategories = categories;

        }

        public Category()
        {
            _id = 0;
            _contact = new Manager();
            /*_pool = new Category();
            _poolCategories = new List<Category>();*/
        }
        public Category(int id)
        {
            _id = id;
           
        }

        public int ID { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string URL { get { return _url; } set { _url = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string Image { get { return _image; } set { _image = value; } }
        public string Text { get { return _text; } set { _text = value; } }
        public bool Visibility { get { return _visibiliy; } set { _visibiliy = value; } }
        public Manager Contact { get { return _contact; } set { _contact = value; } }
        public Category Pool { get { return _pool; } set { _pool = value; } }
        public int Position { get { return _position; } set { _position = value; } }
        public List<Category> PoolCategories { get { return _poolCategories; } set { _poolCategories = value; } }
        public List<Product> Products { get { return _products; } set { _products = value; } }

        public bool CheckDetails()
        {
            bool result = true;
            if (this._name == null || this._url == null)
            {
                result = false;
            }

            return result;
        }

        public void GenerateUrl()
        {
            this._url = Slugify.URLFriendly(this._name);
        }

        private void _getCategoriesForPool()
        {
            Database db = new Database();
            this._poolCategories = db.GetCategoriesForPool(this._id);
        }
    }
}