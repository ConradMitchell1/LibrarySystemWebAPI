using LibrarySystemWebAPI.Models;

namespace LibrarySystemWebAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(int id);
        Task<User> GetByUserNameAsync(string username);
        Task<IReadOnlyList<User>> GetUsersAsync();
        Task AddAsync(User user);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
