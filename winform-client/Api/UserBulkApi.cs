using Academix.WinApp.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class UserBulkApi : ApiClient
    {
        public UserBulkApi(string baseUrl) : base(baseUrl) { }

        public async Task<string> AddUsersBulkAsync(List<UserBulkModel> users)
        {
            if (users == null || users.Count == 0)
                throw new ArgumentException("Danh sách người dùng không được để trống.");

            EnsureAuthToken();

            var body = new { users };
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                string endpoint = "api/Users/bulk";
                var response = await _client.PostAsync(endpoint, content);

                // Nếu server trả lỗi -> throw
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Lỗi API ({response.StatusCode}): {errorContent}");
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Không thể kết nối đến API. Vui lòng kiểm tra lại server.", ex);
            }
        }
    }

    public class UserBulkModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }
        public string role { get; set; } = "Student"; // mặc định là học sinh
    }
}
