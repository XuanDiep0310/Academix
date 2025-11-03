using Academix.Application.DTOs.Question;
using Academix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Academix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        #region Error Wrapper Models
        public class ErrorDetail
        {
            public string Field { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        public class ErrorInfo
        {
            public string Code { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        public class ErrorResponse
        {
            public bool Success { get; set; } = false;
            public ErrorInfo Error { get; set; } = new();
            public List<ErrorDetail>? Details { get; set; }
        }
        #endregion

        // GET: api/Question
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsAsync();
                return Ok(new
                {
                    success = true,
                    message = "Questions retrieved successfully.",
                    data = questions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Error = new ErrorInfo
                    {
                        Code = "INTERNAL_ERROR",
                        Message = ex.Message
                    }
                });
            }
        }

        // GET: api/Question/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound(new ErrorResponse
                {
                    Error = new ErrorInfo
                    {
                        Code = "QUESTION_NOT_FOUND",
                        Message = $"Không tìm thấy câu hỏi với ID = {id}"
                    }
                });
            }

            return Ok(new
            {
                success = true,
                message = "Question retrieved successfully.",
                data = question
            });
        }

        // POST: api/Question
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request)
        {
            if (!ModelState.IsValid)
            {
                var details = ModelState.Keys
                    .SelectMany(k => ModelState[k]!.Errors.Select(e => new ErrorDetail
                    {
                        Field = k,
                        Message = e.ErrorMessage
                    }))
                    .ToList();

                return BadRequest(new ErrorResponse
                {
                    Error = new ErrorInfo
                    {
                        Code = "VALIDATION_ERROR",
                        Message = "Dữ liệu gửi lên không hợp lệ."
                    },
                    Details = details
                });
            }

            try
            {
                var created = await _questionService.CreateQuestionAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.QuestionId }, new
                {
                    success = true,
                    message = "Question created successfully.",
                    data = created
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Error = new ErrorInfo
                    {
                        Code = "CREATE_FAILED",
                        Message = ex.Message
                    }
                });
            }
        }

        // PUT: api/Question/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestionRequest request)
        {
            if (!ModelState.IsValid)
            {
                var details = ModelState.Keys
                    .SelectMany(k => ModelState[k]!.Errors.Select(e => new ErrorDetail
                    {
                        Field = k,
                        Message = e.ErrorMessage
                    }))
                    .ToList();

                return BadRequest(new ErrorResponse
                {
                    Error = new ErrorInfo
                    {
                        Code = "VALIDATION_ERROR",
                        Message = "Dữ liệu gửi lên không hợp lệ."
                    },
                    Details = details
                });
            }

            var updated = await _questionService.UpdateQuestionAsync(id, request);
            if (updated == null)
            {
                return NotFound(new ErrorResponse
                {
                    Error = new ErrorInfo
                    {
                        Code = "QUESTION_NOT_FOUND",
                        Message = $"Không tìm thấy câu hỏi với ID = {id}"
                    }
                });
            }

            return Ok(new
            {
                success = true,
                message = "Question updated successfully.",
                data = updated
            });
        }

        // DELETE: api/Question/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _questionService.DeleteQuestionAsync(id);
            if (!deleted)
            {
                return NotFound(new ErrorResponse
                {
                    Error = new ErrorInfo
                    {
                        Code = "QUESTION_NOT_FOUND",
                        Message = $"Không tìm thấy câu hỏi với ID = {id}"
                    }
                });
            }

            return Ok(new
            {
                success = true,
                message = "Question deleted successfully.",
                data = (object?)null
            });
        }
    }
}
