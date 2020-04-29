using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GO_Player.Constants;

namespace GO_Player.Models
{
    public class Region
    {
        public Region(string name, string tdom, string ccshort, string cc, string lc, Uri dom, Handlers hand)
        {
            Name = name;
            TDomain = tdom;
            CountryCodeShort = ccshort;
            CountryCode = cc;
            LanguageCode = lc;
            SpecialDomain = dom;
            RegionHandler = hand;
        }

        public string Name { get; set; }
        public string TDomain { get; set; }
        public string CountryCodeShort { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public Uri SpecialDomain { get; set; }
        public Handlers RegionHandler { get; set; }
    }
}
