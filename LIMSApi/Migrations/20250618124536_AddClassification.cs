using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MetalClassificationID",
                table: "LaboratoryTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_MetalClassificationID",
                table: "LaboratoryTests",
                column: "MetalClassificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTests_MetalClassificationMasters_MetalClassificationID",
                table: "LaboratoryTests",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTests_MetalClassificationMasters_MetalClassificationID",
                table: "LaboratoryTests");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTests_MetalClassificationID",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "MetalClassificationID",
                table: "LaboratoryTests");
        }
    }
}
