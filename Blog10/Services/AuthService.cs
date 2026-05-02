using Blog10.Data;
using Blog10.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly PasswordHasher<AdminAccount> _passwordHasher;

        public AuthService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<AdminAccount>();
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var admin = await _dbContext.AdminAccounts.FirstOrDefaultAsync(a => a.Username == username);
            if (admin == null) return false;

            if (!admin.Password.StartsWith("AQAAAA"))
            {
                if (admin.Password == password)
                {
                    admin.Password = _passwordHasher.HashPassword(admin, password);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(admin, admin.Password, password);

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                admin.Password = _passwordHasher.HashPassword(admin, password);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return result == PasswordVerificationResult.Success;
        }

        public async Task<string> GetAdminUsernameAsync()
        {
            var admin = await _dbContext.AdminAccounts.AsNoTracking().FirstOrDefaultAsync();
            return admin?.Username ?? "admin";
        }

        public async Task UpdateAdminPasswordAsync(string username, string newPassword)
        {
            var admin = await _dbContext.AdminAccounts.FirstOrDefaultAsync();

            if (admin == null)
            {
                throw new InvalidOperationException("Учетная запись администратора не найдена в базе данных.");
            }

            admin.Username = username;
            admin.Password = _passwordHasher.HashPassword(admin, newPassword);

            await _dbContext.SaveChangesAsync();
        }
    }
}