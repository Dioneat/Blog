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
            var tagsList = await _dbContext.Articles
                .AsNoTracking()
                .Where(a => !a.IsDraft)
                .Select(a => a.Tags)
                .ToListAsync();

            return tagsList
                .Where(t => t != null)
                .SelectMany(t => t)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        public async Task<List<BlogPost>> SearchPostsAsync(string? query, IEnumerable<string>? tags, DateTime? dateFrom, DateTime? dateTo, string sortOrder)
        {
            var q = _dbContext.Articles.AsNoTracking().Where(a => !a.IsDraft).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var likeQuery = $"%{query}%";
                q = q.Where(a => EF.Functions.Like(a.Title, likeQuery) ||
                                 EF.Functions.Like(a.ShortDescription, likeQuery) ||
                                 EF.Functions.Like(a.Content, likeQuery));
            }

            if (dateFrom.HasValue)
                q = q.Where(a => a.CreatedAt >= dateFrom.Value);

            if (dateTo.HasValue)
                q = q.Where(a => a.CreatedAt <= dateTo.Value.AddDays(1).AddTicks(-1));

            if (sortOrder == "oldest")
                q = q.OrderBy(a => a.CreatedAt);
            else
                q = q.OrderByDescending(a => a.CreatedAt);

            var results = await q.ToListAsync();

            if (tags != null && tags.Any())
            {
                results = results.Where(a => a.Tags != null && a.Tags.Any(t => tags.Contains(t))).ToList();
            }

            return results;
        }

        public async Task<List<BlogPost>> GetPublishedPostsAsync()
        {
            return await _dbContext.Articles
                .AsNoTracking()
                .Where(p => !p.IsDraft)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new BlogPost
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    ShortDescription = p.ShortDescription,
                    CoverImageUrl = p.CoverImageUrl,
                    CreatedAt = p.CreatedAt,
                    Tags = p.Tags
                })
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetRecentPublishedPostsAsync(int count)
        {
            return await _dbContext.Articles
                .AsNoTracking()
                .Where(p => !p.IsDraft)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new BlogPost
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    ShortDescription = p.ShortDescription,
                    CoverImageUrl = p.CoverImageUrl,
                    CreatedAt = p.CreatedAt,
                    Tags = p.Tags
                })
                .Take(count)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetPostByIdAsync(int id)
        {
            return await _dbContext.Articles
                .AsNoTracking() 
                .Include(a => a.Quiz)
                .Include(a => a.FlashcardSet)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}