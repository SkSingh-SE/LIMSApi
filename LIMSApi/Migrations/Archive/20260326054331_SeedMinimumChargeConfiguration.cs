using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedMinimumChargeConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed minimum charge configuration for pricing
            // MINIMUM_CHARGE_TEST: Applied when case has test charges
            // MINIMUM_CHARGE_PREP: Applied when case is prep-only (cutting/machining, no tests)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = 'MINIMUM_CHARGE_TEST' AND GroupName = 'BILLING')
                BEGIN
                    INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES ('MINIMUM_CHARGE_TEST', 'BILLING', '2500', 'decimal', 'Minimum charge for cases with test charges', 0, GETUTCDATE(), 'LIMS', 1)
                END

                IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = 'MINIMUM_CHARGE_PREP' AND GroupName = 'BILLING')
                BEGIN
                    INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES ('MINIMUM_CHARGE_PREP', 'BILLING', '1000', 'decimal', 'Minimum charge for prep-only cases (cutting/machining, no tests)', 0, GETUTCDATE(), 'LIMS', 1)
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM Configurations WHERE KeyName IN ('MINIMUM_CHARGE_TEST', 'MINIMUM_CHARGE_PREP') AND GroupName = 'BILLING'
            ");
        }
    }
}
