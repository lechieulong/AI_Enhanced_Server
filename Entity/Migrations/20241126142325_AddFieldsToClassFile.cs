using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    public partial class AddFieldsToClassFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột AttemptNumber vào UserAnswers
            migrationBuilder.Sql(
                "IF COL_LENGTH('UserAnswers', 'AttemptNumber') IS NULL " +
                "BEGIN ALTER TABLE UserAnswers ADD AttemptNumber INT NOT NULL DEFAULT 0 END");

            // Thêm cột AttemptNumber vào TestResult
            migrationBuilder.Sql(
                "IF COL_LENGTH('TestResult', 'AttemptNumber') IS NULL " +
                "BEGIN ALTER TABLE TestResult ADD AttemptNumber INT NOT NULL DEFAULT 0 END");

            // Tạo bảng ClassFiles
            migrationBuilder.CreateTable(
                name: "ClassFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassFiles_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Tạo index cho ClassFiles
            migrationBuilder.CreateIndex(
                name: "IX_ClassFiles_ClassId",
                table: "ClassFiles",
                column: "ClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa bảng ClassFiles
            migrationBuilder.DropTable(name: "ClassFiles");

            // Xóa cột AttemptNumber khỏi UserAnswers
            migrationBuilder.Sql(
                "IF COL_LENGTH('UserAnswers', 'AttemptNumber') IS NOT NULL " +
                "BEGIN ALTER TABLE UserAnswers DROP COLUMN AttemptNumber END");

            // Xóa cột AttemptNumber khỏi TestResult
            migrationBuilder.Sql(
                "IF COL_LENGTH('TestResult', 'AttemptNumber') IS NOT NULL " +
                "BEGIN ALTER TABLE TestResult DROP COLUMN AttemptNumber END");
        }
    }
}
