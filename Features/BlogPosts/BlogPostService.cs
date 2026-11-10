using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;

namespace PortfolioApi.Features.BlogPosts;

public class BlogPostService(PortfolioDbContext db)
{
    public async Task<List<BlogPostDto>> GetAllAsync()
    {
        var blogPosts = await db.BlogPosts.ToListAsync();
        return blogPosts.Select(ToDto).ToList();
    }

    public async Task<BlogPostDto?> GetByIdAsync(int id)
    {
        var blogPost = await db.BlogPosts.FindAsync(id);
        return blogPost is null ? null : ToDto(blogPost);
    }

    public async Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto)
    {
        var blogPost = new BlogPost
        {
            Title = dto.Title,
            Content = dto.Content,
            PublishedAt = dto.PublishedAt
        };

        db.BlogPosts.Add(blogPost);
        await db.SaveChangesAsync();
        return ToDto(blogPost);
    }

    public async Task<BlogPostDto?> UpdateAsync(int id, UpdateBlogPostDto dto)
    {
        var blogPost = await db.BlogPosts.FindAsync(id);
        if (blogPost is null)
        {
            return null;
        }

        blogPost.Title = dto.Title;
        blogPost.Content = dto.Content;
        blogPost.PublishedAt = dto.PublishedAt;

        await db.SaveChangesAsync();
        return ToDto(blogPost);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var blogPost = await db.BlogPosts.FindAsync(id);
        if (blogPost is null)
        {
            return false;
        }

        db.BlogPosts.Remove(blogPost);
        await db.SaveChangesAsync();
        return true;
    }

    private static BlogPostDto ToDto(BlogPost blogPost) =>
        new(blogPost.Id, blogPost.Title, blogPost.Content, blogPost.PublishedAt);
}
