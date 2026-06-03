using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ProductSizeUnitFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ProductSizeMasters");

            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitID",
                table: "ProductSizeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizeMasters_ParameterUnitID",
                table: "ProductSizeMasters",
                column: "ParameterUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ProductSizeMasters",
                column: "ParameterUnitID",
                principalTable: "ParameterUnitMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ProductSizeMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProductSizeMasters_ParameterUnitID",
                table: "ProductSizeMasters");

            migrationBuilder.DropColumn(
                name: "ParameterUnitID",
                table: "ProductSizeMasters");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "ProductSizeMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
