using Academix.WinApp.Models.Common;
using Academix.WinApp.Models.Student;
using Academix.WinApp.Models.Teacher;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            _baseUrl = Config.GetApiBaseUrl() + "/api/classes/";
            _token = SessionManager.Token;
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(_baseUrl) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return client;
        }

        #region Teacher/Admin: Exam Management

        public async Task<ApiResponse<ExamListResponseDto>> GetExamsByClassAsync(
            int classId,
            bool? isPublished = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            using var client = CreateClient();
            var url = new StringBuilder($"{classId}/exams?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}");
            if (isPublished.HasValue) url.Append($"&isPublished={isPublished.Value.ToString().ToLower()}");

            var response = await client.GetAsync(url.ToString());
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<ExamListResponseDto>>() ?? new ApiResponse<ExamListResponseDto>();
        }

        public async Task<ApiResponse<ExamResponseDto>> GetExamByIdAsync(int classId, int examId)
        {
            using var client = CreateClient();
            var response = await client.GetAsync($"{classId}/exams/{examId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new ApiResponse<ExamResponseDto> { Success = false, Message = "Exam not found", Timestamp = DateTime.UtcNow };

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<ExamResponseDto>>() ?? new ApiResponse<ExamResponseDto>();
        }

        public async Task<ApiResponse<ExamResponseDto>> CreateExamAsync(int classId, CreateExamRequestDto request)
        {
            using var client = CreateClient();
            var response = await client.PostAsJsonAsync($"{classId}/exams", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<ExamResponseDto>>() ?? new ApiResponse<ExamResponseDto>();
        }

        public async Task<ApiResponse<ExamResponseDto>> UpdateExamAsync(int classId, int examId, UpdateExamRequestDto request)
        {
            using var client = CreateClient();
            var response = await client.PutAsJsonAsync($"{classId}/exams/{examId}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<ExamResponseDto>>() ?? new ApiResponse<ExamResponseDto>();
        }

        public async Task<ApiResponse<List<ExamQuestionDetailDto>>> GetExamQuestionsAsync(int classId, int examId)
        {
            using var client = CreateClient();
            var response = await client.GetAsync($"{classId}/exams/{examId}/questions");
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<List<ExamQuestionDetailDto>> { Success = false, Message = $"API Error {response.StatusCode}" };
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<ExamQuestionDetailDto>>>() ?? new ApiResponse<List<ExamQuestionDetailDto>>();
        }

        public async Task<ApiResponse<string>> AddQuestionsToExamAsync(int classId, int examId, AddQuestionsToExamRequestDto request)
        {
            using var client = CreateClient();
            var response = await client.PostAsJsonAsync($"{classId}/exams/{examId}/questions", request);
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<string> { Success = false, Message = $"API Error {response.StatusCode}" };
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>() ?? new ApiResponse<string>();
        }

        public async Task<ApiResponse<string>> RemoveQuestionFromExamAsync(int classId, int examId, int questionId)
        {
            using var client = CreateClient();
            var response = await client.DeleteAsync($"{classId}/exams/{examId}/questions/{questionId}");
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<string> { Success = false, Message = $"API Error {response.StatusCode}" };
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>() ?? new ApiResponse<string>();
        }

        public async Task<ApiResponse<string>> DeleteExamAsync(int classId, int examId)
        {
            using var client = CreateClient();
            var response = await client.DeleteAsync($"{classId}/exams/{examId}");
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<string> { Success = false, Message = $"API Error {response.StatusCode}" };
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>() ?? new ApiResponse<string>();
        }

        public async Task<ApiResponse<string>> PublishExamAsync(int classId, int examId)
        {
            using var client = CreateClient();

            var httpReq = new HttpRequestMessage(HttpMethod.Patch, $"{classId}/exams/{examId}/publish");

            var response = await client.SendAsync(httpReq);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"API Error {response.StatusCode}"
                };

            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>()
                   ?? new ApiResponse<string>();
        }


        public async Task<ApiResponse<ExamResultsResponseDto>> GetExamResultsAsync(
            int classId,
            int examId,
            int page = 1,
            int pageSize = 10)
        {
            using var client = CreateClient();
            var url = $"{classId}/exams/{examId}/results?page={page}&pageSize={pageSize}";
            var response = await client.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<ExamResultsResponseDto> { Success = false, Message = $"API Error {response.StatusCode}" };
            
            return await response.Content.ReadFromJsonAsync<ApiResponse<ExamResultsResponseDto>>() 
                ?? new ApiResponse<ExamResultsResponseDto>();
        }


        #endregion

        #region Student: Exam Participation

        public async Task<List<ExamDto>> GetStudentExamsAsync(int? classId = null)
        {
            using var client = CreateClient();
            var endpoint = "/api/student/exams" + (classId.HasValue ? $"?classId={classId.Value}" : "");
            var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ExamDto>>>();
            return result?.Data ?? new List<ExamDto>();
        }

        public async Task<List<StudentExamResultDto>> GetStudentExamHistoryAsync()
        {
            using var client = CreateClient();
            var response = await client.GetAsync("/api/student/exams/history");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<StudentExamResultDto>>>();
            return result?.Data ?? new List<StudentExamResultDto>();
        }

        public async Task<StartExamResponseDto?> StartExamAsync(int examId)
        {
            using var client = CreateClient();
            var response = await client.PostAsync($"/api/student/exams/{examId}/start", null);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<StartExamResponseDto>>();
            return result?.Data;
        }

        public async Task<bool> SaveAnswerAsync(int attemptId, int questionId, int selectedOptionId)
        {
            using var client = CreateClient();
            var payload = new ExamAnswerRequestDto { QuestionId = questionId, SelectedOptionId = selectedOptionId };
            var response = await client.PostAsJsonAsync($"/api/student/exams/attempts/{attemptId}/answer", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<StudentExamResultDto?> SubmitExamAsync(int attemptId, List<ExamAnswerRequestDto> answers)
        {
            using var client = CreateClient();
            var payload = new { answers };
            var response = await client.PostAsJsonAsync($"/api/student/exams/attempts/{attemptId}/submit", payload);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<StudentExamResultDto>>();
            return result?.Data;
        }

        public async Task<StudentExamResultDto?> GetAttemptResultAsync(int attemptId)
        {
            using var client = CreateClient();
            var response = await client.GetAsync($"/api/student/exams/attempts/{attemptId}/result");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<StudentExamResultDto>>();
            return result?.Data;
        }

        #endregion
    }
}
