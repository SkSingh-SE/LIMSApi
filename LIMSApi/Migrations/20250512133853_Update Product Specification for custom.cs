using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSpecificationforcustom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialSpecification",
                table: "ProductSpecifications");

            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "ProductSpecifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "ProductSpecifications");

            migrationBuilder.AddColumn<string>(
                name: "MaterialSpecification",
                table: "ProductSpecifications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
