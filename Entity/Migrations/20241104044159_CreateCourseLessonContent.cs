using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class CreateCourseLessonContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audio",
                table: "CourseLessons");

            migrationBuilder.DropColumn(
                name: "ContentText",
                table: "CourseLessons");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "CourseLessons");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "CourseLessons");

            migrationBuilder.CreateTable(
                name: "CourseLessonContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseLessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseLessonContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseLessonContents_CourseLessons_CourseLessonId",
                        column: x => x.CourseLessonId,
                        principalTable: "CourseLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseLessonContents_CourseLessonId",
                table: "CourseLessonContents",
                column: "CourseLessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseLessonContents");

            migrationBuilder.AddColumn<string>(
                name: "Audio",
                table: "CourseLessons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentText",
                table: "CourseLessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "CourseLessons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "CourseLessons",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
