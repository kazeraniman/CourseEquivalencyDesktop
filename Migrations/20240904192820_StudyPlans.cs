using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseEquivalencyDesktop.Migrations
{
    /// <inheritdoc />
    public partial class StudyPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinationUniversityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Seasonal = table.Column<string>(type: "TEXT", nullable: false),
                    Academic = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Universities_DestinationUniversityId",
                        column: x => x.DestinationUniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseStudyPlan",
                columns: table => new
                {
                    HomeUniversityCoursesId = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeUniversityCoursesStudyPlansId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudyPlan", x => new { x.HomeUniversityCoursesId, x.HomeUniversityCoursesStudyPlansId });
                    table.ForeignKey(
                        name: "FK_CourseStudyPlan_Courses_HomeUniversityCoursesId",
                        column: x => x.HomeUniversityCoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudyPlan_StudyPlans_HomeUniversityCoursesStudyPlansId",
                        column: x => x.HomeUniversityCoursesStudyPlansId,
                        principalTable: "StudyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseStudyPlan1",
                columns: table => new
                {
                    DestinationUniversityCoursesId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinationUniversityCoursesStudyPlansId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudyPlan1", x => new { x.DestinationUniversityCoursesId, x.DestinationUniversityCoursesStudyPlansId });
                    table.ForeignKey(
                        name: "FK_CourseStudyPlan1_Courses_DestinationUniversityCoursesId",
                        column: x => x.DestinationUniversityCoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudyPlan1_StudyPlans_DestinationUniversityCoursesStudyPlansId",
                        column: x => x.DestinationUniversityCoursesStudyPlansId,
                        principalTable: "StudyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudyPlan_HomeUniversityCoursesStudyPlansId",
                table: "CourseStudyPlan",
                column: "HomeUniversityCoursesStudyPlansId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudyPlan1_DestinationUniversityCoursesStudyPlansId",
                table: "CourseStudyPlan1",
                column: "DestinationUniversityCoursesStudyPlansId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_DestinationUniversityId",
                table: "StudyPlans",
                column: "DestinationUniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_StudentId",
                table: "StudyPlans",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseStudyPlan");

            migrationBuilder.DropTable(
                name: "CourseStudyPlan1");

            migrationBuilder.DropTable(
                name: "StudyPlans");
        }
    }
}
