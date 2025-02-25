using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;
using System.IO;
using System.Reflection;

namespace wpfcm1.OlympusApi
{
    public partial class OlympusService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

    }
}
