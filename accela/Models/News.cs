using System;
using System.Collections.Generic;
using accela.Extensions;
using accela.Data;


namespace accela.Models
{
    public class News 
    {
        private int _id;
        private string _title;
        private string _subtitle;
        private string _content;
        private string _contentSmall;
        private string _url;
        private string _videoUrl;
        private Brand _producer;
        private Manager _manager;
        private DateTime _published;
        private DateTime _created;
        private string _status;
        private List<Product> _relatedProducts;
        private List<News> _relatedNews;
        private string _imageBig;
        private string _imageSmall;
        private string _imageNew;
        private List<Tags> _tags;
        private List<Category> _categories;
        private Category _category;
        private bool _visibility;


        public News()
        {
            _id = 0;
        }

        public News(int id, string title, Brand brand, Manager contact, string ImageBig, string ImageSmall, string Content, string ContentSmall, DateTime Published, string VideoURL, string ImageNew , string Subtitle)
        {
            _id = id;
            _title = title;
            _producer = brand;
            _manager = contact;
            _imageBig = ImageBig;
            _imageSmall = ImageSmall;
            _content = Content;
            _contentSmall = ContentSmall;
            _created = Published;
            _videoUrl = VideoURL;
            _imageNew = ImageNew;
            _subtitle = Subtitle;
        }

        public News(int id, string title, Brand brand, Manager contact, string ImageBig, string ImageSmall, string Content, string ContentSmall, DateTime Created, string VideoURL, string ImageNew,List<Tags> tags, List<Product> prods)
        {
            _id = id;
            _title = title;
            _producer = brand;
            _manager = contact;
            _imageBig = ImageBig;
            _imageSmall = ImageSmall;
            _content = Content;
            _contentSmall = ContentSmall;
            _created = Created;
            _videoUrl = VideoURL;
            _imageNew = ImageNew;
            _tags = tags;
            _relatedProducts = prods;
        }
        //for homepage
        public News(int id, string title, Brand brand, string ImageBig, string ImageSmall, string ContentSmall, DateTime Created, string ImageNew, List<Tags> tags)
        {
            _id = id;
            _title = title;
            _producer = brand;
            _imageBig = ImageBig;
            _imageSmall = ImageSmall;
            _contentSmall = ContentSmall;
            _created = Created;
            _videoUrl = VideoURL;
            _imageNew = ImageNew;
            _tags = tags;
        }
        public News(int id, string title, Brand brand, string ImageBig, string ImageSmall, string ContentSmall, DateTime Created, string ImageNew, List<Tags> tags, Category category)
        {
            _id = id;
            _title = title;
            _producer = brand;
            _imageBig = ImageBig;
            _imageSmall = ImageSmall;
            _contentSmall = ContentSmall;
            _created = Created;
            _videoUrl = VideoURL;
            _imageNew = ImageNew;
            _tags = tags;
            _category = category;
        }

        public int ID { get { return _id; } set { _id = value; } }
        public string Title { get { return _title; } set { _title = value; } }
        public string Subtitle { get { return _subtitle; } set { _subtitle = value; } }
        public string Content { get { return _content; } set { _content = value; } }
        public string ContentSmall { get { return _contentSmall; } set { _contentSmall = value; } }
        public string URL { get { return _url; } set { _url = value; } }
        public string VideoURL { get { return _videoUrl; } set { _videoUrl = value; } }
        public Brand Producer { get { return _producer; } set { _producer = value; } }
        public Manager Manager { get { return _manager; } set { _manager = value; } }
        public string Status { get { return _status; } set { _status = value; } }
        public List<Product> RelatedProducts { get { return _relatedProducts; } set { _relatedProducts = value; } }
        public List<News> RelatedNews { get { return _relatedNews; } set { _relatedNews = value; } }
        public DateTime Created { get { return _created; } set { _created = value; }}
        public DateTime Published { get { return _published; } set { _published = value; }}
        public List<Tags> Tags { get { return _tags; } set { _tags = value; } }
        public List<Category> Categories { get { return _categories; } set { _categories = value; } }
        public Category Category { get { return _category; } set { _category = value; } }
        public string ImageBig {get{return _imageBig;} set{_imageBig = value;}}
        public string ImageSmall {get{return _imageSmall;} set{_imageSmall = value;}}
        public string ImageNew {get{return _imageNew;} set{_imageNew = value;}}
        public bool Visibility { get { return _visibility;}  set{_visibility = value; } }


        public void GenerateUrl()

        {
            this._url = Slugify.URLFriendly(this._title);
        }
    }
}