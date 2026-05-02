using Blog10.Data;
using Blog10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog10.Services
{
    public class SettingsService
    {
        private readonly AppDbContext _context;
        private readonly EncryptionService _encryption;
        private readonly IMemoryCache _cache;

        public SettingsService(AppDbContext context, EncryptionService encryption, IMemoryCache cache)
        {
            _context = context;
            _encryption = encryption;
            _cache = cache;
        }

        public async Task<string> GetSettingAsync(string key)
        {
            if (_cache.TryGetValue(key, out string? cachedValue))
            {
                return cachedValue ?? "";
            }

            var setting = await _context.Settings.AsNoTracking().FirstOrDefaultAsync(s => s.Key == key);

            if (setting == null || string.IsNullOrEmpty(setting.Value))
                return "";

            string result = setting.IsEncrypted ? _encryption.Decrypt(setting.Value) : setting.Value;

            _cache.Set(key, result, TimeSpan.FromHours(24));

            return result;
        }

        public async Task SetSettingAsync(string key, string value, bool encrypt = true)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);

            string safeValue = value ?? "";
            string processedValue = encrypt && !string.IsNullOrEmpty(safeValue)
                ? _encryption.Encrypt(safeValue)
                : safeValue;

            if (setting == null)
            {
                _context.Settings.Add(new AppSetting { Key = key, Value = processedValue, IsEncrypted = encrypt });
            }
            else
            {
                setting.Value = processedValue;
                setting.IsEncrypted = encrypt;
            }

            await _context.SaveChangesAsync();

            _cache.Set(key, safeValue, TimeSpan.FromHours(24));
        }
    }
}