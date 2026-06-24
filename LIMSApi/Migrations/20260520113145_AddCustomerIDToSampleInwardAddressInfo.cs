using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerIDToSampleInwardAddressInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerID",
                table: "InwardAddresses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InwardAddresses_CustomerID",
                table: "InwardAddresses",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_InwardAddresses_Customers_CustomerID",
                table: "InwardAddresses",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InwardAddresses_Customers_CustomerID",
                table: "InwardAddresses");

            migrationBuilder.DropIndex(
                name: "IX_InwardAddresses_CustomerID",
                table: "InwardAddresses");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "InwardAddresses");
        }
    }
}
