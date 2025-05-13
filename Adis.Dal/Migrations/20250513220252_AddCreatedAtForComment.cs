using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtForComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "comments",
                type: "datetime",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "comments");
        }
    }
}
