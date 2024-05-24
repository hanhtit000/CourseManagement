using Microsoft.EntityFrameworkCore;
using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ProjectDbContext _context;

        public AssignmentRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task CreateAssignment(Assignment assignment)
        {
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<Assignment> GetAssignmentById(int id)
        {
            return await _context.Assignments.Include(a => a.Course).Include(a => a.StudentAssignments).ThenInclude(sa => sa.Student).FirstOrDefaultAsync(a => a.AssignmentId == id);
        }

        public async Task UpdateAssignment(Assignment assignment)
        {
            _context.Assignments.Update(assignment);
            await _context.SaveChangesAsync();
        }
    }
}
