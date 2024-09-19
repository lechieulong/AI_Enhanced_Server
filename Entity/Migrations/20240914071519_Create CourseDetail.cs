using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class CreateCourseDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CouurseName",
                table: "Courses",
                newName: "CourseName");

            migrationBuilder.CreateTable(
                name: "CourseTimelines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTimelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTimelines_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseTimelineDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimelineId = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTimelineDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTimelineDetails_CourseTimelines_TimelineId",
                        column: x => x.TimelineId,
                        principalTable: "CourseTimelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTimelineDetails_TimelineId",
                table: "CourseTimelineDetails",
                column: "TimelineId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTimelines_CourseId",
                table: "CourseTimelines",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseTimelineDetails");

            migrationBuilder.DropTable(
                name: "CourseTimelines");

            migrationBuilder.RenameColumn(
                name: "CourseName",
                table: "Courses",
                newName: "CouurseName");
        }
    }
}
