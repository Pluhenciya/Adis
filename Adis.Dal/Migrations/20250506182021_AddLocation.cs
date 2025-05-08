using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_tasks",
                table: "tasks");

            migrationBuilder.AddColumn<int>(
                name: "id_location",
                table: "projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "name_work_object",
                table: "projects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id_location = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    geometry = table.Column<Geometry>(type: "geometry", nullable: false),
                    location_type = table.Column<string>(type: "enum('point', 'lineString', 'polygon')", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_location);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_projects_id_location",
                table: "projects",
                column: "id_location");

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

            migrationBuilder.AddForeignKey(
                name: "fk_project_tasks",
                table: "tasks",
                column: "id_project",
                principalTable: "projects",
                principalColumn: "id_project");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_location_project",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "fk_project_tasks",
                table: "tasks");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropIndex(
                name: "ix_projects_id_location",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "id_location",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "name_work_object",
                table: "projects");

            migrationBuilder.AddForeignKey(
                name: "FK_project_tasks",
                table: "tasks",
                column: "id_project",
                principalTable: "projects",
                principalColumn: "id_project");
        }
    }
}
