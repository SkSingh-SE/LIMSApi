using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Updatedinward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "Nature",
                table: "SampleDetails");

            migrationBuilder.AddColumn<long>(
                name: "MetalClassificationID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductConditionID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetalClassificationID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "ProductConditionID",
                table: "SampleDetails");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nature",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
