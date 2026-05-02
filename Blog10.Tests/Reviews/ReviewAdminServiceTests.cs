using Blog10.Models;
using Blog10.Services.Admin;
using Blog10.Tests.Fakes;
using Blog10.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Blog10.Tests.Reviews;

public sealed class ReviewAdminServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidReview_SavesReview()
    {
        using var database = new SqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var fileService = new FakeFileService();
        var reviewService = new ReviewAdminService(dbContext, fileService);

        var review = new Review
        {
            AuthorName = " Анна ",
            ChildInfo = "Мама Оли, 6 лет",
            Text = "Спасибо за занятия!",
            ImageUrl = "/uploads/review.jpg",
            IsDraft = false
        };

        await reviewService.CreateAsync(review);

        var savedReview = await dbContext.Reviews.FirstAsync();

        Assert.Equal("Анна", savedReview.AuthorName);
        Assert.Equal("Мама Оли, 6 лет", savedReview.ChildInfo);
        Assert.Equal("Спасибо за занятия!", savedReview.Text);
        Assert.Equal("/uploads/review.jpg", savedReview.ImageUrl);
        Assert.False(savedReview.IsDraft);
        Assert.Equal(1, fileService.GarbageCollectorRunCount);
    }

    [Fact]
    public void ReplaceImage_WhenOldImageExists_DeletesOldImageAndSetsNewUrl()
    {
        using var database = new SqliteTestDatabase();
        using var dbContext = database.CreateDbContext();

        var fileService = new FakeFileService();
        var reviewService = new ReviewAdminService(dbContext, fileService);

        var review = new Review
        {
            AuthorName = "Мария",
            ImageUrl = "/uploads/old-review.jpg"
        };

        reviewService.ReplaceImage(review, "/uploads/new-review.jpg");

        Assert.Equal("/uploads/new-review.jpg", review.ImageUrl);
        Assert.Contains("/uploads/old-review.jpg", fileService.DeletedFiles);
    }
}