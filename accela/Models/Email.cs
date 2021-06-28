using System;
using System.Collections.Generic;

namespace accela.Models 
{
    public class Email
    {
        private int _id;
        private DateTime _created = DateTime.Now;
        private List<News> _news;
        private List<Dictionary<string,string>> _sendTo = new List<Dictionary<string, string>>();
        private DateTime _planToSend;
        private string _content;
        private string _formUrl;

        public Email()
        {
            _id = 0;
        }

        //Konstruktor pro načtení zpráv ze zpráv poslaných uživatelů (modal okna)
        public Email(int id, DateTime created, string content, string name, string email, string formurl = null)
        {
            _id = id;
            _created = created;
            _content = content;
            _formUrl = formurl;

            //Kdo zadal info do modálního okna
            Dictionary<string, string> user = new Dictionary<string, string>();
            user.Add(name, email);

            _sendTo.Add(user);
        }

        public List<News> News { get { return _news; } set { _news = value; } }
        public List<Dictionary<string, string>> SendTo { get { return _sendTo; } set { _sendTo = value; } }
        public DateTime PlannedToSend { get { return _planToSend; } set { _planToSend = value; } }
        public string Content { get { return _content; } set { _content = value; } }
        public int ID { get { return _id;} set { _id = value; }}
        public DateTime Created { get { return _created; } set { _created = value; }}
        public string FormURL { get { return _formUrl;} set { _formUrl = value; }}
    } 
}