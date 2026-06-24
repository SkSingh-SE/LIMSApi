using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialYearAwarePricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceCases_LaboratoryTestID",
                table: "InvoiceCases");

            migrationBuilder.DropColumn(
                name: "RatePerUnit",
                table: "CuttingPriceMasters");

            migrationBuilder.AddColumn<long>(
                name: "FinancialYearId",
                table: "SampleInwards",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MachiningChargeMasterID",
                table: "MachiningChargeItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "InvoiceCases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "SizeRangeMax",
                table: "CuttingPriceMasters",
                type: "decimal(7,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SizeRangeMin",
                table: "CuttingPriceMasters",
                type: "decimal(7,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CuttingPriceVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuttingPriceMasterID = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    RatePerUnit = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    RatePerUnitHard = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingPriceVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingPriceVersions_CuttingPriceMasters_CuttingPriceMasterID",
                        column: x => x.CuttingPriceMasterID,
                        principalTable: "CuttingPriceMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CuttingPriceVersions_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MachiningChargeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodStandardID = table.Column<long>(type: "bigint", nullable: false),
                    SpecimenRawMaterialSize = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SpecimenSize = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    DrawingFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachiningChargeMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID",
                        column: x => x.TestMethodStandardID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MachiningChargeVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachiningChargeMasterID = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    PriceGeneralMetal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceHardMetal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachiningChargeVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MachiningChargeVersions_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MachiningChargeVersions_MachiningChargeMasters_MachiningChargeMasterID",
                        column: x => x.MachiningChargeMasterID,
                        principalTable: "MachiningChargeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_FinancialYearId",
                table: "SampleInwards",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeItems_MachiningChargeMasterID",
                table: "MachiningChargeItems",
                column: "MachiningChargeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCases_LaboratoryTestID_EffectiveFrom",
                table: "InvoiceCases",
                columns: new[] { "LaboratoryTestID", "EffectiveFrom" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingPriceVersions_CuttingPriceMasterID_EffectiveFrom",
                table: "CuttingPriceVersions",
                columns: new[] { "CuttingPriceMasterID", "EffectiveFrom" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingPriceVersions_FinancialYearId",
                table: "CuttingPriceVersions",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeMasters_LaboratoryTestID",
                table: "MachiningChargeMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeMasters_TestMethodStandardID",
                table: "MachiningChargeMasters",
                column: "TestMethodStandardID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeVersions_FinancialYearId",
                table: "MachiningChargeVersions",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeVersions_MachiningChargeMasterID_EffectiveFrom",
                table: "MachiningChargeVersions",
                columns: new[] { "MachiningChargeMasterID", "EffectiveFrom" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_MachiningChargeItems_MachiningChargeMasters_MachiningChargeMasterID",
                table: "MachiningChargeItems",
                column: "MachiningChargeMasterID",
                principalTable: "MachiningChargeMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleInwards_FinancialYears_FinancialYearId",
                table: "SampleInwards",
                column: "FinancialYearId",
                principalTable: "FinancialYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachiningChargeItems_MachiningChargeMasters_MachiningChargeMasterID",
                table: "MachiningChargeItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleInwards_FinancialYears_FinancialYearId",
                table: "SampleInwards");

            migrationBuilder.DropTable(
                name: "CuttingPriceVersions");

            migrationBuilder.DropTable(
                name: "MachiningChargeVersions");

            migrationBuilder.DropTable(
                name: "MachiningChargeMasters");

            migrationBuilder.DropIndex(
                name: "IX_SampleInwards_FinancialYearId",
                table: "SampleInwards");

            migrationBuilder.DropIndex(
                name: "IX_MachiningChargeItems_MachiningChargeMasterID",
                table: "MachiningChargeItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceCases_LaboratoryTestID_EffectiveFrom",
                table: "InvoiceCases");

            migrationBuilder.DropColumn(
                name: "FinancialYearId",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "MachiningChargeMasterID",
                table: "MachiningChargeItems");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "InvoiceCases");

            migrationBuilder.DropColumn(
                name: "SizeRangeMax",
                table: "CuttingPriceMasters");

            migrationBuilder.DropColumn(
                name: "SizeRangeMin",
                table: "CuttingPriceMasters");

            migrationBuilder.AddColumn<decimal>(
                name: "RatePerUnit",
                table: "CuttingPriceMasters",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCases_LaboratoryTestID",
                table: "InvoiceCases",
                column: "LaboratoryTestID");
        }
    }
}
