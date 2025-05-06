using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_projects_budget",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "budget",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "description",
                table: "projects");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "projects",
                type: "enum('designing', 'contractorSearch', 'inExecution', 'completed')",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('draft', 'inProgress', 'completed', 'overdue')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id_task = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_project = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_task);
                    table.ForeignKey(
                        name: "FK_project_tasks",
                        column: x => x.id_project,
                        principalTable: "projects",
                        principalColumn: "id_project");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_id_project",
                table: "tasks",
                column: "id_project");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "projects",
                type: "enum('draft', 'inProgress', 'completed', 'overdue')",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('designing', 'contractorSearch', 'inExecution', 'completed')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "budget",
                table: "projects",
                type: "DECIMAL(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "projects",
                type: "datetime",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "projects",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddCheckConstraint(
                name: "chk_projects_budget",
                table: "projects",
                sql: "budget >= 0");
        }
    }
}
