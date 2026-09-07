using Microsoft.EntityFrameworkCore.Migrations;

namespace LIMSApi.Migrations
{
    /// <summary>
    /// Screen 14 — Part A: Enforce parity invariant between
    /// UniversalTestGroup.SpecificationHeaderID and UniversalTestGroup.SpecificationVersionID:
    ///
    ///   Standardless test: Header = NULL AND Version = NULL
    ///   Standard-driven:   Header != NULL AND Version != NULL
    ///
    /// The following combinations are NOT allowed and will be rejected by SQL Server:
    ///   Header = NULL + Version != NULL
    ///   Header != NULL + Version = NULL
    ///
    /// The migration first audits existing rows. If any row violates the invariant,
    /// the CHECK constraint is NOT added. Per Screen 14 Part A, the migration
    /// refuses to silently delete or modify production/demo rows.
    ///
    /// To apply manually:
    ///   dotnet ef migrations add 20260907120000_Screen14_SpecVersion_ParityCheck
    ///   dotnet ef database update
    /// </summary>
    public partial class Screen14_SpecVersion_ParityCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Audit existing data — surface violations to the migration log
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM UniversalTestGroups
                    WHERE IsActive = 1
                      AND (
                          (SpecificationHeaderID IS NULL     AND SpecificationVersionID IS NOT NULL)
                       OR (SpecificationHeaderID IS NOT NULL AND SpecificationVersionID IS NULL)
                      )
                )
                BEGIN
                    DECLARE @violationCount INT = (
                        SELECT COUNT(*) FROM UniversalTestGroups
                        WHERE IsActive = 1
                          AND (
                              (SpecificationHeaderID IS NULL     AND SpecificationVersionID IS NOT NULL)
                           OR (SpecificationHeaderID IS NOT NULL AND SpecificationVersionID IS NULL)
                          )
                    );
                    PRINT 'Screen14 ParityCheck: ' + CAST(@violationCount AS VARCHAR(20)) +
                          ' rows violate the (Header/Version) parity invariant. ' +
                          'The CHECK constraint will NOT be added. Please remediate before re-running this migration.';
                    -- Refuse to add the constraint.
                    RETURN;
                END
            ");

            // 2) Add the CHECK constraint only if no violations exist.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.check_constraints
                    WHERE name = 'CK_UniversalTestGroups_SpecVersion_Parity'
                )
                BEGIN
                    ALTER TABLE UniversalTestGroups
                    ADD CONSTRAINT CK_UniversalTestGroups_SpecVersion_Parity
                    CHECK (
                        (SpecificationHeaderID IS NULL     AND SpecificationVersionID IS NULL)
                     OR (SpecificationHeaderID IS NOT NULL AND SpecificationVersionID IS NOT NULL)
                    );
                    PRINT 'Screen14 ParityCheck: constraint CK_UniversalTestGroups_SpecVersion_Parity added.';
                END
                ELSE
                BEGIN
                    PRINT 'Screen14 ParityCheck: constraint already exists. No action taken.';
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.check_constraints
                    WHERE name = 'CK_UniversalTestGroups_SpecVersion_Parity'
                )
                BEGIN
                    ALTER TABLE UniversalTestGroups DROP CONSTRAINT CK_UniversalTestGroups_SpecVersion_Parity;
                END
            ");
        }
    }
}