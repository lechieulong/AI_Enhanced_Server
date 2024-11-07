using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class updateCourseChangeCoursePart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseParts_CourseSkills_CoursePartId",
                table: "CourseParts");

            migrationBuilder.RenameColumn(
                name: "CoursePartId",
                table: "CourseParts",
                newName: "CourseSkillId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseParts_CoursePartId",
                table: "CourseParts",
                newName: "IX_CourseParts_CourseSkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseParts_CourseSkills_CourseSkillId",
                table: "CourseParts",
                column: "CourseSkillId",
                principalTable: "CourseSkills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseParts_CourseSkills_CourseSkillId",
                table: "CourseParts");

            migrationBuilder.RenameColumn(
                name: "CourseSkillId",
                table: "CourseParts",
                newName: "CoursePartId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseParts_CourseSkillId",
                table: "CourseParts",
                newName: "IX_CourseParts_CoursePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseParts_CourseSkills_CoursePartId",
                table: "CourseParts",
                column: "CoursePartId",
                principalTable: "CourseSkills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
