using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDimensionalFactorFKToTestMethodSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodStandards_DefaultTestMethodID",
                table: "DimensionalFactorMasters");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodSpecifications_DefaultTestMethodID",
                table: "DimensionalFactorMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodSpecifications_DefaultTestMethodID",
                table: "DimensionalFactorMasters");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionalFactorMasters_TestMethodStandards_DefaultTestMethodID",
                table: "DimensionalFactorMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodStandards",
                principalColumn: "ID");
        }
    }
}
