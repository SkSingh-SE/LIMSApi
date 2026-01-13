using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddChargeEventAndStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectionType",
                table: "GeneralTestMethods");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "GeneralTestMethods");

            migrationBuilder.AddColumn<string>(
                name: "BillingStatus",
                table: "SampleInwards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChargeEvents",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: true),
                    ReportID = table.Column<long>(type: "bigint", nullable: true),
                    ChargeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaxInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    ProformaInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    SelectionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoicedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeEvents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChargeEvents_ProformaInvoiceHeader_ProformaInvoiceID",
                        column: x => x.ProformaInvoiceID,
                        principalTable: "ProformaInvoiceHeader",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChargeEvents_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChargeEvents_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargeEvents_TaxInvoices_TaxInvoiceID",
                        column: x => x.TaxInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargeEvents_InwardID",
                table: "ChargeEvents",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeEvents_ProformaInvoiceID",
                table: "ChargeEvents",
                column: "ProformaInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeEvents_SampleID",
                table: "ChargeEvents",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeEvents_TaxInvoiceID",
                table: "ChargeEvents",
                column: "TaxInvoiceID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeEvents");

            migrationBuilder.DropColumn(
                name: "BillingStatus",
                table: "SampleInwards");

            migrationBuilder.AddColumn<string>(
                name: "SelectionType",
                table: "GeneralTestMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "GeneralTestMethods",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
