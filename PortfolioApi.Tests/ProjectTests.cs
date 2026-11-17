using System.Net;
using System.Net.Http.Json;
using PortfolioApi.Features.Projects;

namespace PortfolioApi.Tests;

public class ProjectTests(PortfolioApiFactory factory) : IClassFixture<PortfolioApiFactory>
{
    private static CreateProjectDto NewProject() =>
        new("Testproject", "Een project uit de tests", "Test", null, new DateOnly(2026, 11, 17));

    private static UpdateProjectDto ChangedProject() =>
        new("Aangepast project", "Een aangepaste beschrijving", "Werk", "https://github.com/test", new DateOnly(2025, 1, 1));

    private async Task<ProjectDto> CreateProjectAsync()
    {
        var editor = await factory.CreateLoggedInClientAsync("Editor");
        var response = await editor.PostAsJsonAsync("/api/projects", NewProject());
        return (await response.Content.ReadFromJsonAsync<ProjectDto>())!;
    }

    private async Task<int> CountProjectsAsync()
    {
        var projects = await factory.CreateClient().GetFromJsonAsync<List<ProjectDto>>("/api/projects");
        return projects!.Count;
    }

    [Fact]
    public async Task GetAll_WithoutLogin_Returns200AndProjects()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var projects = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
        Assert.Contains(projects!, p => p.Title == "PFA Configurator");
        Assert.Contains(projects!, p => p.Title == "Motorush");
    }

    [Fact]
    public async Task GetById_WithExistingId_Returns200AndProject()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/projects/2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.Equal(2, project!.Id);
        Assert.Equal("Motorush", project.Title);
        Assert.Equal("Hobby", project.Category);
    }

    [Fact]
    public async Task GetById_WithUnknownId_Returns404()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/projects/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Create_AsEditorOrAdmin_Returns201AndSavesProject(string role)
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync(role);
        var dto = NewProject();

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.Equal(dto.Title, created!.Title);
        Assert.Equal(dto.Description, created.Description);
        Assert.Equal(dto.Category, created.Category);
        Assert.Equal(dto.Date, created.Date);
        var saved = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{created.Id}");
        Assert.Equal(created, saved);
    }

    [Fact]
    public async Task Create_WithoutLogin_Returns401AndSavesNothing()
    {
        // Arrange
        var client = factory.CreateClient();
        var countBefore = await CountProjectsAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", NewProject());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(countBefore, await CountProjectsAsync());
    }

    [Fact]
    public async Task Create_AsGast_Returns403AndSavesNothing()
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync("Gast");
        var countBefore = await CountProjectsAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", NewProject());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(countBefore, await CountProjectsAsync());
    }

    [Fact]
    public async Task Create_WithEmptyTitle_Returns400AndSavesNothing()
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync("Editor");
        var countBefore = await CountProjectsAsync();
        var dto = NewProject() with { Title = "" };

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(countBefore, await CountProjectsAsync());
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Update_AsEditorOrAdmin_Returns200AndChangesProject(string role)
    {
        // Arrange
        var project = await CreateProjectAsync();
        var client = await factory.CreateLoggedInClientAsync(role);
        var dto = ChangedProject();

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.Equal(project.Id, updated!.Id);
        Assert.Equal(dto.Title, updated.Title);
        Assert.Equal(dto.Description, updated.Description);
        Assert.Equal(dto.Category, updated.Category);
        Assert.Equal(dto.GitHubUrl, updated.GitHubUrl);
        Assert.Equal(dto.Date, updated.Date);
        var saved = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{project.Id}");
        Assert.Equal(updated, saved);
    }

    [Fact]
    public async Task Update_WithUnknownId_Returns404()
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync("Editor");

        // Act
        var response = await client.PutAsJsonAsync("/api/projects/99999", ChangedProject());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithEmptyTitle_Returns400AndChangesNothing()
    {
        // Arrange
        var project = await CreateProjectAsync();
        var client = await factory.CreateLoggedInClientAsync("Editor");
        var dto = ChangedProject() with { Title = "" };

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var saved = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{project.Id}");
        Assert.Equal(project, saved);
    }

    [Fact]
    public async Task Update_WithoutLogin_Returns401AndChangesNothing()
    {
        // Arrange
        var project = await CreateProjectAsync();
        var client = factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", ChangedProject());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var saved = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{project.Id}");
        Assert.Equal(project, saved);
    }

    [Fact]
    public async Task Update_AsGast_Returns403AndChangesNothing()
    {
        // Arrange
        var project = await CreateProjectAsync();
        var client = await factory.CreateLoggedInClientAsync("Gast");

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", ChangedProject());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var saved = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{project.Id}");
        Assert.Equal(project, saved);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Delete_AsEditorOrAdmin_Returns204AndRemovesProject(string role)
    {
        // Arrange
        var project = await CreateProjectAsync();
        var client = await factory.CreateLoggedInClientAsync(role);

        // Act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var getResponse = await client.GetAsync($"/api/projects/{project.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithUnknownId_Returns404()
    {
        // Arrange
        var client = await factory.CreateLoggedInClientAsync("Editor");

        // Act
        var response = await client.DeleteAsync("/api/projects/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithoutLogin_Returns401AndKeepsProject()
    {
        // Arrange
        var project = await CreateProjectAsync();
        var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var getResponse = await client.GetAsync($"/api/projects/{project.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_AsGast_Returns403AndKeepsProject()
    {
        // Arrange
        var project = await CreateProjectAsync();
        var client = await factory.CreateLoggedInClientAsync("Gast");

        // Act
        var response = await client.DeleteAsync($"/api/projects/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var getResponse = await client.GetAsync($"/api/projects/{project.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }
}
