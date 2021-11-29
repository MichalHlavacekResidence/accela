using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace accela.Models.EmailModels
{
    public class EmailUsers
    {
        private int _id;
        private string _firstname;
        private string _surnamename;
        private string _email;
        private string _status;
        private string _address;
        private DateTime _created;
        private DateTime _datachanged;

        private EmailTags _emailtag;
        private List<EmailTags> _emailtags;




        public EmailUsers()
        {
            _id = 0;
        }
        public EmailUsers(int id, string firstname, string surname, string email, List<EmailTags> emailTags, string status)
        {
            _id = id;
            _firstname = firstname;
            _surnamename = surname;
            _email = email;
            _emailtags = emailTags;
            _status = status;
        }
        public EmailUsers(int id,string firstname, string surname, string email, EmailTags emailtag, string status)
        {
            _id = id;
            _firstname = firstname;
            _surnamename = surname;
            _email = email;
            _emailtag = emailtag;
            _status = status;
        }
        public EmailUsers(int id, string firstname, string surname, string email, string status)
        {
            _id = id;
            _firstname = firstname;
            _surnamename = surname;
            _email = email;
            _status = status;
        }

        //Konstruktor pro načtení zpráv ze zpráv poslaných uživatelů (modal okna)

        public int ID { get { return _id; } set { _id = value; } }
        public string Firstname { get { return _firstname; } set { _firstname = value; } }
        public string Surname { get { return _surnamename; } set { _surnamename = value; } }
        public string Email { get { return _email; } set { _email = value; } }
        public string Status { get { return _status; } set { _status = value; } }
        public string Address { get { return _address; } set { _address = value; } }
        public DateTime Created { get { return _created; } set { _created = value; } }
        public DateTime DataChanged { get { return _datachanged; } set { _datachanged = value; } }

        public EmailTags EmailTag { get { return _emailtag; } set { _emailtag = value; } }
        public List<EmailTags> EmailTags { get { return _emailtags; } set { _emailtags = value; } }

    }
}