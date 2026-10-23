using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLabTestProfileArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubGroup",
                table: "LaboratoryTests");

            migrationBuilder.AddColumn<bool>(
                name: "IsMechanical",
                table: "LaboratoryTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LaboratoryTestTechniques",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
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
                    table.PrimaryKey("PK_LaboratoryTestTechniques", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestTechniques_AnalysisTechniqueMasters_AnalysisTechniqueID",
                        column: x => x.AnalysisTechniqueID,
                        principalTable: "AnalysisTechniqueMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestTechniques_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestProfiles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestTechniqueID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    InvoiceCaption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestProfiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfiles_LaboratoryTestTechniques_LaboratoryTestTechniqueID",
                        column: x => x.LaboratoryTestTechniqueID,
                        principalTable: "LaboratoryTestTechniques",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestPricingConfigurations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
                    PricingRuleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true),
                    InvoiceCaseID = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    FormulaExpression = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestPricingConfigurations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestPricingConfigurations_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestPricingConfigurations_InvoiceCases_InvoiceCaseID",
                        column: x => x.InvoiceCaseID,
                        principalTable: "InvoiceCases",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestPricingConfigurations_LaboratoryTestProfiles_LaboratoryTestProfileID",
                        column: x => x.LaboratoryTestProfileID,
                        principalTable: "LaboratoryTestProfiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestProfileInvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LaboratoryTestProfileInvoiceCases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileInvoiceCases_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileInvoiceCases_LaboratoryTestProfiles_LaboratoryTestProfileID",
                        column: x => x.LaboratoryTestProfileID,
                        principalTable: "LaboratoryTestProfiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestProfileMachines",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestProfileMachines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileMachines_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileMachines_LaboratoryTestProfiles_LaboratoryTestProfileID",
                        column: x => x.LaboratoryTestProfileID,
                        principalTable: "LaboratoryTestProfiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestProfileMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestProfileMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileMethods_LaboratoryTestProfiles_LaboratoryTestProfileID",
                        column: x => x.LaboratoryTestProfileID,
                        principalTable: "LaboratoryTestProfiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestProfileParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestProfileParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileParameters_LaboratoryTestProfiles_LaboratoryTestProfileID",
                        column: x => x.LaboratoryTestProfileID,
                        principalTable: "LaboratoryTestProfiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestProfileSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    ProductSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestProfileSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileSpecifications_LaboratoryTestProfiles_LaboratoryTestProfileID",
                        column: x => x.LaboratoryTestProfileID,
                        principalTable: "LaboratoryTestProfiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileSpecifications_ProductSpecifications_ProductSpecificationID",
                        column: x => x.ProductSpecificationID,
                        principalTable: "ProductSpecifications",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileSpecifications_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestProfileInvoiceCasePrices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestProfileInvoiceCaseID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestProfileInvoiceCasePrices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileInvoiceCasePrices_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestProfileInvoiceCasePrices_LaboratoryTestProfileInvoiceCases_LaboratoryTestProfileInvoiceCaseID",
                        column: x => x.LaboratoryTestProfileInvoiceCaseID,
                        principalTable: "LaboratoryTestProfileInvoiceCases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestPricingConfigurations_InvoiceCaseConfigID",
                table: "LaboratoryTestPricingConfigurations",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestPricingConfigurations_InvoiceCaseID",
                table: "LaboratoryTestPricingConfigurations",
                column: "InvoiceCaseID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestPricingConfigurations_LaboratoryTestProfileID",
                table: "LaboratoryTestPricingConfigurations",
                column: "LaboratoryTestProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileInvoiceCasePrices_InvoiceCaseConfigID",
                table: "LaboratoryTestProfileInvoiceCasePrices",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileInvoiceCasePrices_LaboratoryTestProfileInvoiceCaseID",
                table: "LaboratoryTestProfileInvoiceCasePrices",
                column: "LaboratoryTestProfileInvoiceCaseID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileInvoiceCases_FinancialYearId",
                table: "LaboratoryTestProfileInvoiceCases",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileInvoiceCases_LaboratoryTestProfileID_EffectiveFrom",
                table: "LaboratoryTestProfileInvoiceCases",
                columns: new[] { "LaboratoryTestProfileID", "EffectiveFrom" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileMachines_EquipmentID",
                table: "LaboratoryTestProfileMachines",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileMachines_LaboratoryTestProfileID",
                table: "LaboratoryTestProfileMachines",
                column: "LaboratoryTestProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileMethods_LaboratoryTestProfileID",
                table: "LaboratoryTestProfileMethods",
                column: "LaboratoryTestProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileMethods_TestMethodSpecificationID",
                table: "LaboratoryTestProfileMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileParameters_LaboratoryTestProfileID",
                table: "LaboratoryTestProfileParameters",
                column: "LaboratoryTestProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileParameters_ParameterID",
                table: "LaboratoryTestProfileParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfiles_LaboratoryTestTechniqueID",
                table: "LaboratoryTestProfiles",
                column: "LaboratoryTestTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileSpecifications_LaboratoryTestProfileID",
                table: "LaboratoryTestProfileSpecifications",
                column: "LaboratoryTestProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileSpecifications_ProductSpecificationID",
                table: "LaboratoryTestProfileSpecifications",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestProfileSpecifications_SpecificationHeaderID",
                table: "LaboratoryTestProfileSpecifications",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestTechniques_AnalysisTechniqueID",
                table: "LaboratoryTestTechniques",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestTechniques_LaboratoryTestSubGroupID",
                table: "LaboratoryTestTechniques",
                column: "LaboratoryTestSubGroupID");

            // Custom SQL Data Migration
            migrationBuilder.Sql(@"
                -- 1. Create LaboratoryTestTechnique records for existing SubTypes
                INSERT INTO LaboratoryTestTechniques (LaboratoryTestSubGroupID, AnalysisTechniqueID, Label, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                SELECT DISTINCT 
                    st.LaboratoryTestSubGroupID, 
                    st.AnalysisTechniqueID,
                    COALESCE((SELECT Name FROM AnalysisTechniqueMasters WHERE ID = st.AnalysisTechniqueID), 'Manual'),
                    0, 
                    0, 
                    GETUTCDATE(), 
                    st.CompanyCode, 
                    1
                FROM LaboratoryTestSubTypes st
                WHERE st.IsActive = 1
                  AND NOT EXISTS (
                      SELECT 1 FROM LaboratoryTestTechniques 
                      WHERE LaboratoryTestSubGroupID = st.LaboratoryTestSubGroupID 
                        AND (AnalysisTechniqueID = st.AnalysisTechniqueID OR (AnalysisTechniqueID IS NULL AND st.AnalysisTechniqueID IS NULL))
                  );

                -- 2. Migrate LaboratoryTestSubTypes into LaboratoryTestProfiles
                INSERT INTO LaboratoryTestProfiles (LaboratoryTestTechniqueID, Name, InvoiceCaption, Description, DisplayOrder, IsDefault, CreatedBy, CreatedOn, CompanyCode, IsActive)
                SELECT 
                    t.ID,
                    st.Name,
                    st.InvoiceCaption,
                    'Migrated from SubType',
                    st.DisplayOrder,
                    1,
                    0,
                    GETUTCDATE(),
                    st.CompanyCode,
                    1
                FROM LaboratoryTestSubTypes st
                JOIN LaboratoryTestTechniques t ON t.LaboratoryTestSubGroupID = st.LaboratoryTestSubGroupID 
                  AND (t.AnalysisTechniqueID = st.AnalysisTechniqueID OR (t.AnalysisTechniqueID IS NULL AND st.AnalysisTechniqueID IS NULL))
                WHERE st.IsActive = 1;

                -- 3. Migrate SubType Invoice Cases to Profile Invoice Cases
                INSERT INTO LaboratoryTestProfileInvoiceCases (LaboratoryTestProfileID, FinancialYearId, EffectiveFrom, DefaultPricingType, CreatedBy, CreatedOn, CompanyCode, IsActive)
                SELECT 
                    p.ID,
                    ic.FinancialYearId,
                    ic.EffectiveFrom,
                    ic.DefaultPricingType,
                    ic.CreatedBy,
                    ic.CreatedOn,
                    ic.CompanyCode,
                    ic.IsActive
                FROM LaboratoryTestSubTypeInvoiceCases ic
                JOIN LaboratoryTestSubTypes st ON st.ID = ic.LaboratoryTestSubTypeID
                JOIN LaboratoryTestTechniques t ON t.LaboratoryTestSubGroupID = st.LaboratoryTestSubGroupID 
                  AND (t.AnalysisTechniqueID = st.AnalysisTechniqueID OR (t.AnalysisTechniqueID IS NULL AND st.AnalysisTechniqueID IS NULL))
                JOIN LaboratoryTestProfiles p ON p.LaboratoryTestTechniqueID = t.ID AND p.Name = st.Name
                WHERE ic.IsActive = 1;

                -- 4. Migrate SubType Invoice Case Prices
                INSERT INTO LaboratoryTestProfileInvoiceCasePrices (LaboratoryTestProfileInvoiceCaseID, Name, Price, InvoiceCaseConfigID)
                SELECT 
                    pic.ID,
                    pr.Name,
                    pr.Price,
                    pr.InvoiceCaseConfigID
                FROM LaboratoryTestSubTypeInvoiceCasePrices pr
                JOIN LaboratoryTestSubTypeInvoiceCases ic ON ic.ID = pr.LaboratoryTestSubTypeInvoiceCaseID
                JOIN LaboratoryTestSubTypes st ON st.ID = ic.LaboratoryTestSubTypeID
                JOIN LaboratoryTestTechniques t ON t.LaboratoryTestSubGroupID = st.LaboratoryTestSubGroupID 
                  AND (t.AnalysisTechniqueID = st.AnalysisTechniqueID OR (t.AnalysisTechniqueID IS NULL AND st.AnalysisTechniqueID IS NULL))
                JOIN LaboratoryTestProfiles p ON p.LaboratoryTestTechniqueID = t.ID AND p.Name = st.Name
                JOIN LaboratoryTestProfileInvoiceCases pic ON pic.LaboratoryTestProfileID = p.ID AND pic.EffectiveFrom = ic.EffectiveFrom;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaboratoryTestPricingConfigurations");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfileInvoiceCasePrices");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfileMachines");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfileMethods");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfileParameters");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfileSpecifications");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfileInvoiceCases");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfiles");

            migrationBuilder.DropTable(
                name: "LaboratoryTestTechniques");

            migrationBuilder.DropColumn(
                name: "IsMechanical",
                table: "LaboratoryTests");

            migrationBuilder.AddColumn<string>(
                name: "SubGroup",
                table: "LaboratoryTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
