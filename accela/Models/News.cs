using System;
using System.Collections.Generic;

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
        private DateTime _planToPublish;
        private DateTime _created;
        private string _status;
        private List<Product> _relatedProducts;
        private List<News> _relatedNews;
        private string _imageBig;
        private string _imageSmall;
        private string _imageNew;

        public News()
        {
            _id = 0;
        }

        public News(int id, string title, Brand brand, Manager contact, string ImageBig, string ImageSmall, string Content, string ContentSmall, DateTime Created, string VideoURL, string ImageNew)
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
        }

        public int ID { get { return _id; } set { _id = value; } }
        public string Title { get { return _title; } set { _title = value; } }
        public string Subtitle { get { return _subtitle; } set { _subtitle = value; } }
        public string Content { get { return _content; } set { _content = value; } }
        public string URL { get { return _url; } set { _url = value; } }
        public string VideoURL { get { return _videoUrl; } set { _videoUrl = value; } }
        public Brand Producer { get { return _producer; } set { _producer = value; } }
        public Manager Manager { get { return _manager; } set { _manager = value; } }
        public DateTime PlannedToPublish { get { return _planToPublish; } set { _planToPublish = value; } }
        public string Status { get { return _status; } set { _status = value; } }
        public List<Product> RelatedProducts { get { return _relatedProducts; } set { _relatedProducts = value; } }
        public List<News> RelatedNews { get { return _relatedNews; } set { _relatedNews = value; } }
        public DateTime Created { get { return _created; } set { _created = value; }}
    }
}