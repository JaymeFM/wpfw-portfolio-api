namespace PortfolioApi.Features.Projects;

public static class ProjectEndpoints
{
    private static readonly List<Project> Projects =
    [
        new Project
        {
            Id = 1,
            Title = "PFA Configurator",
            Description = "3D configurator voor PerfectFit Automotive die ik bij Moogue van de grond af heb opgebouwd. Gemaakt met Vue3, Three.js, GraphQL en express.js.",
            Category = "Werk",
            Date = new DateOnly(2023, 9, 1)
        },
        new Project
        {
            Id = 2,
            Title = "Motorush",
            Description = "Race/freeroam motorspel op Roblox dat ik samen met wat maten heb gemaakt. Het spel heeft al bijna 100 miljoen speelsessies gehad.",
            Category = "Hobby",
            Date = new DateOnly(2023, 1, 1)
        }
    ];

    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        group.MapGet("/", () => Results.Ok(Projects));

        group.MapGet("/{id}", (int id) =>
        {
            var project = Projects.FirstOrDefault(p => p.Id == id);
            return project is null ? Results.NotFound() : Results.Ok(project);
        });
    }
}
