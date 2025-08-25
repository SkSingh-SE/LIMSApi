using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddKeySampleDetailsID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleDetailID",
                table: "SampleAdditionalDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleAdditionalDetails_SampleDetailID",
                table: "SampleAdditionalDetails");

            migrationBuilder.DropColumn(
                name: "SampleDetailID",
                table: "SampleAdditionalDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SampleDetailID",
                table: "SampleAdditionalDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SampleAdditionalDetails_SampleDetailID",
                table: "SampleAdditionalDetails",
                column: "SampleDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleDetailID",
                table: "SampleAdditionalDetails",
                column: "SampleDetailID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
