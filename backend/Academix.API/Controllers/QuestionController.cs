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

        // GET: api/Question
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        // GET: api/Question/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        // POST: api/Question
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _questionService.CreateQuestionAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.QuestionId }, created);
        }

        // PUT: api/Question/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _questionService.UpdateQuestionAsync(id, request);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE: api/Question/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _questionService.DeleteQuestionAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
