using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Exams;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.API.Controllers
{
    [ApiController]
    [Route("api/student/exams")]
    [Authorize(Roles = UserRoles.Student)]
    public class StudentExamsController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILogger<StudentExamsController> _logger;

        public StudentExamsController(IExamService examService, ILogger<StudentExamsController> logger)
        {
            _examService = examService;
            _logger = logger;
        }

        /// <summary>
        /// Get my exams (Students)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ExamResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyExams([FromQuery] int? classId = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var studentId))
            {
                return Unauthorized(ApiResponse<List<ExamResponseDto>>.ErrorResponse("Invalid user"));
            }

            var result = await _examService.GetMyExamsAsync(studentId, classId);
            return Ok(result);
        }

        /// <summary>
        /// Start exam (Students)
        /// </summary>
        [HttpPost("{examId}/start")]
        [ProducesResponseType(typeof(ApiResponse<StartExamResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<StartExamResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartExam(int examId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var studentId))
            {
                return Unauthorized(ApiResponse<StartExamResponseDto>.ErrorResponse("Invalid user"));
            }

            var result = await _examService.StartExamAsync(examId, studentId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Submit answer during exam (Students)
        /// </summary>
        [HttpPost("attempts/{attemptId}/answer")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitAnswer(int attemptId, [FromBody] SubmitAnswerRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var result = await _examService.SubmitAnswerAsync(attemptId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Submit exam (Students)
        /// </summary>
        [HttpPost("attempts/{attemptId}/submit")]
        [ProducesResponseType(typeof(ApiResponse<ExamResultResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExamResultResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitExam(int attemptId, [FromBody] SubmitExamRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ExamResultResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var result = await _examService.SubmitExamAsync(attemptId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get my exam result (Students)
        /// </summary>
        [HttpGet("attempts/{attemptId}/result")]
        [ProducesResponseType(typeof(ApiResponse<ExamResultResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExamResultResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyExamResult(int attemptId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var studentId))
            {
                return Unauthorized(ApiResponse<ExamResultResponseDto>.ErrorResponse("Invalid user"));
            }

            var result = await _examService.GetMyExamResultAsync(attemptId, studentId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get my exam history - List of all exams I've taken (Students)
        /// </summary>
        [HttpGet("history")]
        [ProducesResponseType(typeof(ApiResponse<List<ExamResultResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyExamHistory([FromQuery] int? classId = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var studentId))
            {
                return Unauthorized(ApiResponse<List<ExamResultResponseDto>>.ErrorResponse("Invalid user"));
            }

            var result = await _examService.GetMyExamHistoryAsync(studentId, classId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
