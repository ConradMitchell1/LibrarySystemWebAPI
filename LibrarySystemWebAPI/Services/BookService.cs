using LibrarySystemWebAPI.Interfaces;
using LibrarySystemWebAPI.Models;
using LibrarySystemWebAPI.Models.DTOs;

namespace LibrarySystemWebAPI.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookLoanRepository _bookLoanRepository;
        public event BookActionHandler? OnBookBorrowed;
        public event BookActionHandler? OnBookReturned;
        public BookService(IBookRepository bookRepository, IBookLoanRepository bookLoanRepository)
        {
            _bookRepository = bookRepository;
            _bookLoanRepository = bookLoanRepository;
        }
        public async Task BorrowAsync(int bookId, int userId)
        {
            var book = await _bookRepository.GetBookAsync(bookId);
            if (book == null || book.CopiesAvailable == 0)
            {
                throw new Exception("Book not found");
            }
            // Check if book is already borrowed by the user.
            if(await _bookLoanRepository.IsBookBorrowedAsync(bookId, userId))
            {
                throw new Exception("Book already borrowed by the user");
            }

            var loan = new BookLoan
            {
                BookId = bookId,
                UserId = userId,
                LoanDate = DateTime.UtcNow,
            };
            book.Borrow();
            await _bookLoanRepository.AddAsync(loan);
            await _bookLoanRepository.SaveAsync();
            await _bookRepository.UpdateAsync(book);

            OnBookBorrowed?.Invoke(book);

        }

        public async Task<IEnumerable<Book>> GetAvailableAsync()
        {
            var books = await _bookRepository.GetBooksAsync();
            return books.Where(b => b.CopiesAvailable > 0);
        }

        public async Task<IEnumerable<Book>> GetTopBorrowedBooksAsync(int count = 3)
        {
            var books = await _bookRepository.GetBooksAsync();
            return books.OrderByDescending(b => b.BorrowCount).Take(count);
        }

        public async Task<IEnumerable<IGrouping<string, Book>>> GroupByAuthorAsync()
        {
            var books = await _bookRepository.GetBooksAsync();
            var groupedBooks = books.GroupBy(b => b.Author);
            return groupedBooks;
        }

        public async Task ReturnAsync(int bookId, int userId)
        {
            var loan = await _bookLoanRepository.GetActiveLoanAsync(bookId, userId);
            if(loan == null)
            {
                throw new Exception("No active loan found for this book and user");
            }
            loan.Book.Return();
            loan.ReturnDate = DateTime.UtcNow;
            await _bookLoanRepository.SaveAsync();
            await _bookRepository.UpdateAsync(loan.Book);

            OnBookReturned?.Invoke(loan.Book);
        }

        public async Task<IEnumerable<Book>> SearchByAuthorAsync(string author)
        {
            var books = await _bookRepository.GetBooksAsync();
            return books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Book>> SearchByTitleAsync(string title)
        {
            var books = await _bookRepository.GetBooksAsync();
            return books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        public async Task AddAsync(BookDTO bookDTO)
        {
            var book = bookDTO.ToEntity();
            book.ChartColor = GenerateRandomHexColor();
            book.ChartBorderColor = DarkenColor(book.ChartColor, 30);

            await _bookRepository.AddAsync(book);
        }

        public async Task DeleteAsync(int id)
        {
            await _bookRepository.DeleteAsync(id);
        }

        public async Task UpdateAsync(int id, BookDTO book)
        {
            await _bookRepository.UpdateAsync(book.ToEntity(id));
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            var book = await _bookRepository.GetBookAsync(id);
            return book;
        }

        public async Task<IReadOnlyList<Book>> GetAllAsync()
        {
            var books = await _bookRepository.GetBooksAsync();
            return books;
        }

        public async Task<IEnumerable<BorrowedBookDTO>> GetUserBorrowedBooks(int id)
        {
            var books = await _bookLoanRepository.GetLoansByUserAsync(id);
            return books.Select(b => new BorrowedBookDTO
            {
                BookId = b.BookId,
                Title = b.Book.Title,
                Author = b.Book.Author,
                BorrowedDate = b.LoanDate
            });
        }

        public async Task<IEnumerable<UserDTO>> FindBooksBorrowedByUsersAsync(int bookId)
        {
            var users = await _bookLoanRepository.FindUsersByBookAsync(bookId);
            return users.Select(u => new UserDTO
            {
                UserName = u.UserName,
                Password = ""
            });
        }
        private static string GenerateRandomHexColor()
        {
            var rand = new Random();
            return $"#{rand.Next(0x1000000):X6}"; // Produces "#A1B2C3"
        }
        private static string DarkenColor(string hex, int amount = 40)
        {
            int r = Convert.ToInt32(hex.Substring(1, 2), 16) - amount;
            int g = Convert.ToInt32(hex.Substring(3, 2), 16) - amount;
            int b = Convert.ToInt32(hex.Substring(5, 2), 16) - amount;

            r = Math.Clamp(r, 0, 255);
            g = Math.Clamp(g, 0, 255);
            b = Math.Clamp(b, 0, 255);

            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }
}
