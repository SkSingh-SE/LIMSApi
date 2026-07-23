using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablMethodValidationAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Accuracy",
                table: "NablMethodValidations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccuracyStudyJson",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BiasMax",
                table: "NablMethodValidations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Conclusion",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfidenceLevel",
                table: "NablMethodValidations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CoverageFactor",
                table: "NablMethodValidations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrmMaterialParametersJson",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentId",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentName",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpandedUncertainty",
                table: "NablMethodValidations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Humidity",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Measurement",
                table: "NablMethodValidations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MeasurementUncertainty",
                table: "NablMethodValidations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Precision",
                table: "NablMethodValidations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrecisionStudyJson",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForValidation",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonNotValid",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Recovery",
                table: "NablMethodValidations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RecoveryMax",
                table: "NablMethodValidations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RecoveryMin",
                table: "NablMethodValidations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceStandard",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Repeatability",
                table: "NablMethodValidations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevIssue",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Robustness",
                table: "NablMethodValidations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RsdMax",
                table: "NablMethodValidations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Temperature",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestMethodName",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidStatus",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationType",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDate",
                table: "NablMethodValidations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                table: "NablMethodValidations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accuracy",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "AccuracyStudyJson",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "BiasMax",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "Conclusion",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "ConfidenceLevel",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "CoverageFactor",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "CrmMaterialParametersJson",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "EquipmentName",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "ExpandedUncertainty",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "Measurement",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "MeasurementUncertainty",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "Precision",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "PrecisionStudyJson",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "ReasonForValidation",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "ReasonNotValid",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "Recovery",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "RecoveryMax",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "RecoveryMin",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "ReferenceStandard",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "Repeatability",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "RevIssue",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "Robustness",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "RsdMax",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "TestMethodName",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "ValidStatus",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "ValidationType",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "NablMethodValidations");
        }
    }
}
