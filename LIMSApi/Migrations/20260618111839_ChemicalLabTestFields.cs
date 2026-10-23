using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ChemicalLabTestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTests_SubGroup",
                table: "LaboratoryTests");

            migrationBuilder.AddColumn<string>(
                name: "ChemicalCategory",
                table: "LaboratoryTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChemicalTest",
                table: "LaboratoryTests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChemicalCategory",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "IsChemicalTest",
                table: "LaboratoryTests");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_SubGroup",
                table: "LaboratoryTests",
                column: "SubGroup",
                unique: true);
        }
    }
}
