using System;
using System.Collections.Generic;

namespace accela.Models
{
    public class Mails
    {
        private int _id;
        private string _from;
        private string _to;
        private string _content;
        private bool _visibility;

        public Mails()
        {
            _id = 0;
        }
        public Mails(int id, string from, string to, string content)
        {
            _id = id;
            _from = from;
            _to = to;
            _content = content;
        }
        public int ID { get { return _id; } set { _id = value; } }
        public string From { get { return _from; } set { _from = value; } }
        public string To { get { return _to; } set { _to = value; } }
        public string Content { get { return _content; } set { _content = value; } }
        public bool Visibility { get { return _visibility; } set { _visibility = value; } }
    }
   
}
