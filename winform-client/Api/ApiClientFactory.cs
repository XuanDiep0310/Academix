using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using Academix.WinApp.Utils;

namespace Academix.WinApp.Api
{
    public static class ApiClientFactory
    {
        private static HttpClient _sharedClient;
        private static readonly object _lock = new object();

        public static HttpClient GetClient(string baseUrl)
        {
            if (_sharedClient == null)
            {
                lock (_lock)
                {
                    if (_sharedClient == null)
                    {
                        Debug.WriteLine("=== Creating new HttpClient ===");
                        _sharedClient = new HttpClient
                        {
                            BaseAddress = new Uri(baseUrl),
                            Timeout = TimeSpan.FromSeconds(30)
                        };
                    }
                }
            }

            // Cập nhật token mỗi khi lấy client
            UpdateAuthToken();
            return _sharedClient;
        }

        public static void UpdateAuthToken()
        {
            if (_sharedClient == null)
            {
                Debug.WriteLine("UpdateAuthToken: _sharedClient is NULL");
                return;
            }

            Debug.WriteLine("=== UpdateAuthToken ===");
            Debug.WriteLine("SessionManager.Token: " + (SessionManager.Token ?? "NULL"));

            if (!string.IsNullOrEmpty(SessionManager.Token))
            {
                _sharedClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.Token);
                Debug.WriteLine("Token SET to HttpClient: Bearer " + SessionManager.Token.Substring(0, 30) + "...");
            }
            else
            {
                _sharedClient.DefaultRequestHeaders.Authorization = null;
                Debug.WriteLine("Token CLEARED from HttpClient");
            }

            // Verify
            var authHeader = _sharedClient.DefaultRequestHeaders.Authorization;
            Debug.WriteLine("Current Auth Header: " + (authHeader != null ? $"{authHeader.Scheme} {authHeader.Parameter?.Substring(0, 30)}..." : "NULL"));
        }

        public static void ClearClient()
        {
            _sharedClient?.Dispose();
            _sharedClient = null;
        }
    }
}