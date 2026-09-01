using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLaboratoryTestAnalysisTypeIDToChemicalTestType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestTypes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestTypes",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestTypes_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestTypes",
                column: "LaboratoryTestAnalysisTypeID",
                principalTable: "LaboratoryTestAnalysisTypes",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestTypes_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestTypes");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTestTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestTypes");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestTypes");
        }
    }
}
