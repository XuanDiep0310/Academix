using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Questions;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academix.Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly AcademixDbContext _context;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(AcademixDbContext context, ILogger<QuestionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<QuestionListResponseDto>> GetQuestionsAsync(
            int teacherId,
            string? subject = null,
            string? difficulty = null,
            string? type = null,
            string? search = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            try
            {
                var query = _context.Questions
                    .Include(q => q.Teacher)
                    .Include(q => q.QuestionOptions)
                    .Where(q => q.TeacherId == teacherId)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(subject))
                {
                    query = query.Where(q => q.Subject == subject);
                }

                if (!string.IsNullOrEmpty(difficulty))
                {
                    query = query.Where(q => q.DifficultyLevel == difficulty);
                }

                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(q => q.QuestionType == type);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(q => q.QuestionText.ToLower().Contains(searchLower));
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply sorting
                query = sortBy.ToLower() switch
                {
                    "subject" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(q => q.Subject)
                        : query.OrderByDescending(q => q.Subject),
                    "difficulty" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(q => q.DifficultyLevel)
                        : query.OrderByDescending(q => q.DifficultyLevel),
                    "type" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(q => q.QuestionType)
                        : query.OrderByDescending(q => q.QuestionType),
                    _ => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(q => q.CreatedAt)
                        : query.OrderByDescending(q => q.CreatedAt)
                };

                // Apply pagination
                var questions = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(q => new QuestionResponseDto
                    {
                        QuestionId = q.QuestionId,
                        TeacherId = q.TeacherId,
                        TeacherName = q.Teacher.FullName,
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        DifficultyLevel = q.DifficultyLevel,
                        Subject = q.Subject,
                        Options = q.QuestionOptions
                            .OrderBy(o => o.OptionOrder)
                            .Select(o => new QuestionOptionDto
                            {
                                OptionId = o.OptionId,
                                OptionText = o.OptionText,
                                IsCorrect = o.IsCorrect,
                                OptionOrder = o.OptionOrder
                            })
                            .ToList(),
                        CreatedAt = q.CreatedAt,
                        UpdatedAt = q.UpdatedAt
                    })
                    .ToListAsync();

                var response = new QuestionListResponseDto
                {
                    Questions = questions,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<QuestionListResponseDto>.SuccessResponse(response, "Questions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting questions: {ex.Message}");
                return ApiResponse<QuestionListResponseDto>.ErrorResponse("Failed to retrieve questions");
            }
        }

        public async Task<ApiResponse<QuestionResponseDto>> GetQuestionByIdAsync(int questionId)
        {
            try
            {
                var question = await _context.Questions
                    .Include(q => q.Teacher)
                    .Include(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(q => q.QuestionId == questionId);

                if (question == null)
                {
                    return ApiResponse<QuestionResponseDto>.ErrorResponse("Question not found");
                }

                var response = new QuestionResponseDto
                {
                    QuestionId = question.QuestionId,
                    TeacherId = question.TeacherId,
                    TeacherName = question.Teacher.FullName,
                    QuestionText = question.QuestionText,
                    QuestionType = question.QuestionType,
                    DifficultyLevel = question.DifficultyLevel,
                    Subject = question.Subject,
                    Options = question.QuestionOptions
                        .OrderBy(o => o.OptionOrder)
                        .Select(o => new QuestionOptionDto
                        {
                            OptionId = o.OptionId,
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect,
                            OptionOrder = o.OptionOrder
                        })
                        .ToList(),
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt
                };

                return ApiResponse<QuestionResponseDto>.SuccessResponse(response, "Question retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting question {questionId}: {ex.Message}");
                return ApiResponse<QuestionResponseDto>.ErrorResponse("Failed to retrieve question");
            }
        }

        public async Task<ApiResponse<QuestionResponseDto>> CreateQuestionAsync(
            CreateQuestionRequestDto request,
            int teacherId)
        {
            try
            {
                // Validate question type
                if (!QuestionTypes.IsValid(request.QuestionType))
                {
                    return ApiResponse<QuestionResponseDto>.ErrorResponse("Invalid question type");
                }

                // Validate difficulty level
                if (!string.IsNullOrEmpty(request.DifficultyLevel) && !DifficultyLevels.IsValid(request.DifficultyLevel))
                {
                    return ApiResponse<QuestionResponseDto>.ErrorResponse("Invalid difficulty level");
                }

                // Validate options
                var validationError = ValidateQuestionOptions(request.Options, request.QuestionType);
                if (validationError != null)
                {
                    return ApiResponse<QuestionResponseDto>.ErrorResponse(validationError);
                }

                var question = new Question
                {
                    TeacherId = teacherId,
                    QuestionText = request.QuestionText,
                    QuestionType = request.QuestionType,
                    DifficultyLevel = request.DifficultyLevel,
                    Subject = request.Subject,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                // Add options
                var options = request.Options.Select(o => new QuestionOption
                {
                    QuestionId = question.QuestionId,
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect,
                    OptionOrder = o.OptionOrder
                }).ToList();

                _context.QuestionOptions.AddRange(options);
                await _context.SaveChangesAsync();

                // Reload with relationships
                await _context.Entry(question).Reference(q => q.Teacher).LoadAsync();
                await _context.Entry(question).Collection(q => q.QuestionOptions).LoadAsync();

                var response = new QuestionResponseDto
                {
                    QuestionId = question.QuestionId,
                    TeacherId = question.TeacherId,
                    TeacherName = question.Teacher.FullName,
                    QuestionText = question.QuestionText,
                    QuestionType = question.QuestionType,
                    DifficultyLevel = question.DifficultyLevel,
                    Subject = question.Subject,
                    Options = question.QuestionOptions
                        .OrderBy(o => o.OptionOrder)
                        .Select(o => new QuestionOptionDto
                        {
                            OptionId = o.OptionId,
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect,
                            OptionOrder = o.OptionOrder
                        })
                        .ToList(),
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt
                };

                _logger.LogInformation($"Question {question.QuestionId} created successfully");
                return ApiResponse<QuestionResponseDto>.SuccessResponse(response, "Question created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating question: {ex.Message}");
                return ApiResponse<QuestionResponseDto>.ErrorResponse("Failed to create question");
            }
        }

        public async Task<ApiResponse<BulkCreateQuestionsResponseDto>> CreateQuestionsAsync(
            BulkCreateQuestionsRequestDto request,
            int teacherId)
        {
            var response = new BulkCreateQuestionsResponseDto
            {
                TotalProcessed = request.Questions.Count
            };

            for (int i = 0; i < request.Questions.Count; i++)
            {
                var questionRequest = request.Questions[i];
                try
                {
                    var result = await CreateQuestionAsync(questionRequest, teacherId);

                    if (result.Success && result.Data != null)
                    {
                        response.SuccessfulQuestions.Add(result.Data);
                    }
                    else
                    {
                        response.FailedQuestions.Add(new QuestionCreationFailedDto
                        {
                            QuestionText = questionRequest.QuestionText,
                            Reason = result.Message,
                            Index = i + 1
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating question at index {i}: {ex.Message}");
                    response.FailedQuestions.Add(new QuestionCreationFailedDto
                    {
                        QuestionText = questionRequest.QuestionText,
                        Reason = "Internal error",
                        Index = i + 1
                    });
                }
            }

            response.SuccessCount = response.SuccessfulQuestions.Count;
            response.FailedCount = response.FailedQuestions.Count;

            _logger.LogInformation($"Bulk creation completed: {response.SuccessCount} succeeded, {response.FailedCount} failed");

            return ApiResponse<BulkCreateQuestionsResponseDto>.SuccessResponse(
                response,
                $"Processed {response.TotalProcessed} questions: {response.SuccessCount} created, {response.FailedCount} failed");
        }

        public async Task<ApiResponse<QuestionResponseDto>> UpdateQuestionAsync(
            int questionId,
            UpdateQuestionRequestDto request)
        {
            try
            {
                var question = await _context.Questions
                    .Include(q => q.Teacher)
                    .Include(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(q => q.QuestionId == questionId);

                if (question == null)
                {
                    return ApiResponse<QuestionResponseDto>.ErrorResponse("Question not found");
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.QuestionText))
                {
                    question.QuestionText = request.QuestionText;
                }

                if (!string.IsNullOrEmpty(request.QuestionType))
                {
                    if (!QuestionTypes.IsValid(request.QuestionType))
                    {
                        return ApiResponse<QuestionResponseDto>.ErrorResponse("Invalid question type");
                    }
                    question.QuestionType = request.QuestionType;
                }

                if (!string.IsNullOrEmpty(request.DifficultyLevel))
                {
                    if (!DifficultyLevels.IsValid(request.DifficultyLevel))
                    {
                        return ApiResponse<QuestionResponseDto>.ErrorResponse("Invalid difficulty level");
                    }
                    question.DifficultyLevel = request.DifficultyLevel;
                }

                if (request.Subject != null)
                {
                    question.Subject = request.Subject;
                }

                // Update options if provided
                if (request.Options != null && request.Options.Count > 0)
                {
                    var validationError = ValidateUpdateQuestionOptions(request.Options, question.QuestionType);
                    if (validationError != null)
                    {
                        return ApiResponse<QuestionResponseDto>.ErrorResponse(validationError);
                    }

                    // Remove old options
                    _context.QuestionOptions.RemoveRange(question.QuestionOptions);

                    // Add new options
                    var newOptions = request.Options.Select(o => new QuestionOption
                    {
                        QuestionId = question.QuestionId,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect,
                        OptionOrder = o.OptionOrder
                    }).ToList();

                    _context.QuestionOptions.AddRange(newOptions);
                }

                question.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Reload options
                await _context.Entry(question).Collection(q => q.QuestionOptions).LoadAsync();

                var response = new QuestionResponseDto
                {
                    QuestionId = question.QuestionId,
                    TeacherId = question.TeacherId,
                    TeacherName = question.Teacher.FullName,
                    QuestionText = question.QuestionText,
                    QuestionType = question.QuestionType,
                    DifficultyLevel = question.DifficultyLevel,
                    Subject = question.Subject,
                    Options = question.QuestionOptions
                        .OrderBy(o => o.OptionOrder)
                        .Select(o => new QuestionOptionDto
                        {
                            OptionId = o.OptionId,
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect,
                            OptionOrder = o.OptionOrder
                        })
                        .ToList(),
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt
                };

                _logger.LogInformation($"Question {questionId} updated successfully");
                return ApiResponse<QuestionResponseDto>.SuccessResponse(response, "Question updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating question {questionId}: {ex.Message}");
                return ApiResponse<QuestionResponseDto>.ErrorResponse("Failed to update question");
            }
        }

        public async Task<ApiResponse<string>> DeleteQuestionAsync(int questionId)
        {
            try
            {
                var question = await _context.Questions.FindAsync(questionId);

                if (question == null)
                {
                    return ApiResponse<string>.ErrorResponse("Question not found");
                }

                // Check if question is in use
                if (await IsQuestionInUseAsync(questionId))
                {
                    return ApiResponse<string>.ErrorResponse("Cannot delete question that is used in exams");
                }

                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Question {questionId} deleted successfully");
                return ApiResponse<string>.SuccessResponse("Question deleted successfully", "Question deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting question {questionId}: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to delete question");
            }
        }

        public async Task<ApiResponse<QuestionStatisticsDto>> GetQuestionStatisticsAsync(int? teacherId = null)
        {
            try
            {
                var query = _context.Questions.AsQueryable();

                if (teacherId.HasValue)
                {
                    query = query.Where(q => q.TeacherId == teacherId.Value);
                }

                var totalQuestions = await query.CountAsync();

                var questionsByType = await query
                    .GroupBy(q => q.QuestionType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Type, x => x.Count);

                var questionsByDifficulty = await query
                    .Where(q => q.DifficultyLevel != null)
                    .GroupBy(q => q.DifficultyLevel!)
                    .Select(g => new { Level = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Level, x => x.Count);

                var questionsBySubject = await query
                    .Where(q => q.Subject != null)
                    .GroupBy(q => q.Subject!)
                    .Select(g => new { Subject = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Subject, x => x.Count);

                var today = DateTime.UtcNow.Date;
                var weekAgo = DateTime.UtcNow.AddDays(-7);
                var monthAgo = DateTime.UtcNow.AddMonths(-1);

                var questionsCreatedToday = await query.CountAsync(q => q.CreatedAt.Date == today);
                var questionsCreatedThisWeek = await query.CountAsync(q => q.CreatedAt >= weekAgo);
                var questionsCreatedThisMonth = await query.CountAsync(q => q.CreatedAt >= monthAgo);

                var topCreators = await query
                    .GroupBy(q => new { q.TeacherId, q.Teacher.FullName, q.Teacher.Email })
                    .Select(g => new TopQuestionCreatorDto
                    {
                        UserId = g.Key.TeacherId,
                        FullName = g.Key.FullName,
                        Email = g.Key.Email,
                        QuestionCount = g.Count()
                    })
                    .OrderByDescending(c => c.QuestionCount)
                    .Take(10)
                    .ToListAsync();

                var statistics = new QuestionStatisticsDto
                {
                    TotalQuestions = totalQuestions,
                    QuestionsByType = questionsByType,
                    QuestionsByDifficulty = questionsByDifficulty,
                    QuestionsBySubject = questionsBySubject,
                    QuestionsCreatedToday = questionsCreatedToday,
                    QuestionsCreatedThisWeek = questionsCreatedThisWeek,
                    QuestionsCreatedThisMonth = questionsCreatedThisMonth,
                    TopCreators = topCreators
                };

                return ApiResponse<QuestionStatisticsDto>.SuccessResponse(statistics, "Statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting question statistics: {ex.Message}");
                return ApiResponse<QuestionStatisticsDto>.ErrorResponse("Failed to retrieve statistics");
            }
        }

        public async Task<bool> CanAccessQuestionAsync(int questionId, int teacherId)
        {
            try
            {
                var question = await _context.Questions.FindAsync(questionId);
                return question != null && question.TeacherId == teacherId;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsQuestionInUseAsync(int questionId)
        {
            // Check if question is used in any exams
            // This will be implemented when ExamQuestions table is added
            // For now, return false
            return await Task.FromResult(false);
        }

        private string? ValidateQuestionOptions(List<CreateQuestionOptionDto> options, string questionType)
        {
            if (options == null || options.Count == 0)
            {
                return "At least one option is required";
            }

            if (options.Count < 2)
            {
                return "At least 2 options are required";
            }

            var correctOptions = options.Count(o => o.IsCorrect);

            if (questionType == QuestionTypes.SingleChoice || questionType == QuestionTypes.TrueFalse)
            {
                if (correctOptions != 1)
                {
                    return $"{questionType} must have exactly one correct answer";
                }
            }
            else if (questionType == QuestionTypes.MultipleChoice)
            {
                if (correctOptions < 1)
                {
                    return "Multiple choice question must have at least one correct answer";
                }
            }

            // Check for duplicate option orders
            var duplicateOrders = options.GroupBy(o => o.OptionOrder).Where(g => g.Count() > 1).Any();
            if (duplicateOrders)
            {
                return "Option orders must be unique";
            }

            return null;
        }

        private string? ValidateUpdateQuestionOptions(List<UpdateQuestionOptionDto> options, string questionType)
        {
            if (options == null || options.Count == 0)
            {
                return "At least one option is required";
            }

            if (options.Count < 2)
            {
                return "At least 2 options are required";
            }

            var correctOptions = options.Count(o => o.IsCorrect);

            if (questionType == QuestionTypes.SingleChoice || questionType == QuestionTypes.TrueFalse)
            {
                if (correctOptions != 1)
                {
                    return $"{questionType} must have exactly one correct answer";
                }
            }
            else if (questionType == QuestionTypes.MultipleChoice)
            {
                if (correctOptions < 1)
                {
                    return "Multiple choice question must have at least one correct answer";
                }
            }

            // Check for duplicate option orders
            var duplicateOrders = options.GroupBy(o => o.OptionOrder).Where(g => g.Count() > 1).Any();
            if (duplicateOrders)
            {
                return "Option orders must be unique";
            }

            return null;
        }
    }
}
