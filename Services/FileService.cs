using Microsoft.EntityFrameworkCore;
using Blog10.Data;
using System.Text.RegularExpressions;

namespace Blog10.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IServiceScopeFactory _scopeFactory;

        public FileService(IWebHostEnvironment env, IServiceScopeFactory scopeFactory)
        {
            _env = env;
            _scopeFactory = scopeFactory;
        }

        public void DeleteFile(string? relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl) || relativeUrl.Contains("default-"))
                return;

            try
            {
                var normalizedPath = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var physicalPath = Path.Combine(_env.WebRootPath, normalizedPath);

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch { }
        }

        public void RunGarbageCollectorInBackground()
        {
            Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    await CleanOrphanedFilesAsync(dbContext);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка фонового сборщика мусора: {ex.Message}");
                }
            });
        }

        private async Task CleanOrphanedFilesAsync(AppDbContext dbContext)
        {
            var usedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1. Статьи
            var articles = await dbContext.Articles.AsNoTracking().ToListAsync();
            foreach (var a in articles)
            {
                if (!string.IsNullOrEmpty(a.CoverImageUrl)) usedUrls.Add(a.CoverImageUrl);
                if (!string.IsNullOrEmpty(a.MainAudioUrl)) usedUrls.Add(a.MainAudioUrl);
                if (!string.IsNullOrEmpty(a.MainVideoUrl)) usedUrls.Add(a.MainVideoUrl);
                if (!string.IsNullOrEmpty(a.FileAttachmentUrl)) usedUrls.Add(a.FileAttachmentUrl);

                if (!string.IsNullOrEmpty(a.Content))
                {
                    var matches = Regex.Matches(a.Content, @"src=""(/uploads/[^""]+)""");
                    foreach (Match m in matches) usedUrls.Add(m.Groups[1].Value);
                }
            }

            // 2. Отзывы
            var reviews = await dbContext.Reviews.AsNoTracking().ToListAsync();
            foreach (var r in reviews)
            {
                if (!string.IsNullOrEmpty(r.ImageUrl)) usedUrls.Add(r.ImageUrl);
            }

            // 3. Обо мне
            var aboutData = await dbContext.AboutPage.AsNoTracking().FirstOrDefaultAsync();
            if (aboutData != null)
            {
                if (!string.IsNullOrEmpty(aboutData.ProfileImageUrl)) usedUrls.Add(aboutData.ProfileImageUrl);
                if (!string.IsNullOrEmpty(aboutData.MainHeroImageUrl)) usedUrls.Add(aboutData.MainHeroImageUrl);

                foreach (var htmlField in new[] { aboutData.BioHtml, aboutData.EducationHtml, aboutData.PrinciplesHtml })
                {
                    if (!string.IsNullOrEmpty(htmlField))
                    {
                        var matches = Regex.Matches(htmlField, @"src=""(/uploads/[^""]+)""");
                        foreach (Match m in matches) usedUrls.Add(m.Groups[1].Value);
                    }
                }
            }

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (Directory.Exists(uploadsPath))
            {
                var allFiles = Directory.GetFiles(uploadsPath, "*.*", SearchOption.AllDirectories);
                foreach (var file in allFiles)
                {
                    var relativePath = "/" + file.Replace(_env.WebRootPath, "").TrimStart('\\', '/').Replace("\\", "/");
                    if (relativePath.Contains(".git") || relativePath.Contains(".keep")) continue;

                    if (!usedUrls.Contains(relativePath))
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
            }
        }
    }
}