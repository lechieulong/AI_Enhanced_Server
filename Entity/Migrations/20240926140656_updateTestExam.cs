using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class updateTestExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartSkill_SkillTestExam_SkillId",
                table: "PartSkill");

            migrationBuilder.DropTable(
                name: "AnswerMatching");

            migrationBuilder.DropTable(
                name: "AnswerOptions");

            migrationBuilder.DropTable(
                name: "SkillTestExam");

            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.RenameColumn(
                name: "SkillId",
                table: "PartSkill",
                newName: "TestId");

            migrationBuilder.RenameIndex(
                name: "IX_PartSkill_SkillId",
                table: "PartSkill",
                newName: "IX_PartSkill_TestId");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionType",
                table: "QuestionTypePart",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "TestExamId",
                table: "QuestionTypePart",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "Question",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AudioURL",
                table: "PartSkill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SkillTest",
                table: "PartSkill",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTypePart_TestExamId",
                table: "QuestionTypePart",
                column: "TestExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartSkill_TestExam_TestId",
                table: "PartSkill",
                column: "TestId",
                principalTable: "TestExam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTypePart_TestExam_TestExamId",
                table: "QuestionTypePart",
                column: "TestExamId",
                principalTable: "TestExam",
                principalColumn: "Id");
        }
    }
}
