namespace LIMSApi.Helpers
{
    /// <summary>
    /// Central catalog of all permission names used across the application.
    /// Names MUST match PermissionMaster.Name exactly (case-sensitive).
    ///
    /// Naming convention:
    ///   CanRead{Entity}    — View/list/details
    ///   CanCreate{Entity}  — POST /create
    ///   CanUpdate{Entity}  — PUT /update
    ///   CanDelete{Entity}  — DELETE /delete/{id}
    ///   CanManage{Entity}  — All CRUD (grants Read+Create+Update+Delete)
    ///   {Entity}_{Action}  — Workflow transitions (uppercase_snake)
    ///
    /// Usage in controllers:
    ///   [RequirePermission(Permissions.Inward.Create)]
    ///   [RequirePermission(Permissions.Workflow.TestResultVerify)]
    ///
    /// Admin role bypasses all permission checks automatically.
    /// </summary>
    public static class Permissions
    {
        // ═══════════════════════════════════════════════════════
        // MASTERS — Administration
        // ═══════════════════════════════════════════════════════
        public static class Department
        {
            public const string Read = "CanReadDepartment";
            public const string Create = "CanCreateDepartment";
            public const string Update = "CanUpdateDepartment";
            public const string Delete = "CanDeleteDepartment";
            public const string Manage = "CanManageDepartment";
        }

        public static class Employee
        {
            public const string Read = "CanReadEmployee";
            public const string Create = "CanCreateEmployee";
            public const string Update = "CanUpdateEmployee";
            public const string Delete = "CanDeleteEmployee";
            public const string Manage = "CanManageEmployee";
        }

        public static class Designation
        {
            public const string Read = "CanReadDesignation";
            public const string Create = "CanCreateDesignation";
            public const string Update = "CanUpdateDesignation";
            public const string Delete = "CanDeleteDesignation";
            public const string Manage = "CanManageDesignation";
        }

        public static class Tax
        {
            public const string Read = "CanReadTax";
            public const string Create = "CanCreateTax";
            public const string Update = "CanUpdateTax";
            public const string Delete = "CanDeleteTax";
            public const string Manage = "CanManageTax";
        }

        public static class Bank
        {
            public const string Read = "CanReadBank";
            public const string Create = "CanCreateBank";
            public const string Update = "CanUpdateBank";
            public const string Delete = "CanDeleteBank";
            public const string Manage = "CanManageBank";
        }

        public static class Courier
        {
            public const string Read = "CanReadCourier";
            public const string Create = "CanCreateCourier";
            public const string Update = "CanUpdateCourier";
            public const string Delete = "CanDeleteCourier";
            public const string Manage = "CanManageCourier";
        }

        public static class TPI
        {
            public const string Read = "CanReadTPI";
            public const string Create = "CanCreateTPI";
            public const string Update = "CanUpdateTPI";
            public const string Delete = "CanDeleteTPI";
            public const string Manage = "CanManageTPI";
        }

        public static class Supplier
        {
            public const string Read = "CanReadSupplier";
            public const string Create = "CanCreateSupplier";
            public const string Update = "CanUpdateSupplier";
            public const string Delete = "CanDeleteSupplier";
            public const string Manage = "CanManageSupplier";
        }

        public static class Equipment
        {
            public const string Read = "CanReadEquipment";
            public const string Create = "CanCreateEquipment";
            public const string Update = "CanUpdateEquipment";
            public const string Delete = "CanDeleteEquipment";
            public const string Manage = "CanManageEquipment";
        }

        public static class OEM
        {
            public const string Read = "CanReadOEM";
            public const string Create = "CanCreateOEM";
            public const string Update = "CanUpdateOEM";
            public const string Delete = "CanDeleteOEM";
            public const string Manage = "CanManageOEM";
        }

        public static class CalibrationAgency
        {
            public const string Read = "CanReadCalibrationAgency";
            public const string Create = "CanCreateCalibrationAgency";
            public const string Update = "CanUpdateCalibrationAgency";
            public const string Delete = "CanDeleteCalibrationAgency";
            public const string Manage = "CanManageCalibrationAgency";
        }

        // ═══════════════════════════════════════════════════════
        // MASTERS — Technical
        // ═══════════════════════════════════════════════════════
        public static class MaterialSpecification
        {
            public const string Read = "CanReadMaterialSpecification";
            public const string Create = "CanCreateMaterialSpecification";
            public const string Update = "CanUpdateMaterialSpecification";
            public const string Delete = "CanDeleteMaterialSpecification";
            public const string Manage = "CanManageMaterialSpecification";
        }

        public static class ProductSpecification
        {
            public const string Read = "CanReadProductSpecification";
            public const string Create = "CanCreateProductSpecification";
            public const string Update = "CanUpdateProductSpecification";
            public const string Delete = "CanDeleteProductSpecification";
            public const string Manage = "CanManageProductSpecification";
        }

