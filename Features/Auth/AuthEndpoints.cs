using Microsoft.AspNetCore.Identity;

namespace PortfolioApi.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterDto dto, UserManager<IdentityUser> userManager) =>
        {
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(user, "Gast");
            return Results.Ok();
        });

        group.MapPost("/login", async (LoginDto dto, SignInManager<IdentityUser> signInManager) =>
        {
            var result = await signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);
            return result.Succeeded ? Results.Ok() : Results.Unauthorized();
        });
    }
}
