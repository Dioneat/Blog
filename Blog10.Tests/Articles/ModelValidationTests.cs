using System.ComponentModel.DataAnnotations;
using Blog10.Models;
using Xunit;

namespace Blog10.Tests.Articles;

public sealed class ModelValidationTests
{
    [Fact]
    public void BlogPost_WithoutTitle_IsInvalid()
    {
        var post = new BlogPost
        {
            Title = "",
            Content = "Текст статьи"
        };

        var results = ValidateModel(post);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(BlogPost.Title)));
    }

    [Fact]
    public void Review_WithoutAuthorName_IsInvalid()
    {
        var review = new Review
        {
            AuthorName = "",
            Text = "Текст отзыва"
        };

        var results = ValidateModel(review);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(Review.AuthorName)));
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(
            model,
            context,
            results,
            validateAllProperties: true);

        return results;
    }
}