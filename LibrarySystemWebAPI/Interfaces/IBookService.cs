using LibrarySystemWebAPI.Models;
using LibrarySystemWebAPI.Models.DTOs;

namespace LibrarySystemWebAPI.Interfaces
{
    public delegate void BookActionHandler(Book book);
    public interface IBookService
    {
        // Domain Notifications

        event BookActionHandler? OnBookBorrowed;
        event BookActionHandler? OnBookReturned;

        // Queries
        Task<IEnumerable<Book>> GetAvailableAsync();
        Task<IEnumerable<BorrowedBookDTO>> GetUserBorrowedBooks(int id);
        Task<IEnumerable<Book>> SearchByTitleAsync(string title);
        Task<IEnumerable<Book>> SearchByAuthorAsync(string title);
        Task<IEnumerable<IGrouping<string, Book>>> GroupByAuthorAsync();
        Task<IEnumerable<Book>> GetTopBorrowedBooksAsync(int count = 3);

        Task<IEnumerable<UserDTO>> FindBooksBorrowedByUsersAsync(int bookId);

        Task AddAsync(BookDTO book);
        Task DeleteAsync(int id);
        Task UpdateAsync(int id, BookDTO book);
        Task<IReadOnlyList<Book>> GetAllAsync();
        Task BorrowAsync(int bookId, int userId);
        Task ReturnAsync(int bookId, int userId); 
    }
}
