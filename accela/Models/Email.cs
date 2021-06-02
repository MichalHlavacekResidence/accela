using System;
using System.Collections.Generic;

namespace accela.Models 
{
    public class Email
    {
        private List<News> _news;
        private List<Dictionary<string,string>> _sendTo;
        private DateTime _planToSend;
        private string _content;

        public Email()
        {
            
        }

        public List<News> News { get { return _news; } set { _news = value; } }
        public List<Dictionary<string, string>> SendTo { get { return _sendTo; } set { _sendTo = value; } }
        public DateTime PlannedToSend { get { return _planToSend; } set { _planToSend = value; } }
        public string Content { get { return _content; } set { _content = value; } }
    } 
}