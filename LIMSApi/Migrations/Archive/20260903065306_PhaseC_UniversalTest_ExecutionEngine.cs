using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class PhaseC_UniversalTest_ExecutionEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestExecutions_UniversalTestGroups_UniversalTestGroupID",
                table: "TestExecutions");

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "UniversalTestGroups",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ApprovedBy",
                table: "TestExecutions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "TestExecutions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "TestExecutions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ExecutionNo",
                table: "TestExecutions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRetest",
                table: "TestExecutions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "PreviousExecutionID",
                table: "TestExecutions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewRemarks",
                table: "TestExecutions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VerifiedBy",
                table: "TestExecutions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedOn",
                table: "TestExecutions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultStatus",
                table: "ParameterObservationResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecMax",
                table: "ParameterObservationResults",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecMin",
                table: "ParameterObservationResults",
                type: "decimal(18,6)",
                nullable: true);

            // Dynamically derive and backfill BranchID from SampleInward for UniversalTestGroups
            migrationBuilder.Sql(@"
                UPDATE utg
                SET utg.BranchID = si.BranchID
                FROM UniversalTestGroups utg
                INNER JOIN TestPlans tp ON utg.SampleTestPlanID = tp.ID
                INNER JOIN SampleDetails sd ON tp.SampleID = sd.ID
                INNER JOIN SampleInwards si ON sd.InwardID = si.ID
                WHERE utg.BranchID = 0;

                UPDATE utg
                SET utg.BranchID = (SELECT TOP 1 ID FROM Branches WHERE OrganizationID = utg.OrganizationID AND IsActive = 1)
                FROM UniversalTestGroups utg
                WHERE utg.BranchID = 0 AND EXISTS (SELECT 1 FROM Branches WHERE OrganizationID = utg.OrganizationID AND IsActive = 1);

                UPDATE te
                SET te.BranchID = utg.BranchID
                FROM TestExecutions te
                INNER JOIN UniversalTestGroups utg ON te.UniversalTestGroupID = utg.ID
                WHERE te.BranchID = 0;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_BranchID",
                table: "UniversalTestGroups",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_SpecificationGradeID",
                table: "UniversalTestGroups",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_SpecificationHeaderID",
                table: "UniversalTestGroups",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_TestMethodSpecificationID",
                table: "UniversalTestGroups",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_BranchID",
                table: "TestExecutions",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_PreviousExecutionID",
                table: "TestExecutions",
                column: "PreviousExecutionID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestExecutions_Branches_BranchID",
                table: "TestExecutions",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestExecutions_TestExecutions_PreviousExecutionID",
                table: "TestExecutions",
                column: "PreviousExecutionID",
                principalTable: "TestExecutions",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestExecutions_UniversalTestGroups_UniversalTestGroupID",
                table: "TestExecutions",
                column: "UniversalTestGroupID",
                principalTable: "UniversalTestGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalTestGroups_Branches_BranchID",
                table: "UniversalTestGroups",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalTestGroups_SpecificationGrades_SpecificationGradeID",
                table: "UniversalTestGroups",
                column: "SpecificationGradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalTestGroups_SpecificationHeaders_SpecificationHeaderID",
                table: "UniversalTestGroups",
                column: "SpecificationHeaderID",
                principalTable: "SpecificationHeaders",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversalTestGroups_TestMethodSpecifications_TestMethodSpecificationID",
                table: "UniversalTestGroups",
                column: "TestMethodSpecificationID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestExecutions_Branches_BranchID",
                table: "TestExecutions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestExecutions_TestExecutions_PreviousExecutionID",
                table: "TestExecutions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestExecutions_UniversalTestGroups_UniversalTestGroupID",
                table: "TestExecutions");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalTestGroups_Branches_BranchID",
                table: "UniversalTestGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalTestGroups_SpecificationGrades_SpecificationGradeID",
                table: "UniversalTestGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalTestGroups_SpecificationHeaders_SpecificationHeaderID",
                table: "UniversalTestGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalTestGroups_TestMethodSpecifications_TestMethodSpecificationID",
                table: "UniversalTestGroups");

            migrationBuilder.DropIndex(
                name: "IX_UniversalTestGroups_BranchID",
                table: "UniversalTestGroups");

            migrationBuilder.DropIndex(
                name: "IX_UniversalTestGroups_SpecificationGradeID",
                table: "UniversalTestGroups");

            migrationBuilder.DropIndex(
                name: "IX_UniversalTestGroups_SpecificationHeaderID",
                table: "UniversalTestGroups");

            migrationBuilder.DropIndex(
                name: "IX_UniversalTestGroups_TestMethodSpecificationID",
                table: "UniversalTestGroups");

            migrationBuilder.DropIndex(
                name: "IX_TestExecutions_BranchID",
                table: "TestExecutions");

            migrationBuilder.DropIndex(
                name: "IX_TestExecutions_PreviousExecutionID",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "UniversalTestGroups");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "ExecutionNo",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "IsRetest",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "PreviousExecutionID",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "ReviewRemarks",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "VerifiedOn",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "ResultStatus",
                table: "ParameterObservationResults");

            migrationBuilder.DropColumn(
                name: "SpecMax",
                table: "ParameterObservationResults");

            migrationBuilder.DropColumn(
                name: "SpecMin",
                table: "ParameterObservationResults");

            migrationBuilder.AddForeignKey(
                name: "FK_TestExecutions_UniversalTestGroups_UniversalTestGroupID",
                table: "TestExecutions",
                column: "UniversalTestGroupID",
                principalTable: "UniversalTestGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
