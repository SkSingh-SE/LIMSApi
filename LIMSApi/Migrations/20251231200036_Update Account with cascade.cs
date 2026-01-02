using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountwithcascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_Customers_CustomerId",
                table: "PaymentOrders");

            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiceGenerated",
                table: "SampleInwards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalTestCharges",
                table: "SampleInwards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "ReportHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "PaymentOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "TaxInvoiceID",
                table: "PaymentOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailId",
                table: "InwardAddresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "InwardAddresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ReportAmendmentTokens",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    LinkExpiryOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FreeUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportAmendmentTokens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportAmendmentTokens_Reports_ReportID",
                        column: x => x.ReportID,
                        principalTable: "Reports",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxInvoices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxInvoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TaxInvoices_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxInvoices_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAmendments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportID = table.Column<long>(type: "bigint", nullable: false),
                    TokenID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsChargeable = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentOrderID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAmendments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerAmendments_ReportAmendmentTokens_TokenID",
                        column: x => x.TokenID,
                        principalTable: "ReportAmendmentTokens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerAmendments_Reports_ReportID",
                        column: x => x.ReportID,
                        principalTable: "Reports",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_TaxInvoiceID",
                table: "PaymentOrders",
                column: "TaxInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAmendments_ReportID",
                table: "CustomerAmendments",
                column: "ReportID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAmendments_TokenID",
                table: "CustomerAmendments",
                column: "TokenID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAmendmentTokens_ReportID",
                table: "ReportAmendmentTokens",
                column: "ReportID");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInvoices_CustomerID",
                table: "TaxInvoices",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInvoices_InwardID",
                table: "TaxInvoices",
                column: "InwardID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_Customers_CustomerId",
                table: "PaymentOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_TaxInvoices_TaxInvoiceID",
                table: "PaymentOrders",
                column: "TaxInvoiceID",
                principalTable: "TaxInvoices",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_Customers_CustomerId",
                table: "PaymentOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_TaxInvoices_TaxInvoiceID",
                table: "PaymentOrders");

            migrationBuilder.DropTable(
                name: "CustomerAmendments");

            migrationBuilder.DropTable(
                name: "TaxInvoices");

            migrationBuilder.DropTable(
                name: "ReportAmendmentTokens");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrders_TaxInvoiceID",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "IsInvoiceGenerated",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "TotalTestCharges",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "ReportHeaders");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "TaxInvoiceID",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "InwardAddresses");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "InwardAddresses");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_Customers_CustomerId",
                table: "PaymentOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
