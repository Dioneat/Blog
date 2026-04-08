using Blog10.Models;

namespace Blog10.Services
{
    public interface IBlogService
    {
        Task<List<BlogPost>> GetPublishedPostsAsync();

        Task<List<BlogPost>> GetRecentPublishedPostsAsync(int count);

        Task<BlogPost?> GetPostByIdAsync(int id);
        Task<List<BlogPost>> SearchPostsAsync(string? query, IEnumerable<string>? tags, DateTime? dateFrom, DateTime? dateTo, string sortOrder);
        Task<List<string>> GetAllTagsAsync();
    }
}