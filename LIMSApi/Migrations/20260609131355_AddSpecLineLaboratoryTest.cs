using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecLineLaboratoryTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_LaboratoryTestID",
                table: "SpecificationLines",
                column: "LaboratoryTestID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationLines_LaboratoryTests_LaboratoryTestID",
                table: "SpecificationLines",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationLines_LaboratoryTests_LaboratoryTestID",
                table: "SpecificationLines");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationLines_LaboratoryTestID",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID",
                table: "SpecificationLines");
        }
    }
}
