using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseEquivalencyDesktop.Migrations
{
    /// <inheritdoc />
    public partial class Equivalencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equivalencies",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquivalentCourseId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equivalencies", x => new { x.CourseId, x.EquivalentCourseId });
                    table.ForeignKey(
                        name: "FK_Equivalencies_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Equivalencies_Courses_EquivalentCourseId",
                        column: x => x.EquivalentCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equivalencies_EquivalentCourseId",
                table: "Equivalencies",
                column: "EquivalentCourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equivalencies");
        }
    }
}
