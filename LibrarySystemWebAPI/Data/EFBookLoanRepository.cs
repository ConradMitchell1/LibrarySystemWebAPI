using LibrarySystemWebAPI.Interfaces;
using LibrarySystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemWebAPI.Data
{
    public class EFBookLoanRepository : IBookLoanRepository
    {
        private readonly AppDbContext _db;
        public EFBookLoanRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(BookLoan bookLoan)
        {
            await _db.BookLoans.AddAsync(bookLoan);
        }

        public Task<BookLoan?> GetActiveLoanAsync(int bookId, int userId)
        {
            return _db.BookLoans
                .Include(bl => bl.Book)
                .SingleOrDefaultAsync(bl => bl.BookId == bookId && bl.UserId == userId && bl.ReturnDate == null);
        }

        public async Task<IEnumerable<BookLoan>> GetLoansByUserAsync(int userId)
        {
            var bookLoans = await _db.BookLoans
                .Where(bl => bl.UserId == userId && bl.ReturnDate == null)
                .Include(bl => bl.Book)
                .ToListAsync();
            return bookLoans;
        }

        public async Task<IEnumerable<User>> FindUsersByBookAsync(int bookId)
        {
            var users = await _db.BookLoans
                .Where(bl => bl.BookId == bookId && bl.ReturnDate == null)
                .Include(bl => bl.User)
                .Select(bl => bl.User)
                .Distinct()
                .ToListAsync();
            return users;
        }

        public Task<bool> IsBookBorrowedAsync(int bookId, int userId)
        {
            return _db.BookLoans.AnyAsync(l => l.BookId == bookId && l.UserId == userId && l.ReturnDate == null);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
            int x = Convert.ToInt32("32");
        }
    }
}
