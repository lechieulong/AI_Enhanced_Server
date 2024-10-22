using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class CheckChangeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("232d02b9-d8b3-4814-ada5-5db0ca3b14d4"));

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("6e6f4b59-1063-4b8d-8f93-6506450c9481"));

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("8375c110-3031-40d1-9f23-1110bb77fb6f"));

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("b4c3d959-fdcc-4eef-93e3-4b20f4efc26c"));

            migrationBuilder.DropColumn(
                name: "Section",
                table: "Question");

            migrationBuilder.AlterColumn<int>(
                name: "Skill",
                table: "Question",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PartNumber",
                table: "Question",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("2388536a-c918-40b1-9aa9-8ba40b1c2d4a"), "Listening" },
                    { new Guid("29ebb27f-0dba-4afa-99f1-b4da7dfc1193"), "Reading" },
                    { new Guid("82d4e522-e1b2-4777-8d43-73c56cfdd3d6"), "Speaking" },
                    { new Guid("c14de6b4-3244-4fe3-8338-b8bcf16ce3d9"), "Writing" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("2388536a-c918-40b1-9aa9-8ba40b1c2d4a"));

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("29ebb27f-0dba-4afa-99f1-b4da7dfc1193"));

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("82d4e522-e1b2-4777-8d43-73c56cfdd3d6"));

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("c14de6b4-3244-4fe3-8338-b8bcf16ce3d9"));

            migrationBuilder.AlterColumn<int>(
                name: "Skill",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PartNumber",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Section",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
