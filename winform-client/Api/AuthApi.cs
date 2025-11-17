using Academix.WinApp.Models.Academix.WinApp.Models;
using Academix.WinApp.Models.Auth;
using Academix.WinApp.Models.Common;
using Academix.WinApp.Utils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class AuthApi : ApiClient
    {
        public AuthApi(string baseUrl) : base(baseUrl)
        {
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("api/Auth/login", content);
                var body = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("Response Status: " + response.StatusCode);
                Debug.WriteLine("Response Body: " + body);

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LoginData>>(body);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    // Lưu session
                    SessionManager.Token = apiResponse.Data.AccessToken;
                    SessionManager.RefreshToken = apiResponse.Data.RefreshToken;
                    SessionManager.CurrentUser = new UserData
                    {
                        UserId = apiResponse.Data.User.UserId,
                        Email = apiResponse.Data.User.Email,
                        FullName = apiResponse.Data.User.FullName,
                        Role = apiResponse.Data.User.Role
                    };

                    // CẬP NHẬT TOKEN CHO TẤT CẢ API CALLS
                    ApiClientFactory.UpdateAuthToken();

                    Debug.WriteLine("Token saved: " + SessionManager.Token);

                    return new LoginResponse
                    {
                        Success = true,
                        Message = apiResponse.Message,
                        Token = apiResponse.Data.AccessToken,
                        RefreshToken = apiResponse.Data.RefreshToken,
                        User = SessionManager.CurrentUser
                    };
                }

                return new LoginResponse
                {
                    Success = false,
                    Message = apiResponse?.Message ?? "Login failed"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Login error: " + ex);
                return new LoginResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}