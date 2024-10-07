using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherAvailableScheduleTableBookedTeacherSessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeacherAvailableSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LearnerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAvailableSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAvailableSchedules_AspNetUsers_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherAvailableSchedules_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookedTeacherSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AvailableDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LearnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TeacherAvailableScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedTeacherSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookedTeacherSessions_AspNetUsers_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookedTeacherSessions_TeacherAvailableSchedules_TeacherAvailableScheduleId",
                        column: x => x.TeacherAvailableScheduleId,
                        principalTable: "TeacherAvailableSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookedTeacherSessions_LearnerId",
                table: "BookedTeacherSessions",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BookedTeacherSessions_TeacherAvailableScheduleId",
                table: "BookedTeacherSessions",
                column: "TeacherAvailableScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAvailableSchedules_LearnerId",
                table: "TeacherAvailableSchedules",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAvailableSchedules_TeacherId",
                table: "TeacherAvailableSchedules",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookedTeacherSessions");

            migrationBuilder.DropTable(
                name: "TeacherAvailableSchedules");
        }
    }
}
