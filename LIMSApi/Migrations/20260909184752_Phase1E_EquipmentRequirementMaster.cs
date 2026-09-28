using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase1E_EquipmentRequirementMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentRequirementMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterID = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentTypeID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: true),
                    RequiredCapability = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinimumRange = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    MaximumRange = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    RangeUnitID = table.Column<long>(type: "bigint", nullable: true),
                    AccuracyRequirement = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResolutionRequirement = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRequirementMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentRequirementMasters_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentRequirementMasters_EquipmentTypeMasters_EquipmentTypeID",
                        column: x => x.EquipmentTypeID,
                        principalTable: "EquipmentTypeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentRequirementMasters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentRequirementMasters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentRequirementMasters_ParameterUnitMasters_RangeUnitID",
                        column: x => x.RangeUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentRequirementMasters_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentRequirementMasters_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FactorConversionMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FactorType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FactorValue = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    InputParameterID = table.Column<long>(type: "bigint", nullable: false),
                    OutputParameterID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true),
                    AppliedOn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactorConversionMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FactorConversionMasters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FactorConversionMasters_ParameterMasters_InputParameterID",
                        column: x => x.InputParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FactorConversionMasters_ParameterMasters_OutputParameterID",
                        column: x => x.OutputParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FactorConversionMasters_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FactorConversionMasters_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequirementMasters_CompanyCode_Code",
                table: "EquipmentRequirementMasters",
                columns: new[] { "CompanyCode", "Code" },
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequirementMasters_EquipmentID",
                table: "EquipmentRequirementMasters",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequirementMasters_EquipmentTypeID",
                table: "EquipmentRequirementMasters",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequirementMasters_LaboratoryTestID",
                table: "EquipmentRequirementMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequirementMasters_ParameterID",
                table: "EquipmentRequirementMasters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequirementMasters_RangeUnitID",
                table: "EquipmentRequirementMasters",
                column: "RangeUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequirementMasters_TestMethodSpecificationID",
                table: "EquipmentRequirementMasters",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequirementMasters_TestMethodSpecificationVersionID",
                table: "EquipmentRequirementMasters",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_FactorConversionMasters_CompanyCode_Code",
                table: "FactorConversionMasters",
                columns: new[] { "CompanyCode", "Code" },
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FactorConversionMasters_InputParameterID",
                table: "FactorConversionMasters",
                column: "InputParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_FactorConversionMasters_LaboratoryTestID",
                table: "FactorConversionMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_FactorConversionMasters_OutputParameterID",
                table: "FactorConversionMasters",
                column: "OutputParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_FactorConversionMasters_TestMethodSpecificationID",
                table: "FactorConversionMasters",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_FactorConversionMasters_TestMethodSpecificationVersionID",
                table: "FactorConversionMasters",
                column: "TestMethodSpecificationVersionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentRequirementMasters");

            migrationBuilder.DropTable(
                name: "FactorConversionMasters");
        }
    }
}
