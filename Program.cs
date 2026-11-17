using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Features.Auth;
using PortfolioApi.Features.BlogPosts;
using PortfolioApi.Features.Projects;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PortfolioDb")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<PortfolioDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditorOfAdmin", policy => policy.RequireRole("Editor", "Admin"));
});

builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogPostService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapProjectEndpoints();
app.MapBlogPostEndpoints();

app.Run();
