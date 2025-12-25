using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class TestResultupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowNumeric",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "TextValue",
                table: "TestResultParameters");

            migrationBuilder.RenameColumn(
                name: "NumericValue",
                table: "TestResultParameters",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "AllowText",
                table: "TestResultParameters",
                newName: "IsAdditional");

            migrationBuilder.AddColumn<long>(
                name: "TestID",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestID",
                table: "TestResultHeaders");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "TestResultParameters",
                newName: "NumericValue");

            migrationBuilder.RenameColumn(
                name: "IsAdditional",
                table: "TestResultParameters",
                newName: "AllowText");

            migrationBuilder.AddColumn<bool>(
                name: "AllowNumeric",
                table: "TestResultParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TextValue",
                table: "TestResultParameters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
