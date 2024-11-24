using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class addAttempNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SectionCourseId",
                table: "TestExams",
                newName: "LessonId");

            migrationBuilder.AddColumn<int>(
                name: "AttemptNumber",
                table: "UserAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttemptNumber",
                table: "TestResult",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "TestExams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptNumber",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "AttemptNumber",
                table: "TestResult");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "TestExams");

            migrationBuilder.RenameColumn(
                name: "LessonId",
                table: "TestExams",
                newName: "SectionCourseId");
        }
    }
}
