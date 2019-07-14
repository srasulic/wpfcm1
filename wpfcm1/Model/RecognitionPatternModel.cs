using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using wpfcm1.Settings;

namespace wpfcm1.Model
{
    public class RecognitionPatternModel
    {
        public enum PageOrientation { Portrait, Landscape, RotatedPortrait, Undefined}
        public class Coordinates
        {
            public int x { get; set; }
            public int y { get; set; }
            public int xx { get; set; }
            public int yy { get; set; }
        }

        public class MappingElement
        {
            public Coordinates DocNumAttribute;
            public Coordinates PibAttribute;
            public PageOrientation pageOrientationSpecific;
            public int regexToApply;
            public bool isForcedMapping;

            public MappingElement(Coordinates pibAtt, Coordinates docAtt)
            {
                DocNumAttribute = new Coordinates() ;
                PibAttribute = new Coordinates() ;

                // NAPOMENA: 
                // dok ne napravimo promenu na strani servera, uvodimo slanje tipa mapiranja u kao dela broja koordinate x za PIB
                //          10000 označava mapiranje namenjano samo za portrait
                //          20000 označava mapiranje namenjano samo za landscape
                //          90000 označava mapiranje samo za portrait rotiran za 90 stepeni (čest slučaj da se landscape ovako realizuje)
                //              0 označava mapiranje koje je univerzalno za sve tipove orjentacije

                // NAPOMENA2: Za regex koji će se primeniti info šaljemo kroz x koordinatu Doc mapiranja:
                //          10000 označava korišćenje regex 1
                //          20000 označava korišćenje regex 1
                //          30000 označava korišćenje regex 1
                //          40000 označava korišćenje regex 1
                //          50000 označava korišćenje regex 1
                //          60000 označava korišćenje regex 1, 2 i 3
                //              0 označava korišćenje svih zadatih regexa

                // NAPOMENA3: kroz Y koordinatu PIBa šaljemo info da li je to mapiranje forsirano (da li se izvršava bez obzira na prethodne rezultate)
                //          90000 označava forsirano mapiranje
                //              0 označava obicno mapiranje


                int tipMap = pibAtt.x - (pibAtt.x % 10000); // izracunavamo umesto da smo ga dobili
                pibAtt.x = pibAtt.x % 10000;                       // izracunavamo upotrebljuvi deo broja koji je x koordinata (sada mu dodajemo 10,20 ili 30 hiljada da bi poslali tip mapiranja
                switch (tipMap)
                {
                    case 10000:
                        pageOrientationSpecific = PageOrientation.Portrait;
                        break;
                    case 20000:
                        pageOrientationSpecific = PageOrientation.Landscape;
                        break;
                    case 90000:
                        pageOrientationSpecific = PageOrientation.RotatedPortrait;
                        break;
                    case 0:
                        pageOrientationSpecific = PageOrientation.Undefined;
                        break;
                }

                int forcedMap = pibAtt.y - (pibAtt.y % 10000); // izracunavamo umesto da smo ga dobili
                pibAtt.y = pibAtt.y % 10000;                       // izracunavamo upotrebljuvi deo broja koji je y koordinata
                isForcedMapping = (forcedMap == 90000) ? true : false;

                int regexId = (docAtt.x - (docAtt.x % 10000));  // izračunavamo koji regex treba primeniti
                docAtt.x = docAtt.x % 10000;                     // izracunavamo upotrebljuvi deo broja koji je x koordinata
                if (regexId == 0)
                    regexToApply = 0;
                else
                    regexToApply = regexId / 10000;

                PibAttribute = pibAtt;
                DocNumAttribute = docAtt;


            }
    }
        public List<MappingElement> MappingElementList;
        public List<MappingElement> MappingElementIssuerList;

        public class DocNumRegexList {
            private string _regex1 = "";
            private string _regex2 = "";
            private string _regex3 = "";
            private string _regex4 = "";
            private string _regex5 = "";
            private string _notRecognizedString = "";
            public string Regex1 { get { return _regex1; } set { if (value == null) { _regex1 = ""; } else { _regex1 = value; } } }
            public string Regex2 { get { return _regex2; } set { if (value == null) { _regex2 = ""; } else { _regex2 = value; } } }
            public string Regex3 { get { return _regex3; } set { if (value == null) { _regex3 = ""; } else { _regex3 = value; } } }
            public string Regex4 { get { return _regex4; } set { if (value == null) { _regex4 = ""; } else { _regex4 = value; } } }
            public string Regex5 { get { return _regex5; } set { if (value == null) { _regex5 = ""; } else { _regex5 = value; } } }
            public string notRecognizedString { get { return _notRecognizedString; } set { if (value == null) { _notRecognizedString = ""; } else { _notRecognizedString = value; } } }
        }

