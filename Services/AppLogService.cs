using Blog10.Data;
using Blog10.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Services
{
    public class AppLogService
    {
        private readonly AppDbContext _context;

        public AppLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string level, string source, string message, string? exceptionDetails = null)
        {
            try
            {
                var log = new SystemLog
                {
                    Level = level,
                    Source = source,
                    Message = message,
                    ExceptionDetails = exceptionDetails,
                    CreatedAt = DateTime.Now
                };

                _context.SystemLogs.Add(log);

                var count = await _context.SystemLogs.CountAsync();
                if (count >= 1000)
                {
                    var oldestLogs = await _context.SystemLogs
                        .OrderBy(l => l.CreatedAt)
                        .Take(count - 900) 
                        .ToListAsync();

                    _context.SystemLogs.RemoveRange(oldestLogs);
                }

                await _context.SaveChangesAsync();
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