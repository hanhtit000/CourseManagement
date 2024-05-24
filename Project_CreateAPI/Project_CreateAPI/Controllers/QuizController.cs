using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Project_CreateAPI.DTO;
using Project_CreateAPI.Models;
using Project_CreateAPI.Repositories;

namespace Project_CreateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ProjectDbContext _context;

        public QuizController(IQuizRepository quizRepository, ProjectDbContext context)
        {
            _quizRepository = quizRepository;
            _context = context;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _quizRepository.GetAllQuizzes();
            if(quizzes == null)
            {
                return NotFound("Can't get list quizzes");
            }
            return Ok(quizzes);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuiz(QuizDTO dto)
        {
            var quiz = new Quiz()
            {
                QuizId = dto.QuizId,
                CourseId = dto.CourseId,
                Title = dto.Title
            };
            await _quizRepository.AddQuiz(quiz);
            return Ok("Quiz created successfully!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, QuizDTO dto)
        {
            var quiz = await _quizRepository.GetQuizById(id);
            if(quiz == null)
            {
                return NotFound("Can't find the quiz");
            }
            quiz.Title = dto.Title;
            await _quizRepository.UpdateQuiz(quiz);
            return Ok("Quiz updated successfully!");
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> GetQuizById(int id)
        {
            var quiz = await _quizRepository.GetQuizById(id);
            if(quiz == null)
            {
                return NotFound("Can't find the quiz");
            }
            return Ok(quiz);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteQuiz(int id)
        {
            //Delete child records
            var questionsToDelete = _context.Questions
                .Where(q => q.QuizId == id);
            _context.Questions.RemoveRange(questionsToDelete);
            _context.SaveChanges();

            var quizAttendancesToDelete = _context.QuizAttendances
                .Where(qa => qa.QuizId == id);
            _context.QuizAttendances.RemoveRange(quizAttendancesToDelete);
            _context.SaveChanges();

            //Delete the quiz
            var quizToDelete = _context.Quizzes.Find(id);
            if (quizToDelete == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quizToDelete);
            _context.SaveChanges();

            return Ok("Quiz and associated records deleted successfully!");
        }

    }
}
