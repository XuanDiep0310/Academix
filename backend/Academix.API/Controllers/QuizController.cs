using Academix.API.Extensions;
using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Quiz;
using Academix.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService quizService, ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        /// <summary>
        /// Get quiz dashboard with all quizzes (ongoing, upcoming, completed)
        /// </summary>
        /// <response code="200">Returns quiz dashboard</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<QuizDashboardDto>), 200)]
        public async Task<ActionResult<ApiResponse<QuizDashboardDto>>> GetDashboard()
        {
            try
            {
                var userId = User.GetUserId();
                var dashboard = await _quizService.GetDashboardAsync(userId);

                return ApiResponse<QuizDashboardDto>.SuccessResponse(
                    dashboard,
                    "Lấy danh sách bài kiểm tra thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz dashboard");
                return ApiResponse<QuizDashboardDto>.ErrorResponse(
                    "QUIZ_ERROR",
                    "Không thể lấy danh sách bài kiểm tra");
            }
        }

        /// <summary>
        /// Get quiz detail with attempt history
        /// </summary>
        /// <param name="examId">Exam ID</param>
        /// <response code="200">Returns quiz detail</response>
        /// <response code="404">Quiz not found</response>
        /// <response code="403">No access to this quiz</response>
        [HttpGet("{examId}")]
        [ProducesResponseType(typeof(ApiResponse<QuizDetailDto>), 200)]
        public async Task<ActionResult<ApiResponse<QuizDetailDto>>> GetQuizDetail(int examId)
        {
            try
            {
                var userId = User.GetUserId();
                var quiz = await _quizService.GetQuizDetailAsync(examId, userId);

                if (quiz == null)
                {
                    return ApiResponse<QuizDetailDto>.ErrorResponse(
                        "QUIZ_NOT_FOUND",
                        "Không tìm thấy bài kiểm tra");
                }

                return ApiResponse<QuizDetailDto>.SuccessResponse(quiz);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<QuizDetailDto>.ErrorResponse(
                    "QUIZ_ACCESS_DENIED",
                    ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz detail {ExamId}", examId);
                return ApiResponse<QuizDetailDto>.ErrorResponse(
                    "QUIZ_ERROR",
                    "Không thể lấy thông tin bài kiểm tra");
            }
        }

        /// <summary>
        /// Start a new quiz attempt
        /// </summary>
        /// <param name="examId">Exam ID</param>
        /// <response code="200">Quiz started successfully</response>
        /// <response code="400">Cannot start quiz (validation failed)</response>
        /// <response code="403">No access to this quiz</response>
        [HttpPost("{examId}/start")]
        [ProducesResponseType(typeof(ApiResponse<StartQuizResponse>), 200)]
        public async Task<ActionResult<ApiResponse<StartQuizResponse>>> StartQuiz(int examId)
        {
            try
            {
                var userId = User.GetUserId();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                var response = await _quizService.StartQuizAsync(
                    examId,
                    userId,
                    ipAddress,
                    userAgent);

                return ApiResponse<StartQuizResponse>.SuccessResponse(
                    response,
                    "Bắt đầu làm bài thành công");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<StartQuizResponse>.ErrorResponse(
                    "QUIZ_ACCESS_DENIED",
                    ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<StartQuizResponse>.ErrorResponse(
                    "CANNOT_START_QUIZ",
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting quiz {ExamId}", examId);
                return ApiResponse<StartQuizResponse>.ErrorResponse(
                    "START_ERROR",
                    "Không thể bắt đầu bài kiểm tra");
            }
        }

        /// <summary>
        /// Save answer for a question (auto-save)
        /// </summary>
        /// <param name="request">Answer data</param>
        /// <response code="200">Answer saved</response>
        /// <response code="400">Invalid request</response>
        [HttpPost("save-answer")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<ActionResult<ApiResponse<bool>>> SaveAnswer([FromBody] SaveAnswerRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                await _quizService.SaveAnswerAsync(request, userId);

                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<bool>.ErrorResponse(
                    "INVALID_ATTEMPT",
                    ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "SAVE_ERROR",
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving answer for attempt {AttemptId}", request.AttemptId);
                return ApiResponse<bool>.ErrorResponse(
                    "SAVE_ERROR",
                    "Không thể lưu câu trả lời");
            }
        }

        /// <summary>
        /// Submit quiz
        /// </summary>
        /// <param name="request">Submit data with all answers</param>
        /// <response code="200">Quiz submitted successfully</response>
        /// <response code="400">Cannot submit (validation failed)</response>
        [HttpPost("submit")]
        [ProducesResponseType(typeof(ApiResponse<SubmitQuizResponse>), 200)]
        public async Task<ActionResult<ApiResponse<SubmitQuizResponse>>> SubmitQuiz(
            [FromBody] SubmitQuizRequest request)
        {
            try
            {
                // Validate
                if (request.AttemptId <= 0)
                {
                    return ApiResponse<SubmitQuizResponse>.ErrorResponse(
                        "VALIDATION_ERROR",
                        "Thông tin không hợp lệ",
                        new List<ValidationError>
                        {
                        new() { Field = "attemptId", Message = "AttemptId không hợp lệ" }
                        });
                }

                var userId = User.GetUserId();
                var response = await _quizService.SubmitQuizAsync(request, userId);

                return ApiResponse<SubmitQuizResponse>.SuccessResponse(
                    response,
                    "Đã nộp bài thành công!");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<SubmitQuizResponse>.ErrorResponse(
                    "INVALID_ATTEMPT",
                    ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<SubmitQuizResponse>.ErrorResponse(
                    "SUBMIT_ERROR",
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting quiz for attempt {AttemptId}", request.AttemptId);
                return ApiResponse<SubmitQuizResponse>.ErrorResponse(
                    "SUBMIT_ERROR",
                    "Không thể nộp bài");
            }
        }

        /// <summary>
        /// Get quiz review/results with correct answers
        /// </summary>
        /// <param name="attemptId">Attempt ID</param>
        /// <response code="200">Returns quiz review</response>
        /// <response code="404">Attempt not found</response>
        /// <response code="400">Quiz not submitted yet</response>
        [HttpGet("attempt/{attemptId}/review")]
        [ProducesResponseType(typeof(ApiResponse<QuizReviewDto>), 200)]
        public async Task<ActionResult<ApiResponse<QuizReviewDto>>> GetReview(long attemptId)
        {
            try
            {
                var userId = User.GetUserId();
                var review = await _quizService.GetReviewAsync(attemptId, userId);

                if (review == null)
                {
                    return ApiResponse<QuizReviewDto>.ErrorResponse(
                        "ATTEMPT_NOT_FOUND",
                        "Không tìm thấy bài làm");
                }

                return ApiResponse<QuizReviewDto>.SuccessResponse(review);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<QuizReviewDto>.ErrorResponse(
                    "REVIEW_NOT_AVAILABLE",
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review for attempt {AttemptId}", attemptId);
                return ApiResponse<QuizReviewDto>.ErrorResponse(
                    "REVIEW_ERROR",
                    "Không thể xem kết quả");
            }
        }

        /// <summary>
        /// Track focus lost event (anti-cheat)
        /// </summary>
        /// <param name="attemptId">Attempt ID</param>
        /// <response code="200">Event tracked</response>
        [HttpPost("attempt/{attemptId}/focus-lost")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<ActionResult<ApiResponse<bool>>> TrackFocusLost(long attemptId)
        {
            try
            {
                var userId = User.GetUserId();
                await _quizService.TrackFocusLostAsync(attemptId, userId);

                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking focus lost for attempt {AttemptId}", attemptId);
                // Don't fail the request if tracking fails
                return ApiResponse<bool>.SuccessResponse(true);
            }
        }

        /// <summary>
        /// Get current attempt status (for reconnection)
        /// </summary>
        /// <param name="attemptId">Attempt ID</param>
        /// <response code="200">Returns attempt status</response>
        [HttpGet("attempt/{attemptId}/status")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<ActionResult<ApiResponse<object>>> GetAttemptStatus(long attemptId)
        {
            try
            {
                var userId = User.GetUserId();

                // This would be useful for resuming quiz after disconnect
                // TODO: Implement get attempt status with saved answers

                return ApiResponse<object>.SuccessResponse(new
                {
                    attemptId,
                    status = "InProgress",
                    timeRemaining = 3600 // seconds
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attempt status {AttemptId}", attemptId);
                return ApiResponse<object>.ErrorResponse(
                    "STATUS_ERROR",
                    "Không thể lấy trạng thái");
            }
        }
    }
}
