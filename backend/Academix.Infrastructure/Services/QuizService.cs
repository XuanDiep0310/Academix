using Academix.Application.DTOs.Quiz;
using Academix.Application.Interfaces;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class QuizService : IQuizService
    {
        private readonly AcademixDbContext _context;
        private readonly ILogger<QuizService> _logger;

        public QuizService(AcademixDbContext context, ILogger<QuizService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<QuizDashboardDto> GetDashboardAsync(int userId, CancellationToken ct = default)
        {
            try
            {
                // Get user's enrolled classes
                var userClassIds = await _context.Enrollments
                    .Where(e => e.UserId == userId && e.IsActive)
                    .Select(e => e.ClassId)
                    .ToListAsync(ct);

                if (!userClassIds.Any())
                {
                    return new QuizDashboardDto();
                }

                var now = DateTime.UtcNow;

                // Get all exams for user's classes
                var exams = await _context.Exams
                    .Include(e => e.Class)
                    .Include(e => e.ExamQuestions)
                    .Where(e => e.ClassId.HasValue && userClassIds.Contains(e.ClassId.Value))
                    .OrderBy(e => e.StartAt)
                    .ToListAsync(ct);

                // Get user's attempts
                var userAttempts = await _context.StudentExamAttempts
                    .Where(a => a.UserId == userId)
                    .GroupBy(a => a.ExamId)
                    .Select(g => new
                    {
                        ExamId = g.Key,
                        AttemptsCount = g.Count(),
                        BestScore = g.Max(a => a.Score),
                        HasInProgress = g.Any(a => a.Status == "InProgress")
                    })
                    .ToListAsync(ct);

                var dashboard = new QuizDashboardDto();

                foreach (var exam in exams)
                {
                    var attempts = userAttempts.FirstOrDefault(a => a.ExamId == exam.ExamId);
                    var attemptsUsed = attempts?.AttemptsCount ?? 0;

                    var quizSummary = new QuizSummaryDto
                    {
                        ExamId = exam.ExamId,
                        Title = exam.Title,
                        Description = exam.Description,
                        ClassName = exam.Class?.Title ?? "",
                        TotalQuestions = exam.ExamQuestions.Count,
                        DurationMinutes = exam.DurationMinutes,
                        DurationFormatted = exam.DurationMinutes.HasValue ? $"{exam.DurationMinutes} phút" : "Không giới hạn",
                        StartAt = exam.StartAt,
                        EndAt = exam.EndAt,
                        BestScore = attempts?.BestScore,
                        AttemptsUsed = attemptsUsed,
                        CanAttempt = !(attempts?.HasInProgress ?? false) &&
                                     (!exam.EndAt.HasValue || exam.EndAt > now)
                    };

                    // Determine status
                    if (!exam.StartAt.HasValue && !exam.EndAt.HasValue)
                    {
                        quizSummary.Status = "Đang diễn ra";
                        quizSummary.StatusBadge = "Đang diễn ra";
                        dashboard.Ongoing.Add(quizSummary);
                    }
                    else if (exam.StartAt.HasValue && exam.StartAt > now)
                    {
                        quizSummary.Status = "Sắp diễn ra";
                        quizSummary.StatusBadge = "Sắp diễn ra";
                        dashboard.Upcoming.Add(quizSummary);
                    }
                    else if (exam.EndAt.HasValue && exam.EndAt < now)
                    {
                        quizSummary.Status = "Đã kết thúc";
                        quizSummary.StatusBadge = "Đã kết thúc";
                        quizSummary.CanAttempt = false;
                        dashboard.Completed.Add(quizSummary);
                    }
                    else
                    {
                        quizSummary.Status = "Đang diễn ra";
                        quizSummary.StatusBadge = "Đang diễn ra";
                        dashboard.Ongoing.Add(quizSummary);
                    }
                }

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz dashboard for user {UserId}", userId);
                throw;
            }
        }

        public async Task<QuizDetailDto?> GetQuizDetailAsync(int examId, int userId, CancellationToken ct = default)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.Class)
                    .Include(e => e.ExamQuestions)
                    .FirstOrDefaultAsync(e => e.ExamId == examId, ct);

                if (exam == null)
                    return null;

                // Check if user has access
                if (exam.ClassId.HasValue)
                {
                    var hasAccess = await _context.Enrollments
                        .AnyAsync(e => e.ClassId == exam.ClassId.Value &&
                                      e.UserId == userId &&
                                      e.IsActive, ct);

                    if (!hasAccess)
                        throw new UnauthorizedAccessException("Bạn không có quyền truy cập bài kiểm tra này");
                }

                // Get user's attempts
                var attempts = await _context.StudentExamAttempts
                    .Where(a => a.ExamId == examId && a.UserId == userId)
                    .OrderByDescending(a => a.StartedAt)
                    .Select(a => new AttemptHistoryDto
                    {
                        AttemptId = a.AttemptId,
                        StartedAt = a.StartedAt,
                        SubmittedAt = a.SubmittedAt,
                        Score = a.Score,
                        Status = a.Status,
                        TimeSpentSeconds = a.SubmittedAt.HasValue
                            ? (int)(a.SubmittedAt.Value - a.StartedAt).TotalSeconds
                            : null,
                        TimeSpentFormatted = a.SubmittedAt.HasValue
                            ? FormatDuration((int)(a.SubmittedAt.Value - a.StartedAt).TotalSeconds)
                            : null,
                        FocusLostCount = a.FocusLostCount
                    })
                    .ToListAsync(ct);

                var hasInProgress = attempts.Any(a => a.Status == "InProgress");
                var now = DateTime.UtcNow;

                return new QuizDetailDto
                {
                    ExamId = exam.ExamId,
                    Title = exam.Title,
                    Description = exam.Description,
                    ClassName = exam.Class?.Title ?? "",
                    TotalQuestions = exam.ExamQuestions.Count,
                    DurationMinutes = exam.DurationMinutes,
                    StartAt = exam.StartAt,
                    EndAt = exam.EndAt,
                    AllowBackNavigation = exam.AllowBackNavigation,
                    AttemptsUsed = attempts.Count,
                    CanAttempt = !hasInProgress && (!exam.EndAt.HasValue || exam.EndAt > now),
                    Attempts = attempts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz detail {ExamId} for user {UserId}", examId, userId);
                throw;
            }
        }

        public async Task<StartQuizResponse> StartQuizAsync(
            int examId,
            int userId,
            string? ipAddress = null,
            string? browserInfo = null,
            CancellationToken ct = default)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.ExamQuestions)
                        .ThenInclude(eq => eq.Question)
                            .ThenInclude(q => q.QuestionOptions)
                    .Include(e => e.ExamQuestions)
                        .ThenInclude(eq => eq.Question)
                            .ThenInclude(q => q.Type)
                    .FirstOrDefaultAsync(e => e.ExamId == examId, ct);

                if (exam == null)
                    throw new InvalidOperationException("Không tìm thấy bài kiểm tra");

                // Validate access
                if (exam.ClassId.HasValue)
                {
                    var hasAccess = await _context.Enrollments
                        .AnyAsync(e => e.ClassId == exam.ClassId.Value &&
                                      e.UserId == userId &&
                                      e.IsActive, ct);

                    if (!hasAccess)
                        throw new UnauthorizedAccessException("Bạn không có quyền truy cập bài kiểm tra này");
                }

                // Check time window
                var now = DateTime.UtcNow;
                if (exam.StartAt.HasValue && exam.StartAt > now)
                    throw new InvalidOperationException("Bài kiểm tra chưa bắt đầu");

                if (exam.EndAt.HasValue && exam.EndAt < now)
                    throw new InvalidOperationException("Bài kiểm tra đã kết thúc");

                // Check for existing in-progress attempt
                var existingAttempt = await _context.StudentExamAttempts
                    .FirstOrDefaultAsync(a => a.ExamId == examId &&
                                            a.UserId == userId &&
                                            a.Status == "InProgress", ct);

                if (existingAttempt != null)
                    throw new InvalidOperationException("Bạn đang có một bài làm chưa hoàn thành");

                // Create new attempt
                var attempt = new StudentExamAttempt
                {
                    ExamId = examId,
                    UserId = userId,
                    ClassId = exam.ClassId,
                    StartedAt = now,
                    Status = "InProgress",
                    Ipaddress = ipAddress,
                    BrowserInfo = browserInfo
                };

                _context.StudentExamAttempts.Add(attempt);
                await _context.SaveChangesAsync(ct);

                // Prepare questions
                var questions = exam.ExamQuestions
                    .OrderBy(eq => eq.OrderIndex)
                    .Select(eq => new QuizQuestionDto
                    {
                        QuestionId = eq.QuestionId,
                        ExamQuestionId = eq.ExamQuestionId,
                        Stem = eq.Question.Stem,
                        QuestionType = eq.Question.Type.Name,
                        Score = eq.Score,
                        OrderIndex = eq.OrderIndex,
                        Options = eq.RandomizeOptions
                            ? eq.Question.QuestionOptions.OrderBy(o => Guid.NewGuid()).Select(o => new QuizOptionDto
                            {
                                OptionId = o.OptionId,
                                Text = o.Text,
                                OrderIndex = o.OrderIndex
                            }).ToList()
                            : eq.Question.QuestionOptions.OrderBy(o => o.OrderIndex).Select(o => new QuizOptionDto
                            {
                                OptionId = o.OptionId,
                                Text = o.Text,
                                OrderIndex = o.OrderIndex
                            }).ToList()
                    })
                    .ToList();

                // Shuffle if needed
                if (exam.ShuffleQuestions)
                {
                    questions = questions.OrderBy(q => Guid.NewGuid()).ToList();
                    // Re-index after shuffle
                    for (int i = 0; i < questions.Count; i++)
                    {
                        questions[i].OrderIndex = i;
                    }
                }

                _logger.LogInformation("User {UserId} started exam {ExamId}, attempt {AttemptId}",
                    userId, examId, attempt.AttemptId);

                return new StartQuizResponse
                {
                    AttemptId = attempt.AttemptId,
                    ExamId = examId,
                    Title = exam.Title,
                    DurationMinutes = exam.DurationMinutes,
                    StartedAt = attempt.StartedAt,
                    ExpiresAt = exam.DurationMinutes.HasValue
                        ? attempt.StartedAt.AddMinutes(exam.DurationMinutes.Value)
                        : exam.EndAt,
                    Questions = questions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting quiz {ExamId} for user {UserId}", examId, userId);
                throw;
            }
        }

        public async Task<bool> SaveAnswerAsync(SaveAnswerRequest request, int userId, CancellationToken ct = default)
        {
            try
            {
                // Verify attempt belongs to user
                var attempt = await _context.StudentExamAttempts
                    .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId &&
                                            a.UserId == userId, ct);

                if (attempt == null)
                    throw new UnauthorizedAccessException("Invalid attempt");

                if (attempt.Status != "InProgress")
                    throw new InvalidOperationException("Attempt is not in progress");

                // Check if answer already exists
                var existingAnswer = await _context.StudentAnswers
                    .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId &&
                                            a.QuestionId == request.QuestionId, ct);

                if (existingAnswer != null)
                {
                    // Update existing answer
                    existingAnswer.SelectedOptionId = request.SelectedOptionId;
                    existingAnswer.AnswerText = request.AnswerText;
                }
                else
                {
                    // Create new answer
                    var answer = new StudentAnswer
                    {
                        AttemptId = request.AttemptId,
                        QuestionId = request.QuestionId,
                        SelectedOptionId = request.SelectedOptionId,
                        AnswerText = request.AnswerText
                    };

                    _context.StudentAnswers.Add(answer);
                }

                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving answer for attempt {AttemptId}", request.AttemptId);
                throw;
            }
        }

        public async Task<SubmitQuizResponse> SubmitQuizAsync(
            SubmitQuizRequest request,
            int userId,
            CancellationToken ct = default)
        {
            try
            {
                // Verify attempt
                var attempt = await _context.StudentExamAttempts
                    .Include(a => a.Exam)
                        .ThenInclude(e => e.ExamQuestions)
                            .ThenInclude(eq => eq.Question)
                                .ThenInclude(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId &&
                                            a.UserId == userId, ct);

                if (attempt == null)
                    throw new UnauthorizedAccessException("Invalid attempt");

                if (attempt.Status != "InProgress")
                    throw new InvalidOperationException("Attempt already submitted");

                // Save all answers
                foreach (var answerRequest in request.Answers)
                {
                    var existingAnswer = await _context.StudentAnswers
                        .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId &&
                                                a.QuestionId == answerRequest.QuestionId, ct);

                    if (existingAnswer != null)
                    {
                        existingAnswer.SelectedOptionId = answerRequest.SelectedOptionId;
                        existingAnswer.AnswerText = answerRequest.AnswerText;
                    }
                    else
                    {
                        _context.StudentAnswers.Add(new StudentAnswer
                        {
                            AttemptId = request.AttemptId,
                            QuestionId = answerRequest.QuestionId,
                            SelectedOptionId = answerRequest.SelectedOptionId,
                            AnswerText = answerRequest.AnswerText
                        });
                    }
                }

                // Grade the attempt
                var (totalScore, correctCount) = await GradeAttemptAsync(attempt, ct);

                // Update attempt
                attempt.SubmittedAt = DateTime.UtcNow;
                attempt.Status = "Submitted";
                attempt.Score = totalScore;
                attempt.FocusLostCount = request.FocusLostCount;

                await _context.SaveChangesAsync(ct);

                var timeSpent = (int)(attempt.SubmittedAt.Value - attempt.StartedAt).TotalSeconds;
                var maxScore = attempt.Exam.ExamQuestions.Sum(eq => eq.Score);

                // Generate warnings
                var warnings = new List<string>();
                if (request.FocusLostCount > 5)
                    warnings.Add($"Bạn đã mất focus {request.FocusLostCount} lần. Hành vi này đã được ghi nhận.");

                _logger.LogInformation("User {UserId} submitted exam attempt {AttemptId}, score: {Score}/{MaxScore}",
                    userId, attempt.AttemptId, totalScore, maxScore);

                return new SubmitQuizResponse
                {
                    AttemptId = attempt.AttemptId,
                    TotalScore = totalScore,
                    MaxScore = maxScore,
                    ScoreFormatted = $"{totalScore:F1}/{maxScore:F1}",
                    Percentage = maxScore > 0 ? (totalScore / maxScore * 100) : 0,
                    TimeSpentSeconds = timeSpent,
                    TimeSpentFormatted = FormatDuration(timeSpent),
                    CorrectAnswers = correctCount,
                    TotalQuestions = attempt.Exam.ExamQuestions.Count,
                    FocusLostCount = request.FocusLostCount,
                    Warnings = warnings.Any() ? warnings : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting quiz for attempt {AttemptId}", request.AttemptId);
                throw;
            }
        }

        public async Task<QuizReviewDto?> GetReviewAsync(long attemptId, int userId, CancellationToken ct = default)
        {
            try
            {
                var attempt = await _context.StudentExamAttempts
                    .Include(a => a.Exam)
                    .Include(a => a.StudentAnswers)
                        .ThenInclude(ans => ans.Question)
                            .ThenInclude(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(a => a.AttemptId == attemptId && a.UserId == userId, ct);

                if (attempt == null)
                    return null;

                if (attempt.Status == "InProgress")
                    throw new InvalidOperationException("Bài kiểm tra chưa được nộp");

                // Get exam questions with scores
                var examQuestions = await _context.ExamQuestions
                    .Where(eq => eq.ExamId == attempt.ExamId)
                    .Include(eq => eq.Question)
                        .ThenInclude(q => q.QuestionOptions)
                    .OrderBy(eq => eq.OrderIndex)
                    .ToListAsync(ct);

                var reviewQuestions = new List<QuizReviewQuestionDto>();

                foreach (var examQuestion in examQuestions)
                {
                    var studentAnswer = attempt.StudentAnswers
                        .FirstOrDefault(a => a.QuestionId == examQuestion.QuestionId);

                    var reviewQuestion = new QuizReviewQuestionDto
                    {
                        QuestionId = examQuestion.QuestionId,
                        Stem = examQuestion.Question.Stem,
                        Score = examQuestion.Score,
                        ScoreAwarded = studentAnswer?.ScoreAwarded,
                        IsCorrect = studentAnswer?.ScoreAwarded > 0,
                        SelectedOptionId = studentAnswer?.SelectedOptionId,
                        AnswerText = studentAnswer?.AnswerText,
                        Feedback = studentAnswer?.Feedback,
                        Options = examQuestion.Question.QuestionOptions.Select(o => new QuizReviewOptionDto
                        {
                            OptionId = o.OptionId,
                            Text = o.Text,
                            IsCorrect = o.IsCorrect,
                            IsSelected = o.OptionId == studentAnswer?.SelectedOptionId
                        }).ToList()
                    };

                    reviewQuestions.Add(reviewQuestion);
                }

                return new QuizReviewDto
                {
                    AttemptId = attempt.AttemptId,
                    QuizTitle = attempt.Exam.Title,
                    TotalScore = attempt.Score ?? 0,
                    MaxScore = examQuestions.Sum(eq => eq.Score),
                    SubmittedAt = attempt.SubmittedAt ?? attempt.StartedAt,
                    Questions = reviewQuestions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review for attempt {AttemptId}", attemptId);
                throw;
            }
        }

        public async Task TrackFocusLostAsync(long attemptId, int userId, CancellationToken ct = default)
        {
            try
            {
                var attempt = await _context.StudentExamAttempts
                    .FirstOrDefaultAsync(a => a.AttemptId == attemptId &&
                                            a.UserId == userId, ct);

                if (attempt == null || attempt.Status != "InProgress")
                    return;

                attempt.FocusLostCount++;

                var focusLog = new FocusLog
                {
                    AttemptId = attemptId,
                    UserId = userId,
                    OccurredAt = DateTime.UtcNow
                };

                _context.FocusLogs.Add(focusLog);
                await _context.SaveChangesAsync(ct);

                _logger.LogWarning("User {UserId} lost focus on attempt {AttemptId}, count: {Count}",
                    userId, attemptId, attempt.FocusLostCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking focus lost for attempt {AttemptId}", attemptId);
            }
        }

        // ===== PRIVATE HELPER METHODS =====

        private async Task<(decimal totalScore, int correctCount)> GradeAttemptAsync(
            StudentExamAttempt attempt,
            CancellationToken ct)
        {
            decimal totalScore = 0;
            int correctCount = 0;

            var answers = await _context.StudentAnswers
                .Where(a => a.AttemptId == attempt.AttemptId)
                .ToListAsync(ct);

            foreach (var answer in answers)
            {
                var examQuestion = attempt.Exam.ExamQuestions
                    .FirstOrDefault(eq => eq.QuestionId == answer.QuestionId);

                if (examQuestion == null)
                    continue;

                var question = examQuestion.Question;

                // Auto-grade multiple choice
                if (question.Type.Name == "MultipleChoice" && answer.SelectedOptionId.HasValue)
                {
                    var selectedOption = question.QuestionOptions
                        .FirstOrDefault(o => o.OptionId == answer.SelectedOptionId);

                    if (selectedOption?.IsCorrect == true)
                    {
                        answer.ScoreAwarded = examQuestion.Score;
                        answer.AutoGraded = true;
                        totalScore += examQuestion.Score;
                        correctCount++;
                    }
                    else
                    {
                        answer.ScoreAwarded = 0;
                        answer.AutoGraded = true;
                    }
                }
                // Auto-grade true/false
                else if (question.Type.Name == "TrueFalse" && answer.SelectedOptionId.HasValue)
                {
                    var selectedOption = question.QuestionOptions
                        .FirstOrDefault(o => o.OptionId == answer.SelectedOptionId);

                    if (selectedOption?.IsCorrect == true)
                    {
                        answer.ScoreAwarded = examQuestion.Score;
                        answer.AutoGraded = true;
                        totalScore += examQuestion.Score;
                        correctCount++;
                    }
                    else
                    {
                        answer.ScoreAwarded = 0;
                        answer.AutoGraded = true;
                    }
                }
                // Essay/Short answer - requires manual grading
                else
                {
                    answer.ScoreAwarded = null;
                    answer.AutoGraded = false;
                }
            }

            return (totalScore, correctCount);
        }

        private static string FormatDuration(int seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}:{ts.Minutes:00}:{ts.Seconds:00}";
            return $"{(int)ts.TotalMinutes}:{ts.Seconds:00}";
        }
    }
}
