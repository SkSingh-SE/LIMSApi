using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePermissionMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Permissions_PermissionID",
                table: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionMasters",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "Addp",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "Deletep",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "Editp",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "ExportP",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "PermissionMasters");

            migrationBuilder.DropColumn(
                name: "Viewp",
                table: "PermissionMasters");

            migrationBuilder.RenameTable(
                name: "PermissionMasters",
                newName: "PermissionMaster");

            migrationBuilder.RenameColumn(
                name: "CompanyCode",
                table: "PermissionMaster",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PermissionMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "PermissionMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PermissionMaster",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionMaster",
                table: "PermissionMaster",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionMaster_MenuID",
                table: "PermissionMaster",
                column: "MenuID");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_PermissionMaster_MenuID",
                table: "PermissionMaster");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PermissionMaster");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "PermissionMaster");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PermissionMaster");

            migrationBuilder.RenameTable(
                name: "PermissionMaster",
                newName: "PermissionMasters");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "PermissionMasters",
                newName: "CompanyCode");

            migrationBuilder.AddColumn<bool>(
                name: "Addp",
                table: "PermissionMasters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "PermissionMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "PermissionMasters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deletep",
                table: "PermissionMasters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Editp",
                table: "PermissionMasters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExportP",
                table: "PermissionMasters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PermissionMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "PermissionMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "PermissionMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoleID",
                table: "PermissionMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Viewp",
                table: "PermissionMasters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionMasters",
                table: "PermissionMasters",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuID = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Permissions_MenuMasters_MenuID",
                        column: x => x.MenuID,
                        principalTable: "MenuMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_MenuID",
                table: "Permissions",
                column: "MenuID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Permissions_PermissionID",
                table: "UserPermissions",
                column: "PermissionID",
                principalTable: "Permissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
