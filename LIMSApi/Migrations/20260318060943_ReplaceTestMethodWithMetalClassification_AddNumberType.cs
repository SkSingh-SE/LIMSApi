using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceTestMethodWithMetalClassification_AddNumberType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationGrades_TestMethodSpecifications_TestMethodSpecificationID",
                table: "SpecificationGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationHeaders_TestMethodSpecifications_TestMethodSpecificationID",
                table: "SpecificationHeaders");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationHeaders_TestMethodSpecificationID",
                table: "SpecificationHeaders");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationID",
                table: "SpecificationHeaders");

            migrationBuilder.RenameColumn(
                name: "TestMethodSpecificationID",
                table: "SpecificationGrades",
                newName: "MetalClassificationID");

            migrationBuilder.RenameIndex(
                name: "IX_SpecificationGrades_TestMethodSpecificationID",
                table: "SpecificationGrades",
                newName: "IX_SpecificationGrades_MetalClassificationID");

            migrationBuilder.AddColumn<string>(
                name: "NumberType",
                table: "StandardOrganizationMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationGrades_MetalClassificationMasters_MetalClassificationID",
                table: "SpecificationGrades",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationGrades_MetalClassificationMasters_MetalClassificationID",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "NumberType",
                table: "StandardOrganizationMasters");

            migrationBuilder.RenameColumn(
                name: "MetalClassificationID",
                table: "SpecificationGrades",
                newName: "TestMethodSpecificationID");

            migrationBuilder.RenameIndex(
                name: "IX_SpecificationGrades_MetalClassificationID",
                table: "SpecificationGrades",
                newName: "IX_SpecificationGrades_TestMethodSpecificationID");

            migrationBuilder.AddColumn<long>(
                name: "TestMethodSpecificationID",
                table: "SpecificationHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaders_TestMethodSpecificationID",
                table: "SpecificationHeaders",
                column: "TestMethodSpecificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationGrades_TestMethodSpecifications_TestMethodSpecificationID",
                table: "SpecificationGrades",
                column: "TestMethodSpecificationID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationHeaders_TestMethodSpecifications_TestMethodSpecificationID",
                table: "SpecificationHeaders",
                column: "TestMethodSpecificationID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");
        }
    }
}
