using Academix.Application.DTOs.Classes;
using Academix.Application.DTOs.Common;
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
    [Authorize]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly ILogger<ClassesController> _logger;

        public ClassesController(IClassService classService, ILogger<ClassesController> logger)
        {
            _classService = classService;
            _logger = logger;
        }

        /// <summary>
        /// Get all classes with filtering, pagination and sorting (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<ClassListResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllClasses(
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "desc")
        {
            var result = await _classService.GetAllClassesAsync(search, isActive, page, pageSize, sortBy, sortOrder);
            return Ok(result);
        }

        /// <summary>
        /// Get class by ID (Admin and Teachers)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
        [ProducesResponseType(typeof(ApiResponse<ClassDetailResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ClassDetailResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClassById(int id)
        {
            var result = await _classService.GetClassByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new class (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<ClassResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ClassResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ClassResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<ClassResponseDto>.ErrorResponse("Invalid user"));
            }

            var result = await _classService.CreateClassAsync(request, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetClassById), new { id = result.Data!.ClassId }, result);
        }

        /// <summary>
        /// Update class information (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<ClassResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ClassResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ClassResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateClassRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ClassResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var result = await _classService.UpdateClassAsync(id, request);

            if (!result.Success)
            {
                return result.Message == "Class not found" ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete class (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var result = await _classService.DeleteClassAsync(id);

            if (!result.Success)
            {
                return result.Message == "Class not found" ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get my classes (Teacher and Student)
        /// </summary>
        [HttpGet("my-classes")]
        [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Student}")]
        [ProducesResponseType(typeof(ApiResponse<List<MyClassResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyClasses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<List<MyClassResponseDto>>.ErrorResponse("Invalid user"));
            }

            var result = await _classService.GetMyClassesAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Add teachers to class (Admin only)
        /// </summary>
        [HttpPost("{classId}/members/teachers")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<AddMembersResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AddMembersResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddTeachersToClass(int classId, [FromBody] AddMembersRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AddMembersResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var result = await _classService.AddTeachersToClassAsync(classId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Add students to class (Admin only)
        /// </summary>
        [HttpPost("{classId}/members/students")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<AddMembersResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AddMembersResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddStudentsToClass(int classId, [FromBody] AddMembersRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AddMembersResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var result = await _classService.AddStudentsToClassAsync(classId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Remove member from class (Admin only)
        /// </summary>
        [HttpDelete("{classId}/members/{userId}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMemberFromClass(int classId, int userId)
        {
            var result = await _classService.RemoveMemberFromClassAsync(classId, userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get class members (Admin and Teachers)
        /// </summary>
        [HttpGet("{classId}/members")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}, {UserRoles.Student}")]
        [ProducesResponseType(typeof(ApiResponse<List<ClassMemberDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClassMembers(int classId, [FromQuery] string? role = null)
        {
            var result = await _classService.GetClassMembersAsync(classId, role);
            return Ok(result);
        }

        /// <summary>
        /// Get class students (Teachers can see their class students)
        /// </summary>
        [HttpGet("{classId}/students")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
        [ProducesResponseType(typeof(ApiResponse<List<ClassMemberDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClassStudents(int classId)
        {
            var result = await _classService.GetClassStudentsAsync(classId);
            return Ok(result);
        }

        /// <summary>
        /// Get class teachers (Admin only)
        /// </summary>
        [HttpGet("{classId}/teachers")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<List<ClassMemberDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClassTeachers(int classId)
        {
            var result = await _classService.GetClassTeachersAsync(classId);
            return Ok(result);
        }

        /// <summary>
        /// Get class statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<ClassStatisticsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _classService.GetClassStatisticsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Check if class code exists (Admin only)
        /// </summary>
        [HttpGet("check-code")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckClassCodeExists([FromQuery] string classCode, [FromQuery] int? excludeClassId = null)
        {
            if (string.IsNullOrEmpty(classCode))
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Class code is required"));
            }

            var exists = await _classService.IsClassCodeExistsAsync(classCode, excludeClassId);
            return Ok(ApiResponse<bool>.SuccessResponse(exists, exists ? "Class code exists" : "Class code is available"));
        }
    }
}
