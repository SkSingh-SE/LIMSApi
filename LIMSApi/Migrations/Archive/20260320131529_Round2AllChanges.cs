using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Round2AllChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OrientationDeviationAcknowledged",
                table: "TestResultHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FormatType",
                table: "ReportTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HeaderConfigJson",
                table: "ReportTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageLayout",
                table: "ReportTemplates",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SectionVisibilityJson",
                table: "ReportTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowCalibrationTable",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowNablColumn",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowSpecificationRow",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SignatoryConfigJson",
                table: "ReportTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WatermarkText",
                table: "ReportTemplates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrientationDeviationAcknowledged",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "FormatType",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "HeaderConfigJson",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "PageLayout",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "SectionVisibilityJson",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "ShowCalibrationTable",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "ShowNablColumn",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "ShowSpecificationRow",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "SignatoryConfigJson",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "WatermarkText",
                table: "ReportTemplates");
        }
    }
}
