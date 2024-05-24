using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public interface IAssignmentRepository
    {
        Task CreateAssignment(Assignment assignment);
        Task<Assignment> GetAssignmentById(int id);
        Task UpdateAssignment(Assignment assignment);
    }
}
