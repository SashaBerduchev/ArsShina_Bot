using System;
using System.Collections.Generic;
using System.Text;

namespace ArsShina_Bot.Http
{
    class User
    {
        public User(string Name, string Last_name)
        {
            this.Name = Name;
            this.LastName = Last_name;
        }
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}
