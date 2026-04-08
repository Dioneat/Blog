using Blog10.Data;
using Blog10.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Services
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _dbContext;

        public BlogService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<string>> GetAllTagsAsync()
        {
            var posts = await _dbContext.Articles.Where(a => !a.IsDraft).ToListAsync();
            return posts.Where(p => p.Tags != null)
                        .SelectMany(p => p.Tags)
                        .Distinct()
                        .OrderBy(t => t)
                        .ToList();
        }

        public async Task<List<BlogPost>> SearchPostsAsync(string? query, IEnumerable<string>? tags, DateTime? dateFrom, DateTime? dateTo, string sortOrder)
        {
            var q = _dbContext.Articles.Where(a => !a.IsDraft).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLower();
                q = q.Where(a => a.Title.ToLower().Contains(lowerQuery) ||
                                 a.ShortDescription.ToLower().Contains(lowerQuery) ||
                                 a.Content.ToLower().Contains(lowerQuery));
            }

            // 3. Фильтр по датам
            if (dateFrom.HasValue)
                q = q.Where(a => a.CreatedAt >= dateFrom.Value);

            if (dateTo.HasValue)
                q = q.Where(a => a.CreatedAt <= dateTo.Value.AddDays(1).AddTicks(-1)); // Включая конец дня

            // 4. Сортировка
            if (sortOrder == "oldest")
                q = q.OrderBy(a => a.CreatedAt);
            else
                q = q.OrderByDescending(a => a.CreatedAt);

            // Выполняем запрос к базе
            var results = await q.ToListAsync();

            // 5. Фильтр по тегам (делаем в памяти, так как SQLite иногда капризничает со списками JSON)
            if (tags != null && tags.Any())
            {
                results = results.Where(a => a.Tags != null && a.Tags.Any(t => tags.Contains(t))).ToList();
            }

            return results;
        }
        public async Task<List<BlogPost>> GetPublishedPostsAsync()
        {
            return await _dbContext.Articles
                .AsNoTracking() // МАГИЯ ОПТИМИЗАЦИИ: отключаем слежение
                .Where(p => !p.IsDraft)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetRecentPublishedPostsAsync(int count)
        {
            return await _dbContext.Articles
                .AsNoTracking() // МАГИЯ ОПТИМИЗАЦИИ
                .Where(p => !p.IsDraft)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetPostByIdAsync(int id)
        {
            return await _dbContext.Articles
                .Include(a => a.Quiz)           // ОБЯЗАТЕЛЬНО: Подгружаем тест
                .Include(a => a.FlashcardSet)    // ОБЯЗАТЕЛЬНО: Подгружаем карточки
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}