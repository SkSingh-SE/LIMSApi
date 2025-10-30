using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedFieldofmaterialspecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TestMethodSpecificationID",
                table: "SpecificationHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaders_TestMethodSpecificationID",
                table: "SpecificationHeaders",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTestMappings_GradeID",
                table: "MaterialTestMappings",
                column: "GradeID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTestMappings_MetalClassificationID",
                table: "MaterialTestMappings",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTestMappings_ProductConditionID",
                table: "MaterialTestMappings",
                column: "ProductConditionID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_MetalClassificationMasters_MetalClassificationID",
                table: "MaterialTestMappings",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_ProductConditionMasters_ProductConditionID",
                table: "MaterialTestMappings",
                column: "ProductConditionID",
                principalTable: "ProductConditionMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_SpecificationGrades_GradeID",
                table: "MaterialTestMappings",
                column: "GradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationHeaders_TestMethodSpecifications_TestMethodSpecificationID",
                table: "SpecificationHeaders",
                column: "TestMethodSpecificationID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_MetalClassificationMasters_MetalClassificationID",
                table: "MaterialTestMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_ProductConditionMasters_ProductConditionID",
                table: "MaterialTestMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_SpecificationGrades_GradeID",
                table: "MaterialTestMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationHeaders_TestMethodSpecifications_TestMethodSpecificationID",
                table: "SpecificationHeaders");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationHeaders_TestMethodSpecificationID",
                table: "SpecificationHeaders");

            migrationBuilder.DropIndex(
                name: "IX_MaterialTestMappings_GradeID",
                table: "MaterialTestMappings");

            migrationBuilder.DropIndex(
                name: "IX_MaterialTestMappings_MetalClassificationID",
                table: "MaterialTestMappings");

            migrationBuilder.DropIndex(
                name: "IX_MaterialTestMappings_ProductConditionID",
                table: "MaterialTestMappings");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationID",
                table: "SpecificationHeaders");
        }
    }
}
