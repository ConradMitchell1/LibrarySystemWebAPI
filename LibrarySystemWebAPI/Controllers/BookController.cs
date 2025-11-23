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

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Book>>> GetAvailableBooks()
        {   
            return Ok(await _bookService.GetAvailableAsync());
        }
        [HttpGet("borrowed")]
        public async Task<ActionResult<IEnumerable<BorrowedBookDTO>>> GetUserBorrowedBooks()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Invalid or missing user id claim");
            }
            var borrowedBooks = await _bookService.GetUserBorrowedBooks(userId);
            return Ok(borrowedBooks);
        }

        [HttpGet("find-books/{id:int}")]
        public async Task<ActionResult<IEnumerable<User>>> FindBooks(int id)
        {
            var booksBorrowedByUsers = await _bookService.FindBooksBorrowedByUsersAsync(id);
            return Ok(booksBorrowedByUsers);
        }


        [HttpPost("borrow/{id:int}")]
        public async Task<IActionResult> BorrowBook(int id)
        {   
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdString);
            await _bookService.BorrowAsync(id, userId);
            return Ok();
        }
        [HttpPost("return/{id:int}")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdString);
            await _bookService.ReturnAsync(id, userId);
            return Ok();
        }
        [HttpGet("top-borrowed")]
        public async Task<ActionResult<IEnumerable<Book>>> GetTopBorrowedBooks()
        {
            return Ok(await _bookService.GetTopBorrowedBooksAsync());
        }
        [HttpGet("search/title")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchByTitle([FromQuery] string q)
        {
            return Ok(await _bookService.SearchByTitleAsync(q));
        }
        [HttpGet("search/author")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchByAuthor([FromQuery] string q)
        {
            return Ok(await _bookService.SearchByAuthorAsync(q));
        }
        [HttpGet("group-by-author")]
        public async Task<ActionResult> GroupByAuthor()
        {
            var groups = await _bookService.GroupByAuthorAsync();

            var result = groups.Select(g => new { Author = g.Key, Books = g.Select(b => new { b.Id, b.Title }) });
            return Ok(result);
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
