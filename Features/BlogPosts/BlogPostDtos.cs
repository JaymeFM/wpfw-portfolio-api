namespace PortfolioApi.Features.BlogPosts;

public record BlogPostDto(int Id, string Title, string Content, DateTime PublishedAt);

public record CreateBlogPostDto(string Title, string Content, DateTime PublishedAt);

public record UpdateBlogPostDto(string Title, string Content, DateTime PublishedAt);
