using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "documents",
                type: "enum('GOST', 'SNIP', 'SP', 'TU', 'technicalRegulation', 'estimate', 'other')",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('estimate', 'other')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "documents",
                type: "enum('estimate', 'other')",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('GOST', 'SNIP', 'SP', 'TU', 'technicalRegulation', 'estimate', 'other')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
