using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase9WorkflowPerformanceAndPhase7Masters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =============================================
            // Phase 7.1: EquipmentMaster enhancements
            // =============================================
            migrationBuilder.AddColumn<DateTime>(
                name: "LastCalibrationDate",
                table: "EquipmentMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalibrationFrequencyDays",
                table: "EquipmentMasters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceSchedule",
                table: "EquipmentMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // =============================================
            // Phase 7.2: EmployeeMaster enhancements
            // =============================================
            migrationBuilder.AddColumn<string>(
                name: "QualificationSummary",
                table: "EmployeeMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "EmployeeMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrainingRecordsJson",
                table: "EmployeeMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompetencyLevel",
                table: "EmployeeMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // =============================================
            // Phase 7.3: TestMethodSpecification enhancements
            // =============================================
            migrationBuilder.AddColumn<string>(
                name: "LinkedStandard",
                table: "TestMethodSpecifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormulaExpression",
                table: "TestMethodSpecifications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultParameters",
                table: "TestMethodSpecifications",
                type: "nvarchar(max)",
                nullable: true);

            // =============================================
            // Phase 9: Database indexes for workflow performance
            // =============================================

            // SampleInwards indexes
            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_InwardStatus",
                table: "SampleInwards",
                column: "InwardStatus");

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_CustomerID",
                table: "SampleInwards",
                column: "CustomerID");

            // TestResultHeaders indexes
            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_Status",
                table: "TestResultHeaders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_SampleID",
                table: "TestResultHeaders",
                column: "SampleID");

            // ReportHeaders indexes
            migrationBuilder.CreateIndex(
                name: "IX_ReportHeaders_Status",
                table: "ReportHeaders",
                column: "Status");

            // WorkflowSteps indexes
            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_OrderNo",
                table: "WorkflowSteps",
                column: "OrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_IsActive",
                table: "WorkflowSteps",
                column: "IsActive");

            // WorkflowInstances indexes
            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_Status",
                table: "WorkflowInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_EntityID_EntityType_IsActive",
                table: "WorkflowInstances",
                columns: new[] { "EntityID", "EntityType", "IsActive" });

            // WorkflowActionLogs indexes
            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActionLogs_InstanceID",
                table: "WorkflowActionLogs",
                column: "InstanceID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActionLogs_WorkflowID",
                table: "WorkflowActionLogs",
                column: "WorkflowID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes
            migrationBuilder.DropIndex(name: "IX_WorkflowActionLogs_WorkflowID", table: "WorkflowActionLogs");
            migrationBuilder.DropIndex(name: "IX_WorkflowActionLogs_InstanceID", table: "WorkflowActionLogs");
            migrationBuilder.DropIndex(name: "IX_WorkflowInstances_EntityID_EntityType_IsActive", table: "WorkflowInstances");
            migrationBuilder.DropIndex(name: "IX_WorkflowInstances_Status", table: "WorkflowInstances");
            migrationBuilder.DropIndex(name: "IX_WorkflowSteps_IsActive", table: "WorkflowSteps");
            migrationBuilder.DropIndex(name: "IX_WorkflowSteps_OrderNo", table: "WorkflowSteps");
            migrationBuilder.DropIndex(name: "IX_ReportHeaders_Status", table: "ReportHeaders");
            migrationBuilder.DropIndex(name: "IX_TestResultHeaders_SampleID", table: "TestResultHeaders");
            migrationBuilder.DropIndex(name: "IX_TestResultHeaders_Status", table: "TestResultHeaders");
            migrationBuilder.DropIndex(name: "IX_SampleInwards_CustomerID", table: "SampleInwards");
            migrationBuilder.DropIndex(name: "IX_SampleInwards_InwardStatus", table: "SampleInwards");

            // Drop Phase 7.3 columns
            migrationBuilder.DropColumn(name: "DefaultParameters", table: "TestMethodSpecifications");
            migrationBuilder.DropColumn(name: "FormulaExpression", table: "TestMethodSpecifications");
            migrationBuilder.DropColumn(name: "LinkedStandard", table: "TestMethodSpecifications");

            // Drop Phase 7.2 columns
            migrationBuilder.DropColumn(name: "CompetencyLevel", table: "EmployeeMasters");
            migrationBuilder.DropColumn(name: "TrainingRecordsJson", table: "EmployeeMasters");
            migrationBuilder.DropColumn(name: "Experience", table: "EmployeeMasters");
            migrationBuilder.DropColumn(name: "QualificationSummary", table: "EmployeeMasters");

            // Drop Phase 7.1 columns
            migrationBuilder.DropColumn(name: "MaintenanceSchedule", table: "EquipmentMasters");
            migrationBuilder.DropColumn(name: "CalibrationFrequencyDays", table: "EquipmentMasters");
            migrationBuilder.DropColumn(name: "LastCalibrationDate", table: "EquipmentMasters");
        }
    }
}
