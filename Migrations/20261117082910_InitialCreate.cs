using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PortfolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GitHubUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "Content", "PublishedAt", "Title" },
                values: new object[,]
                {
                    { 1, "Ik heb vandaag 1 miljoen miljard koekjes gebakken en ze waren zo enorm lekker.", new DateTime(2026, 9, 18, 20, 0, 0, 0, DateTimeKind.Unspecified), "Koekjes" },
                    { 2, "Ik ben echt koekjes aan het bakken als een speer je zal het niet geloven hoeveel ik er aan het maken ben.", new DateTime(2026, 9, 18, 19, 0, 0, 0, DateTimeKind.Unspecified), "Bakken" },
                    { 3, "Ik heb een nieuw vestje gekocht van Rick Owens, ik vind hemzelf echt gaaf.", new DateTime(2026, 9, 16, 13, 0, 0, 0, DateTimeKind.Unspecified), "Nieuw vestje" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Category", "Date", "Description", "GitHubUrl", "Title" },
                values: new object[,]
                {
                    { 1, "Werk", new DateOnly(2023, 9, 1), "3D configurator voor PerfectFit Automotive die ik bij Moogue van de grond af heb opgebouwd. Gemaakt met Vue3, Three.js, GraphQL en express.js.", null, "PFA Configurator" },
                    { 2, "Hobby", new DateOnly(2023, 1, 1), "Race/freeroam motorspel op Roblox dat ik samen met wat maten heb gemaakt. Het spel heeft al bijna 100 miljoen speelsessies gehad.", null, "Motorush" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
