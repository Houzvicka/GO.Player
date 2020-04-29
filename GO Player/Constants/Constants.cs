using GO_Player.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO_Player
{
    public static class Constants
    {
        public enum Handlers {
            EU = 0,
            NORDIC = 1,
            SPAIN = 1,
            US = 2,
            LATIN_AMERICA = 3,
            ASIA = 4
        }

        public enum Platforms
        {
            ANTA,
            ANTV,
            APMO,
            APTA,
            APTV,
            BRMO,
            BRTA,
            CHBR,
            COMP,
            COTV,
            CSAT,
            DASH,
            EDBR,
            FFBR,
            GOCT,
            IEBR,
            LGNC,
            LGWO,
            MOBI,
            PLS3,
            PLS4,
            PS4P,
            PLSP,
            SABR,
            SAOR,
            SATI,
            SERV,
            SETX,
            SFBR,
            TABL,
            TVHI,
            TVLO,
            XBOX,
            XONE
        }

        public static List<Region> Countries = new List<Region>()
        { 
            new Region("Bosnia and Herzegovina", "ba", "ba", "BIH", "HRV", null, Handlers.EU),
            new Region("Bulgaria", "bg", "bg", "BGR", "BUL", null, Handlers.EU),
            new Region("Croatia", "hr", "hr", "HRV", "HRV", null, Handlers.EU),
            new Region("Czech Republic", "cz", "cz", "CZE", "CES", null, Handlers.EU),
            new Region("Denmark", "dk", "dk", "DNK", "da_hbon", new Uri("https://dk.hbonordic.com/"), Handlers.NORDIC),
            new Region("Finland", "fi", "fi", "FIN", "fi_hbon", new Uri("https://fi.hbonordic.com/"), Handlers.NORDIC),
            new Region("Hungary", "hu", "hu", "HUN", "HUN", null, Handlers.EU),
            new Region("Macedonia", "mk", "mk", "MKD", "MKD", null, Handlers.EU),
            new Region("Montenegro", "me", "me", "MNE", "SRP", null, Handlers.EU),
            new Region("Norway", "no", "no", "NOR", "no_hbon", new Uri("https://no.hbonordic.com/"), Handlers.NORDIC),
            new Region("Poland", "pl", "pl", "POL", "POL", null, Handlers.EU),
            new Region("Portugal", "pt", "pt", "PRT", "POR", new Uri("https://hboportugal.com"), Handlers.EU),
            new Region("Romania", "ro", "ro", "ROU", "RON", null, Handlers.EU),
            new Region("Serbia", "rs", "sr", "SRB", "SRP", null, Handlers.EU),
            new Region("Slovakia", "sk", "sk", "SVK", "SLO", null, Handlers.EU),
            new Region("Slovenija", "si", "si", "SVN", "SLV", null, Handlers.EU),
            new Region("Spain", "es", "es", "ESP", "es_hboespana", new Uri("https://es.hboespana.com"), Handlers.SPAIN),
            new Region("Sweden", "se", "se", "SWE", "sv_hbon", new Uri("https://se.hbonordic.com/"), Handlers.NORDIC),
        };

        public static string SkylinkID = "c55e69f0-2471-46a9-a8b7-24dac54e6eb9";

        public static Dictionary<string, string> eu_redirect_login = new Dictionary<string, string>();
    }
}
