using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseEquivalencyDesktop.Migrations
{
    /// <inheritdoc />
    public partial class StudyPlanExportNewlyNeededFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Universities",
                type: "TEXT",
                nullable: false,
                defaultValue: "Canada");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeEndDate",
                table: "StudyPlans",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeStartDate",
                table: "StudyPlans",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastCompletedAcademicTerm",
                table: "StudyPlans",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ExpectedGraduationYear",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                defaultValue: 2025);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "ExchangeEndDate",
                table: "StudyPlans");

            migrationBuilder.DropColumn(
                name: "ExchangeStartDate",
                table: "StudyPlans");

            migrationBuilder.DropColumn(
                name: "LastCompletedAcademicTerm",
                table: "StudyPlans");

            migrationBuilder.DropColumn(
                name: "ExpectedGraduationYear",
                table: "Students");
        }
    }
}
