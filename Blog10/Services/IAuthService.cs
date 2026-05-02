namespace Blog10.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task UpdateAdminPasswordAsync(string username, string newPassword);
        Task<string> GetAdminUsernameAsync();
    }
}