using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecLineLimitDecimalValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LowerLimitDecimalValue",
                table: "SpecificationLines",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UpperLimitDecimalValue",
                table: "SpecificationLines",
                type: "decimal(18,6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LowerLimitDecimalValue",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "UpperLimitDecimalValue",
                table: "SpecificationLines");
        }
    }
}
