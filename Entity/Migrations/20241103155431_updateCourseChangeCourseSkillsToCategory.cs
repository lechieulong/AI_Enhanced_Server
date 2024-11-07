using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class updateCourseChangeCourseSkillsToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseParts_CourseSkills_CourseSkillId",
                table: "CourseParts");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSkills_Courses_CourseId",
                table: "CourseSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Skills_SkillId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_TestExams_TestExamId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skills",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseSkills",
                table: "CourseSkills");

            migrationBuilder.RenameTable(
                name: "Skills",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "CourseSkills",
                newName: "CourseCategories");

            migrationBuilder.RenameColumn(
                name: "Skills",
                table: "Courses",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_TestExamId",
                table: "Categories",
                newName: "IX_Categories_TestExamId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseSkills_CourseId",
                table: "CourseCategories",
                newName: "IX_CourseCategories_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseCategories",
                table: "CourseCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_TestExams_TestExamId",
                table: "Categories",
                column: "TestExamId",
                principalTable: "TestExams",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Categories_SkillId",
                table: "Parts",
                column: "SkillId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_TestExams_TestExamId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseCategories_Courses_CourseId",
                table: "CourseCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseParts_CourseCategories_CourseSkillId",
                table: "CourseParts");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Categories_SkillId",
                table: "Parts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseCategories",
                table: "CourseCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "CourseCategories",
                newName: "CourseSkills");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Skills");

            migrationBuilder.RenameColumn(
                name: "Categories",
                table: "Courses",
                newName: "Skills");

            migrationBuilder.RenameIndex(
                name: "IX_CourseCategories_CourseId",
                table: "CourseSkills",
                newName: "IX_CourseSkills_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_TestExamId",
                table: "Skills",
                newName: "IX_Skills_TestExamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseSkills",
                table: "CourseSkills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skills",
                table: "Skills",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Skills_SkillId",
                table: "Parts",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_TestExams_TestExamId",
                table: "Skills",
                column: "TestExamId",
                principalTable: "TestExams",
                principalColumn: "Id");
        }
    }
}
