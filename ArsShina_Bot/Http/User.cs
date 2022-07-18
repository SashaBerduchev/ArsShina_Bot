using System;
using System.Collections.Generic;
using System.Text;

namespace ArsShina_Bot.Http
{
    class User
    {
        public User(string Name, string Last_name, string Email, string Password)
        {
            this.Name = Name;
            this.Sername = Last_name;
            this.Email = Email;
            this.Password = Password;
        }
        public string Name { get; set; }
        public string Sername { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
