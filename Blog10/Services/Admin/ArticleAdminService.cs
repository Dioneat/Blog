using Blog10.Data;
using Blog10.Models;
using Blog10.Services.Interfaces;
using Blog10.Utils;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Services.Admin
{
    public interface IArticleAdminService
    {
        Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default);
        Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        void ReplaceCover(BlogPost post, string newUrl);
        void ReplaceAudio(BlogPost post, string newUrl);
        void ReplaceVideo(BlogPost post, string newUrl);
        void ReplaceDocument(BlogPost post, string newUrl);
    }

    public sealed class ArticleAdminService : IArticleAdminService
    {
        private readonly AppDbContext _dbContext;
        private readonly IFileService _fileService;

        public ArticleAdminService(AppDbContext dbContext, IFileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public async Task<BlogPost> CreateAsync(
            BlogPost post,
            CancellationToken cancellationToken = default)
        {
            NormalizePost(post);

            _dbContext.Articles.Add(post);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _fileService.RunGarbageCollectorInBackground();

            return post;
        }

        public async Task<BlogPost> UpdateAsync(
            BlogPost post,
            CancellationToken cancellationToken = default)
        {
            NormalizePost(post);

            _dbContext.Articles.Update(post);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _fileService.RunGarbageCollectorInBackground();

            return post;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var post = await _dbContext.Articles
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (post == null)
                return;

            _fileService.DeleteFile(post.CoverImageUrl);
            _fileService.DeleteFile(post.MainAudioUrl);
            _fileService.DeleteFile(post.MainVideoUrl);
            _fileService.DeleteFile(post.FileAttachmentUrl);

            _dbContext.Articles.Remove(post);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _fileService.RunGarbageCollectorInBackground();
        }

        public void ReplaceCover(BlogPost post, string newUrl)
        {
            _fileService.DeleteFile(post.CoverImageUrl);
            post.CoverImageUrl = newUrl;
        }

        public void ReplaceAudio(BlogPost post, string newUrl)
        {
            _fileService.DeleteFile(post.MainAudioUrl);
            post.MainAudioUrl = newUrl;
        }

        public void ReplaceVideo(BlogPost post, string newUrl)
        {
            _fileService.DeleteFile(post.MainVideoUrl);
            post.MainVideoUrl = newUrl;
        }

        public void ReplaceDocument(BlogPost post, string newUrl)
        {
            _fileService.DeleteFile(post.FileAttachmentUrl);
            post.FileAttachmentUrl = newUrl;
        }

        private static void NormalizePost(BlogPost post)
        {
            post.Title = post.Title.Trim();

            if (string.IsNullOrWhiteSpace(post.Slug))
                post.Slug = SlugGenerator.Generate(post.Title);
        }
    }
}