using System;
using System.Collections.Generic;
using System.Text;

namespace ArsShina_Bot.Http
{
    class TiresImages
    {
        public int id { get; set; }
        public string NameTire { get; set; }
        public string TypeTire { get; set; }
        public string Articul { get; set; }
        public string TypeSeason { get; set; }
        public string ImageMimeTypeOfData { get; set; }
        public byte[] Image { get; set; } 
    }
}
