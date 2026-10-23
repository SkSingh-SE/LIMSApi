using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOverrideParameterIDToInvoiceCaseConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OverrideParameterID",
                table: "InvoiceCaseConfigurations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCaseConfigurations_OverrideParameterID",
                table: "InvoiceCaseConfigurations",
                column: "OverrideParameterID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceCaseConfigurations_ParameterMasters_OverrideParameterID",
                table: "InvoiceCaseConfigurations",
                column: "OverrideParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceCaseConfigurations_ParameterMasters_OverrideParameterID",
                table: "InvoiceCaseConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceCaseConfigurations_OverrideParameterID",
                table: "InvoiceCaseConfigurations");

            migrationBuilder.DropColumn(
                name: "OverrideParameterID",
                table: "InvoiceCaseConfigurations");
        }
    }
}
