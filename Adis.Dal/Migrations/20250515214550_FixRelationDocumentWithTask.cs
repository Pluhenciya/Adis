using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationDocumentWithTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tasks_has_documents");

            migrationBuilder.AddColumn<int>(
                name: "id_task",
                table: "documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_documents_id_task",
                table: "documents",
                column: "id_task");

            migrationBuilder.AddForeignKey(
                name: "fk_documents_task",
                table: "documents",
                column: "id_task",
                principalTable: "tasks",
                principalColumn: "id_task");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documents_task",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "ix_documents_id_task",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "id_task",
                table: "documents");

            migrationBuilder.CreateTable(
                name: "tasks_has_documents",
                columns: table => new
                {
                    id_document = table.Column<int>(type: "int", nullable: false),
                    id_task = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks_has_documents", x => new { x.id_document, x.id_task });
                    table.ForeignKey(
                        name: "fk_document_tasks",
                        column: x => x.id_document,
                        principalTable: "documents",
                        principalColumn: "id_document",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_task_documents",
                        column: x => x.id_task,
                        principalTable: "tasks",
                        principalColumn: "id_task",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_has_documents_id_task",
                table: "tasks_has_documents",
                column: "id_task");
        }
    }
}
