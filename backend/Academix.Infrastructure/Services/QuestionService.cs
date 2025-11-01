using Academix.Application.DTOs.Question;
using Academix.Application.Interfaces;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly AcademixDbContext _context;

        public QuestionService(AcademixDbContext context)
        {
            _context = context;
        }

        // 🧩 Ánh xạ từ Entity → DTO
        private static QuestionDto MapToDto(Question entity)
        {
            return new QuestionDto
            {
                QuestionId = entity.QuestionId,
                OrganizationId = entity.OrganizationId,
                CreatedBy = entity.CreatedBy.HasValue ? entity.CreatedBy.Value.ToString() : null,
                CreatedAt = entity.CreatedAt,
                TypeId = entity.TypeId,
                Stem = entity.Stem,
                Solution = entity.Solution,
                Difficulty = entity.Difficulty,
                Metadata = entity.Metadata,
                Options = entity.QuestionOptions.Select(o => new QuestionOptionResponse
                {
                    Id = o.OptionId,
                    Content = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            };
        }

        // 1️⃣ Tạo mới câu hỏi
        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionRequest request)
        {
            var newQuestion = new Question
            {
                OrganizationId = request.OrganizationId,
                CreatedBy = request.CreatedBy,
                TypeId = request.TypeId,
                Stem = request.Stem,
                Solution = request.Solution,
                Difficulty = request.Difficulty ?? 1,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                QuestionOptions = request.Options?.Select(o => new QuestionOption
                {
                    Text = o.Content,
                    IsCorrect = o.IsCorrect
                }).ToList() ?? new List<QuestionOption>()
            };

            _context.Questions.Add(newQuestion);
            await _context.SaveChangesAsync();

            return MapToDto(newQuestion);
        }

        // 2️⃣ Lấy danh sách câu hỏi
        public async Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync()
        {
            var questions = await _context.Questions
                .Include(q => q.QuestionOptions)
                .Where(q => !q.IsDeleted)
                .ToListAsync();

            return questions.Select(MapToDto);
        }

        // 3️⃣ Lấy câu hỏi theo ID
        public async Task<QuestionDto?> GetQuestionByIdAsync(int questionId)
        {
            var entity = await _context.Questions
                .Include(q => q.QuestionOptions)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId && !q.IsDeleted);

            return entity == null ? null : MapToDto(entity);
        }

        // 4️⃣ Cập nhật câu hỏi
        public async Task<QuestionDto?> UpdateQuestionAsync(int questionId, UpdateQuestionRequest request)
        {
            var existingQuestion = await _context.Questions
                .Include(q => q.QuestionOptions)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId && !q.IsDeleted);

            if (existingQuestion == null)
                return null;

            existingQuestion.Stem = request.Stem ?? existingQuestion.Stem;
            existingQuestion.Solution = request.Solution ?? existingQuestion.Solution;
            existingQuestion.Difficulty = request.Difficulty ?? existingQuestion.Difficulty;
            existingQuestion.TypeId = request.TypeId == 0 ? existingQuestion.TypeId : request.TypeId;
            existingQuestion.Metadata = request.Metadata ?? existingQuestion.Metadata;

            if (request.Options != null)
            {
                existingQuestion.QuestionOptions.Clear();
                existingQuestion.QuestionOptions = request.Options.Select(o => new QuestionOption
                {
                    Text = o.Content,
                    IsCorrect = o.IsCorrect
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return MapToDto(existingQuestion);
        }

        // 5️⃣ Xóa mềm (IsDeleted = true)
        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var existingQuestion = await _context.Questions
                .FirstOrDefaultAsync(q => q.QuestionId == questionId && !q.IsDeleted);

            if (existingQuestion == null)
                return false;

            existingQuestion.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
