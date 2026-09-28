using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdditionSampleDetailsrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleAdditionalDetails_SampleInwards_SampleInwardID",
                table: "SampleAdditionalDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleAdditionalDetails_SampleInwardID",
                table: "SampleAdditionalDetails");

            migrationBuilder.DropColumn(
                name: "SampleInwardID",
                table: "SampleAdditionalDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SampleInwardID",
                table: "SampleAdditionalDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleAdditionalDetails_SampleInwardID",
                table: "SampleAdditionalDetails",
                column: "SampleInwardID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleAdditionalDetails_SampleInwards_SampleInwardID",
                table: "SampleAdditionalDetails",
                column: "SampleInwardID",
                principalTable: "SampleInwards",
                principalColumn: "ID");
        }
    }
}
