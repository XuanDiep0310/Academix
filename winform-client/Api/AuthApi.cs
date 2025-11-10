using Academix.WinApp.Models;
using Academix.WinApp.Models.Academix.WinApp.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class AuthApi
    {
        private static readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5235";

        static AuthApi()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var url = $"{BaseUrl}/api/Auth/login";

            try
            {
                Debug.WriteLine($"========== LOGIN REQUEST ==========");
                Debug.WriteLine($"URL: {url}");
                Debug.WriteLine($"Email: {request.Email}");

                var json = JsonConvert.SerializeObject(request);
                Debug.WriteLine($"Request JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                Debug.WriteLine($"Response Status: {(int)response.StatusCode} {response.StatusCode}");
                Debug.WriteLine($"Response Body: {responseString}");
                Debug.WriteLine($"===================================");

                if (response.IsSuccessStatusCode)
                {
                    // Parse API response
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LoginResponseDto>>(responseString);

                    if (apiResponse?.Success == true && apiResponse.Data != null)
                    {
                        // Map từ DTO sang LoginResponse
                        return new LoginResponse
                        {
                            Success = true,
                            Message = apiResponse.Message ?? "Đăng nhập thành công!",
                            Token = apiResponse.Data.AccessToken,  // AccessToken -> Token
                            RefreshToken = apiResponse.Data.RefreshToken,
                            User = apiResponse.Data.User != null ? new UserData
                            {
                                Id = apiResponse.Data.User.UserId,  // UserId -> Id
                                Email = apiResponse.Data.User.Email,
                                FullName = apiResponse.Data.User.FullName,
                                Role = apiResponse.Data.User.Role
                            } : null
                        };
                    }
                    else
                    {
                        return new LoginResponse
                        {
                            Success = false,
                            Message = apiResponse?.Message ?? "Đăng nhập không thành công"
                        };
                    }
                }
                else
                {
                    // Xử lý lỗi
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ApiResponse<LoginResponseDto>>(responseString);

                        var errorMessage = errorResponse?.Message ?? $"Lỗi {response.StatusCode}";

                        if (errorResponse?.Errors != null && errorResponse.Errors.Count > 0)
                        {
                            errorMessage += "\n\nChi tiết:\n" + string.Join("\n", errorResponse.Errors);
                        }

                        return new LoginResponse
                        {
                            Success = false,
                            Message = errorMessage
                        };
                    }
                    catch
                    {
                        return new LoginResponse
                        {
                            Success = false,
                            Message = $"Lỗi {response.StatusCode}:\n{responseString}"
                        };
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HttpRequestException: {ex}");
                return new LoginResponse
                {
                    Success = false,
                    Message = "❌ Không thể kết nối đến API!\n\n" +
                             $"URL: {url}\n\n" +
                             "Kiểm tra:\n" +
                             "1. API có đang chạy tại http://localhost:5235 không?\n" +
                             "2. Thử mở: http://localhost:5235/swagger\n\n" +
                             $"Chi tiết: {ex.Message}"
                };
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Request timeout");
                return new LoginResponse
                {
                    Success = false,
                    Message = "⏱️ Yêu cầu hết thời gian chờ!\n\nAPI phản hồi quá chậm."
                };
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON parse error: {ex}");
                return new LoginResponse
                {
                    Success = false,
                    Message = $"❌ Lỗi parse JSON:\n{ex.Message}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex}");
                return new LoginResponse
                {
                    Success = false,
                    Message = $"❌ Lỗi không xác định:\n{ex.GetType().Name}\n{ex.Message}"
                };
            }
        }
    }
}