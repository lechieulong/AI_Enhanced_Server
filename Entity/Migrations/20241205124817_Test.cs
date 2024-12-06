using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseReport_AspNetUsers_UserId",
                table: "CourseReport");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseReport_Courses_CourseId",
                table: "CourseReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseReport",
                table: "CourseReport");

            migrationBuilder.RenameTable(
                name: "CourseReport",
                newName: "CourseReports");

            migrationBuilder.RenameIndex(
                name: "IX_CourseReport_UserId",
                table: "CourseReports",
                newName: "IX_CourseReports_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseReport_CourseId",
                table: "CourseReports",
                newName: "IX_CourseReports_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseReports",
                table: "CourseReports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseReports_AspNetUsers_UserId",
                table: "CourseReports",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseReports_Courses_CourseId",
                table: "CourseReports",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseReports_AspNetUsers_UserId",
                table: "CourseReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseReports_Courses_CourseId",
                table: "CourseReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseReports",
                table: "CourseReports");

            migrationBuilder.RenameTable(
                name: "CourseReports",
                newName: "CourseReport");

            migrationBuilder.RenameIndex(
                name: "IX_CourseReports_UserId",
                table: "CourseReport",
                newName: "IX_CourseReport_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseReports_CourseId",
                table: "CourseReport",
                newName: "IX_CourseReport_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseReport",
                table: "CourseReport",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseReport_AspNetUsers_UserId",
                table: "CourseReport",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseReport_Courses_CourseId",
                table: "CourseReport",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
