using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "text_result",
                table: "tasks",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id_document = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    filename = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_user = table.Column<int>(type: "int", nullable: true),
                    type = table.Column<string>(type: "enum('estimate', 'other')", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_document);
                    table.ForeignKey(
                        name: "fk_user_documents",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id_user");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users_check_tasks",
                columns: table => new
                {
                    id_task = table.Column<int>(type: "int", nullable: false),
                    id_user = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_check_tasks", x => new { x.id_task, x.id_user });
                    table.ForeignKey(
                        name: "fk_task_check_tasks",
                        column: x => x.id_task,
                        principalTable: "tasks",
                        principalColumn: "id_task",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "fk_user_check_tasks",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.NoAction);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users_execute_tasks",
                columns: table => new
                {
                    id_task = table.Column<int>(type: "int", nullable: false),
                    id_user = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_execute_tasks", x => new { x.id_task, x.id_user });
                    table.ForeignKey(
                        name: "fk_task_execute_tasks",
                        column: x => x.id_task,
                        principalTable: "tasks",
                        principalColumn: "id_task",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "fk_user_execute_tasks",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.NoAction);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "fk_task_documents",
                        column: x => x.id_task,
                        principalTable: "tasks",
                        principalColumn: "id_task",
                        onDelete: ReferentialAction.NoAction);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_documents_id_user",
                table: "documents",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_has_documents_id_task",
                table: "tasks_has_documents",
                column: "id_task");

            migrationBuilder.CreateIndex(
                name: "ix_users_check_tasks_id_user",
                table: "users_check_tasks",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "ix_users_execute_tasks_id_user",
                table: "users_execute_tasks",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tasks_has_documents");

            migrationBuilder.DropTable(
                name: "users_check_tasks");

            migrationBuilder.DropTable(
                name: "users_execute_tasks");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropColumn(
                name: "text_result",
                table: "tasks");
        }
    }
}
