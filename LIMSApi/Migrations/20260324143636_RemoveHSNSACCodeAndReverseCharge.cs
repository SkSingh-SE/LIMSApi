using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHSNSACCodeAndReverseCharge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HSNSACCode",
                table: "GstConfigs");

            migrationBuilder.DropColumn(
                name: "ReverseChargeApplicable",
                table: "GstConfigs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HSNSACCode",
                table: "GstConfigs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ReverseChargeApplicable",
                table: "GstConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
