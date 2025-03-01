using System.Collections.Generic;
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
                Log.Error(response.ReasonPhrase);
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
                Log.Error(response.ReasonPhrase);
                return null;
            }
        }

        public async Task<TenantSingleResult> GetUsersTenant(Token token, Tenant tenant)
        {
            if (token is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            string uri = $"/olympus/v1/users/tenant?tenant={tenant.tenant}";
            using (HttpResponseMessage response = await _client.GetAsync(uri))
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                JsonNode rootNode = JsonNode.Parse(responseBody);
                var tr = JsonSerializer.Deserialize<TenantSingleResult>(rootNode);
                return tr;
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

            using (HttpResponseMessage response = await _client.GetAsync("/olympus/v1/users/tenants"))
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JsonNode rootNode = JsonNode.Parse(responseBody);
                var tenants = JsonSerializer.Deserialize<TenantsResult>(rootNode);

                return tenants;
            }
        }

        public async Task<Result> PutUsersSetTenant(Token token, Tenant tenant)
        {
            if (token is null || tenant is null)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

            string jsonPayload = $"{{ \"tenant\": \"{tenant.tenant}\" }}";
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await _client.PutAsync("/olympus/v1/users/set_tenant", content))
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JsonNode rootNode = JsonNode.Parse(responseBody);
                var result = JsonSerializer.Deserialize<Result>(rootNode["result"]);

                return result;
            }
        }

    }
}
