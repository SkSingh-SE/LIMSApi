using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablInternalAuditorAddcols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ISOClauses",
                table: "NablInternalAuditors",
                newName: "ISOClaus");

            migrationBuilder.AlterColumn<bool>(
                name: "LeadAuditorCourse",
                table: "NablInternalAuditors",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "InternalAuditorCourse",
                table: "NablInternalAuditors",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AuditExperience",
                table: "NablInternalAuditors",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AuthorizedById",
                table: "NablInternalAuditors",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorizedByName",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CertificateExpiryDate",
                table: "NablInternalAuditors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CertificateIssueDate",
                table: "NablInternalAuditors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateNo",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "NablInternalAuditors",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentListJson",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ISOClausesJson",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrainingOrganization",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizedById",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "AuthorizedByName",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "CertificateExpiryDate",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "CertificateIssueDate",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "CertificateNo",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "DepartmentListJson",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "ISOClausesJson",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "TrainingOrganization",
                table: "NablInternalAuditors");

            migrationBuilder.RenameColumn(
                name: "ISOClaus",
                table: "NablInternalAuditors",
                newName: "ISOClauses");

            migrationBuilder.AlterColumn<string>(
                name: "LeadAuditorCourse",
                table: "NablInternalAuditors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InternalAuditorCourse",
                table: "NablInternalAuditors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuditExperience",
                table: "NablInternalAuditors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
