using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public class QuizAttendanceRepository : IQuizAttendanceRepository
    {
        private readonly ProjectDbContext _context;

        public QuizAttendanceRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task AddQuizResult(QuizAttendance quizAttendance)
        {
            _context.QuizAttendances.Add(quizAttendance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAttempt(QuizAttendance quizAttendance)
        {
            _context.QuizAttendances.Remove(quizAttendance);
            await _context.SaveChangesAsync();
        }

        public async Task<QuizAttendance> GetAttempt(int studentId, int quizId)
        {
            return _context.QuizAttendances.FirstOrDefault(qa => qa.StudentId == studentId && qa.QuizId == quizId);
        }
    }
}
