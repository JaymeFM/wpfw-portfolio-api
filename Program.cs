using PortfolioApi.Features.BlogPosts;
using PortfolioApi.Features.Projects;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapProjectEndpoints();
app.MapBlogPostEndpoints();

app.Run();
