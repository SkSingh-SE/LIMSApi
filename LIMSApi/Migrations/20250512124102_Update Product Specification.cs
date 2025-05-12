using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductSpecifications",
                newName: "SpecificationName");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "ProductSpecifications",
                newName: "SpecificationCode");

            migrationBuilder.AddColumn<long>(
                name: "MateriaSpecificationID",
                table: "ProductSpecifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_MateriaSpecificationID",
                table: "ProductSpecifications",
                column: "MateriaSpecificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_SpecificationHeaders_MateriaSpecificationID",
                table: "ProductSpecifications",
                column: "MateriaSpecificationID",
                principalTable: "SpecificationHeaders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_SpecificationHeaders_MateriaSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_ProductSpecifications_MateriaSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropColumn(
                name: "MateriaSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.RenameColumn(
                name: "SpecificationName",
                table: "ProductSpecifications",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SpecificationCode",
                table: "ProductSpecifications",
                newName: "Code");
        }
    }
}
