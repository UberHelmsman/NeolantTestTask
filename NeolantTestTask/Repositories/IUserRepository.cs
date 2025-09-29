using NeolantTestTask.Models;

namespace NeolantTestTask.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task UpdateAsync(User user, string? avatarFileName);
        Task DeleteAsync(int id);
    }
}