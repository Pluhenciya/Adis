using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddConstractor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "end_execution_date",
                table: "projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_constractor",
                table: "projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "start_execution_date",
                table: "projects",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "constractor",
                columns: table => new
                {
                    id_constractor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_constractor);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_projects_id_constractor",
                table: "projects",
                column: "id_constractor");

            migrationBuilder.AddForeignKey(
                name: "fk_projects_constractor",
                table: "projects",
                column: "id_constractor",
                principalTable: "constractor",
                principalColumn: "id_constractor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_projects_constractor",
                table: "projects");

            migrationBuilder.DropTable(
                name: "constractor");

            migrationBuilder.DropIndex(
                name: "IX_projects_id_constractor",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "end_execution_date",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "id_constractor",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "start_execution_date",
                table: "projects");
        }
    }
}
