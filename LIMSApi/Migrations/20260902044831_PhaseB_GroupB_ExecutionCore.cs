using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class PhaseB_GroupB_ExecutionCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExecutionConfigSnapshots",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SnapshotHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionConfigSnapshots", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TestConditionDimensions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestConditionDimensions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UniversalTestGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleTestPlanID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalTestGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_TestPlans_SampleTestPlanID",
                        column: x => x.SampleTestPlanID,
                        principalTable: "TestPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationLineConditions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationLineID = table.Column<long>(type: "bigint", nullable: false),
                    TestConditionDimensionID = table.Column<long>(type: "bigint", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Value1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationLineConditions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationLineConditions_SpecificationLines_SpecificationLineID",
                        column: x => x.SpecificationLineID,
                        principalTable: "SpecificationLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecificationLineConditions_TestConditionDimensions_TestConditionDimensionID",
                        column: x => x.TestConditionDimensionID,
                        principalTable: "TestConditionDimensions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestExecutions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversalTestGroupID = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionAnalystID = table.Column<long>(type: "bigint", nullable: true),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExecutionConfigSnapshotID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestExecutions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestExecutions_ExecutionConfigSnapshots_ExecutionConfigSnapshotID",
                        column: x => x.ExecutionConfigSnapshotID,
                        principalTable: "ExecutionConfigSnapshots",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestExecutions_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TestExecutions_UniversalTestGroups_UniversalTestGroupID",
                        column: x => x.UniversalTestGroupID,
                        principalTable: "UniversalTestGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TestSpecimens",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestExecutionID = table.Column<long>(type: "bigint", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    SpecimenIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDiscarded = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSpecimens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestSpecimens_TestExecutions_TestExecutionID",
                        column: x => x.TestExecutionID,
                        principalTable: "TestExecutions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestObservations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestSpecimenID = table.Column<long>(type: "bigint", nullable: false),
                    ReadingNo = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestObservations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestObservations_TestSpecimens_TestSpecimenID",
                        column: x => x.TestSpecimenID,
                        principalTable: "TestSpecimens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParameterObservationResults",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestObservationID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterMasterID = table.Column<long>(type: "bigint", nullable: false),
                    RawValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NumericValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CalculatedValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsFormulaCalculated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterObservationResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParameterObservationResults_ParameterMasters_ParameterMasterID",
                        column: x => x.ParameterMasterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParameterObservationResults_TestObservations_TestObservationID",
                        column: x => x.TestObservationID,
                        principalTable: "TestObservations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterObservationResults_ParameterMasterID",
                table: "ParameterObservationResults",
                column: "ParameterMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterObservationResults_TestObservationID",
                table: "ParameterObservationResults",
                column: "TestObservationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineConditions_SpecificationLineID",
                table: "SpecificationLineConditions",
                column: "SpecificationLineID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineConditions_TestConditionDimensionID",
                table: "SpecificationLineConditions",
                column: "TestConditionDimensionID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_ExecutionConfigSnapshotID",
                table: "TestExecutions",
                column: "ExecutionConfigSnapshotID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_OrganizationID",
                table: "TestExecutions",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_UniversalTestGroupID",
                table: "TestExecutions",
                column: "UniversalTestGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TestObservations_TestSpecimenID",
                table: "TestObservations",
                column: "TestSpecimenID");

            migrationBuilder.CreateIndex(
                name: "IX_TestSpecimens_TestExecutionID",
                table: "TestSpecimens",
                column: "TestExecutionID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_LaboratoryTestID",
                table: "UniversalTestGroups",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_OrganizationID",
                table: "UniversalTestGroups",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_SampleTestPlanID",
                table: "UniversalTestGroups",
                column: "SampleTestPlanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParameterObservationResults");

            migrationBuilder.DropTable(
                name: "SpecificationLineConditions");

            migrationBuilder.DropTable(
                name: "TestObservations");

            migrationBuilder.DropTable(
                name: "TestConditionDimensions");

            migrationBuilder.DropTable(
                name: "TestSpecimens");

            migrationBuilder.DropTable(
                name: "TestExecutions");

            migrationBuilder.DropTable(
                name: "ExecutionConfigSnapshots");

            migrationBuilder.DropTable(
                name: "UniversalTestGroups");
        }
    }
}
