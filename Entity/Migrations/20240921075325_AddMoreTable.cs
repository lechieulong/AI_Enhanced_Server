using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestExam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestExam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseTimelines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTimelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTimelines_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartSkill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartNumber = table.Column<int>(type: "int", nullable: false),
                    SkillTest = table.Column<int>(type: "int", nullable: false),
                    ContentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AudioURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartSkill_TestExam_TestId",
                        column: x => x.TestId,
                        principalTable: "TestExam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseTimelineDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimelineId = table.Column<int>(type: "int", nullable: false),
                    CourseTimelineId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTimelineDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTimelineDetails_CourseTimelines_CourseTimelineId",
                        column: x => x.CourseTimelineId,
                        principalTable: "CourseTimelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTypePart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionGuide = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartSkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypePart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionTypePart_PartSkill_PartSkillId",
                        column: x => x.PartSkillId,
                        principalTable: "PartSkill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionTypePart_TestExam_TestExamId",
                        column: x => x.TestExamId,
                        principalTable: "TestExam",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypePartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxMarks = table.Column<int>(type: "int", nullable: false),
                    QuestionTypePartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_QuestionTypePart_QuestionTypePartId",
                        column: x => x.QuestionTypePartId,
                        principalTable: "QuestionTypePart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTimelineDetails_CourseTimelineId",
                table: "CourseTimelineDetails",
                column: "CourseTimelineId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTimelines_CourseId",
                table: "CourseTimelines",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PartSkill_TestId",
                table: "PartSkill",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionTypePartId",
                table: "Question",
                column: "QuestionTypePartId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTypePart_PartSkillId",
                table: "QuestionTypePart",
                column: "PartSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTypePart_TestExamId",
                table: "QuestionTypePart",
                column: "TestExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseTimelineDetails");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "CourseTimelines");

            migrationBuilder.DropTable(
                name: "QuestionTypePart");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "PartSkill");

            migrationBuilder.DropTable(
                name: "TestExam");
        }
    }
}
