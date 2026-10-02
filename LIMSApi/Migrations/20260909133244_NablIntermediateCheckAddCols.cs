using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablIntermediateCheckAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablIntermediateChecks_EquipmentMasters_EquipmentId",
                table: "NablIntermediateChecks");

            migrationBuilder.DropIndex(
                name: "IX_NablIntermediateChecks_EquipmentId",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "CorrectiveAction",
                table: "NablIntermediateChecks");

            migrationBuilder.RenameColumn(
                name: "EquipmentCode",
                table: "NablIntermediateChecks",
                newName: "EquipmentName");

            migrationBuilder.AddColumn<string>(
                name: "CalibrationFrequencyDays",
                table: "NablIntermediateChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablIntermediateChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentNo",
                table: "NablIntermediateChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentType",
                table: "NablIntermediateChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntermediateCheckInterval",
                table: "NablIntermediateChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntermediateCheckLogsJson",
                table: "NablIntermediateChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCalibrationDate",
                table: "NablIntermediateChecks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelNumber",
                table: "NablIntermediateChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextCalibrationDueDate",
                table: "NablIntermediateChecks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEMName",
                table: "NablIntermediateChecks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalibrationFrequencyDays",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "EquipmentNo",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "EquipmentType",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "IntermediateCheckInterval",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "IntermediateCheckLogsJson",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "LastCalibrationDate",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "ModelNumber",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "NextCalibrationDueDate",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "OEMName",
                table: "NablIntermediateChecks");

            migrationBuilder.RenameColumn(
                name: "EquipmentName",
                table: "NablIntermediateChecks",
                newName: "EquipmentCode");

            migrationBuilder.AddColumn<string>(
                name: "CorrectiveAction",
                table: "NablIntermediateChecks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablIntermediateChecks_EquipmentId",
                table: "NablIntermediateChecks",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablIntermediateChecks_EquipmentMasters_EquipmentId",
                table: "NablIntermediateChecks",
                column: "EquipmentId",
                principalTable: "EquipmentMasters",
                principalColumn: "ID");
        }
    }
}
