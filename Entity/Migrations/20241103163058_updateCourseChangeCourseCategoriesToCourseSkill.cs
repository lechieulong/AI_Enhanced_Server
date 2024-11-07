using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class updateCourseChangeCourseCategoriesToCourseSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseCategories_Courses_CourseId",
                table: "CourseCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseParts_CourseCategories_CourseSkillId",
                table: "CourseParts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseCategories",
                table: "CourseCategories");

            migrationBuilder.RenameTable(
                name: "CourseCategories",
                newName: "CourseSkills");

            migrationBuilder.RenameIndex(
                name: "IX_CourseCategories_CourseId",
                table: "CourseSkills",
                newName: "IX_CourseSkills_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseSkills",
                table: "CourseSkills",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseParts_CourseSkills_CourseSkillId",
                table: "CourseParts",
                column: "CourseSkillId",
                principalTable: "CourseSkills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSkills_Courses_CourseId",
                table: "CourseSkills",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseParts_CourseSkills_CourseSkillId",
                table: "CourseParts");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSkills_Courses_CourseId",
                table: "CourseSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseSkills",
                table: "CourseSkills");

            migrationBuilder.RenameTable(
                name: "CourseSkills",
                newName: "CourseCategories");

            migrationBuilder.RenameIndex(
                name: "IX_CourseSkills_CourseId",
                table: "CourseCategories",
                newName: "IX_CourseCategories_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseCategories",
                table: "CourseCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCategories_Courses_CourseId",
                table: "CourseCategories",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseParts_CourseCategories_CourseSkillId",
                table: "CourseParts",
                column: "CourseSkillId",
                principalTable: "CourseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
