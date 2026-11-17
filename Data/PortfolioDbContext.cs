using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Features.BlogPosts;
using PortfolioApi.Features.Projects;

namespace PortfolioApi.Data;

public class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Gast", NormalizedName = "GAST", ConcurrencyStamp = "1" },
            new IdentityRole { Id = "2", Name = "Editor", NormalizedName = "EDITOR", ConcurrencyStamp = "2" },
            new IdentityRole { Id = "3", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "3" }
        );

        modelBuilder.Entity<Project>().HasData(
            new Project
            {
                Id = 1,
                Title = "PFA Configurator",
                Description = "3D configurator voor PerfectFit Automotive die ik bij Moogue van de grond af heb opgebouwd. Gemaakt met Vue3, Three.js, GraphQL en express.js.",
                Category = "Werk",
                Date = new DateOnly(2023, 9, 1)
            },
            new Project
            {
                Id = 2,
                Title = "Motorush",
                Description = "Race/freeroam motorspel op Roblox dat ik samen met wat maten heb gemaakt. Het spel heeft al bijna 100 miljoen speelsessies gehad.",
                Category = "Hobby",
                Date = new DateOnly(2023, 1, 1)
            }
        );

        modelBuilder.Entity<BlogPost>().HasData(
            new BlogPost
            {
                Id = 1,
                Title = "Koekjes",
                Content = "Ik heb vandaag 1 miljoen miljard koekjes gebakken en ze waren zo enorm lekker.",
                PublishedAt = new DateTime(2026, 9, 18, 20, 0, 0)
            },
            new BlogPost
            {
                Id = 2,
                Title = "Bakken",
                Content = "Ik ben echt koekjes aan het bakken als een speer je zal het niet geloven hoeveel ik er aan het maken ben.",
                PublishedAt = new DateTime(2026, 9, 18, 19, 0, 0)
            },
            new BlogPost
            {
                Id = 3,
                Title = "Nieuw vestje",
                Content = "Ik heb een nieuw vestje gekocht van Rick Owens, ik vind hemzelf echt gaaf.",
                PublishedAt = new DateTime(2026, 9, 16, 13, 0, 0)
            }
        );
    }
}
