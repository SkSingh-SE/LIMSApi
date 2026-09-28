using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitConversionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Columns may already exist from previous manual SQL or migrations
            // Use raw SQL with IF NOT EXISTS to be safe
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TestResultParameters') AND name = 'DecimalPrecision')
                    ALTER TABLE [TestResultParameters] ADD [DecimalPrecision] int NOT NULL DEFAULT 2;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TestResultParameters') AND name = 'ConversionFactor')
                    ALTER TABLE [TestResultParameters] ADD [ConversionFactor] decimal(18,6) NOT NULL DEFAULT 1;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TestResultParameters') AND name = 'ConvertedValue')
                    ALTER TABLE [TestResultParameters] ADD [ConvertedValue] decimal(18,6) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TestResultParameters') AND name = 'SelectedUnit')
                    ALTER TABLE [TestResultParameters] ADD [SelectedUnit] nvarchar(50) NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DecimalPrecision", table: "TestResultParameters");
            migrationBuilder.DropColumn(name: "ConversionFactor", table: "TestResultParameters");
            migrationBuilder.DropColumn(name: "ConvertedValue", table: "TestResultParameters");
            migrationBuilder.DropColumn(name: "SelectedUnit", table: "TestResultParameters");
        }
    }
}
