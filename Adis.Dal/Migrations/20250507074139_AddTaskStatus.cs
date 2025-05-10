using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "tasks",
                type: "enum('toDo', 'doing', 'checking', 'completed')",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "tasks");
        }
    }
}
