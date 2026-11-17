using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.Features.Auth;

namespace PortfolioApi.Tests;

public class AuthTests(PortfolioApiFactory factory) : IClassFixture<PortfolioApiFactory>
{
    [Fact]
    public async Task Register_WithValidData_Returns200AndGivesOnlyRoleGast()
    {
        // Arrange
        var client = factory.CreateClient();
        var dto = new RegisterDto($"{Guid.NewGuid()}@test.nl", "Test1!" + Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.FindByEmailAsync(dto.Email);
        Assert.NotNull(user);
        var role = Assert.Single(await userManager.GetRolesAsync(user));
        Assert.Equal("Gast", role);
    }

    [Fact]
    public async Task Register_WithWeakPassword_Returns400AndCreatesNoUser()
    {
        // Arrange
        var client = factory.CreateClient();
        var dto = new RegisterDto($"{Guid.NewGuid()}@test.nl", "abc");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        Assert.Null(await userManager.FindByEmailAsync(dto.Email));
    }

    [Fact]
    public async Task Register_WithExistingEmail_Returns400()
    {
        // Arrange
        var client = factory.CreateClient();
        var dto = new RegisterDto($"{Guid.NewGuid()}@test.nl", "Test1!" + Guid.NewGuid());
        await client.PostAsJsonAsync("/api/auth/register", dto);

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithCorrectPassword_Returns200AndCookie()
    {
        // Arrange
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.nl";
        var password = "Test1!" + Guid.NewGuid();
        await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(email, password));

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto(email, password));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(response.Headers.GetValues("Set-Cookie"), cookie => cookie.StartsWith(".AspNetCore.Identity.Application="));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        // Arrange
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.nl";
        await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(email, "Test1!" + Guid.NewGuid()));

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto(email, "Fout1!" + Guid.NewGuid()));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
