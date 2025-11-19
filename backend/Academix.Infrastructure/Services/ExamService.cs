using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Exams;
using Academix.Application.DTOs.Questions;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academix.Infrastructure.Services
{
    public class ExamService : IExamService
    {
        private readonly AcademixDbContext _context;
        private readonly ILogger<ExamService> _logger;

        public ExamService(AcademixDbContext context, ILogger<ExamService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Part 1: Exam Management (CRUD)
        public async Task<ApiResponse<ExamListResponseDto>> GetExamsByClassAsync(
            int classId,
            bool? isPublished = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            try
            {
                var query = _context.Exams
                    .Include(e => e.CreatedByNavigation)
                    .Include(e => e.Class)
                    .Include(e => e.ExamQuestions)
                    .Include(e => e.StudentExamAttempts)
                    .Where(e => e.ClassId == classId)
                    .AsQueryable();

                if (isPublished.HasValue)
                {
                    query = query.Where(e => e.IsPublished == isPublished.Value);
                }

                var totalCount = await query.CountAsync();

                query = sortBy.ToLower() switch
                {
                    "title" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(e => e.Title)
                        : query.OrderByDescending(e => e.Title),
                    "starttime" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(e => e.StartTime)
                        : query.OrderByDescending(e => e.StartTime),
                    _ => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(e => e.CreatedAt)
                        : query.OrderByDescending(e => e.CreatedAt)
                };

                var exams = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new ExamResponseDto
                    {
                        ExamId = e.ExamId,
                        ClassId = e.ClassId,
                        ClassName = e.Class.ClassName,
                        Title = e.Title,
                        Description = e.Description,
                        Duration = e.Duration,
                        TotalMarks = e.TotalMarks,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        CreatedBy = e.CreatedBy,
                        CreatedByName = e.CreatedByNavigation.FullName,
                        IsPublished = e.IsPublished,
                        QuestionCount = e.ExamQuestions.Count,
                        AttemptCount = e.StudentExamAttempts.Count,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt
                    })
                    .ToListAsync();

                var response = new ExamListResponseDto
                {
                    Exams = exams,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<ExamListResponseDto>.SuccessResponse(response, "Exams retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting exams for class {classId}: {ex.Message}");
                return ApiResponse<ExamListResponseDto>.ErrorResponse("Failed to retrieve exams");
            }
        }

        public async Task<ApiResponse<ExamDetailResponseDto>> GetExamByIdAsync(int examId)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.CreatedByNavigation)
                    .Include(e => e.Class)
                    .Include(e => e.ExamQuestions)
                        .ThenInclude(eq => eq.Question)
                            .ThenInclude(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(e => e.ExamId == examId);

                if (exam == null)
                {
                    return ApiResponse<ExamDetailResponseDto>.ErrorResponse("Exam not found");
                }

                var questions = exam.ExamQuestions
                    .OrderBy(eq => eq.QuestionOrder)
                    .Select(eq => new ExamQuestionDetailDto
                    {
                        ExamQuestionId = eq.ExamQuestionId,
                        QuestionId = eq.QuestionId,
                        QuestionText = eq.Question.QuestionText,
                        QuestionType = eq.Question.QuestionType,
                        QuestionOrder = eq.QuestionOrder,
                        Marks = eq.Marks,
                        Options = eq.Question.QuestionOptions
                            .OrderBy(o => o.OptionOrder)
                            .Select(o => new QuestionOptionDto
                            {
                                OptionId = o.OptionId,
                                OptionText = o.OptionText,
                                IsCorrect = o.IsCorrect,
                                OptionOrder = o.OptionOrder
                            })
                            .ToList()
                    })
                    .ToList();

                var response = new ExamDetailResponseDto
                {
                    ExamId = exam.ExamId,
                    ClassId = exam.ClassId,
                    ClassName = exam.Class.ClassName,
                    Title = exam.Title,
                    Description = exam.Description,
                    Duration = exam.Duration,
                    TotalMarks = exam.TotalMarks,
                    StartTime = exam.StartTime,
                    EndTime = exam.EndTime,
                    CreatedBy = exam.CreatedBy,
                    CreatedByName = exam.CreatedByNavigation.FullName,
                    IsPublished = exam.IsPublished,
                    Questions = questions,
                    CreatedAt = exam.CreatedAt,
                    UpdatedAt = exam.UpdatedAt
                };

                return ApiResponse<ExamDetailResponseDto>.SuccessResponse(response, "Exam retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting exam {examId}: {ex.Message}");
                return ApiResponse<ExamDetailResponseDto>.ErrorResponse("Failed to retrieve exam");
            }
        }

        public async Task<ApiResponse<ExamResponseDto>> CreateExamAsync(
            int classId,
            CreateExamRequestDto request,
            int createdBy)
        {
            try
            {
                // Validate class exists
                var classExists = await _context.Classes.AnyAsync(c => c.ClassId == classId);
                if (!classExists)
                {
                    return ApiResponse<ExamResponseDto>.ErrorResponse("Class not found");
                }

                // Validate questions belong to teacher
                var questionIds = request.Questions.Select(q => q.QuestionId).ToList();
                var questions = await _context.Questions
                    .Where(q => questionIds.Contains(q.QuestionId))
                    .ToListAsync();

                if (questions.Count != questionIds.Count)
                {
                    return ApiResponse<ExamResponseDto>.ErrorResponse("Some questions not found");
                }

                var invalidQuestions = questions.Where(q => q.TeacherId != createdBy).ToList();
                if (invalidQuestions.Any())
                {
                    return ApiResponse<ExamResponseDto>.ErrorResponse("Cannot use questions from other teachers");
                }

                // Validate question orders are unique
                var duplicateOrders = request.Questions
                    .GroupBy(q => q.QuestionOrder)
                    .Where(g => g.Count() > 1)
                    .Any();
                if (duplicateOrders)
                {
                    return ApiResponse<ExamResponseDto>.ErrorResponse("Question orders must be unique");
                }

                var exam = new Exam
                {
                    ClassId = classId,
                    Title = request.Title,
                    Description = request.Description,
                    Duration = request.Duration,
                    TotalMarks = request.TotalMarks,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    CreatedBy = createdBy,
                    IsPublished = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();

                // Add questions
                var examQuestions = request.Questions.Select(q => new ExamQuestion
                {
                    ExamId = exam.ExamId,
                    QuestionId = q.QuestionId,
                    QuestionOrder = q.QuestionOrder,
                    Marks = q.Marks
                }).ToList();

                _context.ExamQuestions.AddRange(examQuestions);
                await _context.SaveChangesAsync();

                // Reload with relationships
                await _context.Entry(exam).Reference(e => e.CreatedByNavigation).LoadAsync();
                await _context.Entry(exam).Reference(e => e.Class).LoadAsync();

                var response = new ExamResponseDto
                {
                    ExamId = exam.ExamId,
                    ClassId = exam.ClassId,
                    ClassName = exam.Class.ClassName,
                    Title = exam.Title,
                    Description = exam.Description,
                    Duration = exam.Duration,
                    TotalMarks = exam.TotalMarks,
                    StartTime = exam.StartTime,
                    EndTime = exam.EndTime,
                    CreatedBy = exam.CreatedBy,
                    CreatedByName = exam.CreatedByNavigation.FullName,
                    IsPublished = exam.IsPublished,
                    QuestionCount = examQuestions.Count,
                    AttemptCount = 0,
                    CreatedAt = exam.CreatedAt,
                    UpdatedAt = exam.UpdatedAt
                };

                _logger.LogInformation($"Exam {exam.ExamId} created successfully");
                return ApiResponse<ExamResponseDto>.SuccessResponse(response, "Exam created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating exam: {ex.Message}");
                return ApiResponse<ExamResponseDto>.ErrorResponse("Failed to create exam");
            }
        }

        // Part 2: Exam Editing & Question Management
        public async Task<ApiResponse<ExamResponseDto>> UpdateExamAsync(int examId, UpdateExamRequestDto request)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.CreatedByNavigation)
                    .Include(e => e.Class)
                    .Include(e => e.ExamQuestions)
                    .Include(e => e.StudentExamAttempts)
                    .FirstOrDefaultAsync(e => e.ExamId == examId);

                if (exam == null)
                {
                    return ApiResponse<ExamResponseDto>.ErrorResponse("Exam not found");
                }

                // Cannot update published exam with attempts
                if (exam.IsPublished && exam.StudentExamAttempts.Any())
                {
                    return ApiResponse<ExamResponseDto>.ErrorResponse("Cannot update exam with existing attempts");
                }

                if (!string.IsNullOrEmpty(request.Title))
                {
                    exam.Title = request.Title;
                }

                if (request.Description != null)
                {
                    exam.Description = request.Description;
                }

                if (request.Duration.HasValue)
                {
                    exam.Duration = request.Duration.Value;
                }

                if (request.TotalMarks.HasValue)
                {
                    exam.TotalMarks = request.TotalMarks.Value;
                }

                if (request.StartTime.HasValue)
                {
                    exam.StartTime = request.StartTime;
                }

                if (request.EndTime.HasValue)
                {
                    exam.EndTime = request.EndTime;
                }

                exam.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var response = new ExamResponseDto
                {
                    ExamId = exam.ExamId,
                    ClassId = exam.ClassId,
                    ClassName = exam.Class.ClassName,
                    Title = exam.Title,
                    Description = exam.Description,
                    Duration = exam.Duration,
                    TotalMarks = exam.TotalMarks,
                    StartTime = exam.StartTime,
                    EndTime = exam.EndTime,
                    CreatedBy = exam.CreatedBy,
                    CreatedByName = exam.CreatedByNavigation.FullName,
                    IsPublished = exam.IsPublished,
                    QuestionCount = exam.ExamQuestions.Count,
                    AttemptCount = exam.StudentExamAttempts.Count,
                    CreatedAt = exam.CreatedAt,
                    UpdatedAt = exam.UpdatedAt
                };

                _logger.LogInformation($"Exam {examId} updated successfully");
                return ApiResponse<ExamResponseDto>.SuccessResponse(response, "Exam updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating exam {examId}: {ex.Message}");
                return ApiResponse<ExamResponseDto>.ErrorResponse("Failed to update exam");
            }
        }

        public async Task<ApiResponse<string>> DeleteExamAsync(int examId)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.StudentExamAttempts)
                    .FirstOrDefaultAsync(e => e.ExamId == examId);

                if (exam == null)
                {
                    return ApiResponse<string>.ErrorResponse("Exam not found");
                }

                if (exam.StudentExamAttempts.Any())
                {
                    return ApiResponse<string>.ErrorResponse("Cannot delete exam with existing attempts");
                }

                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Exam {examId} deleted successfully");
                return ApiResponse<string>.SuccessResponse("Exam deleted successfully", "Exam deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting exam {examId}: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to delete exam");
            }
        }

        public async Task<ApiResponse<string>> PublishExamAsync(int examId)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.ExamQuestions)
                    .FirstOrDefaultAsync(e => e.ExamId == examId);

                if (exam == null)
                {
                    return ApiResponse<string>.ErrorResponse("Exam not found");
                }

                if (exam.IsPublished)
                {
                    return ApiResponse<string>.ErrorResponse("Exam is already published");
                }

                if (!exam.ExamQuestions.Any())
                {
                    return ApiResponse<string>.ErrorResponse("Cannot publish exam without questions");
                }

                exam.IsPublished = true;
                exam.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Exam {examId} published successfully");
                return ApiResponse<string>.SuccessResponse("Exam published successfully", "Exam published successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing exam {examId}: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to publish exam");
            }
        }

        public async Task<ApiResponse<List<ExamQuestionDetailDto>>> GetExamQuestionsAsync(int examId)
        {
            try
            {
                var questions = await _context.ExamQuestions
                    .Include(eq => eq.Question)
                        .ThenInclude(q => q.QuestionOptions)
                    .Where(eq => eq.ExamId == examId)
                    .OrderBy(eq => eq.QuestionOrder)
                    .Select(eq => new ExamQuestionDetailDto
                    {
                        ExamQuestionId = eq.ExamQuestionId,
                        QuestionId = eq.QuestionId,
                        QuestionText = eq.Question.QuestionText,
                        QuestionType = eq.Question.QuestionType,
                        QuestionOrder = eq.QuestionOrder,
                        Marks = eq.Marks,
                        Options = eq.Question.QuestionOptions
                            .OrderBy(o => o.OptionOrder)
                            .Select(o => new QuestionOptionDto
                            {
                                OptionId = o.OptionId,
                                OptionText = o.OptionText,
                                IsCorrect = o.IsCorrect,
                                OptionOrder = o.OptionOrder
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return ApiResponse<List<ExamQuestionDetailDto>>.SuccessResponse(questions, "Questions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting exam questions: {ex.Message}");
                return ApiResponse<List<ExamQuestionDetailDto>>.ErrorResponse("Failed to retrieve questions");
            }
        }

        public async Task<ApiResponse<string>> AddQuestionsToExamAsync(int examId, AddQuestionsToExamRequestDto request)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.StudentExamAttempts)
                    .Include(e => e.ExamQuestions)
                    .FirstOrDefaultAsync(e => e.ExamId == examId);

                if (exam == null)
                {
                    return ApiResponse<string>.ErrorResponse("Exam not found");
                }

                if (exam.IsPublished && exam.StudentExamAttempts.Any())
                {
                    return ApiResponse<string>.ErrorResponse("Cannot modify exam with existing attempts");
                }

                var questionIds = request.Questions.Select(q => q.QuestionId).ToList();
                var existingQuestions = exam.ExamQuestions.Select(eq => eq.QuestionId).ToList();
                var newQuestionIds = questionIds.Except(existingQuestions).ToList();

                if (!newQuestionIds.Any())
                {
                    return ApiResponse<string>.ErrorResponse("All questions are already in the exam");
                }

                var questions = await _context.Questions
                    .Where(q => newQuestionIds.Contains(q.QuestionId))
                    .ToListAsync();

                if (questions.Count != newQuestionIds.Count)
                {
                    return ApiResponse<string>.ErrorResponse("Some questions not found");
                }

                var examQuestions = request.Questions
                    .Where(q => newQuestionIds.Contains(q.QuestionId))
                    .Select(q => new ExamQuestion
                    {
                        ExamId = examId,
                        QuestionId = q.QuestionId,
                        QuestionOrder = q.QuestionOrder,
                        Marks = q.Marks
                    })
                    .ToList();

                _context.ExamQuestions.AddRange(examQuestions);
                exam.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Added {examQuestions.Count} questions to exam {examId}");
                return ApiResponse<string>.SuccessResponse(
                    $"{examQuestions.Count} questions added successfully",
                    "Questions added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding questions to exam: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to add questions");
            }
        }

        public async Task<ApiResponse<string>> RemoveQuestionFromExamAsync(int examId, int questionId)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.StudentExamAttempts)
                    .FirstOrDefaultAsync(e => e.ExamId == examId);

                if (exam == null)
                {
                    return ApiResponse<string>.ErrorResponse("Exam not found");
                }

                if (exam.IsPublished && exam.StudentExamAttempts.Any())
                {
                    return ApiResponse<string>.ErrorResponse("Cannot modify exam with existing attempts");
                }

                var examQuestion = await _context.ExamQuestions
                    .FirstOrDefaultAsync(eq => eq.ExamId == examId && eq.QuestionId == questionId);

                if (examQuestion == null)
                {
                    return ApiResponse<string>.ErrorResponse("Question not found in exam");
                }

                _context.ExamQuestions.Remove(examQuestion);
                exam.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Question {questionId} removed from exam {examId}");
                return ApiResponse<string>.SuccessResponse("Question removed successfully", "Question removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing question from exam: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to remove question");
            }
        }

        // Part 3: Student Exam Taking
        public async Task<ApiResponse<StartExamResponseDto>> StartExamAsync(int examId, int studentId)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.ExamQuestions)
                        .ThenInclude(eq => eq.Question)
                            .ThenInclude(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(e => e.ExamId == examId);

                if (exam == null)
                {
                    return ApiResponse<StartExamResponseDto>.ErrorResponse("Exam not found");
                }

                if (!exam.IsPublished)
                {
                    return ApiResponse<StartExamResponseDto>.ErrorResponse("Exam is not published yet");
                }

                // Check if exam is within time window
                var now = DateTime.UtcNow;
                if (exam.StartTime.HasValue && now < exam.StartTime.Value)
                {
                    return ApiResponse<StartExamResponseDto>.ErrorResponse("Exam has not started yet");
                }

                if (exam.EndTime.HasValue && now > exam.EndTime.Value)
                {
                    return ApiResponse<StartExamResponseDto>.ErrorResponse("Exam has ended");
                }

                // IMPORTANT: Check if student has already attempted this exam (one attempt per student)
                var existingAttempt = await _context.StudentExamAttempts
                    .AnyAsync(a => a.ExamId == examId && a.StudentId == studentId);

                if (existingAttempt)
                {
                    return ApiResponse<StartExamResponseDto>.ErrorResponse("You have already taken this exam. Each student can only take an exam once.");
                }

                // Check if student can take exam
                if (!await CanTakeExamAsync(examId, studentId))
                {
                    return ApiResponse<StartExamResponseDto>.ErrorResponse("You are not enrolled in this class");
                }

                // Create attempt
                var attempt = new StudentExamAttempt
                {
                    ExamId = examId,
                    StudentId = studentId,
                    StartTime = DateTime.UtcNow,
                    Status = ExamStatus.InProgress
                };

                _context.StudentExamAttempts.Add(attempt);
                await _context.SaveChangesAsync();

                var endTime = attempt.StartTime.AddMinutes(exam.Duration);

                var questions = exam.ExamQuestions
                    .OrderBy(eq => eq.QuestionOrder)
                    .Select(eq => new ExamQuestionDetailDto
                    {
                        ExamQuestionId = eq.ExamQuestionId,
                        QuestionId = eq.QuestionId,
                        QuestionText = eq.Question.QuestionText,
                        QuestionType = eq.Question.QuestionType,
                        QuestionOrder = eq.QuestionOrder,
                        Marks = eq.Marks,
                        Options = eq.Question.QuestionOptions
                            .OrderBy(o => o.OptionOrder)
                            .Select(o => new QuestionOptionDto
                            {
                                OptionId = o.OptionId,
                                OptionText = o.OptionText,
                                IsCorrect = false, // Don't show correct answers during exam
                                OptionOrder = o.OptionOrder
                            })
                            .ToList()
                    })
                    .ToList();

                var response = new StartExamResponseDto
                {
                    AttemptId = attempt.AttemptId,
                    ExamId = exam.ExamId,
                    Title = exam.Title,
                    Description = exam.Description,
                    Duration = exam.Duration,
                    StartTime = attempt.StartTime,
                    EndTime = endTime,
                    Questions = questions
                };

                _logger.LogInformation($"Student {studentId} started exam {examId}");
                return ApiResponse<StartExamResponseDto>.SuccessResponse(response, "Exam started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting exam: {ex.Message}");
                return ApiResponse<StartExamResponseDto>.ErrorResponse("Failed to start exam");
            }
        }

        public async Task<ApiResponse<string>> SubmitAnswerAsync(int attemptId, SubmitAnswerRequestDto request)
        {
            try
            {
                var attempt = await _context.StudentExamAttempts
                    .Include(a => a.Exam)
                    .FirstOrDefaultAsync(a => a.AttemptId == attemptId);

                if (attempt == null)
                {
                    return ApiResponse<string>.ErrorResponse("Attempt not found");
                }

                if (attempt.Status != ExamStatus.InProgress)
                {
                    return ApiResponse<string>.ErrorResponse("Exam is not in progress");
                }

                // Check if time has expired
                var endTime = attempt.StartTime.AddMinutes(attempt.Exam.Duration);
                if (DateTime.UtcNow > endTime)
                {
                    return ApiResponse<string>.ErrorResponse("Exam time has expired");
                }

                // Check if answer already exists
                var existingAnswer = await _context.StudentAnswers
                    .FirstOrDefaultAsync(sa => sa.AttemptId == attemptId && sa.QuestionId == request.QuestionId);

                if (existingAnswer != null)
                {
                    // Update existing answer
                    existingAnswer.SelectedOptionId = request.SelectedOptionId;
                    existingAnswer.AnsweredAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new answer
                    var answer = new StudentAnswer
                    {
                        AttemptId = attemptId,
                        QuestionId = request.QuestionId,
                        SelectedOptionId = request.SelectedOptionId,
                        AnsweredAt = DateTime.UtcNow
                    };

                    _context.StudentAnswers.Add(answer);
                }

                await _context.SaveChangesAsync();

                return ApiResponse<string>.SuccessResponse("Answer saved successfully", "Answer submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error submitting answer: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to submit answer");
            }
        }

        public async Task<ApiResponse<ExamResultResponseDto>> SubmitExamAsync(int attemptId, SubmitExamRequestDto request)
        {
            try
            {
                var attempt = await _context.StudentExamAttempts
                    .Include(a => a.Exam)
                        .ThenInclude(e => e.ExamQuestions)
                    .Include(a => a.Student)
                    .Include(a => a.StudentAnswers)
                    .FirstOrDefaultAsync(a => a.AttemptId == attemptId);

                if (attempt == null)
                {
                    return ApiResponse<ExamResultResponseDto>.ErrorResponse("Attempt not found");
                }

                if (attempt.Status != ExamStatus.InProgress)
                {
                    return ApiResponse<ExamResultResponseDto>.ErrorResponse("Exam is not in progress");
                }

                // Save all answers
                foreach (var answer in request.Answers)
                {
                    var existingAnswer = attempt.StudentAnswers
                        .FirstOrDefault(sa => sa.QuestionId == answer.QuestionId);

                    if (existingAnswer != null)
                    {
                        existingAnswer.SelectedOptionId = answer.SelectedOptionId;
                        existingAnswer.AnsweredAt = DateTime.UtcNow;
                    }
                    else
                    {
                        var newAnswer = new StudentAnswer
                        {
                            AttemptId = attemptId,
                            QuestionId = answer.QuestionId,
                            SelectedOptionId = answer.SelectedOptionId,
                            AnsweredAt = DateTime.UtcNow
                        };

                        _context.StudentAnswers.Add(newAnswer);
                    }
                }

                attempt.SubmitTime = DateTime.UtcNow;
                attempt.Status = ExamStatus.Submitted;

                await _context.SaveChangesAsync();

                // Grade the exam
                await GradeExamAsync(attemptId);

                // Get result
                var result = await GetMyExamResultAsync(attemptId, attempt.StudentId);

                _logger.LogInformation($"Student {attempt.StudentId} submitted exam {attempt.ExamId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error submitting exam: {ex.Message}");
                return ApiResponse<ExamResultResponseDto>.ErrorResponse("Failed to submit exam");
            }
        }

        private async Task GradeExamAsync(int attemptId)
        {
            var attempt = await _context.StudentExamAttempts
                .Include(a => a.Exam)
                    .ThenInclude(e => e.ExamQuestions)
                .Include(a => a.StudentAnswers)
                .FirstOrDefaultAsync(a => a.AttemptId == attemptId);

            if (attempt == null) return;

            decimal totalScore = 0;

            foreach (var answer in attempt.StudentAnswers)
            {
                var examQuestion = attempt.Exam.ExamQuestions
                    .FirstOrDefault(eq => eq.QuestionId == answer.QuestionId);

                if (examQuestion == null) continue;

                // Check if answer is correct
                var selectedOption = await _context.QuestionOptions
                    .FirstOrDefaultAsync(o => o.OptionId == answer.SelectedOptionId);

                if (selectedOption != null && selectedOption.IsCorrect)
                {
                    answer.IsCorrect = true;
                    answer.MarksObtained = examQuestion.Marks;
                    totalScore += examQuestion.Marks;
                }
                else
                {
                    answer.IsCorrect = false;
                    answer.MarksObtained = 0;
                }
            }

            attempt.TotalScore = totalScore;
            attempt.Status = ExamStatus.Graded;

            await _context.SaveChangesAsync();
        }

        // Part 4: Results & Validation
        public async Task<ApiResponse<ExamResultResponseDto>> GetMyExamResultAsync(int attemptId, int studentId)
        {
            try
            {
                var attempt = await _context.StudentExamAttempts
                    .Include(a => a.Exam)
                    .Include(a => a.Student)
                    .Include(a => a.StudentAnswers)
                        .ThenInclude(sa => sa.Question)
                            .ThenInclude(q => q.QuestionOptions)
                    .Include(a => a.StudentAnswers)
                        .ThenInclude(sa => sa.SelectedOption)
                    .FirstOrDefaultAsync(a => a.AttemptId == attemptId && a.StudentId == studentId);

                if (attempt == null)
                {
                    return ApiResponse<ExamResultResponseDto>.ErrorResponse("Result not found");
                }

                if (attempt.Status == ExamStatus.InProgress)
                {
                    return ApiResponse<ExamResultResponseDto>.ErrorResponse("Exam is still in progress");
                }

                var examQuestions = await _context.ExamQuestions
                    .Where(eq => eq.ExamId == attempt.ExamId)
                    .ToListAsync();

                var answers = attempt.StudentAnswers.Select(sa =>
                {
                    var correctOption = sa.Question.QuestionOptions.FirstOrDefault(o => o.IsCorrect);
                    var examQuestion = examQuestions.FirstOrDefault(eq => eq.QuestionId == sa.QuestionId);

                    return new StudentAnswerDetailDto
                    {
                        QuestionId = sa.QuestionId,
                        QuestionText = sa.Question.QuestionText,
                        SelectedOptionId = sa.SelectedOptionId,
                        SelectedOptionText = sa.SelectedOption?.OptionText,
                        CorrectOptionId = correctOption?.OptionId,
                        CorrectOptionText = correctOption?.OptionText,
                        IsCorrect = sa.IsCorrect,
                        MarksObtained = sa.MarksObtained ?? 0,
                        TotalMarks = examQuestion?.Marks ?? 0
                    };
                }).ToList();

                var correctAnswers = answers.Count(a => a.IsCorrect == true);
                var wrongAnswers = answers.Count(a => a.IsCorrect == false);

                var result = new ExamResultResponseDto
                {
                    AttemptId = attempt.AttemptId,
                    ExamId = attempt.ExamId,
                    ExamTitle = attempt.Exam.Title,
                    StudentId = attempt.StudentId,
                    StudentName = attempt.Student.FullName,
                    StartTime = attempt.StartTime,
                    SubmitTime = attempt.SubmitTime,
                    TotalScore = attempt.TotalScore ?? 0,
                    TotalMarks = attempt.Exam.TotalMarks,
                    Percentage = attempt.TotalScore.HasValue
                        ? Math.Round((attempt.TotalScore.Value / attempt.Exam.TotalMarks) * 100, 2)
                        : null,
                    Status = attempt.Status,
                    TotalQuestions = answers.Count,
                    CorrectAnswers = correctAnswers,
                    WrongAnswers = wrongAnswers,
                    Answers = answers
                };

                return ApiResponse<ExamResultResponseDto>.SuccessResponse(result, "Result retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting exam result: {ex.Message}");
                return ApiResponse<ExamResultResponseDto>.ErrorResponse("Failed to retrieve result");
            }
        }

        public async Task<ApiResponse<List<ExamResponseDto>>> GetMyExamsAsync(int studentId, int? classId = null)
        {
            try
            {
                var query = _context.Exams
                    .Include(e => e.Class)
                    .Include(e => e.CreatedByNavigation)
                    .Include(e => e.ExamQuestions)
                    .Include(e => e.StudentExamAttempts)
                    .Where(e => e.IsPublished)
                    .AsQueryable();

                if (classId.HasValue)
                {
                    query = query.Where(e => e.ClassId == classId.Value);
                }

                // Filter by classes student is enrolled in
                var studentClasses = await _context.ClassMembers
                    .Where(cm => cm.UserId == studentId && cm.Role == UserRoles.Student)
                    .Select(cm => cm.ClassId)
                    .ToListAsync();

                query = query.Where(e => studentClasses.Contains(e.ClassId));

                var exams = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .Select(e => new ExamResponseDto
                    {
                        ExamId = e.ExamId,
                        ClassId = e.ClassId,
                        ClassName = e.Class.ClassName,
                        Title = e.Title,
                        Description = e.Description,
                        Duration = e.Duration,
                        TotalMarks = e.TotalMarks,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        CreatedBy = e.CreatedBy,
                        CreatedByName = e.CreatedByNavigation.FullName,
                        IsPublished = e.IsPublished,
                        QuestionCount = e.ExamQuestions.Count,
                        AttemptCount = e.StudentExamAttempts.Count(a => a.StudentId == studentId),
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse<List<ExamResponseDto>>.SuccessResponse(exams, "Exams retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting student exams: {ex.Message}");
                return ApiResponse<List<ExamResponseDto>>.ErrorResponse("Failed to retrieve exams");
            }
        }

        public async Task<ApiResponse<List<ExamResultResponseDto>>> GetMyExamHistoryAsync(int studentId, int? classId = null)
        {
            try
            {
                var query = _context.StudentExamAttempts
                    .Include(a => a.Exam)
                        .ThenInclude(e => e.Class)
                    .Include(a => a.Student)
                    .Include(a => a.StudentAnswers)
                        .ThenInclude(sa => sa.Question)
                            .ThenInclude(q => q.QuestionOptions)
                    .Include(a => a.StudentAnswers)
                        .ThenInclude(sa => sa.SelectedOption)
                    .Where(a => a.StudentId == studentId)
                    .AsQueryable();

                if (classId.HasValue)
                {
                    query = query.Where(a => a.Exam.ClassId == classId.Value);
                }

                var attempts = await query
                    .OrderByDescending(a => a.StartTime)
                    .ToListAsync();

                var results = new List<ExamResultResponseDto>();

                foreach (var attempt in attempts)
                {
                    var examQuestions = await _context.ExamQuestions
                        .Where(eq => eq.ExamId == attempt.ExamId)
                        .ToListAsync();

                    var answers = attempt.StudentAnswers.Select(sa =>
                    {
                        var correctOption = sa.Question.QuestionOptions.FirstOrDefault(o => o.IsCorrect);
                        var examQuestion = examQuestions.FirstOrDefault(eq => eq.QuestionId == sa.QuestionId);

                        return new StudentAnswerDetailDto
                        {
                            QuestionId = sa.QuestionId,
                            QuestionText = sa.Question.QuestionText,
                            SelectedOptionId = sa.SelectedOptionId,
                            SelectedOptionText = sa.SelectedOption?.OptionText,
                            CorrectOptionId = correctOption?.OptionId,
                            CorrectOptionText = correctOption?.OptionText,
                            IsCorrect = sa.IsCorrect,
                            MarksObtained = sa.MarksObtained ?? 0,
                            TotalMarks = examQuestion?.Marks ?? 0
                        };
                    }).ToList();

                    var correctAnswers = answers.Count(a => a.IsCorrect == true);
                    var wrongAnswers = answers.Count(a => a.IsCorrect == false);

                    results.Add(new ExamResultResponseDto
                    {
                        AttemptId = attempt.AttemptId,
                        ExamId = attempt.ExamId,
                        ExamTitle = attempt.Exam.Title,
                        StudentId = attempt.StudentId,
                        StudentName = attempt.Student.FullName,
                        StartTime = attempt.StartTime,
                        SubmitTime = attempt.SubmitTime,
                        TotalScore = attempt.TotalScore ?? 0,
                        TotalMarks = attempt.Exam.TotalMarks,
                        Percentage = attempt.TotalScore.HasValue
                            ? Math.Round((attempt.TotalScore.Value / attempt.Exam.TotalMarks) * 100, 2)
                            : null,
                        Status = attempt.Status,
                        TotalQuestions = answers.Count,
                        CorrectAnswers = correctAnswers,
                        WrongAnswers = wrongAnswers,
                        Answers = answers
                    });
                }

                return ApiResponse<List<ExamResultResponseDto>>.SuccessResponse(results, "Exam history retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting student exam history: {ex.Message}");
                return ApiResponse<List<ExamResultResponseDto>>.ErrorResponse("Failed to retrieve exam history");
            }
        }

        public async Task<ApiResponse<ExamResultsListResponseDto>> GetExamResultsAsync(int examId, int page = 1, int pageSize = 10)
        {
            try
            {
                var exam = await _context.Exams.FindAsync(examId);
                if (exam == null)
                {
                    return ApiResponse<ExamResultsListResponseDto>.ErrorResponse("Exam not found");
                }

                var query = _context.StudentExamAttempts
                    .Include(a => a.Student)
                    .Include(a => a.StudentAnswers)
                    .Where(a => a.ExamId == examId && a.Status == ExamStatus.Graded)
                    .AsQueryable();

                var totalCount = await query.CountAsync();

                var attempts = await query
                    .OrderByDescending(a => a.TotalScore)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var results = attempts.Select(a => new ExamResultSummaryDto
                {
                    AttemptId = a.AttemptId,
                    StudentId = a.StudentId,
                    StudentName = a.Student.FullName,
                    StudentEmail = a.Student.Email,
                    StartTime = a.StartTime,
                    SubmitTime = a.SubmitTime,
                    TotalScore = a.TotalScore,
                    Percentage = a.TotalScore.HasValue
                        ? Math.Round((a.TotalScore.Value / exam.TotalMarks) * 100, 2)
                        : null,
                    Status = a.Status,
                    CorrectAnswers = a.StudentAnswers.Count(sa => sa.IsCorrect == true),
                    TotalQuestions = a.StudentAnswers.Count
                }).ToList();

                // Calculate statistics
                var allAttempts = await _context.StudentExamAttempts
                    .Where(a => a.ExamId == examId)
                    .ToListAsync();

                var completedAttempts = allAttempts.Where(a => a.Status == ExamStatus.Graded).ToList();

                var statistics = new ExamStatisticsSummaryDto
                {
                    AverageScore = completedAttempts.Any() ? completedAttempts.Average(a => a.TotalScore ?? 0) : null,
                    HighestScore = completedAttempts.Any() ? completedAttempts.Max(a => a.TotalScore ?? 0) : null,
                    LowestScore = completedAttempts.Any() ? completedAttempts.Min(a => a.TotalScore ?? 0) : null,
                    TotalAttempts = allAttempts.Count,
                    CompletedAttempts = completedAttempts.Count,
                    InProgressAttempts = allAttempts.Count(a => a.Status == ExamStatus.InProgress),
                    PassRate = completedAttempts.Any()
                        ? Math.Round((decimal)completedAttempts.Count(a => (a.TotalScore ?? 0) >= exam.TotalMarks * 0.5m) / completedAttempts.Count * 100, 2)
                        : null
                };

                var response = new ExamResultsListResponseDto
                {
                    Results = results,
                    Statistics = statistics,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<ExamResultsListResponseDto>.SuccessResponse(response, "Results retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting exam results: {ex.Message}");
                return ApiResponse<ExamResultsListResponseDto>.ErrorResponse("Failed to retrieve results");
            }
        }

        public async Task<ApiResponse<ExamResultResponseDto>> GetStudentExamResultAsync(int attemptId)
        {
            try
            {
                var attempt = await _context.StudentExamAttempts
                    .Include(a => a.Exam)
                    .Include(a => a.Student)
                    .Include(a => a.StudentAnswers)
                        .ThenInclude(sa => sa.Question)
                            .ThenInclude(q => q.QuestionOptions)
                    .Include(a => a.StudentAnswers)
                        .ThenInclude(sa => sa.SelectedOption)
                    .FirstOrDefaultAsync(a => a.AttemptId == attemptId);

                if (attempt == null)
                {
                    return ApiResponse<ExamResultResponseDto>.ErrorResponse("Result not found");
                }

                var examQuestions = await _context.ExamQuestions
                    .Where(eq => eq.ExamId == attempt.ExamId)
                    .ToListAsync();

                var answers = attempt.StudentAnswers.Select(sa =>
                {
                    var correctOption = sa.Question.QuestionOptions.FirstOrDefault(o => o.IsCorrect);
                    var examQuestion = examQuestions.FirstOrDefault(eq => eq.QuestionId == sa.QuestionId);

                    return new StudentAnswerDetailDto
                    {
                        QuestionId = sa.QuestionId,
                        QuestionText = sa.Question.QuestionText,
                        SelectedOptionId = sa.SelectedOptionId,
                        SelectedOptionText = sa.SelectedOption?.OptionText,
                        CorrectOptionId = correctOption?.OptionId,
                        CorrectOptionText = correctOption?.OptionText,
                        IsCorrect = sa.IsCorrect,
                        MarksObtained = sa.MarksObtained ?? 0,
                        TotalMarks = examQuestion?.Marks ?? 0
                    };
                }).ToList();

                var result = new ExamResultResponseDto
                {
                    AttemptId = attempt.AttemptId,
                    ExamId = attempt.ExamId,
                    ExamTitle = attempt.Exam.Title,
                    StudentId = attempt.StudentId,
                    StudentName = attempt.Student.FullName,
                    StartTime = attempt.StartTime,
                    SubmitTime = attempt.SubmitTime,
                    TotalScore = attempt.TotalScore ?? 0,
                    TotalMarks = attempt.Exam.TotalMarks,
                    Percentage = attempt.TotalScore.HasValue
                        ? Math.Round((attempt.TotalScore.Value / attempt.Exam.TotalMarks) * 100, 2)
                        : null,
                    Status = attempt.Status,
                    TotalQuestions = answers.Count,
                    CorrectAnswers = answers.Count(a => a.IsCorrect == true),
                    WrongAnswers = answers.Count(a => a.IsCorrect == false),
                    Answers = answers
                };

                return ApiResponse<ExamResultResponseDto>.SuccessResponse(result, "Result retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting student exam result: {ex.Message}");
                return ApiResponse<ExamResultResponseDto>.ErrorResponse("Failed to retrieve result");
            }
        }

        public async Task<bool> CanAccessExamAsync(int examId, int userId, string role)
        {
            try
            {
                if (role == UserRoles.Admin)
                {
                    return true;
                }

                var exam = await _context.Exams.FindAsync(examId);
                if (exam == null) return false;

                if (role == UserRoles.Teacher)
                {
                    return exam.CreatedBy == userId;
                }

                if (role == UserRoles.Student)
                {
                    return await _context.ClassMembers
                        .AnyAsync(cm => cm.ClassId == exam.ClassId && cm.UserId == userId && cm.Role == UserRoles.Student);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CanTakeExamAsync(int examId, int studentId)
        {
            try
            {
                var exam = await _context.Exams.FindAsync(examId);
                if (exam == null) return false;

                return await _context.ClassMembers
                    .AnyAsync(cm => cm.ClassId == exam.ClassId && cm.UserId == studentId && cm.Role == UserRoles.Student);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasActiveAttemptAsync(int examId, int studentId)
        {
            // Changed: Check if student has ANY attempt (not just active ones)
            // One attempt per student rule
            return await _context.StudentExamAttempts
                .AnyAsync(a => a.ExamId == examId && a.StudentId == studentId);
        }
    }
}
