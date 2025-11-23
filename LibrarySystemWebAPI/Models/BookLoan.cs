namespace LibrarySystemWebAPI.Models
{
    public class BookLoan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = default!;
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public bool IsReturned => ReturnDate.HasValue;
    }
}
