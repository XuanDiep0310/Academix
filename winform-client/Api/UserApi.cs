using Academix.WinApp.Models.Common;
using Academix.WinApp.Models.Users;
using Academix.WinApp.Utils;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class UserApi : ApiClient
    {
        public UserApi(string baseUrl) : base(baseUrl)
        {
        }
        //public void SetToken(string token)
        //{
        //    SessionManager.Token = token; // lưu vào SessionManager
        //    SetAuthToken(token);          // set vào HttpClient
        //}

        public async Task<List<UserData>> GetAllUsersAsync(
             int page = 1,
             int pageSize = 100,
             string sortBy = "CreatedAt",
             string sortOrder = "desc")
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

        public async Task<ApiResponse<UserData>> CreateUserAsync(UserCreateRequest request)
        {
            if (!SessionManager.IsAuthenticated)
                throw new Exception("Chưa đăng nhập hoặc token không hợp lệ.");

            EnsureAuthToken();

            string endpoint = "api/Users";
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Debug.WriteLine("=== REQUEST INFO ===");
            Debug.WriteLine($"POST URL: {new Uri(_client.BaseAddress, endpoint)}");
            Debug.WriteLine($"Body: {json}");

            var response = await _client.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("HTTP Status: " + response.StatusCode);
            Debug.WriteLine("Response Body: " + body);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<UserData>
                {
                    Success = false,
                    Message = $"Lỗi API: {response.StatusCode}",
                    Data = null
                };
            }

            try
            {
                return JsonConvert.DeserializeObject<ApiResponse<UserData>>(body);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Deserialize error: " + ex.Message);
                return new ApiResponse<UserData>
                {
                    Success = false,
                    Message = "Lỗi parse dữ liệu từ server.",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<UserBulkResponse>> CreateUsersBulkAsync(UserBulkRequest request)
        {
            if (!SessionManager.IsAuthenticated)
                throw new Exception("Chưa đăng nhập hoặc token không hợp lệ.");

            EnsureAuthToken();

            string endpoint = "api/Users/bulk";
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Debug.WriteLine("=== REQUEST INFO ===");
            Debug.WriteLine($"POST URL: {new Uri(_client.BaseAddress, endpoint)}");
            Debug.WriteLine($"Body: {json}");

            var response = await _client.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("HTTP Status: " + response.StatusCode);
            Debug.WriteLine("Response Body: " + body);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<UserBulkResponse>
                {
                    Success = false,
                    Message = $"Lỗi API: {response.StatusCode}",
                    Data = null
                };
            }

            try
            {
                return JsonConvert.DeserializeObject<ApiResponse<UserBulkResponse>>(body);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Deserialize error: " + ex.Message);
                return new ApiResponse<UserBulkResponse>
                {
                    Success = false,
                    Message = "Lỗi parse dữ liệu từ server.",
                    Data = null
                };
            }
        }


    }
}