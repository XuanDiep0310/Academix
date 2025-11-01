using Academix.Application.DTOs.Class;
using Academix.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassDto>>> GetAllClasses()
        {
            try
            {
                var classes = await _classService.GetAllClassesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Classes retrieved successfully.",
                    data = classes
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ClassDto>> GetClassById(int id)
        {
            var classItem = await _classService.GetClassByIdAsync(id);
            if (classItem == null)
            {
                return NotFound(new
                {
                    success = false,
                    error = new { code = "CLASS_NOT_FOUND", message = "Class not found." }
                });
            }

            return Ok(new
            {
                success = true,
                message = "Class retrieved successfully.",
                data = classItem
            });
        }

        [HttpPost]
        public async Task<ActionResult<ClassDto>> CreateClass([FromBody] CreateClassRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new { code = "VALIDATION_ERROR", message = "Invalid input data." },
                    details = ModelState.Select(kvp => new
                    {
                        field = kvp.Key,
                        message = string.Join("; ", kvp.Value.Errors.Select(e => e.ErrorMessage))
                    })
                });
            }

            try
            {
                var created = await _classService.CreateClassAsync(request);
                return CreatedAtAction(nameof(GetClassById), new { id = created.ClassId }, new
                {
                    success = true,
                    message = "Class created successfully.",
                    data = created
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new { code = "CREATE_FAILED", message = ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateClassRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new { code = "VALIDATION_ERROR", message = "Invalid input data." },
                    details = ModelState.Select(kvp => new
                    {
                        field = kvp.Key,
                        message = string.Join("; ", kvp.Value.Errors.Select(e => e.ErrorMessage))
                    })
                });
            }

            var updated = await _classService.UpdateClassAsync(id, request);
            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    error = new { code = "CLASS_NOT_FOUND", message = "Class not found." }
                });
            }

            return Ok(new
            {
                success = true,
                message = "Class updated successfully.",
                data = (object?)null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var deleted = await _classService.DeleteClassAsync(id);
            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    error = new { code = "CLASS_NOT_FOUND", message = "Class not found." }
                });
            }

            return Ok(new
            {
                success = true,
                message = "Class deleted successfully.",
                data = (object?)null
            });
        }

        [HttpGet("my-classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            // ✅ Lấy userId từ JWT (Claim)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized(new { message = "User not authenticated" });

            int userId = int.Parse(userIdClaim);

            var classes = await _classService.GetMyClassesAsync(userId);
            return Ok(new
            {
                success = true,
                message = "Lấy danh sách lớp thành công",
                data = classes
            });
        }
    }
}
