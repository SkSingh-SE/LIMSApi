using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablPurchaseMaterialVerificationAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectiveActions",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Deviations",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GstNo",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionBy",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceDate",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderType",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PODate",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNo",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoNo",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNo",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "CorrectiveActions",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "Deviations",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "GstNo",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "InspectionBy",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "PODate",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "PhoneNo",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "PoNo",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNo",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "NablPurchaseMaterialVerifications");
        }
    }
}
