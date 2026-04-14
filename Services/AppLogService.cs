using Blog10.Data;
using Blog10.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Services
{
    public class AppLogService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AppLogService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task LogAsync(string level, string source, string message, string? exceptionDetails = null)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var log = new SystemLog
                {
                    Level = level,
                    Source = source,
                    Message = message,
                    ExceptionDetails = exceptionDetails,
                    CreatedAt = DateTime.Now
                };

                context.SystemLogs.Add(log);

                var count = await context.SystemLogs.CountAsync();
                if (count >= 1000)
                {
                    var oldestLogs = await context.SystemLogs
                        .OrderBy(l => l.CreatedAt)
                        .Take(count - 900)
                        .ToListAsync();

                    context.SystemLogs.RemoveRange(oldestLogs);
                }

                await context.SaveChangesAsync();
            }
            catch
            {
            }
        }

        public async Task LogInfoAsync(string source, string message) =>
            await LogAsync("Info", source, message);

        public async Task LogWarningAsync(string source, string message) =>
            await LogAsync("Warning", source, message);

        public async Task LogErrorAsync(string source, string message, Exception? ex = null) =>
            await LogAsync("Error", source, message, ex?.ToString());
    }
}