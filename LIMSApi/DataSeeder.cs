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
            // Quick check: if Admin role exists, seeding was already done
            var alreadySeeded = await db.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM RoleMasters WHERE Name = N'Admin' AND IsActive = 1")
                .FirstAsync();
            if (alreadySeeded > 0)
            {
                logger.LogInformation("DataSeeder: seed data already present — skipping.");
                return;
            }

            logger.LogInformation("DataSeeder: first-time deployment detected — seeding data...");

            await SeedRolesAsync(db);
            await SeedMenusAsync(db);
            await SeedPermissionsAsync(db);
            await SeedRoleMenuMappingsAsync(db);
            await SeedConfigurationsAsync(db);
            await SeedCurrenciesAsync(db);
            await SeedAdminUserAsync(db, logger);

            logger.LogInformation("DataSeeder: seeding complete.");
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

    // ───────────────────────────────────────────────
    // 2. MENUS  (145 items, 4 hierarchy levels)
    //    Top-level IDs remapped to 1001-1012 to avoid
    //    conflict with child IDs 11,12 etc.
    // ───────────────────────────────────────────────
    private static async Task SeedMenusAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT MenuMasters ON;

            -- ══════ Level 0: Top-level menus ══════
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1001) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1001,N'Administration',N'bi-people',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1002) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1002,N'Specification',N'bi-box',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1003) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1003,N'Test',N'bi-file-earmark',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1004) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1004,N'Customer',N'bi-people',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1005) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1005,N'Sample',N'bi-layout-text-sidebar',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1006) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1006,N'Invoice',N'bi-receipt-cutoff',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1007) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1007,N'NABL ISO 17025',N'bi-shield-check',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1008) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1008,N'User Management',N'bi-person-fill-gear',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1009) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1009,N'Testing',N'bi-dropbox',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1010) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1010,N'Configuration',N'bi-gear',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1011) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1011,N'Reporting',N'bi-file-text',0,NULL,NULL,NULL);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 1012) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (1012,N'Accounts',N'bi-wallet2',0,NULL,NULL,NULL);

            -- ══════ Level 1: Direct children of top-level ══════
            -- Administration (1001)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 11)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (11,N'Department Master',NULL,0,N'/department',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 12)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (12,N'Employee Master',NULL,0,N'/employee',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 13)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (13,N'Designation Master',NULL,0,N'/designation',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 14)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (14,N'Tax Master',NULL,0,N'/tax',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 15)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (15,N'Bank Master',NULL,0,N'/bank',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 16)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (16,N'Courier Master',NULL,0,N'/courier',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 17)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (17,N'TPI Master',NULL,0,N'/tpi',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 18)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (18,N'Supplier Master',NULL,0,N'/supplier',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 19)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (19,N'Equipment',NULL,0,N'/equipment',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 20)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (20,N'OEM Master',NULL,0,N'/oem',NULL,1001);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 21)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (21,N'Calibration Agency',NULL,0,N'/calibration-agency',NULL,1001);
            -- Specification (1002)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 200) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (200,N'Material Specification',NULL,0,NULL,NULL,1002);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 202) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (202,N'Product Specification',NULL,0,NULL,NULL,1002);
            -- Test (1003)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 35)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (35,N'Laboratory Test',NULL,0,N'/test',NULL,1003);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 36)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (36,N'Test Method Specification',NULL,0,N'/test-specification',NULL,1003);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 37)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (37,N'Invoice Case',NULL,0,N'/invoice-case',NULL,1003);
            -- Customer (1004)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 38)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (38,N'Company Category',NULL,0,N'/company-category',NULL,1004);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 39)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (39,N'Customer Master',NULL,0,N'/customer',NULL,1004);
            -- Sample (1005)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 40)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (40,N'Inward',NULL,0,N'/sample/inward',NULL,1005);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 41)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (41,N'Plan',NULL,0,N'/sample/plan',NULL,1005);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 42)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (42,N'Review',NULL,0,N'/sample/review',NULL,1005);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 500) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (500,N'Sample Preparation',NULL,0,NULL,NULL,1005);
            -- Invoice (1006)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 47)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (47,N'Invoice Case Config',NULL,0,N'/invoice-case-config',NULL,1006);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 48)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (48,N'Invoice Case',NULL,0,N'/invoice-case',NULL,1006);
            -- NABL ISO 17025 (1007)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 71)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (71,N'General Requirements',NULL,0,NULL,NULL,1007);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 72)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (72,N'Structural Requirements',NULL,0,NULL,NULL,1007);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 73)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (73,N'Resource Requirements',NULL,0,NULL,NULL,1007);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 74)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (74,N'Process Requirements',NULL,0,NULL,NULL,1007);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 75)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (75,N'Management System',NULL,0,NULL,NULL,1007);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 49)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (49,N'Lab Scope Master',NULL,0,N'/scope',NULL,1007);
            -- User Management (1008)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 50)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (50,N'Lab Employee Master',NULL,0,N'/nabl/lab-employee',NULL,1008);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 51)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (51,N'Lab Score Master',NULL,0,N'/nabl/lab-score',NULL,1008);
            -- Testing (1009)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 56)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (56,N'Testing Dashboard',NULL,0,N'/testing/dashboard',NULL,1009);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 57)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (57,N'Perform Test',NULL,0,N'/testing/perform/:id',NULL,1009);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 58)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (58,N'Long Term Tracking',NULL,0,N'/testing/longterm',NULL,1009);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 59)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (59,N'Test Results',NULL,0,N'/testing/results/:id',NULL,1009);
            -- Configuration (1010)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 60)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (60,N'Configuration Manager',NULL,0,N'/config',NULL,1010);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 61)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (61,N'Menu Management',NULL,0,N'/menu',NULL,1010);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 62)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (62,N'Menu Permission',NULL,0,N'/menu-permission',NULL,1010);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 63)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (63,N'Role Management',NULL,0,N'/role',NULL,1010);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 64)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (64,N'User Permission',NULL,0,N'/user-permission',NULL,1010);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 65)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (65,N'Workflow',NULL,0,N'/workflow',NULL,1010);
            -- Reporting (1011)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 66)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (66,N'Reporting Dashboard',NULL,0,N'/reporting/dashboard',NULL,1011);
            -- Accounts (1012)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 121) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (121,N'Accounts Dashboard',NULL,0,N'/accounts/dashboard',NULL,1012);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 122) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (122,N'Case Accounts',NULL,0,N'/accounts/cases',NULL,1012);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 123) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (123,N'Customer Ledger',NULL,0,N'/account/ledger',NULL,1012);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 124) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (124,N'Record Payment',NULL,0,N'/account/record-payment',NULL,1012);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 125) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (125,N'Aging Report',NULL,0,N'/account/aging-report',NULL,1012);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 126) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (126,N'Outstanding Report',NULL,0,N'/account/outstanding-report',NULL,1012);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 127) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (127,N'Customer Purchase Orders',NULL,0,N'/accounts/purchase-orders',NULL,1012);

            -- ══════ Level 2: Sub-children ══════
            -- Under Material Specification (200)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 31)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (31,N'Material Specification',NULL,0,N'/material-specification',NULL,200);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 32)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (32,N'Custom Material Specification',NULL,0,N'/custom-material-specification',NULL,200);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 201) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (201,N'Linked Masters',NULL,0,NULL,NULL,200);
            -- Under Product Specification (202)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 33)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (33,N'Product Specification',NULL,0,N'/product-specification',NULL,202);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 34)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (34,N'Custom Product Specification',NULL,0,N'/custom-product-specification',NULL,202);
            -- Under Sample Preparation (500)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 43)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (43,N'Sample Cutting',NULL,0,N'/sample/cutting',NULL,500);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 46)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (46,N'Machining Charges',NULL,0,N'/sample/machining',NULL,500);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 44)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (44,N'Cutting Price Master',NULL,0,N'/cutting-price-master',NULL,500);
            -- Under General Requirements (71)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7101) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7101,N'F-2: Confidentiality Agree.',NULL,0,N'/supplier-confidentiality-agreement',NULL,71);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7102) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7102,N'F-4: Impartiality Agree.',NULL,0,N'/employee/impartiality-agreement',NULL,71);
            -- Under Structural Requirements (72)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7201) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7201,N'Organization Chart',NULL,0,N'/org-chart',NULL,72);
            -- Under Resource Requirements (73)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7301) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7301,N'Personnel',NULL,0,NULL,NULL,73);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7302) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7302,N'Facilities & Environment',NULL,0,NULL,NULL,73);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7303) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7303,N'Equipment',NULL,0,NULL,NULL,73);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7304) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7304,N'External Products',NULL,0,NULL,NULL,73);
            -- Under Process Requirements (74)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7401) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7401,N'F-27: Test Request',NULL,0,N'/nabl/test-request',NULL,74);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7402) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7402,N'Methods Management',NULL,0,NULL,NULL,74);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7403) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7403,N'Sample Handling',NULL,0,NULL,NULL,74);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7404) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7404,N'F-34: Technical Raw Data',NULL,0,N'/nabl/technical-raw-data',NULL,74);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7405) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7405,N'F-35: Uncertainty Rec.',NULL,0,N'/measurement-uncertainty',NULL,74);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7406) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7406,N'Ensuring Validity',NULL,0,NULL,NULL,74);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7407) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7407,N'F-39: Test Report',NULL,0,N'/test-report',NULL,74);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7408) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7408,N'F-40: Complaint Reg.',NULL,0,N'/complaint-register',NULL,74);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7409) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7409,N'F-41: NC Work Records',NULL,0,N'/non-conforming-work',NULL,74);
            -- Under Management System (75)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7501) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7501,N'Documentation Control',NULL,0,NULL,NULL,75);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7502) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7502,N'F-46: Risk Assessment',NULL,0,N'/risk-assessment',NULL,75);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7503) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7503,N'Improvement & Actions',NULL,0,NULL,NULL,75);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7504) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7504,N'Internal Audits',NULL,0,NULL,NULL,75);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 7505) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (7505,N'Management Review',NULL,0,NULL,NULL,75);

            -- ══════ Level 3: Deep children ══════
            -- Linked Masters (201)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 28)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (28,N'Standard Organization',NULL,0,N'/standard-organization',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 30)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (30,N'Metal Classification',NULL,0,N'/metal-classification',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 24)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (24,N'Chemical Parameter',NULL,0,N'/chemical-parameter',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 25)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (25,N'Mechanical Parameter',NULL,0,N'/mechanical-parameter',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 210) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (210,N'Parameter Unit',NULL,0,N'/parameter-unit',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 23)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (23,N'Heat Treatment',NULL,0,N'/heat-treatment',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 26)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (26,N'Product Condition',NULL,0,N'/product-condition',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 27)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (27,N'Specimen Orientation',NULL,0,N'/specimen-orientation',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 22)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (22,N'Dimensional Factor',NULL,0,N'/dimesional-factor',NULL,201);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 29)  INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (29,N'Universal Code Type',NULL,0,N'/universal-code-type',NULL,201);
            -- Personnel (7301)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730101) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730101,N'F-1: Job Description',NULL,0,N'/job-description',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730102) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730102,N'F-3: Resp. & Authority',NULL,0,N'/responsibility-authority',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730103) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730103,N'F-5: Competence Req.',NULL,0,N'/competence-requirement',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730104) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730104,N'F-6: Induction Training',NULL,0,N'/induction-training',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730105) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730105,N'F-7: Competence Report',NULL,0,N'/employee/competence',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730106) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730106,N'F-8: Training Plan',NULL,0,N'/training-plan',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730107) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730107,N'F-9: Training Attendance',NULL,0,N'/training-attendance',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730108) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730108,N'F-10: Training Effectiv.',NULL,0,N'/training-effectiveness',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730109) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730109,N'F-11: Skill Matrix',NULL,0,N'/skill-matrix',NULL,7301);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730110) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730110,N'F-13: Employee Authorization',NULL,0,N'/employee/equipment-authorization/list',NULL,7301);
            -- Facilities & Environment (7302)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730201) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730201,N'F-12: Environment Mon.',NULL,0,N'/environment-monitoring',NULL,7302);
            -- Equipment (7303)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730301) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730301,N'F-14: Equipment History',NULL,0,N'/equipment-history-card',NULL,7303);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730302) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730302,N'F-15: Calibration Review',NULL,0,N'/calibration-review',NULL,7303);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730303) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730303,N'F-16: Intermediate Check',NULL,0,N'/intermediate-check-records',NULL,7303);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730304) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730304,N'F-17: Ref. Material List',NULL,0,N'/reference-material',NULL,7303);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730305) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730305,N'F-18: CRM Consumption',NULL,0,N'/reference-material-consumption',NULL,7303);
            -- External Products (7304)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730401) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730401,N'F-19: Supplier Reg.',NULL,0,N'/supplier-registration',NULL,7304);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730402) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730402,N'F-20: Approved Suppliers',NULL,0,N'/approved-supplier',NULL,7304);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730403) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730403,N'F-21: Purchase Indent',NULL,0,N'/purchase-indent',NULL,7304);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730404) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730404,N'F-22: Purchase Order',NULL,0,N'/purchase-order',NULL,7304);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730405) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730405,N'F-23: Inspection Plan',NULL,0,N'/product-inspection',NULL,7304);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730406) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730406,N'F-24: Incoming Mat. Rec.',NULL,0,N'/incoming-material',NULL,7304);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730407) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730407,N'F-25: Mat. Verification',NULL,0,N'/purchase-material-verification',NULL,7304);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 730408) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (730408,N'F-26: Supplier Eval.',NULL,0,N'/supplier-evaluation',NULL,7304);
            -- Methods Management (7402)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740201) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740201,N'F-28: Test Methods',NULL,0,N'/nabl/test-method',NULL,7402);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740202) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740202,N'F-29: Verification',NULL,0,N'/nabl/method-verification',NULL,7402);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740203) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740203,N'F-30: Validation',NULL,0,N'/nabl/method-validation',NULL,7402);
            -- Sample Handling (7403)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740301) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740301,N'F-31: Inward Register',NULL,0,N'/nabl/sample-inward-register',NULL,7403);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740302) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740302,N'F-32: Muster Register',NULL,0,N'/nabl/sample-muster-register',NULL,7403);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740303) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740303,N'F-33: Sample Label',NULL,0,N'/nabl/sample-label',NULL,7403);
            -- Ensuring Validity (7406)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740601) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740601,N'F-36: PT / ILC Plan',NULL,0,N'/pt-ilc-plan',NULL,7406);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740602) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740602,N'F-37: QC Plan',NULL,0,N'/quality-control-plan',NULL,7406);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 740603) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (740603,N'F-38: Retesting Rec.',NULL,0,N'/retesting-retained-sample',NULL,7406);
            -- Documentation Control (7501)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750101) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750101,N'F-43: Master Document',NULL,0,N'/master-document',NULL,7501);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750102) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750102,N'F-44: Doc. Change Req.',NULL,0,N'/document-change-request',NULL,7501);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750103) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750103,N'F-45: Doc. Review Rec.',NULL,0,N'/document-review',NULL,7501);
            -- Improvement & Actions (7503)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750301) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750301,N'F-47: Cust. Feedback',NULL,0,N'/customer-feedback',NULL,7503);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750302) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750302,N'F-48: Feedback Analys.',NULL,0,N'/feedback-analysis',NULL,7503);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750303) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750303,N'F-42: NC & Corr. Action',NULL,0,N'/nc-corrective-action',NULL,7503);
            -- Internal Audits (7504)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750401) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750401,N'F-49: Internal Auditors',NULL,0,N'/internal-auditor',NULL,7504);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750402) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750402,N'F-50: Audit Plan',NULL,0,N'/audit-plan',NULL,7504);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750403) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750403,N'F-51: Audit Checklist',NULL,0,N'/audit-checklist',NULL,7504);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750404) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750404,N'F-52: Audit Summary',NULL,0,N'/audit-summary',NULL,7504);
            -- Management Review (7505)
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750501) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750501,N'F-53: Meeting Agenda',NULL,0,N'/meeting-agenda',NULL,7505);
            IF NOT EXISTS (SELECT 1 FROM MenuMasters WHERE ID = 750502) INSERT INTO MenuMasters (ID,Title,Icon,IsExpanded,Route,Color,ParentID) VALUES (750502,N'F-54: Meeting Minutes',NULL,0,N'/meeting-minutes',NULL,7505);

            SET IDENTITY_INSERT MenuMasters OFF;
        ");
    }

    // ───────────────────────────────────────────────
    // 3. PERMISSIONS  (136 total: 112 read + 20 action + 4 backend)
    // ───────────────────────────────────────────────
    private static async Task SeedPermissionsAsync(LIMSContext db)
    {
        // Use temp table for clean typed inserts
        await db.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID('tempdb..#SeedPerms') IS NOT NULL DROP TABLE #SeedPerms;
            CREATE TABLE #SeedPerms (Name NVARCHAR(100), DisplayName NVARCHAR(200), MenuID BIGINT NULL, Type NVARCHAR(50));

            INSERT INTO #SeedPerms (Name, DisplayName, MenuID, Type) VALUES
            -- ── Administration ──
            (N'CanReadDepartment',N'View Department',11,N'Read'),
            (N'CanReadEmployee',N'View Employee',12,N'Read'),
            (N'CanReadDesignation',N'View Designation',13,N'Read'),
            (N'CanReadTax',N'View Tax',14,N'Read'),
            (N'CanReadBank',N'View Bank',15,N'Read'),
            (N'CanReadCourier',N'View Courier',16,N'Read'),
            (N'CanReadTPI',N'View TPI',17,N'Read'),
            (N'CanReadSupplier',N'View Supplier',18,N'Read'),
            (N'CanReadEquipment',N'View Equipment',19,N'Read'),
            (N'CanReadOEM',N'View OEM',20,N'Read'),
            (N'CanReadCalibrationAgency',N'View Calibration Agency',21,N'Read'),
            -- ── Specification ──
            (N'CanReadMaterialSpecification',N'View Material Specification',31,N'Read'),
            (N'CanReadCustomMaterialSpecification',N'View Custom Material Specification',32,N'Read'),
            (N'CanReadStandardOrganization',N'View Standard Organization',28,N'Read'),
            (N'CanReadMetalClassification',N'View Metal Classification',30,N'Read'),
            (N'CanReadChemicalParameter',N'View Chemical Parameter',24,N'Read'),
            (N'CanReadMechanicalParameter',N'View Mechanical Parameter',25,N'Read'),
            (N'CanReadParameterUnit',N'View Parameter Unit',210,N'Read'),
            (N'CanReadHeatTreatment',N'View Heat Treatment',23,N'Read'),
            (N'CanReadProductCondition',N'View Product Condition',26,N'Read'),
            (N'CanReadSpecimenOrientation',N'View Specimen Orientation',27,N'Read'),
            (N'CanReadDimensionalFactors',N'View Dimensional Factors',22,N'Read'),
            (N'CanReadUniversalCode',N'View Universal Code',29,N'Read'),
            (N'CanReadProductSpecification',N'View Product Specification',33,N'Read'),
            (N'CanReadCustomProductSpecification',N'View Custom Product Specification',34,N'Read'),
            -- ── Test ──
            (N'CanReadLaboratoryTest',N'View Laboratory Test',35,N'Read'),
            (N'CanReadTestMethodSpecification',N'View Test Method Specification',36,N'Read'),
            (N'CanReadInvoiceCase',N'View Invoice Case',37,N'Read'),
            -- ── Customer ──
            (N'CanReadCompanyCategory',N'View Company Category',38,N'Read'),
            (N'CanReadCustomerMaster',N'View Customer Master',39,N'Read'),
            -- ── Sample ──
            (N'CanReadInward',N'View Inward',40,N'Read'),
            (N'CanReadPlan',N'View Plan',41,N'Read'),
            (N'CanReadReview',N'View Review',42,N'Read'),
            (N'CanReadSampleCutting',N'View Sample Cutting',43,N'Read'),
            (N'CanReadMachiningChallan',N'View Machining Challan',46,N'Read'),
            (N'CanReadCuttingPrice',N'View Cutting Price',44,N'Read'),
            -- ── Invoice ──
            (N'CanReadInvoiceCaseConfig',N'View Invoice Case Config',47,N'Read'),
            -- ── NABL: General Requirements ──
            (N'CanReadConfidentialityAgreement',N'View Confidentiality Agreement',7101,N'Read'),
            (N'CanReadImpartialityAgreement',N'View Impartiality Agreement',7102,N'Read'),
            -- ── NABL: Structural ──
            (N'CanReadOrgChart',N'View Organization Chart',7201,N'Read'),
            -- ── NABL: Personnel ──
            (N'CanReadJobDescription',N'View Job Description',730101,N'Read'),
            (N'CanReadRA',N'View Resp. & Authority',730102,N'Read'),
            (N'CanReadCompetenceRequirement',N'View Competence Requirement',730103,N'Read'),
            (N'CanReadInductionTraining',N'View Induction Training',730104,N'Read'),
            (N'CanReadEmployeeCompetence',N'View Employee Competence',730105,N'Read'),
            (N'CanReadTrainingPlan',N'View Training Plan',730106,N'Read'),
            (N'CanReadTrainingAttendance',N'View Training Attendance',730107,N'Read'),
            (N'CanReadTrainingEffectiveness',N'View Training Effectiveness',730108,N'Read'),
            (N'CanReadSkillMatrix',N'View Skill Matrix',730109,N'Read'),
            (N'CanReadEmployeeAuthorization',N'View Employee Authorization',730110,N'Read'),
            -- ── NABL: Facilities ──
            (N'CanReadEnvironmentMonitoring',N'View Environment Monitoring',730201,N'Read'),
            -- ── NABL: Equipment ──
            (N'CanReadEquipmentHistory',N'View Equipment History',730301,N'Read'),
            (N'CanReadCalibrationReview',N'View Calibration Review',730302,N'Read'),
            (N'CanReadIntermediateCheck',N'View Intermediate Check',730303,N'Read'),
            (N'CanReadReferenceMaterial',N'View Reference Material',730304,N'Read'),
            (N'CanReadCRMConsumption',N'View CRM Consumption',730305,N'Read'),
            -- ── NABL: External Products ──
            (N'CanReadSupplierRegistration',N'View Supplier Registration',730401,N'Read'),
            (N'CanReadApprovedSupplier',N'View Approved Supplier',730402,N'Read'),
            (N'CanReadPurchaseIndent',N'View Purchase Indent',730403,N'Read'),
            (N'CanReadPurchaseOrder',N'View Purchase Order',730404,N'Read'),
            (N'CanReadProductInspection',N'View Product Inspection',730405,N'Read'),
            (N'CanReadIncomingMaterial',N'View Incoming Material',730406,N'Read'),
            (N'CanReadMaterialVerification',N'View Material Verification',730407,N'Read'),
            (N'CanReadSupplierEvaluation',N'View Supplier Evaluation',730408,N'Read'),
            -- ── NABL: Process Requirements ──
            (N'CanReadTestRequest',N'View Test Request',7401,N'Read'),
            (N'CanReadTestMethod',N'View Test Method',740201,N'Read'),
            (N'CanReadMethodVerification',N'View Method Verification',740202,N'Read'),
            (N'CanReadMethodValidation',N'View Method Validation',740203,N'Read'),
            (N'CanReadSampleInwardRegister',N'View Inward Register',740301,N'Read'),
            (N'CanReadSampleMusterRegister',N'View Muster Register',740302,N'Read'),
            (N'CanReadSampleLabel',N'View Sample Label',740303,N'Read'),
            (N'CanReadTechnicalRawData',N'View Technical Raw Data',7404,N'Read'),
            (N'CanReadUncertainty',N'View Uncertainty Records',7405,N'Read'),
            (N'CanReadPTPlan',N'View PT/ILC Plan',740601,N'Read'),
            (N'CanReadQCPlan',N'View QC Plan',740602,N'Read'),
            (N'CanReadRetesting',N'View Retesting Records',740603,N'Read'),
            (N'CanReadTestReport',N'View Test Report',7407,N'Read'),
            (N'CanReadComplaintRegister',N'View Complaint Register',7408,N'Read'),
            (N'CanReadNonConformingWork',N'View Non-Conforming Work',7409,N'Read'),
            -- ── NABL: Management System ──
            (N'CanReadMasterDocument',N'View Master Document',750101,N'Read'),
            (N'CanReadDocChangeRequest',N'View Document Change Request',750102,N'Read'),
            (N'CanReadDocumentReview',N'View Document Review',750103,N'Read'),
            (N'CanReadRiskAssessment',N'View Risk Assessment',7502,N'Read'),
            (N'CanReadCustomerFeedback',N'View Customer Feedback',750301,N'Read'),
            (N'CanReadFeedbackAnalysis',N'View Feedback Analysis',750302,N'Read'),
            (N'CanReadNCAction',N'View NC & Corrective Action',750303,N'Read'),
            (N'CanReadInternalAuditor',N'View Internal Auditors',750401,N'Read'),
            (N'CanReadAuditPlan',N'View Audit Plan',750402,N'Read'),
            (N'CanReadAuditChecklist',N'View Audit Checklist',750403,N'Read'),
            (N'CanReadAuditSummary',N'View Audit Summary',750404,N'Read'),
            (N'CanReadMeetingAgenda',N'View Meeting Agenda',750501,N'Read'),
            (N'CanReadMeetingMinutes',N'View Meeting Minutes',750502,N'Read'),
            -- ── Lab Scope ──
            (N'CanReadLabScopeMaster',N'View Lab Scope',49,N'Read'),
            -- ── User Management ──
            (N'CanReadLabEmployeeMaster',N'View Lab Employee',50,N'Read'),
            (N'CanReadLabScore',N'View Lab Score',51,N'Read'),
            -- ── Testing ──
            (N'CanReadTestingDashboard',N'View Testing Dashboard',56,N'Read'),
            (N'CanReadPerformTest',N'View Perform Test',57,N'Read'),
            (N'CanReadLongTermTracking',N'View Long Term Tracking',58,N'Read'),
            (N'CanReadTestResults',N'View Test Results',59,N'Read'),
            -- ── Configuration ──
            (N'CanReadConfiguration',N'View Configuration',60,N'Read'),
            (N'CanReadMenuManagement',N'View Menu Management',61,N'Read'),
            (N'CanReadMenuPermission',N'View Menu Permission',62,N'Read'),
            (N'CanReadRoleManagement',N'View Role Management',63,N'Read'),
            (N'CanReadUserPermission',N'View User Permission',64,N'Read'),
            (N'CanReadWorkflow',N'View Workflow',65,N'Read'),
            -- ── Reporting ──
            (N'CanReadReporting',N'View Reporting',66,N'Read'),
            -- ── Accounts ──
            (N'CanReadAccountsDashboard',N'View Accounts Dashboard',121,N'Read'),
            (N'CanReadCaseAccounts',N'View Case Accounts',122,N'Read'),
            (N'CanReadCustomerLedger',N'View Customer Ledger',123,N'Read'),
            (N'CanReadRecordPayment',N'View Record Payment',124,N'Read'),
            (N'CanReadAgingReport',N'View Aging Report',125,N'Read'),
            (N'CanReadOutstandingReport',N'View Outstanding Report',126,N'Read'),
            (N'CanReadCustomerPO',N'View Customer Purchase Orders',127,N'Read'),

            -- ═══════════════════════════════════════
            -- ACTION PERMISSIONS (frontend role-helper)
            -- ═══════════════════════════════════════
            (N'CanReadAccount',N'Read Account',1012,N'Read'),
            (N'CanManageAccount',N'Manage Account',1012,N'Manage'),
            (N'CanGeneratePI',N'Generate Proforma Invoice',122,N'Action'),
            (N'CanGenerateInvoice',N'Generate Invoice',122,N'Action'),
            (N'CanManageInvoice',N'Manage Invoice',122,N'Manage'),
            (N'CanApproveReport',N'Approve Report',66,N'Action'),
            (N'CanManageReporting',N'Manage Reporting',66,N'Manage'),
            (N'CanAmendReport',N'Amend Report',66,N'Action'),
            (N'CanReadTesting',N'Read Testing',1009,N'Read'),
            (N'CanManageTesting',N'Manage Testing',1009,N'Manage'),
            (N'CanPerformTest',N'Perform Test',57,N'Action'),
            (N'CanReadSampleInward',N'Read Sample Inward',40,N'Read'),
            (N'CanManageSampleInward',N'Manage Sample Inward',40,N'Manage'),
            (N'CanCreateSampleInward',N'Create Sample Inward',40,N'Create'),
            (N'CanUpdateSampleInward',N'Update Sample Inward',40,N'Update'),
            (N'CanDeleteSampleInward',N'Delete Sample Inward',40,N'Delete'),
            (N'CanCreateLabScopeMaster',N'Create Lab Scope',49,N'Create'),
            (N'CanUpdateLabScopeMaster',N'Update Lab Scope',49,N'Update'),
            (N'CanDeleteLabScopeMaster',N'Delete Lab Scope',49,N'Delete'),
            (N'CanManageLabScopeMaster',N'Manage Lab Scope',49,N'Manage'),

            -- ═══════════════════════════════════════
            -- BACKEND [RequirePermission] attributes
            -- ═══════════════════════════════════════
            (N'TEST_RESULT_SAVE',N'Save Test Result',59,N'Action'),
            (N'TEST_PRICE_OVERRIDE',N'Override Test Price',59,N'Action'),
            (N'TEST_RESULT_VERIFY',N'Verify Test Result',59,N'Action'),
            (N'INVOICE_GENERATE',N'Generate Invoice (Backend)',122,N'Action');

            -- Insert only permissions that don't already exist (by name)
            INSERT INTO PermissionMasters (Name, DisplayName, MenuID, Type)
            SELECT s.Name, s.DisplayName, s.MenuID, s.Type
            FROM #SeedPerms s
            WHERE NOT EXISTS (SELECT 1 FROM PermissionMasters p WHERE p.Name = s.Name);

            DROP TABLE #SeedPerms;
        ");
    }

    // ───────────────────────────────────────────────
    // 4. ROLE-MENU MAPPINGS  (Admin role → all menus)
    // ───────────────────────────────────────────────
    private static async Task SeedRoleMenuMappingsAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            DECLARE @AdminRoleID BIGINT = (SELECT TOP 1 ID FROM RoleMasters WHERE Name = N'Admin' AND IsActive = 1);
            IF @AdminRoleID IS NULL RETURN;

            -- All 145 menu IDs
            INSERT INTO RoleMenuMappings (RoleID, MenuID)
            SELECT @AdminRoleID, v.MenuID
            FROM (VALUES
                -- Top level
                (1001),(1002),(1003),(1004),(1005),(1006),(1007),(1008),(1009),(1010),(1011),(1012),
                -- Administration
                (11),(12),(13),(14),(15),(16),(17),(18),(19),(20),(21),
                -- Specification
                (200),(201),(202),(31),(32),(33),(34),(28),(30),(24),(25),(210),(23),(26),(27),(22),(29),
                -- Test / Customer
                (35),(36),(37),(38),(39),
                -- Sample
                (40),(41),(42),(500),(43),(46),(44),
                -- Invoice
                (47),(48),
                -- NABL
                (71),(72),(73),(74),(75),(49),
                (7101),(7102),(7201),
                (7301),(7302),(7303),(7304),
                (730101),(730102),(730103),(730104),(730105),(730106),(730107),(730108),(730109),(730110),
                (730201),
                (730301),(730302),(730303),(730304),(730305),
                (730401),(730402),(730403),(730404),(730405),(730406),(730407),(730408),
                (7401),(7402),(7403),(7404),(7405),(7406),(7407),(7408),(7409),
                (740201),(740202),(740203),
                (740301),(740302),(740303),
                (740601),(740602),(740603),
                (7501),(7502),(7503),(7504),(7505),
                (750101),(750102),(750103),
                (750301),(750302),(750303),
                (750401),(750402),(750403),(750404),
                (750501),(750502),
                -- User Management
                (50),(51),
                -- Testing
                (56),(57),(58),(59),
                -- Configuration
                (60),(61),(62),(63),(64),(65),
                -- Reporting
                (66),
                -- Accounts
                (121),(122),(123),(124),(125),(126)
            ) AS v(MenuID)
            WHERE NOT EXISTS (
                SELECT 1 FROM RoleMenuMappings rm
                WHERE rm.RoleID = @AdminRoleID AND rm.MenuID = v.MenuID
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
        ");
    }

    // ───────────────────────────────────────────────
    // 5b. DEFAULT CURRENCIES
    // ───────────────────────────────────────────────
    private static async Task SeedCurrenciesAsync(LIMSContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT 1 FROM CurrencyMasters WHERE Code = N'INR' AND IsActive = 1)
                INSERT INTO CurrencyMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Indian Rupee', N'INR', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM CurrencyMasters WHERE Code = N'USD' AND IsActive = 1)
                INSERT INTO CurrencyMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'US Dollar', N'USD', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM CurrencyMasters WHERE Code = N'EUR' AND IsActive = 1)
                INSERT INTO CurrencyMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'Euro', N'EUR', 0, GETUTCDATE(), N'LIMS', 1);

            IF NOT EXISTS (SELECT 1 FROM CurrencyMasters WHERE Code = N'GBP' AND IsActive = 1)
                INSERT INTO CurrencyMasters (Name, Code, CreatedBy, CreatedOn, CompanyCode, IsActive)
                VALUES (N'British Pound', N'GBP', 0, GETUTCDATE(), N'LIMS', 1);
        ");
    }

    // ───────────────────────────────────────────────
    // 6. ADMIN USER  (Department → Designation → Employee → User)
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
                DesignationID = desig.ID,
                DepartmentID = dept.ID,
                DateOfJoin = DateTime.UtcNow,
                RoleID = adminRole.ID,
                MobileNo = "0000000000",
                // Required address fields (placeholder — update via Settings)
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
            ForcePasswordChange = true
        };
        db.UserMasters.Add(user);
        await db.SaveChangesAsync();

        logger.LogInformation("DataSeeder: Admin user created (admin / Admin@123). Password change required on first login.");
    }
}