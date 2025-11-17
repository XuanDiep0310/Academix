<<<<<<< HEAD
﻿using Academix.WinApp.Models.Teacher;
=======
﻿using Academix.WinApp.Models;
using Academix.WinApp.Models.Classes;
using Academix.WinApp.Models.Common;
using Academix.WinApp.Models.Teacher;
>>>>>>> 1c51b105e83d91cb367732955f16453246f4014a
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public async Task<ClassPagedResult> GetClassesAsync(
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            string url = $"{_baseUrl}?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<
                ApiResponse<ClassPagedResult>
            >();

            return result?.Data ?? new ClassPagedResult();
        }

        public async Task<MyClassResponseDto> CreateClassAsync(string className, string classCode, string description)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var payload = new
            {
                className,
                classCode,
                description
            };

            var response = await client.PostAsJsonAsync(_baseUrl, payload);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            // đọc dữ liệu trả về theo schema ApiResponse
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<MyClassResponseDto>>();

            if (apiResponse == null || !apiResponse.Success)
                throw new Exception($"API ERROR: {apiResponse?.Errors?.FirstOrDefault() ?? "Không có dữ liệu"}");

            return apiResponse.Data;
        }

        // Lấy tất cả thành viên (học sinh + giáo viên) trong lớp
        public async Task<List<ClassMember>> GetAllMembersAsync(string classId)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/members";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ClassMember>>>();
            return result?.Data ?? new List<ClassMember>();
        }

        // Lấy danh sách học sinh trong lớp
        public async Task<List<ClassMember>> GetStudentsAsync(string classId)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/students";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ClassMember>>>();
            return result?.Data ?? new List<ClassMember>();
        }

        // Lấy danh sách giáo viên trong lớp
        public async Task<List<ClassMember>> GetTeachersAsync(string classId)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/teachers";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ClassMember>>>();
            return result?.Data ?? new List<ClassMember>();
        }




    }
}
