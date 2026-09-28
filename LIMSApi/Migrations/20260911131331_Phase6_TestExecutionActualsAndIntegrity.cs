using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase6_TestExecutionActualsAndIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestExecutions_ExecutionConfigSnapshots_ExecutionConfigSnapshotID",
                table: "TestExecutions");

            migrationBuilder.DropIndex(
                name: "IX_TestExecutions_ExecutionConfigSnapshotID",
                table: "TestExecutions");

            migrationBuilder.DropIndex(
                name: "IX_TestExecutions_UniversalTestGroupID",
                table: "TestExecutions");

            migrationBuilder.AddColumn<int>(
                name: "ExecutionCount",
                table: "UniversalTestGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ActualConditionsJson",
                table: "TestExecutions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualEquipmentJson",
                table: "TestExecutions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_ExecutionConfigSnapshotID",
                table: "TestExecutions",
                column: "ExecutionConfigSnapshotID",
                unique: true,
                filter: "[ExecutionConfigSnapshotID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_UTG_ExecutionNo",
                table: "TestExecutions",
                columns: new[] { "UniversalTestGroupID", "ExecutionNo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestExecutions_ExecutionConfigSnapshots_ExecutionConfigSnapshotID",
                table: "TestExecutions",
                column: "ExecutionConfigSnapshotID",
                principalTable: "ExecutionConfigSnapshots",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestExecutions_ExecutionConfigSnapshots_ExecutionConfigSnapshotID",
                table: "TestExecutions");

            migrationBuilder.DropIndex(
                name: "IX_TestExecutions_ExecutionConfigSnapshotID",
                table: "TestExecutions");

            migrationBuilder.DropIndex(
                name: "IX_TestExecutions_UTG_ExecutionNo",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "ExecutionCount",
                table: "UniversalTestGroups");

            migrationBuilder.DropColumn(
                name: "ActualConditionsJson",
                table: "TestExecutions");

            migrationBuilder.DropColumn(
                name: "ActualEquipmentJson",
                table: "TestExecutions");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_ExecutionConfigSnapshotID",
                table: "TestExecutions",
                column: "ExecutionConfigSnapshotID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_UniversalTestGroupID",
                table: "TestExecutions",
                column: "UniversalTestGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestExecutions_ExecutionConfigSnapshots_ExecutionConfigSnapshotID",
                table: "TestExecutions",
                column: "ExecutionConfigSnapshotID",
                principalTable: "ExecutionConfigSnapshots",
                principalColumn: "ID");
        }
    }
}
