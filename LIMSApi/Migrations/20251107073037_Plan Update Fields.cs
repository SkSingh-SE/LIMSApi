using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class PlanUpdateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxValue",
                table: "ChemicalTestElement",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValue",
                table: "ChemicalTestElement",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParameterUnit",
                table: "ChemicalTestElement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitID",
                table: "ChemicalTestElement",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SpecificationLineID",
                table: "ChemicalTestElement",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTestMappings_LaboratoryTestID",
                table: "MaterialTestMappings",
                column: "LaboratoryTestID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_LaboratoryTests_LaboratoryTestID",
                table: "MaterialTestMappings",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_LaboratoryTests_LaboratoryTestID",
                table: "MaterialTestMappings");

            migrationBuilder.DropIndex(
                name: "IX_MaterialTestMappings_LaboratoryTestID",
                table: "MaterialTestMappings");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "ChemicalTestElement");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "ChemicalTestElement");

            migrationBuilder.DropColumn(
                name: "ParameterUnit",
                table: "ChemicalTestElement");

            migrationBuilder.DropColumn(
                name: "ParameterUnitID",
                table: "ChemicalTestElement");

            migrationBuilder.DropColumn(
                name: "SpecificationLineID",
                table: "ChemicalTestElement");
        }
    }
}
