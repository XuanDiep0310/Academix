using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Quiz
{
    public class QuizSummaryDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int? DurationMinutes { get; set; }
        public string? DurationFormatted { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusBadge { get; set; } = string.Empty;
        public decimal? BestScore { get; set; }
        public int AttemptsUsed { get; set; }
        public bool CanAttempt { get; set; }
    }

    public class QuizDashboardDto
    {
        public List<QuizSummaryDto> Ongoing { get; set; } = new();
        public List<QuizSummaryDto> Upcoming { get; set; } = new();
        public List<QuizSummaryDto> Completed { get; set; } = new();
    }

    public class QuizDetailDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int? DurationMinutes { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool AllowBackNavigation { get; set; }
        public int AttemptsUsed { get; set; }
        public bool CanAttempt { get; set; }
        public List<AttemptHistoryDto> Attempts { get; set; } = new();
    }

    public class AttemptHistoryDto
    {
        public long AttemptId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal? Score { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? TimeSpentSeconds { get; set; }
        public string? TimeSpentFormatted { get; set; }
        public int FocusLostCount { get; set; }
    }

    public class StartQuizResponse
    {
        public long AttemptId { get; set; }
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<QuizQuestionDto> Questions { get; set; } = new();
    }

    public class QuizQuestionDto
    {
        public int QuestionId { get; set; }
        public int ExamQuestionId { get; set; }
        public string Stem { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public int OrderIndex { get; set; }
        public List<QuizOptionDto> Options { get; set; } = new();
    }

    public class QuizOptionDto
    {
        public int OptionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }

    public class SaveAnswerRequest
    {
        public long AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string? AnswerText { get; set; }
    }

    public class SubmitQuizRequest
    {
        public long AttemptId { get; set; }
        public List<QuizAnswerRequest> Answers { get; set; } = new();
        public int FocusLostCount { get; set; }
    }

    public class QuizAnswerRequest
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string? AnswerText { get; set; }
    }

    public class SubmitQuizResponse
    {
        public long AttemptId { get; set; }
        public decimal TotalScore { get; set; }
        public decimal MaxScore { get; set; }
        public string ScoreFormatted { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public int TimeSpentSeconds { get; set; }
        public string TimeSpentFormatted { get; set; } = string.Empty;
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int FocusLostCount { get; set; }
        public List<string>? Warnings { get; set; }
    }

    public class QuizReviewDto
    {
        public long AttemptId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public decimal TotalScore { get; set; }
        public decimal MaxScore { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<QuizReviewQuestionDto> Questions { get; set; } = new();
    }

    public class QuizReviewQuestionDto
    {
        public int QuestionId { get; set; }
        public string Stem { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public decimal? ScoreAwarded { get; set; }
        public bool? IsCorrect { get; set; }
        public List<QuizReviewOptionDto> Options { get; set; } = new();
        public int? SelectedOptionId { get; set; }
        public string? AnswerText { get; set; }
        public string? Feedback { get; set; }
    }

    public class QuizReviewOptionDto
    {
        public int OptionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}
