using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class MovePreparationToTestMethodsAndAddTpiInspectors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MachiningAmount",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "MachiningRequired",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "OtherPreparation",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "OtherPreparationCharge",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "PreparationRequired",
                table: "SampleDetails");

            migrationBuilder.AddColumn<string>(
                name: "TpiInspectorsJson",
                table: "SampleDetails",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PreparationRequired",
                table: "GeneralTestMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PreparationRequired",
                table: "ChemicalTestMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TpiInspectorsJson",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "PreparationRequired",
                table: "GeneralTestMethods");

            migrationBuilder.DropColumn(
                name: "PreparationRequired",
                table: "ChemicalTestMethods");

            migrationBuilder.AddColumn<decimal>(
                name: "MachiningAmount",
                table: "SampleDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "MachiningRequired",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OtherPreparation",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherPreparationCharge",
                table: "SampleDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "PreparationRequired",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
