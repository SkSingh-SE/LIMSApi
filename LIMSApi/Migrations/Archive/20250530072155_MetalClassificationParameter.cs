using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class MetalClassificationParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameter_MetalClassificationMasters_MetalClassificationMasterID",
                table: "MetalClassificationParameter");

            migrationBuilder.DropIndex(
                name: "IX_MetalClassificationParameter_MetalClassificationMasterID",
                table: "MetalClassificationParameter");

            migrationBuilder.DropColumn(
                name: "MetalClassificationMasterID",
                table: "MetalClassificationParameter");

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationParameter_ParameterID",
                table: "MetalClassificationParameter",
                column: "ParameterID");

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameter_MetalClassificationMasters_MetalClassificationID",
                table: "MetalClassificationParameter",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameter_ParameterMasters_ParameterID",
                table: "MetalClassificationParameter",
                column: "ParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameter_MetalClassificationMasters_MetalClassificationID",
                table: "MetalClassificationParameter");

            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameter_ParameterMasters_ParameterID",
                table: "MetalClassificationParameter");

            migrationBuilder.DropIndex(
                name: "IX_MetalClassificationParameter_ParameterID",
                table: "MetalClassificationParameter");

            migrationBuilder.AddColumn<long>(
                name: "MetalClassificationMasterID",
                table: "MetalClassificationParameter",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationParameter_MetalClassificationMasterID",
                table: "MetalClassificationParameter",
                column: "MetalClassificationMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameter_MetalClassificationMasters_MetalClassificationMasterID",
                table: "MetalClassificationParameter",
                column: "MetalClassificationMasterID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");
        }
    }
}
