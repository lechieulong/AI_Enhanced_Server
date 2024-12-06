using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseReportTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinalTestRelationship");

            migrationBuilder.CreateTable(
                name: "CourseReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseReport_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseReport_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseReport_CourseId",
                table: "CourseReport",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReport_UserId",
                table: "CourseReport",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseReport");

            migrationBuilder.CreateTable(
                name: "FinalTestRelationship",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillIdCourse = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalTestRelationship", x => x.Id);
                });
        }
    }
}
