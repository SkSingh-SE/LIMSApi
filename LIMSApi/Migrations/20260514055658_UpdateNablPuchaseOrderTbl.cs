using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNablPuchaseOrderTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GSTNo",
                table: "NablPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrandTotal",
                table: "NablPurchaseOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GstAmount",
                table: "NablPurchaseOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GstPercentage",
                table: "NablPurchaseOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderType",
                table: "NablPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierAddress",
                table: "NablPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TearmCondition",
                table: "NablPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GSTNo",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "GrandTotal",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "GstAmount",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "GstPercentage",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "SupplierAddress",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "TearmCondition",
                table: "NablPurchaseOrders");
        }
    }
}
