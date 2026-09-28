using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSimilarUnits4to7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor4",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor5",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor6",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor7",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit4",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit5",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit6",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit7",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversionFactor4",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor5",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor6",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor7",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit4",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit5",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit6",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit7",
                table: "ParameterUnitMasters");
        }
    }
}
