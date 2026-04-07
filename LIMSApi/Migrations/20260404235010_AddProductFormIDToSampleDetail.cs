using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProductFormIDToSampleDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductFormID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ProductFormID",
                table: "SampleDetails",
                column: "ProductFormID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_ProductFormMasters_ProductFormID",
                table: "SampleDetails",
                column: "ProductFormID",
                principalTable: "ProductFormMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_ProductFormMasters_ProductFormID",
                table: "SampleDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_ProductFormID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "ProductFormID",
                table: "SampleDetails");
        }
    }
}
