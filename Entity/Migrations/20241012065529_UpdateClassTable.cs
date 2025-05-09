﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TeacherAvailableSchedules",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAvailableSchedules_Classes_ClassId",
                table: "TeacherAvailableSchedules");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAvailableSchedules_ClassId",
                table: "TeacherAvailableSchedules");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "TeacherAvailableSchedules");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TeacherAvailableSchedules",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");
        }
    }
}
