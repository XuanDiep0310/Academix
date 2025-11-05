using Academix.Application.DTOs.Teacher;
using Academix.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Academix.API.Controllers.QuestionController;

namespace Academix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        private int? GetTeacherId()
        {
            var claim = User.FindFirst("userId")?.Value
                       ?? User.FindFirst("UserId")?.Value
                       ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out var id))
                return id;
            return null;
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var teacherId = GetTeacherId();
                if (teacherId == null)
                    return Unauthorized(new
                    {
                        success = false,
                        error = new { code = "INVALID_AUTH", message = "Không thể xác định giáo viên từ token." }
                    });

                var dashboardData = await _teacherService.GetDashboardAsync(teacherId.Value);

                if (dashboardData == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new
                        {
                            code = "DATA_NOT_FOUND",
                            message = "No dashboard data found for this teacher."
                        }
                    });
                }
                return Ok(new
                {
                    success = true,
                    message = "Dashboard data retrieved successfully.",
                    data = dashboardData
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("Access denied. You do not have permission to view this dashboard.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new
                    {
                        code = "INTERNAL_SERVER_ERROR",
                        message = "An unexpected error occurred while loading the dashboard: " + ex.Message
                    }
                });
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("classes")]
        public async Task<IActionResult> GetTeachingClasses()
        {
            try
            {
                var teacherId = GetTeacherId();
                if (teacherId == null)
                    return Unauthorized(new
                    {
                        success = false,
                        error = new { code = "INVALID_AUTH", message = "Không thể xác định giáo viên từ token." }
                    });

                var classes = await _teacherService.GetTeachingClassesAsync(teacherId.Value);
                if (classes == null || !classes.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "The teacher is not currently assigned to any active classes.",
                        data = Array.Empty<object>()
                    });
                }
                return Ok(new
                {
                    success = true,
                    message = "Teaching classes retrieved successfully.",
                    data = classes
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    success = false,
                    error = new
                    {
                        code = "FORBIDDEN",
                        message = "You do not have permission to access this resource.",
                        detail = ex.Message
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    error = new
                    {
                        code = "INTERNAL_SERVER_ERROR",
                        message = "An unexpected error occurred while retrieving teaching classes.",
                        detail = ex.Message
                    }
                });
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("exams")]
        public async Task<IActionResult> GetAllExams()
        {
            try
            {
                var teacherId = GetTeacherId();
                if (teacherId == null)
                    return Unauthorized(new
                    {
                        success = false,
                        error = new { code = "INVALID_AUTH", message = "Không thể xác định giáo viên từ token." }
                    });

                var exams = await _teacherService.GetExamsAsync(teacherId.Value);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách bài kiểm tra thành công.",
                    data = exams
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new
                {
                    success = false,
                    error = new
                    {
                        code = "UNAUTHORIZED_ACCESS",
                        message = ex.Message
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new
                    {
                        code = "INTERNAL_SERVER_ERROR",
                        message = "Lỗi khi lấy danh sách bài kiểm tra: " + ex.Message
                    }
                });
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("exams")]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamRequest request)
        {
            try
            {
                var teacherId = GetTeacherId();
                if (teacherId == null)
                    return Unauthorized(new
                    {
                        success = false,
                        error = new { code = "INVALID_AUTH", message = "Không thể xác định giáo viên từ token." }
                    });

                var newExamId = await _teacherService.CreateExamAsync(teacherId.Value, request);

                return Ok(new
                {
                    success = true,
                    message = "Tạo bài kiểm tra thành công.",
                    data = new
                    {
                        examId = newExamId,
                        title = request.Title,
                        startAt = request.StartAt,
                        endAt = request.EndAt,
                        durationMinutes = request.DurationMinutes
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new
                {
                    success = false,
                    error = new
                    {
                        code = "UNAUTHORIZED_ACCESS",
                        message = ex.Message
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new
                    {
                        code = "INTERNAL_SERVER_ERROR",
                        message = "Lỗi khi tạo bài kiểm tra: " + ex.Message
                    }
                });
            }

        }

    }
}
