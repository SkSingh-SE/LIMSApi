using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Wave4MasterEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultParameters",
                table: "TestMethodSpecifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormulaExpression",
                table: "TestMethodSpecifications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedStandard",
                table: "TestMethodSpecifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalibrationFrequencyDays",
                table: "EquipmentMasters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCalibrationDate",
                table: "EquipmentMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceSchedule",
                table: "EquipmentMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompetencyLevel",
                table: "EmployeeMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "EmployeeMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QualificationSummary",
                table: "EmployeeMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrainingRecordsJson",
                table: "EmployeeMasters",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultParameters",
                table: "TestMethodSpecifications");

            migrationBuilder.DropColumn(
                name: "FormulaExpression",
                table: "TestMethodSpecifications");

            migrationBuilder.DropColumn(
                name: "LinkedStandard",
                table: "TestMethodSpecifications");

            migrationBuilder.DropColumn(
                name: "CalibrationFrequencyDays",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "LastCalibrationDate",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "MaintenanceSchedule",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "CompetencyLevel",
                table: "EmployeeMasters");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "EmployeeMasters");

            migrationBuilder.DropColumn(
                name: "QualificationSummary",
                table: "EmployeeMasters");

            migrationBuilder.DropColumn(
                name: "TrainingRecordsJson",
                table: "EmployeeMasters");
        }
    }
}
