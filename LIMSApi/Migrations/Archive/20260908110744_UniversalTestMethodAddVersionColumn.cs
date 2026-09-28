using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UniversalTestMethodAddVersionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods",
                type: "bigint",
                nullable: true);

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTestMethods_LaboratoryTestID_TestMethodSpecificationID",
                table: "LaboratoryTestMethods");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestMethods_LaboratoryTestID_TestMethodSpecificationID_TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods",
                columns: new[] { "LaboratoryTestID", "TestMethodSpecificationID", "TestMethodSpecificationVersionID" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods",
                column: "TestMethodSpecificationVersionID",
                principalTable: "TestMethodSpecificationVersions",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods",
                column: "TestMethodSpecificationVersionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTestMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTestMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTestMethods_LaboratoryTestID_TestMethodSpecificationID_TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestMethods_LaboratoryTestID_TestMethodSpecificationID",
                table: "LaboratoryTestMethods",
                columns: new[] { "LaboratoryTestID", "TestMethodSpecificationID" },
                unique: true);
        }
    }
}
