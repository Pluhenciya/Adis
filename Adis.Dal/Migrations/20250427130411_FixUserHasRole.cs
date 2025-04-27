using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adis.Dal.Migrations
{
    /// <inheritdoc />
    public partial class FixUserHasRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_has_roles_roles_role_id",
                table: "user_has_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_has_roles_users_user_id",
                table: "user_has_roles");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user_has_roles",
                newName: "id_user");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "user_has_roles",
                newName: "id_role");

            migrationBuilder.RenameIndex(
                name: "ix_user_has_roles_user_id",
                table: "user_has_roles",
                newName: "IX_user_has_roles_id_user");

            migrationBuilder.AddForeignKey(
                name: "fk_user_has_roles_roles_id_role",
                table: "user_has_roles",
                column: "id_role",
                principalTable: "roles",
                principalColumn: "id_role",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "dk_user_has_roles_users_id_user",
                table: "user_has_roles",
                column: "id_user",
                principalTable: "users",
                principalColumn: "id_user",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_has_roles_roles_id_role",
                table: "user_has_roles");

            migrationBuilder.DropForeignKey(
                name: "dk_user_has_roles_users_id_user",
                table: "user_has_roles");

            migrationBuilder.RenameColumn(
                name: "id_user",
                table: "user_has_roles",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "id_role",
                table: "user_has_roles",
                newName: "role_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_has_roles_id_user",
                table: "user_has_roles",
                newName: "IX_user_has_roles_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_has_roles_roles_role_id",
                table: "user_has_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id_role",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_has_roles_users_user_id",
                table: "user_has_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id_user",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.Cascade);
        }
    }
}
