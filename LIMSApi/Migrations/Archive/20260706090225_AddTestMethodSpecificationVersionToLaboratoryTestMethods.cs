using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTestMethodSpecificationVersionToLaboratoryTestMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TestMethodSpecificationVersionID",
                table: "LaboratoryTestSubGroupMethods",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestMethodSpecificationVersionID",
                table: "LaboratoryTestAnalysisTypeMethods",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestSubGroupMethods",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestAnalysisTypeMethods",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestAnalysisTypeMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "LaboratoryTestAnalysisTypeMethods",
                column: "TestMethodSpecificationVersionID",
                principalTable: "TestMethodSpecificationVersions",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestSubGroupMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "LaboratoryTestSubGroupMethods",
                column: "TestMethodSpecificationVersionID",
                principalTable: "TestMethodSpecificationVersions",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTestAnalysisTypeMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "LaboratoryTestAnalysisTypeMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTestSubGroupMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "LaboratoryTestSubGroupMethods");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTestSubGroupMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestSubGroupMethods");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTestAnalysisTypeMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestAnalysisTypeMethods");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationVersionID",
                table: "LaboratoryTestSubGroupMethods");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationVersionID",
                table: "LaboratoryTestAnalysisTypeMethods");
        }
    }
}
