using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class updateCoursePart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseParts_CourseSkills_SkillId",
                table: "CourseParts");

            migrationBuilder.DropColumn(
                name: "Audio",
                table: "CourseParts");

            migrationBuilder.DropColumn(
                name: "ContentText",
                table: "CourseParts");

            migrationBuilder.RenameColumn(
                name: "VideoUrl",
                table: "CourseParts",
                newName: "ContentUrl");

            migrationBuilder.RenameColumn(
                name: "SkillId",
                table: "CourseParts",
                newName: "CoursePartId");

            migrationBuilder.RenameColumn(
                name: "PartNumber",
                table: "CourseParts",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "CourseParts",
                newName: "ContentType");

            migrationBuilder.RenameIndex(
                name: "IX_CourseParts_SkillId",
                table: "CourseParts",
                newName: "IX_CourseParts_CoursePartId");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "CourseLessons",
                newName: "PartNumber");

            migrationBuilder.RenameColumn(
                name: "ContentUrl",
                table: "CourseLessons",
                newName: "VideoUrl");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "CourseLessons",
                newName: "Image");

            migrationBuilder.AddColumn<string>(
                name: "Audio",
                table: "CourseLessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentText",
                table: "CourseLessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseParts_CourseSkills_CoursePartId",
                table: "CourseParts",
                column: "CoursePartId",
                principalTable: "CourseSkills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseParts_CourseSkills_CoursePartId",
                table: "CourseParts");

            migrationBuilder.DropColumn(
                name: "Audio",
                table: "CourseLessons");

            migrationBuilder.DropColumn(
                name: "ContentText",
                table: "CourseLessons");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "CourseParts",
                newName: "PartNumber");

            migrationBuilder.RenameColumn(
                name: "CoursePartId",
                table: "CourseParts",
                newName: "SkillId");

            migrationBuilder.RenameColumn(
                name: "ContentUrl",
                table: "CourseParts",
                newName: "VideoUrl");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "CourseParts",
                newName: "Image");

            migrationBuilder.RenameIndex(
                name: "IX_CourseParts_CoursePartId",
                table: "CourseParts",
                newName: "IX_CourseParts_SkillId");

            migrationBuilder.RenameColumn(
                name: "VideoUrl",
                table: "CourseLessons",
                newName: "ContentUrl");

            migrationBuilder.RenameColumn(
                name: "PartNumber",
                table: "CourseLessons",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "CourseLessons",
                newName: "ContentType");

            migrationBuilder.AddColumn<string>(
                name: "Audio",
                table: "CourseParts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentText",
                table: "CourseParts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseParts_CourseSkills_SkillId",
                table: "CourseParts",
                column: "SkillId",
                principalTable: "CourseSkills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
