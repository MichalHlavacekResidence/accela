using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Models
{
    public class Product {

        private int _id;
        private string _name;
        private string _url;
        private string _description;
        private string _subtitle;
        private string _smallDescription;
        private string _referenceLink;
        private Brand _producer;
        private string _videoURL;
        private bool _visibility;

        public Product()
        {
            
        }

        public int ID { get { return _id;} set { _id = value; }}
        public string Name { get { return _name;} set { _name = value;}}
        public string URL { get { return _url;} set { _url = value; }}
        public string Description { get { return _description; } set { _description = value; }}
        public string Subtitle { get { return _subtitle;} set { _subtitle = value; }}
        public string SmallDescription { get { return _smallDescription;} set { _smallDescription = value; }}
        public string ReferenceLink { get { return _referenceLink;} set { _referenceLink = value; }}
        public Brand Producer { get { return _producer; } set { _producer = value; }}
        public string VideoURL { get { return _videoURL;} set { _videoURL = value; }}
        public bool Visibility { get { return _visibility; } set { _visibility = value; }}  
    }
}
