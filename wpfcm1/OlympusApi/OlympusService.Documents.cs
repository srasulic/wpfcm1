using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace wpfcm1.OlympusApi
{
    partial class OlympusService
    {
        public async Task<Result> PostDocumentsUpload(TipDokPristup tdp, Token token, string tenant, string filePath)
        {
            if (tdp is null || token is null || tenant is null || filePath is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            string base64Content = Convert.ToBase64String(File.ReadAllBytes(filePath));
            var payload = new Payload { tenant = tenant, tip_dok = tdp.tip_dok, teh_naziv_fajla = Path.GetFileName(filePath), sadrzaj = base64Content };
            var jsonPayload = JsonSerializer.Serialize(payload);

            string uri = $"/olympus/v1/documents/upload_{tdp.smer}";
            using (StringContent jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
            using (HttpResponseMessage response = await _client.PostAsync(uri, jsonContent))
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                JsonNode rootNode = JsonNode.Parse(responseBody);

                var result = JsonSerializer.Deserialize<Result>(rootNode["result"]);
                return result;
            }
        }

        public async Task<DocumentsResult> GetDocuments(TipDokPristup tdp, Token token, string tenant, string sinceDate)
        {
            if (tdp is null || token is null || tenant is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            string uri = $"/olympus/v1/documents/{tdp.smer}?tenant={tenant}&tip_dok={tdp.tip_dok}&edok_status=SS_PEND&created_since={sinceDate}&start_index=0&page_size=1000";
            using (HttpResponseMessage response = await _client.GetAsync(uri))
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                JsonNode rootNode = JsonNode.Parse(responseBody);

                var docs = JsonSerializer.Deserialize<DocumentsResult>(rootNode);
                return docs;
            }
        }

    }
}
