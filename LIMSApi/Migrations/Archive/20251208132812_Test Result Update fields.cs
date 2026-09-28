using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class TestResultUpdatefields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StringValue",
                table: "TestResultParameters",
                newName: "TextValue");

            migrationBuilder.AddColumn<bool>(
                name: "AllowNumeric",
                table: "TestResultParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowText",
                table: "TestResultParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWithinLimit",
                table: "TestResultParameters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValue",
                table: "TestResultParameters",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValue",
                table: "TestResultParameters",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpecificationLineID",
                table: "TestResultParameters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EquipmentID",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNabl",
                table: "TestResultHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverallPass",
                table: "TestResultHeaders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabNo",
                table: "TestResultHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowNumeric",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "AllowText",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "IsWithinLimit",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "SpecificationLineID",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "EquipmentID",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "IsNabl",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "IsOverallPass",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "LabNo",
                table: "TestResultHeaders");

            migrationBuilder.RenameColumn(
                name: "TextValue",
                table: "TestResultParameters",
                newName: "StringValue");
        }
    }
}
