using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class TestMethodSpecParamRemoveSymbolMinMax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "TestMethodSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "TestMethodSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "TestMethodSpecificationParameters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxValue",
                table: "TestMethodSpecificationParameters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValue",
                table: "TestMethodSpecificationParameters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "TestMethodSpecificationParameters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
