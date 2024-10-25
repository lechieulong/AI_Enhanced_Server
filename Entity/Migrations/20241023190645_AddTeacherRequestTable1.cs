using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherRequestTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherRequest_AspNetUsers_UserId",
                table: "TeacherRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherRequest",
                table: "TeacherRequest");

            migrationBuilder.RenameTable(
                name: "TeacherRequest",
                newName: "TeacherRequests");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherRequest_UserId",
                table: "TeacherRequests",
                newName: "IX_TeacherRequests_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherRequests",
                table: "TeacherRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherRequests_AspNetUsers_UserId",
                table: "TeacherRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherRequests_AspNetUsers_UserId",
                table: "TeacherRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherRequests",
                table: "TeacherRequests");

            migrationBuilder.RenameTable(
                name: "TeacherRequests",
                newName: "TeacherRequest");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherRequests_UserId",
                table: "TeacherRequest",
                newName: "IX_TeacherRequest_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherRequest",
                table: "TeacherRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherRequest_AspNetUsers_UserId",
                table: "TeacherRequest",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
