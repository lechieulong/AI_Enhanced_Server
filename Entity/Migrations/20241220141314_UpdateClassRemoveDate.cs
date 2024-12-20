using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassRemoveDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Classes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndDate",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartDate",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
