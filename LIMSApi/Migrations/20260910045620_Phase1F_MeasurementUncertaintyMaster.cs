using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase1F_MeasurementUncertaintyMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementUncertaintyMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    UncertaintyType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Basis = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CombinedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ExpandedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CoverageFactor = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ConfidenceLevel = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ComponentsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_MeasurementUncertaintyMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MeasurementUncertaintyMasters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MeasurementUncertaintyMasters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MeasurementUncertaintyMasters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MeasurementUncertaintyMasters_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MeasurementUncertaintyMasters_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUncertaintyMasters_CompanyCode_Code",
                table: "MeasurementUncertaintyMasters",
                columns: new[] { "CompanyCode", "Code" },
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUncertaintyMasters_LaboratoryTestID",
                table: "MeasurementUncertaintyMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUncertaintyMasters_ParameterID",
                table: "MeasurementUncertaintyMasters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUncertaintyMasters_ParameterUnitID",
                table: "MeasurementUncertaintyMasters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUncertaintyMasters_TestMethodSpecificationID",
                table: "MeasurementUncertaintyMasters",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUncertaintyMasters_TestMethodSpecificationVersionID",
                table: "MeasurementUncertaintyMasters",
                column: "TestMethodSpecificationVersionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementUncertaintyMasters");
        }
    }
}
