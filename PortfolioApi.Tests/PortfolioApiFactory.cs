using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortfolioApi.Data;
using PortfolioApi.Features.Auth;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PortfolioApi.Tests;

public class PortfolioApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:PortfolioDb",
            "Server=localhost\\SQLEXPRESS;Database=PortfolioDb_Tests;Trusted_Connection=True;TrustServerCertificate=True");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();

        return host;
    }

    public async Task<HttpClient> CreateLoggedInClientAsync(string role)
    {
        var client = CreateClient();
        var email = $"{Guid.NewGuid()}@test.nl";
        var password = "Test1!" + Guid.NewGuid();

        await client.PostAsJsonAsync("/api/auth/register", new RegisterDto(email, password));

        if (role != "Gast")
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);
            await userManager.AddToRoleAsync(user!, role);
        }

        await client.PostAsJsonAsync("/api/auth/login", new LoginDto(email, password));
        return client;
    }
}
