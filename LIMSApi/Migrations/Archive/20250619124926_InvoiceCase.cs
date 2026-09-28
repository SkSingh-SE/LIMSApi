using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialYear = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceCases_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceCasePrices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceCaseID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCasePrices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceCasePrices_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceCasePrices_InvoiceCases_InvoiceCaseID",
                        column: x => x.InvoiceCaseID,
                        principalTable: "InvoiceCases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestInvoiceCase_InvoiceCaseConfigID",
                table: "LaboratoryTestInvoiceCase",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCasePrices_InvoiceCaseConfigID",
                table: "InvoiceCasePrices",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCasePrices_InvoiceCaseID",
                table: "InvoiceCasePrices",
                column: "InvoiceCaseID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCases_LaboratoryTestID",
                table: "InvoiceCases",
                column: "LaboratoryTestID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestInvoiceCase_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                table: "LaboratoryTestInvoiceCase",
                column: "InvoiceCaseConfigID",
                principalTable: "InvoiceCaseConfigurations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTestInvoiceCase_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                table: "LaboratoryTestInvoiceCase");

            migrationBuilder.DropTable(
                name: "InvoiceCasePrices");

            migrationBuilder.DropTable(
                name: "InvoiceCases");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTestInvoiceCase_InvoiceCaseConfigID",
                table: "LaboratoryTestInvoiceCase");
        }
    }
}
