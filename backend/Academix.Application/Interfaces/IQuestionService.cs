using Academix.Application.DTOs.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IQuestionService
    {
        //  Lấy danh sách tất cả câu hỏi
        Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync();

        // Lấy chi tiết 1 câu hỏi theo ID
        Task<QuestionDto?> GetQuestionByIdAsync(int questionId);

        // Tạo mới câu hỏi
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionRequest request);

        // Cập nhật câu hỏi
        Task<QuestionDto?> UpdateQuestionAsync(int questionId, UpdateQuestionRequest request);

        // Xóa (hoặc đánh dấu đã xóa)
        Task<bool> DeleteQuestionAsync(int questionId);
    }
}
