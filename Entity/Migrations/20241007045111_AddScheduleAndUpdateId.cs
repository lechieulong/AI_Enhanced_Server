using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleAndUpdateId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key FK_Events_AspNetUsers_UserId
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UserId",
                table: "Events");

            // Drop the primary key constraint on Events
            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            // Drop the old Id column for Events
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Events");

            // Add a new Id column for Events with Guid type
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Events",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            // Recreate the primary key constraint on the new Id column
            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            // Re-add the foreign key with any updated references if needed
            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); // Update referential action as per your needs (Cascade, SetNull, Restrict, etc.)

            // Repeat similar operations for EmailLogs
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailLogs",
                table: "EmailLogs");

            // Drop the old Id column for EmailLogs
            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmailLogs");

            // Add a new Id column for EmailLogs with Guid type
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "EmailLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailLogs",
                table: "EmailLogs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TeacherAvailableSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAvailableSchedules", x => x.Id);
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
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LearnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookedDate = table.Column<DateTime>(type: "datetime", nullable: false)
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
                        name: "FK_BookedTeacherSessions_TeacherAvailableSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "TeacherAvailableSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookedTeacherSessions_LearnerId",
                table: "BookedTeacherSessions",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BookedTeacherSessions_ScheduleId",
                table: "BookedTeacherSessions",
                column: "ScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAvailableSchedules_TeacherId",
                table: "TeacherAvailableSchedules",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop BookedTeacherSessions and TeacherAvailableSchedules tables
            migrationBuilder.DropTable(
                name: "BookedTeacherSessions");

            migrationBuilder.DropTable(
                name: "TeacherAvailableSchedules");

            // Drop the foreign key FK_Events_AspNetUsers_UserId
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UserId",
                table: "Events");

            // Drop the primary key constraint on Events
            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            // Drop the new Id column for Events
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Events");

            // Recreate the old Id column for Events with int type
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Re-add the primary key constraint on the old Id column
            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            // Re-add the foreign key FK_Events_AspNetUsers_UserId
            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Drop the new Id column for EmailLogs
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailLogs",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmailLogs");

            // Recreate the old Id column for EmailLogs with int type
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EmailLogs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Re-add the primary key constraint on the old Id column
            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailLogs",
                table: "EmailLogs",
                column: "Id");
        }
    }
}
