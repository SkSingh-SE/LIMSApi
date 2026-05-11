using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencySymbolAndExchangeRateToInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencySymbol",
                table: "TaxInvoices",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "TaxInvoices",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ForeignCurrencyGrandTotal",
                table: "TaxInvoices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "CurrencyMasters",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            // Seed symbols for common currencies
            migrationBuilder.Sql("UPDATE CurrencyMasters SET Symbol = N'₹' WHERE Code = 'INR'");
            migrationBuilder.Sql("UPDATE CurrencyMasters SET Symbol = '$' WHERE Code = 'USD'");
            migrationBuilder.Sql("UPDATE CurrencyMasters SET Symbol = N'€' WHERE Code = 'EUR'");
            migrationBuilder.Sql("UPDATE CurrencyMasters SET Symbol = N'£' WHERE Code = 'GBP'");

            // Default ₹ for existing TaxInvoice rows (all historical invoices are INR)
            migrationBuilder.Sql("UPDATE TaxInvoices SET CurrencySymbol = N'₹' WHERE CurrencySymbol = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencySymbol",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "ForeignCurrencyGrandTotal",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "CurrencyMasters");
        }
    }
}
