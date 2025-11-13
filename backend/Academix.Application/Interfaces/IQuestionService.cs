using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IQuestionService
    {
        // Question CRUD Operations
        Task<ApiResponse<QuestionListResponseDto>> GetQuestionsAsync(
            int teacherId,
            string? subject = null,
            string? difficulty = null,
            string? type = null,
            string? search = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc");

        Task<ApiResponse<QuestionResponseDto>> GetQuestionByIdAsync(int questionId);

        Task<ApiResponse<QuestionResponseDto>> CreateQuestionAsync(CreateQuestionRequestDto request, int teacherId);

        Task<ApiResponse<BulkCreateQuestionsResponseDto>> CreateQuestionsAsync(
            BulkCreateQuestionsRequestDto request,
            int teacherId);

        Task<ApiResponse<QuestionResponseDto>> UpdateQuestionAsync(int questionId, UpdateQuestionRequestDto request);

        Task<ApiResponse<string>> DeleteQuestionAsync(int questionId);

        // Statistics
        Task<ApiResponse<QuestionStatisticsDto>> GetQuestionStatisticsAsync(int? teacherId = null);

        // Validation
        Task<bool> CanAccessQuestionAsync(int questionId, int teacherId);
        Task<bool> IsQuestionInUseAsync(int questionId);
    }
}
