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
            // LabScopeSpecifications columns (conditional)
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecifications' AND COLUMN_NAME='CompanyCode') ALTER TABLE [LabScopeSpecifications] ADD [CompanyCode] nvarchar(max) NOT NULL DEFAULT 'LIMS';");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecifications' AND COLUMN_NAME='CreatedBy') ALTER TABLE [LabScopeSpecifications] ADD [CreatedBy] bigint NOT NULL DEFAULT 0;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecifications' AND COLUMN_NAME='CreatedOn') ALTER TABLE [LabScopeSpecifications] ADD [CreatedOn] datetime2 NOT NULL DEFAULT '0001-01-01';");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecifications' AND COLUMN_NAME='IsActive') ALTER TABLE [LabScopeSpecifications] ADD [IsActive] bit NOT NULL DEFAULT 1;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecifications' AND COLUMN_NAME='ModifiedBy') ALTER TABLE [LabScopeSpecifications] ADD [ModifiedBy] bigint NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecifications' AND COLUMN_NAME='ModifiedOn') ALTER TABLE [LabScopeSpecifications] ADD [ModifiedOn] datetime2 NULL;");

            // LabScopeSpecificationParameters columns (conditional)
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecificationParameters' AND COLUMN_NAME='CompanyCode') ALTER TABLE [LabScopeSpecificationParameters] ADD [CompanyCode] nvarchar(max) NOT NULL DEFAULT 'LIMS';");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecificationParameters' AND COLUMN_NAME='CreatedBy') ALTER TABLE [LabScopeSpecificationParameters] ADD [CreatedBy] bigint NOT NULL DEFAULT 0;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecificationParameters' AND COLUMN_NAME='CreatedOn') ALTER TABLE [LabScopeSpecificationParameters] ADD [CreatedOn] datetime2 NOT NULL DEFAULT '0001-01-01';");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecificationParameters' AND COLUMN_NAME='IsActive') ALTER TABLE [LabScopeSpecificationParameters] ADD [IsActive] bit NOT NULL DEFAULT 1;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecificationParameters' AND COLUMN_NAME='ModifiedBy') ALTER TABLE [LabScopeSpecificationParameters] ADD [ModifiedBy] bigint NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeSpecificationParameters' AND COLUMN_NAME='ModifiedOn') ALTER TABLE [LabScopeSpecificationParameters] ADD [ModifiedOn] datetime2 NULL;");

            // LabScopeMasters columns (conditional)
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeMasters' AND COLUMN_NAME='NextReviewDate') ALTER TABLE [LabScopeMasters] ADD [NextReviewDate] datetime2 NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeMasters' AND COLUMN_NAME='ScopeRemarks') ALTER TABLE [LabScopeMasters] ADD [ScopeRemarks] nvarchar(500) NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeMasters' AND COLUMN_NAME='ValidFrom') ALTER TABLE [LabScopeMasters] ADD [ValidFrom] datetime2 NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LabScopeMasters' AND COLUMN_NAME='ValidUntil') ALTER TABLE [LabScopeMasters] ADD [ValidUntil] datetime2 NULL;");

            // LabScopeChangeLogs table (conditional)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LabScopeChangeLogs')
                CREATE TABLE [LabScopeChangeLogs] (
                    [ID] bigint IDENTITY(1,1) NOT NULL,
                    [LabScopeID] bigint NOT NULL,
                    [ChangeType] nvarchar(50) NOT NULL,
                    [EntityName] nvarchar(200) NULL,
                    [OldValue] nvarchar(500) NULL,
                    [NewValue] nvarchar(500) NULL,
                    [Remarks] nvarchar(500) NULL,
                    [ChangedBy] bigint NOT NULL,
                    [ChangedOn] datetime2 NOT NULL,
                    CONSTRAINT [PK_LabScopeChangeLogs] PRIMARY KEY ([ID])
                );
            ");
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
