using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Balance_Historys_AccountBalanceId",
                table: "Balance_Historys");

            migrationBuilder.CreateIndex(
                name: "IX_Balance_Historys_AccountBalanceId",
                table: "Balance_Historys",
                column: "AccountBalanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Balance_Historys_AccountBalanceId",
                table: "Balance_Historys");

            migrationBuilder.CreateIndex(
                name: "IX_Balance_Historys_AccountBalanceId",
                table: "Balance_Historys",
                column: "AccountBalanceId",
                unique: true);
        }
    }
}
