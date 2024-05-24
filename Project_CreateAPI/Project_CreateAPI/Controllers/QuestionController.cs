using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Project_CreateAPI.DTO;
using Project_CreateAPI.Models;
using Project_CreateAPI.Repositories;

namespace Project_CreateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ProjectDbContext _context;

        public QuestionController(IQuestionRepository questionRepository, ProjectDbContext context)
        {
            _questionRepository = questionRepository;
            _context = context;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _questionRepository.GetAllQuestions();
            if(questions == null)
            {
                return NotFound("Can't get list of questions");
            }
            return Ok(questions);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(QuestionDTO dto)
        {
            var question = new Question()
            {
                QuestionId = dto.QuestionId,
                QuizId = dto.QuizId,
                Question1 = dto.Question1,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                CorrectOption = dto.CorrectOption
            };
            await _questionRepository.AddQuestion(question);
            return Ok("Question added successfully!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, QuestionDTO dto)
        {
            var question = await _questionRepository.GetQuestionById(id);
            if(question == null)
            {
                return NotFound("Can't get the question!");
            }
            question.Question1 = dto.Question1;
            question.OptionA = dto.OptionA;
            question.OptionB = dto.OptionB;
            question.OptionC = dto.OptionC;
            question.OptionD = dto.OptionD;
            question.CorrectOption = dto.CorrectOption;
            await _questionRepository.UpdateQuestion(question);
            return Ok("Question updated successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteQuestion(int id)
        {
            var questionToDelete = _context.Questions.Find(id);
            if (questionToDelete == null)
            {
                return NotFound();
            }
            _context.Questions.Remove(questionToDelete);
            _context.SaveChanges();
            return Ok("Question deleted successfully!");
        }
    }
}
