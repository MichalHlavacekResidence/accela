using System;
using System.Collections.Generic;
using MySqlConnector;

namespace accela.Data 
{
    public class AppDb : IDisposable 
    {
        public readonly MySqlConnection Connection;
        public AppDb()
        {
            Connection = new MySqlConnection("host=127.0.0.1;port=3306;user id=accela;password=mysql-123-heslo;database=Accela");
            //Connection = new MySqlConnection("host=mysql;port=3306;user id=root;password=mysql-123-heslo;database=Accela");

        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}