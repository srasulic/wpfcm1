using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace wpfcm1.AlfrescoApi
{
    public class AlfrescoService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly HttpClient _client;

        public AlfrescoService(string uri)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(uri)
            };
        }

        public async Task GetNode(string ticket, string node_id)
        {
            string uri = $"/alfresco/api/-default-/public/alfresco/versions/1/nodes/{node_id}?alf_ticket={ticket}";
            using (HttpResponseMessage response = await _client.GetAsync(uri))
            {
                string responseBody = await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<bool> PostDocument(string ticket, string node_id, string filePath)
        {
            string uri = $"/alfresco/api/-default-/public/alfresco/versions/1/nodes/{node_id}/children?alf_ticket={ticket}";

            using (var form = new MultipartFormDataContent())
            {
                string filenName = Path.GetFileName(filePath);
                string[] nameParts = filenName.Split('_');

                string dms_vrstaDokumenta = "Robni-dokument";
                string dms_nodeType = "dms:dokumentLogisticki";
                string dms_description = "";
                string dms_autoRename = "true";
                string dms_yearShort = nameParts[3].Substring(2, 2);
                string dms_godina = nameParts[3].Substring(0, 4);
                string dms_obracunskiPeriod = nameParts[3].Substring(4, 2);
                string dms_sifraErp = nameParts[2];
                string dms_firmaInfoERPCode = nameParts[0];
                string dms_tip = "Izvorno elektronski";
                string dms_brojDokumenta = nameParts[2];
                string dms_podVrstaDokumenta = "OTP";
                string dms_extId = nameParts[0] + "##" + "OTP" + "-" + nameParts[2];

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var fileContent = new StreamContent(fileStream))
                {
                    //fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    form.Add(fileContent, "filedata", filenName);

                    form.Add(new StringContent(dms_description), "description");
                    form.Add(new StringContent(dms_nodeType), "nodeType");
                    form.Add(new StringContent(dms_vrstaDokumenta), "dms:vrstaDokumenta");
                    form.Add(new StringContent(dms_extId), "dms:extId");
                    form.Add(new StringContent(dms_autoRename), "autoRename");
                    form.Add(new StringContent(dms_obracunskiPeriod), "dms:obracunskiPeriod");
                    form.Add(new StringContent(dms_sifraErp), "dms:sifraErp");
                    form.Add(new StringContent(dms_firmaInfoERPCode), "dms:firmaInfoERPCode");
                    form.Add(new StringContent(dms_podVrstaDokumenta), "dms:podVrstaDokumenta");
                    form.Add(new StringContent(dms_brojDokumenta), "dms:brojDokumenta");
                    form.Add(new StringContent(dms_tip), "dms:tip");

                    using (HttpResponseMessage response = await _client.PostAsync(uri, form))
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JsonNode rootNode = JsonNode.Parse(responseBody);

                        if (!response.IsSuccessStatusCode)
                        {
                            var result = JsonSerializer.Deserialize<Error>(rootNode["error"]);
                            Log.Error($"ERROR {result.briefSummary}");
                            // 1. loguj ovde gresku
                            // 2. alfresko vraca drugaciji json u zavisnosti od hyyp response-a, pa je teze definisati povratni tip
                        }

                        return response.IsSuccessStatusCode;
                    }
                }

            }
        }

    }
}
