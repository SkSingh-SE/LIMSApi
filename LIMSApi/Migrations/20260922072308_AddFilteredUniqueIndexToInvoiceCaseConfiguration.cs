using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredUniqueIndexToInvoiceCaseConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_InvoiceCases_LaboratoryTestID_EffectiveFrom' AND object_id = OBJECT_ID('InvoiceCases'))
BEGIN
    DROP INDEX [IX_InvoiceCases_LaboratoryTestID_EffectiveFrom] ON [InvoiceCases];
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_InvoiceCases_LaboratoryTestID_AnalysisTypeID_EffectiveFrom' AND object_id = OBJECT_ID('InvoiceCases'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_InvoiceCases_LaboratoryTestID_AnalysisTypeID_EffectiveFrom]
    ON [dbo].[InvoiceCases]([LaboratoryTestID] ASC, [AnalysisTypeID] ASC, [EffectiveFrom] ASC)
    WHERE [IsActive] = 1;
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_InvoiceCaseConfigurations_Name_SelectionType' AND object_id = OBJECT_ID('InvoiceCaseConfigurations'))
BEGIN
    DROP INDEX [IX_InvoiceCaseConfigurations_Name_SelectionType] ON [InvoiceCaseConfigurations];
END

CREATE UNIQUE NONCLUSTERED INDEX [IX_InvoiceCaseConfigurations_Name_SelectionType]
ON [dbo].[InvoiceCaseConfigurations]([Name] ASC, [SelectionType] ASC)
WHERE [IsActive] = 1;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestResultHeaders' AND COLUMN_NAME = 'Remarks')
BEGIN
    ALTER TABLE [TestResultHeaders] ADD [Remarks] nvarchar(1000) NULL;
END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestResultHeaders' AND COLUMN_NAME = 'TestPurpose')
BEGIN
    ALTER TABLE [TestResultHeaders] ADD [TestPurpose] nvarchar(100) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_InvoiceCaseConfigurations_Name_SelectionType' AND object_id = OBJECT_ID('InvoiceCaseConfigurations'))
BEGIN
    DROP INDEX [IX_InvoiceCaseConfigurations_Name_SelectionType] ON [InvoiceCaseConfigurations];
END

CREATE UNIQUE NONCLUSTERED INDEX [IX_InvoiceCaseConfigurations_Name_SelectionType]
ON [dbo].[InvoiceCaseConfigurations]([Name] ASC, [SelectionType] ASC);
");
        }
    }
}
