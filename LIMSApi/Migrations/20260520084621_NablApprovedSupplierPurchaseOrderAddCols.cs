using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablApprovedSupplierPurchaseOrderAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizedBy",
                table: "NablPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NablPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PONo",
                table: "NablPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNo",
                table: "NablPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestedQuantity",
                table: "NablPurchaseOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "NablApprovedSuppliers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GstNo",
                table: "NablApprovedSuppliers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizedBy",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PONo",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PhoneNo",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RequestedQuantity",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "GstNo",
                table: "NablApprovedSuppliers");
        }
    }
}
