using Academix.WinApp.Models;
using Academix.WinApp.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class UserApi : ApiClient
    {
        public UserApi(string baseUrl) : base(baseUrl)
        {
        }

        public async Task<List<UserData>> GetAllUsersAsync(int page = 1, int pageSize = 100,
    string sortBy = "CreatedAt", string sortOrder = "desc")
        {
            if (!SessionManager.IsAuthenticated)
                throw new Exception("Chưa đăng nhập hoặc token không hợp lệ.");

            EnsureAuthToken();

            string endpoint = $"api/Users?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";

            Debug.WriteLine("=== REQUEST INFO ===");
            Debug.WriteLine("Full URL: " + new Uri(_client.BaseAddress, endpoint));

            var response = await _client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("HTTP Status: " + response.StatusCode);
            Debug.WriteLine("Response Body: " + content);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine("ERROR: Request failed!");
                return new List<UserData>();
            }

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserListData>>(content);
            return apiResponse?.Data?.Users ?? new List<UserData>();
        }
    }
}