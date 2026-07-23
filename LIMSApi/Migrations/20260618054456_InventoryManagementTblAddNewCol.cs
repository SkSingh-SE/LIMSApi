using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class InventoryManagementTblAddNewCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StorageCondition",
                table: "InventoryManagements",
                newName: "StorageLocation");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "InventoryManagements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "InventoryManagements");

            migrationBuilder.RenameColumn(
                name: "StorageLocation",
                table: "InventoryManagements",
                newName: "StorageCondition");
        }
    }
}
