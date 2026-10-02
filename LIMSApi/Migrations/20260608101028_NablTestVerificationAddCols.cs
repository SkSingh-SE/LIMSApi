using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablTestVerificationAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BiasMax",
                table: "NablMethodVerifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CalibrationDueDate",
                table: "NablMethodVerifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Conclusion",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrmParametersJson",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentId",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentName",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Humidity",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonNotVerified",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecoveryMax",
                table: "NablMethodVerifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecoveryMin",
                table: "NablMethodVerifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceStandard",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevIssue",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RsdMax",
                table: "NablMethodVerifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Temperature",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestMethodName",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationDataJson",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "NablMethodVerifications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BiasMax",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "CalibrationDueDate",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "Conclusion",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "CrmParametersJson",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "EquipmentName",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "ReasonNotVerified",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "RecoveryMax",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "RecoveryMin",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "ReferenceStandard",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "RevIssue",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "RsdMax",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "TestMethodName",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "VerificationDataJson",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "NablMethodVerifications");
        }
    }
}
