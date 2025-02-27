using System;
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
        public async Task<DownloadResultWithBytes> PostFilesDownload(Token token, string tenant, string fileId)
        {
            if (tenant is null || fileId is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            var jsonPayload = $"{{\"tenant\": \"{tenant}\", \"id_fajl\": \"{fileId}\"}}";
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await _client.PostAsync("olympus/v1/files/download", content))
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                JsonNode rootNode = JsonNode.Parse(responseBody);

                var downloadResult = JsonSerializer.Deserialize<DownloadResult>(rootNode);

                if (downloadResult.result.code == 0)
                {
                    byte[] fileBytes = Convert.FromBase64String(downloadResult.content);

                    var result = new DownloadResultWithBytes
                    {
                        result = downloadResult,
                        bytes = fileBytes
                    };
                    return result;
                }
                else
                {
                    var result = new DownloadResultWithBytes
                    {
                        result = downloadResult,
                        bytes = null
                    };
                    return result;
                }
            }
        }
    }
}
