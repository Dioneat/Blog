using Blog10.Data;
using Blog10.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Services
{
    public class SettingsService
    {
        private readonly AppDbContext _context;
        private readonly EncryptionService _encryption;

        public SettingsService(AppDbContext context, EncryptionService encryption)
        {
            _context = context;
            _encryption = encryption;
        }

        public async Task<string> GetSettingAsync(string key)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting == null || string.IsNullOrEmpty(setting.Value)) return "";

            return setting.IsEncrypted ? _encryption.Decrypt(setting.Value) : setting.Value;
        }

        public async Task SetSettingAsync(string key, string value, bool encrypt = true)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
            string processedValue = encrypt ? _encryption.Encrypt(value) : value;

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
        }
    }
}