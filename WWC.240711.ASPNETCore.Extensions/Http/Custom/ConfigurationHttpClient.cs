using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Http.Custom
{
    public class ConfigurationHttpClient
    {
        private readonly HttpClient _httpClient;

        // 直接注入 HttpClient
        public ConfigurationHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(int id) =>
            await _httpClient.GetFromJsonAsync<T>($"/posts/{id}");
    }
}
