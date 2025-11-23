using LibrarySystemWebAPI.Interfaces;
using LibrarySystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemWebAPI.Data
{
    public class EFBookRepository : IBookRepository
    {
        private readonly AppDbContext _db;
        public EFBookRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book != null)
            {
                _db.Books.Remove(book);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Book> GetBookAsync(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book != null) 
            {
                return book;
            }
            return null;
        }

        public async Task<IReadOnlyList<Book>> GetBooksAsync()
        {
            IQueryable<Book> books = _db.Books;
            return await books.ToListAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            var newBook = await _db.Books.FindAsync(book.Id);
            newBook.Title = book.Title;
            newBook.Author = book.Author;
            newBook.ISBN = book.ISBN;
            newBook.CopiesAvailable = book.CopiesAvailable;
            _db.Books.Update(newBook);
            await _db.SaveChangesAsync();
        }
    }
}
