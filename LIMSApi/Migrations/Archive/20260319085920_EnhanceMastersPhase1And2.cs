using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceMastersPhase1And2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SpecimenOrientationMasters",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SpecimenOrientationMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpecimenOrientationCategoryID",
                table: "SpecimenOrientationMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CalibrationRequired",
                table: "ProductConditionMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ProductConditionMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDestructive",
                table: "ProductConditionMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "LinkedHeatTreatmentID",
                table: "ProductConditionMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductConditionCategoryID",
                table: "ProductConditionMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ParameterMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DecimalPrecision",
                table: "ParameterMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "DefaultTestMethodID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinReportableLimit",
                table: "ParameterMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParameterCategoryID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "ParameterMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "MetalClassificationMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasChemicalParams",
                table: "MetalClassificationMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMechanicalParams",
                table: "MetalClassificationMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ParentID",
                table: "MetalClassificationMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "MetalClassificationMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "HeatTreatmentMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CoolingMediumID",
                table: "HeatTreatmentMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HeatTreatmentCategoryID",
                table: "HeatTreatmentMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TempRangeDescription",
                table: "HeatTreatmentMasters",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TempRangeMax",
                table: "HeatTreatmentMasters",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TempRangeMin",
                table: "HeatTreatmentMasters",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "DimensionalFactorMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultTestMethodID",
                table: "DimensionalFactorMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instrument",
                table: "DimensionalFactorMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToleranceType",
                table: "DimensionalFactorMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "DimensionalFactorMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoolingMediumMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoolingMediumMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HeatTreatmentCategoryMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatTreatmentCategoryMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HeatTreatmentMetalClassifications",
                columns: table => new
                {
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatTreatmentMetalClassifications", x => new { x.HeatTreatmentID, x.MetalClassificationID });
                    table.ForeignKey(
                        name: "FK_HeatTreatmentMetalClassifications_HeatTreatmentMasters_HeatTreatmentID",
                        column: x => x.HeatTreatmentID,
                        principalTable: "HeatTreatmentMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeatTreatmentMetalClassifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParameterCategoryMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterCategoryMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ParameterSpecimenOrientations",
                columns: table => new
                {
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterSpecimenOrientations", x => new { x.ParameterID, x.SpecimenOrientationID });
                    table.ForeignKey(
                        name: "FK_ParameterSpecimenOrientations_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParameterSpecimenOrientations_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductConditionCategoryMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConditionCategoryMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductFormMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFormMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTypeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTypeMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenOrientationCategoryMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenOrientationCategoryMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenOrientationMetalClassifications",
                columns: table => new
                {
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenOrientationMetalClassifications", x => new { x.SpecimenOrientationID, x.MetalClassificationID });
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationMetalClassifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationMetalClassifications_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DimensionalFactorProductForms",
                columns: table => new
                {
                    DimensionalFactorID = table.Column<long>(type: "bigint", nullable: false),
                    ProductFormID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionalFactorProductForms", x => new { x.DimensionalFactorID, x.ProductFormID });
                    table.ForeignKey(
                        name: "FK_DimensionalFactorProductForms_DimensionalFactorMasters_DimensionalFactorID",
                        column: x => x.DimensionalFactorID,
                        principalTable: "DimensionalFactorMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DimensionalFactorProductForms_ProductFormMasters_ProductFormID",
                        column: x => x.ProductFormID,
                        principalTable: "ProductFormMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenOrientationProductForms",
                columns: table => new
                {
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: false),
                    ProductFormID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenOrientationProductForms", x => new { x.SpecimenOrientationID, x.ProductFormID });
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationProductForms_ProductFormMasters_ProductFormID",
                        column: x => x.ProductFormID,
                        principalTable: "ProductFormMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationProductForms_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductConditionPropertyTypes",
                columns: table => new
                {
                    ProductConditionID = table.Column<long>(type: "bigint", nullable: false),
                    PropertyTypeID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConditionPropertyTypes", x => new { x.ProductConditionID, x.PropertyTypeID });
                    table.ForeignKey(
                        name: "FK_ProductConditionPropertyTypes_ProductConditionMasters_ProductConditionID",
                        column: x => x.ProductConditionID,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductConditionPropertyTypes_PropertyTypeMasters_PropertyTypeID",
                        column: x => x.PropertyTypeID,
                        principalTable: "PropertyTypeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenOrientationMaster_Code",
                table: "SpecimenOrientationMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenOrientationMasters_SpecimenOrientationCategoryID",
                table: "SpecimenOrientationMasters",
                column: "SpecimenOrientationCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConditionMaster_Code",
                table: "ProductConditionMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConditionMasters_LinkedHeatTreatmentID",
                table: "ProductConditionMasters",
                column: "LinkedHeatTreatmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConditionMasters_ProductConditionCategoryID",
                table: "ProductConditionMasters",
                column: "ProductConditionCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMaster_Code",
                table: "ParameterMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_DefaultTestMethodID",
                table: "ParameterMasters",
                column: "DefaultTestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_ParameterCategoryID",
                table: "ParameterMasters",
                column: "ParameterCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationMaster_Code",
                table: "MetalClassificationMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationMasters_ParentID",
                table: "MetalClassificationMasters",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentMaster_Code",
                table: "HeatTreatmentMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentMasters_CoolingMediumID",
                table: "HeatTreatmentMasters",
                column: "CoolingMediumID");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentMasters_HeatTreatmentCategoryID",
                table: "HeatTreatmentMasters",
                column: "HeatTreatmentCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorMaster_Code",
                table: "DimensionalFactorMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorMasters_DefaultTestMethodID",
                table: "DimensionalFactorMasters",
                column: "DefaultTestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorProductForms_ProductFormID",
                table: "DimensionalFactorProductForms",
                column: "ProductFormID");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentMetalClassifications_MetalClassificationID",
                table: "HeatTreatmentMetalClassifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterSpecimenOrientations_SpecimenOrientationID",
                table: "ParameterSpecimenOrientations",
                column: "SpecimenOrientationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConditionPropertyTypes_PropertyTypeID",
                table: "ProductConditionPropertyTypes",
                column: "PropertyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenOrientationMetalClassifications_MetalClassificationID",
                table: "SpecimenOrientationMetalClassifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenOrientationProductForms_ProductFormID",
                table: "SpecimenOrientationProductForms",
                column: "ProductFormID");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodStandards_DefaultTestMethodID",
                table: "DimensionalFactorMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodStandards",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HeatTreatmentMasters_CoolingMediumMasters_CoolingMediumID",
                table: "HeatTreatmentMasters",
                column: "CoolingMediumID",
                principalTable: "CoolingMediumMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HeatTreatmentMasters_HeatTreatmentCategoryMasters_HeatTreatmentCategoryID",
                table: "HeatTreatmentMasters",
                column: "HeatTreatmentCategoryID",
                principalTable: "HeatTreatmentCategoryMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationMasters_MetalClassificationMasters_ParentID",
                table: "MetalClassificationMasters",
                column: "ParentID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_ParameterCategoryMasters_ParameterCategoryID",
                table: "ParameterMasters",
                column: "ParameterCategoryID",
                principalTable: "ParameterCategoryMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_TestMethodStandards_DefaultTestMethodID",
                table: "ParameterMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodStandards",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConditionMasters_HeatTreatmentMasters_LinkedHeatTreatmentID",
                table: "ProductConditionMasters",
                column: "LinkedHeatTreatmentID",
                principalTable: "HeatTreatmentMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConditionMasters_ProductConditionCategoryMasters_ProductConditionCategoryID",
                table: "ProductConditionMasters",
                column: "ProductConditionCategoryID",
                principalTable: "ProductConditionCategoryMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecimenOrientationMasters_SpecimenOrientationCategoryMasters_SpecimenOrientationCategoryID",
                table: "SpecimenOrientationMasters",
                column: "SpecimenOrientationCategoryID",
                principalTable: "SpecimenOrientationCategoryMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodStandards_DefaultTestMethodID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_HeatTreatmentMasters_CoolingMediumMasters_CoolingMediumID",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_HeatTreatmentMasters_HeatTreatmentCategoryMasters_HeatTreatmentCategoryID",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationMasters_MetalClassificationMasters_ParentID",
                table: "MetalClassificationMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_ParameterCategoryMasters_ParameterCategoryID",
                table: "ParameterMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_TestMethodStandards_DefaultTestMethodID",
                table: "ParameterMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductConditionMasters_HeatTreatmentMasters_LinkedHeatTreatmentID",
                table: "ProductConditionMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductConditionMasters_ProductConditionCategoryMasters_ProductConditionCategoryID",
                table: "ProductConditionMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecimenOrientationMasters_SpecimenOrientationCategoryMasters_SpecimenOrientationCategoryID",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropTable(
                name: "CoolingMediumMasters");

            migrationBuilder.DropTable(
                name: "DimensionalFactorProductForms");

            migrationBuilder.DropTable(
                name: "HeatTreatmentCategoryMasters");

            migrationBuilder.DropTable(
                name: "HeatTreatmentMetalClassifications");

            migrationBuilder.DropTable(
                name: "ParameterCategoryMasters");

            migrationBuilder.DropTable(
                name: "ParameterSpecimenOrientations");

            migrationBuilder.DropTable(
                name: "ProductConditionCategoryMasters");

            migrationBuilder.DropTable(
                name: "ProductConditionPropertyTypes");

            migrationBuilder.DropTable(
                name: "SpecimenOrientationCategoryMasters");

            migrationBuilder.DropTable(
                name: "SpecimenOrientationMetalClassifications");

            migrationBuilder.DropTable(
                name: "SpecimenOrientationProductForms");

            migrationBuilder.DropTable(
                name: "PropertyTypeMasters");

            migrationBuilder.DropTable(
                name: "ProductFormMasters");

            migrationBuilder.DropIndex(
                name: "IX_SpecimenOrientationMaster_Code",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropIndex(
                name: "IX_SpecimenOrientationMasters_SpecimenOrientationCategoryID",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductConditionMaster_Code",
                table: "ProductConditionMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductConditionMasters_LinkedHeatTreatmentID",
                table: "ProductConditionMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductConditionMasters_ProductConditionCategoryID",
                table: "ProductConditionMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMaster_Code",
                table: "ParameterMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMasters_DefaultTestMethodID",
                table: "ParameterMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMasters_ParameterCategoryID",
                table: "ParameterMasters");

            migrationBuilder.DropIndex(
                name: "IX_MetalClassificationMaster_Code",
                table: "MetalClassificationMasters");

            migrationBuilder.DropIndex(
                name: "IX_MetalClassificationMasters_ParentID",
                table: "MetalClassificationMasters");

            migrationBuilder.DropIndex(
                name: "IX_HeatTreatmentMaster_Code",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropIndex(
                name: "IX_HeatTreatmentMasters_CoolingMediumID",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropIndex(
                name: "IX_HeatTreatmentMasters_HeatTreatmentCategoryID",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropIndex(
                name: "IX_DimensionalFactorMaster_Code",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropIndex(
                name: "IX_DimensionalFactorMasters_DefaultTestMethodID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropColumn(
                name: "SpecimenOrientationCategoryID",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropColumn(
                name: "CalibrationRequired",
                table: "ProductConditionMasters");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ProductConditionMasters");

            migrationBuilder.DropColumn(
                name: "IsDestructive",
                table: "ProductConditionMasters");

            migrationBuilder.DropColumn(
                name: "LinkedHeatTreatmentID",
                table: "ProductConditionMasters");

            migrationBuilder.DropColumn(
                name: "ProductConditionCategoryID",
                table: "ProductConditionMasters");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "DecimalPrecision",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "DefaultTestMethodID",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "MinReportableLimit",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "ParameterCategoryID",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "MetalClassificationMasters");

            migrationBuilder.DropColumn(
                name: "HasChemicalParams",
                table: "MetalClassificationMasters");

            migrationBuilder.DropColumn(
                name: "HasMechanicalParams",
                table: "MetalClassificationMasters");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "MetalClassificationMasters");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "MetalClassificationMasters");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropColumn(
                name: "CoolingMediumID",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropColumn(
                name: "HeatTreatmentCategoryID",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropColumn(
                name: "TempRangeDescription",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropColumn(
                name: "TempRangeMax",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropColumn(
                name: "TempRangeMin",
                table: "HeatTreatmentMasters");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "DefaultTestMethodID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "Instrument",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "ToleranceType",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "DimensionalFactorMasters");
        }
    }
}
