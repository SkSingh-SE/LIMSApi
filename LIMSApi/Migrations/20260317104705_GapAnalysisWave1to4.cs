using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class GapAnalysisWave1to4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParameterType",
                table: "TestResultParameters",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestMethodUsed",
                table: "TestResultParameters",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlacklistDate",
                table: "SupplierMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor1",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor2",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor3",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit1",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit2",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit3",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestMasterID",
                table: "LaboratoryTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParameterType",
                table: "LaboratoryTestParameter",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinExperience",
                table: "DesignationMasters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalityTraits",
                table: "DesignationMasters",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Qualification",
                table: "DesignationMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RolesAndResponsibilities",
                table: "DesignationMasters",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquipmentReferenceMaterials",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentMasterID = table.Column<long>(type: "bigint", nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LotNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_EquipmentReferenceMaterials", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentReferenceMaterials_EquipmentMasters_EquipmentMasterID",
                        column: x => x.EquipmentMasterID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "HardnessEquivalences",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HardnessScale = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IndenterSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoadKgf = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FromValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EquivalentScale = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EquivalentFromValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EquivalentToValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Standard = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardnessEquivalences", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLineItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProformaInvoiceHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    TaxInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    SampleInwardID = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_InvoiceLineItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_ProformaInvoiceHeader_ProformaInvoiceHeaderID",
                        column: x => x.ProformaInvoiceHeaderID,
                        principalTable: "ProformaInvoiceHeader",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_SampleInwards_SampleInwardID",
                        column: x => x.SampleInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_TaxInvoices_TaxInvoiceID",
                        column: x => x.TaxInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MachiningChargeItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_MachiningChargeItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MachiningChargeItems_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecificationGrades",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: false),
                    AliasName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecificationGrades", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductSpecificationGrades_ProductSpecifications_ProductSpecificationID",
                        column: x => x.ProductSpecificationID,
                        principalTable: "ProductSpecifications",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductSpecificationGrades_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProductTestGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodStandardID = table.Column<long>(type: "bigint", nullable: true),
                    IsPerBatch = table.Column<bool>(type: "bit", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ProductTestGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductTestGroups_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductTestGroups_ProductSpecifications_ProductSpecificationID",
                        column: x => x.ProductSpecificationID,
                        principalTable: "ProductSpecifications",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductTestGroups_TestMethodStandards_TestMethodStandardID",
                        column: x => x.TestMethodStandardID,
                        principalTable: "TestMethodStandards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerPurchaseOrderID = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BilledAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_CustomerPurchaseOrders_CustomerPurchaseOrderID",
                        column: x => x.CustomerPurchaseOrderID,
                        principalTable: "CustomerPurchaseOrders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SamplePreparationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodStandardID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    SpecimenType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Dimensions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaterialType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_SamplePreparationMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SamplePreparationMasters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparationMasters_TestMethodStandards_TestMethodStandardID",
                        column: x => x.TestMethodStandardID,
                        principalTable: "TestMethodStandards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ToleranceMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterID = table.Column<long>(type: "bigint", nullable: true),
                    StandardName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValueRangeStart = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ValueRangeEnd = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Tolerance = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ToleranceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_ToleranceMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToleranceMasters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ToleranceMasters_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TpiInspections",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleInwardID = table.Column<long>(type: "bigint", nullable: false),
                    SampleDetailID = table.Column<long>(type: "bigint", nullable: true),
                    TPIMasterID = table.Column<long>(type: "bigint", nullable: false),
                    Stage = table.Column<string>(type: "varchar(30)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    InspectorComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TpiInspections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TpiInspections_SampleDetails_SampleDetailID",
                        column: x => x.SampleDetailID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TpiInspections_SampleInwards_SampleInwardID",
                        column: x => x.SampleInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TpiInspections_TPIMasters_TPIMasterID",
                        column: x => x.TPIMasterID,
                        principalTable: "TPIMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_TestMasterID",
                table: "LaboratoryTests",
                column: "TestMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReferenceMaterials_EquipmentMasterID",
                table: "EquipmentReferenceMaterials",
                column: "EquipmentMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_ProformaInvoiceHeaderID",
                table: "InvoiceLineItems",
                column: "ProformaInvoiceHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_SampleInwardID",
                table: "InvoiceLineItems",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_TaxInvoiceID",
                table: "InvoiceLineItems",
                column: "TaxInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeItems_SampleID",
                table: "MachiningChargeItems",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationGrades_ProductSpecificationID",
                table: "ProductSpecificationGrades",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationGrades_SpecificationGradeID",
                table: "ProductSpecificationGrades",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTestGroups_LaboratoryTestID",
                table: "ProductTestGroups",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTestGroups_ProductSpecificationID",
                table: "ProductTestGroups",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTestGroups_TestMethodStandardID",
                table: "ProductTestGroups",
                column: "TestMethodStandardID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_CustomerPurchaseOrderID",
                table: "PurchaseOrderItems",
                column: "CustomerPurchaseOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparationMasters_LaboratoryTestID",
                table: "SamplePreparationMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparationMasters_TestMethodStandardID",
                table: "SamplePreparationMasters",
                column: "TestMethodStandardID");

            migrationBuilder.CreateIndex(
                name: "IX_ToleranceMasters_ParameterID",
                table: "ToleranceMasters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_ToleranceMasters_SpecificationHeaderID",
                table: "ToleranceMasters",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TpiInspections_SampleDetailID",
                table: "TpiInspections",
                column: "SampleDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_TpiInspections_SampleInwardID",
                table: "TpiInspections",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_TpiInspections_TPIMasterID",
                table: "TpiInspections",
                column: "TPIMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTests_TestMasters_TestMasterID",
                table: "LaboratoryTests",
                column: "TestMasterID",
                principalTable: "TestMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTests_TestMasters_TestMasterID",
                table: "LaboratoryTests");

            migrationBuilder.DropTable(
                name: "EquipmentReferenceMaterials");

            migrationBuilder.DropTable(
                name: "HardnessEquivalences");

            migrationBuilder.DropTable(
                name: "InvoiceLineItems");

            migrationBuilder.DropTable(
                name: "MachiningChargeItems");

            migrationBuilder.DropTable(
                name: "ProductSpecificationGrades");

            migrationBuilder.DropTable(
                name: "ProductTestGroups");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "SamplePreparationMasters");

            migrationBuilder.DropTable(
                name: "ToleranceMasters");

            migrationBuilder.DropTable(
                name: "TpiInspections");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTests_TestMasterID",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "ParameterType",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "TestMethodUsed",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "BlacklistDate",
                table: "SupplierMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor1",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor2",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor3",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit1",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit2",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit3",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "TestMasterID",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "ParameterType",
                table: "LaboratoryTestParameter");

            migrationBuilder.DropColumn(
                name: "MinExperience",
                table: "DesignationMasters");

            migrationBuilder.DropColumn(
                name: "PersonalityTraits",
                table: "DesignationMasters");

            migrationBuilder.DropColumn(
                name: "Qualification",
                table: "DesignationMasters");

            migrationBuilder.DropColumn(
                name: "RolesAndResponsibilities",
                table: "DesignationMasters");
        }
    }
}
