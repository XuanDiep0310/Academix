using Academix.WinApp.Models.Common;
using Academix.WinApp.Models.Student;
using Academix.WinApp.Models.Teacher;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class ExamApiService
    {
        private readonly string _baseUrl;

        public ExamApiService()
        {
            _baseUrl = Config.GetApiBaseUrl();
        }

        public async Task<ExamPagedResult> GetExamsByClassAsync(
            int classId,
            bool? isPublished,
            int page,
            int pageSize,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var query =
                $"page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";

            if (isPublished.HasValue)
            {
                query += $"&isPublished={isPublished.Value.ToString().ToLower()}";
            }

            var url = $"/api/classes/{classId}/exams?{query}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<ExamListResponseDto>>();

            return new ExamPagedResult
            {
                Exams = result?.Data?.Exams ?? new(),
                Page = result?.Data?.Page ?? page,
                PageSize = result?.Data?.PageSize ?? pageSize,
                TotalPages = result?.Data?.TotalPages ?? 0,
                TotalCount = result?.Data?.TotalCount ?? 0
            };
        }

        // Danh sách bài kiểm tra của student (tất cả lớp mà student tham gia)
        public async Task<List<ExamDto>> GetStudentExamsAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var response = await client.GetAsync("/api/student/exams");
            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<List<ExamDto>>>();

            return result?.Data ?? new List<ExamDto>();
        }

        // Lịch sử làm bài của học sinh
        public async Task<List<StudentExamResultDto>> GetStudentExamHistoryAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var response = await client.GetAsync("/api/student/exams/history");
            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<List<StudentExamResultDto>>>();

            return result?.Data ?? new List<StudentExamResultDto>();
        }

        // Bắt đầu một attempt làm bài cho student
        public async Task<StartExamResponseDto?> StartExamAsync(int examId)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var response = await client.PostAsync($"/api/student/exams/{examId}/start", null);
            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<StartExamResponseDto>>();

            return result?.Data;
        }

        // Lưu 1 câu trả lời (từng câu) cho attempt hiện tại
        public async Task<bool> SaveAnswerAsync(int attemptId, int questionId, int selectedOptionId)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var payload = new ExamAnswerRequestDto
            {
                QuestionId = questionId,
                SelectedOptionId = selectedOptionId
            };

            var response = await client.PostAsJsonAsync(
                $"/api/student/exams/attempts/{attemptId}/answer",
                payload
            );

            return response.IsSuccessStatusCode;
        }

        // Nộp bài với toàn bộ câu trả lời
        public async Task<StudentExamResultDto?> SubmitExamAsync(
            int attemptId,
            List<ExamAnswerRequestDto> answers
        )
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var payload = new { answers };

            var response = await client.PostAsJsonAsync(
                $"/api/student/exams/attempts/{attemptId}/submit",
                payload
            );
            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<StudentExamResultDto>>();

            return result?.Data;
        }

        // Xem chi tiết kết quả của một attempt bất kỳ
        public async Task<StudentExamResultDto?> GetAttemptResultAsync(int attemptId)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var response = await client.GetAsync($"/api/student/exams/attempts/{attemptId}/result");
            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<StudentExamResultDto>>();

            return result?.Data;
        }
    }
}


