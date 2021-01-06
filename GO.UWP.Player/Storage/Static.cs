using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO.UWP.Player.Static
{
    public class CountryItem
    {
        public CountryItem(string name, string natDom, string ccShort, string ccLong, string langCode, Uri specialDomain)
        {
            Name = name;
            NationalDomain = natDom;
            CountryCodeShort = ccShort;
            CountryCodeLong = ccLong;
            LanguageCode = langCode;
            SpecialDomain = specialDomain;
        }

        public string Name { get; set; }
        public string NationalDomain { get; set; }
        public string CountryCodeShort { get; set; }
        public string CountryCodeLong { get; set; }
        public string LanguageCode { get; set; }
        public Uri SpecialDomain { get; set; }
    }

    public class Static
    {
        public Uri FallbackOperatorIcon => new Uri("https://www.hbo-europe.com/images/hbo_eu_logo.png");
        public Guid SkylinkID = new Guid("c55e69f0-2471-46a9-a8b7-24dac54e6eb9");  // Skylink Operator ID, Skylink require special steps in login
        public Guid FallbackCK = new Guid("10727db2-602e-4988-9a9b-e7dc57af795e");
        
        // supported countries:
        //   1 name
        //   2 national domain
        //   3 country code short
        //   4 country code long
        //   5 default language code
        //   6 special domain

        public static List<CountryItem> CountriesList => new List<CountryItem>
            {
                new CountryItem("Bosnia and Herzegovina", "ba", "ba", "BIH", "HRV", null),
                new CountryItem("Bulgaria", "bg", "bg", "BGR", "BUL", null),
                new CountryItem("Croatia", "hr", "hr", "HRV", "HRV", null),
                new CountryItem("Czech Republic", "cz", "cz", "CZE", "CES", null),
                //new CountryItem("Denmark", "dk", "dk", "DNK", "da_hbon", new Uri("https://dk.hbonordic.com/")),
                //new CountryItem("Finland", "fi", "fi", "FIN", "fi_hbon", new Uri("https://fi.hbonordic.com/")),
                new CountryItem("Hungary", "hu", "hu", "HUN", "HUN", null),
                new CountryItem("Macedonia", "mk", "mk", "MKD", "MKD", null),
                new CountryItem("Montenegro", "me", "me", "MNE", "SRP", null),
                //new CountryItem("Norway", "no", "no", "NOR", "no_hbon", new Uri("https://no.hbonordic.com/")),
                new CountryItem("Poland", "pl", "pl", "POL", "POL", null),
                //new CountryItem("Portugal", "pt", "pt", "PRT", "POR", new Uri("https://hboportugal.com")),
                new CountryItem("Romania", "ro", "ro", "ROU", "RON", null),
                new CountryItem("Serbia", "rs", "sr", "SRB", "SRP", null),
                new CountryItem("Slovakia", "sk", "sk", "SVK", "SLO", null),
                new CountryItem("Slovenija", "si", "si", "SVN", "SLV", null),
                //new CountryItem("Spain", "es", "es", "ESP", "es_hboespana", new Uri("https://es.hboespana.com")),
                //new CountryItem("Sweden", "se", "se", "SWE", "sv_hbon", new Uri("https://se.hbonordic.com/"))
            };

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

        // Special data for OAUTH that require custom actions
        // each entry is one operator
        // each operator has:
        // id: the id of the operator for easy retrieval
        // confirm_uri: the URI that is called at a 2nd callback for auth success
        // payload: the data to send in this 2nd request
        public string SpecialData = $@"
        {{
           ""telekom_ro"":
           {{
               ""id"":""972706fe-094c-4ea5-ae98-e8c5d907f6a2"",
               ""confirm_uri"":""https://my.telekom.ro/oauth2/rest/approval"",
               ""payload"":
                           {{
                               ""state"": None,
                               ""act"": 1,
                           }}
           }}
        }}";

        public static List<Tuple<Guid, Tuple<Uri, string, string, string>>> EuRedirectLogin =
            new List<Tuple<Guid, Tuple<Uri, string, string, string>>>() // 0 - operator website login form url, 1 - username field name, 2 - password field name, 3 form payload
                {
                    new Tuple<Guid, Tuple<Uri, string, string, string>>(new Guid("c55e69f0-2471-46a9-a8b7-24dac54e6eb9"), new Tuple<Uri, string, string, string>(new Uri("https://hbogo.skylink.cz/goauthenticate.aspx?client_id=HBO&redirect_uri=https%3a%2f%2fczapi.hbogo.eu%2foauthskylink%2frequest2.aspx&state=5zveHRYBaocYXvjTxHozRg&scope=HBO&response_type=code"), "txtLogin", "txtPassword", "\"__LASTFOCUS\": None, \"__EVENTTARGET\": \"btnSubmit\", \"__EVENTARGUMENT\": None, \"__VIEWSTATE\": None, \"__VIEWSTATEGENERATOR\": None, \"txtLogin\": None, \"txtPassword\": None")), // Czech Republic: Skylink + Slovakia: Skylink
                    new Tuple<Guid, Tuple<Uri, string, string, string>>(new Guid("f0e09ddb-1286-4ade-bb30-99bf1ade7cff"), new Tuple<Uri, string, string, string>(new Uri("https://service.upc.cz/pkmslogin.form"), "username", "password", "\"login-form-type\": \"pwd\", \"username\": None, \"password\": None")), // Czech Republic: UPC CZ + Slovakia: UPC CZ
                    new Tuple<Guid, Tuple<Uri, string, string, string>>(new Guid("414847a0-635c-4587-8076-079e3aa96035"), new Tuple<Uri, string, string, string>(new Uri("https://icok.cyfrowypolsat.pl/logowanie.cp"), "j_username", "j_password", "\"j_username\": None, \"j_password\": None, \"loginFormM_SUBMIT\": \"1\", \"sInBtn\": \"\", \"javax.faces.ViewState\": \"\"")), // Polonia: Cyfrowy Polsat
                    new Tuple<Guid, Tuple<Uri, string, string, string>>(new Guid("972706fe-094c-4ea5-ae98-e8c5d907f6a2"), new Tuple<Uri, string, string, string>(new Uri("https://my.telekom.ro/oam/server/auth_cred_submit"), "username", "password", "\"username\": None, \"password\": None")), // Romania: Telekom Romania (My Account)
                    new Tuple<Guid, Tuple<Uri, string, string, string>>(new Guid("41a660dc-ee15-4125-8e92-cdb8c2602c5d"), new Tuple<Uri, string, string, string>(new Uri("https://www.upc.ro/rest/v40/session/start?protocol=oidc&rememberMe=false"), "username", "credential", "\"username\": None, \"credential\": None")), // Romania: UPC Romania
                };
    }
}
