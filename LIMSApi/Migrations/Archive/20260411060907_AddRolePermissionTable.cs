using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Snapshot drift fix: the DB may already have FK pointing to TestMethodSpecifications
            // (from an earlier migration). Drop whichever variant exists, safely.
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DimensionalFactorMasters_TestMethodStandards_DefaultTestMethodID')
                    ALTER TABLE [DimensionalFactorMasters] DROP CONSTRAINT [FK_DimensionalFactorMasters_TestMethodStandards_DefaultTestMethodID];
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DimensionalFactorMasters_TestMethodSpecifications_DefaultTestMethodID')
                    ALTER TABLE [DimensionalFactorMasters] DROP CONSTRAINT [FK_DimensionalFactorMasters_TestMethodSpecifications_DefaultTestMethodID];
            ");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_RolePermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RolePermissions_PermissionMasters_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "PermissionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_RolePermissions_RoleMasters_RoleID",
                        column: x => x.RoleID,
                        principalTable: "RoleMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_Role_Permission",
                table: "RolePermissions",
                columns: new[] { "RoleID", "PermissionID" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionID",
                table: "RolePermissions",
                column: "PermissionID");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodSpecifications_DefaultTestMethodID",
                table: "DimensionalFactorMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodSpecifications_DefaultTestMethodID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodStandards_DefaultTestMethodID",
                table: "DimensionalFactorMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodStandards",
                principalColumn: "ID");
        }
    }
}
