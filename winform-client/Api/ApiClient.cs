using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Academix.WinApp.Utils;
using System.Diagnostics;

namespace Academix.WinApp.Api
{
    public class ApiClient
    {
        protected readonly HttpClient _client;
        private readonly string _baseUrl;

        public ApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _client = ApiClientFactory.GetClient(baseUrl);
        }

        // Gọi method này TRƯỚC MỖI request
        protected void EnsureAuthToken()
        {
            if (!string.IsNullOrEmpty(SessionManager.Token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.Token);
                Debug.WriteLine("EnsureAuthToken: Token set!");
            }
            else
            {
                Debug.WriteLine("EnsureAuthToken: No token in SessionManager!");
            }
        }

        protected void SetAuthToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            else
                _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}