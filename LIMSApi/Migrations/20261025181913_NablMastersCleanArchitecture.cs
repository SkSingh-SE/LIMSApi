using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablMastersCleanArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TestLayoutTemplates') AND name = 'TestMethodName')
    ALTER TABLE TestLayoutTemplates ADD TestMethodName nvarchar(250) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TestLayoutTemplates') AND name = 'TestMethodSpecificationID')
    ALTER TABLE TestLayoutTemplates ADD TestMethodSpecificationID bigint NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TestLayoutTemplateMappings') AND name = 'TestMethodName')
    ALTER TABLE TestLayoutTemplateMappings ADD TestMethodName nvarchar(250) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TestLayoutTemplateMappings') AND name = 'TestMethodSpecificationID')
    ALTER TABLE TestLayoutTemplateMappings ADD TestMethodSpecificationID bigint NULL;
");

            migrationBuilder.AddColumn<string>(
                name: "CrackOrNotchDirection",
                table: "SpecimenOrientationMasters",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LengthAxis",
                table: "SpecimenOrientationMasters",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotationType",
                table: "SpecimenOrientationMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StandardNotation",
                table: "SpecimenOrientationMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductConditionMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultEquipmentTypeID",
                table: "DimensionalFactorMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactorType",
                table: "DimensionalFactorMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DimensionalFactorShapeFormulas",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DimensionalFactorID = table.Column<long>(type: "bigint", nullable: false),
                    SpecimenTypeID = table.Column<long>(type: "bigint", nullable: true),
                    SpecimenShape = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FormulaExpression = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionalFactorShapeFormulas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DimensionalFactorShapeFormulas_DimensionalFactorMasters_DimensionalFactorID",
                        column: x => x.DimensionalFactorID,
                        principalTable: "DimensionalFactorMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DimensionalFactorShapeFormulas_SpecimenTypeMasters_SpecimenTypeID",
                        column: x => x.SpecimenTypeID,
                        principalTable: "SpecimenTypeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "HeatTreatmentStages",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: false),
                    StageNumber = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TempRangeMin = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TempRangeMax = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SoakingDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CoolingMediumID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatTreatmentStages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HeatTreatmentStages_CoolingMediumMasters_CoolingMediumID",
                        column: x => x.CoolingMediumID,
                        principalTable: "CoolingMediumMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_HeatTreatmentStages_HeatTreatmentMasters_HeatTreatmentID",
                        column: x => x.HeatTreatmentID,
                        principalTable: "HeatTreatmentMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorMasters_DefaultEquipmentTypeID",
                table: "DimensionalFactorMasters",
                column: "DefaultEquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorShapeFormulas_DimensionalFactorID",
                table: "DimensionalFactorShapeFormulas",
                column: "DimensionalFactorID");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorShapeFormulas_SpecimenTypeID",
                table: "DimensionalFactorShapeFormulas",
                column: "SpecimenTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentStages_CoolingMediumID",
                table: "HeatTreatmentStages",
                column: "CoolingMediumID");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentStages_HeatTreatmentID",
                table: "HeatTreatmentStages",
                column: "HeatTreatmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionalFactorMasters_EquipmentTypeMasters_DefaultEquipmentTypeID",
                table: "DimensionalFactorMasters",
                column: "DefaultEquipmentTypeID",
                principalTable: "EquipmentTypeMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionalFactorMasters_EquipmentTypeMasters_DefaultEquipmentTypeID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropTable(
                name: "DimensionalFactorShapeFormulas");

            migrationBuilder.DropTable(
                name: "HeatTreatmentStages");

            migrationBuilder.DropIndex(
                name: "IX_DimensionalFactorMasters_DefaultEquipmentTypeID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "TestMethodName",
                table: "TestLayoutTemplates");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationID",
                table: "TestLayoutTemplates");

            migrationBuilder.DropColumn(
                name: "TestMethodName",
                table: "TestLayoutTemplateMappings");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationID",
                table: "TestLayoutTemplateMappings");

            migrationBuilder.DropColumn(
                name: "CrackOrNotchDirection",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropColumn(
                name: "LengthAxis",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropColumn(
                name: "NotationType",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropColumn(
                name: "StandardNotation",
                table: "SpecimenOrientationMasters");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductConditionMasters");

            migrationBuilder.DropColumn(
                name: "DefaultEquipmentTypeID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "FactorType",
                table: "DimensionalFactorMasters");
        }
    }
}
