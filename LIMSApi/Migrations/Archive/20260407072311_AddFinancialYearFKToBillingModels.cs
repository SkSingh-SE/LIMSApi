using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialYearFKToBillingModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialYear",
                table: "InvoiceCases");

            migrationBuilder.AddColumn<long>(
                name: "FinancialYearId",
                table: "TaxInvoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FinancialYearId",
                table: "ProformaInvoiceHeader",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FinancialYearId",
                table: "InvoiceCases",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FinancialYearId",
                table: "DebitNotes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FinancialYearId",
                table: "CreditNotes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FinancialYearId",
                table: "AdvancePaymentVouchers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxInvoices_FinancialYearId",
                table: "TaxInvoices",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeader_FinancialYearId",
                table: "ProformaInvoiceHeader",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCases_FinancialYearId",
                table: "InvoiceCases",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_FinancialYearId",
                table: "DebitNotes",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_FinancialYearId",
                table: "CreditNotes",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePaymentVouchers_FinancialYearId",
                table: "AdvancePaymentVouchers",
                column: "FinancialYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvancePaymentVouchers_FinancialYears_FinancialYearId",
                table: "AdvancePaymentVouchers",
                column: "FinancialYearId",
                principalTable: "FinancialYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditNotes_FinancialYears_FinancialYearId",
                table: "CreditNotes",
                column: "FinancialYearId",
                principalTable: "FinancialYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DebitNotes_FinancialYears_FinancialYearId",
                table: "DebitNotes",
                column: "FinancialYearId",
                principalTable: "FinancialYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceCases_FinancialYears_FinancialYearId",
                table: "InvoiceCases",
                column: "FinancialYearId",
                principalTable: "FinancialYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProformaInvoiceHeader_FinancialYears_FinancialYearId",
                table: "ProformaInvoiceHeader",
                column: "FinancialYearId",
                principalTable: "FinancialYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaxInvoices_FinancialYears_FinancialYearId",
                table: "TaxInvoices",
                column: "FinancialYearId",
                principalTable: "FinancialYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvancePaymentVouchers_FinancialYears_FinancialYearId",
                table: "AdvancePaymentVouchers");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditNotes_FinancialYears_FinancialYearId",
                table: "CreditNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_DebitNotes_FinancialYears_FinancialYearId",
                table: "DebitNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceCases_FinancialYears_FinancialYearId",
                table: "InvoiceCases");

            migrationBuilder.DropForeignKey(
                name: "FK_ProformaInvoiceHeader_FinancialYears_FinancialYearId",
                table: "ProformaInvoiceHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxInvoices_FinancialYears_FinancialYearId",
                table: "TaxInvoices");

            migrationBuilder.DropIndex(
                name: "IX_TaxInvoices_FinancialYearId",
                table: "TaxInvoices");

            migrationBuilder.DropIndex(
                name: "IX_ProformaInvoiceHeader_FinancialYearId",
                table: "ProformaInvoiceHeader");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceCases_FinancialYearId",
                table: "InvoiceCases");

            migrationBuilder.DropIndex(
                name: "IX_DebitNotes_FinancialYearId",
                table: "DebitNotes");

            migrationBuilder.DropIndex(
                name: "IX_CreditNotes_FinancialYearId",
                table: "CreditNotes");

            migrationBuilder.DropIndex(
                name: "IX_AdvancePaymentVouchers_FinancialYearId",
                table: "AdvancePaymentVouchers");

            migrationBuilder.DropColumn(
                name: "FinancialYearId",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "FinancialYearId",
                table: "ProformaInvoiceHeader");

            migrationBuilder.DropColumn(
                name: "FinancialYearId",
                table: "InvoiceCases");

            migrationBuilder.DropColumn(
                name: "FinancialYearId",
                table: "DebitNotes");

            migrationBuilder.DropColumn(
                name: "FinancialYearId",
                table: "CreditNotes");

            migrationBuilder.DropColumn(
                name: "FinancialYearId",
                table: "AdvancePaymentVouchers");

            migrationBuilder.AddColumn<string>(
                name: "FinancialYear",
                table: "InvoiceCases",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
