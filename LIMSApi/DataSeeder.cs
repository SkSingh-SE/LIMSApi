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
            // Quick check: if Admin role exists, first-time seeding was already done
            var alreadySeeded = await db.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM RoleMasters WHERE Name = N'Admin' AND IsActive = 1")
                .FirstAsync();

            if (alreadySeeded == 0)
            {
                logger.LogInformation("DataSeeder: first-time deployment detected — seeding core data...");

                await SeedRolesAsync(db);
                await SeedConfigurationsAsync(db);
                await SeedCurrenciesAsync(db);
                await SeedDispatchModesAsync(db);
                await SeedAdminUserAsync(db, logger);

                logger.LogInformation("DataSeeder: core seeding complete.");
            }
            else
            {
                logger.LogInformation("DataSeeder: core seed data already present — skipping core.");
            }

            // Always ensure admin user exists (idempotent — checks before creating)
            await SeedAdminUserAsync(db, logger);

            // Menus — always runs via usp_SeedMenus (title-based, NOT EXISTS duplicate guard)
            await SeedMenusAsync(db);

            // Role-menu mappings — always runs (Admin → all active menus, NOT EXISTS guard)
            await SeedRoleMenuMappingsAsync(db);

            // Permissions — always runs so new permissions are picked up on every deployment
            // Uses usp_SeedPermissions SP (title-based lookup, no hardcoded MenuIDs)
            await SeedPermissionsAsync(db);

            // Master data (ProductForm, SpecimenType, etc.) always runs — idempotent with IF NOT EXISTS checks
            await SeedMasterDataAsync(db);
            await SeedPriceDimensionTypesAsync(db);
            await SeedFinancialYearsAsync(db);
            await BackfillFinancialYearIdsAsync(db, logger);

            // Role-Permission defaults — idempotent, runs every startup to pick up new permissions
            await SeedRolePermissionDefaultsAsync(db, logger);

            logger.LogInformation("DataSeeder: master data seed complete.");
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
            (N'TPI Master',          NULL, N'/tpi',                NULL, N'Administration'),
            (N'Supplier Master',     NULL, N'/supplier',           NULL, N'Administration'),
            (N'Equipment',           NULL, N'/equipment',          NULL, N'Administration'),
            (N'OEM Master',          NULL, N'/oem',                NULL, N'Administration'),
            (N'Calibration Agency',  NULL, N'/calibration-agency', NULL, N'Administration'),
            -- ══ Level 1: Specification — folders only ══
            (N'Material Specification', NULL, NULL, NULL, N'Specification'),
            (N'Product Specification',  NULL, NULL, NULL, N'Specification'),
            -- ══ Level 1: Test ══
            (N'Laboratory Test',           NULL, N'/test',              NULL, N'Test'),
            (N'Test Method Specification', NULL, N'/test-specification', NULL, N'Test'),
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
            ('CanReadHeatTreatment','View Heat Treatment','Heat Treatment',NULL,'Read'),
            ('CanReadProductCondition','View Product Condition','Product Condition',NULL,'Read'),
            ('CanReadSpecimenOrientation','View Specimen Orientation','Specimen Orientation',NULL,'Read'),
            ('CanReadSpecimenType','View Specimen Type','Specimen Type',NULL,'Read'),
            ('CanReadProductForm','View Product Form','Product Form',NULL,'Read'),
            ('CanReadDimensionalFactors','View Dimensional Factors','Dimensional Factor',NULL,'Read'),
            ('CanReadUniversalCode','View Universal Code','Universal Code Type',NULL,'Read'),

            -- 'Product Specification' title exists on BOTH folder (202) and leaf (33).
            -- ParentTitle='Product Specification' picks the leaf (33).
            ('CanReadProductSpecification','View Product Specification','Product Specification','Product Specification','Read'),
            ('CanCreateProductSpecification','Create Product Spec','Product Specification','Product Specification','Create'),
            ('CanUpdateProductSpecification','Update Product Spec','Product Specification','Product Specification','Update'),
            ('CanDeleteProductSpecification','Delete Product Spec','Product Specification','Product Specification','Delete'),
            ('CanManageProductSpecification','Manage Product Spec','Product Specification','Product Specification','Manage'),

            ('CanReadCustomProductSpecification','View Custom Product Specification','Custom Product Specification',NULL,'Read'),

            -- ═══════════════════════ Test ═══════════════════════
            ('CanReadLaboratoryTest','View Laboratory Test','Laboratory Test',NULL,'Read'),
            ('CanCreateLaboratoryTest','Create Laboratory Test','Laboratory Test',NULL,'Create'),
            ('CanUpdateLaboratoryTest','Update Laboratory Test','Laboratory Test',NULL,'Update'),
            ('CanDeleteLaboratoryTest','Delete Laboratory Test','Laboratory Test',NULL,'Delete'),
            ('CanManageLaboratoryTest','Manage Laboratory Test','Laboratory Test',NULL,'Manage'),

            ('CanReadTestMethodSpecification','View Test Method Specification','Test Method Specification',NULL,'Read'),
            ('CanCreateTestMethodSpecification','Create Test Method Spec','Test Method Specification',NULL,'Create'),
            ('CanUpdateTestMethodSpecification','Update Test Method Spec','Test Method Specification',NULL,'Update'),
            ('CanDeleteTestMethodSpecification','Delete Test Method Spec','Test Method Specification',NULL,'Delete'),
            ('CanManageTestMethodSpecification','Manage Test Method Spec','Test Method Specification',NULL,'Manage'),

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
    // 8. ADMIN USER  (Department → Designation → Employee → User)
    //    Password: Admin@123 (ForcePasswordChange = true)
    // ───────────────────────────────────────────────
    private static async Task SeedAdminUserAsync(LIMSContext db, ILogger logger)
    {
        // Check if admin user already exists
        var exists = await db.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM UserMasters WHERE UserName = N'admin' AND IsActive = 1")
            .FirstAsync();
        if (exists > 0) return;

        // Get Admin role
        var adminRole = await db.RoleMasters
            .FirstOrDefaultAsync(r => r.Name == "Admin" && r.IsActive);
        if (adminRole == null)
        {
            logger.LogWarning("DataSeeder: Admin role not found — skipping user creation.");
            return;
        }

        // Department
        var dept = await db.DepartmentMasters
            .FirstOrDefaultAsync(d => d.Name == "Administration" && d.IsActive);
        if (dept == null)
        {
            dept = new DepartmentMaster { Name = "Administration", Description = "Administration Department" };
            db.DepartmentMasters.Add(dept);
            await db.SaveChangesAsync();
        }

        // Designation
        var desig = await db.DesignationMasters
            .FirstOrDefaultAsync(d => d.Name == "System Administrator" && d.IsActive);
        if (desig == null)
        {
            desig = new DesignationMaster
            {
                Name = "System Administrator",
                Description = "Full system access",
                RoleID = adminRole.ID
            };
            db.DesignationMasters.Add(desig);
            await db.SaveChangesAsync();
        }

        // Employee
        var emp = await db.EmployeeMasters
            .FirstOrDefaultAsync(e => e.Name == "System Admin" && e.IsActive);
        if (emp == null)
        {
            emp = new EmployeeMaster
            {
                Name = "System Admin",
                EmailId = "admin@lims.com",
                Gender = "Male",
                DesignationID = desig.ID,
                DepartmentID = dept.ID,
                DateOfJoin = DateTime.UtcNow,
                DateOfBirth = new DateTime(1990, 1, 1),
                RoleID = adminRole.ID,
                MobileNo = "0000000000",
                ResidentialPinCode = "000000",
                ResidentialAreaID = 0,
                PermanentPinCode = "000000",
                PermanentAreaID = 0
            };
            db.EmployeeMasters.Add(emp);
            await db.SaveChangesAsync();
        }

        // User with hashed password
        var passwordHasher = new PasswordHasher<UserMaster>();
        var user = new UserMaster
        {
            UserName = "admin",
            EmailId = "admin@lims.com",
            Password = passwordHasher.HashPassword(null!, "Admin@123"),
            RoleID = adminRole.ID,
            RoleName = "Admin",
            IsAdmin = true,
            EmployeeID = emp.ID,
            IsLoginEnabled = true,
            AccountStatus = "Active",
            ForcePasswordChange = false
        };
        db.UserMasters.Add(user);
        await db.SaveChangesAsync();

        logger.LogInformation("DataSeeder: Admin user created (admin / Admin@123). Password change required on first login.");
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
                "CanReadMaterialSpecification", "CanReadProductSpecification",
                "CanReadMetalClassification", "CanReadHeatTreatment",
                "CanReadProductCondition", "CanReadSpecimenOrientation",
                "CanReadProductForm", "CanReadLaboratoryTest",
            },

            ["Technical"] = new[]
            {
                // Plan management
                "CanReadPlan", "CanManagePlan", "CanApprovePlan", "CanRejectPlan",
                "CanReadReview", "CanApproveReview", "CanRejectReview", "CanManageReview",
                // Sample prep
                "CanReadSampleCutting", "CanManageSampleCutting",
                // Inward read (plan upstream)
                "CanReadSampleInward",
                // Testing read
                "CanReadTesting", "CanReadTestingDashboard", "CanReadTestResults",
                // Reporting read
                "CanReadReporting",
                // Most masters read
                "CanReadMaterialSpecification", "CanReadProductSpecification",
                "CanReadLaboratoryTest", "CanReadTestMethodSpecification",
                "CanReadChemicalParameter", "CanReadMechanicalParameter",
                "CanReadParameterUnit", "CanReadMetalClassification",
                "CanReadHeatTreatment", "CanReadProductCondition",
                "CanReadSpecimenOrientation", "CanReadProductForm",
                "CanReadDimensionalFactors", "CanReadStandardOrganization",
                "CanReadEquipment", "CanReadCalibrationAgency",
            },

            ["Lab"] = new[]
            {
                // Testing CRUD
                "CanReadTesting", "CanManageTesting", "CanPerformTest",
                "CanReadTestingDashboard", "CanReadTestResults",
                "CanReadPerformTest", "CanReadLongTermTracking",
                "TEST_RESULT_SAVE",
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

                // Masters — FULL CRUD (LabManager can add/edit/delete any master)
                "CanReadCustomerMaster", "CanCreateCustomerMaster",
                "CanUpdateCustomerMaster", "CanDeleteCustomerMaster", "CanManageCustomerMaster",
                "CanReadEmployee", "CanCreateEmployee", "CanUpdateEmployee", "CanDeleteEmployee", "CanManageEmployee",
                "CanReadDepartment", "CanCreateDepartment", "CanUpdateDepartment", "CanDeleteDepartment", "CanManageDepartment",
                "CanReadDesignation", "CanCreateDesignation", "CanUpdateDesignation", "CanDeleteDesignation", "CanManageDesignation",
                "CanReadEquipment", "CanCreateEquipment", "CanUpdateEquipment", "CanDeleteEquipment", "CanManageEquipment",
                "CanReadCalibrationAgency", "CanCreateCalibrationAgency", "CanUpdateCalibrationAgency", "CanDeleteCalibrationAgency", "CanManageCalibrationAgency",
                "CanReadMaterialSpecification", "CanCreateMaterialSpecification", "CanUpdateMaterialSpecification", "CanDeleteMaterialSpecification", "CanManageMaterialSpecification",
                "CanReadProductSpecification", "CanCreateProductSpecification", "CanUpdateProductSpecification", "CanDeleteProductSpecification", "CanManageProductSpecification",
                "CanReadLaboratoryTest", "CanCreateLaboratoryTest", "CanUpdateLaboratoryTest", "CanDeleteLaboratoryTest", "CanManageLaboratoryTest",
                "CanReadTestMethodSpecification", "CanCreateTestMethodSpecification", "CanUpdateTestMethodSpecification", "CanDeleteTestMethodSpecification", "CanManageTestMethodSpecification",
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
                "CanReadCustomerPO", "CanReadCuttingPrice",

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
}
