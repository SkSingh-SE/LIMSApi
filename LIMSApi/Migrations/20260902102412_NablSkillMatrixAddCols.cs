using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablSkillMatrixAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablSkillMatrices_DesignationMasters_DesignationId",
                table: "NablSkillMatrices");

            migrationBuilder.DropIndex(
                name: "IX_NablSkillMatrices_DesignationId",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "Decision",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "IssuedBy",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "ReviewedApprovedBy",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "NablSkillMatrices");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "NablSkillMatrices",
                newName: "EvaluationDate");

            migrationBuilder.RenameColumn(
                name: "EmployeeSkillsJson",
                table: "NablSkillMatrices",
                newName: "EmployeeName");

            migrationBuilder.AddColumn<string>(
                name: "AverageRequiredSkill",
                table: "NablSkillMatrices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AverageRequiredSkillLevel",
                table: "NablSkillMatrices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "NablSkillMatrices",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRequiredSkill",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "AverageRequiredSkillLevel",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "NablSkillMatrices");

            migrationBuilder.RenameColumn(
                name: "EvaluationDate",
                table: "NablSkillMatrices",
                newName: "LastUpdated");

            migrationBuilder.RenameColumn(
                name: "EmployeeName",
                table: "NablSkillMatrices",
                newName: "EmployeeSkillsJson");

            migrationBuilder.AddColumn<string>(
                name: "Decision",
                table: "NablSkillMatrices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuedBy",
                table: "NablSkillMatrices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedApprovedBy",
                table: "NablSkillMatrices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "NablSkillMatrices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablSkillMatrices_DesignationId",
                table: "NablSkillMatrices",
                column: "DesignationId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablSkillMatrices_DesignationMasters_DesignationId",
                table: "NablSkillMatrices",
                column: "DesignationId",
                principalTable: "DesignationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
