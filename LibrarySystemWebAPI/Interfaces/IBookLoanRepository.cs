using LibrarySystemWebAPI.Models;

namespace LibrarySystemWebAPI.Interfaces
{
    public interface IBookLoanRepository
    {
        Task<bool> IsBookBorrowedAsync(int bookId, int userId);
        Task<BookLoan?> GetActiveLoanAsync(int bookId, int userId);
        Task<IEnumerable<BookLoan>> GetLoansByUserAsync(int userId);
        Task<IEnumerable<User>> FindUsersByBookAsync(int bookId);
        Task AddAsync(BookLoan bookLoan);
        Task SaveAsync();
    }
}
