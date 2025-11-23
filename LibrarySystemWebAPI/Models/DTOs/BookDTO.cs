using System.ComponentModel.DataAnnotations;

namespace LibrarySystemWebAPI.Models.DTOs
{
    public class BookDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [Required]
        public string ISBN { get; set; } = string.Empty;
        [Required]
        public int CopiesAvailable { get; set; }

        public Book ToEntity() =>
            new()
            {
                Title = Title,
                Author = Author,
                ISBN = ISBN,
                CopiesAvailable = CopiesAvailable,
            };
        public Book ToEntity(int id) =>
            new()
            {
                Id = id,
                Title = Title,
                Author = Author,
                ISBN = ISBN,
                CopiesAvailable = CopiesAvailable,
            };
    }
}
