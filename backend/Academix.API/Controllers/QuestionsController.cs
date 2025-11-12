using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Questions;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{UserRoles.Teacher}, {UserRoles.Admin}")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(IQuestionService questionService, ILogger<QuestionsController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all questions for current teacher (Teachers only)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<QuestionListResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuestions(
            [FromQuery] string? subject = null,
            [FromQuery] string? difficulty = null,
            [FromQuery] string? type = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "desc")
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized(ApiResponse<QuestionListResponseDto>.ErrorResponse("Invalid user"));
            }

            var result = await _questionService.GetQuestionsAsync(
                teacherId, subject, difficulty, type, search, page, pageSize, sortBy, sortOrder);

            return Ok(result);
        }

        /// <summary>
        /// Get question by ID (Teachers only)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized(ApiResponse<QuestionResponseDto>.ErrorResponse("Invalid user"));
            }

            // Check access
            var canAccess = await _questionService.CanAccessQuestionAsync(id, teacherId);
            if (!canAccess)
            {
                return Forbid();
            }

            var result = await _questionService.GetQuestionByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new question (Teachers only)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<QuestionResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized(ApiResponse<QuestionResponseDto>.ErrorResponse("Invalid user"));
            }

            var result = await _questionService.CreateQuestionAsync(request, teacherId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetQuestionById), new { id = result.Data!.QuestionId }, result);
        }

        /// <summary>
        /// Create multiple questions at once (Teachers only)
        /// </summary>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(ApiResponse<BulkCreateQuestionsResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<BulkCreateQuestionsResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateQuestionsBulk([FromBody] BulkCreateQuestionsRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<BulkCreateQuestionsResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized(ApiResponse<BulkCreateQuestionsResponseDto>.ErrorResponse("Invalid user"));
            }

            var result = await _questionService.CreateQuestionsAsync(request, teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Update question (Teachers only)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<QuestionResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized(ApiResponse<QuestionResponseDto>.ErrorResponse("Invalid user"));
            }

            // Check access
            var canAccess = await _questionService.CanAccessQuestionAsync(id, teacherId);
            if (!canAccess)
            {
                return Forbid();
            }

            var result = await _questionService.UpdateQuestionAsync(id, request);

            if (!result.Success)
            {
                return result.Message == "Question not found" ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete question (Teachers only)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Invalid user"));
            }

            // Check access
            var canAccess = await _questionService.CanAccessQuestionAsync(id, teacherId);
            if (!canAccess)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<string>.ErrorResponse("Question not found or Teacher can not access this question"));
            }


            var result = await _questionService.DeleteQuestionAsync(id);

            if (!result.Success)
            {
                return result.Message == "Question not found" ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get question statistics for current teacher (Teachers only)
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ApiResponse<QuestionStatisticsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyStatistics()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized(ApiResponse<QuestionStatisticsDto>.ErrorResponse("Invalid user"));
            }

            var result = await _questionService.GetQuestionStatisticsAsync(teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Get global question statistics (Admin only)
        /// </summary>
        [HttpGet("statistics/global")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<QuestionStatisticsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGlobalStatistics()
        {
            var result = await _questionService.GetQuestionStatisticsAsync();
            return Ok(result);
        }
    }
}
