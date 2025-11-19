using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Exams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IExamService
    {
        // Exam Management (Teacher)
        Task<ApiResponse<ExamListResponseDto>> GetExamsByClassAsync(
            int classId,
            bool? isPublished = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc");

        Task<ApiResponse<ExamDetailResponseDto>> GetExamByIdAsync(int examId);

        Task<ApiResponse<ExamResponseDto>> CreateExamAsync(int classId, CreateExamRequestDto request, int createdBy);

        Task<ApiResponse<ExamResponseDto>> UpdateExamAsync(int examId, UpdateExamRequestDto request);

        Task<ApiResponse<string>> DeleteExamAsync(int examId);

        Task<ApiResponse<string>> PublishExamAsync(int examId);

        Task<ApiResponse<List<ExamQuestionDetailDto>>> GetExamQuestionsAsync(int examId);

        Task<ApiResponse<string>> AddQuestionsToExamAsync(int examId, AddQuestionsToExamRequestDto request);

        Task<ApiResponse<string>> RemoveQuestionFromExamAsync(int examId, int questionId);

        // Student Exam Taking
        Task<ApiResponse<StartExamResponseDto>> StartExamAsync(int examId, int studentId);

        Task<ApiResponse<string>> SubmitAnswerAsync(int attemptId, SubmitAnswerRequestDto request);

        Task<ApiResponse<ExamResultResponseDto>> SubmitExamAsync(int attemptId, SubmitExamRequestDto request);

        Task<ApiResponse<ExamResultResponseDto>> GetMyExamResultAsync(int attemptId, int studentId);

        Task<ApiResponse<List<ExamResponseDto>>> GetMyExamsAsync(int studentId, int? classId = null);
        Task<ApiResponse<List<ExamResultResponseDto>>> GetMyExamHistoryAsync(int studentId, int? classId = null);

        // Teacher Results Review
        Task<ApiResponse<ExamResultsListResponseDto>> GetExamResultsAsync(
            int examId,
            int page = 1,
            int pageSize = 10);

        Task<ApiResponse<ExamResultResponseDto>> GetStudentExamResultAsync(int attemptId);

        // Validation
        Task<bool> CanAccessExamAsync(int examId, int userId, string role);
        Task<bool> CanTakeExamAsync(int examId, int studentId);
        Task<bool> HasActiveAttemptAsync(int examId, int studentId); // Returns true if student has ANY attempt (one attempt per exam rule)
    }
}
