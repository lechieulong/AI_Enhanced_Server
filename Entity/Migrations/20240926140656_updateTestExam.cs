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
            migrationBuilder.DropForeignKey(
                name: "FK_PartSkill_TestExam_TestId",
                table: "PartSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTypePart_TestExam_TestExamId",
                table: "QuestionTypePart");

            migrationBuilder.DropIndex(
                name: "IX_QuestionTypePart_TestExamId",
                table: "QuestionTypePart");

            migrationBuilder.DropColumn(
                name: "TestExamId",
                table: "QuestionTypePart");

            migrationBuilder.DropColumn(
                name: "Answer",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "AudioURL",
                table: "PartSkill");

            migrationBuilder.DropColumn(
                name: "SkillTest",
                table: "PartSkill");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Classes");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "PartSkill",
                newName: "SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_PartSkill_TestId",
                table: "PartSkill",
                newName: "IX_PartSkill_SkillId");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionType",
                table: "QuestionTypePart",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerFilling = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerTrueFalse = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answer_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillTestExam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillType = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTestExam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillTestExam_TestExam_TestId",
                        column: x => x.TestId,
                        principalTable: "TestExam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerMatching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Heading = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Matching = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerMatching", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerMatching_Answer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerOptions_Answer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_QuestionId",
                table: "Answer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerMatching_AnswerId",
                table: "AnswerMatching",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_AnswerId",
                table: "AnswerOptions",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillTestExam_TestId",
                table: "SkillTestExam",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartSkill_SkillTestExam_SkillId",
                table: "PartSkill",
                column: "SkillId",
                principalTable: "SkillTestExam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
