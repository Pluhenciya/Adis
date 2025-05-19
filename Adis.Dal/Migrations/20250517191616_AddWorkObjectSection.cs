using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkObjectSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_work_object_section",
                table: "execution_task",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "work_object_section",
                columns: table => new
                {
                    id_work_object_section = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_work_object_section);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_execution_task_id_work_object_section",
                table: "execution_task",
                column: "id_work_object_section");

            migrationBuilder.AddForeignKey(
                name: "fk_work_object_section_execution_tasks",
                table: "execution_task",
                column: "id_work_object_section",
                principalTable: "work_object_section",
                principalColumn: "id_work_object_section");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_object_section_execution_tasks",
                table: "execution_task");

            migrationBuilder.DropTable(
                name: "work_object_section");

            migrationBuilder.DropIndex(
                name: "ix_execution_task_id_work_object_section",
                table: "execution_task");

            migrationBuilder.DropColumn(
                name: "id_work_object_section",
                table: "execution_task");
        }
    }
}
