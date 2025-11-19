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
    public class ExamApiService
    {
        private readonly string _baseUrl;
        private readonly string _token;

        public ExamApiService()
        {
            _baseUrl = Config.GetApiBaseUrl() + "/api/classes";
            _token = SessionManager.Token;
        }

        public async Task<ApiResponse<ExamListResponseDto>> GetExamsByClassAsync(
        int classId,
        bool? isPublished = null,
        int page = 1,
        int pageSize = 10,
        string sortBy = "CreatedAt",
        string sortOrder = "desc")
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            var url = new StringBuilder($"{_baseUrl}/{classId}/exams?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}");

            if (isPublished.HasValue)
                url.Append($"&isPublished={isPublished.Value}");

            var response = await client.GetAsync(url.ToString());
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ExamListResponseDto>>();
            return apiResponse ?? new ApiResponse<ExamListResponseDto>();
        }
        public async Task<ApiResponse<ExamResponseDto>> GetExamByIdAsync(int classId, int examId)
        {
            var result = new ApiResponse<ExamResponseDto>
            {
                Success = false,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);

                var url = $"{_baseUrl}/{classId}/exams/{examId}";

                var response = await client.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    result.Message = "Exam not found";
                    return result;
                }

                response.EnsureSuccessStatusCode();

                // Đọc JSON về ApiResponse<ExamResponseDto>
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ExamResponseDto>>();

                if (apiResponse != null)
                {
                    // Copy dữ liệu từ API về
                    result.Success = apiResponse.Success;
                    result.Message = apiResponse.Message;
                    result.Data = apiResponse.Data;
                    result.Errors = apiResponse.Errors ?? new List<string>();
                }
                else
                {
                    result.Message = "Empty response from server";
                }
            }
            catch (Exception ex)
            {
                result.Message = $"Error: {ex.Message}";
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }


        public async Task<ApiResponse<ExamResponseDto>> CreateExamAsync(int classId, CreateExamRequestDto request)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            var url = $"{_baseUrl}/{classId}/exams";

            var response = await client.PostAsJsonAsync(url, request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API ERROR {response.StatusCode}: {content}");

            return response.Content.ReadFromJsonAsync<ApiResponse<ExamResponseDto>>().Result
                   ?? new ApiResponse<ExamResponseDto>();
        }

        public async Task<ApiResponse<ExamResponseDto>> UpdateExamAsync(
            int classId, int examId, UpdateExamRequestDto request)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            var url = $"{_baseUrl}/{classId}/exams/{examId}";

            var response = await client.PutAsJsonAsync(url, request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API ERROR {response.StatusCode}: {content}");

            return response.Content.ReadFromJsonAsync<ApiResponse<ExamResponseDto>>().Result
                   ?? new ApiResponse<ExamResponseDto>();
        }

        public async Task<ApiResponse<List<ExamQuestionDetailDto>>> GetExamQuestionsAsync(int classId, int examId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = $"{_baseUrl}/{classId}/exams/{examId}/questions";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<ExamQuestionDetailDto>> { Success = false, Message = $"API Error {response.StatusCode}" };

            return await response.Content.ReadFromJsonAsync<ApiResponse<List<ExamQuestionDetailDto>>>()
                   ?? new ApiResponse<List<ExamQuestionDetailDto>>();
        }

        public async Task<ApiResponse<string>> AddQuestionsToExamAsync(int classId, int examId, AddQuestionsToExamRequestDto request)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = $"{_baseUrl}/{classId}/exams/{examId}/questions";
            var response = await client.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<string> { Success = false, Message = $"API Error {response.StatusCode}" };

            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>()
                   ?? new ApiResponse<string>();
        }

        public async Task<ApiResponse<string>> RemoveQuestionFromExamAsync(int classId, int examId, int questionId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = $"{_baseUrl}/{classId}/exams/{examId}/questions/{questionId}";
            var response = await client.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<string> { Success = false, Message = $"API Error {response.StatusCode}" };

            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>()
                   ?? new ApiResponse<string>();
        }

        public async Task<List<QuestionDto>> GetQuestionsByClassAsync(int classId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = $"{_baseUrl}/{classId}/questions"; // backend phải có endpoint này
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<QuestionDto>();

            var apiResp = await response.Content.ReadFromJsonAsync<ApiResponse<List<QuestionDto>>>();
            return apiResp?.Data ?? new List<QuestionDto>();
        }


    }
}
