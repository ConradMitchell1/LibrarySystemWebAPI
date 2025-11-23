namespace LibrarySystemWebAPI.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage)> SignUpAsync(string username, string password);
        Task<(bool Success, string? Token, string? Role, string? ErrorMessage)> LoginAsync(string username, string password);
    }
}
