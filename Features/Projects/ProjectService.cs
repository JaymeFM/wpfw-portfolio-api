using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;

namespace PortfolioApi.Features.Projects;

public class ProjectService(PortfolioDbContext db)
{
    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await db.Projects.ToListAsync();
        return projects.Select(ToDto).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await db.Projects.FindAsync(id);
        return project is null ? null : ToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            GitHubUrl = dto.GitHubUrl,
            Date = dto.Date
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return ToDto(project);
    }

    public async Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null)
        {
            return null;
        }

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.Category = dto.Category;
        project.GitHubUrl = dto.GitHubUrl;
        project.Date = dto.Date;

        await db.SaveChangesAsync();
        return ToDto(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null)
        {
            return false;
        }

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return true;
    }

    private static ProjectDto ToDto(Project project) =>
        new(project.Id, project.Title, project.Description, project.Category, project.GitHubUrl, project.Date);
}
