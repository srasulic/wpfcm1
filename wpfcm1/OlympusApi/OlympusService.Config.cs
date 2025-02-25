using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace wpfcm1.OlympusApi
{
    partial class OlympusService
    {
        public async Task<ProfileResult> GetConfigPolisign(Token token, Tenant tenant)
        {
            if (token is null || tenant is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            string uri = $"/olympus/v1/config/polisign?tenant={tenant.tenant}";
            HttpResponseMessage response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                JsonNode rootNode = JsonNode.Parse(responseBody);

                var profile = JsonSerializer.Deserialize<ProfileResult>(rootNode);
                return profile;
            }
            else
            {
                Log.Error(response.ReasonPhrase);
                return null;
            }
        }

        public async Task<DocumentTypeMappingsResult> GetConfigDocumentTypeMappings(Token token)
        {
            if (token is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            string uri = $"/olympus/v1/config/document_type_mappings";
            using (HttpResponseMessage response = await _client.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JsonNode rootNode = JsonNode.Parse(responseBody);

                    var profile = JsonSerializer.Deserialize<DocumentTypeMappingsResult>(rootNode);
                    return profile;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
