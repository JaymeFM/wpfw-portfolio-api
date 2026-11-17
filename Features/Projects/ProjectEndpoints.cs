namespace PortfolioApi.Features.Projects;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        group.MapGet("/", async (ProjectService service) =>
            Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id}", async (int id, ProjectService service) =>
        {
            var project = await service.GetByIdAsync(id);
            return project is null ? Results.NotFound() : Results.Ok(project);
        });

        group.MapPost("/", async (CreateProjectDto dto, ProjectService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description) || string.IsNullOrWhiteSpace(dto.Category))
            {
                return Results.BadRequest("Title, Description en Category zijn verplicht.");
            }

            var project = await service.CreateAsync(dto);
            return Results.Created($"/api/projects/{project.Id}", project);
        }).RequireAuthorization("EditorOfAdmin");

        group.MapPut("/{id}", async (int id, UpdateProjectDto dto, ProjectService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description) || string.IsNullOrWhiteSpace(dto.Category))
            {
                return Results.BadRequest("Title, Description en Category zijn verplicht.");
            }

            var project = await service.UpdateAsync(id, dto);
            return project is null ? Results.NotFound() : Results.Ok(project);
        }).RequireAuthorization("EditorOfAdmin");

        group.MapDelete("/{id}", async (int id, ProjectService service) =>
            await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound())
            .RequireAuthorization("EditorOfAdmin");
    }
}
