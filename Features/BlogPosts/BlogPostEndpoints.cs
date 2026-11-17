namespace PortfolioApi.Features.BlogPosts;

public static class BlogPostEndpoints
{
    public static void MapBlogPostEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/blogposts").WithTags("BlogPosts");

        group.MapGet("/", async (BlogPostService service) =>
            Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id}", async (int id, BlogPostService service) =>
        {
            var blogPost = await service.GetByIdAsync(id);
            return blogPost is null ? Results.NotFound() : Results.Ok(blogPost);
        });

        group.MapPost("/", async (CreateBlogPostDto dto, BlogPostService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
            {
                return Results.BadRequest("Title en Content zijn verplicht.");
            }

            var blogPost = await service.CreateAsync(dto);
            return Results.Created($"/api/blogposts/{blogPost.Id}", blogPost);
        }).RequireAuthorization("EditorOfAdmin");

        group.MapPut("/{id}", async (int id, UpdateBlogPostDto dto, BlogPostService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
            {
                return Results.BadRequest("Title en Content zijn verplicht.");
            }

            var blogPost = await service.UpdateAsync(id, dto);
            return blogPost is null ? Results.NotFound() : Results.Ok(blogPost);
        }).RequireAuthorization("EditorOfAdmin");

        group.MapDelete("/{id}", async (int id, BlogPostService service) =>
            await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound())
            .RequireAuthorization("EditorOfAdmin");
    }
}
