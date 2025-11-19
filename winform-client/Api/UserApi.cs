using Academix.WinApp.Models.Common;
using Academix.WinApp.Models.Users;
using Academix.WinApp.Utils;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<UserListData> GetAllUsersAsync(
             int page = 1,
             int pageSize = 20,
             string sortBy = "CreatedAt",
             string sortOrder = "desc")
        {
            EnsureAuthToken();

            string endpoint =
                $"api/Users?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";

            var response = await _client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("API Error: " + content);

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserListData>>(content);

            return apiResponse?.Data ?? new UserListData();
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

        public async Task<ApiResponse<UserData>> UpdateUserAsync(int userId, string fullName, string email, bool isActive)
        {
            if (!SessionManager.IsAuthenticated)
                throw new Exception("Chưa đăng nhập hoặc token không hợp lệ.");

            EnsureAuthToken();

            string endpoint = $"api/Users/{userId}";
            var payload = new
            {
                fullName = fullName,
                email = email,
                isActive = isActive
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = new Uri(_client.BaseAddress, endpoint);
            Debug.WriteLine("PUT URL: " + url); // debug URL xem có đúng không

            var response = await _client.PutAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<UserData>
                {
                    Success = false,
                    Message = $"Lỗi API: {response.StatusCode}",
                    Data = null
                };
            }

            return JsonConvert.DeserializeObject<ApiResponse<UserData>>(body);
        }

        public async Task<ApiResponse<string>> DeleteUserAsync(int userId)
        {
            if (!SessionManager.IsAuthenticated)
                throw new Exception("Chưa đăng nhập hoặc token không hợp lệ.");

            if (userId <= 0)
                throw new ArgumentException("UserId không hợp lệ.");

            EnsureAuthToken(); // giả sử ở đây đã set JWT token cho _client

            string endpoint = $"api/Users/{userId}";
            var url = new Uri(_client.BaseAddress, endpoint);
            Debug.WriteLine("DELETE URL: " + url);

            var response = await _client.DeleteAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResult = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = errorResult?.Message ?? $"Lỗi API: {response.StatusCode}",
                    Errors = errorResult?.Errors ?? new List<string> { body }
                };
            }

            return JsonConvert.DeserializeObject<ApiResponse<string>>(body) ?? new ApiResponse<string>
            {
                Success = true,
                Message = "User deleted successfully",
                Data = "OK",
                Errors = null
            };
        }
    }
}