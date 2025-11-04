using Academix.Application.DTOs.Student;
using Academix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Academix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET /api/students?classId=&q=
        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] int? classId, [FromQuery] string? q)
        {
            try
            {
                var students = await _studentService.GetStudentsAsync(classId, q);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách học sinh thành công.",
                    data = students
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
                        message = $"Lỗi hệ thống: {ex.Message}"
                    }
                });
            }
        }

        // POST /api/students
        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] CreateStudentRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        code = "VALIDATION_ERROR",
                        message = "Dữ liệu học sinh không hợp lệ."
                    }
                });
            }

            try
            {
                var newStudent = await _studentService.AddStudentAsync(request);
                return StatusCode(201, new
                {
                    success = true,
                    message = "Thêm học sinh mới thành công.",
                    data = newStudent
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
                        message = $"Không thể thêm học sinh: {ex.Message}"
                    }
                });
            }
        }

        // DELETE /api/students/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                var result = await _studentService.DeleteStudentAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new
                        {
                            code = "NOT_FOUND",
                            message = $"Không tìm thấy học sinh với ID = {id}."
                        }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa học sinh thành công.",
                    data = new { studentId = id }
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
                        message = $"Lỗi khi xóa học sinh: {ex.Message}"
                    }
                });
            }
        }

        // GET /api/students/{id}/stats
        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetStudentStats(int id)
        {
            try
            {
                var stats = await _studentService.GetStudentStatsAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê học sinh thành công.",
                    data = stats
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
                        message = $"Lỗi khi lấy thống kê học sinh: {ex.Message}"
                    }
                });
            }
        }

        // GET /api/students/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportStudents([FromQuery] int? classId)
        {
            try
            {
                var fileBytes = await _studentService.ExportStudentsAsync(classId);
                var fileName = $"students_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new
                    {
                        code = "EXPORT_ERROR",
                        message = $"Không thể xuất danh sách học sinh: {ex.Message}"
                    }
                });
            }
        }
    }
}
