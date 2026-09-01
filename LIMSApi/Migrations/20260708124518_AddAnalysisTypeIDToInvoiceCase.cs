using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisTypeIDToInvoiceCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AnalysisTypeID",
                table: "InvoiceCases",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCases_AnalysisTypeID",
                table: "InvoiceCases",
                column: "AnalysisTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceCases_LaboratoryTestAnalysisTypes_AnalysisTypeID",
                table: "InvoiceCases",
                column: "AnalysisTypeID",
                principalTable: "LaboratoryTestAnalysisTypes",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceCases_LaboratoryTestAnalysisTypes_AnalysisTypeID",
                table: "InvoiceCases");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceCases_AnalysisTypeID",
                table: "InvoiceCases");

            migrationBuilder.DropColumn(
                name: "AnalysisTypeID",
                table: "InvoiceCases");
        }
    }
}
