using Academix.API.Attributes;
using Academix.Application.DTOs.Class;
using Academix.Application.DTOs.Course;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Academix.Domain.Entities;
using Academix.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [RequirePermission(Permissions.CourseView)]
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Courses retrieved successfully.",
                    data = courses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new { code = "INTERNAL_ERROR", message = ex.Message }
                });
            }
        }

        [RequirePermission(Permissions.CourseView)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { code = "NOT_FOUND", message = "Course not found." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Course retrieved successfully.",
                    data = course
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new { code = "INTERNAL_ERROR", message = ex.Message }
                });
            }
        }

        [RequirePermission(Permissions.CourseCreate)]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = new { code = "VALIDATION_ERROR", message = "Invalid request payload." }
                    });
                }

                var newCourse = await _courseService.CreateCoursesAsync(request);

                return StatusCode(201, new
                {
                    success = true,
                    message = "Course created successfully.",
                    data = newCourse
                });
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "DB_CONSTRAINT_ERROR",
                        message = dbEx.InnerException?.Message ?? "Database constraint violated."
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new { code = "INTERNAL_ERROR", message = ex.Message }
                });
            }
        }

        [RequirePermission(Permissions.CourseEdit)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
        {
            try
            {
                var updated = await _courseService.UpdateCourseAsync(id, request);
                if (!updated)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { code = "NOT_FOUND", message = "Course not found." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Course updated successfully."
                });
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "DB_CONSTRAINT_ERROR",
                        message = dbEx.InnerException?.Message ?? "Database constraint violated."
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new { code = "INTERNAL_ERROR", message = ex.Message }
                });
            }
        }

        [RequirePermission(Permissions.CourseDelete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var deleted = await _courseService.DeleteCourseAsync(id);
                if (!deleted)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { code = "NOT_FOUND", message = "Course not found." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Course deleted successfully."
                });
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "DB_CONSTRAINT_ERROR",
                        message = dbEx.InnerException?.Message ?? "Database constraint violated."
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new { code = "INTERNAL_ERROR", message = ex.Message }
                });
            }
        }
    }
}
