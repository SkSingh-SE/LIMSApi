using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RestructureLabTestHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTests_LaboratoryTestSubTypes_LaboratoryTestSubTypeID",
                table: "ChemicalTests");

            migrationBuilder.DropTable(
                name: "LaboratoryTestInvoiceCase");

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
                name: "LaboratoryTestSubTypeInvoiceCasePrices");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfileInvoiceCases");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubTypeInvoiceCases");

            migrationBuilder.DropTable(
                name: "LaboratoryTestProfiles");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubTypes");

            migrationBuilder.DropTable(
                name: "LaboratoryTestTechniques");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTests_LaboratoryTestSubTypeID",
                table: "ChemicalTests");

            migrationBuilder.DropColumn(
                name: "ChemicalCategory",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "InvoiceCaption",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "TestCaption",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestSubTypeID",
                table: "ChemicalTests");

            migrationBuilder.RenameColumn(
                name: "SamplePlanID",
                table: "ChemicalTests",
                newName: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.AddColumn<string>(
                name: "ReportTestName",
                table: "LaboratoryTestSubGroups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TestDuration",
                table: "LaboratoryTestSubGroups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestSubGroupID",
                table: "GeneralTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TestDuration = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypes_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypes_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupEquipments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupEquipments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupEquipments_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupEquipments_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupInvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupInvoiceCases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupInvoiceCases_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupInvoiceCases_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupMethods_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupParameters_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    ProductSpecificationID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupSpecifications_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupSpecifications_ProductSpecifications_ProductSpecificationID",
                        column: x => x.ProductSpecificationID,
                        principalTable: "ProductSpecifications",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupSpecifications_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeEquipments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeEquipments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeEquipments_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeEquipments_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeInvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeInvoiceCases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeInvoiceCases_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeInvoiceCases_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeMethods_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeParameters_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    ProductSpecificationID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeSpecifications_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeSpecifications_ProductSpecifications_ProductSpecificationID",
                        column: x => x.ProductSpecificationID,
                        principalTable: "ProductSpecifications",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeSpecifications_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeTechniques",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeTechniques", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeTechniques_AnalysisTechniqueMasters_AnalysisTechniqueID",
                        column: x => x.AnalysisTechniqueID,
                        principalTable: "AnalysisTechniqueMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeTechniques_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTests_LaboratoryTestSubGroupID",
                table: "GeneralTests",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTests_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTests",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeEquipments_EquipmentID",
                table: "LaboratoryTestAnalysisTypeEquipments",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeEquipments_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeEquipments",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeInvoiceCases_InvoiceCaseConfigID",
                table: "LaboratoryTestAnalysisTypeInvoiceCases",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeInvoiceCases_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeInvoiceCases",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeMethods_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeMethods",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeMethods_TestMethodSpecificationID",
                table: "LaboratoryTestAnalysisTypeMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeParameters_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeParameters",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeParameters_ParameterID",
                table: "LaboratoryTestAnalysisTypeParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypes_LaboratoryTestSubGroupID",
                table: "LaboratoryTestAnalysisTypes",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypes_MetalClassificationID",
                table: "LaboratoryTestAnalysisTypes",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_ProductSpecificationID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_SpecificationHeaderID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeTechniques_AnalysisTechniqueID",
                table: "LaboratoryTestAnalysisTypeTechniques",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeTechniques_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeTechniques",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupEquipments_EquipmentID",
                table: "LaboratoryTestSubGroupEquipments",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupEquipments_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupEquipments",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupInvoiceCases_InvoiceCaseConfigID",
                table: "LaboratoryTestSubGroupInvoiceCases",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupInvoiceCases_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupInvoiceCases",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupMethods_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupMethods",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupMethods_TestMethodSpecificationID",
                table: "LaboratoryTestSubGroupMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupParameters_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupParameters",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupParameters_ParameterID",
                table: "LaboratoryTestSubGroupParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_ProductSpecificationID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_SpecificationHeaderID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "SpecificationHeaderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTests_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTests",
                column: "LaboratoryTestAnalysisTypeID",
                principalTable: "LaboratoryTestAnalysisTypes",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTests_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                table: "GeneralTests",
                column: "LaboratoryTestSubGroupID",
                principalTable: "LaboratoryTestSubGroups",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTests_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTests");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTests_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                table: "GeneralTests");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeEquipments");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeInvoiceCases");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeMethods");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeParameters");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeSpecifications");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeTechniques");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupEquipments");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupInvoiceCases");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupMethods");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupParameters");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupSpecifications");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypes");

            migrationBuilder.DropIndex(
                name: "IX_GeneralTests_LaboratoryTestSubGroupID",
                table: "GeneralTests");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTests_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTests");

            migrationBuilder.DropColumn(
                name: "ReportTestName",
                table: "LaboratoryTestSubGroups");

            migrationBuilder.DropColumn(
                name: "TestDuration",
                table: "LaboratoryTestSubGroups");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestSubGroupID",
                table: "GeneralTests");

            migrationBuilder.RenameColumn(
                name: "LaboratoryTestAnalysisTypeID",
                table: "ChemicalTests",
                newName: "SamplePlanID");

            migrationBuilder.AddColumn<string>(
                name: "ChemicalCategory",
                table: "LaboratoryTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCaption",
                table: "LaboratoryTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestCaption",
                table: "LaboratoryTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestSubTypeID",
                table: "ChemicalTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboratoryTestInvoiceCase",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: false),
                    LabTestID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestInvoiceCase", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestInvoiceCase_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestInvoiceCase_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    InvoiceCaption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PricingRuleType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
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
                        name: "FK_LaboratoryTestSubTypes_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubTypes_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestTechniques",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "LaboratoryTestSubTypeInvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestSubTypeID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DefaultPricingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "LaboratoryTestProfiles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestTechniqueID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    InvoiceCaption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
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
                name: "LaboratoryTestSubTypeInvoiceCasePrices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestSubTypeInvoiceCaseID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "LaboratoryTestPricingConfigurations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true),
                    InvoiceCaseID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormulaExpression = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PricingRuleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
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
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DefaultPricingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestProfileID = table.Column<long>(type: "bigint", nullable: false),
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
                    ProductSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
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
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestProfileInvoiceCaseID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                name: "IX_ChemicalTests_LaboratoryTestSubTypeID",
                table: "ChemicalTests",
                column: "LaboratoryTestSubTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestInvoiceCase_InvoiceCaseConfigID",
                table: "LaboratoryTestInvoiceCase",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestInvoiceCase_LaboratoryTestID",
                table: "LaboratoryTestInvoiceCase",
                column: "LaboratoryTestID");

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
                name: "IX_LaboratoryTestSubTypes_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubTypes",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubTypes_MetalClassificationID",
                table: "LaboratoryTestSubTypes",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestTechniques_AnalysisTechniqueID",
                table: "LaboratoryTestTechniques",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestTechniques_LaboratoryTestSubGroupID",
                table: "LaboratoryTestTechniques",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTests_LaboratoryTestSubTypes_LaboratoryTestSubTypeID",
                table: "ChemicalTests",
                column: "LaboratoryTestSubTypeID",
                principalTable: "LaboratoryTestSubTypes",
                principalColumn: "ID");
        }
    }
}
