using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddActualEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_projects_dates",
                table: "projects");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "tasks",
                newName: "planned_end_date");

            migrationBuilder.RenameColumn(
                name: "end_execution_date",
                table: "projects",
                newName: "planned_end_execution_date");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "projects",
                newName: "planned_end_date");

            migrationBuilder.AddColumn<DateOnly>(
                name: "actual_end_date",
                table: "tasks",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "actual_end_date",
                table: "projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "actual_end_execution_date",
                table: "projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "chk_projects_dates",
                table: "projects",
                sql: "start_date <= planned_end_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_projects_dates",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "actual_end_date",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "actual_end_date",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "actual_end_execution_date",
                table: "projects");

            migrationBuilder.RenameColumn(
                name: "planned_end_date",
                table: "tasks",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "planned_end_execution_date",
                table: "projects",
                newName: "end_execution_date");

            migrationBuilder.RenameColumn(
                name: "planned_end_date",
                table: "projects",
                newName: "end_date");

            migrationBuilder.AddCheckConstraint(
                name: "chk_projects_dates",
                table: "projects",
                sql: "start_date <= end_date");
        }
    }
}
