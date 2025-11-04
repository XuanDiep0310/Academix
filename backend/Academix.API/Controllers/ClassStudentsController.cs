using Academix.Application.DTOs.Student;
using Academix.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academix.API.Controllers
{
    [ApiController]
    [Route("api/classes/{classId}/students")]
    public class ClassStudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public ClassStudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET /api/classes/{classId}/students?q=
        [HttpGet]
        public async Task<IActionResult> GetStudents(int classId, [FromQuery] string? q)
        {
            var students = await _studentService.GetStudentsAsync(classId, q);
            return Ok(new
            {
                success = true,
                message = "Lấy danh sách sinh viên trong lớp thành công.",
                data = students
            });
        }

        // POST /api/classes/{classId}/students
        [HttpPost]
        public async Task<IActionResult> AddStudentToClass(int classId, [FromBody] CreateStudentRequest request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

            request.ClassId = classId;

            try
            {
                var student = await _studentService.AddStudentToClassAsync(request);
                return StatusCode(201, new
                {
                    success = true,
                    message = "Thêm sinh viên vào lớp thành công.",
                    data = student
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new { code = "INTERNAL_SERVER_ERROR", message = ex.Message }
                });
            }
        }

        // ✅ POST /api/classes/{classId}/students/bulk
        // Thêm nhiều sinh viên cùng lúc
        /*
          Body:
          {
              "students": [
                  { "name": "Nguyen Van A", "email": "a@example.com" },
                  { "name": "Tran Thi B", "email": "b@example.com" },
                  { "name": "Le Van C", "email": "c@example.com" }
              ]
          }
        */
        [HttpPost("bulk")]
        public async Task<IActionResult> AddStudentsToClass(int classId, [FromBody] CreateStudentsRequest request)
        {
            if (request == null || request.Students == null || !request.Students.Any())
                return BadRequest(new { success = false, message = "Danh sách học sinh không hợp lệ." });

            request.ClassId = classId;

            try
            {
                var addedStudents = await _studentService.AddStudentsToClassAsync(request);
                return StatusCode(201, new
                {
                    success = true,
                    message = $"Đã thêm {addedStudents.Count()} học sinh vào lớp thành công.",
                    data = addedStudents
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = new { code = "INTERNAL_SERVER_ERROR", message = ex.Message }
                });
            }
        }

        // DELETE /api/classes/{classId}/students/{studentId}
        [HttpDelete("{studentId}")]
        public async Task<IActionResult> RemoveStudentFromClass(int classId, int studentId)
        {
            var success = await _studentService.RemoveStudentFromClassAsync(classId, studentId);

            if (!success)
                return NotFound(new { success = false, message = "Không tìm thấy sinh viên trong lớp này." });

            return Ok(new { success = true, message = "Đã gỡ sinh viên khỏi lớp thành công." });
        }

        // GET /api/classes/{classId}/students/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportStudents(int classId)
        {
            var fileBytes = await _studentService.ExportStudentsAsync(classId);
            var fileName = $"students_class_{classId}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
            return File(fileBytes, "text/csv", fileName);
        }
    }
}
