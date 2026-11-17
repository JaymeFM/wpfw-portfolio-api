using System.Net;
using System.Net.Http.Json;
using PortfolioApi.Features.BlogPosts;

namespace PortfolioApi.Tests;

public class BlogPostTests(PortfolioApiFactory factory) : IClassFixture<PortfolioApiFactory>
{
    private static CreateBlogPostDto NewBlogPost() =>
        new("Testpost", "Een blogpost uit de tests", new DateTime(2026, 11, 17, 12, 0, 0));

    private static UpdateBlogPostDto ChangedBlogPost() =>
        new("Aangepaste post", "Aangepaste inhoud", new DateTime(2026, 11, 18, 9, 30, 0));

    private async Task<BlogPostDto> CreateBlogPostAsync()
    {
        var editor = await factory.CreateLoggedInClientAsync("Editor");
        var response = await editor.PostAsJsonAsync("/api/blogposts", NewBlogPost());
        return (await response.Content.ReadFromJsonAsync<BlogPostDto>())!;
    }

    private async Task<int> CountBlogPostsAsync()
    {
        var blogPosts = await factory.CreateClient().GetFromJsonAsync<List<BlogPostDto>>("/api/blogposts");
        return blogPosts!.Count;
    }

    [Fact]
    public async Task GetAll_WithoutLogin_Returns200AndBlogPosts()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var blogPosts = await response.Content.ReadFromJsonAsync<List<BlogPostDto>>();
        Assert.Contains(blogPosts!, b => b.Title == "Koekjes");
        Assert.Contains(blogPosts!, b => b.Title == "Bakken");
        Assert.Contains(blogPosts!, b => b.Title == "Nieuw vestje");
    }

    [Fact]
    public async Task GetById_WithExistingId_Returns200AndBlogPost()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogposts/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var blogPost = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        Assert.Equal(1, blogPost!.Id);
        Assert.Equal("Koekjes", blogPost.Title);
        Assert.Equal(new DateTime(2026, 9, 18, 20, 0, 0), blogPost.PublishedAt);
    }

    [Fact]
    public async Task GetById_WithUnknownId_Returns404()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogposts/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Create_AsEditorOrAdmin_Returns201AndSavesBlogPost(string role)
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync(role);
        var dto = NewBlogPost();

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        Assert.Equal(dto.Title, created!.Title);
        Assert.Equal(dto.Content, created.Content);
        Assert.Equal(dto.PublishedAt, created.PublishedAt);
        var saved = await client.GetFromJsonAsync<BlogPostDto>($"/api/blogposts/{created.Id}");
        Assert.Equal(created, saved);
    }

    [Fact]
    public async Task Create_WithoutLogin_Returns401AndSavesNothing()
    {
        // Arrange
        var client = factory.CreateClient();
        var countBefore = await CountBlogPostsAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", NewBlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(countBefore, await CountBlogPostsAsync());
    }

    [Fact]
    public async Task Create_AsGast_Returns403AndSavesNothing()
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync("Gast");
        var countBefore = await CountBlogPostsAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", NewBlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(countBefore, await CountBlogPostsAsync());
    }

    [Fact]
    public async Task Create_WithEmptyTitle_Returns400AndSavesNothing()
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync("Editor");
        var countBefore = await CountBlogPostsAsync();
        var dto = NewBlogPost() with { Title = "" };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(countBefore, await CountBlogPostsAsync());
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Update_AsEditorOrAdmin_Returns200AndChangesBlogPost(string role)
    {
        // Arrange
        var blogPost = await CreateBlogPostAsync();
        var client = await factory.CreateLoggedInClientAsync(role);
        var dto = ChangedBlogPost();

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{blogPost.Id}", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        Assert.Equal(blogPost.Id, updated!.Id);
        Assert.Equal(dto.Title, updated.Title);
        Assert.Equal(dto.Content, updated.Content);
        Assert.Equal(dto.PublishedAt, updated.PublishedAt);
        var saved = await client.GetFromJsonAsync<BlogPostDto>($"/api/blogposts/{blogPost.Id}");
        Assert.Equal(updated, saved);
    }

    [Fact]
    public async Task Update_WithUnknownId_Returns404()
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync("Editor");

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/99999", ChangedBlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithEmptyContent_Returns400AndChangesNothing()
    {
        // Arrange
        var blogPost = await CreateBlogPostAsync();
        var client = await factory.CreateLoggedInClientAsync("Editor");
        var dto = ChangedBlogPost() with { Content = "" };

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{blogPost.Id}", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var saved = await client.GetFromJsonAsync<BlogPostDto>($"/api/blogposts/{blogPost.Id}");
        Assert.Equal(blogPost, saved);
    }

    [Fact]
    public async Task Update_WithoutLogin_Returns401AndChangesNothing()
    {
        // Arrange
        var blogPost = await CreateBlogPostAsync();
        var client = factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{blogPost.Id}", ChangedBlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var saved = await client.GetFromJsonAsync<BlogPostDto>($"/api/blogposts/{blogPost.Id}");
        Assert.Equal(blogPost, saved);
    }

    [Fact]
    public async Task Update_AsGast_Returns403AndChangesNothing()
    {
        // Arrange
        var blogPost = await CreateBlogPostAsync();
        var client = await factory.CreateLoggedInClientAsync("Gast");

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{blogPost.Id}", ChangedBlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var saved = await client.GetFromJsonAsync<BlogPostDto>($"/api/blogposts/{blogPost.Id}");
        Assert.Equal(blogPost, saved);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Delete_AsEditorOrAdmin_Returns204AndRemovesBlogPost(string role)
    {
        // Arrange
        var blogPost = await CreateBlogPostAsync();
        var client = await factory.CreateLoggedInClientAsync(role);

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{blogPost.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var getResponse = await client.GetAsync($"/api/blogposts/{blogPost.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithUnknownId_Returns404()
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync("Editor");

        // Act
        var response = await client.DeleteAsync("/api/blogposts/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithoutLogin_Returns401AndKeepsBlogPost()
    {
        // Arrange
        var blogPost = await CreateBlogPostAsync();
        var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{blogPost.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var getResponse = await client.GetAsync($"/api/blogposts/{blogPost.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_AsGast_Returns403AndKeepsBlogPost()
    {
        // Arrange
        var blogPost = await CreateBlogPostAsync();
        var client = await factory.CreateLoggedInClientAsync("Gast");

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{blogPost.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var getResponse = await client.GetAsync($"/api/blogposts/{blogPost.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }
}
