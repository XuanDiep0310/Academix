using Academix.WinApp.Models.Teacher;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class ClassApiService
    {
        private readonly string _baseUrl;
        private readonly string _token;

        public ClassApiService()
        {
            _baseUrl = Config.GetApiBaseUrl() + "/api/Classes";
            _token = SessionManager.Token; // Lấy token đã lưu sau khi đăng nhập
        }

        // Lấy danh sách lớp mà giáo viên dạy
        public async Task<List<MyClassResponseDto>> GetMyClassesAsync()
        {
            var url = $"{_baseUrl}/my-classes";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {error}");
            }

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<List<MyClassResponseDto>>>();

            return apiResponse?.Data ?? new List<MyClassResponseDto>();
        }


        // Lấy danh sách học viên trong lớp
        public async Task<List<ClassStudentDto>> GetStudentsByClassAsync(int classId)
        {
            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            string url = $"{_baseUrl}/{classId}/students";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<List<ClassStudentDto>>>();

            return result?.Data ?? new List<ClassStudentDto>();
        }

    }
}
