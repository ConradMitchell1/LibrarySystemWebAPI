using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace LibrarySystemWebAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int BorrowCount { get; private set; }
        public int CopiesAvailable { get; set; }

        // Chart fields
        public string ChartColor { get; set; } = "#000000";
        public string ChartBorderColor { get; set; } = "#000000";

        [JsonIgnore]
        public ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();

        public void Borrow()
        {
            CopiesAvailable--;
            BorrowCount++;
        }

        public void Return()
        {
            CopiesAvailable++;
        }
    }
}
