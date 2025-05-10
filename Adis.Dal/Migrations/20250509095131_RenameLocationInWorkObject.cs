using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class RenameLocationInWorkObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_location_project",
                table: "projects");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropColumn(
                name: "name_work_object",
                table: "projects");

            migrationBuilder.CreateTable(
                name: "work_objects",
                columns: table => new
                {
                    id_work_object = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    geometry = table.Column<Geometry>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_work_object);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_work_objects_geometry",
                table: "work_objects",
                column: "geometry")
                .Annotation("MySql:SpatialIndex", true);

            migrationBuilder.AddForeignKey(
                name: "fk_work_object_project",
                table: "projects",
                column: "id_location",
                principalTable: "work_objects",
                principalColumn: "id_work_object");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_object_project",
                table: "projects");

            migrationBuilder.DropTable(
                name: "work_objects");

            migrationBuilder.AddColumn<string>(
                name: "name_work_object",
                table: "projects",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id_location = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    geometry = table.Column<Geometry>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_location);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_locations_geometry",
                table: "locations",
                column: "geometry")
                .Annotation("MySql:SpatialIndex", true);

            migrationBuilder.AddForeignKey(
                name: "fk_location_project",
                table: "projects",
                column: "id_location",
                principalTable: "locations",
                principalColumn: "id_location");
        }
    }
}
