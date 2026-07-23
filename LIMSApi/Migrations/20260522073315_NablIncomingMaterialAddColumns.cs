using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablIncomingMaterialAddColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralRemarks",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GstNo",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndentNoPoNo",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionPlanNo",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionStage",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemsParametersJson",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderType",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNo",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoNo",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskLevel",
                table: "NablIncomingMaterials",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "GeneralRemarks",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "GstNo",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "IndentNoPoNo",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "InspectionPlanNo",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "InspectionStage",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "ItemsParametersJson",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "PhoneNo",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "PoNo",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "NablIncomingMaterials");
        }
    }
}
