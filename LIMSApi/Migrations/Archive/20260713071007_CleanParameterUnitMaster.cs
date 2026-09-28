using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class CleanParameterUnitMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversaionFactor",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor1",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor2",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor3",
                table: "ParameterUnitMasters");

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
                name: "SimilarUnit1",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit2",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "SimilarUnit3",
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

            migrationBuilder.RenameColumn(
                name: "ConversionFactor7",
                table: "ParameterUnitMasters",
                newName: "ConversionFactor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConversionFactor",
                table: "ParameterUnitMasters",
                newName: "ConversionFactor7");

            migrationBuilder.AddColumn<string>(
                name: "ConversaionFactor",
                table: "ParameterUnitMasters",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor1",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor2",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor3",
                table: "ParameterUnitMasters",
                type: "decimal(18,6)",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit1",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit2",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimilarUnit3",
                table: "ParameterUnitMasters",
                type: "nvarchar(50)",
                maxLength: 50,
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
    }
}
