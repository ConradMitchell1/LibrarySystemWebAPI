using LibrarySystemWebAPI.Models;

namespace LibrarySystemWebAPI.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> GetBookAsync(int id);
        Task<IReadOnlyList<Book>> GetBooksAsync();
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
    }
}
