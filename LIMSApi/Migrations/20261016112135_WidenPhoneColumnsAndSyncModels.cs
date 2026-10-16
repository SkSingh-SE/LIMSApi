using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class WidenPhoneColumnsAndSyncModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- 1. TestLayoutTemplateMappings Columns (safe check) ---
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[TestLayoutTemplateMappings]') AND name = N'LaboratoryTestAnalysisTypeID')
BEGIN
    ALTER TABLE [TestLayoutTemplateMappings] ADD [LaboratoryTestAnalysisTypeID] bigint NULL;
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[TestLayoutTemplateMappings]') AND name = N'LeafNode')
BEGIN
    ALTER TABLE [TestLayoutTemplateMappings] ADD [LeafNode] nvarchar(250) NULL;
END");

            // --- 2. Widen SubContractorMasters Phone Columns (15 -> 25) ---
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNo",
                table: "SubContractorMasters",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "SubContractorMasters",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            // --- 3. LabScopeMasters Columns (safe check) ---
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[LabScopeMasters]') AND name = N'AnalysisTypeID')
BEGIN
    ALTER TABLE [LabScopeMasters] ADD [AnalysisTypeID] bigint NULL;
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[LabScopeMasters]') AND name = N'LaboratoryTestSubGroupID')
BEGIN
    ALTER TABLE [LabScopeMasters] ADD [LaboratoryTestSubGroupID] bigint NULL;
END");

            // --- 4. EquipmentMasters SupplierID (safe check) ---
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[EquipmentMasters]') AND name = N'SupplierID')
BEGIN
    ALTER TABLE [EquipmentMasters] ADD [SupplierID] bigint NULL;
