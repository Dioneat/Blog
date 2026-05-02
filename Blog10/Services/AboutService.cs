using Blog10.Data;
using Blog10.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Services
{
    public class AboutService : IAboutService
    {
        private readonly AppDbContext _context;

        public AboutService(AppDbContext context) => _context = context;

        public async Task<AboutData> GetAboutDataAsync()
        {
            var data = await _context.AboutPage.AsNoTracking().FirstOrDefaultAsync();
            if (data == null)
            {
                data = new AboutData { Id = 1, BioHtml = "<p>Напишите здесь о себе...</p>" };
            }
            return data;
        }

        public async Task UpdateAboutDataAsync(AboutData data)
        {
            var existing = await _context.AboutPage.FirstOrDefaultAsync();
            if (existing == null) _context.AboutPage.Add(data);
            else _context.Entry(existing).CurrentValues.SetValues(data);

            await _context.SaveChangesAsync();
        }
    }
}