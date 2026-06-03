using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNablProductInspectionPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "NablProductInspections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionStage",
                table: "NablProductInspections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "NablProductInspections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "NablProductInspections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "NablProductInspections",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "NablProductInspections");

            migrationBuilder.DropColumn(
                name: "InspectionStage",
                table: "NablProductInspections");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "NablProductInspections");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "NablProductInspections");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "NablProductInspections");
        }
    }
}
