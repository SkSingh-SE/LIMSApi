using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePIdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProformaInvoiceDetails_ProformaInvoiceHeader_ProformaInvoiceHeaderID",
                table: "ProformaInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "PIHeaderId",
                table: "ProformaInvoiceDetails");

            migrationBuilder.AlterColumn<long>(
                name: "ProformaInvoiceHeaderID",
                table: "ProformaInvoiceDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceCaseConfigID",
                table: "ProformaInvoiceDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectionType",
                table: "ProformaInvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UsedValue",
                table: "ProformaInvoiceDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProformaInvoiceDetails_ProformaInvoiceHeader_ProformaInvoiceHeaderID",
                table: "ProformaInvoiceDetails",
                column: "ProformaInvoiceHeaderID",
                principalTable: "ProformaInvoiceHeader",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProformaInvoiceDetails_ProformaInvoiceHeader_ProformaInvoiceHeaderID",
                table: "ProformaInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceCaseConfigID",
                table: "ProformaInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "SelectionType",
                table: "ProformaInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "UsedValue",
                table: "ProformaInvoiceDetails");

            migrationBuilder.AlterColumn<long>(
                name: "ProformaInvoiceHeaderID",
                table: "ProformaInvoiceDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "PIHeaderId",
                table: "ProformaInvoiceDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_ProformaInvoiceDetails_ProformaInvoiceHeader_ProformaInvoiceHeaderID",
                table: "ProformaInvoiceDetails",
                column: "ProformaInvoiceHeaderID",
                principalTable: "ProformaInvoiceHeader",
                principalColumn: "ID");
        }
    }
}
