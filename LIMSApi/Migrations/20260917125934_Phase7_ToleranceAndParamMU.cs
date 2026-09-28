using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase7_ToleranceAndParamMU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AppliedTolerance",
                table: "UniversalTestResultParameters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComplianceValueSource",
                table: "UniversalTestResultParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EffectiveMax",
                table: "UniversalTestResultParameters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EffectiveMin",
                table: "UniversalTestResultParameters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MUSource",
                table: "UniversalTestResultParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToleranceSource",
                table: "UniversalTestResultParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToleranceType",
                table: "UniversalTestResultParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedTolerance",
                table: "UniversalTestResultParameters");

            migrationBuilder.DropColumn(
                name: "ComplianceValueSource",
                table: "UniversalTestResultParameters");

            migrationBuilder.DropColumn(
                name: "EffectiveMax",
                table: "UniversalTestResultParameters");

            migrationBuilder.DropColumn(
                name: "EffectiveMin",
                table: "UniversalTestResultParameters");

            migrationBuilder.DropColumn(
                name: "MUSource",
                table: "UniversalTestResultParameters");

            migrationBuilder.DropColumn(
                name: "ToleranceSource",
                table: "UniversalTestResultParameters");

            migrationBuilder.DropColumn(
                name: "ToleranceType",
                table: "UniversalTestResultParameters");
        }
    }
}