END");

            // --- 5. Widen EmployeeMasters Phone Columns (15 -> 25) ---
            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "EmployeeMasters",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyMobileNo",
                table: "EmployeeMasters",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            // --- 6. Widen ContactPersons Phone Columns (15 -> 25) ---
            migrationBuilder.AlterColumn<string>(
                name: "TelephoneNo",
                table: "ContactPersons",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "ContactPersons",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            // --- 7. Indexes (safe check) ---
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TestLayoutTemplateMappings_LaboratoryTestAnalysisTypeID' AND object_id = OBJECT_ID(N'[TestLayoutTemplateMappings]'))
BEGIN
    CREATE INDEX [IX_TestLayoutTemplateMappings_LaboratoryTestAnalysisTypeID] ON [TestLayoutTemplateMappings] ([LaboratoryTestAnalysisTypeID]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabScopeMasters_AnalysisTypeID' AND object_id = OBJECT_ID(N'[LabScopeMasters]'))
BEGIN
    CREATE INDEX [IX_LabScopeMasters_AnalysisTypeID] ON [LabScopeMasters] ([AnalysisTypeID]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabScopeMasters_LaboratoryTestSubGroupID' AND object_id = OBJECT_ID(N'[LabScopeMasters]'))
BEGIN
    CREATE INDEX [IX_LabScopeMasters_LaboratoryTestSubGroupID] ON [LabScopeMasters] ([LaboratoryTestSubGroupID]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EquipmentMasters_SupplierID' AND object_id = OBJECT_ID(N'[EquipmentMasters]'))
BEGIN
    CREATE INDEX [IX_EquipmentMasters_SupplierID] ON [EquipmentMasters] ([SupplierID]);
END");

            // --- 8. Foreign Keys (safe check) ---
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_EquipmentMasters_SupplierMasters_SupplierID')
BEGIN
    ALTER TABLE [EquipmentMasters] ADD CONSTRAINT [FK_EquipmentMasters_SupplierMasters_SupplierID] 
    FOREIGN KEY ([SupplierID]) REFERENCES [SupplierMasters] ([ID]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_LabScopeMasters_LaboratoryTestAnalysisTypes_AnalysisTypeID')
BEGIN
    ALTER TABLE [LabScopeMasters] ADD CONSTRAINT [FK_LabScopeMasters_LaboratoryTestAnalysisTypes_AnalysisTypeID] 
    FOREIGN KEY ([AnalysisTypeID]) REFERENCES [LaboratoryTestAnalysisTypes] ([ID]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_LabScopeMasters_LaboratoryTestSubGroups_LaboratoryTestSubGroupID')
BEGIN
    ALTER TABLE [LabScopeMasters] ADD CONSTRAINT [FK_LabScopeMasters_LaboratoryTestSubGroups_LaboratoryTestSubGroupID] 
    FOREIGN KEY ([LaboratoryTestSubGroupID]) REFERENCES [LaboratoryTestSubGroups] ([ID]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TestLayoutTemplateMappings_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID')
BEGIN
    ALTER TABLE [TestLayoutTemplateMappings] ADD CONSTRAINT [FK_TestLayoutTemplateMappings_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID] 
    FOREIGN KEY ([LaboratoryTestAnalysisTypeID]) REFERENCES [LaboratoryTestAnalysisTypes] ([ID]);
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop Foreign Keys if exist
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_EquipmentMasters_SupplierMasters_SupplierID') ALTER TABLE [EquipmentMasters] DROP CONSTRAINT [FK_EquipmentMasters_SupplierMasters_SupplierID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_LabScopeMasters_LaboratoryTestAnalysisTypes_AnalysisTypeID') ALTER TABLE [LabScopeMasters] DROP CONSTRAINT [FK_LabScopeMasters_LaboratoryTestAnalysisTypes_AnalysisTypeID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_LabScopeMasters_LaboratoryTestSubGroups_LaboratoryTestSubGroupID') ALTER TABLE [LabScopeMasters] DROP CONSTRAINT [FK_LabScopeMasters_LaboratoryTestSubGroups_LaboratoryTestSubGroupID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TestLayoutTemplateMappings_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID') ALTER TABLE [TestLayoutTemplateMappings] DROP CONSTRAINT [FK_TestLayoutTemplateMappings_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID];");

            // Drop Indexes if exist
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TestLayoutTemplateMappings_LaboratoryTestAnalysisTypeID' AND object_id = OBJECT_ID(N'[TestLayoutTemplateMappings]')) DROP INDEX [IX_TestLayoutTemplateMappings_LaboratoryTestAnalysisTypeID] ON [TestLayoutTemplateMappings];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabScopeMasters_AnalysisTypeID' AND object_id = OBJECT_ID(N'[LabScopeMasters]')) DROP INDEX [IX_LabScopeMasters_AnalysisTypeID] ON [LabScopeMasters];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabScopeMasters_LaboratoryTestSubGroupID' AND object_id = OBJECT_ID(N'[LabScopeMasters]')) DROP INDEX [IX_LabScopeMasters_LaboratoryTestSubGroupID] ON [LabScopeMasters];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EquipmentMasters_SupplierID' AND object_id = OBJECT_ID(N'[EquipmentMasters]')) DROP INDEX [IX_EquipmentMasters_SupplierID] ON [EquipmentMasters];");

            // Drop Columns if exist
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[TestLayoutTemplateMappings]') AND name = N'LaboratoryTestAnalysisTypeID') ALTER TABLE [TestLayoutTemplateMappings] DROP COLUMN [LaboratoryTestAnalysisTypeID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[TestLayoutTemplateMappings]') AND name = N'LeafNode') ALTER TABLE [TestLayoutTemplateMappings] DROP COLUMN [LeafNode];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[LabScopeMasters]') AND name = N'AnalysisTypeID') ALTER TABLE [LabScopeMasters] DROP COLUMN [AnalysisTypeID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[LabScopeMasters]') AND name = N'LaboratoryTestSubGroupID') ALTER TABLE [LabScopeMasters] DROP COLUMN [LaboratoryTestSubGroupID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[EquipmentMasters]') AND name = N'SupplierID') ALTER TABLE [EquipmentMasters] DROP COLUMN [SupplierID];");

            // Revert Phone Columns to 15
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNo",
                table: "SubContractorMasters",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "SubContractorMasters",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "EmployeeMasters",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyMobileNo",
                table: "EmployeeMasters",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TelephoneNo",
                table: "ContactPersons",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "ContactPersons",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);
        }
    }
}
