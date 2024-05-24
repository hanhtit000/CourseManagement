using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public interface IStudentAssignmentRepository
    {
        Task AddSubmission(StudentAssignment studentAssignment);
        Task UpdateSubmission(StudentAssignment studentAssignment);
        Task<StudentAssignment> GetSubmissionById(int id);
    }
}
