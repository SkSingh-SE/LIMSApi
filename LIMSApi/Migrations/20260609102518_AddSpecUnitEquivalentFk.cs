using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecUnitEquivalentFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitEquivalentID",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitEquivalentID",
                table: "SpecificationHeaderParameters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ParameterUnitEquivalentID",
                table: "SpecificationLines",
                column: "ParameterUnitEquivalentID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaderParameters_ParameterUnitEquivalentID",
                table: "SpecificationHeaderParameters",
                column: "ParameterUnitEquivalentID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationHeaderParameters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "SpecificationHeaderParameters",
                column: "ParameterUnitEquivalentID",
                principalTable: "ParameterUnitEquivalents",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationLines_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "SpecificationLines",
                column: "ParameterUnitEquivalentID",
                principalTable: "ParameterUnitEquivalents",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationHeaderParameters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "SpecificationHeaderParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationLines_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "SpecificationLines");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationLines_ParameterUnitEquivalentID",
                table: "SpecificationLines");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationHeaderParameters_ParameterUnitEquivalentID",
                table: "SpecificationHeaderParameters");

            migrationBuilder.DropColumn(
                name: "ParameterUnitEquivalentID",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "ParameterUnitEquivalentID",
                table: "SpecificationHeaderParameters");
        }
    }
}
