using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class SamplePlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTest_TestPlans_SampleTestPlanID",
                table: "ChemicalTest");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestElement_ChemicalTest_ChemicalTestID",
                table: "ChemicalTestElement");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTest_TestPlans_SampleTestPlanID",
                table: "GeneralTest");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTestMethod_GeneralTest_GeneralTestID",
                table: "GeneralTestMethod");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTest_SampleTestPlanID",
                table: "ChemicalTest");

            migrationBuilder.DropColumn(
                name: "GradeID",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "SampleTestPlanID",
                table: "ChemicalTest");

            migrationBuilder.RenameColumn(
                name: "UrlNo",
                table: "ChemicalTest",
                newName: "UlrNo");

            migrationBuilder.AddColumn<string>(
                name: "DecisionRule",
                table: "SampleInwards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewStatus",
                table: "SampleInwards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ReviewedBy",
                table: "SampleInwards",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedOn",
                table: "SampleInwards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatementOfConformity",
                table: "SampleInwards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CuttingRequired",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MachiningAmount",
                table: "SampleDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "MachiningRequired",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OtherPreparation",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherPreparationCharge",
                table: "SampleDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "TpiRequired",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "GeneralTestMethod",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "GeneralTestID",
                table: "GeneralTestMethod",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Specification2",
                table: "GeneralTest",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "SampleTestPlanID",
                table: "GeneralTest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ChemicalTestID",
                table: "ChemicalTestElement",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SamplePlanID",
                table: "ChemicalTest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TestPlans_SampleID",
                table: "TestPlans",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestElement_ParameterID",
                table: "ChemicalTestElement",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTest_SamplePlanID",
                table: "ChemicalTest",
                column: "SamplePlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTest_TestPlans_SamplePlanID",
                table: "ChemicalTest",
                column: "SamplePlanID",
                principalTable: "TestPlans",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_TestPlans_SampleDetails_SampleID",
                table: "TestPlans",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FK_GeneralTest_TestPlans_SampleTestPlanID",
                table: "GeneralTest");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTestMethod_GeneralTest_GeneralTestID",
                table: "GeneralTestMethod");

            migrationBuilder.DropForeignKey(
                name: "FK_TestPlans_SampleDetails_SampleID",
                table: "TestPlans");

            migrationBuilder.DropIndex(
                name: "IX_TestPlans_SampleID",
                table: "TestPlans");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTestElement_ParameterID",
                table: "ChemicalTestElement");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTest_SamplePlanID",
                table: "ChemicalTest");

            migrationBuilder.DropColumn(
                name: "DecisionRule",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "ReviewedOn",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "StatementOfConformity",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "CuttingRequired",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "MachiningAmount",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "MachiningRequired",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "OtherPreparation",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "OtherPreparationCharge",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "TpiRequired",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "SamplePlanID",
                table: "ChemicalTest");

            migrationBuilder.RenameColumn(
                name: "UlrNo",
                table: "ChemicalTest",
                newName: "UrlNo");

            migrationBuilder.AddColumn<long>(
                name: "GradeID",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "GeneralTestMethod",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "GeneralTestID",
                table: "GeneralTestMethod",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Specification2",
                table: "GeneralTest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SampleTestPlanID",
                table: "GeneralTest",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ChemicalTestID",
                table: "ChemicalTestElement",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "SampleTestPlanID",
                table: "ChemicalTest",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTest_SampleTestPlanID",
                table: "ChemicalTest",
                column: "SampleTestPlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTest_TestPlans_SampleTestPlanID",
                table: "ChemicalTest",
                column: "SampleTestPlanID",
                principalTable: "TestPlans",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestElement_ChemicalTest_ChemicalTestID",
                table: "ChemicalTestElement",
                column: "ChemicalTestID",
                principalTable: "ChemicalTest",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTest_TestPlans_SampleTestPlanID",
                table: "GeneralTest",
                column: "SampleTestPlanID",
                principalTable: "TestPlans",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTestMethod_GeneralTest_GeneralTestID",
                table: "GeneralTestMethod",
                column: "GeneralTestID",
                principalTable: "GeneralTest",
                principalColumn: "ID");
        }
    }
}
