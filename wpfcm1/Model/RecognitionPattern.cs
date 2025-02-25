using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wpfcm1.OlympusApi;
using wpfcm1.Settings;

namespace wpfcm1.Model
{
    public class RecognitionPattern
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum PageOrientation { Portrait, Landscape, RotatedPortrait, Undefined }

        public class Coordinates
        {
            public int x { get; set; }
            public int y { get; set; }
            public int xx { get; set; }
            public int yy { get; set; }
        }

        public class MappingElement
        {
            public Coordinates DocNumAttribute { get; set; }
            public Coordinates PibAttribute { get; set; }
            public PageOrientation pageOrientationSpecific { get; set; }
            public bool isForcedMapping { get; set; }
            public List<string> RegexList { get; set; }
        }

        public static async Task<List<MappingElement>> GetMappings (string tipDok) {

            Token authToken = OlympusService.DeserializeFromJson<Token>(User.Default.JsonToken);

            var svc = new OlympusService(User.Default.ApiURL);
            var result = await svc.GetConfigDocumentTypeMappings(authToken);
            if (result == null)
            {
                Log.Error($"FAILED GetConfigDocumentTypeMappings");
            }

            var mappingElementList = new List<MappingElement>();

            var td = result.mappings.tipDokList.Find(p => p.tipDok == tipDok);
            foreach (var m in td.mappings)
            {
                MappingElement me = new MappingElement();
                me.DocNumAttribute = new Coordinates
                {
                    x = m.doc.x1,
                    y = m.doc.y1,
                    xx = m.doc.x2,
                    yy = m.doc.y2
                };
                me.PibAttribute = new Coordinates
                {
                    x = m.pib.x1,
                    y = m.pib.y1,
                    xx = m.pib.x2,
                    yy = m.pib.y2
                };
                me.pageOrientationSpecific = (PageOrientation)Enum.Parse(typeof(PageOrientation), m.pageOrientation);
                me.isForcedMapping = m.forcedMap != "N";

                me.RegexList = m.regexList.Select(p => p.regex).ToList();

                mappingElementList.Add(me);
            }

            return mappingElementList;
        }
    }
}
