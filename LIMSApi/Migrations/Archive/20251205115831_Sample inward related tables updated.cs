using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Sampleinwardrelatedtablesupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTest_TestPlans_SamplePlanID",
                table: "ChemicalTest");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestElement_ChemicalTest_ChemicalTestID",
                table: "ChemicalTestElement");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestElement_ParameterMasters_ParameterID",
                table: "ChemicalTestElement");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestType_ChemicalTest_ChemicalTestID",
                table: "ChemicalTestType");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTest_TestPlans_SampleTestPlanID",
                table: "GeneralTest");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTestMethod_GeneralTest_GeneralTestID",
                table: "GeneralTestMethod");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTests_MetalClassificationMasters_MetalClassificationID",
                table: "LaboratoryTests");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTests_MetalClassificationID",
                table: "LaboratoryTests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneralTestMethod",
                table: "GeneralTestMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneralTest",
                table: "GeneralTest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChemicalTestType",
                table: "ChemicalTestType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChemicalTestElement",
                table: "ChemicalTestElement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChemicalTest",
                table: "ChemicalTest");

            migrationBuilder.DropColumn(
                name: "MetalClassificationID",
                table: "LaboratoryTests");

            migrationBuilder.RenameTable(
                name: "GeneralTestMethod",
                newName: "GeneralTestMethods");

            migrationBuilder.RenameTable(
                name: "GeneralTest",
                newName: "GeneralTests");

            migrationBuilder.RenameTable(
                name: "ChemicalTestType",
                newName: "ChemicalTestTypes");

            migrationBuilder.RenameTable(
                name: "ChemicalTestElement",
                newName: "ChemicalTestElements");

            migrationBuilder.RenameTable(
                name: "ChemicalTest",
                newName: "ChemicalTests");

            migrationBuilder.RenameIndex(
                name: "IX_GeneralTestMethod_GeneralTestID",
                table: "GeneralTestMethods",
                newName: "IX_GeneralTestMethods_GeneralTestID");

            migrationBuilder.RenameIndex(
                name: "IX_GeneralTest_SampleTestPlanID",
                table: "GeneralTests",
                newName: "IX_GeneralTests_SampleTestPlanID");

            migrationBuilder.RenameIndex(
                name: "IX_ChemicalTestType_ChemicalTestID",
                table: "ChemicalTestTypes",
                newName: "IX_ChemicalTestTypes_ChemicalTestID");

            migrationBuilder.RenameIndex(
                name: "IX_ChemicalTestElement_ParameterID",
                table: "ChemicalTestElements",
                newName: "IX_ChemicalTestElements_ParameterID");

            migrationBuilder.RenameIndex(
                name: "IX_ChemicalTestElement_ChemicalTestID",
                table: "ChemicalTestElements",
                newName: "IX_ChemicalTestElements_ChemicalTestID");

            migrationBuilder.RenameIndex(
                name: "IX_ChemicalTest_SamplePlanID",
                table: "ChemicalTests",
                newName: "IX_ChemicalTests_SamplePlanID");

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID",
                table: "ChemicalTestTypes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneralTestMethods",
                table: "GeneralTestMethods",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneralTests",
                table: "GeneralTests",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChemicalTestTypes",
                table: "ChemicalTestTypes",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChemicalTestElements",
                table: "ChemicalTestElements",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChemicalTests",
                table: "ChemicalTests",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "TestResultHeaders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestPlanID = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_TestResultHeaders", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TestResultParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumericValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCalculated = table.Column<bool>(type: "bit", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultParameters_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestTypes_LaboratoryTestID",
                table: "ChemicalTestTypes",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultParameters_TestResultHeaderID",
                table: "TestResultParameters",
                column: "TestResultHeaderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestElements_ChemicalTests_ChemicalTestID",
                table: "ChemicalTestElements",
                column: "ChemicalTestID",
                principalTable: "ChemicalTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestElements_ParameterMasters_ParameterID",
                table: "ChemicalTestElements",
                column: "ParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTests_TestPlans_SamplePlanID",
                table: "ChemicalTests",
                column: "SamplePlanID",
                principalTable: "TestPlans",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestTypes_ChemicalTests_ChemicalTestID",
                table: "ChemicalTestTypes",
                column: "ChemicalTestID",
                principalTable: "ChemicalTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestTypes_LaboratoryTests_LaboratoryTestID",
                table: "ChemicalTestTypes",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTestMethods_GeneralTests_GeneralTestID",
                table: "GeneralTestMethods",
                column: "GeneralTestID",
                principalTable: "GeneralTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTests_TestPlans_SampleTestPlanID",
                table: "GeneralTests",
                column: "SampleTestPlanID",
                principalTable: "TestPlans",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestElements_ChemicalTests_ChemicalTestID",
                table: "ChemicalTestElements");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestElements_ParameterMasters_ParameterID",
                table: "ChemicalTestElements");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTests_TestPlans_SamplePlanID",
                table: "ChemicalTests");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestTypes_ChemicalTests_ChemicalTestID",
                table: "ChemicalTestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestTypes_LaboratoryTests_LaboratoryTestID",
                table: "ChemicalTestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTestMethods_GeneralTests_GeneralTestID",
                table: "GeneralTestMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTests_TestPlans_SampleTestPlanID",
                table: "GeneralTests");

            migrationBuilder.DropTable(
                name: "TestResultParameters");

            migrationBuilder.DropTable(
                name: "TestResultHeaders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneralTests",
                table: "GeneralTests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneralTestMethods",
                table: "GeneralTestMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChemicalTestTypes",
                table: "ChemicalTestTypes");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTestTypes_LaboratoryTestID",
                table: "ChemicalTestTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChemicalTests",
                table: "ChemicalTests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChemicalTestElements",
                table: "ChemicalTestElements");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID",
                table: "ChemicalTestTypes");

            migrationBuilder.RenameTable(
                name: "GeneralTests",
                newName: "GeneralTest");

            migrationBuilder.RenameTable(
                name: "GeneralTestMethods",
                newName: "GeneralTestMethod");

            migrationBuilder.RenameTable(
                name: "ChemicalTestTypes",
                newName: "ChemicalTestType");

            migrationBuilder.RenameTable(
                name: "ChemicalTests",
                newName: "ChemicalTest");

            migrationBuilder.RenameTable(
                name: "ChemicalTestElements",
                newName: "ChemicalTestElement");

            migrationBuilder.RenameIndex(
                name: "IX_GeneralTests_SampleTestPlanID",
                table: "GeneralTest",
                newName: "IX_GeneralTest_SampleTestPlanID");

            migrationBuilder.RenameIndex(
                name: "IX_GeneralTestMethods_GeneralTestID",
                table: "GeneralTestMethod",
                newName: "IX_GeneralTestMethod_GeneralTestID");

            migrationBuilder.RenameIndex(
                name: "IX_ChemicalTestTypes_ChemicalTestID",
                table: "ChemicalTestType",
                newName: "IX_ChemicalTestType_ChemicalTestID");

            migrationBuilder.RenameIndex(
                name: "IX_ChemicalTests_SamplePlanID",
                table: "ChemicalTest",
                newName: "IX_ChemicalTest_SamplePlanID");

            migrationBuilder.RenameIndex(
                name: "IX_ChemicalTestElements_ParameterID",
                table: "ChemicalTestElement",
                newName: "IX_ChemicalTestElement_ParameterID");

            migrationBuilder.RenameIndex(
                name: "IX_ChemicalTestElements_ChemicalTestID",
                table: "ChemicalTestElement",
                newName: "IX_ChemicalTestElement_ChemicalTestID");

            migrationBuilder.AddColumn<long>(
                name: "MetalClassificationID",
                table: "LaboratoryTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneralTest",
                table: "GeneralTest",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneralTestMethod",
                table: "GeneralTestMethod",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChemicalTestType",
                table: "ChemicalTestType",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChemicalTest",
                table: "ChemicalTest",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChemicalTestElement",
                table: "ChemicalTestElement",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_MetalClassificationID",
                table: "LaboratoryTests",
                column: "MetalClassificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTest_TestPlans_SamplePlanID",
                table: "ChemicalTest",
                column: "SamplePlanID",
                principalTable: "TestPlans",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestElement_ChemicalTest_ChemicalTestID",
                table: "ChemicalTestElement",
                column: "ChemicalTestID",
                principalTable: "ChemicalTest",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestElement_ParameterMasters_ParameterID",
                table: "ChemicalTestElement",
                column: "ParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestType_ChemicalTest_ChemicalTestID",
                table: "ChemicalTestType",
                column: "ChemicalTestID",
                principalTable: "ChemicalTest",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTest_TestPlans_SampleTestPlanID",
                table: "GeneralTest",
                column: "SampleTestPlanID",
                principalTable: "TestPlans",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTestMethod_GeneralTest_GeneralTestID",
                table: "GeneralTestMethod",
                column: "GeneralTestID",
                principalTable: "GeneralTest",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTests_MetalClassificationMasters_MetalClassificationID",
                table: "LaboratoryTests",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");
        }
    }
}
