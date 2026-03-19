using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class DimensionalFactorUnitForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "DimensionalFactorMasters");

            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitID",
                table: "DimensionalFactorMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorMasters_ParameterUnitID",
                table: "DimensionalFactorMasters",
                column: "ParameterUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionalFactorMasters_ParameterUnitMasters_ParameterUnitID",
                table: "DimensionalFactorMasters",
                column: "ParameterUnitID",
                principalTable: "ParameterUnitMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionalFactorMasters_ParameterUnitMasters_ParameterUnitID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropIndex(
                name: "IX_DimensionalFactorMasters_ParameterUnitID",
                table: "DimensionalFactorMasters");

            migrationBuilder.DropColumn(
                name: "ParameterUnitID",
                table: "DimensionalFactorMasters");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "DimensionalFactorMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
