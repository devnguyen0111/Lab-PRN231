namespace Services.IService
{
    public interface IAccountService
    {
        Task<string> LoginAsync(string email, string password);
        Task RegisterAsync(string username, string password, string email);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
        Task LogoutAsync();
        Task<string> GetCurrentUserIdAsync();
        Task<string> GetCurrentUserNameAsync();
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
    }
}
