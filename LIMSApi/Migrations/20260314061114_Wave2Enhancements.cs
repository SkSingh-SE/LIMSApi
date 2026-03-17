using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Wave2Enhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SpecimenTypeId",
                table: "CuttingPriceMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Diameter",
                table: "CuttingChargeSamples",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "CuttingChargeSamples",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Orientation",
                table: "CuttingChargeSamples",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "CuttingChargeSamples",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparationInstructions",
                table: "CuttingChargeSamples",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparationStatus",
                table: "CuttingChargeSamples",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SpecimenTypeId",
                table: "CuttingChargeSamples",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Thickness",
                table: "CuttingChargeSamples",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "CuttingChargeSamples",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerLedgers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChequeNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransactionRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InwardId = table.Column<long>(type: "bigint", nullable: true),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerLedgers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerLedgers_SampleInwards_InwardId",
                        column: x => x.InwardId,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerLedgers_TaxInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentReceipts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChequeNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransactionRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvoiceIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentReceipts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuttingPriceMasters_SpecimenTypeId",
                table: "CuttingPriceMasters",
                column: "SpecimenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeSamples_SpecimenTypeId",
                table: "CuttingChargeSamples",
                column: "SpecimenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLedgers_CustomerId",
                table: "CustomerLedgers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLedgers_InvoiceId",
                table: "CustomerLedgers",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLedgers_InwardId",
                table: "CustomerLedgers",
                column: "InwardId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_CustomerId",
                table: "PaymentReceipts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_ReceiptNo",
                table: "PaymentReceipts",
                column: "ReceiptNo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CuttingChargeSamples_SpecimenTypeMasters_SpecimenTypeId",
                table: "CuttingChargeSamples",
                column: "SpecimenTypeId",
                principalTable: "SpecimenTypeMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_CuttingPriceMasters_SpecimenTypeMasters_SpecimenTypeId",
                table: "CuttingPriceMasters",
                column: "SpecimenTypeId",
                principalTable: "SpecimenTypeMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuttingChargeSamples_SpecimenTypeMasters_SpecimenTypeId",
                table: "CuttingChargeSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_CuttingPriceMasters_SpecimenTypeMasters_SpecimenTypeId",
                table: "CuttingPriceMasters");

            migrationBuilder.DropTable(
                name: "CustomerLedgers");

            migrationBuilder.DropTable(
                name: "PaymentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CuttingPriceMasters_SpecimenTypeId",
                table: "CuttingPriceMasters");

            migrationBuilder.DropIndex(
                name: "IX_CuttingChargeSamples_SpecimenTypeId",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "SpecimenTypeId",
                table: "CuttingPriceMasters");

            migrationBuilder.DropColumn(
                name: "Diameter",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "PreparationInstructions",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "PreparationStatus",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "SpecimenTypeId",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "Thickness",
                table: "CuttingChargeSamples");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "CuttingChargeSamples");
        }
    }
}
