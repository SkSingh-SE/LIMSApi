using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleDimensionsAndPricingValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricingDimensionValue",
                table: "TestResultHeaders",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Diameter",
                table: "SampleDetails",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "SampleDetails",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Thickness",
                table: "SampleDetails",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "SampleDetails",
                type: "decimal(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricingDimensionValue",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "Diameter",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "Thickness",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "SampleDetails");
        }
    }
}