        public Coordinates PibAttPrim { get; set; }
        public Coordinates PibAttAlt { get; set; }
        public Coordinates PibAttAlt1 { get; set; }
        public Coordinates PibAttAlt2 { get; set; }
        public Coordinates PibAttAlt4 { get; set; }
        public Coordinates PibAttAlt5 { get; set; }
        public Coordinates PibIssuer1 { get; set; }
        public Coordinates PibIssuer2 { get; set; }
        public Coordinates PibIssuer3 { get; set; }
        public Coordinates DocAttPrim { get; set; }
        public Coordinates DocAttAlt { get; set; }
        public Coordinates DocAttAlt1 { get; set; }
        public Coordinates DocAttAlt2 { get; set; }
        public Coordinates DocAttAlt4 { get; set; }
        public Coordinates DocAttAlt5 { get; set; }
        public Coordinates DocIssuer1 { get; set; }
        public Coordinates DocIssuer2 { get; set; }
        public Coordinates DocIssuer3 { get; set; }
        public DocNumRegexList DocRegexList { get; set; }
    
        public RecognitionPatternModel() {
            MappingElementIssuerList = new List<MappingElement>();
            MappingElementList = new List<MappingElement>();
            DocRegexList = new DocNumRegexList();

        }

        // web upit kojim dobijamo parametre. Ako ne uspe, uzimamo ih iz settingsa
        public void SetRecognitionPatterns (string pib, string tipDok) {
            // TODO: promeniti API da vraća JSON kao listu mapiranja pibova i kao listu mapiranja dokumenata
            // pa ovde izmeniti da se deserijalizacija uradi odmah u liste


            var uri = String.Format("{0}/index/mapping_rules?pib={1}&tip_dok={2}", User.Default.ApiURL, pib, tipDok);
            var request = WebRequest.Create(uri);
            request.Proxy = null;
            request.Method = "GET";
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        string responseFromServer = reader.ReadToEnd();
                        Console.WriteLine(responseFromServer);

                        RecognitionPatternModel r = Newtonsoft.Json.JsonConvert.DeserializeObject<RecognitionPatternModel>(responseFromServer);

                        //MappingElement map = new MappingElement(r.PibAttPrim, r.DocAttPrim);

                        this.MappingElementList.Add(new MappingElement(r.PibAttPrim, r.DocAttPrim));
                        this.MappingElementList.Add(new MappingElement(r.PibAttAlt, r.DocAttAlt));
                        this.MappingElementList.Add(new MappingElement(r.PibAttAlt1, r.DocAttAlt1));
                        this.MappingElementList.Add(new MappingElement(r.PibAttAlt2, r.DocAttAlt2));
                        this.MappingElementList.Add(new MappingElement(r.PibAttAlt4, r.DocAttAlt4));
                        this.MappingElementList.Add(new MappingElement(r.PibAttAlt5, r.DocAttAlt5));
                        if (r.PibIssuer1 != null)
                        {
                            this.MappingElementIssuerList.Add(new MappingElement(r.PibIssuer1, r.DocIssuer1));
                            this.MappingElementIssuerList.Add(new MappingElement(r.PibIssuer2, r.DocIssuer2));
                            this.MappingElementIssuerList.Add(new MappingElement(r.PibIssuer3, r.DocIssuer3));
                        } else
                        {
                            this.MappingElementIssuerList.Add(new MappingElement(new Coordinates(), new Coordinates()));
                            this.MappingElementIssuerList.Add(new MappingElement(new Coordinates(), new Coordinates()));
                            this.MappingElementIssuerList.Add(new MappingElement(new Coordinates(), new Coordinates()));
                        }

                        this.DocRegexList = r.DocRegexList;

                        reader.Close();
                    }
                }
            }
            catch
            {
              //  SetFromSettings();
            }
        }

        public void SetFromSettings()
        {
            
            //PibAttPrim.x = (int)User.Default.LlxPib;
            //PibAttPrim.y = (int)User.Default.LlyPib;
            //PibAttPrim.xx = (int)User.Default.UrxPib;
            //PibAttPrim.yy = (int)User.Default.UryPib;
            //PibAttAlt.x = (int)User.Default.LlxPib;
            //PibAttAlt.y = (int)User.Default.LlyPib;
            //PibAttAlt.xx = (int)User.Default.UrxPib;
            //PibAttAlt.yy = (int)User.Default.UryPib;

            //DocAttPrim.x = (int)User.Default.LlxNo;
            //DocAttPrim.y = (int)User.Default.LlyNo;
            //DocAttPrim.xx = (int)User.Default.UrxNo;
            //DocAttPrim.yy = (int)User.Default.UryNo;
            //DocAttAlt.x = (int)User.Default.LlxNo;
            //DocAttAlt.y = (int)User.Default.LlyNo;
            //DocAttAlt.xx = (int)User.Default.UrxNo;
            //DocAttAlt.yy = (int)User.Default.UryNo;

            //DocRegexList.regex1 = @"F[0-9][0-9][0-9][0-9][0-9][0-9][0-9]";
            //DocRegexList.regex2 = @"[0-9]{2}-[0-9]{3}-[0-9]+";
            //DocRegexList.regex3 = @"[0-9]{1,3}-[0-9]+";
            //DocRegexList.regex4 = @"DPTR[0-9]{1,3}-[0-9]+";
            //DocRegexList.regex5 = @"[A-Z]{2,4}-[0-9]{8,9}";
            //DocRegexList.notRecognizedString = @"ZAP";
        }


    }
}
