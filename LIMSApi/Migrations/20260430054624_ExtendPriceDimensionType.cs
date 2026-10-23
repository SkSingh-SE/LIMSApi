using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ExtendPriceDimensionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SampleField",
                table: "PriceDimensionTypes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValueSource",
                table: "PriceDimensionTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "FallbackToUserInput",
                table: "InvoiceCaseConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleField",
                table: "PriceDimensionTypes");

            migrationBuilder.DropColumn(
                name: "ValueSource",
                table: "PriceDimensionTypes");

            migrationBuilder.DropColumn(
                name: "FallbackToUserInput",
                table: "InvoiceCaseConfigurations");
        }
    }
}
