using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Features.BlogPosts;
using PortfolioApi.Features.Projects;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PortfolioDb")));

builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogPostService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapProjectEndpoints();
app.MapBlogPostEndpoints();

app.Run();
