namespace PortfolioApi.Features.Projects;

public record ProjectDto(int Id, string Title, string Description, string Category, string? GitHubUrl, DateOnly Date);

public record CreateProjectDto(string Title, string Description, string Category, string? GitHubUrl, DateOnly Date);

public record UpdateProjectDto(string Title, string Description, string Category, string? GitHubUrl, DateOnly Date);
