using System;
using System.Collections.Generic;

namespace accela.Models
{
    public class SystemMessage
    {
        private string _title;
        private string _message;
        private string _status;
        private string _color;
        private bool _result;

        public SystemMessage(string title, string message, string status)
        {
            _title = title;
            _message = message;
            _status = status;    

            this._setResult();
        }

        private void _setResult(){
            string lowStatus = this._status.ToLower();

            switch(lowStatus){
                case "ok":
                case "done":
                case "true":
                    this._result = true;
                    break;
                
                case "error":
                case "warning":
                case "warn":
                case "err":
                    this._result = false;
                    break;
            }
        }

        public string Title { get { return _title; } set { _title = value; } }
        public string Message { get { return _message; } set { _message = value; } }
        public string Status { get { return _status; } set { _status = value; } }
        public string Color { get { return _color; } set { _color = value; } }
        public bool Result { get { return _result; } set { _result = value; } }
    }
}