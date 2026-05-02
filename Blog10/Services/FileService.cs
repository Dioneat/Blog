using Blog10.Data;
using Blog10.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Blog10.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly Regex UploadSrcRegex = new(
            @"(?:src|href)=[""'](?<url>/uploads/[^""']+)[""']",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public FileService(IWebHostEnvironment env, IServiceScopeFactory scopeFactory)
        {
            _env = env;
            _scopeFactory = scopeFactory;
        }

        public void DeleteFile(string? relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
                return;

            if (relativeUrl.Contains("default-", StringComparison.OrdinalIgnoreCase))
                return;

            if (!relativeUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                var physicalPath = BuildSafePhysicalPath(relativeUrl);

                if (physicalPath == null)
                    return;

                if (File.Exists(physicalPath))
                    File.Delete(physicalPath);
            }
            catch
            {
                // Лучше не ронять сайт из-за ошибки удаления файла.
                // При желании сюда можно добавить ILogger<FileService>.
            }
        }

        public void RunGarbageCollectorInBackground()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await RunGarbageCollectorAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка фонового сборщика мусора: {ex.Message}");
                }
            });
        }

        public async Task RunGarbageCollectorAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await CleanOrphanedFilesAsync(dbContext, cancellationToken);
        }

        private async Task CleanOrphanedFilesAsync(
            AppDbContext dbContext,
            CancellationToken cancellationToken = default)
        {
            var usedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            await CollectArticleFilesAsync(dbContext, usedUrls, cancellationToken);
            await CollectReviewFilesAsync(dbContext, usedUrls, cancellationToken);
            await CollectAboutFilesAsync(dbContext, usedUrls, cancellationToken);

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsPath))
                return;

            var allFiles = Directory.GetFiles(uploadsPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var relativePath = ToRelativeUploadUrl(file);

                if (string.IsNullOrWhiteSpace(relativePath))
                    continue;

                if (relativePath.Contains(".git", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (relativePath.Contains(".keep", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!usedUrls.Contains(relativePath))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // Не роняем сборщик мусора из-за одного файла.
                    }
                }
            }
        }

        private async Task CollectArticleFilesAsync(
            AppDbContext dbContext,
            HashSet<string> usedUrls,
            CancellationToken cancellationToken)
        {
            var articles = await dbContext.Articles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var article in articles)
            {
                AddIfUploadUrl(usedUrls, article.CoverImageUrl);
                AddIfUploadUrl(usedUrls, article.MainAudioUrl);
                AddIfUploadUrl(usedUrls, article.MainVideoUrl);
                AddIfUploadUrl(usedUrls, article.FileAttachmentUrl);

                AddUploadUrlsFromHtml(usedUrls, article.Content);
            }
        }

        private async Task CollectReviewFilesAsync(
            AppDbContext dbContext,
            HashSet<string> usedUrls,
            CancellationToken cancellationToken)
        {
            var reviews = await dbContext.Reviews
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var review in reviews)
            {
                AddIfUploadUrl(usedUrls, review.ImageUrl);
            }
        }

        private async Task CollectAboutFilesAsync(
            AppDbContext dbContext,
            HashSet<string> usedUrls,
            CancellationToken cancellationToken)
        {
            var aboutData = await dbContext.AboutPage
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (aboutData == null)
                return;

            AddIfUploadUrl(usedUrls, aboutData.ProfileImageUrl);
            AddIfUploadUrl(usedUrls, aboutData.MainHeroImageUrl);

            AddUploadUrlsFromHtml(usedUrls, aboutData.BioHtml);
            AddUploadUrlsFromHtml(usedUrls, aboutData.EducationHtml);
            AddUploadUrlsFromHtml(usedUrls, aboutData.PrinciplesHtml);
        }

        private static void AddUploadUrlsFromHtml(HashSet<string> usedUrls, string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return;

            var matches = UploadSrcRegex.Matches(html);

            foreach (Match match in matches)
            {
                var url = match.Groups["url"].Value;
                AddIfUploadUrl(usedUrls, url);
            }
        }

        private static void AddIfUploadUrl(HashSet<string> usedUrls, string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            if (!url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                return;

            usedUrls.Add(url);
        }

        private string? BuildSafePhysicalPath(string relativeUrl)
        {
            var normalizedRelativePath = relativeUrl
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            var webRootFullPath = Path.GetFullPath(_env.WebRootPath);
            var physicalFullPath = Path.GetFullPath(
                Path.Combine(webRootFullPath, normalizedRelativePath));

            if (!physicalFullPath.StartsWith(webRootFullPath, StringComparison.OrdinalIgnoreCase))
                return null;

            return physicalFullPath;
        }

        private string? ToRelativeUploadUrl(string physicalFilePath)
        {
            var webRootFullPath = Path.GetFullPath(_env.WebRootPath);
            var fileFullPath = Path.GetFullPath(physicalFilePath);

            if (!fileFullPath.StartsWith(webRootFullPath, StringComparison.OrdinalIgnoreCase))
                return null;

            var relativePath = fileFullPath[webRootFullPath.Length..]
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Replace(Path.DirectorySeparatorChar, '/')
                .Replace(Path.AltDirectorySeparatorChar, '/');

            return "/" + relativePath;
        }
    }
}