using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGstConfigInvoiceSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaultGstRate",
                table: "GstConfigs",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 18m);

            migrationBuilder.AddColumn<string>(
                name: "HSNSACCode",
                table: "GstConfigs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "998346");

            migrationBuilder.AddColumn<bool>(
                name: "PIGstApplicable",
                table: "GstConfigs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReverseChargeApplicable",
                table: "GstConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultGstRate",
                table: "GstConfigs");

            migrationBuilder.DropColumn(
                name: "HSNSACCode",
                table: "GstConfigs");

            migrationBuilder.DropColumn(
                name: "PIGstApplicable",
                table: "GstConfigs");

            migrationBuilder.DropColumn(
                name: "ReverseChargeApplicable",
                table: "GstConfigs");
        }
    }
}
