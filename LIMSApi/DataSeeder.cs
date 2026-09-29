using LIMSApi.Data;
using LIMSApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi;

/// <summary>
/// Seeds essential data on first deployment:
///   Roles, Menus, Permissions, RoleMenuMappings, Configurations, Admin User.
/// Idempotent — safe to run on every startup (IF NOT EXISTS checks).
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LIMSContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 1. Repair empty Hangfire schema table if Hangfire objects already exist from backup/restore
            await RepairHangfireSchemaAsync(db, logger);

            // 2. Ensure all incremental schema tables, columns, indexes and constraints exist (idempotent DDLs)
            await EnsureDatabaseSchemaAsync(db, logger);

            // 3. Align tenant codes across all tables (LIMS01 / NULL -> LIMS)
            await AlignTenantCodesAsync(db, logger);

            // 4. Ensure Organization and Branches exist and are linked
            await SeedOrganizationAndBranchesAsync(db);

            // 5. Ensure Admin & SuperAdmin users exist and are synchronized
            await SeedAdminUserAsync(db, logger);

            // 6. Ensure Roles exist
            await SeedRolesAsync(db);

            // 7. Core configurations, currencies, dispatch modes
            await SeedConfigurationsAsync(db);
            await SeedCurrenciesAsync(db);
            await SeedDispatchModesAsync(db);

            // 8. Menus, RoleMenuMappings, Permissions, RolePermissionDefaults
            await SeedMenusAsync(db);
            await SeedPhase1MenuFixupsAsync(db);
            await SeedRoleMenuMappingsAsync(db);
            await SeedPermissionsAsync(db);
            await SeedRolePermissionDefaultsAsync(db, logger);

            // 9. Master Data, Acceptance Criteria, Execution Layouts, Versions
            await SeedMasterDataAsync(db);
            await SeedAcceptanceCriteriaAsync(db);
            await SeedExecutionLayoutsAsync(db);
            await SeedSpecificationAndMethodVersionsAsync(db);
            await SeedPriceDimensionTypesAsync(db);
            await SeedFinancialYearsAsync(db);
            await BackfillFinancialYearIdsAsync(db, logger);
            await FixMachiningChargeMasterConstraintsAsync(db, logger);

            // Versioned Seeding Guard: Stamp CurrentSeedVersion in Configurations
            const string CurrentSeedVersion = "2026.09.21.02";
            var dbSeedVersion = await db.Database
                .SqlQueryRaw<string>("SELECT TOP 1 [Value] AS [Value] FROM Configurations WHERE KeyName = N'SEED_VERSION' AND CompanyCode = N'LIMS'")
                .FirstOrDefaultAsync();

            if (!string.Equals(dbSeedVersion, CurrentSeedVersion, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("DataSeeder: new seed version detected ({Current} vs DB {DbVersion}). Updating seed version...", CurrentSeedVersion, dbSeedVersion ?? "NONE");

                await db.Database.ExecuteSqlRawAsync($@"
                    IF EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'SEED_VERSION' AND CompanyCode = N'LIMS')
                        UPDATE Configurations SET [Value] = N'{CurrentSeedVersion}', ModifiedOn = GETUTCDATE() WHERE KeyName = N'SEED_VERSION' AND CompanyCode = N'LIMS';
                    ELSE
                        INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                        VALUES (N'SEED_VERSION', N'SYSTEM', N'{CurrentSeedVersion}', N'string', N'Data seeder schema and sync version', 0, GETUTCDATE(), N'LIMS', 1);
                ");
            }

            logger.LogInformation("DataSeeder: complete initialization & synchronization successful (Version: {Version}).", CurrentSeedVersion);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DataSeeder: error during seeding");
        }
    }

    // ───────────────────────────────────────────────
    // 1. ROLES
    // ───────────────────────────────────────────────
    private static async Task SeedRolesAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"


            IF NOT EXISTS (SELECT 1 FROM RoleMasters WHERE Name = N'Admin')
                INSERT INTO RoleMasters (Name, Description, IsAdmin, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Admin', N'System Administrator with full access', 1, 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM RoleMasters WHERE Name = N'Accounts')
                INSERT INTO RoleMasters (Name, Description, IsAdmin, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Accounts', N'Accounts and billing management', 0, 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM RoleMasters WHERE Name = N'FrontDesk')
                INSERT INTO RoleMasters (Name, Description, IsAdmin, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'FrontDesk', N'Sample inward and customer handling', 0, 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM RoleMasters WHERE Name = N'Technical')
                INSERT INTO RoleMasters (Name, Description, IsAdmin, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Technical', N'Test planning and review', 0, 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM RoleMasters WHERE Name = N'Lab')
                INSERT INTO RoleMasters (Name, Description, IsAdmin, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Lab', N'Laboratory testing operations', 0, 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM RoleMasters WHERE Name = N'LabManager')
                INSERT INTO RoleMasters (Name, Description, IsAdmin, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'LabManager', N'Lab management and approvals', 0, 0, GETUTCDATE(), N'LIMS', 1);
        ");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 2. MENUS  — stored procedure usp_SeedMenus
    //
    //   Approach: #Menus temp table holds (Title, Icon, Route, Color, ParentTitle).
    //   ParentID resolved at runtime by joining MenuMasters on Title + Route IS NULL
    //   (folder menus never have a Route, which disambiguates same-named folders
    //   from same-named page items, e.g. 'Equipment' folder vs 'Equipment' page).
    //   Three passes after root insert handle up to 4 hierarchy levels.
    //   Duplicate guard: NOT EXISTS WHERE Title = X AND ParentID = Y.
    //   No hardcoded IDs, no IDENTITY_INSERT.
    // ─────────────────────────────────────────────────────────────────────────
    private static async Task SeedMenusAsync(LIMSContext db)
    {
        // Step 1: Create / update the stored procedure
        await db.Database.ExecuteSqlRawAsync(@"
        CREATE OR ALTER PROCEDURE usp_SeedMenus
        AS
        BEGIN
            SET NOCOUNT ON;

            IF OBJECT_ID('tempdb..#Menus') IS NOT NULL DROP TABLE #Menus;
            CREATE TABLE #Menus (
                Title       NVARCHAR(200) NOT NULL,
                Icon        NVARCHAR(100) NULL,
                Route       NVARCHAR(300) NULL,
                Color       NVARCHAR(50)  NULL,
                ParentTitle NVARCHAR(200) NULL   -- NULL = top-level root
            );

            INSERT INTO #Menus (Title, Icon, Route, Color, ParentTitle) VALUES
            -- ══ Level 0: Roots ══
            (N'Administration',   N'bi-people',            NULL, NULL, NULL),
            (N'Specification',    N'bi-box',                NULL, NULL, NULL),
            (N'Test',             N'bi-file-earmark',       NULL, NULL, NULL),
            (N'Customer',         N'bi-people',             NULL, NULL, NULL),
            (N'Sample',           N'bi-layout-text-sidebar',NULL, NULL, NULL),
            (N'Invoice',          N'bi-receipt-cutoff',     NULL, NULL, NULL),
            (N'NABL ISO 17025',   N'bi-shield-check',       NULL, NULL, NULL),
            (N'User Management',  N'bi-person-fill-gear',   NULL, NULL, NULL),
            (N'Testing',          N'bi-dropbox',            NULL, NULL, NULL),
            (N'Configuration',    N'bi-gear',               NULL, NULL, NULL),
            (N'Reporting',        N'bi-file-text',          NULL, NULL, NULL),
            (N'Accounts',         N'bi-wallet2',            NULL, NULL, NULL),
            -- ══ Level 1: Administration ══
            (N'Department Master',   NULL, N'/department',         NULL, N'Administration'),
            (N'Employee Master',     NULL, N'/employee',           NULL, N'Administration'),
            (N'Designation Master',  NULL, N'/designation',        NULL, N'Administration'),
            (N'Tax Master',          NULL, N'/tax',                NULL, N'Administration'),
            (N'Bank Master',         NULL, N'/bank',               NULL, N'Administration'),
            (N'Courier Master',      NULL, N'/courier',            NULL, N'Administration'),
            (N'Product Size Master', NULL, N'/product-size-master',NULL, N'Administration'),
            (N'Analysis Technique Master', NULL, N'/analysis-technique', NULL, N'Test'),
            (N'TPI Master',          NULL, N'/tpi',                NULL, N'Administration'),
            (N'Supplier Master',     NULL, N'/supplier',           NULL, N'Administration'),
            (N'Equipment',           NULL, N'/equipment',          NULL, N'Administration'),
            (N'OEM Master',          NULL, N'/oem',                NULL, N'Administration'),
            (N'Calibration Agency',  NULL, N'/calibration-agency', NULL, N'Administration'),
            -- ══ Level 1: Specification — folders only ══
            (N'Material Specification', NULL, NULL, NULL, N'Specification'),
            (N'Product Specification',  NULL, NULL, NULL, N'Specification'),
            (N'Specification Master',   NULL, N'/specification',              NULL, N'Specification'),
            (N'Specification Version Master', NULL, N'/specification-version', NULL, N'Specification'),
            (N'Product / Material Master',    NULL, N'/product-master',        NULL, N'Specification'),
            (N'Classification Master',        NULL, N'/classification-master', NULL, N'Specification'),
            (N'Acceptance Criteria Master',   NULL, N'/acceptance-criteria',   NULL, N'Specification'),
            (N'Specification Requirement Configuration', NULL, N'/specification-requirement', NULL, N'Specification'),
            -- ══ Level 1: Test ══
            (N'Laboratory Test Master',    NULL, N'/test',              NULL, N'Test'),
            (N'Test Method Specification', NULL, N'/test-specification', NULL, N'Test'),
            (N'Test Method Master',        NULL, N'/test-method',        NULL, N'Test'),
            (N'Universal Test Group & Effective Config', NULL, N'/sample/test-groups', NULL, N'Test'),
            (N'Invoice Case',              NULL, N'/invoice-case',       NULL, N'Test'),
            -- ══ Level 1: Customer ══
            (N'Company Category', NULL, N'/company-category', NULL, N'Customer'),
            (N'Customer Master',  NULL, N'/customer',          NULL, N'Customer'),
            -- ══ Level 1: Sample ══
            (N'Inward',             NULL, N'/sample/inward',  NULL, N'Sample'),
            (N'Plan',               NULL, N'/sample/plan',    NULL, N'Sample'),
            (N'Review',             NULL, N'/sample/review',  NULL, N'Sample'),
            (N'Sample Preparation', NULL, NULL,               NULL, N'Sample'),
            -- ══ Level 1: Invoice ══
            (N'Invoice Case Config', NULL, N'/invoice-case-config', NULL, N'Invoice'),
            (N'Invoice Case',        NULL, N'/invoice-case',        NULL, N'Invoice'),
            -- ══ Level 1: NABL ISO 17025 ══
            (N'General Requirements',    NULL, NULL,      NULL, N'NABL ISO 17025'),
            (N'Structural Requirements', NULL, NULL,      NULL, N'NABL ISO 17025'),
            (N'Resource Requirements',   NULL, NULL,      NULL, N'NABL ISO 17025'),
            (N'Process Requirements',    NULL, NULL,      NULL, N'NABL ISO 17025'),
            (N'Management System',       NULL, NULL,      NULL, N'NABL ISO 17025'),
            (N'Lab Scope Master',        NULL, N'/scope', NULL, N'NABL ISO 17025'),
            -- ══ Level 1: User Management ══
            (N'Lab Employee Master', NULL, N'/nabl/lab-employee', NULL, N'User Management'),
            (N'Lab Score Master',    NULL, N'/nabl/lab-score',    NULL, N'User Management'),
            -- ══ Level 1: Testing ══
            (N'Testing Dashboard',  NULL, N'/testing/dashboard',   NULL, N'Testing'),
            (N'Perform Test',       NULL, N'/testing/perform/:id', NULL, N'Testing'),
            (N'Long Term Tracking', NULL, N'/testing/longterm',    NULL, N'Testing'),
            (N'Test Results',       NULL, N'/testing/results/:id', NULL, N'Testing'),
            -- ══ Level 1: Configuration ══
            (N'Configuration Manager', NULL, N'/config',          NULL, N'Configuration'),
            (N'Menu Management',       NULL, N'/menu',            NULL, N'Configuration'),
            (N'Menu Permission',       NULL, N'/menu-permission', NULL, N'Configuration'),
            (N'Role Management',       NULL, N'/role',            NULL, N'Configuration'),
            (N'User Permission',       NULL, N'/user-permission', NULL, N'Configuration'),
            (N'Workflow',              NULL, N'/workflow',        NULL, N'Configuration'),
            -- ══ Level 1: Reporting ══
            (N'Reporting Dashboard', NULL, N'/reporting/dashboard', NULL, N'Reporting'),
            (N'Report Formats',      NULL, N'/report-format',       NULL, N'Reporting'),
            -- ══ Level 1: Accounts ══
            (N'Accounts Dashboard',       NULL, N'/accounts/dashboard',         NULL, N'Accounts'),
            (N'Case Accounts',            NULL, N'/accounts/cases',             NULL, N'Accounts'),
            (N'Customer Ledger',          NULL, N'/account/ledger',             NULL, N'Accounts'),
            (N'Record Payment',           NULL, N'/account/record-payment',     NULL, N'Accounts'),
            (N'Aging Report',             NULL, N'/account/aging-report',       NULL, N'Accounts'),
            (N'Outstanding Report',       NULL, N'/account/outstanding-report', NULL, N'Accounts'),
            (N'Customer Purchase Orders', NULL, N'/accounts/purchase-orders',   NULL, N'Accounts'),
            -- ══ Level 2: Under Material Specification folder ══
            (N'Material Specification',         NULL, N'/material-specification',        NULL, N'Material Specification'),
            (N'Custom Material Specification',  NULL, N'/custom-material-specification', NULL, N'Material Specification'),
            (N'Linked Masters',                 NULL, NULL,                              NULL, N'Material Specification'),
            -- ══ Level 2: Under Product Specification folder ══
            (N'Product Specification',          NULL, N'/product-specification',         NULL, N'Product Specification'),
            (N'Custom Product Specification',   NULL, N'/custom-product-specification',  NULL, N'Product Specification'),
            -- ══ Level 2: Under Sample Preparation ══
            (N'Preparation Queue',   NULL, N'/sample/preparation',   NULL, N'Sample Preparation'),
            (N'Sample Cutting',      NULL, N'/sample/cutting',       NULL, N'Sample Preparation'),
            (N'Machining Charges',   NULL, N'/sample/machining',     NULL, N'Sample Preparation'),
            (N'Cutting Price Master',NULL, N'/cutting-price-master', NULL, N'Sample Preparation'),
            -- ══ Level 2: Under General Requirements ══
            (N'F-2: Confidentiality Agree.', NULL, N'/supplier-confidentiality-agreement', NULL, N'General Requirements'),
            (N'F-4: Impartiality Agree.',    NULL, N'/employee/impartiality-agreement',    NULL, N'General Requirements'),
            -- ══ Level 2: Under Structural Requirements ══
            (N'Organization Chart', NULL, N'/org-chart', NULL, N'Structural Requirements'),
            -- ══ Level 2: Under Resource Requirements ══
            (N'Personnel',               NULL, NULL, NULL, N'Resource Requirements'),
            (N'Facilities & Environment',NULL, NULL, NULL, N'Resource Requirements'),
            (N'Equipment',               NULL, NULL, NULL, N'Resource Requirements'),
            (N'External Products',       NULL, NULL, NULL, N'Resource Requirements'),
            -- ══ Level 2: Under Process Requirements ══
            (N'F-27: Test Request',       NULL, N'/nabl/test-request',       NULL, N'Process Requirements'),
            (N'Methods Management',       NULL, NULL,                         NULL, N'Process Requirements'),
            (N'Sample Handling',          NULL, NULL,                         NULL, N'Process Requirements'),
            (N'F-34: Technical Raw Data', NULL, N'/nabl/technical-raw-data', NULL, N'Process Requirements'),
            (N'F-35: Uncertainty Rec.',   NULL, N'/measurement-uncertainty',  NULL, N'Process Requirements'),
            (N'Ensuring Validity',        NULL, NULL,                         NULL, N'Process Requirements'),
            (N'F-39: Test Report',        NULL, N'/test-report',              NULL, N'Process Requirements'),
            (N'F-40: Complaint Reg.',     NULL, N'/complaint-register',       NULL, N'Process Requirements'),
            (N'F-41: NC Work Records',    NULL, N'/non-conforming-work',      NULL, N'Process Requirements'),
            -- ══ Level 2: Under Management System ══
            (N'Documentation Control', NULL, NULL,                NULL, N'Management System'),
            (N'F-46: Risk Assessment',  NULL, N'/risk-assessment', NULL, N'Management System'),
            (N'Improvement & Actions', NULL, NULL,                NULL, N'Management System'),
            (N'Internal Audits',       NULL, NULL,                NULL, N'Management System'),
            (N'Management Review',     NULL, NULL,                NULL, N'Management System'),
            -- ══ Level 3: Under Linked Masters ══
            (N'Standard Organization', NULL, N'/standard-organization',  NULL, N'Linked Masters'),
            (N'Metal Classification',  NULL, N'/metal-classification',   NULL, N'Linked Masters'),
            (N'Chemical Parameter',    NULL, N'/chemical-parameter',     NULL, N'Linked Masters'),
            (N'Mechanical Parameter',  NULL, N'/mechanical-parameter',   NULL, N'Linked Masters'),
            (N'Parameter Unit',        NULL, N'/parameter-unit',         NULL, N'Linked Masters'),
            (N'Heat Treatment',        NULL, N'/heat-treatment',         NULL, N'Linked Masters'),
            (N'Product Condition',     NULL, N'/product-condition',      NULL, N'Linked Masters'),
            (N'Specimen Orientation',  NULL, N'/specimen-orientation',   NULL, N'Linked Masters'),
            (N'Specimen Type',         NULL, N'/specimen-type',          NULL, N'Linked Masters'),
            (N'Product Form',          NULL, N'/product-form',           NULL, N'Linked Masters'),
            (N'Dimensional Factor',    NULL, N'/dimesional-factor',      NULL, N'Linked Masters'),
            (N'Universal Code Type',   NULL, N'/universal-code-type',    NULL, N'Linked Masters'),
            (N'Parameter Master',        NULL, N'/parameter',             NULL, N'Linked Masters'),
            (N'Equipment Type Master',  NULL, N'/equipment-type',        NULL, N'Linked Masters'),
            (N'Measurement Uncertainty Master', NULL, N'/measurement-uncertainty-master', NULL, N'Linked Masters'),
            -- ══ PHASE 1G: Execution Layout (Operational Configuration → resolved by title, R5) ══
            (N'Execution Layout Master', NULL, N'/execution-layout-master', NULL, N'Operational Configuration'),
            -- ══ Level 3: Under Personnel ══
            (N'F-1: Job Description',         NULL, N'/job-description',                       NULL, N'Personnel'),
            (N'F-3: Resp. & Authority',        NULL, N'/responsibility-authority',               NULL, N'Personnel'),
            (N'F-5: Competence Req.',          NULL, N'/competence-requirement',                 NULL, N'Personnel'),
            (N'F-6: Induction Training',       NULL, N'/induction-training',                     NULL, N'Personnel'),
            (N'F-7: Competence Report',        NULL, N'/employee/competence',                    NULL, N'Personnel'),
            (N'F-8: Training Plan',            NULL, N'/training-plan',                          NULL, N'Personnel'),
            (N'F-9: Training Attendance',      NULL, N'/training-attendance',                    NULL, N'Personnel'),
            (N'F-10: Training Effectiv.',      NULL, N'/training-effectiveness',                  NULL, N'Personnel'),
            (N'F-11: Skill Matrix',            NULL, N'/skill-matrix',                           NULL, N'Personnel'),
            (N'F-13: Employee Authorization',  NULL, N'/employee/equipment-authorization/list',   NULL, N'Personnel'),
            -- ══ Level 3: Under Facilities & Environment ══
            (N'F-12: Environment Mon.', NULL, N'/environment-monitoring', NULL, N'Facilities & Environment'),
            -- ══ Level 3: Under Equipment folder (under Resource Requirements) ══
            (N'F-14: Equipment History', NULL, N'/equipment-history-card',           NULL, N'Equipment'),
            (N'F-15: Calibration Review',NULL, N'/calibration-review',               NULL, N'Equipment'),
            (N'F-16: Intermediate Check',NULL, N'/intermediate-check-records',        NULL, N'Equipment'),
            (N'F-17: Ref. Material List',NULL, N'/reference-material',               NULL, N'Equipment'),
            (N'F-18: CRM Consumption',   NULL, N'/reference-material-consumption',   NULL, N'Equipment'),
            -- ══ Level 3: Under External Products ══
            (N'F-19: Supplier Reg.',      NULL, N'/supplier-registration',          NULL, N'External Products'),
            (N'F-20: Approved Suppliers', NULL, N'/approved-supplier',              NULL, N'External Products'),
            (N'F-21: Purchase Indent',    NULL, N'/purchase-indent',                NULL, N'External Products'),
            (N'F-22: Purchase Order',     NULL, N'/purchase-order',                 NULL, N'External Products'),
            (N'F-23: Inspection Plan',    NULL, N'/product-inspection',             NULL, N'External Products'),
            (N'F-24: Incoming Mat. Rec.', NULL, N'/incoming-material',              NULL, N'External Products'),
            (N'F-25: Mat. Verification',  NULL, N'/purchase-material-verification', NULL, N'External Products'),
            (N'F-26: Supplier Eval.',     NULL, N'/supplier-evaluation',            NULL, N'External Products'),
            -- ══ Level 3: Under Methods Management ══
            (N'F-28: Test Methods', NULL, N'/nabl/test-method',         NULL, N'Methods Management'),
            (N'F-29: Verification', NULL, N'/nabl/method-verification', NULL, N'Methods Management'),
            (N'F-30: Validation',   NULL, N'/nabl/method-validation',   NULL, N'Methods Management'),
            -- ══ Level 3: Under Sample Handling ══
            (N'F-31: Inward Register', NULL, N'/nabl/sample-inward-register', NULL, N'Sample Handling'),
            (N'F-32: Muster Register', NULL, N'/nabl/sample-muster-register', NULL, N'Sample Handling'),
            (N'F-33: Sample Label',    NULL, N'/nabl/sample-label',           NULL, N'Sample Handling'),
            -- ══ Level 3: Under Ensuring Validity ══
            (N'F-36: PT / ILC Plan',  NULL, N'/pt-ilc-plan',              NULL, N'Ensuring Validity'),
            (N'F-37: QC Plan',        NULL, N'/quality-control-plan',      NULL, N'Ensuring Validity'),
            (N'F-38: Retesting Rec.', NULL, N'/retesting-retained-sample', NULL, N'Ensuring Validity'),
            -- ══ Level 3: Under Documentation Control ══
            (N'F-43: Master Document', NULL, N'/master-document',         NULL, N'Documentation Control'),
            (N'F-44: Doc. Change Req.',NULL, N'/document-change-request', NULL, N'Documentation Control'),
            (N'F-45: Doc. Review Rec.',NULL, N'/document-review',         NULL, N'Documentation Control'),
            -- ══ Level 3: Under Improvement & Actions ══
            (N'F-47: Cust. Feedback',   NULL, N'/customer-feedback',    NULL, N'Improvement & Actions'),
            (N'F-48: Feedback Analys.', NULL, N'/feedback-analysis',    NULL, N'Improvement & Actions'),
            (N'F-42: NC & Corr. Action',NULL, N'/nc-corrective-action', NULL, N'Improvement & Actions'),
            -- ══ Level 3: Under Internal Audits ══
            (N'F-49: Internal Auditors',NULL, N'/internal-auditor', NULL, N'Internal Audits'),
            (N'F-50: Audit Plan',       NULL, N'/audit-plan',       NULL, N'Internal Audits'),
            (N'F-51: Audit Checklist',  NULL, N'/audit-checklist',  NULL, N'Internal Audits'),
            (N'F-52: Audit Summary',    NULL, N'/audit-summary',    NULL, N'Internal Audits'),
            -- ══ Level 3: Under Management Review ══
            (N'F-53: Meeting Agenda',  NULL, N'/meeting-agenda',  NULL, N'Management Review'),
            (N'F-54: Meeting Minutes', NULL, N'/meeting-minutes', NULL, N'Management Review');

            -- ── Step 1: Root menus (no parent) ──
            INSERT INTO MenuMasters (Title, Icon, IsExpanded, Route, Color, ParentID)
            SELECT m.Title, m.Icon, 0, m.Route, m.Color, NULL
            FROM #Menus m
            WHERE m.ParentTitle IS NULL
              AND NOT EXISTS (
                  SELECT 1 FROM MenuMasters mm
                  WHERE mm.Title = m.Title AND mm.ParentID IS NULL
              );

            -- ── Steps 2-4: Child menus — 3 passes handle up to 3 levels of depth ──
            -- Pass N inserts items whose parent was inserted in the previous pass.
            -- Parent folders are found by Title + Route IS NULL (only folders lack a Route).
            -- The NOT EXISTS guard makes every pass fully idempotent.
            DECLARE @Pass INT = 0;
            WHILE @Pass < 3
            BEGIN
                INSERT INTO MenuMasters (Title, Icon, IsExpanded, Route, Color, ParentID)
                SELECT m.Title, m.Icon, 0, m.Route, m.Color, p.ID
                FROM   #Menus m
                JOIN   MenuMasters p ON p.Title = m.ParentTitle AND p.Route IS NULL
                WHERE  m.ParentTitle IS NOT NULL
                  AND  NOT EXISTS (
                           SELECT 1 FROM MenuMasters mm
                           WHERE mm.Title = m.Title AND mm.ParentID = p.ID
                       );
                SET @Pass = @Pass + 1;
            END;

            DROP TABLE #Menus;
        END
        ");

        // Step 2: Execute the SP to seed / update menus
        await db.Database.ExecuteSqlRawAsync("EXEC usp_SeedMenus");
    }

    // ───────────────────────────────────────────────────────────────────────────
    // 3. PERMISSIONS  — stored procedure usp_SeedPermissions
    //
    //   Approach: temp table holds (Name, DisplayName, MenuTitle, ParentTitle, Type).
    //   A JOIN on MenuMasters.Title resolves MenuID at runtime — no hardcoded IDs.
    //   ParentTitle disambiguates menus that share the same Title
    //     e.g. 'Material Specification' folder (ID 200) vs leaf page (ID 31):
    //          → leaf's parent is also 'Material Specification' → ParentTitle='Material Specification'
    //     e.g. 'Invoice Case' under 'Test' (ID 37) vs under 'Invoice' (ID 48):
    //          → ParentTitle='Test' picks 37; ParentTitle='Invoice' picks 48
    //
    //   Idempotent: NOT EXISTS check prevents duplicates.
    //   Runs every startup so new permissions are picked up without DB reset.
    // ───────────────────────────────────────────────────────────────────────────
    private static async Task SeedPermissionsAsync(LIMSContext db)
    {
        // Step 1: Create / update the stored procedure in the database
        await db.Database.ExecuteSqlRawAsync(@"
        CREATE OR ALTER PROCEDURE usp_SeedPermissions
        AS
        BEGIN
            SET NOCOUNT ON;

            IF OBJECT_ID('tempdb..#Perms') IS NOT NULL DROP TABLE #Perms;
            CREATE TABLE #Perms (
                Name        NVARCHAR(100) NOT NULL,
                DisplayName NVARCHAR(200) NOT NULL,
                MenuTitle   NVARCHAR(200) NOT NULL,
                ParentTitle NVARCHAR(200) NULL,   -- NULL = unique title; set to disambiguate duplicates
                Type        NVARCHAR(50)  NOT NULL
            );

            INSERT INTO #Perms (Name, DisplayName, MenuTitle, ParentTitle, Type) VALUES
            -- ═══════════════════════ Administration ═══════════════════════
            ('CanReadDepartment','View Department','Department Master',NULL,'Read'),
            ('CanCreateDepartment','Create Department','Department Master',NULL,'Create'),
            ('CanUpdateDepartment','Update Department','Department Master',NULL,'Update'),
            ('CanDeleteDepartment','Delete Department','Department Master',NULL,'Delete'),
            ('CanManageDepartment','Manage Department','Department Master',NULL,'Manage'),

            ('CanReadEmployee','View Employee','Employee Master',NULL,'Read'),
            ('CanCreateEmployee','Create Employee','Employee Master',NULL,'Create'),
            ('CanUpdateEmployee','Update Employee','Employee Master',NULL,'Update'),
            ('CanDeleteEmployee','Delete Employee','Employee Master',NULL,'Delete'),
            ('CanManageEmployee','Manage Employee','Employee Master',NULL,'Manage'),

            ('CanReadDesignation','View Designation','Designation Master',NULL,'Read'),
            ('CanCreateDesignation','Create Designation','Designation Master',NULL,'Create'),
            ('CanUpdateDesignation','Update Designation','Designation Master',NULL,'Update'),
            ('CanDeleteDesignation','Delete Designation','Designation Master',NULL,'Delete'),
            ('CanManageDesignation','Manage Designation','Designation Master',NULL,'Manage'),

            ('CanReadTax','View Tax','Tax Master',NULL,'Read'),
            ('CanCreateTax','Create Tax','Tax Master',NULL,'Create'),
            ('CanUpdateTax','Update Tax','Tax Master',NULL,'Update'),
            ('CanDeleteTax','Delete Tax','Tax Master',NULL,'Delete'),
            ('CanManageTax','Manage Tax','Tax Master',NULL,'Manage'),

            ('CanReadBank','View Bank','Bank Master',NULL,'Read'),
            ('CanCreateBank','Create Bank','Bank Master',NULL,'Create'),
            ('CanUpdateBank','Update Bank','Bank Master',NULL,'Update'),
            ('CanDeleteBank','Delete Bank','Bank Master',NULL,'Delete'),
            ('CanManageBank','Manage Bank','Bank Master',NULL,'Manage'),

            ('CanReadCourier','View Courier','Courier Master',NULL,'Read'),
            ('CanCreateCourier','Create Courier','Courier Master',NULL,'Create'),
            ('CanUpdateCourier','Update Courier','Courier Master',NULL,'Update'),
            ('CanDeleteCourier','Delete Courier','Courier Master',NULL,'Delete'),
            ('CanManageCourier','Manage Courier','Courier Master',NULL,'Manage'),

            ('CanReadProductSizeMaster','View Product Size','Product Size Master',NULL,'Read'),
            ('CanCreateProductSizeMaster','Create Product Size','Product Size Master',NULL,'Create'),
            ('CanUpdateProductSizeMaster','Update Product Size','Product Size Master',NULL,'Update'),
            ('CanDeleteProductSizeMaster','Delete Product Size','Product Size Master',NULL,'Delete'),
            ('CanManageProductSizeMaster','Manage Product Size','Product Size Master',NULL,'Manage'),


            ('CanReadAnalysisTechnique','View Analysis Technique','Analysis Technique Master',NULL,'Read'),
            ('CanCreateAnalysisTechnique','Create Analysis Technique','Analysis Technique Master',NULL,'Create'),
            ('CanUpdateAnalysisTechnique','Update Analysis Technique','Analysis Technique Master',NULL,'Update'),
            ('CanDeleteAnalysisTechnique','Delete Analysis Technique','Analysis Technique Master',NULL,'Delete'),
            ('CanManageAnalysisTechnique','Manage Analysis Technique','Analysis Technique Master',NULL,'Manage'),

            ('CanReadTPI','View TPI','TPI Master',NULL,'Read'),
            ('CanCreateTPI','Create TPI','TPI Master',NULL,'Create'),
            ('CanUpdateTPI','Update TPI','TPI Master',NULL,'Update'),
            ('CanDeleteTPI','Delete TPI','TPI Master',NULL,'Delete'),
            ('CanManageTPI','Manage TPI','TPI Master',NULL,'Manage'),

            ('CanReadSupplier','View Supplier','Supplier Master',NULL,'Read'),
            ('CanCreateSupplier','Create Supplier','Supplier Master',NULL,'Create'),
            ('CanUpdateSupplier','Update Supplier','Supplier Master',NULL,'Update'),
            ('CanDeleteSupplier','Delete Supplier','Supplier Master',NULL,'Delete'),
            ('CanManageSupplier','Manage Supplier','Supplier Master',NULL,'Manage'),

            ('CanReadEquipment','View Equipment','Equipment',NULL,'Read'),
            ('CanCreateEquipment','Create Equipment','Equipment',NULL,'Create'),
            ('CanUpdateEquipment','Update Equipment','Equipment',NULL,'Update'),
            ('CanDeleteEquipment','Delete Equipment','Equipment',NULL,'Delete'),
            ('CanManageEquipment','Manage Equipment','Equipment',NULL,'Manage'),

            ('CanReadOEM','View OEM','OEM Master',NULL,'Read'),
            ('CanCreateOEM','Create OEM','OEM Master',NULL,'Create'),
            ('CanUpdateOEM','Update OEM','OEM Master',NULL,'Update'),
            ('CanDeleteOEM','Delete OEM','OEM Master',NULL,'Delete'),
            ('CanManageOEM','Manage OEM','OEM Master',NULL,'Manage'),

            ('CanReadCalibrationAgency','View Calibration Agency','Calibration Agency',NULL,'Read'),
            ('CanCreateCalibrationAgency','Create Calibration Agency','Calibration Agency',NULL,'Create'),
            ('CanUpdateCalibrationAgency','Update Calibration Agency','Calibration Agency',NULL,'Update'),
            ('CanDeleteCalibrationAgency','Delete Calibration Agency','Calibration Agency',NULL,'Delete'),
            ('CanManageCalibrationAgency','Manage Calibration Agency','Calibration Agency',NULL,'Manage'),

            -- ═══════════════════════ Specification ═══════════════════════
            -- 'Material Specification' title exists on BOTH folder (200) and leaf (31).
            -- ParentTitle='Material Specification' picks the leaf (31) whose parent is the folder.
            ('CanReadMaterialSpecification','View Material Specification','Material Specification','Material Specification','Read'),
            ('CanCreateMaterialSpecification','Create Material Spec','Material Specification','Material Specification','Create'),
            ('CanUpdateMaterialSpecification','Update Material Spec','Material Specification','Material Specification','Update'),
            ('CanDeleteMaterialSpecification','Delete Material Spec','Material Specification','Material Specification','Delete'),
            ('CanManageMaterialSpecification','Manage Material Spec','Material Specification','Material Specification','Manage'),

            ('CanReadCustomMaterialSpecification','View Custom Material Specification','Custom Material Specification',NULL,'Read'),
            ('CanReadStandardOrganization','View Standard Organization','Standard Organization',NULL,'Read'),

            ('CanReadMetalClassification','View Metal Classification','Metal Classification',NULL,'Read'),
            ('CanCreateMetalClassification','Create Metal Classification','Metal Classification',NULL,'Create'),
            ('CanUpdateMetalClassification','Update Metal Classification','Metal Classification',NULL,'Update'),
            ('CanDeleteMetalClassification','Delete Metal Classification','Metal Classification',NULL,'Delete'),
            ('CanManageMetalClassification','Manage Metal Classification','Metal Classification',NULL,'Manage'),

            ('CanReadChemicalParameter','View Chemical Parameter','Chemical Parameter',NULL,'Read'),
            ('CanReadMechanicalParameter','View Mechanical Parameter','Mechanical Parameter',NULL,'Read'),
            -- Parameter CRUD — linked to Chemical Parameter menu (covers both chemical + mechanical)
            ('CanCreateParameter','Create Parameter','Chemical Parameter',NULL,'Create'),
            ('CanUpdateParameter','Update Parameter','Chemical Parameter',NULL,'Update'),
            ('CanDeleteParameter','Delete Parameter','Chemical Parameter',NULL,'Delete'),
            ('CanManageParameter','Manage Parameter','Chemical Parameter',NULL,'Manage'),

            ('CanReadParameterUnit','View Parameter Unit','Parameter Unit',NULL,'Read'),
            ('CanCreateParameterUnit','Create Parameter Unit','Parameter Unit',NULL,'Create'),
            ('CanUpdateParameterUnit','Update Parameter Unit','Parameter Unit',NULL,'Update'),
            ('CanDeleteParameterUnit','Delete Parameter Unit','Parameter Unit',NULL,'Delete'),
            ('CanManageParameterUnit','Manage Parameter Unit','Parameter Unit',NULL,'Manage'),
            ('CanReadHeatTreatment','View Heat Treatment','Heat Treatment',NULL,'Read'),
            ('CanReadProductCondition','View Product Condition','Product Condition',NULL,'Read'),
            ('CanReadSpecimenOrientation','View Specimen Orientation','Specimen Orientation',NULL,'Read'),
            ('CanReadSpecimenType','View Specimen Type','Specimen Type',NULL,'Read'),
            ('CanReadProductForm','View Product Form','Product Form',NULL,'Read'),
            ('CanReadDimensionalFactors','View Dimensional Factors','Dimensional Factor',NULL,'Read'),
            ('CanReadUniversalCode','View Universal Code','Universal Code Type',NULL,'Read'),
            ('CanReadConditionMaster','View Condition Master','Condition Master',NULL,'Read'),
            ('CanCreateConditionMaster','Create Condition Master','Condition Master',NULL,'Create'),
            ('CanUpdateConditionMaster','Update Condition Master','Condition Master',NULL,'Update'),
            ('CanDeleteConditionMaster','Delete Condition Master','Condition Master',NULL,'Delete'),
            ('CanManageConditionMaster','Manage Condition Master','Condition Master',NULL,'Manage'),

            ('CanReadDiscipline','View Discipline Master','Discipline Master',NULL,'Read'),
            ('CanCreateDiscipline','Create Discipline Master','Discipline Master',NULL,'Create'),
            ('CanUpdateDiscipline','Update Discipline Master','Discipline Master',NULL,'Update'),
            ('CanDeleteDiscipline','Delete Discipline Master','Discipline Master',NULL,'Delete'),
            ('CanManageDiscipline','Manage Discipline Master','Discipline Master',NULL,'Manage'),

            ('CanReadTestGroup','View Test Group','Universal Test Group & Effective Config',NULL,'Read'),
            ('CanCreateTestGroup','Create Test Group','Universal Test Group & Effective Config',NULL,'Create'),
            ('CanUpdateTestGroup','Update Test Group','Universal Test Group & Effective Config',NULL,'Update'),
            ('CanDeleteTestGroup','Delete Test Group','Universal Test Group & Effective Config',NULL,'Delete'),
            ('CanManageTestGroup','Manage Test Group','Universal Test Group & Effective Config',NULL,'Manage'),

            ('CanViewConfigurationAdjustment','View Configuration Adjustment','Universal Test Group & Effective Config',NULL,'Read'),
            ('CanCreateConfigurationAdjustment','Create Configuration Adjustment','Universal Test Group & Effective Config',NULL,'Create'),
            ('CanApplyConfigurationAdjustment','Apply Configuration Adjustment','Universal Test Group & Effective Config',NULL,'Update'),
            ('CanApproveConfigurationAdjustment','Approve Configuration Adjustment','Universal Test Group & Effective Config',NULL,'Approve'),

            ('CanReadClassification','View Classification Master','Classification Master',NULL,'Read'),
            ('CanCreateClassification','Create Classification Master','Classification Master',NULL,'Create'),
            ('CanUpdateClassification','Update Classification Master','Classification Master',NULL,'Update'),
            ('CanDeleteClassification','Delete Classification Master','Classification Master',NULL,'Delete'),
            ('CanManageClassification','Manage Classification Master','Classification Master',NULL,'Manage'),

            ('CanReadAcceptanceCriteria','View Acceptance Criteria Master','Acceptance Criteria Master',NULL,'Read'),
            ('CanCreateAcceptanceCriteria','Create Acceptance Criteria Master','Acceptance Criteria Master',NULL,'Create'),
            ('CanUpdateAcceptanceCriteria','Update Acceptance Criteria Master','Acceptance Criteria Master',NULL,'Update'),
            ('CanDeleteAcceptanceCriteria','Delete Acceptance Criteria Master','Acceptance Criteria Master',NULL,'Delete'),
            ('CanManageAcceptanceCriteria','Manage Acceptance Criteria Master','Acceptance Criteria Master',NULL,'Manage'),

            ('CanReadEquipmentRequirement','View Equipment Requirement Master','Equipment Requirement Master',NULL,'Read'),
            ('CanCreateEquipmentRequirement','Create Equipment Requirement Master','Equipment Requirement Master',NULL,'Create'),
            ('CanUpdateEquipmentRequirement','Update Equipment Requirement Master','Equipment Requirement Master',NULL,'Update'),
            ('CanDeleteEquipmentRequirement','Delete Equipment Requirement Master','Equipment Requirement Master',NULL,'Delete'),
            ('CanManageEquipmentRequirement','Manage Equipment Requirement Master','Equipment Requirement Master',NULL,'Manage'),

            ('CanReadEquipmentType','View Equipment Type Master','Equipment Type Master',NULL,'Read'),
            ('CanCreateEquipmentType','Create Equipment Type Master','Equipment Type Master',NULL,'Create'),
            ('CanUpdateEquipmentType','Update Equipment Type Master','Equipment Type Master',NULL,'Update'),
            ('CanDeleteEquipmentType','Delete Equipment Type Master','Equipment Type Master',NULL,'Delete'),
            ('CanManageEquipmentType','Manage Equipment Type Master','Equipment Type Master',NULL,'Manage'),

            ('CanReadFactorConversion','View Factor Conversion Master','Factor Conversion Master',NULL,'Read'),
            ('CanCreateFactorConversion','Create Factor Conversion Master','Factor Conversion Master',NULL,'Create'),
            ('CanUpdateFactorConversion','Update Factor Conversion Master','Factor Conversion Master',NULL,'Update'),
            ('CanDeleteFactorConversion','Delete Factor Conversion Master','Factor Conversion Master',NULL,'Delete'),
            ('CanManageFactorConversion','Manage Factor Conversion Master','Factor Conversion Master',NULL,'Manage'),

            ('CanReadMeasurementUncertainty','View Measurement Uncertainty Master','Measurement Uncertainty Master',NULL,'Read'),
            ('CanCreateMeasurementUncertainty','Create Measurement Uncertainty Master','Measurement Uncertainty Master',NULL,'Create'),
            ('CanUpdateMeasurementUncertainty','Update Measurement Uncertainty Master','Measurement Uncertainty Master',NULL,'Update'),
            ('CanDeleteMeasurementUncertainty','Delete Measurement Uncertainty Master','Measurement Uncertainty Master',NULL,'Delete'),
            ('CanManageMeasurementUncertainty','Manage Measurement Uncertainty Master','Measurement Uncertainty Master',NULL,'Manage'),

            -- ═══════════════════════ PHASE 1G: Execution Layout ═══════════════════════
            ('CanReadExecutionLayout','View Execution Layout Master','Execution Layout Master',NULL,'Read'),
            ('CanCreateExecutionLayout','Create Execution Layout Master','Execution Layout Master',NULL,'Create'),
            ('CanUpdateExecutionLayout','Update Execution Layout Master','Execution Layout Master',NULL,'Update'),
            ('CanDeleteExecutionLayout','Delete Execution Layout Master','Execution Layout Master',NULL,'Delete'),
            ('CanManageExecutionLayout','Manage Execution Layout Master','Execution Layout Master',NULL,'Manage'),

            -- ═══════════════════════ PHASE 7: Universal Result / Compliance ═══════════════════════
            ('CanReadUniversalResult','View Universal Result','Universal Test Execution',NULL,'Read'),
            ('CanEvaluateUniversalResult','Evaluate Universal Result','Universal Test Execution',NULL,'Create'),
            ('CanFinalizeUniversalResult','Finalize Universal Result','Universal Test Execution',NULL,'Update'),
            ('CanReworkUniversalResult','Rework Universal Result','Universal Test Execution',NULL,'Update'),

            -- ═══════════════════════ PHASE 8: Universal Review / Approval ═══════════════════════
            ('CanReadUniversalReview','View Universal Review','Universal Test Execution',NULL,'Read'),
            ('CanReviewUniversalResult','Review Universal Result','Universal Test Execution',NULL,'Update'),
            ('CanVerifyUniversalResult','Verify Universal Result','Universal Test Execution',NULL,'Verify'),
            ('CanApproveUniversalResult','Approve Universal Result','Universal Test Execution',NULL,'Approve'),

            -- ═══════════════════════ PHASE 9: Universal Report ═══════════════════════
            ('CanReadUniversalReport','View Universal Report','Universal Test Execution',NULL,'Read'),
            ('CanGenerateUniversalReport','Generate Universal Report','Universal Test Execution',NULL,'Create'),
            ('CanReleaseUniversalReport','Release Universal Report','Universal Test Execution',NULL,'Approve'),
            ('CanReissueUniversalReport','Reissue Universal Report','Universal Test Execution',NULL,'Update'),

            -- 'Product Specification' title exists on BOTH folder (202) and leaf (33).
            -- ParentTitle='Product Specification' picks the leaf (33).
            ('CanReadProductMaster','View Product Master','Product / Material Master',NULL,'Read'),
            ('CanCreateProductMaster','Create Product Master','Product / Material Master',NULL,'Create'),
            ('CanUpdateProductMaster','Update Product Master','Product / Material Master',NULL,'Update'),
            ('CanDeleteProductMaster','Delete Product Master','Product / Material Master',NULL,'Delete'),
            ('CanManageProductMaster','Manage Product Master','Product / Material Master',NULL,'Manage'),

            ('CanReadCustomProductSpecification','View Custom Product Specification','Custom Product Specification',NULL,'Read'),

            -- ═══════════════════════ Test ═══════════════════════
            ('CanReadLaboratoryTest','View Laboratory Test','Laboratory Test Master',NULL,'Read'),
            ('CanCreateLaboratoryTest','Create Laboratory Test','Laboratory Test Master',NULL,'Create'),
            ('CanUpdateLaboratoryTest','Update Laboratory Test','Laboratory Test Master',NULL,'Update'),
            ('CanDeleteLaboratoryTest','Delete Laboratory Test','Laboratory Test Master',NULL,'Delete'),
            ('CanManageLaboratoryTest','Manage Laboratory Test','Laboratory Test Master',NULL,'Manage'),

            ('CanReadTestMethodSpecification','View Test Method Specification','Test Method Specification',NULL,'Read'),
            ('CanCreateTestMethodSpecification','Create Test Method Spec','Test Method Specification',NULL,'Create'),
            ('CanUpdateTestMethodSpecification','Update Test Method Spec','Test Method Specification',NULL,'Update'),
            ('CanDeleteTestMethodSpecification','Delete Test Method Spec','Test Method Specification',NULL,'Delete'),
            ('CanManageTestMethodSpecification','Manage Test Method Spec','Test Method Specification',NULL,'Manage'),
            ('CanImportTestMethodSpecification','Import Test Method Spec','Test Method Specification',NULL,'Action'),

            -- 'Invoice Case' exists under ''Test'' (ID 37) AND under ''Invoice'' (ID 48).
            -- ParentTitle=''Test'' picks ID 37 (permissions side); ID 48 is a UI shortcut with same permissions.
            ('CanReadInvoiceCase','View Invoice Case','Invoice Case','Test','Read'),
            ('CanCreateInvoiceCase','Create Invoice Case','Invoice Case','Test','Create'),
            ('CanUpdateInvoiceCase','Update Invoice Case','Invoice Case','Test','Update'),
            ('CanDeleteInvoiceCase','Delete Invoice Case','Invoice Case','Test','Delete'),
            ('CanManageInvoiceCase','Manage Invoice Case','Invoice Case','Test','Manage'),

            -- ═══════════════════════ Customer ═══════════════════════
            ('CanReadCompanyCategory','View Company Category','Company Category',NULL,'Read'),
            ('CanCreateCompanyCategory','Create Company Category','Company Category',NULL,'Create'),
            ('CanUpdateCompanyCategory','Update Company Category','Company Category',NULL,'Update'),
            ('CanDeleteCompanyCategory','Delete Company Category','Company Category',NULL,'Delete'),
            ('CanManageCompanyCategory','Manage Company Category','Company Category',NULL,'Manage'),

            ('CanReadCustomerMaster','View Customer Master','Customer Master',NULL,'Read'),
            ('CanCreateCustomerMaster','Create Customer','Customer Master',NULL,'Create'),
            ('CanUpdateCustomerMaster','Update Customer','Customer Master',NULL,'Update'),
            ('CanDeleteCustomerMaster','Delete Customer','Customer Master',NULL,'Delete'),
            ('CanManageCustomerMaster','Manage Customer','Customer Master',NULL,'Manage'),

            -- ═══════════════════════ Sample ═══════════════════════
            ('CanReadInward','View Inward','Inward',NULL,'Read'),
            ('CanReadSampleInward','Read Sample Inward','Inward',NULL,'Read'),
            ('CanCreateSampleInward','Create Sample Inward','Inward',NULL,'Create'),
            ('CanUpdateSampleInward','Update Sample Inward','Inward',NULL,'Update'),
            ('CanDeleteSampleInward','Delete Sample Inward','Inward',NULL,'Delete'),
            ('CanManageSampleInward','Manage Sample Inward','Inward',NULL,'Manage'),

            ('CanReadPlan','View Plan','Plan',NULL,'Read'),
            ('CanCreatePlan','Create Plan','Plan',NULL,'Create'),
            ('CanUpdatePlan','Update Plan','Plan',NULL,'Update'),
            ('CanDeletePlan','Delete Plan','Plan',NULL,'Delete'),
            ('CanManagePlan','Manage Plan','Plan',NULL,'Manage'),
            ('CanApprovePlan','Approve Plan Change','Plan',NULL,'Action'),
            ('CanRejectPlan','Reject Plan Change','Plan',NULL,'Action'),

            ('CanReadReview','View Review','Review',NULL,'Read'),
            ('CanApproveReview','Approve Review','Review',NULL,'Action'),
            ('CanRejectReview','Reject Review','Review',NULL,'Action'),
            ('CanManageReview','Manage Review','Review',NULL,'Manage'),

            ('CanReadSampleCutting','View Sample Cutting','Sample Cutting',NULL,'Read'),
            ('CanCreateSampleCutting','Create Cutting','Sample Cutting',NULL,'Create'),
            ('CanUpdateSampleCutting','Update Cutting','Sample Cutting',NULL,'Update'),
            ('CanManageSampleCutting','Manage Sample Prep','Sample Cutting',NULL,'Manage'),

            ('CanReadMachiningChallan','View Machining Challan','Machining Charges',NULL,'Read'),

            ('CanReadCuttingPrice','View Cutting Price','Cutting Price Master',NULL,'Read'),
            ('CanCreateCuttingPrice','Create Cutting Price','Cutting Price Master',NULL,'Create'),
            ('CanUpdateCuttingPrice','Update Cutting Price','Cutting Price Master',NULL,'Update'),
            ('CanDeleteCuttingPrice','Delete Cutting Price','Cutting Price Master',NULL,'Delete'),
            ('CanManageCuttingPrice','Manage Cutting Price','Cutting Price Master',NULL,'Manage'),

            ('CanReadMachiningCharge','View Machining Charge','Machining Charge Master',NULL,'Read'),
            ('CanCreateMachiningCharge','Create Machining Charge','Machining Charge Master',NULL,'Create'),
            ('CanUpdateMachiningCharge','Update Machining Charge','Machining Charge Master',NULL,'Update'),
            ('CanDeleteMachiningCharge','Delete Machining Charge','Machining Charge Master',NULL,'Delete'),
            ('CanManageMachiningCharge','Manage Machining Charge','Machining Charge Master',NULL,'Manage'),

            -- ═══════════════════════ Invoice ═══════════════════════
            ('CanReadInvoiceCaseConfig','View Invoice Case Config','Invoice Case Config',NULL,'Read'),
            ('CanCreateInvoiceCaseConfig','Create Invoice Case Config','Invoice Case Config',NULL,'Create'),
            ('CanUpdateInvoiceCaseConfig','Update Invoice Case Config','Invoice Case Config',NULL,'Update'),
            ('CanDeleteInvoiceCaseConfig','Delete Invoice Case Config','Invoice Case Config',NULL,'Delete'),
            ('CanManageInvoiceCaseConfig','Manage Invoice Case Config','Invoice Case Config',NULL,'Manage'),

            -- ═══════════════════════ NABL: General Requirements ═══════════════════════
            ('CanReadConfidentialityAgreement','View Confidentiality Agreement','F-2: Confidentiality Agree.',NULL,'Read'),
            ('CanReadImpartialityAgreement','View Impartiality Agreement','F-4: Impartiality Agree.',NULL,'Read'),

            -- ═══════════════════════ NABL: Structural ═══════════════════════
            ('CanReadOrgChart','View Organization Chart','Organization Chart',NULL,'Read'),

            -- ═══════════════════════ NABL: Personnel ═══════════════════════
            ('CanReadJobDescription','View Job Description','F-1: Job Description',NULL,'Read'),
            ('CanReadRA','View Resp. & Authority','F-3: Resp. & Authority',NULL,'Read'),
            ('CanReadCompetenceRequirement','View Competence Requirement','F-5: Competence Req.',NULL,'Read'),
            ('CanReadInductionTraining','View Induction Training','F-6: Induction Training',NULL,'Read'),
            ('CanReadEmployeeCompetence','View Employee Competence','F-7: Competence Report',NULL,'Read'),
            ('CanReadTrainingPlan','View Training Plan','F-8: Training Plan',NULL,'Read'),
            ('CanReadTrainingAttendance','View Training Attendance','F-9: Training Attendance',NULL,'Read'),
            ('CanReadTrainingEffectiveness','View Training Effectiveness','F-10: Training Effectiv.',NULL,'Read'),
            ('CanReadSkillMatrix','View Skill Matrix','F-11: Skill Matrix',NULL,'Read'),
            ('CanReadEmployeeAuthorization','View Employee Authorization','F-13: Employee Authorization',NULL,'Read'),

            -- ═══════════════════════ NABL: Facilities ═══════════════════════
            ('CanReadEnvironmentMonitoring','View Environment Monitoring','F-12: Environment Mon.',NULL,'Read'),

            -- ═══════════════════════ NABL: Equipment ═══════════════════════
            ('CanReadEquipmentHistory','View Equipment History','F-14: Equipment History',NULL,'Read'),
            ('CanReadCalibrationReview','View Calibration Review','F-15: Calibration Review',NULL,'Read'),
            ('CanReadIntermediateCheck','View Intermediate Check','F-16: Intermediate Check',NULL,'Read'),
            ('CanReadReferenceMaterial','View Reference Material','F-17: Ref. Material List',NULL,'Read'),
            ('CanReadCRMConsumption','View CRM Consumption','F-18: CRM Consumption',NULL,'Read'),

            -- ═══════════════════════ NABL: External Products ═══════════════════════
            ('CanReadSupplierRegistration','View Supplier Registration','F-19: Supplier Reg.',NULL,'Read'),
            ('CanReadApprovedSupplier','View Approved Supplier','F-20: Approved Suppliers',NULL,'Read'),
            ('CanReadPurchaseIndent','View Purchase Indent','F-21: Purchase Indent',NULL,'Read'),
            ('CanReadPurchaseOrder','View Purchase Order','F-22: Purchase Order',NULL,'Read'),
            ('CanReadProductInspection','View Product Inspection','F-23: Inspection Plan',NULL,'Read'),
            ('CanReadIncomingMaterial','View Incoming Material','F-24: Incoming Mat. Rec.',NULL,'Read'),
            ('CanReadMaterialVerification','View Material Verification','F-25: Mat. Verification',NULL,'Read'),
            ('CanReadSupplierEvaluation','View Supplier Evaluation','F-26: Supplier Eval.',NULL,'Read'),

            -- ═══════════════════════ NABL: Process Requirements ═══════════════════════
            ('CanReadTestRequest','View Test Request','F-27: Test Request',NULL,'Read'),
            ('CanReadTestMethod','View Test Method','F-28: Test Methods',NULL,'Read'),
            ('CanReadMethodVerification','View Method Verification','F-29: Verification',NULL,'Read'),
            ('CanReadMethodValidation','View Method Validation','F-30: Validation',NULL,'Read'),
            ('CanReadSampleInwardRegister','View Inward Register','F-31: Inward Register',NULL,'Read'),
            ('CanReadSampleMusterRegister','View Muster Register','F-32: Muster Register',NULL,'Read'),
            ('CanReadSampleLabel','View Sample Label','F-33: Sample Label',NULL,'Read'),
            ('CanReadTechnicalRawData','View Technical Raw Data','F-34: Technical Raw Data',NULL,'Read'),
            ('CanReadUncertainty','View Uncertainty Records','F-35: Uncertainty Rec.',NULL,'Read'),
            ('CanReadPTPlan','View PT/ILC Plan','F-36: PT / ILC Plan',NULL,'Read'),
            ('CanReadQCPlan','View QC Plan','F-37: QC Plan',NULL,'Read'),
            ('CanReadRetesting','View Retesting Records','F-38: Retesting Rec.',NULL,'Read'),
            ('CanReadTestReport','View Test Report','F-39: Test Report',NULL,'Read'),
            ('CanReadComplaintRegister','View Complaint Register','F-40: Complaint Reg.',NULL,'Read'),
            ('CanReadNonConformingWork','View Non-Conforming Work','F-41: NC Work Records',NULL,'Read'),

            -- ═══════════════════════ NABL: Management System ═══════════════════════
            ('CanReadMasterDocument','View Master Document','F-43: Master Document',NULL,'Read'),
            ('CanReadDocChangeRequest','View Document Change Request','F-44: Doc. Change Req.',NULL,'Read'),
            ('CanReadDocumentReview','View Document Review','F-45: Doc. Review Rec.',NULL,'Read'),
            ('CanReadRiskAssessment','View Risk Assessment','F-46: Risk Assessment',NULL,'Read'),
            ('CanReadCustomerFeedback','View Customer Feedback','F-47: Cust. Feedback',NULL,'Read'),
            ('CanReadFeedbackAnalysis','View Feedback Analysis','F-48: Feedback Analys.',NULL,'Read'),
            ('CanReadNCAction','View NC & Corrective Action','F-42: NC & Corr. Action',NULL,'Read'),
            ('CanReadInternalAuditor','View Internal Auditors','F-49: Internal Auditors',NULL,'Read'),
            ('CanReadAuditPlan','View Audit Plan','F-50: Audit Plan',NULL,'Read'),
            ('CanReadAuditChecklist','View Audit Checklist','F-51: Audit Checklist',NULL,'Read'),
            ('CanReadAuditSummary','View Audit Summary','F-52: Audit Summary',NULL,'Read'),
            ('CanReadMeetingAgenda','View Meeting Agenda','F-53: Meeting Agenda',NULL,'Read'),
            ('CanReadMeetingMinutes','View Meeting Minutes','F-54: Meeting Minutes',NULL,'Read'),

            -- ═══════════════════════ Lab Scope ═══════════════════════
            ('CanReadLabScopeMaster','View Lab Scope','Lab Scope Master',NULL,'Read'),
            ('CanCreateLabScopeMaster','Create Lab Scope','Lab Scope Master',NULL,'Create'),
            ('CanUpdateLabScopeMaster','Update Lab Scope','Lab Scope Master',NULL,'Update'),
            ('CanDeleteLabScopeMaster','Delete Lab Scope','Lab Scope Master',NULL,'Delete'),
            ('CanManageLabScopeMaster','Manage Lab Scope','Lab Scope Master',NULL,'Manage'),

            -- ═══════════════════════ User Management (NABL) ═══════════════════════
            ('CanReadLabEmployeeMaster','View Lab Employee','Lab Employee Master',NULL,'Read'),
            ('CanReadLabScore','View Lab Score','Lab Score Master',NULL,'Read'),

            -- ═══════════════════════ Testing ═══════════════════════
            ('CanReadTestingDashboard','View Testing Dashboard','Testing Dashboard',NULL,'Read'),
            ('CanReadPerformTest','View Perform Test','Perform Test',NULL,'Read'),
            ('CanReadLongTermTracking','View Long Term Tracking','Long Term Tracking',NULL,'Read'),
            ('CanReadTestResults','View Test Results','Test Results',NULL,'Read'),
            ('CanReadTesting','Read Testing','Testing',NULL,'Read'),
            ('CanManageTesting','Manage Testing','Testing',NULL,'Manage'),
            ('CanPerformTest','Perform Test','Perform Test',NULL,'Action'),

            -- ═══════════════════════ Configuration ═══════════════════════
            ('CanReadConfiguration','View Configuration','Configuration Manager',NULL,'Read'),
            ('CanManageSettings','Manage Settings','Configuration Manager',NULL,'Manage'),
            ('CanReadMenuManagement','View Menu Management','Menu Management',NULL,'Read'),
            ('CanCreateMenu','Create Menu','Menu Management',NULL,'Create'),
            ('CanUpdateMenu','Update Menu','Menu Management',NULL,'Update'),
            ('CanDeleteMenu','Delete Menu','Menu Management',NULL,'Delete'),
            ('CanReadMenuPermission','View Menu Permission','Menu Permission',NULL,'Read'),
            ('CanAssignMenuPermission','Assign Menu Permission','Menu Permission',NULL,'Action'),
            ('CanReadRoleManagement','View Role Management','Role Management',NULL,'Read'),
            ('CanReadAdmin','Read Admin','Role Management',NULL,'Read'),
            ('CanCreateRole','Create Role','Role Management',NULL,'Create'),
            ('CanUpdateRole','Update Role','Role Management',NULL,'Update'),
            ('CanDeleteRole','Delete Role','Role Management',NULL,'Delete'),
            ('CanCreateAdmin','Create Admin','Role Management',NULL,'Create'),
            ('CanUpdateAdmin','Update Admin','Role Management',NULL,'Update'),
            ('CanDeleteAdmin','Delete Admin','Role Management',NULL,'Delete'),
            ('CanManageAdmin','Manage Admin','Role Management',NULL,'Manage'),
            ('CanReadUserPermission','View User Permission','User Permission',NULL,'Read'),
            ('CanReadUser','Read User','User Permission',NULL,'Read'),
            ('CanCreateUser','Create User','User Permission',NULL,'Create'),
            ('CanUpdateUser','Update User','User Permission',NULL,'Update'),
            ('CanDeleteUser','Delete User','User Permission',NULL,'Delete'),
            ('CanAssignUserPermission','Assign User Permission','User Permission',NULL,'Action'),
            ('CanResetUserPassword','Reset User Password','User Permission',NULL,'Action'),
            ('CanReadWorkflow','View Workflow','Workflow',NULL,'Read'),

            -- ═══════════════════════ Reporting ═══════════════════════
            ('CanReadReporting','View Reporting','Reporting Dashboard',NULL,'Read'),
            ('CanManageReporting','Manage Reporting','Reporting Dashboard',NULL,'Manage'),
            ('CanApproveReport','Approve Report','Reporting Dashboard',NULL,'Action'),
            ('CanAmendReport','Amend Report','Reporting Dashboard',NULL,'Action'),
            ('CanReadReportFormat','View Report Formats','Report Formats',NULL,'Read'),
            ('CanManageReportFormat','Manage Report Formats','Report Formats',NULL,'Manage'),

            -- ═══════════════════════ Accounts ═══════════════════════
            ('CanReadAccount','Read Account','Accounts',NULL,'Read'),
            ('CanManageAccount','Manage Account','Accounts',NULL,'Manage'),
            ('CanReadAccountsDashboard','View Accounts Dashboard','Accounts Dashboard',NULL,'Read'),
            ('CanReadCaseAccounts','View Case Accounts','Case Accounts',NULL,'Read'),
            ('CanGeneratePI','Generate Proforma Invoice','Case Accounts',NULL,'Action'),
            ('CanGenerateInvoice','Generate Invoice','Case Accounts',NULL,'Action'),
            ('CanManageInvoice','Manage Invoice','Case Accounts',NULL,'Manage'),
            ('CanReadInvoiceLineItem','Read Invoice Line Items','Case Accounts',NULL,'Read'),
            ('CanManageInvoiceLineItem','Manage Invoice Line Items','Case Accounts',NULL,'Manage'),
            ('CanCalculatePricing','Calculate Case Pricing','Case Accounts',NULL,'Action'),
            ('CanValidatePricing','Validate Case Pricing','Case Accounts',NULL,'Action'),
            ('CanCloseCase','Close Case','Case Accounts',NULL,'Action'),
            ('CanReadCustomerLedger','View Customer Ledger','Customer Ledger',NULL,'Read'),
            ('CanReadRecordPayment','View Record Payment','Record Payment',NULL,'Read'),
            ('CanRecordPayment','Record Payment','Record Payment',NULL,'Action'),
            ('CanReadReceipt','Read Receipt','Record Payment',NULL,'Read'),
            ('CanProcessPayment','Process Payment','Record Payment',NULL,'Action'),
            ('CanValidatePayment','Validate Payment Token','Record Payment',NULL,'Action'),
            ('CanSendPaymentLink','Send Payment Link','Record Payment',NULL,'Action'),
            ('CanReadAgingReport','View Aging Report','Aging Report',NULL,'Read'),
            ('CanReadCollectionSummary','Read Collection Summary','Aging Report',NULL,'Read'),
            ('CanReadCreditStatus','Read Credit Status','Aging Report',NULL,'Read'),
            ('CanReadOutstandingReport','View Outstanding Report','Outstanding Report',NULL,'Read'),
            ('CanReadCustomerPO','View Customer Purchase Orders','Customer Purchase Orders',NULL,'Read'),
            ('CanCreateCustomerPO','Create Customer PO','Customer Purchase Orders',NULL,'Create'),
            ('CanUpdateCustomerPO','Update Customer PO','Customer Purchase Orders',NULL,'Update'),
            ('CanDeleteCustomerPO','Delete Customer PO','Customer Purchase Orders',NULL,'Delete'),
            ('CanManageCustomerPO','Manage Customer PO','Customer Purchase Orders',NULL,'Manage'),

            -- ═══════════════════════ Backend [RequirePermission] guards ═══════════════════════
            ('TEST_RESULT_SAVE','Save Test Result','Test Results',NULL,'Action'),
            ('TEST_PRICE_OVERRIDE','Override Test Price','Test Results',NULL,'Action'),
            ('TEST_RESULT_VERIFY','Verify Test Result','Test Results',NULL,'Action'),
            ('INVOICE_GENERATE','Generate Invoice (Backend)','Case Accounts',NULL,'Action');

            -- ── Resolve MenuTitle → MenuID and insert only new permissions ──
            INSERT INTO PermissionMasters (Name, DisplayName, MenuID, Type)
            SELECT p.Name, p.DisplayName, m.ID, p.Type
            FROM #Perms p
            JOIN  MenuMasters m      ON m.Title  = p.MenuTitle
            LEFT JOIN MenuMasters par ON par.ID   = m.ParentID
            WHERE (p.ParentTitle IS NULL OR par.Title = p.ParentTitle)
              AND NOT EXISTS (SELECT 1 FROM PermissionMasters pm WHERE pm.Name = p.Name);

            DROP TABLE #Perms;
        END
        ");

        // Step 2: Execute the SP to seed / update permissions
        await db.Database.ExecuteSqlRawAsync("EXEC usp_SeedPermissions");
    }

    // ───────────────────────────────────────────────
    // 4. ROLE-MENU MAPPINGS  (Admin role → all menus)
    // ───────────────────────────────────────────────
    private static async Task SeedRoleMenuMappingsAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            DECLARE @AdminRoleID BIGINT = (SELECT TOP 1 ID FROM RoleMasters WHERE Name = N'Admin' AND IsActive = 1);
            IF @AdminRoleID IS NULL RETURN;

            -- Map Admin to every menu — no hardcoded IDs, picks up new menus automatically
            INSERT INTO RoleMenuMappings (RoleID, MenuID)
            SELECT @AdminRoleID, m.ID
            FROM MenuMasters m
            WHERE NOT EXISTS (
                SELECT 1 FROM RoleMenuMappings rm
                WHERE rm.RoleID = @AdminRoleID AND rm.MenuID = m.ID
              );
        ");
    }

    // ───────────────────────────────────────────────
    // 4b. PHASE 1 MENU FIXUPS — align legacy menu rows to FE-exact titles/routes/parents.
    // ID-targeted + value-guarded (fully idempotent). MUST run before SeedMenusAsync so the
    // SP's (Title + ParentID) NOT EXISTS guard sees the corrected identity and never duplicates.
    // ───────────────────────────────────────────────
    private static async Task SeedPhase1MenuFixupsAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            UPDATE dbo.MenuMasters SET Title = N'Laboratory Test Master'
            WHERE ID = 750528 AND Title <> N'Laboratory Test Master';

            -- Orphan merge: legacy 'Analysis Technique' (750654, Administration) into the
            -- FE-exact 'Analysis Technique Master' (under Test). Re-points permissions + role
            -- mappings by ID, then removes the orphan. Fully idempotent.
            IF EXISTS (SELECT 1 FROM dbo.MenuMasters WHERE ID = 750654 AND Title = N'Analysis Technique')
            BEGIN
                DECLARE @NewTech BIGINT = (SELECT TOP 1 ID FROM dbo.MenuMasters
                    WHERE Title = N'Analysis Technique Master' AND Route = N'/analysis-technique' AND ID <> 750654);
                IF @NewTech IS NOT NULL
                BEGIN
                    UPDATE dbo.PermissionMasters SET MenuID = @NewTech WHERE MenuID = 750654;
                    DELETE FROM dbo.RoleMenuMappings
                    WHERE MenuID = 750654
                      AND EXISTS (SELECT 1 FROM dbo.RoleMenuMappings r2
                                  WHERE r2.RoleID = dbo.RoleMenuMappings.RoleID AND r2.MenuID = @NewTech);
                    UPDATE dbo.RoleMenuMappings SET MenuID = @NewTech WHERE MenuID = 750654;
                    DELETE FROM dbo.MenuMasters WHERE ID = 750654 AND Title = N'Analysis Technique';
                END
            END

            UPDATE dbo.MenuMasters SET Title = N'Lab Scope / NABL Master', Route = N'/lab-scope'
            WHERE ID = 750544
              AND (Title <> N'Lab Scope / NABL Master' OR Route <> N'/lab-scope' OR Route IS NULL);

            -- Phase 7/8: 'Universal Test Execution' menu row (permission seeder JOINs
            -- MenuMasters.Title; without this row the 8 UniversalResult/Review permission
            -- rows are silently skipped and gated buttons stay hidden). Idempotent:
            -- inserts once by title, then ensures exact route/parent. Identity-safe.
            DECLARE @TestParentId BIGINT = (SELECT TOP 1 ID FROM dbo.MenuMasters WHERE Title = N'Test' AND Route IS NULL);
            IF @TestParentId IS NULL AND EXISTS (SELECT 1 FROM dbo.MenuMasters WHERE ID = 750505)
                SET @TestParentId = 750505;

            IF NOT EXISTS (SELECT 1 FROM dbo.MenuMasters WHERE Title = N'Universal Test Execution')
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM dbo.MenuMasters WHERE ID = 760675)
                BEGIN
                    SET IDENTITY_INSERT dbo.MenuMasters ON;
                    INSERT INTO dbo.MenuMasters (ID, Title, Icon, IsExpanded, Route, Color, ParentID)
                    VALUES (760675, N'Universal Test Execution', N'bi-clipboard2-check', 0, N'/universal-test-execution', NULL, @TestParentId);
                    SET IDENTITY_INSERT dbo.MenuMasters OFF;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.MenuMasters (Title, Icon, IsExpanded, Route, Color, ParentID)
                    VALUES (N'Universal Test Execution', N'bi-clipboard2-check', 0, N'/universal-test-execution', NULL, @TestParentId);
                END
            END
            ELSE
            BEGIN
                UPDATE dbo.MenuMasters SET Route = N'/universal-test-execution', ParentID = @TestParentId
                WHERE Title = N'Universal Test Execution'
                  AND (Route <> N'/universal-test-execution' OR Route IS NULL OR ParentID <> @TestParentId);
            END
        ");
    }

    // ───────────────────────────────────────────────
    // 5. CONFIGURATIONS  (3 keys not yet seeded by migrations)
    // ───────────────────────────────────────────────
    private static async Task SeedConfigurationsAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'REPORT_CONDITIONS' AND CompanyCode = N'LIMS')
                INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, CompanyCode, IsActive)
                VALUES (N'REPORT_CONDITIONS', N'Report',
N'1) DMSL certifies that the tests/calibrations were conducted on the sample submitted by the customer.
2) Reproduction of the report is not allowed without written permission of DMSL.
3) Customer-provided samples are stored for 15 days from the dispatch date.
4) DMSL shall not be responsible for result deviations due to sample discrepancy or variations in manufacturing processes.
5) The test results are valid only for the sample submitted.
6) Statement of Conformity for Y(E) is given without considering guardbands element.
7) This report shall not be used for any legal proceedings without prior written consent.',
                N'text', N'Footer conditions for test certificate', 1, GETUTCDATE(), 1, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'COMPANY_STAMP_PATH' AND CompanyCode = N'LIMS')
                INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'COMPANY_STAMP_PATH', N'REPORTING', N'', N'string', N'File path to company stamp image for reports', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'AmendmentChargeableAmount' AND CompanyCode = N'LIMS')
                INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'AmendmentChargeableAmount', N'BILLING', N'500', N'decimal', N'Charge amount for paid customer amendments', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'Customer Type' AND CompanyCode = N'LIMS')
                INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Customer Type', N'CUSTOMER', N'Walk in,Credit Customer,Relationship Credit Customer', N'csv', N'Available customer types for the customer master form', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'PaymentGatewayEnabled' AND CompanyCode = N'LIMS')
                INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'PaymentGatewayEnabled', N'BILLING', N'false', N'boolean', N'Enable/disable Razorpay payment gateway. Set to true when credentials are configured.', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'Entity Type' AND CompanyCode = N'LIMS')
                INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Entity Type', N'dropdown', N'Request Review|Report Review|Report Amendment|Test Result Verification|Customer Field Change', N'string', N'Entity types used for workflow configuration. Values must match the exact strings used by the workflow engine.', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'ProductPrefix' AND CompanyCode = N'LIMS')
                INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'ProductPrefix', N'dropdown', N'Grade|Class|Designation|Type|Series', N'string', N'Product Master Grade Prefix Options', 0, GETUTCDATE(), N'LIMS', 1);

            -- Migrate existing installs: rename old short-form TestResult to canonical Test Result Verification

            UPDATE Configurations
            SET [Value] = REPLACE([Value], N'TestResult', N'Test Result Verification')
            WHERE KeyName = N'Entity Type' AND CompanyCode = N'LIMS'
              AND [Value] LIKE N'%TestResult%'
              AND [Value] NOT LIKE N'%Test Result Verification%';

            -- Add Customer Field Change to existing installs if missing
            UPDATE Configurations
            SET [Value] = [Value] + N'|Customer Field Change'
            WHERE KeyName = N'Entity Type' AND CompanyCode = N'LIMS'
              AND [Value] NOT LIKE N'%Customer Field Change%';

            -- Fix any Workflow definitions using the old short-form label
            UPDATE Workflows SET EntityType = N'Test Result Verification'
            WHERE EntityType = N'TestResult';

            -- Fix any WorkflowInstances using the old short-form label
            UPDATE WorkflowInstances SET EntityType = N'Test Result Verification'
            WHERE EntityType = N'TestResult';

            IF NOT EXISTS (SELECT 1 FROM Configurations WHERE KeyName = N'USE_CONFIG_DRIVEN_REPORTING' AND CompanyCode = N'LIMS')
                INSERT INTO Configurations (KeyName, GroupName, [Value], ValueType, [Description], CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'USE_CONFIG_DRIVEN_REPORTING', N'Report', N'false', N'boolean', N'Enable config-driven report auto-generation on test verification. When true, uses ReportFormat designer; when false, uses legacy reporting system.', 0, GETUTCDATE(), N'LIMS', 1);
        ");
    }

    // ───────────────────────────────────────────────
    // 5b. DEFAULT CURRENCIES
    // ───────────────────────────────────────────────
    private static async Task SeedCurrenciesAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT 1 FROM CurrencyMasters WHERE Code = N'INR' AND IsActive = 1)
                INSERT INTO CurrencyMasters (Name, Code, Symbol, IsDefault, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Indian Rupee', N'INR', N'₹', 1, 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM CurrencyMasters WHERE Code = N'USD' AND IsActive = 1)
                INSERT INTO CurrencyMasters (Name, Code, Symbol, IsDefault, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'US Dollar', N'USD', N'$', 0, 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM CurrencyMasters WHERE Code = N'EUR' AND IsActive = 1)
                INSERT INTO CurrencyMasters (Name, Code, Symbol, IsDefault, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Euro', N'EUR', N'€', 0, 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM CurrencyMasters WHERE Code = N'GBP' AND IsActive = 1)
                INSERT INTO CurrencyMasters (Name, Code, Symbol, IsDefault, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'British Pound', N'GBP', N'£', 0, 0, GETUTCDATE(), N'LIMS', 1);

            -- Ensure INR is marked as default if it already exists but IsDefault is 0
            UPDATE CurrencyMasters SET IsDefault = 1 WHERE Code = N'INR' AND IsActive = 1 AND IsDefault = 0;
            -- Ensure no other currency is marked as default
            UPDATE CurrencyMasters SET IsDefault = 0 WHERE Code != N'INR' AND IsActive = 1 AND IsDefault = 1;
        ");
    }

    // ───────────────────────────────────────────────
    // 7. DISPATCH MODES
    // ───────────────────────────────────────────────
    private static async Task SeedDispatchModesAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT 1 FROM DispatchModeMasters WHERE Name = N'Email')
                INSERT INTO DispatchModeMasters (Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Email', N'Report sent via email', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM DispatchModeMasters WHERE Name = N'WhatsApp')
                INSERT INTO DispatchModeMasters (Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'WhatsApp', N'Report sent via WhatsApp', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM DispatchModeMasters WHERE Name = N'Courier')
                INSERT INTO DispatchModeMasters (Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Courier', N'Report dispatched via courier service', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM DispatchModeMasters WHERE Name = N'Self Pickup')
                INSERT INTO DispatchModeMasters (Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Self Pickup', N'Collected by customer in person', 0, GETUTCDATE(), N'LIMS', 1);
        ");
    }

    // ───────────────────────────────────────────────
    // 7b. ORGANIZATIONS & BRANCHES
    // ───────────────────────────────────────────────
    private static async Task SeedOrganizationAndBranchesAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT 1 FROM Organizations WHERE IsActive = 1)
            BEGIN
                INSERT INTO Organizations (LabName, LabCode, LabAddress, ContactEmail, ContactPhone, CIN, Website, MobileNo, UlrPrefix, LabLocationCode, IsMultiBranch, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Devine Laboratory', N'DMSPL-0000000', N'Plot No 1, Industrial Area, Phase 1', N'info@lims.com', N'0000000000', N'U12345MH2020PTC123456', N'https://lims.com', N'0000000000', N'TC-1234', N'L1', 1, 0, GETUTCDATE(), N'LIMS', 1);
            END

            DECLARE @orgId BIGINT = (SELECT TOP 1 Id FROM Organizations WHERE IsActive = 1 ORDER BY Id);

            IF NOT EXISTS (SELECT 1 FROM Branches WHERE IsActive = 1)
            BEGIN
                INSERT INTO Branches (OrganizationID, Name, Code, IsHeadOffice, Address, ContactEmail, ContactPhone, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (@orgId, N'Devine Laboratory (Head Office)', N'DMSPL-0000000', 1, N'Plot No 1, Industrial Area, Phase 1', N'info@lims.com', N'0000000000', 0, GETUTCDATE(), N'LIMS', 1);
            END
            ELSE
            BEGIN
                -- Link any orphaned active branch to the primary active organization
                UPDATE Branches SET OrganizationID = @orgId WHERE (OrganizationID IS NULL OR OrganizationID = 0) AND IsActive = 1;
            END
        ");
    }

    // ───────────────────────────────────────────────
    // 8. ADMIN USER  (Department → Designation → Employee → User)
    //    Password: Admin@123 (ForcePasswordChange = true)
    // ───────────────────────────────────────────────
    private static async Task SeedAdminUserAsync(LIMSContext db, ILogger logger)
    {
        var passwordHasher = new PasswordHasher<UserMaster>();
        var defaultOrg = await db.Organizations.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.IsActive);
        var defaultBranch = await db.Branches.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.IsActive);
        long orgId = defaultOrg?.Id ?? defaultBranch?.OrganizationID ?? 0;
        long branchId = defaultBranch?.ID ?? 0;

        var adminRole = await db.RoleMasters.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Name == "Admin" && r.IsActive);
        if (adminRole == null) return;

        var dept = await db.DepartmentMasters.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Name == "Administration" && d.IsActive);
        if (dept == null)
        {
            dept = new DepartmentMaster { Name = "Administration", Description = "Administration Department", BranchID = branchId > 0 ? branchId : 0 };
            db.DepartmentMasters.Add(dept);
            await db.SaveChangesAsync();
        }

        var desigAdmin = await db.DesignationMasters.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Name == "System Administrator" && d.IsActive);
        if (desigAdmin == null)
        {
            desigAdmin = new DesignationMaster { Name = "System Administrator", Description = "Full system access", RoleID = adminRole.ID };
            db.DesignationMasters.Add(desigAdmin);
            await db.SaveChangesAsync();
        }

        var desigSuper = await db.DesignationMasters.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Name == "Super Administrator" && d.IsActive);
        if (desigSuper == null)
        {
            desigSuper = new DesignationMaster { Name = "Super Administrator", Description = "Hidden full system access", RoleID = adminRole.ID };
            db.DesignationMasters.Add(desigSuper);
            await db.SaveChangesAsync();
        }

        // ------------------ ADMIN USER ------------------
        var adminUser = await db.UserMasters.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.UserName == "admin");
        if (adminUser == null)
        {
            var emp = await db.EmployeeMasters.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Name == "System Admin" && e.IsActive);
            if (emp == null)
            {
                emp = new EmployeeMaster
                {
                    Name = "System Admin", EmailId = "admin@lims.com", Gender = "Male", DesignationID = desigAdmin.ID, DepartmentID = dept.ID, BranchID = branchId > 0 ? branchId : null,
                    DateOfJoin = DateTime.UtcNow, DateOfBirth = new DateTime(1990, 1, 1), RoleID = adminRole.ID, MobileNo = "0000000000",
                    ResidentialPinCode = "000000", ResidentialAreaID = 0, PermanentPinCode = "000000", PermanentAreaID = 0
                };
                db.EmployeeMasters.Add(emp);
                await db.SaveChangesAsync();
            }

            adminUser = new UserMaster
            {
                UserName = "admin", EmailId = "admin@lims.com", Password = passwordHasher.HashPassword(null!, "Admin@123"),
                RoleID = adminRole.ID, RoleName = "Admin", IsAdmin = true, EmployeeID = emp.ID,
                OrganizationID = orgId > 0 ? orgId : null,
                BranchID = branchId > 0 ? branchId : null,
                CanViewAllBranches = true,
                IsLoginEnabled = true, AccountStatus = "Active", ForcePasswordChange = false
            };
            db.UserMasters.Add(adminUser);
            await db.SaveChangesAsync();
            logger.LogInformation("DataSeeder: Admin user created (admin / Admin@123).");
        }
        else
        {
            bool updated = false;
            if ((adminUser.OrganizationID == null || adminUser.OrganizationID == 0) && orgId > 0)
            {
                adminUser.OrganizationID = orgId;
                updated = true;
            }
            if ((adminUser.BranchID == null || adminUser.BranchID == 0) && branchId > 0)
            {
                adminUser.BranchID = branchId;
                updated = true;
            }
            if (!adminUser.CanViewAllBranches)
            {
                adminUser.CanViewAllBranches = true;
                updated = true;
            }
            if (!adminUser.IsAdmin)
            {
                adminUser.IsAdmin = true;
                updated = true;
            }
            if (!adminUser.IsLoginEnabled)
            {
                adminUser.IsLoginEnabled = true;
                updated = true;
            }
            if (string.IsNullOrEmpty(adminUser.RoleName) || adminUser.RoleName != "Admin")
            {
                adminUser.RoleName = "Admin";
                adminUser.RoleID = adminRole.ID;
                updated = true;
            }
            if (updated)
            {
                db.UserMasters.Update(adminUser);
                await db.SaveChangesAsync();
                logger.LogInformation("DataSeeder: Admin user tenant context synchronized.");
            }
        }

        if (branchId > 0 && adminUser != null)
        {
            var userBranch = await db.UserBranches.IgnoreQueryFilters().FirstOrDefaultAsync(ub => ub.UserID == adminUser.ID && ub.BranchID == branchId);
            if (userBranch == null)
            {
                db.UserBranches.Add(new UserBranch
                {
                    UserID = adminUser.ID,
                    BranchID = branchId,
                    IsDefault = true,
                    CanView = true,
                    CanCreate = true,
                    CanEdit = true,
                    CanExecute = true,
                    CanApprove = true,
                    CanDelete = true,
                    IsActive = true
                });
                await db.SaveChangesAsync();
            }
            else if (!userBranch.IsDefault || !userBranch.CanView || !userBranch.CanApprove)
            {
                userBranch.IsDefault = true;
                userBranch.CanView = true;
                userBranch.CanCreate = true;
                userBranch.CanEdit = true;
                userBranch.CanExecute = true;
                userBranch.CanApprove = true;
                userBranch.CanDelete = true;
                userBranch.IsActive = true;
                db.UserBranches.Update(userBranch);
                await db.SaveChangesAsync();
            }
        }

        // ------------------ SUPER ADMIN USER ------------------
        var superAdminUser = await db.UserMasters.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.UserName == "superadmin");
        if (superAdminUser == null)
        {
            var emp = await db.EmployeeMasters.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Name == "Super Admin" && e.IsActive);
            if (emp == null)
            {
                emp = new EmployeeMaster
                {
                    Name = "Super Admin", EmailId = "superadmin@lims.com", Gender = "Male", DesignationID = desigSuper.ID, DepartmentID = dept?.ID ?? 0, BranchID = branchId > 0 ? branchId : null,
                    DateOfJoin = DateTime.UtcNow, DateOfBirth = new DateTime(1990, 1, 1), RoleID = adminRole.ID, MobileNo = "0000000000",
                    ResidentialPinCode = "000000", ResidentialAreaID = 0, PermanentPinCode = "000000", PermanentAreaID = 0,
                    IsSystemAdmin = true
                };
                db.EmployeeMasters.Add(emp);
                await db.SaveChangesAsync();
            }
            else if (!emp.IsSystemAdmin)
            {
                emp.IsSystemAdmin = true;
                if (emp.BranchID == null || emp.BranchID == 0) emp.BranchID = branchId > 0 ? branchId : null;
                db.EmployeeMasters.Update(emp);
                await db.SaveChangesAsync();
            }

            superAdminUser = new UserMaster
            {
                UserName = "superadmin", EmailId = "superadmin@lims.com", Password = passwordHasher.HashPassword(null!, "SuperAdmin@123"),
                RoleID = adminRole.ID, RoleName = "Admin", IsAdmin = true, EmployeeID = emp.ID,
                OrganizationID = orgId > 0 ? orgId : null,
                BranchID = branchId > 0 ? branchId : null,
                CanViewAllBranches = true,
                IsLoginEnabled = true, AccountStatus = "Active", ForcePasswordChange = false
            };
            db.UserMasters.Add(superAdminUser);
            await db.SaveChangesAsync();
            logger.LogInformation("DataSeeder: Super Admin user created (superadmin / SuperAdmin@123).");
        }
        else
        {
            bool updated = false;
            if ((superAdminUser.OrganizationID == null || superAdminUser.OrganizationID == 0) && orgId > 0)
            {
                superAdminUser.OrganizationID = orgId;
                updated = true;
            }
            if ((superAdminUser.BranchID == null || superAdminUser.BranchID == 0) && branchId > 0)
            {
                superAdminUser.BranchID = branchId;
                updated = true;
            }
            if (!superAdminUser.CanViewAllBranches)
            {
                superAdminUser.CanViewAllBranches = true;
                updated = true;
            }
            if (!superAdminUser.IsAdmin)
            {
                superAdminUser.IsAdmin = true;
                updated = true;
            }
            if (!superAdminUser.IsLoginEnabled)
            {
                superAdminUser.IsLoginEnabled = true;
                updated = true;
            }
            if (string.IsNullOrEmpty(superAdminUser.RoleName) || superAdminUser.RoleName != "Admin")
            {
                superAdminUser.RoleName = "Admin";
                superAdminUser.RoleID = adminRole.ID;
                updated = true;
            }
            if (updated)
            {
                db.UserMasters.Update(superAdminUser);
                await db.SaveChangesAsync();
                logger.LogInformation("DataSeeder: Super Admin user tenant context synchronized.");
            }
        }

        if (branchId > 0 && superAdminUser != null)
        {
            var userBranch = await db.UserBranches.IgnoreQueryFilters().FirstOrDefaultAsync(ub => ub.UserID == superAdminUser.ID && ub.BranchID == branchId);
            if (userBranch == null)
            {
                db.UserBranches.Add(new UserBranch
                {
                    UserID = superAdminUser.ID,
                    BranchID = branchId,
                    IsDefault = true,
                    CanView = true,
                    CanCreate = true,
                    CanEdit = true,
                    CanExecute = true,
                    CanApprove = true,
                    CanDelete = true,
                    IsActive = true
                });
                await db.SaveChangesAsync();
            }
            else if (!userBranch.IsDefault || !userBranch.CanView || !userBranch.CanApprove)
            {
                userBranch.IsDefault = true;
                userBranch.CanView = true;
                userBranch.CanCreate = true;
                userBranch.CanEdit = true;
                userBranch.CanExecute = true;
                userBranch.CanApprove = true;
                userBranch.CanDelete = true;
                userBranch.IsActive = true;
                db.UserBranches.Update(userBranch);
                await db.SaveChangesAsync();
            }
        }
    }

    // ───────────────────────────────────────────────
    // 9. MASTER DATA (ProductForm, SpecimenType, SpecimenOrientation, HeatTreatment, ProductCondition)
    // ───────────────────────────────────────────────
    private static async Task SeedMasterDataAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            -- Product Form Master (Rod, Sheet, Bar, Pipe, etc.)
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Sheet' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Sheet', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Plate' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Plate', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Bar' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Bar', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Rod' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Rod', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Pipe' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Pipe', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Tube' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Tube', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Wire' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Wire', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Strip' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Strip', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Forging' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Forging', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Casting' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Casting', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Coil' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Coil', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Flat' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Flat', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Section' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Section', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Angle' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Angle', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Channel' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Channel', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Beam' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Beam', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Fastener' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Fastener', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductFormMasters WHERE Name = N'Weld Joint' AND IsActive = 1)
                INSERT INTO ProductFormMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Weld Joint', 0, GETUTCDATE(), N'LIMS', 1);

            -- Specimen Type Master (Round, Flat, Sub-size, etc.)
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Round' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Round', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Flat' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Flat', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Sub-Size' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Sub-Size', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Full Section' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Full Section', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Tubular' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Tubular', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Charpy V-Notch' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Charpy V-Notch', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Charpy U-Notch' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Charpy U-Notch', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Izod' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Izod', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Weld Specimen' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Weld Specimen', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenTypeMasters WHERE Name = N'Machined' AND IsActive = 1)
                INSERT INTO SpecimenTypeMasters (Name, NormalCharge, HardCharge, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Machined', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);

            -- Specimen Orientation Master
            IF NOT EXISTS (SELECT 1 FROM SpecimenOrientationMasters WHERE Name = N'Longitudinal' AND IsActive = 1)
                INSERT INTO SpecimenOrientationMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Longitudinal', N'L', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenOrientationMasters WHERE Name = N'Transverse' AND IsActive = 1)
                INSERT INTO SpecimenOrientationMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Transverse', N'T', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenOrientationMasters WHERE Name = N'Through Thickness' AND IsActive = 1)
                INSERT INTO SpecimenOrientationMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Through Thickness', N'Z', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM SpecimenOrientationMasters WHERE Name = N'Diagonal' AND IsActive = 1)
                INSERT INTO SpecimenOrientationMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Diagonal', N'D', 0, GETUTCDATE(), N'LIMS', 1);

            -- Heat Treatment Master
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'As Rolled' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'As Rolled', N'AR', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Normalized' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Normalized', N'N', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Quenched & Tempered' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Quenched & Tempered', N'QT', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Annealed' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Annealed', N'A', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Solution Annealed' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Solution Annealed', N'SA', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Stress Relieved' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Stress Relieved', N'SR', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Hardened' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Hardened', N'H', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Tempered' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Tempered', N'T', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Case Hardened' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Case Hardened', N'CH', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Carburized' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Carburized', N'CB', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'Nitrided' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Nitrided', N'NI', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM HeatTreatmentMasters WHERE Name = N'PWHT' AND IsActive = 1)
                INSERT INTO HeatTreatmentMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'PWHT', N'PWHT', 0, GETUTCDATE(), N'LIMS', 1);

            -- Product Condition Master
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'Hot Rolled' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Hot Rolled', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'Cold Rolled' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Cold Rolled', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'Cold Drawn' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Cold Drawn', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'Hot Forged' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Hot Forged', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'As Cast' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'As Cast', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'Machined' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Machined', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'As Welded' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'As Welded', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'PWHT Treated' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'PWHT Treated', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'Galvanized' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Galvanized', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM ProductConditionMasters WHERE Name = N'Pickled' AND IsActive = 1)
                INSERT INTO ProductConditionMasters (Name, CalibrationRequired, IsDestructive, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Pickled', 0, 0, 0, GETUTCDATE(), N'LIMS', 1);

            -- Analysis Technique Master (chemical testing techniques)
            IF NOT EXISTS (SELECT 1 FROM AnalysisTechniqueMasters WHERE Code = N'OES' AND IsActive = 1)
                INSERT INTO AnalysisTechniqueMasters (Code, Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'OES', N'OES (Optical Emission Spectrometry)', N'Optical Emission Spectrometry', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM AnalysisTechniqueMasters WHERE Code = N'ICP' AND IsActive = 1)
                INSERT INTO AnalysisTechniqueMasters (Code, Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'ICP', N'ICP (Inductively Coupled Plasma)', N'Inductively Coupled Plasma', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM AnalysisTechniqueMasters WHERE Code = N'WET' AND IsActive = 1)
                INSERT INTO AnalysisTechniqueMasters (Code, Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'WET', N'Wet Chemical Analysis', N'Wet Chemical Analysis', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM AnalysisTechniqueMasters WHERE Code = N'LECO' AND IsActive = 1)
                INSERT INTO AnalysisTechniqueMasters (Code, Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'LECO', N'LECO (Combustion Analysis)', N'LECO Combustion Analysis', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM AnalysisTechniqueMasters WHERE Code = N'WDXRF' AND IsActive = 1)
                INSERT INTO AnalysisTechniqueMasters (Code, Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'WDXRF', N'WDXRF (Wavelength Dispersive XRF)', N'Wavelength Dispersive X-Ray Fluorescence', 0, GETUTCDATE(), N'LIMS', 1);
            IF NOT EXISTS (SELECT 1 FROM AnalysisTechniqueMasters WHERE Code = N'EDXRF' AND IsActive = 1)
                INSERT INTO AnalysisTechniqueMasters (Code, Name, Description, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'EDXRF', N'EDXRF (Energy Dispersive XRF)', N'Energy Dispersive X-Ray Fluorescence', 0, GETUTCDATE(), N'LIMS', 1);

            -- Chemical Sample Category
            IF OBJECT_ID(N'ChemicalSampleCategories', N'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM ChemicalSampleCategories WHERE Name = N'Ferro Alloys' AND IsActive = 1)
                    INSERT INTO ChemicalSampleCategories (Name, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Ferro Alloys', 1, 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM ChemicalSampleCategories WHERE Name = N'Pharma' AND IsActive = 1)
                    INSERT INTO ChemicalSampleCategories (Name, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Pharma', 2, 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM ChemicalSampleCategories WHERE Name = N'Industrial' AND IsActive = 1)
                    INSERT INTO ChemicalSampleCategories (Name, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Industrial', 3, 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM ChemicalSampleCategories WHERE Name = N'ROHS' AND IsActive = 1)
                    INSERT INTO ChemicalSampleCategories (Name, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'ROHS', 4, 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM ChemicalSampleCategories WHERE Name = N'Special Chemicals' AND IsActive = 1)
                    INSERT INTO ChemicalSampleCategories (Name, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'Special Chemicals', 5, 0, GETUTCDATE(), N'LIMS', 1);
            END

            -- Normalize Parameter Types (Mechanical -> Reported, Observation -> Observed)
            IF OBJECT_ID(N'ParameterMasters', N'U') IS NOT NULL
            BEGIN
                UPDATE ParameterMasters SET ParameterType = N'Observed' WHERE ParameterType = N'Observation';
                UPDATE ParameterMasters SET ParameterType = N'Reported' WHERE ParameterType = N'Mechanical';
            END

            -- Standard Organization Masters (BIS, ASTM, ISO, ASME, DIN, EN)
            IF OBJECT_ID(N'StandardOrganizationMasters', N'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM StandardOrganizationMasters WHERE Name LIKE '%BIS%' OR Name LIKE '%Indian%' AND IsActive = 1)
                    INSERT INTO StandardOrganizationMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'BIS', 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM StandardOrganizationMasters WHERE Name LIKE '%ASTM%' AND IsActive = 1)
                    INSERT INTO StandardOrganizationMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'ASTM', 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM StandardOrganizationMasters WHERE Name = N'ISO' AND IsActive = 1)
                    INSERT INTO StandardOrganizationMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'ISO', 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM StandardOrganizationMasters WHERE Name = N'ASME' AND IsActive = 1)
                    INSERT INTO StandardOrganizationMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'ASME', 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM StandardOrganizationMasters WHERE Name = N'DIN' AND IsActive = 1)
                    INSERT INTO StandardOrganizationMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'DIN', 0, GETUTCDATE(), N'LIMS', 1);
                IF NOT EXISTS (SELECT 1 FROM StandardOrganizationMasters WHERE Name = N'EN' AND IsActive = 1)
                    INSERT INTO StandardOrganizationMasters (Name, CreatedBy, CreatedOn, CompanyCode, IsActive) VALUES (N'EN', 0, GETUTCDATE(), N'LIMS', 1);
            END

            -- Reference Specification Headers (IS 1786, ASTM A240)
            IF OBJECT_ID(N'SpecificationHeaders', N'U') IS NOT NULL
            BEGIN
                DECLARE @bisId BIGINT = (SELECT TOP 1 ID FROM StandardOrganizationMasters WHERE (Name LIKE '%BIS%' OR Name LIKE '%Indian%') AND IsActive = 1);
                DECLARE @astmId BIGINT = (SELECT TOP 1 ID FROM StandardOrganizationMasters WHERE Name LIKE '%ASTM%' AND IsActive = 1);

                IF NOT EXISTS (SELECT 1 FROM SpecificationHeaders WHERE SpecificationNo = N'IS_1786' OR AliasName = N'IS 1786')
                BEGIN
                    INSERT INTO SpecificationHeaders (SpecificationNo, AliasName, DisplayTitle, StandardOrganizationID, Description, IsActive, CreatedBy, CreatedOn, CompanyCode, IsCustom)
                    VALUES (N'IS_1786', N'IS 1786', N'High strength deformed steel bars and wires for concrete reinforcement', @bisId, N'Standard specification for high strength deformed bars (TMT rebars) for concrete reinforcement', 1, 0, GETUTCDATE(), N'LIMS', 0);
                END

                IF NOT EXISTS (SELECT 1 FROM SpecificationHeaders WHERE SpecificationNo = N'ASTM_A240' OR AliasName = N'ASTM A240')
                BEGIN
                    INSERT INTO SpecificationHeaders (SpecificationNo, AliasName, DisplayTitle, StandardOrganizationID, Description, IsActive, CreatedBy, CreatedOn, CompanyCode, IsCustom)
                    VALUES (N'ASTM_A240', N'ASTM A240', N'Standard Specification for Chromium and Chromium-Nickel Stainless Steel', @astmId, N'Standard specification for chromium and chromium-nickel stainless steel plate, sheet, and strip for pressure vessels and general applications', 1, 0, GETUTCDATE(), N'LIMS', 0);
                END
            END

            -- Specification Grades Seeding for Reference Standards (IS 1786 & ASTM A240)
            IF OBJECT_ID(N'SpecificationGrades', N'U') IS NOT NULL AND OBJECT_ID(N'SpecificationHeaders', N'U') IS NOT NULL
            BEGIN
                -- 1. IS 1786 Grades (Fe 415, Fe 500, Fe 500D, Fe 550, Fe 550D, Fe 600)
                DECLARE @is1786Id BIGINT = (SELECT TOP 1 ID FROM SpecificationHeaders WHERE (SpecificationNo LIKE '%1786%' OR AliasName LIKE '%1786%' OR DisplayTitle LIKE '%1786%') AND IsActive = 1);
                IF @is1786Id IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @is1786Id AND Grade = N'Fe 415')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@is1786Id, N'Fe 415', N'Standard Reinforcement Grade', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @is1786Id AND Grade = N'Fe 500')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@is1786Id, N'Fe 500', N'High Strength Grade', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @is1786Id AND Grade = N'Fe 500D')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@is1786Id, N'Fe 500D', N'High Ductility Earthquake Resistant Grade', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @is1786Id AND Grade = N'Fe 550')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@is1786Id, N'Fe 550', N'Extra High Strength Grade', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @is1786Id AND Grade = N'Fe 550D')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@is1786Id, N'Fe 550D', N'Extra High Strength High Ductility Grade', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @is1786Id AND Grade = N'Fe 600')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@is1786Id, N'Fe 600', N'Ultra High Strength Grade', 1, 0, GETUTCDATE());
                END

                -- 2. ASTM A240 Grades (304, 304L, 316, 316L, 321, 310S)
                DECLARE @astmA240Id BIGINT = (SELECT TOP 1 ID FROM SpecificationHeaders WHERE (SpecificationNo LIKE '%A240%' OR AliasName LIKE '%A240%' OR DisplayTitle LIKE '%A240%') AND IsActive = 1);
                IF @astmA240Id IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @astmA240Id AND Grade = N'304')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@astmA240Id, N'304', N'Standard Austenitic Stainless Steel', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @astmA240Id AND Grade = N'304L')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@astmA240Id, N'304L', N'Low Carbon Austenitic Stainless Steel', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @astmA240Id AND Grade = N'316')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@astmA240Id, N'316', N'Molybdenum-bearing Austenitic Stainless Steel', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @astmA240Id AND Grade = N'316L')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@astmA240Id, N'316L', N'Low Carbon Molybdenum Austenitic Stainless Steel', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @astmA240Id AND Grade = N'321')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@astmA240Id, N'321', N'Titanium Stabilized Austenitic Stainless Steel', 1, 0, GETUTCDATE());
                    IF NOT EXISTS (SELECT 1 FROM SpecificationGrades WHERE SpecificationHeaderID = @astmA240Id AND Grade = N'310S')
                        INSERT INTO SpecificationGrades (SpecificationHeaderID, Grade, Remarks, IsActive, CreatedBy, CreatedOn) VALUES (@astmA240Id, N'310S', N'High Temperature Heat Resistant Stainless Steel', 1, 0, GETUTCDATE());
                END
            END
        ");
    }

    // ───────────────────────────────────────────────
    // 10. FINANCIAL YEARS (current + previous FY)
    // ───────────────────────────────────────────────
    private static async Task SeedFinancialYearsAsync(LIMSContext db)
    {
        // Determine current financial year (Apr-Mar cycle)
        var now = DateTime.UtcNow;
        var fyStartYear = now.Month >= 4 ? now.Year : now.Year - 1;

        // Seed from 2020 to current+1 FY
        for (int startYr = 2020; startYr <= fyStartYear + 1; startYr++)
        {
            var fy = $"{startYr}-{startYr + 1}";
            var isCurrent = startYr == fyStartYear ? 1 : 0;

            await db.Database.ExecuteSqlRawAsync($@"
                IF NOT EXISTS (SELECT 1 FROM FinancialYears WHERE Year = N'{fy}')
                    INSERT INTO FinancialYears (Year, StartDate, EndDate, IsCurrent, OrganizationId, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES (N'{fy}', '{startYr}-04-01', '{startYr + 1}-03-31', {isCurrent}, 0, 0, GETUTCDATE(), N'LIMS', 1);
            ");
        }
    }

    // ───────────────────────────────────────────────
    // 11. BACKFILL FinancialYearId on billing tables
    // ───────────────────────────────────────────────
    private static async Task BackfillFinancialYearIdsAsync(LIMSContext db, ILogger logger)
    {
        // Backfill InvoiceCases — match by created date to FY date range
        var backfilled = await db.Database.ExecuteSqlRawAsync(@"
            UPDATE ic SET ic.FinancialYearId = fy.Id
            FROM InvoiceCases ic
            CROSS APPLY (
                SELECT TOP 1 Id FROM FinancialYears
                WHERE ic.CreatedOn >= StartDate AND ic.CreatedOn <= EndDate
                ORDER BY StartDate DESC
            ) fy
            WHERE ic.FinancialYearId IS NULL;
        ");
        if (backfilled > 0)
            logger.LogInformation("DataSeeder: backfilled FinancialYearId on {Count} InvoiceCases", backfilled);

        // Backfill TaxInvoices
        backfilled = await db.Database.ExecuteSqlRawAsync(@"
            UPDATE t SET t.FinancialYearId = fy.Id
            FROM TaxInvoices t
            CROSS APPLY (
                SELECT TOP 1 Id FROM FinancialYears
                WHERE t.InvoiceDate >= StartDate AND t.InvoiceDate <= EndDate
                ORDER BY StartDate DESC
            ) fy
            WHERE t.FinancialYearId IS NULL;
        ");
        if (backfilled > 0)
            logger.LogInformation("DataSeeder: backfilled FinancialYearId on {Count} TaxInvoices", backfilled);

        // Backfill ProformaInvoiceHeaders
        backfilled = await db.Database.ExecuteSqlRawAsync(@"
            UPDATE p SET p.FinancialYearId = fy.Id
            FROM ProformaInvoiceHeader p
            CROSS APPLY (
                SELECT TOP 1 Id FROM FinancialYears
                WHERE p.PIDate >= StartDate AND p.PIDate <= EndDate
                ORDER BY StartDate DESC
            ) fy
            WHERE p.FinancialYearId IS NULL;
        ");
        if (backfilled > 0)
            logger.LogInformation("DataSeeder: backfilled FinancialYearId on {Count} ProformaInvoiceHeaders", backfilled);

        // Backfill CreditNotes
        backfilled = await db.Database.ExecuteSqlRawAsync(@"
            UPDATE c SET c.FinancialYearId = fy.Id
            FROM CreditNotes c
            CROSS APPLY (
                SELECT TOP 1 Id FROM FinancialYears
                WHERE c.CreditNoteDate >= StartDate AND c.CreditNoteDate <= EndDate
                ORDER BY StartDate DESC
            ) fy
            WHERE c.FinancialYearId IS NULL;
        ");
        if (backfilled > 0)
            logger.LogInformation("DataSeeder: backfilled FinancialYearId on {Count} CreditNotes", backfilled);

        // Backfill DebitNotes
        backfilled = await db.Database.ExecuteSqlRawAsync(@"
            UPDATE d SET d.FinancialYearId = fy.Id
            FROM DebitNotes d
            CROSS APPLY (
                SELECT TOP 1 Id FROM FinancialYears
                WHERE d.DebitNoteDate >= StartDate AND d.DebitNoteDate <= EndDate
                ORDER BY StartDate DESC
            ) fy
            WHERE d.FinancialYearId IS NULL;
        ");
        if (backfilled > 0)
            logger.LogInformation("DataSeeder: backfilled FinancialYearId on {Count} DebitNotes", backfilled);
    }

    // ───────────────────────────────────────────────────────
    // ROLE-PERMISSION DEFAULTS
    // Idempotent — inserts only missing (Role, Permission) pairs.
    // Admin role is NOT populated (bypasses via RequirePermissionAttribute).
    //
    // Role → permission map (DB names must match Permissions.cs catalog):
    //
    //   Accounts    → all Account.* + read Inward/Plan/Report/Customer
    //   FrontDesk   → Inward CRUD, Plan read, Customer CRUD, Courier read
    //   Technical   → Plan CRUD, Review, SamplePrep, Inward read, most masters read
    //   Lab         → Testing CRUD, Equipment read, Parameter read, Inward read
    //   LabManager  → nearly all (except admin/user/role management)
    // ───────────────────────────────────────────────────────
    private static async Task SeedRolePermissionDefaultsAsync(LIMSContext db, ILogger logger)
    {
        // Map: Role name → list of permission names
        var roleMap = new Dictionary<string, string[]>
        {
            ["Accounts"] = new[]
            {
                // Top-level account
                "CanReadAccount", "CanManageAccount",
                "CanReadAccountsDashboard", "CanReadCaseAccounts",

                // Invoice generation + line items
                "CanGeneratePI", "CanGenerateInvoice", "CanManageInvoice",
                "INVOICE_GENERATE",
                "CanReadInvoiceLineItem", "CanManageInvoiceLineItem",

                // Pricing
                "CanCalculatePricing", "CanValidatePricing",
                "TEST_PRICE_OVERRIDE",

                // Case closure
                "CanCloseCase",

                // Ledger / receipts / payments
                "CanReadCustomerLedger", "CanRecordPayment", "CanReadReceipt",
                "CanProcessPayment", "CanValidatePayment", "CanSendPaymentLink",

                // Reports
                "CanReadAgingReport", "CanReadOutstandingReport",
                "CanReadCollectionSummary", "CanReadCreditStatus",

                // Customer PO — full CRUD (Accounts manages PO)
                "CanReadCustomerPO", "CanCreateCustomerPO",
                "CanUpdateCustomerPO", "CanDeleteCustomerPO", "CanManageCustomerPO",

                // Invoice Case + Config — full CRUD
                "CanReadInvoiceCase", "CanCreateInvoiceCase",
                "CanUpdateInvoiceCase", "CanDeleteInvoiceCase", "CanManageInvoiceCase",
                "CanReadInvoiceCaseConfig", "CanCreateInvoiceCaseConfig",
                "CanUpdateInvoiceCaseConfig", "CanDeleteInvoiceCaseConfig", "CanManageInvoiceCaseConfig",

                // Cutting price (for quoting)
                "CanReadCuttingPrice", "CanCreateCuttingPrice",
                "CanUpdateCuttingPrice", "CanDeleteCuttingPrice", "CanManageCuttingPrice",

                // Machining charge (for quoting)
                "CanReadMachiningCharge", "CanCreateMachiningCharge",
                "CanUpdateMachiningCharge", "CanDeleteMachiningCharge", "CanManageMachiningCharge",

                // Read flow stages for context (can see but not act)
                "CanReadSampleInward", "CanReadPlan", "CanReadReview",
                "CanReadReporting", "CanReadCustomerMaster",

                // Masters used in invoice cases / PO
                "CanReadTax", "CanReadBank",
            },

            ["FrontDesk"] = new[]
            {
                // Sample inward full CRUD
                "CanReadSampleInward", "CanCreateSampleInward",
                "CanUpdateSampleInward", "CanDeleteSampleInward",
                "CanManageSampleInward",
                // Customer CRUD
                "CanReadCustomerMaster", "CanCreateCustomerMaster",
                "CanUpdateCustomerMaster", "CanManageCustomerMaster",
                // Read plan for status visibility
                "CanReadPlan", "CanReadReview",
                // Masters typically needed at inward
                "CanReadCourier", "CanReadTPI", "CanReadCompanyCategory",
                "CanReadMaterialSpecification", "CanReadProductMaster",
                "CanReadMetalClassification", "CanReadHeatTreatment",
                "CanReadProductCondition", "CanReadSpecimenOrientation",
                "CanReadProductForm", "CanReadLaboratoryTest",
                "CanReadProductSizeMaster", "CanReadAnalysisTechnique",
            },

            ["Technical"] = new[]
            {
                // Plan management
                "CanReadPlan", "CanManagePlan", "CanApprovePlan", "CanRejectPlan",
                "CanReadReview", "CanApproveReview", "CanRejectReview", "CanManageReview",
                "CanViewConfigurationAdjustment", "CanCreateConfigurationAdjustment", "CanApplyConfigurationAdjustment", "CanApproveConfigurationAdjustment",
                // Sample prep
                "CanReadSampleCutting", "CanManageSampleCutting",
                // Inward read (plan upstream)
                "CanReadSampleInward",
                // Testing read
                "CanReadTesting", "CanReadTestingDashboard", "CanReadTestResults",
                // Reporting read
                "CanReadReporting",
                // Most masters read
                "CanReadMaterialSpecification", "CanReadProductMaster",
                "CanReadLaboratoryTest", "CanReadTestMethodSpecification",
                "CanReadChemicalParameter", "CanReadMechanicalParameter",
                "CanReadParameterUnit", "CanReadMetalClassification",
                "CanReadHeatTreatment", "CanReadProductCondition",
                "CanReadSpecimenOrientation", "CanReadProductForm",
                "CanReadDimensionalFactors", "CanReadStandardOrganization",
                "CanReadEquipment", "CanReadCalibrationAgency",
                "CanReadProductSizeMaster", "CanReadAnalysisTechnique",
            },

            ["Lab"] = new[]
            {
                // Testing CRUD
                "CanReadTesting", "CanManageTesting", "CanPerformTest",
                "CanReadTestingDashboard", "CanReadTestResults",
                "CanReadPerformTest", "CanReadLongTermTracking",
                "TEST_RESULT_SAVE",
                // Configuration Adjustment (Analyst draft & submit - cannot approve)
                "CanViewConfigurationAdjustment", "CanCreateConfigurationAdjustment", "CanApplyConfigurationAdjustment",
                // Read upstream
                "CanReadSampleInward", "CanReadPlan", "CanReadSampleCutting",
                // Masters
                "CanReadLaboratoryTest", "CanReadTestMethodSpecification",
                "CanReadChemicalParameter", "CanReadMechanicalParameter",
                "CanReadParameterUnit", "CanReadEquipment",
                "CanReadDimensionalFactors",
                // Reporting read (for traceability)
                "CanReadReporting",
            },

            ["LabManager"] = new[]
            {
                // Flow — full manage
                "CanReadSampleInward", "CanCreateSampleInward",
                "CanUpdateSampleInward", "CanDeleteSampleInward", "CanManageSampleInward",
                "CanReadPlan", "CanCreatePlan", "CanUpdatePlan", "CanDeletePlan",
                "CanManagePlan", "CanApprovePlan", "CanRejectPlan",
                "CanReadReview", "CanApproveReview", "CanRejectReview", "CanManageReview",
                "CanReadSampleCutting", "CanCreateSampleCutting", "CanUpdateSampleCutting",
                "CanManageSampleCutting",
                "CanReadTesting", "CanManageTesting", "CanPerformTest",
                "CanReadTestingDashboard", "CanReadTestResults", "CanReadPerformTest",
                "CanReadLongTermTracking",
                "TEST_RESULT_SAVE", "TEST_RESULT_VERIFY", "TEST_PRICE_OVERRIDE",
                "CanReadReporting", "CanManageReporting",
                "CanApproveReport", "CanAmendReport",
                "CanReadReportFormat", "CanManageReportFormat",

                // Lab Scope — full CRUD
                "CanReadLabScopeMaster", "CanCreateLabScopeMaster",
                "CanUpdateLabScopeMaster", "CanDeleteLabScopeMaster",
                "CanManageLabScopeMaster",

                // Equipment Requirement — full CRUD (operational configuration for planning)
                "CanReadEquipmentRequirement", "CanCreateEquipmentRequirement",
                "CanUpdateEquipmentRequirement", "CanDeleteEquipmentRequirement",
                "CanManageEquipmentRequirement",

                // Factor / Conversion — full CRUD (operational configuration for planning)
                "CanReadFactorConversion", "CanCreateFactorConversion",
                "CanUpdateFactorConversion", "CanDeleteFactorConversion",
                "CanManageFactorConversion",

                // Measurement Uncertainty — full CRUD (operational configuration for planning)
                "CanReadMeasurementUncertainty", "CanCreateMeasurementUncertainty",
                "CanUpdateMeasurementUncertainty", "CanDeleteMeasurementUncertainty",
                "CanManageMeasurementUncertainty",

                // Execution Layout — full CRUD (Phase 1G operational configuration for execution presentation)
                "CanReadExecutionLayout", "CanCreateExecutionLayout",
                "CanUpdateExecutionLayout", "CanDeleteExecutionLayout",
                "CanManageExecutionLayout",

                // Phase 7/8 — Universal Result / Review (LabManager full cycle)
                "CanReadUniversalResult", "CanEvaluateUniversalResult",
                "CanFinalizeUniversalResult", "CanReworkUniversalResult",
                "CanReadUniversalReview", "CanReviewUniversalResult",
                "CanVerifyUniversalResult", "CanApproveUniversalResult",

                // Phase 9 — Universal Report (LabManager full cycle)
                "CanReadUniversalReport", "CanGenerateUniversalReport",
                "CanReleaseUniversalReport", "CanReissueUniversalReport",

                // Phase 1 masters — full CRUD (Test Definition / Method & Scientific / Spec & Compliance)
                "CanReadDiscipline", "CanCreateDiscipline",
                "CanUpdateDiscipline", "CanDeleteDiscipline",
                "CanManageDiscipline",
                "CanReadTestGroup", "CanCreateTestGroup",
                "CanUpdateTestGroup", "CanDeleteTestGroup",
                "CanManageTestGroup",
                "CanViewConfigurationAdjustment", "CanCreateConfigurationAdjustment",
                "CanApplyConfigurationAdjustment", "CanApproveConfigurationAdjustment",
                "CanCreateParameterUnit", "CanUpdateParameterUnit",
                "CanDeleteParameterUnit", "CanManageParameterUnit",
                "CanReadClassification", "CanCreateClassification",
                "CanUpdateClassification", "CanDeleteClassification",
                "CanManageClassification",
                "CanReadAcceptanceCriteria", "CanCreateAcceptanceCriteria",
                "CanUpdateAcceptanceCriteria", "CanDeleteAcceptanceCriteria",
                "CanManageAcceptanceCriteria",
                "CanReadConditionMaster", "CanCreateConditionMaster",
                "CanUpdateConditionMaster", "CanDeleteConditionMaster",
                "CanManageConditionMaster",

                // Masters — FULL CRUD (LabManager can add/edit/delete any master)
                "CanReadCustomerMaster", "CanCreateCustomerMaster",
                "CanUpdateCustomerMaster", "CanDeleteCustomerMaster", "CanManageCustomerMaster",
                "CanReadEmployee", "CanCreateEmployee", "CanUpdateEmployee", "CanDeleteEmployee", "CanManageEmployee",
                "CanReadDepartment", "CanCreateDepartment", "CanUpdateDepartment", "CanDeleteDepartment", "CanManageDepartment",
                "CanReadDesignation", "CanCreateDesignation", "CanUpdateDesignation", "CanDeleteDesignation", "CanManageDesignation",
                "CanReadEquipment", "CanCreateEquipment", "CanUpdateEquipment", "CanDeleteEquipment", "CanManageEquipment",
                "CanReadEquipmentType", "CanCreateEquipmentType", "CanUpdateEquipmentType", "CanDeleteEquipmentType", "CanManageEquipmentType",
                "CanReadCalibrationAgency", "CanCreateCalibrationAgency", "CanUpdateCalibrationAgency", "CanDeleteCalibrationAgency", "CanManageCalibrationAgency",
                "CanReadMaterialSpecification", "CanCreateMaterialSpecification", "CanUpdateMaterialSpecification", "CanDeleteMaterialSpecification", "CanManageMaterialSpecification",
                "CanReadProductMaster", "CanCreateProductMaster", "CanUpdateProductMaster", "CanDeleteProductMaster", "CanManageProductMaster",
                "CanReadLaboratoryTest", "CanCreateLaboratoryTest", "CanUpdateLaboratoryTest", "CanDeleteLaboratoryTest", "CanManageLaboratoryTest",
                "CanReadTestMethodSpecification", "CanCreateTestMethodSpecification", "CanUpdateTestMethodSpecification", "CanDeleteTestMethodSpecification", "CanManageTestMethodSpecification", "CanImportTestMethodSpecification",
                "CanReadChemicalParameter", "CanReadMechanicalParameter", "CanCreateParameter", "CanUpdateParameter", "CanDeleteParameter", "CanManageParameter",
                "CanReadParameterUnit",
                "CanReadMetalClassification", "CanCreateMetalClassification", "CanUpdateMetalClassification", "CanDeleteMetalClassification", "CanManageMetalClassification",
                "CanReadHeatTreatment", "CanReadProductCondition", "CanReadSpecimenOrientation",
                "CanReadProductForm", "CanReadDimensionalFactors", "CanReadStandardOrganization",
                "CanReadCompanyCategory", "CanCreateCompanyCategory", "CanUpdateCompanyCategory", "CanDeleteCompanyCategory", "CanManageCompanyCategory",
                "CanReadTax", "CanCreateTax", "CanUpdateTax", "CanDeleteTax", "CanManageTax",
                "CanReadBank", "CanCreateBank", "CanUpdateBank", "CanDeleteBank", "CanManageBank",
                "CanReadCourier", "CanCreateCourier", "CanUpdateCourier", "CanDeleteCourier", "CanManageCourier",
                "CanReadTPI", "CanCreateTPI", "CanUpdateTPI", "CanDeleteTPI", "CanManageTPI",
                "CanReadSupplier", "CanCreateSupplier", "CanUpdateSupplier", "CanDeleteSupplier", "CanManageSupplier",
                "CanReadOEM", "CanCreateOEM", "CanUpdateOEM", "CanDeleteOEM", "CanManageOEM",
                "CanReadUniversalCode",

                // Invoice Case / Config — read only (Accounts manages)
                "CanReadInvoiceCase", "CanReadInvoiceCaseConfig",
                "CanReadCustomerPO", "CanReadCuttingPrice", "CanReadMachiningCharge",

                // Account visibility (read, no manage)
                "CanReadAccount", "CanReadAccountsDashboard", "CanReadCaseAccounts",
                "CanReadCustomerLedger", "CanReadAgingReport", "CanReadOutstandingReport",

                // Workflow view
                "CanReadWorkflow",
            },
        };

        int inserted = 0;
        foreach (var (roleName, permissionNames) in roleMap)
        {
            var permList = string.Join(",", permissionNames.Select(p => $"N'{p}'"));
            var sql = $@"
                DECLARE @roleId BIGINT = (SELECT TOP 1 ID FROM RoleMasters WHERE Name = N'{roleName}' AND IsActive = 1);
                IF @roleId IS NULL
                BEGIN
                    PRINT 'Role {roleName} not found — skipping';
                    RETURN;
                END

                INSERT INTO RolePermissions (RoleID, PermissionID, IsGranted, CreatedBy, CreatedOn, CompanyCode, IsActive)
                SELECT @roleId, p.ID, 1, 0, GETUTCDATE(), N'LIMS', 1
                FROM PermissionMasters p
                WHERE p.Name IN ({permList})
                  AND NOT EXISTS (
                      SELECT 1 FROM RolePermissions rp
                      WHERE rp.RoleID = @roleId AND rp.PermissionID = p.ID AND rp.IsActive = 1
                  );
            ";
            var count = await db.Database.ExecuteSqlRawAsync(sql);
            if (count > 0)
            {
                inserted += count;
                logger.LogInformation("DataSeeder: seeded {Count} role permissions for {Role}", count, roleName);
            }
        }

        if (inserted > 0)
            logger.LogInformation("DataSeeder: inserted {Total} role-permission defaults total", inserted);
    }

    // ─── NON-ADMIN ROLE MENU MAPPINGS: managed via UI (Menu Permission screen) ───
    // Non-admin role→menu assignments are configured by Admin after first deployment.
    // Only Admin role is seeded automatically (SeedRoleMenuMappingsAsync above).

    // ───────────────────────────────────────────────
    // PRICE DIMENSION TYPES
    // ───────────────────────────────────────────────
    private static async Task SeedPriceDimensionTypesAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            -- Macro: inserts a PriceDimensionType row if none with same Name exists (active or inactive)
            DECLARE @c INT;

            -- FlatRate: fixed price, no dimension auto-detect
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'FlatRate';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'FlatRate', NULL, 0, N'UserInput', NULL, N'Fixed price — no dimension value needed', 1, 0, GETUTCDATE(), N'LIMS', 1);

            -- Element: count of billable parameters × unit rate
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'Element';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Element', NULL, 0, N'ParameterCount', NULL, N'Price per element — count of billable parameters', 2, 0, GETUTCDATE(), N'LIMS', 1);

            -- Hours: configurable per config (ParameterLinked | TestDuration | UserInputAtEntry)
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'Hours';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Hours', N'hr', 0, N'ParameterLinked', NULL, N'Based on hours — mode configurable per config', 3, 0, GETUTCDATE(), N'LIMS', 1);

            -- HoursRange: range slab variant of Hours
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'HoursRange';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'HoursRange', N'hr', 1, N'ParameterLinked', NULL, N'Based on hours range slab — mode configurable per config', 4, 0, GETUTCDATE(), N'LIMS', 1);

            -- Size: reads Diameter from SampleDetail
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'Size';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Size', N'mm', 0, N'SampleDimension', N'Diameter', N'Based on sample diameter', 5, 0, GETUTCDATE(), N'LIMS', 1);

            -- SizeRange: range slab on Diameter
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'SizeRange';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'SizeRange', N'mm', 1, N'SampleDimension', N'Diameter', N'Based on sample diameter range slab', 6, 0, GETUTCDATE(), N'LIMS', 1);

            -- Load: auto-detect from linked param; fallback to user input handled via FallbackToUserInput on config
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'Load';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Load', N'kN', 0, N'ParameterLinked', NULL, N'Based on load — auto-detect or user input if not found', 7, 0, GETUTCDATE(), N'LIMS', 1);

            -- LoadRange: range slab on load
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'LoadRange';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'LoadRange', N'kN', 1, N'ParameterLinked', NULL, N'Based on load range slab — auto-detect or user input if not found', 8, 0, GETUTCDATE(), N'LIMS', 1);

            -- Temperature: linked temperature parameter
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'Temperature';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Temperature', N'°C', 0, N'ParameterLinked', NULL, N'Based on temperature from linked parameter', 9, 0, GETUTCDATE(), N'LIMS', 1);

            -- TemperatureRange: range slab on temperature
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'TemperatureRange';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'TemperatureRange', N'°C', 1, N'ParameterLinked', NULL, N'Based on temperature range slab', 10, 0, GETUTCDATE(), N'LIMS', 1);

            -- DayWise: linked days parameter
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'DayWise';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'DayWise', N'days', 0, N'ParameterLinked', NULL, N'Based on number of days from linked parameter', 11, 0, GETUTCDATE(), N'LIMS', 1);

            -- SizeLoad: two-dimensional size + load
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'SizeLoad';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'SizeLoad', N'mm/kN', 0, N'SampleDimension', N'Diameter', N'Two-dimensional: size (from sample) + load (from param)', 12, 0, GETUTCDATE(), N'LIMS', 1);

            -- SizeAndLoad: two-dimensional range variant
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'SizeAndLoad';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'SizeAndLoad', N'mm/kN', 0, N'SampleDimension', N'Diameter', N'Two-dimensional size+load range variant', 13, 0, GETUTCDATE(), N'LIMS', 1);

            -- PerIndent: always ask quantity at test result entry
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'PerIndent';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'PerIndent', N'×', 0, N'UserInputAtEntry', NULL, N'Per indent — user enters count at test result entry', 14, 0, GETUTCDATE(), N'LIMS', 1);

            -- PerLocation: always ask quantity at test result entry
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'PerLocation';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'PerLocation', N'×', 0, N'UserInputAtEntry', NULL, N'Per location/point — user enters count at test result entry', 15, 0, GETUTCDATE(), N'LIMS', 1);

            -- PerField: always ask quantity at test result entry
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'PerField';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'PerField', N'×', 0, N'UserInputAtEntry', NULL, N'Per field — user enters count at test result entry', 16, 0, GETUTCDATE(), N'LIMS', 1);

            -- PerDolly: always ask quantity at test result entry (pull-off test specific)
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'PerDolly';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'PerDolly', N'×', 0, N'UserInputAtEntry', NULL, N'Per dolly — user enters count at test result entry (pull-off test)', 17, 0, GETUTCDATE(), N'LIMS', 1);

            -- SpectroCombination: engine checks which param IDs are in billable params
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'SpectroCombination';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'SpectroCombination', NULL, 0, N'ParameterLinked', NULL, N'Spectro element combination pricing — two-step base+extra-tier engine', 18, 0, GETUTCDATE(), N'LIMS', 1);

            -- WithImage: user confirms image was captured (Yes/No) at test result entry
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'WithImage';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'WithImage', NULL, 0, N'UserInputAtEntry', NULL, N'Additional charge when image captured — user confirms Yes/No at entry', 19, 0, GETUTCDATE(), N'LIMS', 1);

            -- WithExtenso: user confirms extensometer was used (Yes/No) at test result entry
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'WithExtenso';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'WithExtenso', NULL, 0, N'UserInputAtEntry', NULL, N'Additional charge when extensometer used — user confirms Yes/No at entry', 20, 0, GETUTCDATE(), N'LIMS', 1);

            -- Algorithm: price formula evaluated at runtime (P3)
            SELECT @c = COUNT(1) FROM PriceDimensionTypes WHERE Name = N'Algorithm';
            IF @c = 0 INSERT INTO PriceDimensionTypes
                (Name, Unit, IsRange, ValueSource, SampleField, Description, SortOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Algorithm', NULL, 0, N'UserInput', NULL, N'Custom price formula evaluated at runtime (P3 feature)', 21, 0, GETUTCDATE(), N'LIMS', 1);
        ");
    }

    private static async Task FixMachiningChargeMasterConstraintsAsync(LIMSContext db, ILogger logger)
    {
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID')
                BEGIN
                    ALTER TABLE [dbo].[MachiningChargeMasters] DROP CONSTRAINT [FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID];
                END

                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID')
                BEGIN
                    ALTER TABLE [dbo].[MachiningChargeMasters] DROP CONSTRAINT [FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID];
                END
            ");
            logger.LogInformation("DataSeeder: MachiningChargeMasters foreign key constraints updated to polymorphic references.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "DataSeeder: Exception checking/dropping MachiningChargeMasters constraints (safe to proceed).");
        }
    }

    private static async Task RepairHangfireSchemaAsync(LIMSContext db, ILogger logger)
    {
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'HangFire' AND TABLE_NAME = 'Job')
                   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'HangFire' AND TABLE_NAME = 'Schema')
                   AND NOT EXISTS (SELECT 1 FROM [HangFire].[Schema])
                BEGIN
                    INSERT INTO [HangFire].[Schema] ([Version]) VALUES (7);
                END
            ");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "DataSeeder: Hangfire schema check/repair warning (safe to proceed)");
        }
    }

    private static async Task AlignTenantCodesAsync(LIMSContext db, ILogger logger)
    {
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID(N'LabScopeMasters', N'U') IS NOT NULL
                    UPDATE LabScopeMasters SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'LabScopeSpecifications', N'U') IS NOT NULL
                    UPDATE LabScopeSpecifications SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'LabScopeSpecificationParameters', N'U') IS NOT NULL
                    UPDATE LabScopeSpecificationParameters SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'LaboratoryTests', N'U') IS NOT NULL
                    UPDATE LaboratoryTests SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'TestMethodSpecifications', N'U') IS NOT NULL
                    UPDATE TestMethodSpecifications SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'RoleMasters', N'U') IS NOT NULL
                    UPDATE RoleMasters SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'UserMasters', N'U') IS NOT NULL
                    UPDATE UserMasters SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'EmployeeMasters', N'U') IS NOT NULL
                    UPDATE EmployeeMasters SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'MenuMasters', N'U') IS NOT NULL
                    UPDATE MenuMasters SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;

                IF OBJECT_ID(N'PermissionMasters', N'U') IS NOT NULL
                    UPDATE PermissionMasters SET CompanyCode = N'LIMS' WHERE CompanyCode = N'LIMS01' OR CompanyCode IS NULL;
            ");
            logger.LogInformation("DataSeeder: tenant alignment completed.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "DataSeeder: tenant alignment warning (safe to proceed)");
        }
    }

    private static async Task EnsureDatabaseSchemaAsync(LIMSContext db, ILogger logger)
    {
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                -- 1. AcceptanceCriteriaMasters
                IF OBJECT_ID(N'dbo.AcceptanceCriteriaMasters', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[AcceptanceCriteriaMasters](
                        [ID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
                        [Code] [nvarchar](50) NOT NULL,
                        [Name] [nvarchar](150) NOT NULL,
                        [Description] [nvarchar](500) NULL,
                        [EvaluationType] [nvarchar](20) NOT NULL,
                        [ComparisonType] [nvarchar](30) NOT NULL,
                        [DecisionRule] [nvarchar](50) NOT NULL,
                        [RoundingRule] [nvarchar](30) NOT NULL,
                        [DisplayOrder] [int] NOT NULL CONSTRAINT [DF_AcceptanceCriteriaMasters_DisplayOrder] DEFAULT (0),
                        [CreatedBy] [bigint] NOT NULL CONSTRAINT [DF_AcceptanceCriteriaMasters_CreatedBy] DEFAULT (0),
                        [CreatedOn] [datetime2](7) NOT NULL CONSTRAINT [DF_AcceptanceCriteriaMasters_CreatedOn] DEFAULT (GETUTCDATE()),
                        [ModifiedBy] [bigint] NULL,
                        [ModifiedOn] [datetime2](7) NULL,
                        [CompanyCode] [nvarchar](450) NOT NULL CONSTRAINT [DF_AcceptanceCriteriaMasters_CompanyCode] DEFAULT (N'LIMS'),
                        [IsActive] [bit] NOT NULL CONSTRAINT [DF_AcceptanceCriteriaMasters_IsActive] DEFAULT (1)
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AcceptanceCriteriaMasters_CompanyCode_Code' AND object_id = OBJECT_ID(N'dbo.AcceptanceCriteriaMasters'))
                BEGIN
                    CREATE UNIQUE NONCLUSTERED INDEX [IX_AcceptanceCriteriaMasters_CompanyCode_Code]
                        ON [dbo].[AcceptanceCriteriaMasters]([CompanyCode] ASC, [Code] ASC)
                        WHERE ([IsActive] = 1 AND [Code] IS NOT NULL);
                END

                -- 2. EquipmentRequirementMasters
                IF OBJECT_ID(N'dbo.EquipmentRequirementMasters', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.EquipmentRequirementMasters
                    (
                        ID                              BIGINT          IDENTITY(1,1) NOT NULL CONSTRAINT PK_EquipmentRequirementMasters PRIMARY KEY,
                        Code                            NVARCHAR(50)    NOT NULL,
                        Name                            NVARCHAR(150)   NOT NULL,
                        Description                     NVARCHAR(500)   NULL,
                        LaboratoryTestID                BIGINT          NOT NULL,
                        TestMethodSpecificationID       BIGINT          NULL,
                        TestMethodSpecificationVersionID BIGINT         NULL,
                        ParameterID                     BIGINT          NULL,
                        EquipmentTypeID                 BIGINT          NOT NULL,
                        EquipmentID                     BIGINT          NULL,
                        RequiredCapability              NVARCHAR(500)   NULL,
                        MinimumRange                    DECIMAL(18,4)   NULL,
                        MaximumRange                    DECIMAL(18,4)   NULL,
                        RangeUnitID                     BIGINT          NULL,
                        AccuracyRequirement             NVARCHAR(200)   NULL,
                        ResolutionRequirement           NVARCHAR(200)   NULL,
                        IsMandatory                     BIT             NOT NULL CONSTRAINT DF_EquipmentRequirementMasters_IsMandatory DEFAULT(1),
                        DisplayOrder                    INT             NOT NULL CONSTRAINT DF_EquipmentRequirementMasters_DisplayOrder DEFAULT(0),
                        CreatedBy                       BIGINT          NOT NULL CONSTRAINT DF_EquipmentRequirementMasters_CreatedBy DEFAULT(0),
                        CreatedOn                       DATETIME2       NOT NULL CONSTRAINT DF_EquipmentRequirementMasters_CreatedOn DEFAULT(GETUTCDATE()),
                        ModifiedBy                      BIGINT          NULL,
                        ModifiedOn                      DATETIME2       NULL,
                        CompanyCode                     NVARCHAR(50)    NOT NULL CONSTRAINT DF_EquipmentRequirementMasters_CompanyCode DEFAULT(N'LIMS'),
                        IsActive                        BIT             NOT NULL CONSTRAINT DF_EquipmentRequirementMasters_IsActive DEFAULT(1)
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EquipmentRequirementMasters_CompanyCode_Code' AND object_id = OBJECT_ID(N'dbo.EquipmentRequirementMasters'))
                BEGIN
                    CREATE UNIQUE INDEX IX_EquipmentRequirementMasters_CompanyCode_Code
                        ON dbo.EquipmentRequirementMasters (CompanyCode, Code)
                        WHERE [IsActive] = 1 AND [Code] IS NOT NULL;
                END

                -- 3. FactorConversionMasters
                IF OBJECT_ID(N'dbo.FactorConversionMasters', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.FactorConversionMasters
                    (
                        ID                              BIGINT          IDENTITY(1,1) NOT NULL CONSTRAINT PK_FactorConversionMasters PRIMARY KEY,
                        Code                            NVARCHAR(50)    NOT NULL,
                        Name                            NVARCHAR(150)   NOT NULL,
                        Description                     NVARCHAR(500)   NULL,
                        FactorType                      NVARCHAR(20)    NOT NULL CONSTRAINT DF_FactorConversionMasters_FactorType DEFAULT(N'MULTIPLICATION'),
                        FactorValue                     DECIMAL(18,6)   NOT NULL CONSTRAINT DF_FactorConversionMasters_FactorValue DEFAULT(1.0),
                        InputParameterID                BIGINT          NOT NULL,
                        OutputParameterID               BIGINT          NULL,
                        LaboratoryTestID                BIGINT          NULL,
                        TestMethodSpecificationID       BIGINT          NULL,
                        TestMethodSpecificationVersionID BIGINT         NULL,
                        AppliedOn                       NVARCHAR(100)   NOT NULL CONSTRAINT DF_FactorConversionMasters_AppliedOn DEFAULT(N'Measured Value'),
                        IsMandatory                     BIT             NOT NULL CONSTRAINT DF_FactorConversionMasters_IsMandatory DEFAULT(1),
                        DisplayOrder                    INT             NOT NULL CONSTRAINT DF_FactorConversionMasters_DisplayOrder DEFAULT(0),
                        CreatedBy                       BIGINT          NOT NULL CONSTRAINT DF_FactorConversionMasters_CreatedBy DEFAULT(0),
                        CreatedOn                       DATETIME2       NOT NULL CONSTRAINT DF_FactorConversionMasters_CreatedOn DEFAULT(GETUTCDATE()),
                        ModifiedBy                      BIGINT          NULL,
                        ModifiedOn                      DATETIME2       NULL,
                        CompanyCode                     NVARCHAR(50)    NOT NULL CONSTRAINT DF_FactorConversionMasters_CompanyCode DEFAULT(N'LIMS'),
                        IsActive                        BIT             NOT NULL CONSTRAINT DF_FactorConversionMasters_IsActive DEFAULT(1)
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FactorConversionMasters_CompanyCode_Code' AND object_id = OBJECT_ID(N'dbo.FactorConversionMasters'))
                BEGIN
                    CREATE UNIQUE INDEX IX_FactorConversionMasters_CompanyCode_Code
                        ON dbo.FactorConversionMasters (CompanyCode, Code)
                        WHERE [IsActive] = 1 AND [Code] IS NOT NULL;
                END

                -- 4. MeasurementUncertaintyMasters
                IF OBJECT_ID(N'dbo.MeasurementUncertaintyMasters', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[MeasurementUncertaintyMasters] (
                        [ID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
                        [Code] [nvarchar](50) NOT NULL,
                        [Name] [nvarchar](150) NOT NULL,
                        [Description] [nvarchar](500) NULL,
                        [LaboratoryTestID] [bigint] NULL,
                        [ParameterID] [bigint] NULL,
                        [TestMethodSpecificationID] [bigint] NULL,
                        [TestMethodSpecificationVersionID] [bigint] NULL,
                        [ParameterUnitID] [bigint] NULL,
                        [UncertaintyType] [nvarchar](20) NOT NULL CONSTRAINT [DF_MUMasters_UncertaintyType] DEFAULT (N'EXPANDED'),
                        [Basis] [nvarchar](20) NOT NULL CONSTRAINT [DF_MUMasters_Basis] DEFAULT (N'Type B'),
                        [CombinedUncertainty] [decimal](18,6) NULL,
                        [ExpandedUncertainty] [decimal](18,6) NULL,
                        [CoverageFactor] [decimal](18,4) NOT NULL CONSTRAINT [DF_MUMasters_CoverageFactor] DEFAULT ((2.0)),
                        [ConfidenceLevel] [decimal](5,2) NULL,
                        [ComponentsJson] [nvarchar](max) NULL,
                        [DisplayOrder] [int] NOT NULL CONSTRAINT [DF_MUMasters_DisplayOrder] DEFAULT ((0)),
                        [CreatedBy] [bigint] NOT NULL CONSTRAINT [DF_MUMasters_CreatedBy] DEFAULT ((0)),
                        [CreatedOn] [datetime2](7) NOT NULL CONSTRAINT [DF_MUMasters_CreatedOn] DEFAULT (GETUTCDATE()),
                        [ModifiedBy] [bigint] NULL,
                        [ModifiedOn] [datetime2](7) NULL,
                        [CompanyCode] [nvarchar](50) NOT NULL CONSTRAINT [DF_MUMasters_CompanyCode] DEFAULT (N'LIMS'),
                        [IsActive] [bit] NOT NULL CONSTRAINT [DF_MUMasters_IsActive] DEFAULT ((1))
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MeasurementUncertaintyMasters_CompanyCode_Code' AND object_id = OBJECT_ID(N'dbo.MeasurementUncertaintyMasters'))
                BEGIN
                    CREATE UNIQUE NONCLUSTERED INDEX [IX_MeasurementUncertaintyMasters_CompanyCode_Code]
                        ON [dbo].[MeasurementUncertaintyMasters] ([CompanyCode] ASC, [Code] ASC)
                        WHERE ([IsActive] = 1 AND [Code] IS NOT NULL);
                END

                -- 5. ExecutionLayoutMasters, ExecutionLayoutSections, ExecutionLayoutItems
                IF OBJECT_ID(N'dbo.ExecutionLayoutMasters', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.ExecutionLayoutMasters
                    (
                        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        Code NVARCHAR(50) NOT NULL,
                        Name NVARCHAR(150) NOT NULL,
                        Description NVARCHAR(500) NULL,
                        RendererType NVARCHAR(50) NOT NULL CONSTRAINT DF_ExecutionLayoutMasters_Renderer DEFAULT (N'Grid'),
                        DisplayOrder INT NOT NULL CONSTRAINT DF_ExecutionLayoutMasters_DisplayOrder DEFAULT (0),
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_ExecutionLayoutMasters_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_ExecutionLayoutMasters_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL,
                        ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_ExecutionLayoutMasters_Company DEFAULT (N'LIMS'),
                        IsActive BIT NOT NULL CONSTRAINT DF_ExecutionLayoutMasters_IsActive DEFAULT (1)
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ExecutionLayoutMasters_Code' AND object_id = OBJECT_ID(N'dbo.ExecutionLayoutMasters'))
                BEGIN
                    CREATE UNIQUE INDEX IX_ExecutionLayoutMasters_Code ON dbo.ExecutionLayoutMasters (CompanyCode, Code) WHERE IsActive = 1;
                END

                IF OBJECT_ID(N'dbo.ExecutionLayoutSections', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.ExecutionLayoutSections
                    (
                        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        ExecutionLayoutID BIGINT NOT NULL,
                        Title NVARCHAR(150) NOT NULL,
                        SectionKey NVARCHAR(50) NOT NULL,
                        SectionType NVARCHAR(50) NOT NULL CONSTRAINT DF_ExecutionLayoutSections_Type DEFAULT (N'Parameters'),
                        DisplayOrder INT NOT NULL CONSTRAINT DF_ExecutionLayoutSections_DisplayOrder DEFAULT (0),
                        IsCollapsible BIT NOT NULL CONSTRAINT DF_ExecutionLayoutSections_Collapsible DEFAULT (0),
                        IsCollapsedByDefault BIT NOT NULL CONSTRAINT DF_ExecutionLayoutSections_Collapsed DEFAULT (0),
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_ExecutionLayoutSections_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_ExecutionLayoutSections_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL,
                        ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_ExecutionLayoutSections_Company DEFAULT (N'LIMS'),
                        IsActive BIT NOT NULL CONSTRAINT DF_ExecutionLayoutSections_IsActive DEFAULT (1)
                    );
                END

                IF OBJECT_ID(N'dbo.ExecutionLayoutItems', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.ExecutionLayoutItems
                    (
                        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        ExecutionLayoutSectionID BIGINT NOT NULL,
                        ItemType NVARCHAR(50) NOT NULL,
                        ItemKey NVARCHAR(100) NOT NULL,
                        Label NVARCHAR(150) NULL,
                        ColumnSpan INT NOT NULL CONSTRAINT DF_ExecutionLayoutItems_ColSpan DEFAULT (1),
                        DisplayOrder INT NOT NULL CONSTRAINT DF_ExecutionLayoutItems_DisplayOrder DEFAULT (0),
                        IsRequired BIT NOT NULL CONSTRAINT DF_ExecutionLayoutItems_Required DEFAULT (0),
                        IsReadOnly BIT NOT NULL CONSTRAINT DF_ExecutionLayoutItems_ReadOnly DEFAULT (0),
                        ConfigurationJson NVARCHAR(MAX) NULL,
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_ExecutionLayoutItems_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_ExecutionLayoutItems_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL,
                        ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_ExecutionLayoutItems_Company DEFAULT (N'LIMS'),
                        IsActive BIT NOT NULL CONSTRAINT DF_ExecutionLayoutItems_IsActive DEFAULT (1)
                    );
                END

                -- 6. LaboratoryTestLayouts
                IF OBJECT_ID(N'dbo.LaboratoryTestLayouts', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.LaboratoryTestLayouts
                    (
                        ID BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LaboratoryTestLayouts PRIMARY KEY,
                        LaboratoryTestID BIGINT NOT NULL,
                        ExecutionLayoutID BIGINT NOT NULL,
                        TestMethodSpecificationID BIGINT NULL,
                        TestMethodSpecificationVersionID BIGINT NULL,
                        Priority INT NOT NULL CONSTRAINT DF_LaboratoryTestLayouts_Priority DEFAULT (0),
                        IsDefault BIT NOT NULL CONSTRAINT DF_LaboratoryTestLayouts_IsDefault DEFAULT (0),
                        CreatedBy BIGINT NULL,
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_LaboratoryTestLayouts_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL,
                        ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_LaboratoryTestLayouts_Company DEFAULT (N'LIMS'),
                        IsActive BIT NOT NULL CONSTRAINT DF_LaboratoryTestLayouts_IsActive DEFAULT (1)
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LaboratoryTestLayouts_Test_Layout_Method_Version' AND object_id = OBJECT_ID(N'dbo.LaboratoryTestLayouts'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_LaboratoryTestLayouts_Test_Layout_Method_Version
                        ON dbo.LaboratoryTestLayouts (LaboratoryTestID, ExecutionLayoutID, TestMethodSpecificationID, TestMethodSpecificationVersionID);
                END

                -- 7. ConfigurationAdjustments & ConfigurationAdjustmentItems
                IF OBJECT_ID(N'dbo.ConfigurationAdjustments', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.ConfigurationAdjustments
                    (
                        ID BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ConfigurationAdjustments PRIMARY KEY,
                        UniversalTestGroupID BIGINT NOT NULL,
                        AdjustmentNumber INT NOT NULL CONSTRAINT DF_ConfigurationAdjustments_AdjNo DEFAULT (1),
                        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_ConfigurationAdjustments_Status DEFAULT (N'Draft'),
                        OverallReason NVARCHAR(500) NOT NULL CONSTRAINT DF_ConfigurationAdjustments_Reason DEFAULT (N''),
                        AppliedBy BIGINT NULL,
                        AppliedOn DATETIME2 NULL,
                        ApprovedBy BIGINT NULL,
                        ApprovedOn DATETIME2 NULL,
                        ApprovalRemarks NVARCHAR(500) NULL,
                        BranchID BIGINT NOT NULL,
                        ConcurrencyToken NVARCHAR(64) NOT NULL CONSTRAINT DF_ConfigurationAdjustments_Token DEFAULT (NEWID()),
                        AdjustedConfigurationJson NVARCHAR(MAX) NULL,
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_ConfigurationAdjustments_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_ConfigurationAdjustments_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL,
                        ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_ConfigurationAdjustments_Company DEFAULT (N'LIMS'),
                        IsActive BIT NOT NULL CONSTRAINT DF_ConfigurationAdjustments_IsActive DEFAULT (1)
                    );
                END

                IF OBJECT_ID(N'dbo.ConfigurationAdjustmentItems', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.ConfigurationAdjustmentItems
                    (
                        ID BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ConfigurationAdjustmentItems PRIMARY KEY,
                        ConfigurationAdjustmentID BIGINT NOT NULL,
                        UniversalTestGroupID BIGINT NOT NULL,
                        Section NVARCHAR(50) NOT NULL,
                        EntityType NVARCHAR(50) NOT NULL,
                        EntityID BIGINT NULL,
                        EntityCode NVARCHAR(100) NULL,
                        EntityName NVARCHAR(250) NULL,
                        FieldName NVARCHAR(100) NOT NULL,
                        ChangeType NVARCHAR(50) NOT NULL,
                        PlannedValue NVARCHAR(MAX) NULL,
                        EffectiveValue NVARCHAR(MAX) NULL,
                        PreviousAdjustedValue NVARCHAR(MAX) NULL,
                        NewAdjustedValue NVARCHAR(MAX) NULL,
                        Reason NVARCHAR(500) NOT NULL CONSTRAINT DF_ConfigurationAdjustmentItems_Reason DEFAULT (N''),
                        AuthorizationStatus NVARCHAR(50) NOT NULL CONSTRAINT DF_ConfigurationAdjustmentItems_AuthStatus DEFAULT (N'Pending'),
                        AuthorizedBy BIGINT NULL,
                        AuthorizedOn DATETIME2 NULL,
                        BranchID BIGINT NOT NULL,
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_ConfigurationAdjustmentItems_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_ConfigurationAdjustmentItems_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL,
                        ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_ConfigurationAdjustmentItems_Company DEFAULT (N'LIMS'),
                        IsActive BIT NOT NULL CONSTRAINT DF_ConfigurationAdjustmentItems_IsActive DEFAULT (1)
                    );
                END

                -- 8. ExecutionConfigSnapshots
                IF OBJECT_ID(N'dbo.ExecutionConfigSnapshots', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.ExecutionConfigSnapshots
                    (
                        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        ConfigJson NVARCHAR(MAX) NOT NULL,
                        SnapshotHash NVARCHAR(256) NULL,
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_ExecutionConfigSnapshots_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_ExecutionConfigSnapshots_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL,
                        ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_ExecutionConfigSnapshots_Company DEFAULT (N'LIMS'),
                        IsActive BIT NOT NULL CONSTRAINT DF_ExecutionConfigSnapshots_IsActive DEFAULT (1)
                    );
                END

                -- 9. UniversalTestResults, UniversalTestResultParameters, UniversalReviewFindings, UniversalResultAudits
                IF OBJECT_ID(N'dbo.UniversalTestResults', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.UniversalTestResults (
                        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        TestExecutionID BIGINT NOT NULL,
                        UniversalTestGroupID BIGINT NOT NULL,
                        BranchID BIGINT NOT NULL,
                        OrganizationID BIGINT NOT NULL,
                        RevisionNo INT NOT NULL CONSTRAINT DF_UniversalTestResults_Rev DEFAULT 1,
                        ResultStatus NVARCHAR(30) NOT NULL CONSTRAINT DF_UniversalTestResults_Status DEFAULT 'Draft',
                        OverallDecision NVARCHAR(30) NOT NULL CONSTRAINT DF_UniversalTestResults_Decision DEFAULT 'NOT_EVALUATED',
                        SnapshotHash NVARCHAR(256) NULL,
                        ExecutionConfigSnapshotID BIGINT NULL,
                        DecisionRule NVARCHAR(50) NULL,
                        AcceptanceCriteriaCode NVARCHAR(50) NULL,
                        CalculationTraceJson NVARCHAR(MAX) NULL,
                        ComplianceSummaryJson NVARCHAR(MAX) NULL,
                        FinalizedBy BIGINT NULL, FinalizedOn DATETIME2 NULL,
                        ReviewerID BIGINT NULL, ReviewedOn DATETIME2 NULL,
                        VerifiedBy BIGINT NULL, VerifiedOn DATETIME2 NULL,
                        ApprovedBy BIGINT NULL, ApprovedOn DATETIME2 NULL,
                        ReviewRemarks NVARCHAR(1000) NULL,
                        ApprovalRemarks NVARCHAR(1000) NULL,
                        ConcurrencyToken NVARCHAR(64) NOT NULL CONSTRAINT DF_UniversalTestResults_Token DEFAULT (NEWID()),
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_UniversalTestResults_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_UniversalTestResults_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL, ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_UniversalTestResults_Company DEFAULT 'LIMS',
                        IsActive BIT NOT NULL CONSTRAINT DF_UniversalTestResults_IsActive DEFAULT 1
                    );
                END

                IF OBJECT_ID(N'dbo.UniversalTestResultParameters', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.UniversalTestResultParameters (
                        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        UniversalTestResultID BIGINT NOT NULL,
                        ParameterMasterID BIGINT NOT NULL,
                        ParameterCode NVARCHAR(100) NOT NULL,
                        ParameterName NVARCHAR(200) NULL,
                        InputType NVARCHAR(20) NOT NULL CONSTRAINT DF_UTRP_InputType DEFAULT 'Decimal',
                        Unit NVARCHAR(50) NULL,
                        DecimalPrecision INT NOT NULL CONSTRAINT DF_UTRP_DecPrec DEFAULT 2,
                        IsCalculated BIT NOT NULL CONSTRAINT DF_UTRP_IsCalc DEFAULT 0,
                        IsMandatory BIT NOT NULL CONSTRAINT DF_UTRP_IsMand DEFAULT 1,
                        IsReportable BIT NOT NULL CONSTRAINT DF_UTRP_IsRep DEFAULT 1,
                        DisplayOrder INT NOT NULL CONSTRAINT DF_UTRP_DispOrder DEFAULT 0,
                        RawValue NVARCHAR(255) NULL,
                        RawNumericValue DECIMAL(18,6) NULL,
                        AppliedFactorCode NVARCHAR(100) NULL,
                        AppliedFactorOperation NVARCHAR(100) NULL,
                        FactoredValue DECIMAL(18,6) NULL,
                        Formula NVARCHAR(MAX) NULL,
                        SubstitutionTrace NVARCHAR(MAX) NULL,
                        CalculatedValue DECIMAL(18,6) NULL,
                        ComplianceValue DECIMAL(18,6) NULL,
                        DisplayValue NVARCHAR(100) NULL,
                        ReportedValue NVARCHAR(100) NULL,
                        SpecMin DECIMAL(18,6) NULL, SpecMax DECIMAL(18,6) NULL,
                        SpecTarget DECIMAL(18,6) NULL,
                        MinTolerance DECIMAL(18,6) NULL, MaxTolerance DECIMAL(18,6) NULL,
                        RequirementStatus NVARCHAR(40) NOT NULL CONSTRAINT DF_UTRP_ReqStatus DEFAULT 'RESOLVED',
                        Verdict NVARCHAR(30) NOT NULL CONSTRAINT DF_UTRP_Verdict DEFAULT 'NOT_EVALUATED',
                        CombinedUncertainty DECIMAL(18,6) NULL,
                        ExpandedUncertainty DECIMAL(18,6) NULL,
                        CoverageFactor DECIMAL(10,4) NULL,
                        GuardBandApplied BIT NOT NULL CONSTRAINT DF_UTRP_GuardBand DEFAULT 0,
                        EvaluationNote NVARCHAR(500) NULL,
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_UTRP_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_UTRP_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL, ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_UTRP_Company DEFAULT 'LIMS',
                        IsActive BIT NOT NULL CONSTRAINT DF_UTRP_IsActive DEFAULT 1
                    );
                END

                IF OBJECT_ID(N'dbo.UniversalReviewFindings', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.UniversalReviewFindings (
                        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        UniversalTestResultID BIGINT NOT NULL,
                        TestExecutionID BIGINT NOT NULL,
                        FindingType NVARCHAR(50) NOT NULL CONSTRAINT DF_URF_Type DEFAULT 'Observation',
                        Description NVARCHAR(1000) NOT NULL,
                        Severity NVARCHAR(20) NOT NULL CONSTRAINT DF_URF_Severity DEFAULT 'Major',
                        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_URF_Status DEFAULT 'Open',
                        IsBlocking BIT NOT NULL CONSTRAINT DF_URF_Blocking DEFAULT 1,
                        Resolution NVARCHAR(1000) NULL,
                        ResolvedBy BIGINT NULL, ResolvedOn DATETIME2 NULL,
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_URF_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_URF_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL, ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_URF_Company DEFAULT 'LIMS',
                        IsActive BIT NOT NULL CONSTRAINT DF_URF_IsActive DEFAULT 1
                    );
                END

                IF OBJECT_ID(N'dbo.UniversalResultAudits', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.UniversalResultAudits (
                        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        UniversalTestResultID BIGINT NOT NULL,
                        TestExecutionID BIGINT NOT NULL,
                        Action NVARCHAR(50) NOT NULL,
                        ActionCategory NVARCHAR(50) NOT NULL CONSTRAINT DF_URA_Category DEFAULT 'WORKFLOW',
                        FromState NVARCHAR(50) NULL,
                        ToState NVARCHAR(50) NULL,
                        Remarks NVARCHAR(1000) NULL,
                        PayloadJson NVARCHAR(MAX) NULL,
                        PerformedBy BIGINT NOT NULL CONSTRAINT DF_URA_PerfBy DEFAULT (0),
                        PerformedOn DATETIME2 NOT NULL CONSTRAINT DF_URA_PerfOn DEFAULT (GETUTCDATE()),
                        BranchID BIGINT NOT NULL CONSTRAINT DF_URA_Branch DEFAULT (1),
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_URA_CreatedBy DEFAULT (0),
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_URA_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy BIGINT NULL, ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_URA_Company DEFAULT 'LIMS',
                        IsActive BIT NOT NULL CONSTRAINT DF_URA_IsActive DEFAULT 1
                    );
                END

                -- 10. UniversalReports
                IF OBJECT_ID(N'dbo.UniversalReports', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.UniversalReports
                    (
                        ID                          BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        TestExecutionID               BIGINT NOT NULL,
                        UniversalTestResultID         BIGINT NOT NULL,
                        ResultRevisionNo              INT NOT NULL CONSTRAINT DF_UniversalReports_ResultRev DEFAULT (1),
                        ReportRevisionNo              INT NOT NULL CONSTRAINT DF_UniversalReports_ReportRev DEFAULT (1),
                        ReportNo                      NVARCHAR(100) NOT NULL,
                        SnapshotHash                  NVARCHAR(256) NULL,
                        ResultRevisionHash            NVARCHAR(256) NULL,
                        ReportDataJson                NVARCHAR(MAX) NULL,
                        ReportDataHash                NVARCHAR(256) NULL,
                        PdfPath                       NVARCHAR(500) NULL,
                        PdfHash                       NVARCHAR(256) NULL,
                        Status                        NVARCHAR(30) NOT NULL CONSTRAINT DF_UniversalReports_Status DEFAULT (N'GENERATED'),
                        BranchID                      BIGINT NOT NULL,
                        OrganizationID                BIGINT NOT NULL,
                        GeneratedOn                   DATETIME2 NULL,
                        GeneratedBy                   BIGINT NULL,
                        ReleasedOn                    DATETIME2 NULL,
                        ReleasedBy                    BIGINT NULL,
                        CreatedBy                     BIGINT NOT NULL CONSTRAINT DF_UniversalReports_CreatedBy DEFAULT (0),
                        CreatedOn                     DATETIME2 NOT NULL CONSTRAINT DF_UniversalReports_CreatedOn DEFAULT (GETUTCDATE()),
                        ModifiedBy                    BIGINT NULL,
                        ModifiedOn                    DATETIME2 NULL,
                        CompanyCode                   NVARCHAR(50) NOT NULL CONSTRAINT DF_UniversalReports_Company DEFAULT (N'LIMS'),
                        IsActive                      BIT NOT NULL CONSTRAINT DF_UniversalReports_IsActive DEFAULT (1)
                    );
                END

                -- 11. SpecificationVersions & SpecificationVersionParameters
                IF OBJECT_ID(N'dbo.SpecificationVersions', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.SpecificationVersions (
                        ID BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SpecificationVersions PRIMARY KEY,
                        SpecificationHeaderID BIGINT NOT NULL,
                        Version NVARCHAR(50) NOT NULL,
                        Year NVARCHAR(20) NULL,
                        Status VARCHAR(20) NOT NULL CONSTRAINT DF_SpecificationVersions_Status DEFAULT 'Draft',
                        EffectiveDate DATETIME2 NULL,
                        SupersededDate DATETIME2 NULL,
                        ReviewDate DATETIME2 NULL,
                        ChangeReason NVARCHAR(500) NULL,
                        StandardFile NVARCHAR(255) NULL,
                        StandardFilePath NVARCHAR(500) NULL,
                        UploadReferenceID BIGINT NULL,
                        IsDefault BIT NOT NULL CONSTRAINT DF_SpecificationVersions_IsDefault DEFAULT 0,
                        CreatedBy BIGINT NOT NULL CONSTRAINT DF_SpecificationVersions_CreatedBy DEFAULT 0,
                        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_SpecificationVersions_CreatedOn DEFAULT GETUTCDATE(),
                        ModifiedBy BIGINT NULL,
                        ModifiedOn DATETIME2 NULL,
                        CompanyCode NVARCHAR(50) NOT NULL CONSTRAINT DF_SpecificationVersions_CompanyCode DEFAULT 'LIMS',
                        IsActive BIT NOT NULL CONSTRAINT DF_SpecificationVersions_IsActive DEFAULT 1
                    );
                END

                IF OBJECT_ID(N'dbo.SpecificationVersionParameters', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.SpecificationVersionParameters (
                        ID BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SpecificationVersionParameters PRIMARY KEY,
                        SpecificationVersionID BIGINT NOT NULL,
                        ParameterID BIGINT NOT NULL,
                        Comment NVARCHAR(1000) NULL,
                        SortOrder INT NOT NULL CONSTRAINT DF_SpecificationVersionParameters_SortOrder DEFAULT 1,
                        IsActive BIT NOT NULL CONSTRAINT DF_SpecificationVersionParameters_IsActive DEFAULT 1
                    );
                END

                -- 12. LaboratoryTest columns and LaboratoryTestParameters / LaboratoryTestMethods
                IF OBJECT_ID(N'dbo.LaboratoryTests', N'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LaboratoryTests' AND COLUMN_NAME = 'Code')
                        ALTER TABLE [dbo].[LaboratoryTests] ADD [Code] NVARCHAR(50) NULL;

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LaboratoryTests' AND COLUMN_NAME = 'Description')
                        ALTER TABLE [dbo].[LaboratoryTests] ADD [Description] NVARCHAR(500) NULL;
                END

                IF OBJECT_ID(N'dbo.LaboratoryTestParameters', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[LaboratoryTestParameters] (
                        [ID] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LaboratoryTestParameters] PRIMARY KEY,
                        [LaboratoryTestID] BIGINT NOT NULL,
                        [ParameterID] BIGINT NOT NULL,
                        [IsMandatory] BIT NOT NULL CONSTRAINT [DF_LaboratoryTestParameters_IsMandatory] DEFAULT 1,
                        [IsReportable] BIT NOT NULL CONSTRAINT [DF_LaboratoryTestParameters_IsReportable] DEFAULT 1,
                        [DisplayOrder] INT NOT NULL CONSTRAINT [DF_LaboratoryTestParameters_DisplayOrder] DEFAULT 0,
                        [CreatedBy] BIGINT NOT NULL CONSTRAINT [DF_LaboratoryTestParameters_CreatedBy] DEFAULT 0,
                        [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_LaboratoryTestParameters_CreatedOn] DEFAULT GETUTCDATE(),
                        [ModifiedBy] BIGINT NULL,
                        [ModifiedOn] DATETIME2 NULL,
                        [CompanyCode] NVARCHAR(50) NOT NULL CONSTRAINT [DF_LaboratoryTestParameters_CompanyCode] DEFAULT 'LIMS',
                        [IsActive] BIT NOT NULL CONSTRAINT [DF_LaboratoryTestParameters_IsActive] DEFAULT 1
                    );
                END

                IF OBJECT_ID(N'dbo.LaboratoryTestMethods', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[LaboratoryTestMethods] (
                        [ID] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LaboratoryTestMethods] PRIMARY KEY,
                        [LaboratoryTestID] BIGINT NOT NULL,
                        [TestMethodSpecificationID] BIGINT NOT NULL,
                        [IsDefault] BIT NOT NULL CONSTRAINT [DF_LaboratoryTestMethods_IsDefault] DEFAULT 0,
                        [DisplayOrder] INT NOT NULL CONSTRAINT [DF_LaboratoryTestMethods_DisplayOrder] DEFAULT 0,
                        [CreatedBy] BIGINT NOT NULL CONSTRAINT [DF_LaboratoryTestMethods_CreatedBy] DEFAULT 0,
                        [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_LaboratoryTestMethods_CreatedOn] DEFAULT GETUTCDATE(),
                        [ModifiedBy] BIGINT NULL,
                        [ModifiedOn] DATETIME2 NULL,
                        [CompanyCode] NVARCHAR(50) NOT NULL CONSTRAINT [DF_LaboratoryTestMethods_CompanyCode] DEFAULT 'LIMS',
                        [IsActive] BIT NOT NULL CONSTRAINT [DF_LaboratoryTestMethods_IsActive] DEFAULT 1
                    );
                END

                -- 13. UniversalTestGroups columns
                IF OBJECT_ID(N'dbo.UniversalTestGroups', N'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UniversalTestGroups' AND COLUMN_NAME = 'ExecutionLayoutID')
                        ALTER TABLE [dbo].[UniversalTestGroups] ADD [ExecutionLayoutID] BIGINT NULL;

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UniversalTestGroups' AND COLUMN_NAME = 'DepartmentID')
                        ALTER TABLE [dbo].[UniversalTestGroups] ADD [DepartmentID] BIGINT NULL;

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UniversalTestGroups' AND COLUMN_NAME = 'PlannedConfigurationJson')
                        ALTER TABLE [dbo].[UniversalTestGroups] ADD [PlannedConfigurationJson] NVARCHAR(MAX) NULL;

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UniversalTestGroups' AND COLUMN_NAME = 'ExecutionCount')
                        ALTER TABLE [dbo].[UniversalTestGroups] ADD [ExecutionCount] INT NOT NULL CONSTRAINT DF_UTG_ExecCount DEFAULT 0;

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UniversalTestGroups' AND COLUMN_NAME = 'TestMethodSpecificationVersionID')
                        ALTER TABLE [dbo].[UniversalTestGroups] ADD [TestMethodSpecificationVersionID] BIGINT NULL;

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UniversalTestGroups' AND COLUMN_NAME = 'SpecificationVersionID')
                        ALTER TABLE [dbo].[UniversalTestGroups] ADD [SpecificationVersionID] BIGINT NULL;
                END

                -- 14. TestExecutions columns
                IF OBJECT_ID(N'dbo.TestExecutions', N'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestExecutions' AND COLUMN_NAME = 'ExecutionConfigSnapshotID')
                        ALTER TABLE [dbo].[TestExecutions] ADD [ExecutionConfigSnapshotID] BIGINT NULL;

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestExecutions' AND COLUMN_NAME = 'ActualConditionsJson')
                        ALTER TABLE [dbo].[TestExecutions] ADD [ActualConditionsJson] NVARCHAR(MAX) NULL;

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestExecutions' AND COLUMN_NAME = 'ActualEquipmentJson')
                        ALTER TABLE [dbo].[TestExecutions] ADD [ActualEquipmentJson] NVARCHAR(MAX) NULL;
                END
            ");
            logger.LogInformation("DataSeeder: incremental database schema validated and ensured.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "DataSeeder: warning during EnsureDatabaseSchemaAsync (safe to proceed)");
        }
    }

    private static async Task SeedAcceptanceCriteriaAsync(LIMSContext db)
    {
        if (await db.Database.SqlQueryRaw<int>("SELECT CASE WHEN OBJECT_ID(N'AcceptanceCriteriaMasters', N'U') IS NOT NULL THEN 1 ELSE 0 END AS [Value]").FirstOrDefaultAsync() == 1)
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT 1 FROM AcceptanceCriteriaMasters WHERE Code = N'ALL_REQUIRED_PASS' AND CompanyCode = N'LIMS')
                BEGIN
                    INSERT INTO AcceptanceCriteriaMasters
                        (Code, Name, Description, EvaluationType, ComparisonType, DecisionRule, RoundingRule, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES
                        (N'ALL_REQUIRED_PASS', N'All Required Parameters Must Pass',
                         N'Default rule: every required parameter must be within its specification requirement (FormulaEvaluator Pass).',
                         N'TEST', N'RANGE', N'ALL_REQUIRED_PASS', N'ROUND_NEAREST', 1, 0, GETUTCDATE(), N'LIMS', 1);
                END

                IF NOT EXISTS (SELECT 1 FROM AcceptanceCriteriaMasters WHERE Code = N'WITH_MOU_GUARD' AND CompanyCode = N'LIMS')
                BEGIN
                    INSERT INTO AcceptanceCriteriaMasters
                        (Code, Name, Description, EvaluationType, ComparisonType, DecisionRule, RoundingRule, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES
                        (N'WITH_MOU_GUARD', N'Guard-Band Acceptance (With MOU Consideration)',
                         N'Guard-band path: acceptance band shrinks by ExpandedUncertainty when case decision rule requires MOU consideration.',
                         N'TEST', N'RANGE', N'WITH_MOU_GUARD', N'ROUND_NEAREST', 2, 0, GETUTCDATE(), N'LIMS', 1);
                END

                IF NOT EXISTS (SELECT 1 FROM AcceptanceCriteriaMasters WHERE Code = N'INFORMATIONAL' AND CompanyCode = N'LIMS')
                BEGIN
                    INSERT INTO AcceptanceCriteriaMasters
                        (Code, Name, Description, EvaluationType, ComparisonType, DecisionRule, RoundingRule, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES
                        (N'INFORMATIONAL', N'Informational (Excluded from Decision)',
                         N'Result is recorded but does not participate in the compliance decision.',
                         N'PARAMETER', N'RANGE', N'INFORMATIONAL', N'ROUND_NEAREST', 3, 0, GETUTCDATE(), N'LIMS', 1);
                END
            ");
        }
    }

    private static async Task SeedExecutionLayoutsAsync(LIMSContext db)
    {
        if (await db.Database.SqlQueryRaw<int>("SELECT CASE WHEN OBJECT_ID(N'ExecutionLayoutMasters', N'U') IS NOT NULL THEN 1 ELSE 0 END AS [Value]").FirstOrDefaultAsync() == 1)
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT 1 FROM ExecutionLayoutMasters WHERE Code = N'TENSILE_STD')
                    INSERT INTO ExecutionLayoutMasters (Code, Name, Description, RendererType, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES (N'TENSILE_STD', N'Tensile Standard Layout', N'Standard layout for tensile and mechanical testing', N'Grid', 1, 0, GETUTCDATE(), N'LIMS', 1);

                IF NOT EXISTS (SELECT 1 FROM ExecutionLayoutMasters WHERE Code = N'OES_CHEM')
                    INSERT INTO ExecutionLayoutMasters (Code, Name, Description, RendererType, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES (N'OES_CHEM', N'OES Chemical Layout', N'Optical Emission Spectrometry chemical analysis layout', N'Table', 2, 0, GETUTCDATE(), N'LIMS', 1);

                IF NOT EXISTS (SELECT 1 FROM ExecutionLayoutMasters WHERE Code = N'CBR_LAYOUT')
                    INSERT INTO ExecutionLayoutMasters (Code, Name, Description, RendererType, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES (N'CBR_LAYOUT', N'CBR Execution Layout', N'California Bearing Ratio soil mechanics testing layout', N'Custom', 3, 0, GETUTCDATE(), N'LIMS', 1);

                IF NOT EXISTS (SELECT 1 FROM ExecutionLayoutMasters WHERE Code = N'MAT_EXEC')
                    INSERT INTO ExecutionLayoutMasters (Code, Name, Description, RendererType, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES (N'MAT_EXEC', N'Material Test Execution Layout', N'Material testing comprehensive execution layout', N'Grid', 4, 0, GETUTCDATE(), N'LIMS', 1);

                IF NOT EXISTS (SELECT 1 FROM ExecutionLayoutMasters WHERE Code = N'GENERIC_STD')
                    INSERT INTO ExecutionLayoutMasters (Code, Name, Description, RendererType, DisplayOrder, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    VALUES (N'GENERIC_STD', N'Generic Standard Layout', N'Default generic test parameter layout', N'Grid', 5, 0, GETUTCDATE(), N'LIMS', 1);
            ");
        }
    }

    private static async Task SeedSpecificationAndMethodVersionsAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            -- Auto-create active default version for any SpecificationHeaders missing versions
            IF OBJECT_ID(N'SpecificationHeaders', N'U') IS NOT NULL AND OBJECT_ID(N'SpecificationVersions', N'U') IS NOT NULL
            BEGIN
                INSERT INTO SpecificationVersions (SpecificationHeaderID, Version, Year, Status, EffectiveDate, IsDefault, CreatedBy, CreatedOn, CompanyCode)
                SELECT sh.ID, COALESCE(NULLIF(sh.StandardYear, ''), '01'), COALESCE(NULLIF(sh.StandardYear, ''), CAST(YEAR(GETUTCDATE()) AS NVARCHAR(10))), 'Active', GETUTCDATE(), 1, 0, GETUTCDATE(), N'LIMS'
                FROM SpecificationHeaders sh
                WHERE sh.IsActive = 1
                  AND NOT EXISTS (
                      SELECT 1 FROM SpecificationVersions sv WHERE sv.SpecificationHeaderID = sh.ID
                  );
            END

            -- Auto-create active default version for any TestMethodSpecifications missing versions
            IF OBJECT_ID(N'TestMethodSpecifications', N'U') IS NOT NULL AND OBJECT_ID(N'TestMethodSpecificationVersions', N'U') IS NOT NULL
            BEGIN
                INSERT INTO TestMethodSpecificationVersions (TestMethodSpecificationID, Version, Year, Status, EffectiveDate, IsDefault, CreatedBy, CreatedOn, CompanyCode)
                SELECT tms.ID, '01', CAST(YEAR(GETUTCDATE()) AS NVARCHAR(10)), 'Active', GETUTCDATE(), 1, 0, GETUTCDATE(), N'LIMS'
                FROM TestMethodSpecifications tms
                WHERE (tms.IsDisabled = 0 OR tms.IsDisabled IS NULL)
                  AND NOT EXISTS (
                      SELECT 1 FROM TestMethodSpecificationVersions tmsv WHERE tmsv.TestMethodSpecificationID = tms.ID
                  );
            END
        ");
    }
}

