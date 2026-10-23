using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ChemicalTestSubTypeArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestSubTypeID",
                table: "ChemicalTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PricingRuleType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    InvoiceCaption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubTypes_AnalysisTechniqueMasters_AnalysisTechniqueID",
                        column: x => x.AnalysisTechniqueID,
                        principalTable: "AnalysisTechniqueMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubTypes_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubTypes_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubTypeInvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubTypeID = table.Column<long>(type: "bigint", nullable: false),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DefaultPricingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubTypeInvoiceCases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubTypeInvoiceCases_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubTypeInvoiceCases_LaboratoryTestSubTypes_LaboratoryTestSubTypeID",
                        column: x => x.LaboratoryTestSubTypeID,
                        principalTable: "LaboratoryTestSubTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubTypeInvoiceCasePrices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubTypeInvoiceCaseID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubTypeInvoiceCasePrices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubTypeInvoiceCasePrices_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubTypeInvoiceCasePrices_LaboratoryTestSubTypeInvoiceCases_LaboratoryTestSubTypeInvoiceCaseID",
                        column: x => x.LaboratoryTestSubTypeInvoiceCaseID,
                        principalTable: "LaboratoryTestSubTypeInvoiceCases",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTests_LaboratoryTestSubTypeID",
                table: "ChemicalTests",
                column: "LaboratoryTestSubTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubTypeInvoiceCasePrices_InvoiceCaseConfigID",
                table: "LaboratoryTestSubTypeInvoiceCasePrices",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubTypeInvoiceCasePrices_LaboratoryTestSubTypeInvoiceCaseID",
                table: "LaboratoryTestSubTypeInvoiceCasePrices",
                column: "LaboratoryTestSubTypeInvoiceCaseID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubTypeInvoiceCases_FinancialYearId",
                table: "LaboratoryTestSubTypeInvoiceCases",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubTypeInvoiceCases_LaboratoryTestSubTypeID_EffectiveFrom",
                table: "LaboratoryTestSubTypeInvoiceCases",
                columns: new[] { "LaboratoryTestSubTypeID", "EffectiveFrom" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubTypes_AnalysisTechniqueID",
                table: "LaboratoryTestSubTypes",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubTypes_LaboratoryTestID",
                table: "LaboratoryTestSubTypes",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubTypes_MetalClassificationID",
                table: "LaboratoryTestSubTypes",
                column: "MetalClassificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTests_LaboratoryTestSubTypes_LaboratoryTestSubTypeID",
                table: "ChemicalTests",
                column: "LaboratoryTestSubTypeID",
                principalTable: "LaboratoryTestSubTypes",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTests_LaboratoryTestSubTypes_LaboratoryTestSubTypeID",
                table: "ChemicalTests");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubTypeInvoiceCasePrices");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubTypeInvoiceCases");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubTypes");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTests_LaboratoryTestSubTypeID",
                table: "ChemicalTests");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestSubTypeID",
                table: "ChemicalTests");
        }
    }
}
