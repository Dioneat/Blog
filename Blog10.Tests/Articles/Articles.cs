using Blog10.Models;
using Blog10.Services.Admin;
using Blog10.Tests.Fakes;
using Blog10.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Blog10.Tests.Articles;

public sealed class ArticleAdminServiceTests
{
    [Fact]
    public void ReplaceCover_WhenOldCoverExists_DeletesOldFileAndSetsNewUrl()
    {
        using var database = new SqliteTestDatabase();
        using var dbContext = database.CreateDbContext();

        var fileService = new FakeFileService();
        var articleService = new ArticleAdminService(dbContext, fileService);

        var post = new BlogPost
        {
            Title = "Статья",
            CoverImageUrl = "/uploads/old-cover.jpg"
        };

        articleService.ReplaceCover(post, "/uploads/new-cover.jpg");

        Assert.Equal("/uploads/new-cover.jpg", post.CoverImageUrl);
        Assert.Contains("/uploads/old-cover.jpg", fileService.DeletedFiles);
    }
    [Fact]
    public async Task CreateAsync_ValidArticle_SavesArticleAndGeneratesSlug()
    {
        using var database = new SqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var fileService = new FakeFileService();
        var articleService = new ArticleAdminService(dbContext, fileService);

        var post = new BlogPost
        {
            Title = "Тестовая статья про развитие речи",
            ShortDescription = "Краткое описание статьи",
            Content = "<p>Основной текст статьи</p>",
            Tags = new List<string> { "Логопедия", "Дети" },
            IsDraft = false,
            CoverImageUrl = "/uploads/cover.jpg",
            MainAudioUrl = "/uploads/audio.mp3",
            MainVideoUrl = "/uploads/video.mp4",
            FileAttachmentUrl = "/uploads/file.pdf",
            FileAttachmentName = "Памятка для родителей"
        };

        await articleService.CreateAsync(post);

        var savedPost = await dbContext.Articles.FirstAsync();

        Assert.True(savedPost.Id > 0);
        Assert.Equal("Тестовая статья про развитие речи", savedPost.Title);
        Assert.Equal("Краткое описание статьи", savedPost.ShortDescription);
        Assert.Equal("<p>Основной текст статьи</p>", savedPost.Content);
        Assert.Equal(new List<string> { "Логопедия", "Дети" }, savedPost.Tags);
        Assert.False(savedPost.IsDraft);

        Assert.Equal("/uploads/cover.jpg", savedPost.CoverImageUrl);
        Assert.Equal("/uploads/audio.mp3", savedPost.MainAudioUrl);
        Assert.Equal("/uploads/video.mp4", savedPost.MainVideoUrl);
        Assert.Equal("/uploads/file.pdf", savedPost.FileAttachmentUrl);
        Assert.Equal("Памятка для родителей", savedPost.FileAttachmentName);

        Assert.False(string.IsNullOrWhiteSpace(savedPost.Slug));
        Assert.Equal(1, fileService.GarbageCollectorRunCount);
    }
}