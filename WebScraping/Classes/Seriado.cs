using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping.Classes
{
    public class Seriado
    {
        public string serie { get; set; }
        public string urlserie { get; set; }
        public int temporada { get; set; }
        public int episodio { get; set; }
        public string data { get; set; }
        public string ImdbId { get; set; }
    }
}
