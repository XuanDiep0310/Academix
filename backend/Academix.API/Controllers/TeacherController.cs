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

        [Authorize(Roles = "Teacher")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                // Lấy teacherId từ JWT Claims
                var userIdClaim = User.FindFirst("userId")?.Value
                                  ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var teacherId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = new
                        {
                            code = "INVALID_CLAIM",
                            message = "Invalid or missing teacher identity in the token."
                        }
                    });
                }

                // Gọi service để lấy dữ liệu dashboard
                var dashboardData = await _teacherService.GetDashboardAsync(teacherId);

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

                // Trả kết quả thành công
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

    }
}
