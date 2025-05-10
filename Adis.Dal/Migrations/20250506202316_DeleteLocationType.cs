using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class DeleteLocationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location_type",
                table: "locations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "location_type",
                table: "locations",
                type: "enum('point', 'lineString', 'polygon')",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
