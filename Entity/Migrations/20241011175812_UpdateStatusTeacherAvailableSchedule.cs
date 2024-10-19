using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusTeacherAvailableSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "TeacherAvailableSchedules");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TeacherAvailableSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TeacherAvailableSchedules");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "TeacherAvailableSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
