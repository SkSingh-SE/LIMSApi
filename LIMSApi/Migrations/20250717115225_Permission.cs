using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Permission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuID = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    PermissionID = table.Column<long>(type: "bigint", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "Permissions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMasters_EmployeeID",
                table: "UserMasters",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMasters_RoleID",
                table: "UserMasters",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_MenuID",
                table: "Permissions",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionID",
                table: "UserPermissions",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserID",
                table: "UserPermissions",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMasters_EmployeeMasters_EmployeeID",
                table: "UserMasters",
                column: "EmployeeID",
                principalTable: "EmployeeMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMasters_RoleMasters_RoleID",
                table: "UserMasters",
                column: "RoleID",
                principalTable: "RoleMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMasters_EmployeeMasters_EmployeeID",
                table: "UserMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMasters_RoleMasters_RoleID",
                table: "UserMasters");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_UserMasters_EmployeeID",
                table: "UserMasters");

            migrationBuilder.DropIndex(
                name: "IX_UserMasters_RoleID",
                table: "UserMasters");
        }
    }
}
