namespace PortfolioApi.Features.BlogPosts;

public static class BlogPostEndpoints
{
    private static readonly List<BlogPost> BlogPosts =
    [
        new BlogPost
        {
            Id = 1,
            Title = "Koekjes",
            Content = "Ik heb vandaag 1 miljoen miljard koekjes gebakken en ze waren zo enorm lekker.",
            PublishedAt = new DateTime(2026, 9, 18, 20, 0, 0)
        },
        new BlogPost
        {
            Id = 2,
            Title = "Bakken",
            Content = "Ik ben echt koekjes aan het bakken als een speer je zal het niet geloven hoeveel ik er aan het maken ben.",
            PublishedAt = new DateTime(2026, 9, 18, 19, 0, 0)
        },
        new BlogPost
        {
            Id = 3,
            Title = "Nieuw vestje",
            Content = "Ik heb een nieuw vestje gekocht van Rick Owens, ik vind hemzelf echt gaaf.",
            PublishedAt = new DateTime(2026, 9, 16, 13, 0, 0)
        }
    ];

    public static void MapBlogPostEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/blogposts").WithTags("BlogPosts");

        group.MapGet("/", () => Results.Ok(BlogPosts));

        group.MapGet("/{id}", (int id) =>
        {
            var blogPost = BlogPosts.FirstOrDefault(b => b.Id == id);
            return blogPost is null ? Results.NotFound() : Results.Ok(blogPost);
        });
    }
}
