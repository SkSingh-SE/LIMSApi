using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePermissionMaster2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionMaster_MenuMasters_MenuID",
                table: "PermissionMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_PermissionMaster_PermissionID",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionMaster",
                table: "PermissionMaster");

            migrationBuilder.RenameTable(
                name: "PermissionMaster",
                newName: "PermissionMasters");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionMaster_MenuID",
                table: "PermissionMasters",
                newName: "IX_PermissionMasters_MenuID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionMasters",
                table: "PermissionMasters",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionMasters_MenuMasters_MenuID",
                table: "PermissionMasters",
                column: "MenuID",
                principalTable: "MenuMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_PermissionMasters_PermissionID",
                table: "UserPermissions",
                column: "PermissionID",
                principalTable: "PermissionMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionMasters_MenuMasters_MenuID",
                table: "PermissionMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_PermissionMasters_PermissionID",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionMasters",
                table: "PermissionMasters");

            migrationBuilder.RenameTable(
                name: "PermissionMasters",
                newName: "PermissionMaster");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionMasters_MenuID",
                table: "PermissionMaster",
                newName: "IX_PermissionMaster_MenuID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionMaster",
                table: "PermissionMaster",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionMaster_MenuMasters_MenuID",
                table: "PermissionMaster",
                column: "MenuID",
                principalTable: "MenuMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_PermissionMaster_PermissionID",
                table: "UserPermissions",
                column: "PermissionID",
                principalTable: "PermissionMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
