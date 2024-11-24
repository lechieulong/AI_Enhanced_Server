using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddAverageCourseRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestExams_SectionCourseId",
                table: "TestExams",
                column: "SectionCourseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestExams_CourseParts_SectionCourseId",
                table: "TestExams",
                column: "SectionCourseId",
                principalTable: "CourseParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestExams_CourseParts_SectionCourseId",
                table: "TestExams");

            migrationBuilder.DropIndex(
                name: "IX_TestExams_SectionCourseId",
                table: "TestExams");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Courses");
        }
    }
}
