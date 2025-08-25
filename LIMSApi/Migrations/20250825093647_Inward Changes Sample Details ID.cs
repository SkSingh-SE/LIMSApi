using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class InwardChangesSampleDetailsID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleID",
                table: "SampleAdditionalDetails");

            migrationBuilder.RenameColumn(
                name: "SampleID",
                table: "SampleAdditionalDetails",
                newName: "SampleDetailID");

            migrationBuilder.RenameIndex(
                name: "IX_SampleAdditionalDetails_SampleID",
                table: "SampleAdditionalDetails",
                newName: "IX_SampleAdditionalDetails_SampleDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleDetailID",
                table: "SampleAdditionalDetails",
                column: "SampleDetailID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleDetailID",
                table: "SampleAdditionalDetails");

            migrationBuilder.RenameColumn(
                name: "SampleDetailID",
                table: "SampleAdditionalDetails",
                newName: "SampleID");

            migrationBuilder.RenameIndex(
                name: "IX_SampleAdditionalDetails_SampleDetailID",
                table: "SampleAdditionalDetails",
                newName: "IX_SampleAdditionalDetails_SampleID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleID",
                table: "SampleAdditionalDetails",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
