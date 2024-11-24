using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCoursePartConnectWithTextExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestExams_CourseParts_SectionCourseId",
                table: "TestExams");

            migrationBuilder.DropIndex(
                name: "IX_TestExams_SectionCourseId",
                table: "TestExams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
