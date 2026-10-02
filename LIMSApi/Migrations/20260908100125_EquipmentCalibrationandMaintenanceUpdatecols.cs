using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentCalibrationandMaintenanceUpdatecols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Desrciption",
                table: "EquipmentMaintenance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintanceCreateBy",
                table: "EquipmentMaintenance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintanceCreateDate",
                table: "EquipmentMaintenance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalibrationCreateBy",
                table: "EquipmentCalibration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CalibrationCreateDate",
                table: "EquipmentCalibration",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Desrciption",
                table: "EquipmentCalibration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewerBy",
                table: "EquipmentCalibration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewerDate",
                table: "EquipmentCalibration",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desrciption",
                table: "EquipmentMaintenance");

            migrationBuilder.DropColumn(
                name: "MaintanceCreateBy",
                table: "EquipmentMaintenance");

            migrationBuilder.DropColumn(
                name: "MaintanceCreateDate",
                table: "EquipmentMaintenance");

            migrationBuilder.DropColumn(
                name: "CalibrationCreateBy",
                table: "EquipmentCalibration");

            migrationBuilder.DropColumn(
                name: "CalibrationCreateDate",
                table: "EquipmentCalibration");

            migrationBuilder.DropColumn(
                name: "Desrciption",
                table: "EquipmentCalibration");

            migrationBuilder.DropColumn(
                name: "ReviewerBy",
                table: "EquipmentCalibration");

            migrationBuilder.DropColumn(
                name: "ReviewerDate",
                table: "EquipmentCalibration");
        }
    }
}
