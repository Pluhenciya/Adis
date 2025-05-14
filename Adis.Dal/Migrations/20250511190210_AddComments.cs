using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id_document = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    filename = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_sender = table.Column<int>(type: "int", nullable: false),
                    id_task = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_document);
                    table.ForeignKey(
                        name: "fk_task_comments",
                        column: x => x.id_task,
                        principalTable: "tasks",
                        principalColumn: "id_task");
                    table.ForeignKey(
                        name: "fk_user_comments",
                        column: x => x.id_sender,
                        principalTable: "users",
                        principalColumn: "id_user");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_comments_id_sender",
                table: "comments",
                column: "id_sender");

            migrationBuilder.CreateIndex(
                name: "ix_comments_id_task",
                table: "comments",
                column: "id_task");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");
        }
    }
}
