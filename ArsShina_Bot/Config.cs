using System;
using System.Collections.Generic;
using System.Text;

namespace ArsShina_Bot
{
    class Config
    {
        private static string connstring;
        public static bool DEBUG_MODE = true;
        public static string GetString()
        {
            if (DEBUG_MODE == true)
            {
                connstring = "https://localhost:44314/";
            }
            else
            {
                connstring = "http://arsshina.com/";
            }
            return connstring;
        }

    }
}
