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
        public class PibAttributes
        {
            public int x { get; set; }
            public int y { get; set; }
            public int xx { get; set; }
            public int yy { get; set; }
        }
        public class PibAttributesPrim : PibAttributes
        {
        }
        public class PibAttributesAlt : PibAttributes
        {
        }
        public class DocNumAttributes
        {
            public int x { get; set; }
            public int y { get; set; }
            public int xx { get; set; }
            public int yy { get; set; }
        }
        public class DocNumAttributesPrim : DocNumAttributes
        {

        }
        public class DocNumAttributesAlt : DocNumAttributes
        {

        }
        
        public class DocNumRegexList {
            private string _regex1 = "";
            private string _regex2 = "";
            private string _regex3 = "";
            private string _regex4 = "";
            private string _regex5 = "";
            private string _notRecognizedString = "";
            public string regex1 { get { return _regex1; } set { if (value == null) { _regex1 = ""; } else { _regex1 = value; } } }
            public string regex2 { get { return _regex2; } set { if (value == null) { _regex2 = ""; } else { _regex2 = value; } } }
            public string regex3 { get { return _regex3; } set { if (value == null) { _regex3 = ""; } else { _regex3 = value; } } }
            public string regex4 { get { return _regex4; } set { if (value == null) { _regex4 = ""; } else { _regex4 = value; } } }
            public string regex5 { get { return _regex5; } set { if (value == null) { _regex5 = ""; } else { _regex5 = value; } } }
            public string notRecognizedString { get { return _notRecognizedString; } set { if (value == null) { _notRecognizedString = ""; } else { _notRecognizedString = value; } } }
        }

        public PibAttributesPrim PibAttPrim { get; set; }
        public PibAttributesAlt PibAttAlt { get; set; }
        public PibAttributesAlt PibAttAlt1 { get; set; }
        public PibAttributesAlt PibAttAlt2 { get; set; }
        public PibAttributesAlt PibAttAlt4 { get; set; }
        public PibAttributesAlt PibAttAlt5 { get; set; }
        public DocNumAttributesPrim DocAttPrim { get; set; }
        public DocNumAttributesAlt DocAttAlt { get; set; }
        public DocNumAttributesAlt DocAttAlt1 { get; set; }
        public DocNumAttributesAlt DocAttAlt2 { get; set; }
        public DocNumAttributesAlt DocAttAlt4 { get; set; }
        public DocNumAttributesAlt DocAttAlt5 { get; set; }
        public DocNumRegexList DocRegexList { get; set; }
    
        public RecognitionPatternModel() {
            PibAttPrim = new PibAttributesPrim();
            PibAttAlt = new PibAttributesAlt();
            DocAttPrim = new DocNumAttributesPrim();
            DocAttAlt = new DocNumAttributesAlt();
            DocRegexList = new DocNumRegexList();

        }

        // web upit kojim dobijamo parametre. Ako ne uspe, uzimamo ih iz settingsa
        public void SetRecognitionPatterns (string pib, string tipDok) {

            var uri = String.Format("https://edokument.aserta.rs/index/mapping_rules?pib={0}&tip_dok={1}", pib, tipDok);
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
                        this.PibAttPrim = r.PibAttPrim;
                        this.PibAttAlt = r.PibAttAlt;
                        this.PibAttAlt1 = r.PibAttAlt1;
                        this.PibAttAlt2 = r.PibAttAlt2;
                        this.PibAttAlt4 = r.PibAttAlt4;
                        this.PibAttAlt5 = r.PibAttAlt5;
                        this.DocAttPrim = r.DocAttPrim;
                        this.DocAttAlt = r.DocAttAlt;
                        this.DocAttAlt1 = r.DocAttAlt1;
                        this.DocAttAlt2 = r.DocAttAlt2;
                        this.DocAttAlt4 = r.DocAttAlt4;
                        this.DocAttAlt5 = r.DocAttAlt5;
                        this.DocRegexList = r.DocRegexList;

                        reader.Close();
                    }
                }
            }
            catch
            {
                SetFromSettings();
            }
        }

        public void SetFromSettings()
        {
            
            PibAttPrim.x = (int)User.Default.LlxPib;
            PibAttPrim.y = (int)User.Default.LlyPib;
            PibAttPrim.xx = (int)User.Default.UrxPib;
            PibAttPrim.yy = (int)User.Default.UryPib;
            PibAttAlt.x = (int)User.Default.LlxPib;
            PibAttAlt.y = (int)User.Default.LlyPib;
            PibAttAlt.xx = (int)User.Default.UrxPib;
            PibAttAlt.yy = (int)User.Default.UryPib;

            DocAttPrim.x = (int)User.Default.LlxNo;
            DocAttPrim.y = (int)User.Default.LlyNo;
            DocAttPrim.xx = (int)User.Default.UrxNo;
            DocAttPrim.yy = (int)User.Default.UryNo;
            DocAttAlt.x = (int)User.Default.LlxNo;
            DocAttAlt.y = (int)User.Default.LlyNo;
            DocAttAlt.xx = (int)User.Default.UrxNo;
            DocAttAlt.yy = (int)User.Default.UryNo;

            DocRegexList.regex1 = @"F[0-9][0-9][0-9][0-9][0-9][0-9][0-9]";
            DocRegexList.regex2 = @"[0-9]{2}-[0-9]{3}-[0-9]+";
            DocRegexList.regex3 = @"[0-9]{1,3}-[0-9]+";
            DocRegexList.regex4 = @"DPTR[0-9]{1,3}-[0-9]+";
            DocRegexList.regex5 = @"[A-Z]{2,4}-[0-9]{8,9}";
            DocRegexList.notRecognizedString = @"ZAP";
        }


    }
}
