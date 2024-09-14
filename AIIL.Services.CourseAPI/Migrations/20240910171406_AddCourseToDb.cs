using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AIIL.Services.CourseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    MentorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "CategoryId", "Description", "ImageUrl", "MentorId", "Price", "Title" },
                values: new object[,]
                {
                    { 1, 1, "A beginner's course for IELTS preparation.", "https://example.com/images/ielts-intro.jpg", 1, 99.99m, "Introduction to IELTS" },
                    { 2, 2, "An advanced course for students looking to master the IELTS exam.", "https://example.com/images/ielts-advanced.jpg", 2, 149.99m, "Advanced IELTS Techniques" },
                    { 3, 1, "Focused on helping students improve their writing skills for IELTS.", "https://example.com/images/ielts-writing.jpg", 1, 79.99m, "IELTS Writing Mastery" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
