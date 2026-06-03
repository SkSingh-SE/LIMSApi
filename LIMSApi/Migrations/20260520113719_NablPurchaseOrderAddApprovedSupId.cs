using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablPurchaseOrderAddApprovedSupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApprovedSupplierId",
                table: "NablPurchaseOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseOrders_ApprovedSupplierId",
                table: "NablPurchaseOrders",
                column: "ApprovedSupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablPurchaseOrders_NablApprovedSuppliers_ApprovedSupplierId",
                table: "NablPurchaseOrders",
                column: "ApprovedSupplierId",
                principalTable: "NablApprovedSuppliers",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablPurchaseOrders_NablApprovedSuppliers_ApprovedSupplierId",
                table: "NablPurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_NablPurchaseOrders_ApprovedSupplierId",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ApprovedSupplierId",
                table: "NablPurchaseOrders");
        }
    }
}
