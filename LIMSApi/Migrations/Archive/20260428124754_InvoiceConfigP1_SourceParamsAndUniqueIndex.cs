using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceConfigP1_SourceParamsAndUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceCaseConfigurations_Name",
                table: "InvoiceCaseConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "SourceParameterIDs",
                table: "InvoiceCaseConfigurations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCaseConfigurations_Name_SelectionType",
                table: "InvoiceCaseConfigurations",
                columns: new[] { "Name", "SelectionType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceCaseConfigurations_Name_SelectionType",
                table: "InvoiceCaseConfigurations");

            migrationBuilder.DropColumn(
                name: "SourceParameterIDs",
                table: "InvoiceCaseConfigurations");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCaseConfigurations_Name",
                table: "InvoiceCaseConfigurations",
                column: "Name",
                unique: true);
        }
    }
}
