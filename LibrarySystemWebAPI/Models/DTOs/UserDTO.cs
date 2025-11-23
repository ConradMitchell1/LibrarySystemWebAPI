using System.ComponentModel.DataAnnotations;

namespace LibrarySystemWebAPI.Models.DTOs
{
    public class UserDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
