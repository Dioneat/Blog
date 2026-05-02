using Blog10.Data;
using Blog10.Models;
using Blog10.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Services.Admin
{
    public interface IReviewAdminService
    {
        Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default);
        Task<Review> UpdateAsync(Review review, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        void ReplaceImage(Review review, string newUrl);
    }

    public sealed class ReviewAdminService : IReviewAdminService
    {
        private readonly AppDbContext _dbContext;
        private readonly IFileService _fileService;

        public ReviewAdminService(AppDbContext dbContext, IFileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public async Task<Review> CreateAsync(
            Review review,
            CancellationToken cancellationToken = default)
        {
            review.AuthorName = review.AuthorName.Trim();

            _dbContext.Reviews.Add(review);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _fileService.RunGarbageCollectorInBackground();

            return review;
        }

        public async Task<Review> UpdateAsync(
            Review review,
            CancellationToken cancellationToken = default)
        {
            review.AuthorName = review.AuthorName.Trim();

            _dbContext.Reviews.Update(review);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _fileService.RunGarbageCollectorInBackground();

            return review;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var review = await _dbContext.Reviews
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (review == null)
                return;

            _fileService.DeleteFile(review.ImageUrl);

            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _fileService.RunGarbageCollectorInBackground();
        }

        public void ReplaceImage(Review review, string newUrl)
        {
            _fileService.DeleteFile(review.ImageUrl);
            review.ImageUrl = newUrl;
        }
    }
}