        public static class LaboratoryTest
        {
            public const string Read = "CanReadLaboratoryTest";
            public const string Create = "CanCreateLaboratoryTest";
            public const string Update = "CanUpdateLaboratoryTest";
            public const string Delete = "CanDeleteLaboratoryTest";
            public const string Manage = "CanManageLaboratoryTest";
        }

        public static class TestMethodSpecification
        {
            public const string Read = "CanReadTestMethodSpecification";
            public const string Create = "CanCreateTestMethodSpecification";
            public const string Update = "CanUpdateTestMethodSpecification";
            public const string Delete = "CanDeleteTestMethodSpecification";
            public const string Manage = "CanManageTestMethodSpecification";
        }

        public static class Parameter
        {
            public const string ReadChemical = "CanReadChemicalParameter";
            public const string ReadMechanical = "CanReadMechanicalParameter";
            public const string ReadUnit = "CanReadParameterUnit";
            public const string Create = "CanCreateParameter";
            public const string Update = "CanUpdateParameter";
            public const string Delete = "CanDeleteParameter";
            public const string Manage = "CanManageParameter";
        }

        public static class MetalClassification
        {
            public const string Read = "CanReadMetalClassification";
            public const string Create = "CanCreateMetalClassification";
            public const string Update = "CanUpdateMetalClassification";
            public const string Delete = "CanDeleteMetalClassification";
            public const string Manage = "CanManageMetalClassification";
        }

        public static class Customer
        {
            public const string Read = "CanReadCustomerMaster";
            public const string Create = "CanCreateCustomerMaster";
            public const string Update = "CanUpdateCustomerMaster";
            public const string Delete = "CanDeleteCustomerMaster";
            public const string Manage = "CanManageCustomerMaster";
        }

        public static class CompanyCategory
        {
            public const string Read = "CanReadCompanyCategory";
            public const string Create = "CanCreateCompanyCategory";
            public const string Update = "CanUpdateCompanyCategory";
            public const string Delete = "CanDeleteCompanyCategory";
            public const string Manage = "CanManageCompanyCategory";
        }

        // ═══════════════════════════════════════════════════════
        // FLOW STAGES — Sample workflow
        // ═══════════════════════════════════════════════════════
        public static class Inward
        {
            public const string Read = "CanReadSampleInward";
            public const string Create = "CanCreateSampleInward";
            public const string Update = "CanUpdateSampleInward";
            public const string Delete = "CanDeleteSampleInward";
            public const string Manage = "CanManageSampleInward";
        }

        public static class Plan
        {
            public const string Read = "CanReadPlan";
            public const string Create = "CanCreatePlan";
            public const string Update = "CanUpdatePlan";
            public const string Delete = "CanDeletePlan";
            public const string Manage = "CanManagePlan";
            public const string Approve = "CanApprovePlan";
            public const string Reject = "CanRejectPlan";
        }

        public static class Review
        {
            public const string Read = "CanReadReview";
            public const string Approve = "CanApproveReview";
            public const string Reject = "CanRejectReview";
            public const string Manage = "CanManageReview";
        }

        public static class SamplePreparation
        {
            public const string Read = "CanReadSampleCutting";
            public const string Create = "CanCreateSampleCutting";
            public const string Update = "CanUpdateSampleCutting";
            public const string Delete = "CanDeleteSampleCutting";
            public const string Manage = "CanManageSampleCutting";
        }

        public static class Testing
        {
            public const string Read = "CanReadTesting";
            public const string ReadDashboard = "CanReadTestingDashboard";
            public const string ReadResults = "CanReadTestResults";
            public const string Manage = "CanManageTesting";
            public const string Perform = "CanPerformTest";
            public const string SaveResult = "TEST_RESULT_SAVE";
            public const string VerifyResult = "TEST_RESULT_VERIFY";
            public const string PriceOverride = "TEST_PRICE_OVERRIDE";
        }

        public static class Reporting
        {
            public const string Read = "CanReadReporting";
            public const string Manage = "CanManageReporting";
            public const string Approve = "CanApproveReport";
            public const string Amend = "CanAmendReport";
            public const string ReadFormat = "CanReadReportFormat";
            public const string ManageFormat = "CanManageReportFormat";
        }

        // ═══════════════════════════════════════════════════════
        // ACCOUNTS — billing, invoicing, payments, ledger
        // ═══════════════════════════════════════════════════════
        public static class Account
        {
            // Top-level
            public const string Read = "CanReadAccount";
            public const string Manage = "CanManageAccount";

            // Dashboard + case list
            public const string ReadDashboard = "CanReadAccountsDashboard";
            public const string ReadCaseAccounts = "CanReadCaseAccounts";

            // Invoice generation
            public const string GeneratePI = "CanGeneratePI";
            public const string GenerateInvoice = "CanGenerateInvoice";
            public const string GenerateInvoiceBackend = "INVOICE_GENERATE";
            public const string ManageInvoice = "CanManageInvoice";

            // Pricing — calculation + validation
            public const string CalculatePricing = "CanCalculatePricing";
            public const string ValidatePricing = "CanValidatePricing";

