using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class updateTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_TestExams_TestId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_TestId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Answers");

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "TestExams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TestExamId",
                table: "Skills",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentText",
                table: "Parts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Parts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AnswerText",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "TypeCorrect",
                table: "Answers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_TestExamId",
                table: "Skills",
                column: "TestExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_TestExams_TestExamId",
                table: "Skills",
                column: "TestExamId",
                principalTable: "TestExams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_TestExams_TestExamId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_TestExamId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "TestExams");

            migrationBuilder.DropColumn(
                name: "TestExamId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "TypeCorrect",
                table: "Answers");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContentText",
                table: "Parts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AnswerText",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsCorrect",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_TestId",
                table: "Skills",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_TestExams_TestId",
                table: "Skills",
                column: "TestId",
                principalTable: "TestExams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
