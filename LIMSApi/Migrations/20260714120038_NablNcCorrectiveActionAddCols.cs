using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablNcCorrectiveActionAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityAssessed",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditNo",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Auditee",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Auditor",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClauseNo",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CorrectiveActionDate",
                table: "NablNcCorrectiveActions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectiveActionProposed",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentID",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EffectivenessOfAction",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ImplementedById",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImplementedByName",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImplementedDate",
                table: "NablNcCorrectiveActions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NcNo",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NcObserved",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ObservedByID",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservedByName",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProposedById",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProposedByName",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SignOfAuditorID",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignOfAuditorName",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SignatureOfQMID",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureOfQMName",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeRequirement",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VerifiedById",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedByName",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "NablNcCorrectiveActions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityAssessed",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "AuditNo",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "Auditee",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "Auditor",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ClauseNo",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "CorrectiveActionDate",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "CorrectiveActionProposed",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "EffectivenessOfAction",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ImplementedById",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ImplementedByName",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ImplementedDate",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "NcNo",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "NcObserved",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ObservedByID",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ObservedByName",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ProposedById",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ProposedByName",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "SignOfAuditorID",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "SignOfAuditorName",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "SignatureOfQMID",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "SignatureOfQMName",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "TimeRequirement",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "VerifiedById",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "VerifiedByName",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "NablNcCorrectiveActions");
        }
    }
}