            // Invoice line items
            public const string ReadInvoiceLineItem = "CanReadInvoiceLineItem";
            public const string ManageInvoiceLineItem = "CanManageInvoiceLineItem";

            // Case closure
            public const string CanCloseCase = "CanCloseCase";

            // Ledger + receipts
            public const string ReadCustomerLedger = "CanReadCustomerLedger";
            public const string RecordPayment = "CanRecordPayment";
            public const string ReadReceipt = "CanReadReceipt";

            // Reports
            public const string ReadAging = "CanReadAgingReport";
            public const string ReadOutstanding = "CanReadOutstandingReport";
            public const string ReadCollectionSummary = "CanReadCollectionSummary";
            public const string ReadCreditStatus = "CanReadCreditStatus";

            // Payments
            public const string ProcessPayment = "CanProcessPayment";
            public const string ValidatePayment = "CanValidatePayment";
            public const string SendPaymentLink = "CanSendPaymentLink";
        }

        public static class InvoiceCase
        {
            public const string Read = "CanReadInvoiceCase";
            public const string Create = "CanCreateInvoiceCase";
            public const string Update = "CanUpdateInvoiceCase";
            public const string Delete = "CanDeleteInvoiceCase";
            public const string Manage = "CanManageInvoiceCase";
        }

        public static class InvoiceCaseConfig
        {
            public const string Read = "CanReadInvoiceCaseConfig";
            public const string Create = "CanCreateInvoiceCaseConfig";
            public const string Update = "CanUpdateInvoiceCaseConfig";
            public const string Delete = "CanDeleteInvoiceCaseConfig";
            public const string Manage = "CanManageInvoiceCaseConfig";
        }

        public static class CustomerPO
        {
            public const string Read = "CanReadCustomerPO";
            public const string Create = "CanCreateCustomerPO";
            public const string Update = "CanUpdateCustomerPO";
            public const string Delete = "CanDeleteCustomerPO";
            public const string Manage = "CanManageCustomerPO";
        }

        public static class CuttingPrice
        {
            public const string Read = "CanReadCuttingPrice";
            public const string Create = "CanCreateCuttingPrice";
            public const string Update = "CanUpdateCuttingPrice";
            public const string Delete = "CanDeleteCuttingPrice";
            public const string Manage = "CanManageCuttingPrice";
        }

        // ═══════════════════════════════════════════════════════
        // NABL — ISO 17025
        // ═══════════════════════════════════════════════════════
        public static class LabScope
        {
            public const string Read = "CanReadLabScopeMaster";
            public const string Create = "CanCreateLabScopeMaster";
            public const string Update = "CanUpdateLabScopeMaster";
            public const string Delete = "CanDeleteLabScopeMaster";
            public const string Manage = "CanManageLabScopeMaster";
        }

        // ═══════════════════════════════════════════════════════
        // ADMIN / SETTINGS
        // ═══════════════════════════════════════════════════════
        public static class Admin
        {
            // Module-level flags (broad)
            public const string Read = "CanReadAdmin";          // list-level read on any admin entity
            public const string Create = "CanCreateAdmin";
            public const string Update = "CanUpdateAdmin";
            public const string Delete = "CanDeleteAdmin";
            public const string Manage = "CanManageAdmin";

            // Granular flags (existing — kept for backward compat)
            public const string ReadConfiguration = "CanReadConfiguration";
            public const string ReadMenuManagement = "CanReadMenuManagement";
            public const string ReadMenuPermission = "CanReadMenuPermission";
            public const string ReadRoleManagement = "CanReadRoleManagement";
            public const string ReadUserPermission = "CanReadUserPermission";
            public const string ReadWorkflow = "CanReadWorkflow";
            public const string ManageUser = "CanManageUser";
            public const string ManageRole = "CanManageRole";
            public const string ManageMenu = "CanManageMenu";
            public const string AssignPermission = "CanAssignPermission";
            public const string ManageSettings = "CanManageSettings";
        }

        public static class Role
        {
            public const string Read = "CanReadRoleManagement";
            public const string Create = "CanCreateRole";
            public const string Update = "CanUpdateRole";
            public const string Delete = "CanDeleteRole";
            public const string Manage = "CanManageRole";
        }

        public static class Menu
        {
            public const string Read = "CanReadMenuManagement";
            public const string Create = "CanCreateMenu";
            public const string Update = "CanUpdateMenu";
            public const string Delete = "CanDeleteMenu";
            public const string Manage = "CanManageMenu";
            public const string AssignPermission = "CanAssignMenuPermission";
        }

        public static class User
        {
            public const string Read = "CanReadUser";
            public const string Create = "CanCreateUser";
            public const string Update = "CanUpdateUser";
            public const string Delete = "CanDeleteUser";
            public const string Manage = "CanManageUser";
            public const string AssignPermission = "CanAssignUserPermission";
            public const string ResetPassword = "CanResetUserPassword";
        }
    }
}
