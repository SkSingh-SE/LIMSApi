using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecificationGradeIDToLaboratoryTestSpecifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SpecificationGradeID",
                table: "LaboratoryTestSubGroupSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpecificationGradeID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_SpecificationGradeID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_SpecificationGradeID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "SpecificationGradeID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestAnalysisTypeSpecifications_SpecificationGrades_SpecificationGradeID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "SpecificationGradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestSubGroupSpecifications_SpecificationGrades_SpecificationGradeID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "SpecificationGradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTestAnalysisTypeSpecifications_SpecificationGrades_SpecificationGradeID",
                table: "LaboratoryTestAnalysisTypeSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTestSubGroupSpecifications_SpecificationGrades_SpecificationGradeID",
                table: "LaboratoryTestSubGroupSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_SpecificationGradeID",
                table: "LaboratoryTestSubGroupSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_SpecificationGradeID",
                table: "LaboratoryTestAnalysisTypeSpecifications");

            migrationBuilder.DropColumn(
                name: "SpecificationGradeID",
                table: "LaboratoryTestSubGroupSpecifications");

            migrationBuilder.DropColumn(
                name: "SpecificationGradeID",
                table: "LaboratoryTestAnalysisTypeSpecifications");
        }
    }
}
