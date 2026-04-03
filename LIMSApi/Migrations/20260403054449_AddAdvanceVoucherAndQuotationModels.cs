using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvanceVoucherAndQuotationModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvancePaymentVouchers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AdjustedToInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    AdjustedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancePaymentVouchers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AdvancePaymentVouchers_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AdvancePaymentVouchers_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AdvancePaymentVouchers_TaxInvoices_AdjustedToInvoiceID",
                        column: x => x.AdjustedToInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QuotationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AcceptedBy = table.Column<long>(type: "bigint", nullable: true),
                    AcceptedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConvertedToInwardID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Quotations_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Quotations_SampleInwards_ConvertedToInwardID",
                        column: x => x.ConvertedToInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "QuotationItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationID = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuotationItems_Quotations_QuotationID",
                        column: x => x.QuotationID,
                        principalTable: "Quotations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePaymentVouchers_AdjustedToInvoiceID",
                table: "AdvancePaymentVouchers",
                column: "AdjustedToInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePaymentVouchers_CustomerID",
                table: "AdvancePaymentVouchers",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePaymentVouchers_InwardID",
                table: "AdvancePaymentVouchers",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_QuotationID",
                table: "QuotationItems",
                column: "QuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_ConvertedToInwardID",
                table: "Quotations",
                column: "ConvertedToInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CustomerID",
                table: "Quotations",
                column: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvancePaymentVouchers");

            migrationBuilder.DropTable(
                name: "QuotationItems");

            migrationBuilder.DropTable(
                name: "Quotations");
        }
    }
}
