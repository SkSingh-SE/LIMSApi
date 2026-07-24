using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablRiskAssessmentAddcols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewDate",
                table: "NablRiskAssessments",
                newName: "RiskDate");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "NablRiskAssessments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Effectiveness",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EffectivenessRemarks",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExistingControls",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExistingSituation",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpectedBenefit",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdentifiedById",
                table: "NablRiskAssessments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentifiedByName",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Impact",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Likelihood",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Opportunity",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskLevel",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskNo",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskOwner",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskRemarks",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskScore",
                table: "NablRiskAssessments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "NablRiskAssessments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "Effectiveness",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EffectivenessRemarks",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "ExistingControls",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "ExistingSituation",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "ExpectedBenefit",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "IdentifiedById",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "IdentifiedByName",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "Impact",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "Likelihood",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "Opportunity",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "RiskNo",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "RiskOwner",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "RiskRemarks",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "RiskScore",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "NablRiskAssessments");

            migrationBuilder.RenameColumn(
                name: "RiskDate",
                table: "NablRiskAssessments",
                newName: "ReviewDate");
        }
    }
}
