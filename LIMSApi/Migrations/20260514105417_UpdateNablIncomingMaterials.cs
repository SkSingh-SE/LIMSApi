using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNablIncomingMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectiveActions",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Deviations",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrnNo",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionParameterJson",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotNo",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialCode",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialName",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectiveActions",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "Deviations",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "GrnNo",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "InspectionParameterJson",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "LotNo",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "MaterialCode",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "MaterialName",
                table: "NablIncomingMaterials");
        }
    }
}
