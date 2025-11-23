namespace LibrarySystemWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Role { get; set; } = "User";
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();

    }
}
