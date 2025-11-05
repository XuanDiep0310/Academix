using Academix.Application.DTOs.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IQuizService
    {
        Task<QuizDashboardDto> GetDashboardAsync(int userId, CancellationToken ct = default);

        Task<QuizDetailDto?> GetQuizDetailAsync(int examId, int userId, CancellationToken ct = default);

        Task<StartQuizResponse> StartQuizAsync(
            int examId,
            int userId,
            string? ipAddress = null,
            string? browserInfo = null,
            CancellationToken ct = default);

        Task<bool> SaveAnswerAsync(SaveAnswerRequest request, int userId, CancellationToken ct = default);

        Task<SubmitQuizResponse> SubmitQuizAsync(
            SubmitQuizRequest request,
            int userId,
            CancellationToken ct = default);

        Task<QuizReviewDto?> GetReviewAsync(long attemptId, int userId, CancellationToken ct = default);

        Task TrackFocusLostAsync(long attemptId, int userId, CancellationToken ct = default);
    }
}
