using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class LaboratoryTestUpdatelinearstructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "TestMethodMasters");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCase",
                table: "TestMethodMasters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubGroup",
                table: "TestMethodMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceCase",
                table: "TestMethodMasters");

            migrationBuilder.DropColumn(
                name: "SubGroup",
                table: "TestMethodMasters");

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "TestMethodMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
