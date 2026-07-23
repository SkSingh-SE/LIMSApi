using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablMesurmentUncertaintyAddCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TestMethod",
                table: "NablMeasurementUncertainties",
                newName: "TestMethodName");

            migrationBuilder.AddColumn<long>(
                name: "EquipmentID",
                table: "NablMeasurementUncertainties",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentName",
                table: "NablMeasurementUncertainties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID",
                table: "NablMeasurementUncertainties",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaboratoryTestName",
                table: "NablMeasurementUncertainties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MUCode",
                table: "NablMeasurementUncertainties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "NablMeasurementUncertainties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SumOfSquares",
                table: "NablMeasurementUncertainties",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestMethodID",
                table: "NablMeasurementUncertainties",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "NablMeasurementUncertainties",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentID",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "EquipmentName",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestName",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "MUCode",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "SumOfSquares",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "TestMethodID",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "NablMeasurementUncertainties");

            migrationBuilder.RenameColumn(
                name: "TestMethodName",
                table: "NablMeasurementUncertainties",
                newName: "TestMethod");
        }
    }
}
