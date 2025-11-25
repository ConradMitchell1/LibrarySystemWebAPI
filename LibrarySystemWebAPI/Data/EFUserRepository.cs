using LibrarySystemWebAPI.Interfaces;
using LibrarySystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemWebAPI.Data
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public EFUserRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if(user != null)
            {
                _db.Users.Remove(user);
            }
        }

        public async Task<User> GetByUserNameAsync(string username)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public async Task<IReadOnlyList<User>> GetUsersAsync()
        {
            IQueryable<User> users = _db.Users;
            return await users.ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
