using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddParameterUnitEquivalentToProductSizeAndDimensionalFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitEquivalentID",
                table: "ProductSizeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitEquivalentID",
                table: "DimensionalFactorMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizeMasters_ParameterUnitEquivalentID",
                table: "ProductSizeMasters",
                column: "ParameterUnitEquivalentID");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorMasters_ParameterUnitEquivalentID",
                table: "DimensionalFactorMasters",
                column: "ParameterUnitEquivalentID");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionalFactorMasters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "DimensionalFactorMasters",
                column: "ParameterUnitEquivalentID",
                principalTable: "ParameterUnitEquivalents",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeMasters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "ProductSizeMasters",
                column: "ParameterUnitEquivalentID",
                principalTable: "ParameterUnitEquivalents",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionalFactorMasters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeMasters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "ProductSizeMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductSizeMasters_ParameterUnitEquivalentID",
                table: "ProductSizeMasters");

            migrationBuilder.DropIndex(
                name: "IX_DimensionalFactorMasters_ParameterUnitEquivalentID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "ParameterUnitEquivalentID",
                table: "ProductSizeMasters");

            migrationBuilder.DropColumn(
                name: "ParameterUnitEquivalentID",
                table: "DimensionalFactorMasters");
        }
    }
}
