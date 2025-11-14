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
    [Route("api/classes/{classId}/exams")]
    [Authorize(Roles = UserRoles.Teacher)]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILogger<ExamsController> _logger;

        public ExamsController(IExamService examService, ILogger<ExamsController> logger)
        {
            _examService = examService;
            _logger = logger;
        }

        /// <summary>
        /// Get all exams in a class (Teachers)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ExamListResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExamsByClass(
            int classId,
            [FromQuery] bool? isPublished = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "desc")
        {
            var result = await _examService.GetExamsByClassAsync(classId, isPublished, page, pageSize, sortBy, sortOrder);
            return Ok(result);
        }

        /// <summary>
        /// Get exam by ID (Teachers)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ExamDetailResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExamDetailResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExamById(int classId, int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<ExamDetailResponseDto>.ErrorResponse("Invalid user"));
            }

            // Check access
            var canAccess = await _examService.CanAccessExamAsync(id, userId, userRole!);
            if (!canAccess)
            {
                return Forbid();
            }

            var result = await _examService.GetExamByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new exam (Teachers)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExamResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ExamResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExam(int classId, [FromBody] CreateExamRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ExamResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized(ApiResponse<ExamResponseDto>.ErrorResponse("Invalid user"));
            }

            var result = await _examService.CreateExamAsync(classId, request, teacherId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetExamById), new { classId, id = result.Data!.ExamId }, result);
        }

        /// <summary>
        /// Update exam (Teachers)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ExamResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExamResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateExam(int classId, int id, [FromBody] UpdateExamRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ExamResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<ExamResponseDto>.ErrorResponse("Invalid user"));
            }

            // Check access
            var canAccess = await _examService.CanAccessExamAsync(id, userId, userRole!);
            if (!canAccess)
            {
                return Forbid();
            }

            var result = await _examService.UpdateExamAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete exam (Teachers)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteExam(int classId, int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Invalid user"));
            }

            // Check access
            var canAccess = await _examService.CanAccessExamAsync(id, userId, userRole!);
            if (!canAccess)
            {
                return Forbid();
            }

            var result = await _examService.DeleteExamAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Publish exam (Teachers)
        /// </summary>
        [HttpPatch("{id}/publish")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PublishExam(int classId, int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Invalid user"));
            }

            // Check access
            var canAccess = await _examService.CanAccessExamAsync(id, userId, userRole!);
            if (!canAccess)
            {
                return Forbid();
            }

            var result = await _examService.PublishExamAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get exam questions (Teachers)
        /// </summary>
        [HttpGet("{id}/questions")]
        [ProducesResponseType(typeof(ApiResponse<List<ExamQuestionDetailDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExamQuestions(int classId, int id)
        {
            var result = await _examService.GetExamQuestionsAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Add questions to exam (Teachers)
        /// </summary>
        [HttpPost("{id}/questions")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddQuestionsToExam(int classId, int id, [FromBody] AddQuestionsToExamRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var result = await _examService.AddQuestionsToExamAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Remove question from exam (Teachers)
        /// </summary>
        [HttpDelete("{id}/questions/{questionId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveQuestionFromExam(int classId, int id, int questionId)
        {
            var result = await _examService.RemoveQuestionFromExamAsync(id, questionId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get exam results (Teachers)
        /// </summary>
        [HttpGet("{id}/results")]
        [ProducesResponseType(typeof(ApiResponse<ExamResultsListResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExamResults(
            int classId,
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _examService.GetExamResultsAsync(id, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get student's exam result (Teachers)
        /// </summary>
        [HttpGet("{id}/results/{attemptId}")]
        [ProducesResponseType(typeof(ApiResponse<ExamResultResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExamResultResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentExamResult(int classId, int id, int attemptId)
        {
            var result = await _examService.GetStudentExamResultAsync(attemptId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
