namespace PortfolioApi.Features.Projects;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string? GitHubUrl { get; set; }
    public DateOnly Date { get; set; }
}
