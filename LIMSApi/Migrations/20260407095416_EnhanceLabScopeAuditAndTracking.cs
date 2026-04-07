using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceLabScopeAuditAndTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "LabScopeSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "LIMS");

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "LabScopeSpecifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "LabScopeSpecifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LabScopeSpecifications",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "LabScopeSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "LabScopeSpecifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "LabScopeSpecificationParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "LIMS");

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "LabScopeSpecificationParameters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "LabScopeSpecificationParameters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LabScopeSpecificationParameters",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "LabScopeSpecificationParameters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "LabScopeSpecificationParameters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextReviewDate",
                table: "LabScopeMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScopeRemarks",
                table: "LabScopeMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "LabScopeMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidUntil",
                table: "LabScopeMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LabScopeChangeLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabScopeID = table.Column<long>(type: "bigint", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangedBy = table.Column<long>(type: "bigint", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeChangeLogs", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabScopeChangeLogs");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "LabScopeSpecifications");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LabScopeSpecifications");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "LabScopeSpecifications");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LabScopeSpecifications");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "LabScopeSpecifications");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "LabScopeSpecifications");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "LabScopeSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LabScopeSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "LabScopeSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LabScopeSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "LabScopeSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "LabScopeSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "NextReviewDate",
                table: "LabScopeMasters");

            migrationBuilder.DropColumn(
                name: "ScopeRemarks",
                table: "LabScopeMasters");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "LabScopeMasters");

            migrationBuilder.DropColumn(
                name: "ValidUntil",
                table: "LabScopeMasters");
        }
    }
}
