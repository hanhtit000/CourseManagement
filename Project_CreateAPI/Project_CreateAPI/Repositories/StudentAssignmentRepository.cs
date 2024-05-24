using Microsoft.EntityFrameworkCore;
using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public class StudentAssignmentRepository : IStudentAssignmentRepository
    {
        private readonly ProjectDbContext _context;

        public StudentAssignmentRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task AddSubmission(StudentAssignment studentAssignment)
        {
            _context.StudentAssignments.Add(studentAssignment);
            await _context.SaveChangesAsync();
        }

        public async Task<StudentAssignment> GetSubmissionById(int id)
        {
            return await _context.StudentAssignments.Include(s => s.Assignment).Include(s => s.Student).FirstOrDefaultAsync(s => s.SubmissionId == id);
        }

        public async Task UpdateSubmission(StudentAssignment studentAssignment)
        {
            _context.StudentAssignments.Update(studentAssignment);
            await _context.SaveChangesAsync();
        }
    }
}
