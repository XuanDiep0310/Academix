using Academix.WinApp.Models.Common;
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
    public class QuestionApiService
    {
        private readonly string _baseUrl;
        private readonly string _token;

        public QuestionApiService()
        {
            _baseUrl = Config.GetApiBaseUrl() + "/api/questions";
            _token = SessionManager.Token;
        }

        // Lấy danh sách câu hỏi của giáo viên hiện tại
        public async Task<ApiResponse<QuestionListResponseDto>> GetMyQuestionsPagedAsync(
            string? subject = null,
            string? difficulty = null,
            string? type = null,
            string? search = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            // Build URL
            var urlBuilder = new StringBuilder($"{_baseUrl}?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}");

            if (!string.IsNullOrWhiteSpace(subject))
                urlBuilder.Append($"&subject={subject}");

            if (!string.IsNullOrWhiteSpace(difficulty))
                urlBuilder.Append($"&difficulty={difficulty}");

            if (!string.IsNullOrWhiteSpace(type))
                urlBuilder.Append($"&type={type}");

            if (!string.IsNullOrWhiteSpace(search))
                urlBuilder.Append($"&search={search}");

            string url = urlBuilder.ToString();

            // Call API
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {errorBody}");
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<QuestionListResponseDto>>()
                   ?? new ApiResponse<QuestionListResponseDto>();
        }

        // Lấy 1 câu hỏi theo ID
        public async Task<ApiResponse<QuestionResponseDto>> GetQuestionByIdAsync(int id)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync($"{_baseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {error}");
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<QuestionResponseDto>>()
                   ?? new ApiResponse<QuestionResponseDto>();
        }

        // Tạo câu hỏi mới
        public async Task<ApiResponse<QuestionResponseDto>> CreateQuestionAsync(CreateQuestionRequestDto request)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.PostAsJsonAsync(_baseUrl, request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {error}");
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<QuestionResponseDto>>()
                   ?? new ApiResponse<QuestionResponseDto>();
        }

        // Cập nhật câu hỏi
        public async Task<ApiResponse<QuestionResponseDto>> UpdateQuestionAsync(int id, UpdateQuestionRequestDto request)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.PutAsJsonAsync($"{_baseUrl}/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {error}");
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<QuestionResponseDto>>()
                   ?? new ApiResponse<QuestionResponseDto>();
        }

        // Xóa câu hỏi
        public async Task<ApiResponse<string>> DeleteQuestionAsync(int id)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.DeleteAsync($"{_baseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {error}");
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>()
                   ?? new ApiResponse<string>();
        }

        


    }

}
