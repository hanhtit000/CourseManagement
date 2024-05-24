using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_CreateAPI.DTO;
using Project_CreateAPI.Models;
using Project_CreateAPI.Repositories;

namespace Project_CreateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizResultController : ControllerBase
    {
        private readonly IQuizAttendanceRepository _quizAttendanceRepository;

        public QuizResultController(IQuizAttendanceRepository quizAttendanceRepository)
        {
            _quizAttendanceRepository = quizAttendanceRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddQuizResult(QuizAttendanceDTO dto)
        {
            var result = new QuizAttendance()
            {
                AttendanceId = dto.AttendanceId,
                QuizId = dto.QuizId,
                StudentId = dto.StudentId,
                Score = dto.Score
            };
            await _quizAttendanceRepository.AddQuizResult(result);
            return Ok("Result saved successfully!");
        }

        [HttpDelete("{studentId}&{quizId}")]
        public async Task<IActionResult> DeleteAttempt(int studentId, int quizId)
        {
            var attempt = await _quizAttendanceRepository.GetAttempt(studentId, quizId);
            if(attempt == null)
            {
                return NotFound("Can't find student's attempt!");
            }
            await _quizAttendanceRepository.DeleteAttempt(attempt);
            return Ok("Remove student's attempt successfully!");
        }
    }
}
