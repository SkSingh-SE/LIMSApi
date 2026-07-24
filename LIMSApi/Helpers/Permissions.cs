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
    ///   {ENTITY}_{ACTION}  — Workflow transitions (uppercase_snake, kept for back-compat)
    ///
    /// Usage in controllers:
    ///   [RequirePermission(Permissions.Inward.Create)]
    ///   [RequirePermission(Permissions.Testing.VerifyResult)]
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
            public const string Read   = "CanReadDepartment";
            public const string Create = "CanCreateDepartment";
            public const string Update = "CanUpdateDepartment";
            public const string Delete = "CanDeleteDepartment";
            public const string Manage = "CanManageDepartment";
        }

        public static class Employee
        {
            public const string Read   = "CanReadEmployee";
            public const string Create = "CanCreateEmployee";
            public const string Update = "CanUpdateEmployee";
            public const string Delete = "CanDeleteEmployee";
            public const string Manage = "CanManageEmployee";
        }

        public static class Designation
        {
            public const string Read   = "CanReadDesignation";
            public const string Create = "CanCreateDesignation";
            public const string Update = "CanUpdateDesignation";
            public const string Delete = "CanDeleteDesignation";
            public const string Manage = "CanManageDesignation";
        }

        public static class Tax
        {
            public const string Read   = "CanReadTax";
            public const string Create = "CanCreateTax";
            public const string Update = "CanUpdateTax";
            public const string Delete = "CanDeleteTax";
            public const string Manage = "CanManageTax";
        }

        public static class Bank
        {
            public const string Read   = "CanReadBank";
            public const string Create = "CanCreateBank";
            public const string Update = "CanUpdateBank";
            public const string Delete = "CanDeleteBank";
            public const string Manage = "CanManageBank";
        }

        public static class Courier
        {
            public const string Read   = "CanReadCourier";
            public const string Create = "CanCreateCourier";
            public const string Update = "CanUpdateCourier";
            public const string Delete = "CanDeleteCourier";
            public const string Manage = "CanManageCourier";
        }

        public static class ProductSizeMaster
        {
            public const string Read   = "CanReadProductSizeMaster";
            public const string Create = "CanCreateProductSizeMaster";
            public const string Update = "CanUpdateProductSizeMaster";
            public const string Delete = "CanDeleteProductSizeMaster";
            public const string Manage = "CanManageProductSizeMaster";
        }

        public static class ChemicalSampleCategory
        {
            public const string Read   = "CanReadChemicalSampleCategory";
            public const string Create = "CanCreateChemicalSampleCategory";
            public const string Update = "CanUpdateChemicalSampleCategory";
            public const string Delete = "CanDeleteChemicalSampleCategory";
            public const string Manage = "CanManageChemicalSampleCategory";
        }

        public static class AnalysisTechnique
        {
            public const string Read   = "CanReadAnalysisTechnique";
            public const string Create = "CanCreateAnalysisTechnique";
            public const string Update = "CanUpdateAnalysisTechnique";
            public const string Delete = "CanDeleteAnalysisTechnique";
            public const string Manage = "CanManageAnalysisTechnique";
        }

        public static class LaboratoryTestSubType
        {
            public const string Read   = "CanReadLaboratoryTestSubType";
            public const string Create = "CanCreateLaboratoryTestSubType";
            public const string Update = "CanUpdateLaboratoryTestSubType";
            public const string Delete = "CanDeleteLaboratoryTestSubType";
            public const string Manage = "CanManageLaboratoryTestSubType";
        }

        public static class TPI
        {
            public const string Read   = "CanReadTPI";
            public const string Create = "CanCreateTPI";
            public const string Update = "CanUpdateTPI";
            public const string Delete = "CanDeleteTPI";
            public const string Manage = "CanManageTPI";
        }

        public static class Supplier
        {
            public const string Read   = "CanReadSupplier";
            public const string Create = "CanCreateSupplier";
            public const string Update = "CanUpdateSupplier";
            public const string Delete = "CanDeleteSupplier";
            public const string Manage = "CanManageSupplier";
        }

        public static class Equipment
        {
            public const string Read   = "CanReadEquipment";
            public const string Create = "CanCreateEquipment";
            public const string Update = "CanUpdateEquipment";
            public const string Delete = "CanDeleteEquipment";
            public const string Manage = "CanManageEquipment";
        }

        public static class EquipmentType
        {
            public const string Read   = "CanReadEquipmentType";
            public const string Create = "CanCreateEquipmentType";
            public const string Update = "CanUpdateEquipmentType";
            public const string Delete = "CanDeleteEquipmentType";
            public const string Manage = "CanManageEquipmentType";
        }

        public static class EquipmentReferenceMaterial
        {
            public const string Read   = "CanReadEquipmentReferenceMaterial";
            public const string Create = "CanCreateEquipmentReferenceMaterial";
            public const string Update = "CanUpdateEquipmentReferenceMaterial";
            public const string Delete = "CanDeleteEquipmentReferenceMaterial";
            public const string Manage = "CanManageEquipmentReferenceMaterial";
        }

        public static class OEM
        {
            public const string Read   = "CanReadOEM";
            public const string Create = "CanCreateOEM";
            public const string Update = "CanUpdateOEM";
            public const string Delete = "CanDeleteOEM";
            public const string Manage = "CanManageOEM";
        }

        public static class CalibrationAgency
        {
            public const string Read   = "CanReadCalibrationAgency";
            public const string Create = "CanCreateCalibrationAgency";
            public const string Update = "CanUpdateCalibrationAgency";
            public const string Delete = "CanDeleteCalibrationAgency";
            public const string Manage = "CanManageCalibrationAgency";
        }

        public static class Vendor
        {
            public const string Read   = "CanReadVendor";
            public const string Create = "CanCreateVendor";
            public const string Update = "CanUpdateVendor";
            public const string Delete = "CanDeleteVendor";
            public const string Manage = "CanManageVendor";
        }

        public static class Industry
        {
            public const string Read   = "CanReadIndustry";
            public const string Create = "CanCreateIndustry";
            public const string Update = "CanUpdateIndustry";
            public const string Delete = "CanDeleteIndustry";
            public const string Manage = "CanManageIndustry";
        }

        public static class Currency
        {
            public const string Read   = "CanReadCurrency";
            public const string Create = "CanCreateCurrency";
            public const string Update = "CanUpdateCurrency";
            public const string Delete = "CanDeleteCurrency";
            public const string Manage = "CanManageCurrency";
        }

        // ═══════════════════════════════════════════════════════
        // MASTERS — Lookup / Reference (geo + code lists)
        // ═══════════════════════════════════════════════════════
        public static class Country
        {
            public const string Read   = "CanReadCountry";
            public const string Create = "CanCreateCountry";
            public const string Update = "CanUpdateCountry";
            public const string Delete = "CanDeleteCountry";
            public const string Manage = "CanManageCountry";
        }

        public static class State
        {
            public const string Read   = "CanReadState";
            public const string Create = "CanCreateState";
            public const string Update = "CanUpdateState";
            public const string Delete = "CanDeleteState";
            public const string Manage = "CanManageState";
        }

        public static class City
        {
            public const string Read   = "CanReadCity";
            public const string Create = "CanCreateCity";
            public const string Update = "CanUpdateCity";
            public const string Delete = "CanDeleteCity";
            public const string Manage = "CanManageCity";
        }

        public static class Area
        {
            public const string Read   = "CanReadArea";
            public const string Create = "CanCreateArea";
            public const string Update = "CanUpdateArea";
            public const string Delete = "CanDeleteArea";
            public const string Manage = "CanManageArea";
        }

        public static class UniversalCodeType
        {
            public const string Read   = "CanReadUniversalCodeType";
            public const string Create = "CanCreateUniversalCodeType";
            public const string Update = "CanUpdateUniversalCodeType";
            public const string Delete = "CanDeleteUniversalCodeType";
            public const string Manage = "CanManageUniversalCodeType";
        }

        public static class DispatchMode
        {
            public const string Read   = "CanReadDispatchMode";
            public const string Create = "CanCreateDispatchMode";
            public const string Update = "CanUpdateDispatchMode";
            public const string Delete = "CanDeleteDispatchMode";
            public const string Manage = "CanManageDispatchMode";
        }

        public static class Remark
        {
            public const string Read   = "CanReadRemark";
            public const string Create = "CanCreateRemark";
            public const string Update = "CanUpdateRemark";
            public const string Delete = "CanDeleteRemark";
            public const string Manage = "CanManageRemark";
        }

        // ═══════════════════════════════════════════════════════
        // MASTERS — Technical / Lab
        // ═══════════════════════════════════════════════════════
        public static class MaterialSpecification
        {
            public const string Read   = "CanReadMaterialSpecification";
            public const string Create = "CanCreateMaterialSpecification";
            public const string Update = "CanUpdateMaterialSpecification";
            public const string Delete = "CanDeleteMaterialSpecification";
            public const string Manage = "CanManageMaterialSpecification";
        }

        public static class ProductSpecification
        {
            public const string Read   = "CanReadProductSpecification";
            public const string Create = "CanCreateProductSpecification";
            public const string Update = "CanUpdateProductSpecification";
            public const string Delete = "CanDeleteProductSpecification";
            public const string Manage = "CanManageProductSpecification";
        }

        public static class ProductSpecificationGrade
        {
            public const string Read   = "CanReadProductSpecificationGrade";
            public const string Create = "CanCreateProductSpecificationGrade";
            public const string Update = "CanUpdateProductSpecificationGrade";
            public const string Delete = "CanDeleteProductSpecificationGrade";
            public const string Manage = "CanManageProductSpecificationGrade";
        }

        public static class LaboratoryTest
        {
            public const string Read   = "CanReadLaboratoryTest";
            public const string Create = "CanCreateLaboratoryTest";
            public const string Update = "CanUpdateLaboratoryTest";
            public const string Delete = "CanDeleteLaboratoryTest";
            public const string Manage = "CanManageLaboratoryTest";
        }

        public static class TestMethodSpecification
        {
            public const string Read   = "CanReadTestMethodSpecification";
            public const string Create = "CanCreateTestMethodSpecification";
            public const string Update = "CanUpdateTestMethodSpecification";
            public const string Delete = "CanDeleteTestMethodSpecification";
            public const string Manage = "CanManageTestMethodSpecification";
            public const string Import = "CanImportTestMethodSpecification";
        }

        public static class TestMethodStandard
        {
            public const string Read   = "CanReadTestMethodStandard";
            public const string Create = "CanCreateTestMethodStandard";
            public const string Update = "CanUpdateTestMethodStandard";
            public const string Delete = "CanDeleteTestMethodStandard";
            public const string Manage = "CanManageTestMethodStandard";
        }

        public static class Parameter
        {
            public const string ReadChemical  = "CanReadChemicalParameter";
            public const string ReadMechanical = "CanReadMechanicalParameter";
            public const string ReadUnit      = "CanReadParameterUnit";
            public const string Create        = "CanCreateParameter";
            public const string Update        = "CanUpdateParameter";
            public const string Delete        = "CanDeleteParameter";
            public const string Manage        = "CanManageParameter";
        }

        public static class ParameterCategory
        {
            public const string Read   = "CanReadParameterCategory";
            public const string Create = "CanCreateParameterCategory";
            public const string Update = "CanUpdateParameterCategory";
            public const string Delete = "CanDeleteParameterCategory";
            public const string Manage = "CanManageParameterCategory";
        }

        public static class ParameterUnit
        {
            public const string Read   = "CanReadParameterUnit";
            public const string Create = "CanCreateParameterUnit";
            public const string Update = "CanUpdateParameterUnit";
            public const string Delete = "CanDeleteParameterUnit";
            public const string Manage = "CanManageParameterUnit";
        }

        public static class MetalClassification
        {
            public const string Read   = "CanReadMetalClassification";
            public const string Create = "CanCreateMetalClassification";
            public const string Update = "CanUpdateMetalClassification";
            public const string Delete = "CanDeleteMetalClassification";
            public const string Manage = "CanManageMetalClassification";
        }

        public static class HeatTreatment
        {
            public const string Read   = "CanReadHeatTreatment";
            public const string Create = "CanCreateHeatTreatment";
            public const string Update = "CanUpdateHeatTreatment";
            public const string Delete = "CanDeleteHeatTreatment";
            public const string Manage = "CanManageHeatTreatment";
        }

        public static class HeatTreatmentCategory
        {
            public const string Read   = "CanReadHeatTreatmentCategory";
            public const string Create = "CanCreateHeatTreatmentCategory";
            public const string Update = "CanUpdateHeatTreatmentCategory";
            public const string Delete = "CanDeleteHeatTreatmentCategory";
            public const string Manage = "CanManageHeatTreatmentCategory";
        }

        public static class CoolingMedium
        {
            public const string Read   = "CanReadCoolingMedium";
            public const string Create = "CanCreateCoolingMedium";
            public const string Update = "CanUpdateCoolingMedium";
            public const string Delete = "CanDeleteCoolingMedium";
            public const string Manage = "CanManageCoolingMedium";
        }

        public static class DimensionalFactor
        {
            public const string Read   = "CanReadDimensionalFactor";
            public const string Create = "CanCreateDimensionalFactor";
            public const string Update = "CanUpdateDimensionalFactor";
            public const string Delete = "CanDeleteDimensionalFactor";
            public const string Manage = "CanManageDimensionalFactor";
        }

        public static class Discipline
        {
            public const string Read   = "CanReadDiscipline";
            public const string Create = "CanCreateDiscipline";
            public const string Update = "CanUpdateDiscipline";
            public const string Delete = "CanDeleteDiscipline";
            public const string Manage = "CanManageDiscipline";
        }

        public static class ProductForm
        {
            public const string Read   = "CanReadProductForm";
            public const string Create = "CanCreateProductForm";
            public const string Update = "CanUpdateProductForm";
            public const string Delete = "CanDeleteProductForm";
            public const string Manage = "CanManageProductForm";
        }

        public static class ProductCondition
        {
            public const string Read   = "CanReadProductCondition";
            public const string Create = "CanCreateProductCondition";
            public const string Update = "CanUpdateProductCondition";
            public const string Delete = "CanDeleteProductCondition";
            public const string Manage = "CanManageProductCondition";
        }

        public static class ProductConditionCategory
        {
            public const string Read   = "CanReadProductConditionCategory";
            public const string Create = "CanCreateProductConditionCategory";
            public const string Update = "CanUpdateProductConditionCategory";
            public const string Delete = "CanDeleteProductConditionCategory";
            public const string Manage = "CanManageProductConditionCategory";
        }

        public static class SpecimenOrientation
        {
            public const string Read   = "CanReadSpecimenOrientation";
            public const string Create = "CanCreateSpecimenOrientation";
            public const string Update = "CanUpdateSpecimenOrientation";
            public const string Delete = "CanDeleteSpecimenOrientation";
            public const string Manage = "CanManageSpecimenOrientation";
        }

        public static class SpecimenOrientationCategory
        {
            public const string Read   = "CanReadSpecimenOrientationCategory";
            public const string Create = "CanCreateSpecimenOrientationCategory";
            public const string Update = "CanUpdateSpecimenOrientationCategory";
            public const string Delete = "CanDeleteSpecimenOrientationCategory";
            public const string Manage = "CanManageSpecimenOrientationCategory";
        }

        public static class SpecimenType
        {
            public const string Read   = "CanReadSpecimenType";
            public const string Create = "CanCreateSpecimenType";
            public const string Update = "CanUpdateSpecimenType";
            public const string Delete = "CanDeleteSpecimenType";
            public const string Manage = "CanManageSpecimenType";
        }

        public static class PropertyType
        {
            public const string Read   = "CanReadPropertyType";
            public const string Create = "CanCreatePropertyType";
            public const string Update = "CanUpdatePropertyType";
            public const string Delete = "CanDeletePropertyType";
            public const string Manage = "CanManagePropertyType";
        }

        public static class StandardOrganization
        {
            public const string Read   = "CanReadStandardOrganization";
            public const string Create = "CanCreateStandardOrganization";
            public const string Update = "CanUpdateStandardOrganization";
            public const string Delete = "CanDeleteStandardOrganization";
            public const string Manage = "CanManageStandardOrganization";
        }

        public static class Group
        {
            public const string Read   = "CanReadGroup";
            public const string Create = "CanCreateGroup";
            public const string Update = "CanUpdateGroup";
            public const string Delete = "CanDeleteGroup";
            public const string Manage = "CanManageGroup";
        }

        public static class SubGroup
        {
            public const string Read   = "CanReadSubGroup";
            public const string Create = "CanCreateSubGroup";
            public const string Update = "CanUpdateSubGroup";
            public const string Delete = "CanDeleteSubGroup";
            public const string Manage = "CanManageSubGroup";
        }

        public static class Item
        {
            public const string Read   = "CanReadItem";
            public const string Create = "CanCreateItem";
            public const string Update = "CanUpdateItem";
            public const string Delete = "CanDeleteItem";
            public const string Manage = "CanManageItem";
        }

        public static class PriceDimensionType
        {
            public const string Read   = "CanReadPriceDimensionType";
            public const string Create = "CanCreatePriceDimensionType";
            public const string Update = "CanUpdatePriceDimensionType";
            public const string Delete = "CanDeletePriceDimensionType";
            public const string Manage = "CanManagePriceDimensionType";
        }

        public static class SamplePreparationMaster
        {
            public const string Read   = "CanReadSamplePreparationMaster";
            public const string Create = "CanCreateSamplePreparationMaster";
            public const string Update = "CanUpdateSamplePreparationMaster";
            public const string Delete = "CanDeleteSamplePreparationMaster";
            public const string Manage = "CanManageSamplePreparationMaster";
        }

        public static class Customer
        {
            public const string Read   = "CanReadCustomerMaster";
            public const string Create = "CanCreateCustomerMaster";
            public const string Update = "CanUpdateCustomerMaster";
            public const string Delete = "CanDeleteCustomerMaster";
            public const string Manage = "CanManageCustomerMaster";
        }

        public static class CompanyCategory
        {
            public const string Read   = "CanReadCompanyCategory";
            public const string Create = "CanCreateCompanyCategory";
            public const string Update = "CanUpdateCompanyCategory";
            public const string Delete = "CanDeleteCompanyCategory";
            public const string Manage = "CanManageCompanyCategory";
        }

        // ═══════════════════════════════════════════════════════
        // TEST MANAGEMENT
        // ═══════════════════════════════════════════════════════
        public static class TestMaster
        {
            public const string Read   = "CanReadTestMaster";
            public const string Create = "CanCreateTestMaster";
            public const string Update = "CanUpdateTestMaster";
            public const string Delete = "CanDeleteTestMaster";
            public const string Manage = "CanManageTestMaster";
        }

        public static class TestGroup
        {
            public const string Read   = "CanReadTestGroup";
            public const string Create = "CanCreateTestGroup";
            public const string Update = "CanUpdateTestGroup";
            public const string Delete = "CanDeleteTestGroup";
            public const string Manage = "CanManageTestGroup";
        }

        public static class ProductTestGroup
        {
            public const string Read   = "CanReadProductTestGroup";
            public const string Create = "CanCreateProductTestGroup";
            public const string Update = "CanUpdateProductTestGroup";
            public const string Delete = "CanDeleteProductTestGroup";
            public const string Manage = "CanManageProductTestGroup";
        }

        public static class TestAutoSuggest
        {
            public const string Read   = "CanReadTestAutoSuggest";
            public const string Manage = "CanManageTestAutoSuggest";
        }

        // ═══════════════════════════════════════════════════════
        // FLOW STAGES — Sample workflow
        // ═══════════════════════════════════════════════════════
        public static class Inward
        {
            public const string Read   = "CanReadSampleInward";
            public const string Create = "CanCreateSampleInward";
            public const string Update = "CanUpdateSampleInward";
            public const string Delete = "CanDeleteSampleInward";
            public const string Manage = "CanManageSampleInward";
        }

        public static class Plan
        {
            public const string Read    = "CanReadPlan";
            public const string Create  = "CanCreatePlan";
            public const string Update  = "CanUpdatePlan";
            public const string Delete  = "CanDeletePlan";
            public const string Manage  = "CanManagePlan";
            public const string Approve = "CanApprovePlan";
            public const string Reject  = "CanRejectPlan";
        }

        public static class Review
        {
            public const string Read    = "CanReadReview";
            public const string Approve = "CanApproveReview";
            public const string Reject  = "CanRejectReview";
            public const string Manage  = "CanManageReview";
        }

        public static class SamplePreparation
        {
            public const string Read   = "CanReadSampleCutting";
            public const string Create = "CanCreateSampleCutting";
            public const string Update = "CanUpdateSampleCutting";
            public const string Delete = "CanDeleteSampleCutting";
            public const string Manage = "CanManageSampleCutting";
        }

        public static class Testing
        {
            public const string Read          = "CanReadTesting";
            public const string ReadDashboard = "CanReadTestingDashboard";
            public const string ReadResults   = "CanReadTestResults";
            public const string Manage        = "CanManageTesting";
            public const string Perform       = "CanPerformTest";
            public const string SaveResult    = "TEST_RESULT_SAVE";
            public const string VerifyResult  = "TEST_RESULT_VERIFY";
            public const string PriceOverride = "TEST_PRICE_OVERRIDE";
        }

        public static class TpiInspection
        {
            public const string Read   = "CanReadTpiInspection";
            public const string Create = "CanCreateTpiInspection";
            public const string Update = "CanUpdateTpiInspection";
            public const string Delete = "CanDeleteTpiInspection";
            public const string Manage = "CanManageTpiInspection";
            public const string Approve = "CanApproveTpiInspection";
        }

        public static class MachineIntegration
        {
            public const string Read   = "CanReadMachineIntegration";
            public const string Manage = "CanManageMachineIntegration";
        }

        public static class Reporting
        {
            public const string Read         = "CanReadReporting";
            public const string Manage       = "CanManageReporting";
            public const string Approve      = "CanApproveReport";
            public const string Amend        = "CanAmendReport";
            public const string ReadFormat   = "CanReadReportFormat";
            public const string ManageFormat = "CanManageReportFormat";
        }

        public static class ReportTemplate
        {
            public const string Read   = "CanReadReportTemplate";
            public const string Create = "CanCreateReportTemplate";
            public const string Update = "CanUpdateReportTemplate";
            public const string Delete = "CanDeleteReportTemplate";
            public const string Manage = "CanManageReportTemplate";
        }

        public static class ReportFormat
        {
            public const string Read   = "CanReadReportFormat";
            public const string Create = "CanCreateReportFormat";
            public const string Update = "CanUpdateReportFormat";
            public const string Delete = "CanDeleteReportFormat";
            public const string Manage = "CanManageReportFormat";
        }

        // ═══════════════════════════════════════════════════════
        // ACCOUNTS — billing, invoicing, payments, ledger
        // ═══════════════════════════════════════════════════════
        public static class Account
        {
            // Top-level
            public const string Read   = "CanReadAccount";
            public const string Manage = "CanManageAccount";

            // Dashboard + case list
            public const string ReadDashboard    = "CanReadAccountsDashboard";
            public const string ReadCaseAccounts = "CanReadCaseAccounts";

            // Invoice generation
            public const string GeneratePI             = "CanGeneratePI";
            public const string GenerateInvoice        = "CanGenerateInvoice";
            public const string GenerateInvoiceBackend = "INVOICE_GENERATE";
            public const string ManageInvoice          = "CanManageInvoice";

            // Pricing
            public const string CalculatePricing = "CanCalculatePricing";
            public const string ValidatePricing  = "CanValidatePricing";

            // Invoice line items
            public const string ReadInvoiceLineItem   = "CanReadInvoiceLineItem";
            public const string ManageInvoiceLineItem = "CanManageInvoiceLineItem";

            // Case closure
            public const string CanCloseCase = "CanCloseCase";

            // Ledger + receipts
            public const string ReadCustomerLedger = "CanReadCustomerLedger";
            public const string RecordPayment      = "CanRecordPayment";
            public const string ReadReceipt        = "CanReadReceipt";

            // Reports
            public const string ReadAging            = "CanReadAgingReport";
            public const string ReadOutstanding      = "CanReadOutstandingReport";
            public const string ReadCollectionSummary = "CanReadCollectionSummary";
            public const string ReadCreditStatus     = "CanReadCreditStatus";

            // Payments
            public const string ProcessPayment  = "CanProcessPayment";
            public const string ValidatePayment = "CanValidatePayment";
            public const string SendPaymentLink = "CanSendPaymentLink";
        }

        public static class Payment
        {
            public const string Read    = "CanReadPayment";
            public const string Create  = "CanCreatePayment";
            public const string Process = "CanProcessPayment";
            public const string Refund  = "CanRefundPayment";
            public const string Manage  = "CanManagePayment";
        }

        public static class InvoiceCase
        {
            public const string Read   = "CanReadInvoiceCase";
            public const string Create = "CanCreateInvoiceCase";
            public const string Update = "CanUpdateInvoiceCase";
            public const string Delete = "CanDeleteInvoiceCase";
            public const string Manage = "CanManageInvoiceCase";
        }

        public static class InvoiceCaseConfig
        {
            public const string Read   = "CanReadInvoiceCaseConfig";
            public const string Create = "CanCreateInvoiceCaseConfig";
            public const string Update = "CanUpdateInvoiceCaseConfig";
            public const string Delete = "CanDeleteInvoiceCaseConfig";
            public const string Manage = "CanManageInvoiceCaseConfig";
        }

        public static class CustomerPO
        {
            public const string Read   = "CanReadCustomerPO";
            public const string Create = "CanCreateCustomerPO";
            public const string Update = "CanUpdateCustomerPO";
            public const string Delete = "CanDeleteCustomerPO";
            public const string Manage = "CanManageCustomerPO";
        }

        public static class CuttingPrice
        {
            public const string Read   = "CanReadCuttingPrice";
            public const string Create = "CanCreateCuttingPrice";
            public const string Update = "CanUpdateCuttingPrice";
            public const string Delete = "CanDeleteCuttingPrice";
            public const string Manage = "CanManageCuttingPrice";
        }

        public static class GstValidator
        {
            public const string Use = "CanUseGstValidator";
        }

        // ═══════════════════════════════════════════════════════
        // NABL — ISO 17025
        // ═══════════════════════════════════════════════════════
        public static class LabScope
        {
            public const string Read   = "CanReadLabScopeMaster";
            public const string Create = "CanCreateLabScopeMaster";
            public const string Update = "CanUpdateLabScopeMaster";
            public const string Delete = "CanDeleteLabScopeMaster";
            public const string Manage = "CanManageLabScopeMaster";
        }

        public static class Nabl
        {
            public const string Read     = "CanReadNabl";
            public const string Create   = "CanCreateNabl";
            public const string Update   = "CanUpdateNabl";
            public const string Delete   = "CanDeleteNabl";
            public const string Manage   = "CanManageNabl";
            public const string Submit   = "CanSubmitNabl";
            public const string Review   = "CanReviewNabl";
            public const string Approve  = "CanApproveNabl";
            public const string Reject   = "CanRejectNabl";
        }

        // ═══════════════════════════════════════════════════════
        // ADMIN / SETTINGS
        // ═══════════════════════════════════════════════════════
        public static class Admin
        {
            // Module-level flags
            public const string Read   = "CanReadAdmin";
            public const string Create = "CanCreateAdmin";
            public const string Update = "CanUpdateAdmin";
            public const string Delete = "CanDeleteAdmin";
            public const string Manage = "CanManageAdmin";

            // Granular flags (kept for backward compat)
            public const string ReadConfiguration  = "CanReadConfiguration";
            public const string ReadMenuManagement = "CanReadMenuManagement";
            public const string ReadMenuPermission = "CanReadMenuPermission";
            public const string ReadRoleManagement = "CanReadRoleManagement";
            public const string ReadUserPermission = "CanReadUserPermission";
            public const string ReadWorkflow       = "CanReadWorkflow";
            public const string ManageUser         = "CanManageUser";
            public const string ManageRole         = "CanManageRole";
            public const string ManageMenu         = "CanManageMenu";
            public const string AssignPermission   = "CanAssignPermission";
            public const string ManageSettings     = "CanManageSettings";
        }

        public static class Role
        {
            public const string Read   = "CanReadRoleManagement";
            public const string Create = "CanCreateRole";
            public const string Update = "CanUpdateRole";
            public const string Delete = "CanDeleteRole";
            public const string Manage = "CanManageRole";
        }

        public static class Menu
        {
            public const string Read             = "CanReadMenuManagement";
            public const string Create           = "CanCreateMenu";
            public const string Update           = "CanUpdateMenu";
            public const string Delete           = "CanDeleteMenu";
            public const string Manage           = "CanManageMenu";
            public const string AssignPermission = "CanAssignMenuPermission";
        }

        public static class User
        {
            public const string Read             = "CanReadUser";
            public const string Create           = "CanCreateUser";
            public const string Update           = "CanUpdateUser";
            public const string Delete           = "CanDeleteUser";
            public const string Manage           = "CanManageUser";
            public const string AssignPermission = "CanAssignUserPermission";
            public const string ResetPassword    = "CanResetUserPassword";
        }

        public static class Configuration
        {
            public const string Read   = "CanReadConfiguration";
            public const string Update = "CanUpdateConfiguration";
            public const string Manage = "CanManageConfiguration";
        }

        public static class Settings
        {
            public const string Read   = "CanReadSettings";
            public const string Update = "CanUpdateSettings";
            public const string Manage = "CanManageSettings";
        }

        public static class Workflow
        {
            public const string Read   = "CanReadWorkflow";
            public const string Create = "CanCreateWorkflow";
            public const string Update = "CanUpdateWorkflow";
            public const string Delete = "CanDeleteWorkflow";
            public const string Manage = "CanManageWorkflow";
        }

        public static class Dashboard
        {
            public const string Read   = "CanReadDashboard";
            public const string Manage = "CanManageDashboard";
        }

        public static class Notification
        {
            public const string Read    = "CanReadNotification";
            public const string Manage  = "CanManageNotification";
            public const string Send    = "CanSendNotification";
        }

        public static class FileUpload
        {
            public const string Upload = "CanUploadFile";
            public const string Delete = "CanDeleteFile";
            public const string Manage = "CanManageFileUpload";
        }
    }
}