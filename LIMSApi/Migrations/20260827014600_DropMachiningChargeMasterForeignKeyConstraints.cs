using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class DropMachiningChargeMasterForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID')
                BEGIN
                    ALTER TABLE [dbo].[MachiningChargeMasters] DROP CONSTRAINT [FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID];
                END

                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID')
                BEGIN
                    ALTER TABLE [dbo].[MachiningChargeMasters] DROP CONSTRAINT [FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID];
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
