using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using accela.Structs;
using accela.Extensions;

namespace accela.Models
{
    public class Product {

        private int _id;
        private string _name;
        private string _url;
        private string _description;
        private string _subtitle;
        private string _smallDescription;
        private string _referenceLink;
        private string _image;
        private Brand _producer;
        private Category _category;
        private Manager _manager;
        private string _videoURL;
        private bool _visibility = false; //default hodnota = false
        private List<Product> _relatedProducts;
        private List<Dictionary<string, string>> _discoverMore;
        private List<Documentation> _documentation;
        private List<References> _references;
        private List<News> _news;

        public Product()
        {
            _id = 0;
            _producer = new Brand();
            _category = new Category();
            _manager = new Manager();
        }

        public Product(int id, string name, string url, string description, string subtitle, string smallDesc, string referenceLink, Manager manager, string videoUrl, bool visibility){
            _id = id;
            _name = name;
            _url = url;
            _description = description;
            _subtitle = subtitle;
            _smallDescription = smallDesc;
            _referenceLink = referenceLink;
            _manager = manager;
            _videoURL = videoUrl;
            _visibility = visibility;
        }

        //
        public Product(int id, string name, string url, string description, string subtitle, string smallDesc, string referenceLink, Brand producer, Manager manager, string videoUrl, bool visibility, string image){
            _id = id;
            _name = name;
            _url = url;
            _description = description;
            _subtitle = subtitle;
            _smallDescription = smallDesc;
            _referenceLink = referenceLink;
            _producer = producer;
            _manager = manager;
            _videoURL = videoUrl;
            _visibility = visibility;
            _image = image;
        }
        // no manager
        public Product(int id, string name, string url, string description, string subtitle, string smallDesc, string referenceLink, string videoUrl, bool visibility, string image)
        {
            _id = id;
            _name = name;
            _url = url;
            _description = description;
            _subtitle = subtitle;
            _smallDescription = smallDesc;
            _referenceLink = referenceLink;
            _videoURL = videoUrl;
            _visibility = visibility;
            _image = image;
        }
        // ReferencesProduct 
        public Product(int id, string name, string url, string subtitle, string smallDesc, string image)
        {
            _id = id;
            _name = name;
            _url = url;
            _subtitle = subtitle;
            _smallDescription = smallDesc;
            _image = image;
        }
        //Detail Products 
        public Product(int id, string name, string url, string description, string subtitle, string referenceLink, Brand producer, Manager manager, string videoUrl, string image, List<Product> relatedProducts, List<References> references, List<News> news)
        {
            _id = id;
            _name = name;
            _url = url;
            _description = description;
            _subtitle = subtitle;
            _referenceLink = referenceLink;
            _producer = producer;
            _manager = manager;
            _videoURL = videoUrl;
            _image = image;
            _relatedProducts = relatedProducts;
            _references = references;
            _news = news;
        }

        public Product(int id, string name, string url, string description, string subtitle, string referenceLink, Brand producer, Manager manager, string videoUrl, string image)
        {
            _id = id;
            _name = name;
            _url = url;
            _description = description;
            _subtitle = subtitle;
            _referenceLink = referenceLink;
            _producer = producer;
            _manager = manager;
            _videoURL = videoUrl;
            _image = image;
        }



        public int ID { get { return _id;} set { _id = value; }}
        public string Name { get { return _name;} set { _name = value;}}
        public string URL { get { return _url;} set { _url = value; }}
        public string Description { get { return _description; } set { _description = value; }}
        public string Subtitle { get { return _subtitle;} set { _subtitle = value; }}
        public string SmallDescription { get { return _smallDescription;} set { _smallDescription = value; }}
        public string ReferenceLink { get { return _referenceLink;} set { _referenceLink = value; }}
        public string Image { get { return _image; } set { _image = value; }}
        public Brand Producer { get { return _producer; } set { _producer = value; } }
        public Manager Manager { get { return _manager; } set { _manager = value; } }
        public Category Category { get { return _category; } set { _category = value; } }
        public string VideoURL { get { return _videoURL;} set { _videoURL = value; }}
        public bool Visibility { get { return _visibility; } set { _visibility = value; }}  
        public List<Product> RelatedProducts { get { return _relatedProducts; } set { _relatedProducts = value; } }
        public List<Dictionary<string, string>> DiscoverMore { get { return _discoverMore; } set { _discoverMore = value; } }
        public List<Documentation> Documentations { get { return _documentation; } set { _documentation = value; }}
        public List<References> References { get { return _references; } set { _references = value; } }
        public List<News> News { get { return _news; } set { _news = value; } }

        public void GenerateUrl(){
            this._url = Slugify.URLFriendly(this._name);
        }

        public bool CheckDetails(bool checkID = true){
            bool result = true;
            if(checkID == true){
                if(this._id == 0){
                    result = false;
                }
            }

            //Povinné položky
            if(this._name == null || this._url == null || this._description == null || this._referenceLink == null){
                result = false;
            }

            //Pokud není nastaven producer, zkontroluj custom manažera
            if(this._producer.ID == 0){
                /*if(this._manager.ID == 0 ){
                    result = false;
                }*/
                result = false;
            }
            return result;
        }
    }
}
