using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class ApiClient
    {
        private readonly HttpClient _client;

        public ApiClient(string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        // Gửi GET request
        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        // Gửi POST request
        public async Task<TResponse> PostAsync<TResponse>(string url, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            // Nếu API trả về dữ liệu JSON
            if (response.Content.Headers.ContentLength > 0)
                return await response.Content.ReadFromJsonAsync<TResponse>();

            return default!;
        }

        // (Tuỳ chọn) Gửi PUT hoặc DELETE
        public async Task<TResponse> PutAsync<TResponse>(string url, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task DeleteAsync(string url)
        {
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}
