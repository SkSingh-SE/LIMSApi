using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddParameterUnitEquivalentToParameterMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitEquivalentID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitConversionFactor",
                table: "ParameterMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_ParameterUnitEquivalentID",
                table: "ParameterMasters",
                column: "ParameterUnitEquivalentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "ParameterMasters",
                column: "ParameterUnitEquivalentID",
                principalTable: "ParameterUnitEquivalents",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                table: "ParameterMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMasters_ParameterUnitEquivalentID",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "ParameterUnitEquivalentID",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "UnitConversionFactor",
                table: "ParameterMasters");
        }
    }
}
