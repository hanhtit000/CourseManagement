using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public interface IQuizAttendanceRepository
    {
        Task AddQuizResult(QuizAttendance quizAttendance);
        Task<QuizAttendance> GetAttempt(int studentId, int quizId);
        Task DeleteAttempt(QuizAttendance quizAttendance);
    }
}
