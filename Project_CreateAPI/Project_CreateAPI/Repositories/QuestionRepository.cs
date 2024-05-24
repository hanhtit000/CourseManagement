using Microsoft.EntityFrameworkCore;
using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ProjectDbContext _context;

        public QuestionRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task AddQuestion(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Question>> GetAllQuestions()
        {
            return _context.Questions.AsQueryable();
        }

        public async Task<Question> GetQuestionById(int id)
        {
            return await _context.Questions.Include(q => q.Quiz).FirstOrDefaultAsync(q => q.QuestionId == id);
        }

        public async Task UpdateQuestion(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }
    }
}
