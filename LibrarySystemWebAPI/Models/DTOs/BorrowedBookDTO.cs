namespace LibrarySystemWebAPI.Models.DTOs
{
    public class BorrowedBookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime BorrowedDate { get; set; }
    }
}
