using LibrarySystemWebAPI.Interfaces;
using LibrarySystemWebAPI.Models;
using LibrarySystemWebAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibrarySystemWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // ------------------------------
        // Helpers
        // ------------------------------

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
            {
                return false;
            }
            return true;
        }

        // ------------------------------
        // Public / general book queries
        // ------------------------------

        [HttpGet("available")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<Book>>> GetAvailableBooks()
        {   
            return Ok(await _bookService.GetAvailableAsync());
        }

        [HttpGet("search/title")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchByTitle([FromQuery] string q)
        {
            return Ok(await _bookService.SearchByTitleAsync(q));
        }

        [HttpGet("search/author")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchByAuthor([FromQuery] string q)
        {
            return Ok(await _bookService.SearchByAuthorAsync(q));
        }

        [HttpGet("group-by-author")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> GroupByAuthor()
        {
            var groups = await _bookService.GroupByAuthorAsync();

            var result = groups.Select(g => new { Author = g.Key, Books = g.Select(b => new { b.Id, b.Title }) });
            return Ok(result);
        }

        // ------------------------------
        // User-Specific actions
        // ------------------------------

        [HttpPost("borrow/{id:int}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> BorrowBook(int id)
        {   
            if(!TryGetUserId(out var userId))
            {
                return Unauthorized("Invalid or missing user id claim");
            }
            await _bookService.BorrowAsync(id, userId);
            return Ok();
        }

        [HttpPost("return/{id:int}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            if(!TryGetUserId(out var userId))
            {
                return Unauthorized("Invalid or missing user id claim");
            }
            await _bookService.ReturnAsync(id, userId);
            return Ok();
        }

        [HttpGet("borrowed")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<BorrowedBookDTO>>> GetUserBorrowedBooks()
        {
            if(!TryGetUserId(out var userId))
            {
                return Unauthorized("Invalid or missing user id claim");
            }
            var borrowedBooks = await _bookService.GetUserBorrowedBooks(userId);
            return Ok(borrowedBooks);
        }

        [HttpGet("find-books/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> FindBooks(int id)
        {
            var booksBorrowedByUsers = await _bookService.FindBooksBorrowedByUsersAsync(id);
            return Ok(booksBorrowedByUsers);
        }

        // ------------------------------
        // Admin-Specific actions
        // ------------------------------

        
        [HttpGet("top-borrowed")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Book>>> GetTopBorrowedBooks()
        {
            return Ok(await _bookService.GetTopBorrowedBooksAsync());
        }
        
        [HttpGet("all-books")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            return Ok(await _bookService.GetAllAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Book>> AddBookAsync([FromBody] BookDTO newBook)
        {
            await _bookService.AddAsync(newBook);
            return CreatedAtAction(nameof(GetAvailableBooks), null);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookAsync(int id, BookDTO updatedBook)
        {
            await _bookService.UpdateAsync(id, updatedBook);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }
    }
}
