using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInwardmethoddependentadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectionType",
                table: "GeneralTestMethod",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestCaseID",
                table: "GeneralTestMethod",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "GeneralTestMethod",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProformaInvoiceHeader",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    PINo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PIDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaInvoiceHeader", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeader_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProformaInvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PIHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    ChargeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProformaInvoiceHeaderID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaInvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetails_ProformaInvoiceHeader_ProformaInvoiceHeaderID",
                        column: x => x.ProformaInvoiceHeaderID,
                        principalTable: "ProformaInvoiceHeader",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetails_ProformaInvoiceHeaderID",
                table: "ProformaInvoiceDetails",
                column: "ProformaInvoiceHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeader_InwardID",
                table: "ProformaInvoiceHeader",
                column: "InwardID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProformaInvoiceDetails");

            migrationBuilder.DropTable(
                name: "ProformaInvoiceHeader");

            migrationBuilder.DropColumn(
                name: "SelectionType",
                table: "GeneralTestMethod");

            migrationBuilder.DropColumn(
                name: "TestCaseID",
                table: "GeneralTestMethod");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "GeneralTestMethod");
        }
    }
}
