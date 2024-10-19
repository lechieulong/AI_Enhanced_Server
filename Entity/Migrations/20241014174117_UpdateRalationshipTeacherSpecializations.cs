using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRalationshipTeacherSpecializations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "UserEducations");

            migrationBuilder.AddColumn<bool>(
                name: "IsReject",
                table: "UserEducations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEducationSpecializations",
                columns: table => new
                {
                    SpecializationsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserEducationsTeacherId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEducationSpecializations", x => new { x.SpecializationsId, x.UserEducationsTeacherId });
                    table.ForeignKey(
                        name: "FK_UserEducationSpecializations_Specializations_SpecializationsId",
                        column: x => x.SpecializationsId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEducationSpecializations_UserEducations_UserEducationsTeacherId",
                        column: x => x.UserEducationsTeacherId,
                        principalTable: "UserEducations",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("232d02b9-d8b3-4814-ada5-5db0ca3b14d4"), "Speaking" },
                    { new Guid("6e6f4b59-1063-4b8d-8f93-6506450c9481"), "Listening" },
                    { new Guid("8375c110-3031-40d1-9f23-1110bb77fb6f"), "Reading" },
                    { new Guid("b4c3d959-fdcc-4eef-93e3-4b20f4efc26c"), "Writing" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEducationSpecializations_UserEducationsTeacherId",
                table: "UserEducationSpecializations",
                column: "UserEducationsTeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEducationSpecializations");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropColumn(
                name: "IsReject",
                table: "UserEducations");

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "UserEducations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
