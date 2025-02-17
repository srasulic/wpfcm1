using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;
using System.IO;

namespace wpfcm1.OlympusApi
{
    public class OlympusService
    {
        private readonly HttpClient _client;

        public OlympusService(string uri)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(uri)
            };
        }

        //TODO: ove 3 static funkcije stavi na drugo mesto
        public static T DeepCopy<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string SerializeToJson<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T DeserializeFromJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task<Token> PostUsersLogin(string username, string password)
        {
            if (username is null || password is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var parameters = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };
            var content = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await _client.PostAsync("/olympus/v1/users/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<Token>(responseBody);
                return token;
            }
            else
            {
                return null;
            }
        }

        public async Task<UsersMe> GetUsersMe(Token token)
        {
            if (token is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            HttpResponseMessage response = await _client.GetAsync("/olympus/v1/users/me");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                UsersMe me = JsonSerializer.Deserialize<UsersMe>(responseBody);
                return me;
            }
            else
            {
                return null;
            }
        }

        public async Task<TenantsResult> GetUsersTenants(Token token)
        {
            if (token is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            HttpResponseMessage response = await _client.GetAsync("/olympus/v1/users/tenants");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                JsonNode rootNode = JsonNode.Parse(responseBody);

                var tenants = JsonSerializer.Deserialize<TenantsResult>(rootNode);
                return tenants;
            }
            else
            {
                return null;
            }
        }

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
                return null;
            }
        }

        public async Task<bool> PostDocumentsUploadOutbound(Token token, string tipDok, string filePath)
        {
            if (token is null || filePath is null)
            {
                return false;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            string base64Content = Convert.ToBase64String(File.ReadAllBytes(filePath));
            var payload = new Payload { tip_dok = tipDok, teh_naziv_fajla = Path.GetFileName(filePath), sadrzaj = base64Content };
            var jsonPayload = JsonSerializer.Serialize(payload);

            using (StringContent jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
            using (HttpResponseMessage response = await _client.PostAsync("/olympus/v1/documents/upload_outbound", jsonContent))
            {
                return response.IsSuccessStatusCode;
            }
        }
    }

}
