using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_UniversalPlanningBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UniversalTestGroups') AND name = 'DepartmentID')
    ALTER TABLE [UniversalTestGroups] ADD [DepartmentID] bigint NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UniversalTestGroups') AND name = 'ExecutionLayoutID')
    ALTER TABLE [UniversalTestGroups] ADD [ExecutionLayoutID] bigint NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UniversalTestGroups') AND name = 'PlannedConfigurationJson')
    ALTER TABLE [UniversalTestGroups] ADD [PlannedConfigurationJson] nvarchar(max) NULL;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('ExecutionLayoutSections') AND name = 'SectionType')
BEGIN
    DECLARE @cName1 sysname;
    SELECT @cName1 = d.name FROM sys.default_constraints d INNER JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id WHERE d.parent_object_id = OBJECT_ID('ExecutionLayoutSections') AND c.name = 'SectionType';
    IF @cName1 IS NOT NULL EXEC('ALTER TABLE [ExecutionLayoutSections] DROP CONSTRAINT [' + @cName1 + '];');
    ALTER TABLE [ExecutionLayoutSections] ALTER COLUMN [SectionType] nvarchar(100) NOT NULL;
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('ExecutionLayoutSections') AND name = 'SectionCode')
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ExecutionLayoutSections_Layout_SectionCode' AND object_id = OBJECT_ID('ExecutionLayoutSections'))
        DROP INDEX [IX_ExecutionLayoutSections_Layout_SectionCode] ON [ExecutionLayoutSections];
    DECLARE @cName2 sysname;
    SELECT @cName2 = d.name FROM sys.default_constraints d INNER JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id WHERE d.parent_object_id = OBJECT_ID('ExecutionLayoutSections') AND c.name = 'SectionCode';
    IF @cName2 IS NOT NULL EXEC('ALTER TABLE [ExecutionLayoutSections] DROP CONSTRAINT [' + @cName2 + '];');
    ALTER TABLE [ExecutionLayoutSections] ALTER COLUMN [SectionCode] nvarchar(100) NOT NULL;
    CREATE UNIQUE INDEX [IX_ExecutionLayoutSections_Layout_SectionCode] ON [ExecutionLayoutSections] ([ExecutionLayoutID], [SectionCode]);
END

IF OBJECT_ID(N'[dbo].[LaboratoryTestLayouts]', N'U') IS NULL
BEGIN
    CREATE TABLE [LaboratoryTestLayouts] (
        [ID] bigint NOT NULL IDENTITY,
        [LaboratoryTestID] bigint NOT NULL,
        [ExecutionLayoutID] bigint NOT NULL,
        [TestMethodSpecificationID] bigint NULL,
        [TestMethodSpecificationVersionID] bigint NULL,
        [Priority] int NOT NULL,
        [IsDefault] bit NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NULL,
        [ModifiedOn] datetime2 NULL,
        [CompanyCode] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_LaboratoryTestLayouts] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_LaboratoryTestLayouts_ExecutionLayoutMasters_ExecutionLayoutID] FOREIGN KEY ([ExecutionLayoutID]) REFERENCES [ExecutionLayoutMasters] ([ID]),
        CONSTRAINT [FK_LaboratoryTestLayouts_LaboratoryTests_LaboratoryTestID] FOREIGN KEY ([LaboratoryTestID]) REFERENCES [LaboratoryTests] ([ID]),
        CONSTRAINT [FK_LaboratoryTestLayouts_TestMethodSpecificationVersions_TestMethodSpecificationVersionID] FOREIGN KEY ([TestMethodSpecificationVersionID]) REFERENCES [TestMethodSpecificationVersions] ([ID]),
        CONSTRAINT [FK_LaboratoryTestLayouts_TestMethodSpecifications_TestMethodSpecificationID] FOREIGN KEY ([TestMethodSpecificationID]) REFERENCES [TestMethodSpecifications] ([ID])
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UniversalTestGroups_DepartmentID' AND object_id = OBJECT_ID('UniversalTestGroups'))
    CREATE INDEX [IX_UniversalTestGroups_DepartmentID] ON [UniversalTestGroups] ([DepartmentID]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UniversalTestGroups_ExecutionLayoutID' AND object_id = OBJECT_ID('UniversalTestGroups'))
    CREATE INDEX [IX_UniversalTestGroups_ExecutionLayoutID] ON [UniversalTestGroups] ([ExecutionLayoutID]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LaboratoryTestLayouts_ExecutionLayoutID' AND object_id = OBJECT_ID('LaboratoryTestLayouts'))
    CREATE INDEX [IX_LaboratoryTestLayouts_ExecutionLayoutID] ON [LaboratoryTestLayouts] ([ExecutionLayoutID]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LaboratoryTestLayouts_Test_Layout_Method_Version' AND object_id = OBJECT_ID('LaboratoryTestLayouts'))
    CREATE INDEX [IX_LaboratoryTestLayouts_Test_Layout_Method_Version] ON [LaboratoryTestLayouts] ([LaboratoryTestID], [ExecutionLayoutID], [TestMethodSpecificationID], [TestMethodSpecificationVersionID]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LaboratoryTestLayouts_TestMethodSpecificationID' AND object_id = OBJECT_ID('LaboratoryTestLayouts'))
    CREATE INDEX [IX_LaboratoryTestLayouts_TestMethodSpecificationID] ON [LaboratoryTestLayouts] ([TestMethodSpecificationID]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LaboratoryTestLayouts_TestMethodSpecificationVersionID' AND object_id = OBJECT_ID('LaboratoryTestLayouts'))
    CREATE INDEX [IX_LaboratoryTestLayouts_TestMethodSpecificationVersionID] ON [LaboratoryTestLayouts] ([TestMethodSpecificationVersionID]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_UniversalTestGroups_DepartmentMasters_DepartmentID')
    ALTER TABLE [UniversalTestGroups] ADD CONSTRAINT [FK_UniversalTestGroups_DepartmentMasters_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [DepartmentMasters] ([ID]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_UniversalTestGroups_ExecutionLayoutMasters_ExecutionLayoutID')
    ALTER TABLE [UniversalTestGroups] ADD CONSTRAINT [FK_UniversalTestGroups_ExecutionLayoutMasters_ExecutionLayoutID] FOREIGN KEY ([ExecutionLayoutID]) REFERENCES [ExecutionLayoutMasters] ([ID]);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UniversalTestGroups_DepartmentMasters_DepartmentID",
                table: "UniversalTestGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversalTestGroups_ExecutionLayoutMasters_ExecutionLayoutID",
                table: "UniversalTestGroups");

            migrationBuilder.DropTable(
                name: "LaboratoryTestLayouts");

            migrationBuilder.DropIndex(
                name: "IX_UniversalTestGroups_DepartmentID",
                table: "UniversalTestGroups");

            migrationBuilder.DropIndex(
                name: "IX_UniversalTestGroups_ExecutionLayoutID",
                table: "UniversalTestGroups");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "UniversalTestGroups");

            migrationBuilder.DropColumn(
                name: "ExecutionLayoutID",
                table: "UniversalTestGroups");

            migrationBuilder.DropColumn(
                name: "PlannedConfigurationJson",
                table: "UniversalTestGroups");

            migrationBuilder.AlterColumn<string>(
                name: "SectionType",
                table: "ExecutionLayoutSections",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SectionCode",
                table: "ExecutionLayoutSections",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
