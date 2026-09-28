using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSpecificationGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_SpecificationHeaders_MaterialSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.RenameColumn(
                name: "MaterialSpecificationID",
                table: "ProductSpecifications",
                newName: "GradeID");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSpecifications_MaterialSpecificationID",
                table: "ProductSpecifications",
                newName: "IX_ProductSpecifications_GradeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_SpecificationGrades_GradeID",
                table: "ProductSpecifications",
                column: "GradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_SpecificationGrades_GradeID",
                table: "ProductSpecifications");

            migrationBuilder.RenameColumn(
                name: "GradeID",
                table: "ProductSpecifications",
                newName: "MaterialSpecificationID");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSpecifications_GradeID",
                table: "ProductSpecifications",
                newName: "IX_ProductSpecifications_MaterialSpecificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_SpecificationHeaders_MaterialSpecificationID",
                table: "ProductSpecifications",
                column: "MaterialSpecificationID",
                principalTable: "SpecificationHeaders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
