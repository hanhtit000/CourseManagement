using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<List<User>> GetUsers();
    }
}
