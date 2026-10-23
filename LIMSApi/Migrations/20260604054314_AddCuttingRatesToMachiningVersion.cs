using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCuttingRatesToMachiningVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CuttingRateGeneralMetal",
                table: "MachiningChargeVersions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CuttingRateHardMetal",
                table: "MachiningChargeVersions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuttingRateGeneralMetal",
                table: "MachiningChargeVersions");

            migrationBuilder.DropColumn(
                name: "CuttingRateHardMetal",
                table: "MachiningChargeVersions");
        }
    }
}
