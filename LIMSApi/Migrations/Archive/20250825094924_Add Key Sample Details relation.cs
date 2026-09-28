using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddKeySampleDetailsrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SampleID",
                table: "SampleAdditionalDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SampleAdditionalDetails_SampleID",
                table: "SampleAdditionalDetails",
                column: "SampleID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleID",
                table: "SampleAdditionalDetails",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleID",
                table: "SampleAdditionalDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleAdditionalDetails_SampleID",
                table: "SampleAdditionalDetails");

            migrationBuilder.DropColumn(
                name: "SampleID",
                table: "SampleAdditionalDetails");
        }
    }
}
