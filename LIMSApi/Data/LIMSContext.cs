using System;
using System.Collections.Generic;
using System.Diagnostics;
using LIMSApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LIMSApi.Data;

public partial class LIMSContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LIMSContext(DbContextOptions<LIMSContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual DbSet<AreaMaster> AreaMasters { get; set; }
    public virtual DbSet<BankMaster> BankMasters { get; set; }
    public virtual DbSet<CalibrationAgencyMaster> CalibrationAgencyMasters { get; set; }
    public virtual DbSet<CityMaster> CityMasters { get; set; }
    public virtual DbSet<ClassificationMaster> ClassificationMasters { get; set; }
    public virtual DbSet<CompanyCategoryMaster> CompanyCategoryMasters { get; set; }
    public virtual DbSet<CompanyMaster> CompanyMasters { get; set; }
    public virtual DbSet<ContactPerson> ContactPersons { get; set; }
    public virtual DbSet<CountryMaster> CountryMasters { get; set; }
    public virtual DbSet<CourierMaster> CourierMasters { get; set; }
    public virtual DbSet<ProductSizeMaster> ProductSizeMasters { get; set; }
    public virtual DbSet<CurrencyMaster> CurrencyMasters { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<CustomerChangeRequest> CustomerChangeRequests { get; set; }
    public virtual DbSet<CustomerCompanyCategory> CustomerCompanyCategories { get; set; }
    public virtual DbSet<CustomerDispatchMode> CustomerDispatchModes { get; set; }
    public virtual DbSet<CuttingPriceMaster> CuttingPriceMasters { get; set; }
    public virtual DbSet<CuttingPriceVersion> CuttingPriceVersions { get; set; }
    public virtual DbSet<Configuration> Configurations { get; set; }
    public virtual DbSet<DepartmentMaster> DepartmentMasters { get; set; }
    public virtual DbSet<DesignationMaster> DesignationMasters { get; set; }
    public virtual DbSet<DimensionalFactorMaster> DimensionalFactorMasters { get; set; }
    public virtual DbSet<DisciplineMaster> DisciplineMasters { get; set; }
    public virtual DbSet<DispatchModeMaster> DispatchModeMasters { get; set; }
    public virtual DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
    public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }
    public virtual DbSet<EmployeeQualification> EmployeeQualifications { get; set; }
    public virtual DbSet<EquipmentMaster> EquipmentMasters { get; set; }
    public virtual DbSet<EquipmentTypeMaster> EquipmentTypeMasters { get; set; }
    public virtual DbSet<GroupMaster> GroupMasters { get; set; }
    public virtual DbSet<HeatTreatmentMaster> HeatTreatmentMasters { get; set; }
    public virtual DbSet<IndustryMaster> IndustryMasters { get; set; }
    public virtual DbSet<InvoiceCaseConfiguration> InvoiceCaseConfigurations { get; set; }
    public virtual DbSet<InvoiceCaseAliasName> InvoiceCaseAliasNames { get; set; }
    public virtual DbSet<InvoiceCase> InvoiceCases { get; set; }
    public virtual DbSet<InvoiceCasePrice> InvoiceCasePrices { get; set; }
    public virtual DbSet<ItemMaster> ItemMasters { get; set; }
    public virtual DbSet<LabScopeMaster> LabScopeMasters { get; set; }
    public DbSet<LabScopeSpecification> LabScopeSpecifications { get; set; }
    public DbSet<LabScopeSpecificationParameter> LabScopeSpecificationParameters { get; set; }
    public DbSet<LabScopeSpecificationParameterEquipment> LabScopeSpecificationParameterEquipments { get; set; }
    public DbSet<LabScopeChangeLog> LabScopeChangeLogs { get; set; }
    public virtual DbSet<MakerMaster> MakerMasters { get; set; }
    public virtual DbSet<MenuMaster> MenuMasters { get; set; }
    public virtual DbSet<MetalClassificationMaster> MetalClassificationMasters { get; set; }
    public virtual DbSet<ParameterCategoryMaster> ParameterCategoryMasters { get; set; }
    public virtual DbSet<HeatTreatmentCategoryMaster> HeatTreatmentCategoryMasters { get; set; }
    public virtual DbSet<CoolingMediumMaster> CoolingMediumMasters { get; set; }
    public virtual DbSet<ParameterSpecimenOrientation> ParameterSpecimenOrientations { get; set; }
    public virtual DbSet<HeatTreatmentMetalClassification> HeatTreatmentMetalClassifications { get; set; }
    public virtual DbSet<ProductFormMaster> ProductFormMasters { get; set; }
    public virtual DbSet<SpecimenOrientationCategoryMaster> SpecimenOrientationCategoryMasters { get; set; }
    public virtual DbSet<ProductConditionCategoryMaster> ProductConditionCategoryMasters { get; set; }
    public virtual DbSet<PropertyTypeMaster> PropertyTypeMasters { get; set; }
    public virtual DbSet<SpecimenOrientationProductForm> SpecimenOrientationProductForms { get; set; }
    public virtual DbSet<SpecimenOrientationMetalClassification> SpecimenOrientationMetalClassifications { get; set; }
    public virtual DbSet<DimensionalFactorProductForm> DimensionalFactorProductForms { get; set; }
    public virtual DbSet<ProductConditionPropertyType> ProductConditionPropertyTypes { get; set; }
    public virtual DbSet<OEMMaster> OEMMasters { get; set; }
    public virtual DbSet<OrganisationMaster> OrganisationMasters { get; set; }
    public virtual DbSet<ParameterMaster> ParameterMasters { get; set; }
    public virtual DbSet<ParameterUnitMaster> ParameterUnitMasters { get; set; }
    public virtual DbSet<ParameterUnitEquivalent> ParameterUnitEquivalents { get; set; }
    public virtual DbSet<HardnessEquivalence> HardnessEquivalences { get; set; }
    public virtual DbSet<ToleranceMaster> ToleranceMasters { get; set; }
    public virtual DbSet<ProductConditionMaster> ProductConditionMasters { get; set; }
    public virtual DbSet<ProductSpecification> ProductSpecifications { get; set; }
    public virtual DbSet<RemarkMaster> RemarkMasters { get; set; }
    public virtual DbSet<RoleMaster> RoleMasters { get; set; }
    public virtual DbSet<RoleMenuMapping> RoleMenuMappings { get; set; }
    public virtual DbSet<SiteActivity> SiteActivities { get; set; }
    public virtual DbSet<SiteError> SiteErrors { get; set; }
    public virtual DbSet<SpecificationHeader> SpecificationHeaders { get; set; }
    public virtual DbSet<SpecificationHeaderParameter> SpecificationHeaderParameters { get; set; }
    public virtual DbSet<SpecificationGrade> SpecificationGrades { get; set; }
    public virtual DbSet<SpecificationLine> SpecificationLines { get; set; }
    public virtual DbSet<SpecificationLineTestMethod> SpecificationLineTestMethods { get; set; }
    public virtual DbSet<SpecimenOrientationMaster> SpecimenOrientationMasters { get; set; }
    public virtual DbSet<SpecimenTypeMaster> SpecimenTypeMasters { get; set; }
    public virtual DbSet<StandardOrganizationMaster> StandardOrganizationMasters { get; set; }
    public virtual DbSet<StateMaster> StateMasters { get; set; }
    public virtual DbSet<SubContractorMaster> SubContractorMasters { get; set; }
    public virtual DbSet<SubGroupMaster> SubGroupMasters { get; set; }
    public virtual DbSet<SupplierMaster> SupplierMasters { get; set; }
    public virtual DbSet<SampleInward> SampleInwards { get; set; }
    public virtual DbSet<SampleInwardDispatchMode> InwardDispatchModes { get; set; }
    public virtual DbSet<SampleInwardContactPerson> InwardContacts { get; set; }
    public virtual DbSet<SampleInwardAddressInfo> InwardAddresses { get; set; }
    public virtual DbSet<SampleDetail> SampleDetails { get; set; }
    public virtual DbSet<SampleStatusHistory> SampleStatusHistories { get; set; }
    public virtual DbSet<SampleAdditionalDetail> SampleAdditionalDetails { get; set; }
    public virtual DbSet<SampleTestPlan> TestPlans { get; set; }
    public virtual DbSet<TaxMaster> TaxMasters { get; set; }
    public virtual DbSet<TestGroup> TestGroups { get; set; }
    public virtual DbSet<TestGroupMapping> TestGroupMappings { get; set; }
    public virtual DbSet<TestMaster> TestMasters { get; set; }
    public virtual DbSet<LaboratoryTest> LaboratoryTests { get; set; }
    public virtual DbSet<LaboratoryTestInvoiceCase> LaboratoryTestInvoiceCase { get; set; }
    public virtual DbSet<TestMethodSubGroup> TestMethodSubGroups { get; set; }
    public virtual DbSet<TestMethodSpecification> TestMethodSpecifications { get; set; }
    public virtual DbSet<TestMethodSpecificationVersion> TestMethodSpecificationVersions { get; set; }
    public virtual DbSet<TestTypeMaster> TestTypeMasters { get; set; }
    public virtual DbSet<TPIMaster> TPIMasters { get; set; }
    public virtual DbSet<UniversalCodeTypeMaster> UniversalCodeTypeMasters { get; set; }
    public virtual DbSet<UOMMaster> UOMMasters { get; set; }
    public virtual DbSet<UploadFile> UploadFiles { get; set; }
    public virtual DbSet<UserMaster> UserMasters { get; set; }
    public virtual DbSet<VendorMaster> VendorMasters { get; set; }
    public virtual DbSet<PermissionMaster> PermissionMasters { get; set; }
    public virtual DbSet<UserPermission> UserPermissions { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<Workflow> Workflows { get; set; }
    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }
    public virtual DbSet<WorkflowTransition> WorkflowTransitions { get; set; }
    public virtual DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public virtual DbSet<WorkflowActionLog> WorkflowActionLogs { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<UserPushSubscription> UserPushSubscriptions { get; set; }
    public virtual DbSet<JobExecutionLog> JobExecutionLogs { get; set; }


    public DbSet<CuttingChargeHeader> CuttingChargeHeaders { get; set; }
    public DbSet<CuttingChargeSample> CuttingChargeSamples { get; set; }
    public DbSet<CuttingChargeDetail> CuttingChargeDetails { get; set; }
    public DbSet<MachiningChargeItem> MachiningChargeItems { get; set; }
    public DbSet<MachiningChargeMaster> MachiningChargeMasters { get; set; }
    public DbSet<MachiningChargeVersion> MachiningChargeVersions { get; set; }
    public DbSet<SamplePreparationMaster> SamplePreparationMasters { get; set; }
    public DbSet<SamplePreparation> SamplePreparations { get; set; }
    public DbSet<ProductTestGroup> ProductTestGroups { get; set; }
    public DbSet<ProductSpecificationGrade> ProductSpecificationGrades { get; set; }

    public DbSet<ProformaInvoiceHeader> ProformaInvoiceHeader { get; set; }
    public DbSet<ProformaInvoiceDetail> ProformaInvoiceDetails { get; set; }
    public DbSet<TestResultHeader> TestResultHeaders { get; set; }
    public DbSet<TestResultParameter> TestResultParameters { get; set; }
    public DbSet<TestResultImage> TestResultImages { get; set; }
    public DbSet<GeneralTest> GeneralTests { get; set; }
    public DbSet<GeneralTestMethod> GeneralTestMethods { get; set; }
    public DbSet<ChemicalTest> ChemicalTests { get; set; }
    public DbSet<ChemicalTestElement> ChemicalTestElements { get; set; }
    public DbSet<ChemicalTestType> ChemicalTestTypes { get; set; }
    public DbSet<LongTermTest> LongTermTests { get; set; }
    public DbSet<LongTermRecord> LongTermRecords { get; set; }

    public DbSet<ReportHeader> ReportHeaders { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<ReportBlock> ReportBlocks { get; set; }
    public DbSet<ReportTemplate> ReportTemplates { get; set; }
    public DbSet<ReportTemplateBlock> ReportTemplateBlocks { get; set; }
    public DbSet<AmendmentRequest> AmendmentRequests { get; set; }

    public DbSet<PaymentOrder> PaymentOrders { get; set; }
    public DbSet<ReportAmendmentToken> ReportAmendmentTokens { get; set; }

    // ── Dynamic Report Format System (parallel to existing reporting) ──
    public DbSet<ReportFormat> ReportFormats { get; set; }
    public DbSet<ReportFormatSection> ReportFormatSections { get; set; }
    public DbSet<ReportFormatMapping> ReportFormatMappings { get; set; }
    public DbSet<GeneratedReport> GeneratedReports { get; set; }
    public DbSet<CustomerAmendment> CustomerAmendments { get; set; }
    public DbSet<TaxInvoice> TaxInvoices { get; set; }
    public DbSet<ChargeEvent> ChargeEvents { get; set; }
    public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
    public DbSet<CreditNote> CreditNotes { get; set; }
    public DbSet<DebitNote> DebitNotes { get; set; }
    public DbSet<AdvancePaymentVoucher> AdvancePaymentVouchers { get; set; }
    public DbSet<Quotation> Quotations { get; set; }
    public DbSet<QuotationItem> QuotationItems { get; set; }
    public DbSet<MessageTemplate> MessageTemplates { get; set; }

    /* ============================
           SETTINGS MODULE
           ============================ */

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<NablAccreditation> NablAccreditations => Set<NablAccreditation>();
    public DbSet<NablJobDescription> NablJobDescriptions => Set<NablJobDescription>();
    public DbSet<NablFormRevisionHistory> NablFormRevisionHistory => Set<NablFormRevisionHistory>();
    public DbSet<NablAuditLog> NablAuditLogs => Set<NablAuditLog>();
    public DbSet<NablFormAttachment> NablFormAttachments => Set<NablFormAttachment>();
    public DbSet<NablResponsibilityAuthority> NablResponsibilityAuthorities => Set<NablResponsibilityAuthority>();
    public DbSet<NablEmployeeCompetence> NablEmployeeCompetences => Set<NablEmployeeCompetence>();
    public DbSet<NablEmployeePerformanceRecord> NablEmployeePerformanceRecords => Set<NablEmployeePerformanceRecord>();
    public DbSet<NablEmployeeAuthorization> NablEmployeeAuthorizations => Set<NablEmployeeAuthorization>();
    public DbSet<NablCompetenceRequirement> NablCompetenceRequirements => Set<NablCompetenceRequirement>();
    public DbSet<NablInductionTraining> NablInductionTrainings => Set<NablInductionTraining>();
    public DbSet<NablSkillMatrix> NablSkillMatrices => Set<NablSkillMatrix>();
    public DbSet<NablSkillMatrixDecision> NablSkillMatrixDecisions => Set<NablSkillMatrixDecision>();
    public DbSet<NablTrainingPlan> NablTrainingPlans => Set<NablTrainingPlan>();
    public DbSet<NablTrainingAttendance> NablTrainingAttendances => Set<NablTrainingAttendance>();
    public DbSet<NablTrainingEffectiveness> NablTrainingEffectivenesses => Set<NablTrainingEffectiveness>();
    public DbSet<NablEnvironmentMonitoring> NablEnvironmentMonitorings => Set<NablEnvironmentMonitoring>();
    public DbSet<EnvironmentDailyRecord> EnvironmentDailyRecords { get; set; }
    public DbSet<LabRoom> LabRooms { get; set; }
    public DbSet<NablQualityControlPlan> NablQualityControlPlans => Set<NablQualityControlPlan>();
    public DbSet<NablTestRequest> NablTestRequests => Set<NablTestRequest>();
    public DbSet<NablTestMethod> NablTestMethods => Set<NablTestMethod>();
    public DbSet<NablMethodVerification> NablMethodVerifications => Set<NablMethodVerification>();
    public DbSet<NablMethodValidation> NablMethodValidations => Set<NablMethodValidation>();
    public DbSet<NablSampleInwardRegister> NablSampleInwardRegisters => Set<NablSampleInwardRegister>();
    public DbSet<NablSampleMusterRegister> NablSampleMusterRegisters => Set<NablSampleMusterRegister>();
    public DbSet<NablSampleLabel> NablSampleLabels => Set<NablSampleLabel>();
    public DbSet<NablTechnicalRawData> NablTechnicalRawDatas => Set<NablTechnicalRawData>();
    public DbSet<NablTestReport> NablTestReports => Set<NablTestReport>();
    public DbSet<NablEquipmentHistory> NablEquipmentHistories => Set<NablEquipmentHistory>();
    public DbSet<NablCalibrationReview> NablCalibrationReviews => Set<NablCalibrationReview>();
    public DbSet<NablIntermediateCheck> NablIntermediateChecks => Set<NablIntermediateCheck>();
    public DbSet<NablReferenceMaterial> NablReferenceMaterials => Set<NablReferenceMaterial>();
    public DbSet<NablCrmConsumption> NablCrmConsumptions => Set<NablCrmConsumption>();
    public DbSet<NablSupplierRegistration> NablSupplierRegistrations => Set<NablSupplierRegistration>();
    public DbSet<NablSupplierEvaluation> NablSupplierEvaluations => Set<NablSupplierEvaluation>();
    public DbSet<NablApprovedSupplier> NablApprovedSuppliers => Set<NablApprovedSupplier>();
    public DbSet<NablSupplierConfidentiality> NablSupplierConfidentialities => Set<NablSupplierConfidentiality>();
    public DbSet<NablIncomingMaterial> NablIncomingMaterials => Set<NablIncomingMaterial>();
    public DbSet<NablProductInspection> NablProductInspections => Set<NablProductInspection>();
    public DbSet<NablPurchaseIndent> NablPurchaseIndents => Set<NablPurchaseIndent>();
    public DbSet<NablPurchaseOrder> NablPurchaseOrders => Set<NablPurchaseOrder>();
    public DbSet<NablPurchaseMaterialVerification> NablPurchaseMaterialVerifications => Set<NablPurchaseMaterialVerification>();
    public DbSet<NablComplaint> NablComplaints => Set<NablComplaint>();
    public DbSet<NablCustomerFeedback> NablCustomerFeedbacks => Set<NablCustomerFeedback>();
    public DbSet<NablFeedbackAnalysis> NablFeedbackAnalyses => Set<NablFeedbackAnalysis>();
    public DbSet<NablAuditPlan> NablAuditPlans => Set<NablAuditPlan>();
    public DbSet<NablAuditChecklist> NablAuditChecklists => Set<NablAuditChecklist>();
    public DbSet<NablAuditSummary> NablAuditSummaries => Set<NablAuditSummary>();
    public DbSet<NablInternalAuditor> NablInternalAuditors => Set<NablInternalAuditor>();
    public DbSet<NablMeetingAgenda> NablMeetingAgendas => Set<NablMeetingAgenda>();
    public DbSet<NablMeetingMinutes> NablMeetingMinutes => Set<NablMeetingMinutes>();
    public DbSet<NablNonConformingWork> NablNonConformingWorks => Set<NablNonConformingWork>();
    public DbSet<NablNcCorrectiveAction> NablNcCorrectiveActions => Set<NablNcCorrectiveAction>();
    public DbSet<NablRetesting> NablRetestings => Set<NablRetesting>();
    public DbSet<NablRiskAssessment> NablRiskAssessments => Set<NablRiskAssessment>();
    public DbSet<NablDocumentChangeRequest> NablDocumentChangeRequests => Set<NablDocumentChangeRequest>();
    public DbSet<NablDocumentReview> NablDocumentReviews => Set<NablDocumentReview>();
    public DbSet<NablMasterDocument> NablMasterDocuments => Set<NablMasterDocument>();
    public DbSet<NablMeasurementUncertainty> NablMeasurementUncertainties => Set<NablMeasurementUncertainty>();
    public DbSet<NablPtIlcPlan> NablPtIlcPlans => Set<NablPtIlcPlan>();
    public DbSet<NumberingConfig> NumberingConfigs => Set<NumberingConfig>();
    public DbSet<GstConfig> GstConfigs => Set<GstConfig>();
    public DbSet<FinancialYear> FinancialYears => Set<FinancialYear>();
    public DbSet<FinancialYearChangeLog> FinancialYearChangeLogs => Set<FinancialYearChangeLog>();
    public DbSet<AuthorizedSignatory> AuthorizedSignatories => Set<AuthorizedSignatory>();

    public DbSet<CustomerLedger> CustomerLedgers { get; set; }
    public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
    public DbSet<CustomerPurchaseOrder> CustomerPurchaseOrders { get; set; }
    public DbSet<PriceDimensionType> PriceDimensionTypes { get; set; }
    public DbSet<MachineDataLog> MachineDataLogs { get; set; }

    public DbSet<PlanHistory> PlanHistories { get; set; }
    public DbSet<ReplanRequest> ReplanRequests { get; set; }
    public DbSet<TestUsageStats> TestUsageStats { get; set; }
    public DbSet<TpiInspection> TpiInspections { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public DbSet<EquipmentReferenceMaterial> EquipmentReferenceMaterials { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the BankName= syntax to read it from _configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=LIMS_Backup;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // RolePermission — one (Role, Permission) pair only when active
        modelBuilder.Entity<RolePermission>()
            .HasIndex(x => new { x.RoleID, x.PermissionID })
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("IX_RolePermission_Role_Permission");

        modelBuilder.Entity<RolePermission>()
            .HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RolePermission>()
            .HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MetalClassificationParameter>().HasKey(x => new { x.MetalClassificationID, x.ParameterID });

        modelBuilder.Entity<ParameterSpecimenOrientation>().HasKey(x => new { x.ParameterID, x.SpecimenOrientationID });

        modelBuilder.Entity<HeatTreatmentMetalClassification>().HasKey(x => new { x.HeatTreatmentID, x.MetalClassificationID });

        // MetalClassificationMaster self-referencing hierarchy
        modelBuilder.Entity<MetalClassificationMaster>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MetalClassificationMaster>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [Code] IS NOT NULL")
            .HasDatabaseName("IX_MetalClassificationMaster_Code");

        modelBuilder.Entity<ParameterMaster>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [Code] IS NOT NULL")
            .HasDatabaseName("IX_ParameterMaster_Code");

        modelBuilder.Entity<HeatTreatmentMaster>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [Code] IS NOT NULL")
            .HasDatabaseName("IX_HeatTreatmentMaster_Code");

        // HeatTreatmentMaster FK configs
        modelBuilder.Entity<HeatTreatmentMaster>()
            .HasOne(x => x.HeatTreatmentCategory)
            .WithMany()
            .HasForeignKey(x => x.HeatTreatmentCategoryID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HeatTreatmentMaster>()
            .HasOne(x => x.CoolingMedium)
            .WithMany()
            .HasForeignKey(x => x.CoolingMediumID)
            .OnDelete(DeleteBehavior.NoAction);

        // ParameterMaster FK configs
        modelBuilder.Entity<ParameterMaster>()
            .HasOne(x => x.DefaultTestMethod)
            .WithMany()
            .HasForeignKey(x => x.DefaultTestMethodID)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ParameterMaster>()
            .HasOne(x => x.ParameterCategory)
            .WithMany()
            .HasForeignKey(x => x.ParameterCategoryID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ParameterMaster>()
            .HasOne(x => x.ParameterUnit)
            .WithMany()
            .HasForeignKey(x => x.ParameterUnitID)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Phase 2: Junction table composite keys
        modelBuilder.Entity<SpecimenOrientationProductForm>().HasKey(x => new { x.SpecimenOrientationID, x.ProductFormID });
        modelBuilder.Entity<SpecimenOrientationMetalClassification>().HasKey(x => new { x.SpecimenOrientationID, x.MetalClassificationID });
        modelBuilder.Entity<DimensionalFactorProductForm>().HasKey(x => new { x.DimensionalFactorID, x.ProductFormID });
        modelBuilder.Entity<ProductConditionPropertyType>().HasKey(x => new { x.ProductConditionID, x.PropertyTypeID });

        // Phase 2: Unique indexes on Code
        modelBuilder.Entity<SpecimenOrientationMaster>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [Code] IS NOT NULL")
            .HasDatabaseName("IX_SpecimenOrientationMaster_Code");

        modelBuilder.Entity<ProductConditionMaster>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [Code] IS NOT NULL")
            .HasDatabaseName("IX_ProductConditionMaster_Code");

        modelBuilder.Entity<DimensionalFactorMaster>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [Code] IS NOT NULL")
            .HasDatabaseName("IX_DimensionalFactorMaster_Code");

        // Phase 2: SpecimenOrientationMaster FK
        modelBuilder.Entity<SpecimenOrientationMaster>()
            .HasOne(x => x.SpecimenOrientationCategory)
            .WithMany()
            .HasForeignKey(x => x.SpecimenOrientationCategoryID)
            .OnDelete(DeleteBehavior.NoAction);

        // Phase 2: ProductConditionMaster FKs
        modelBuilder.Entity<ProductConditionMaster>()
            .HasOne(x => x.ProductConditionCategory)
            .WithMany()
            .HasForeignKey(x => x.ProductConditionCategoryID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ProductConditionMaster>()
            .HasOne(x => x.LinkedHeatTreatment)
            .WithMany()
            .HasForeignKey(x => x.LinkedHeatTreatmentID)
            .OnDelete(DeleteBehavior.NoAction);

        // Phase 2: DimensionalFactorMaster FKs
        modelBuilder.Entity<DimensionalFactorMaster>()
            .HasOne(x => x.DefaultTestMethod)
            .WithMany()
            .HasForeignKey(x => x.DefaultTestMethodID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DimensionalFactorMaster>()
            .HasOne(x => x.ParameterUnit)
            .WithMany()
            .HasForeignKey(x => x.ParameterUnitID)
            .OnDelete(DeleteBehavior.NoAction);

        // MessageTemplate unique constraint
        modelBuilder.Entity<MessageTemplate>()
            .HasIndex(t => new { t.TemplateKey, t.Type, t.Version })
            .IsUnique()
            .HasDatabaseName("IX_MessageTemplate_TemplateKey_Channel_Version");

        // Workflow → Steps (keep cascade)
        modelBuilder.Entity<WorkflowStep>()
            .HasOne(s => s.Workflow)
            .WithMany(w => w.Steps)
            .HasForeignKey(s => s.WorkflowID)
            .OnDelete(DeleteBehavior.Cascade);

        // Workflow → Instances (disable cascade)
        modelBuilder.Entity<WorkflowInstance>()
            .HasOne(i => i.Workflow)
            .WithMany()
            .HasForeignKey(i => i.WorkflowID)
            .OnDelete(DeleteBehavior.NoAction);

        // Step → CurrentStep (disable cascade)
        modelBuilder.Entity<WorkflowInstance>()
            .HasOne(i => i.CurrentStep)
            .WithMany()
            .HasForeignKey(i => i.CurrentStepID)
            .OnDelete(DeleteBehavior.NoAction);

        // Workflow → ActionLogs (disable cascade)
        modelBuilder.Entity<WorkflowActionLog>()
            .HasOne(l => l.Workflow)
            .WithMany()
            .HasForeignKey(l => l.WorkflowID)
            .OnDelete(DeleteBehavior.NoAction);

        // Step → ActionLogs (disable cascade)
        modelBuilder.Entity<WorkflowActionLog>()
            .HasOne(l => l.WorkflowStep)
            .WithMany()
            .HasForeignKey(l => l.StepID)
            .OnDelete(DeleteBehavior.NoAction);

        // --------------------------------------------------
        // SampleDetail → TestResultHeader
        // ❌ NO CASCADE (prevents multiple cascade paths)
        // --------------------------------------------------
        modelBuilder.Entity<TestResultHeader>()
            .HasOne(trh => trh.Sample)
            .WithMany() // SampleDetail does not need navigation here
            .HasForeignKey(trh => trh.SampleID)
            .OnDelete(DeleteBehavior.Restrict); // or NoAction

        // Prevent duplicate headers for same sample + test + plan (race condition guard)
        // Quantity > 1 creates multiple headers intentionally via AutoCreateHeadersFromPlanAsync,
        // but the basic auto-create (from UI load) should never duplicate.
        // Using filtered index on IsActive to allow soft-deleted duplicates.
        modelBuilder.Entity<TestResultHeader>()
            .HasIndex(h => new { h.SampleID, h.LaboratoryTestID, h.TestPlanID })
            .HasFilter("[IsActive] = 1")
            .IsUnique(false); // Non-unique because Quantity > 1 creates multiple intentionally

        // --------------------------------------------------
        // SampleDetail → Report
        // ❌ NO CASCADE (workflow-controlled delete)
        // --------------------------------------------------
        modelBuilder.Entity<ReportHeader>()
            .HasOne(rh => rh.Sample)
            .WithMany()
            .HasForeignKey(rh => rh.SampleID)
            .OnDelete(DeleteBehavior.Restrict);

        // --------------------------------------------------
        // Report → Report
        // ✅ CASCADE (version lifecycle)
        // --------------------------------------------------
        modelBuilder.Entity<Report>()
            .HasOne(r => r.ReportHeader)
            .WithMany(h => h.Reports)
            .HasForeignKey(r => r.ReportHeaderID)
            .OnDelete(DeleteBehavior.Cascade);

        // --------------------------------------------------
        // Report → ReportBlock
        // ✅ CASCADE
        // --------------------------------------------------
        modelBuilder.Entity<ReportBlock>()
            .HasOne(rb => rb.Report)
            .WithMany(r => r.Blocks)
            .HasForeignKey(rb => rb.ReportID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LongTermTest>()
        .HasOne(l => l.Sample)
        .WithMany()
        .HasForeignKey(l => l.SampleID)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SampleTestPlan>()
        .HasOne(p => p.SampleDetail)
        .WithMany(s => s.TestPlans)
        .HasForeignKey(p => p.SampleID)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GeneralTest>()
        .HasOne(g => g.SampleTestPlan)
        .WithMany(p => p.GeneralTests)
        .HasForeignKey(g => g.SampleTestPlanID)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GeneralTestMethod>()
        .HasOne(m => m.GeneralTest)
        .WithMany(g => g.Methods)
        .HasForeignKey(m => m.GeneralTestID)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChemicalTest>()
        .HasOne(c => c.SampleTestPlan)
        .WithMany(p => p.ChemicalTests)
        .HasForeignKey(c => c.SampleTestPlanID)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChemicalTestElement>()
        .HasOne(e => e.ChemicalTest)
        .WithMany(c => c.Elements)
        .HasForeignKey(e => e.ChemicalTestID)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChemicalTestType>()
        .HasOne(t => t.ChemicalTest)
        .WithMany(c => c.TestTypes)
        .HasForeignKey(t => t.ChemicalTestID)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlanHistory>()
        .HasOne(h => h.SampleTestPlan)
        .WithMany(p => p.Histories)
        .HasForeignKey(h => h.PlanId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReplanRequest>()
        .HasOne(r => r.SampleTestPlan)
        .WithMany(p => p.ReplanRequests)
        .HasForeignKey(r => r.PlanId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaxInvoice>()
    .HasOne(x => x.Inward)
    .WithMany()
    .HasForeignKey(x => x.InwardID)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaxInvoice>()
    .HasOne(x => x.Customer)
    .WithMany()
    .HasForeignKey(x => x.CustomerID)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentOrder>()
    .HasOne(x => x.Customer)
    .WithMany()
    .HasForeignKey(x => x.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentOrder>()
    .HasOne(x => x.TaxInvoice)
    .WithMany()
    .HasForeignKey(x => x.TaxInvoiceID)
    .OnDelete(DeleteBehavior.Restrict);

        // CustomerLedger relationships
        modelBuilder.Entity<CustomerLedger>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustomerLedger>()
            .HasOne(x => x.Inward)
            .WithMany()
            .HasForeignKey(x => x.InwardId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustomerLedger>()
            .HasOne(x => x.Invoice)
            .WithMany()
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // PaymentReceipt relationships
        modelBuilder.Entity<PaymentReceipt>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentReceipt>()
            .HasIndex(x => x.ReceiptNo)
            .IsUnique();

        // CustomerPurchaseOrder relationships
        modelBuilder.Entity<CustomerPurchaseOrder>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaxInvoice>()
            .HasOne(x => x.PurchaseOrder)
            .WithMany(po => po.Invoices)
            .HasForeignKey(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SampleInward>()
            .HasOne(x => x.PurchaseOrder)
            .WithMany()
            .HasForeignKey(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustomerAmendment>()
    .HasOne(x => x.Report)
    .WithMany()
    .HasForeignKey(x => x.ReportID)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustomerAmendment>()
    .HasOne(x => x.Token)
    .WithMany()
    .HasForeignKey(x => x.TokenID)
    .OnDelete(DeleteBehavior.Restrict);

        // SpecificationHeaderParameter → SpecificationHeader (cascade with header)
        modelBuilder.Entity<SpecificationHeaderParameter>()
            .HasOne(x => x.SpecificationHeader)
            .WithMany(h => h.HeaderParameters)
            .HasForeignKey(x => x.SpecificationHeaderID)
            .OnDelete(DeleteBehavior.Cascade);
        // SpecificationHeaderParameter → Parameter / ParameterUnit (no cascade)
        modelBuilder.Entity<SpecificationHeaderParameter>()
            .HasOne(x => x.Parameter)
            .WithMany()
            .HasForeignKey(x => x.ParameterID)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<SpecificationHeaderParameter>()
            .HasOne(x => x.ParameterUnit)
            .WithMany()
            .HasForeignKey(x => x.ParameterUnitID)
            .OnDelete(DeleteBehavior.NoAction);
        // SpecificationLine → ProductSizeMaster (MS-D, no cascade)
        modelBuilder.Entity<SpecificationLine>()
            .HasOne(x => x.ProductSizeMaster)
            .WithMany()
            .HasForeignKey(x => x.ProductSizeMasterID)
            .OnDelete(DeleteBehavior.NoAction);

        // ParameterUnitEquivalent → ParameterUnitMaster (cascade with base unit)
        modelBuilder.Entity<ParameterUnitEquivalent>()
            .HasOne(x => x.BaseParameterUnit)
            .WithMany(u => u.Equivalents)
            .HasForeignKey(x => x.BaseParameterUnitID)
            .OnDelete(DeleteBehavior.Cascade);

        // Phase 2: SpecificationLine / SpecificationHeaderParameter → ParameterUnitEquivalent (no cascade)
        modelBuilder.Entity<SpecificationLine>()
            .HasOne(x => x.ParameterUnitEquivalent)
            .WithMany()
            .HasForeignKey(x => x.ParameterUnitEquivalentID)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<SpecificationLine>()
            .HasOne(x => x.LaboratoryTest)
            .WithMany()
            .HasForeignKey(x => x.LaboratoryTestID)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<SpecificationHeaderParameter>()
            .HasOne(x => x.ParameterUnitEquivalent)
            .WithMany()
            .HasForeignKey(x => x.ParameterUnitEquivalentID)
            .OnDelete(DeleteBehavior.NoAction);

        // MS-E: SpecificationLineTestMethod → SpecificationLine (cascade with line)
        modelBuilder.Entity<SpecificationLineTestMethod>()
            .HasOne(x => x.SpecificationLine)
            .WithMany(l => l.TestMethodMappings)
            .HasForeignKey(x => x.SpecificationLineID)
            .OnDelete(DeleteBehavior.Cascade);
        // MS-E: SpecificationLineTestMethod → LaboratoryTest / TestMethodSpecification (no cascade)
        modelBuilder.Entity<SpecificationLineTestMethod>()
            .HasOne(x => x.LaboratoryTest)
            .WithMany()
            .HasForeignKey(x => x.LaboratoryTestID)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<SpecificationLineTestMethod>()
            .HasOne(x => x.TestMethodSpecification)
            .WithMany()
            .HasForeignKey(x => x.TestMethodSpecificationID)
            .OnDelete(DeleteBehavior.NoAction);

        // ProductSpecificationGrade → SpecificationGrade (no cascade)
        modelBuilder.Entity<ProductSpecificationGrade>()
            .HasOne(x => x.SpecificationGrade)
            .WithMany()
            .HasForeignKey(x => x.SpecificationGradeID)
            .OnDelete(DeleteBehavior.NoAction);

        // ProductSpecificationGrade → ProductSpecification (no cascade)
        modelBuilder.Entity<ProductSpecificationGrade>()
            .HasOne(x => x.ProductSpecification)
            .WithMany(p => p.ProductSpecificationGrades)
            .HasForeignKey(x => x.ProductSpecificationID)
            .OnDelete(DeleteBehavior.NoAction);

        // ProductTestGroup → ProductSpecification (no cascade)
        modelBuilder.Entity<ProductTestGroup>()
            .HasOne(x => x.ProductSpecification)
            .WithMany(p => p.ProductTestGroups)
            .HasForeignKey(x => x.ProductSpecificationID)
            .OnDelete(DeleteBehavior.NoAction);

        // ProductSpecification → TestMethodSpecificationVersion (no cascade, nullable)
        modelBuilder.Entity<ProductSpecification>()
            .HasOne(x => x.TestMethodSpecificationVersion)
            .WithMany()
            .HasForeignKey(x => x.TestMethodSpecificationVersionID)
            .OnDelete(DeleteBehavior.NoAction);

        // LabScopeSpecification → TestMethodSpecificationVersion (no cascade, nullable)
        modelBuilder.Entity<LabScopeSpecification>()
            .HasOne(x => x.TestMethodSpecificationVersion)
            .WithMany()
            .HasForeignKey(x => x.TestMethodSpecificationVersionID)
            .OnDelete(DeleteBehavior.NoAction);

        // ProductTestGroup → LaboratoryTest (no cascade)
        modelBuilder.Entity<ProductTestGroup>()
            .HasOne(x => x.LaboratoryTest)
            .WithMany()
            .HasForeignKey(x => x.LaboratoryTestID)
            .OnDelete(DeleteBehavior.NoAction);

        // ProductTestGroup → TestMethodSpecification (no cascade, nullable)
        modelBuilder.Entity<ProductTestGroup>()
            .HasOne(x => x.TestMethodSpecification)
            .WithMany()
            .HasForeignKey(x => x.TestMethodStandardID)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        // TpiInspection FKs (no cascade)
        modelBuilder.Entity<TpiInspection>()
            .HasOne(x => x.SampleInward)
            .WithMany()
            .HasForeignKey(x => x.SampleInwardID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TpiInspection>()
            .HasOne(x => x.SampleDetail)
            .WithMany()
            .HasForeignKey(x => x.SampleDetailID)
            .OnDelete(DeleteBehavior.NoAction);

        // InvoiceLineItem FKs (no cascade)
        modelBuilder.Entity<InvoiceLineItem>()
            .HasOne(x => x.ProformaInvoiceHeader)
            .WithMany(p => p.LineItems)
            .HasForeignKey(x => x.ProformaInvoiceHeaderID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<InvoiceLineItem>()
            .HasOne(x => x.TaxInvoice)
            .WithMany()
            .HasForeignKey(x => x.TaxInvoiceID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<InvoiceLineItem>()
            .HasOne(x => x.SampleInward)
            .WithMany()
            .HasForeignKey(x => x.SampleInwardID)
            .OnDelete(DeleteBehavior.NoAction);

        // CreditNote / DebitNote FKs (no cascade)
        modelBuilder.Entity<CreditNote>()
            .HasOne(x => x.TaxInvoice)
            .WithMany()
            .HasForeignKey(x => x.TaxInvoiceID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CreditNote>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DebitNote>()
            .HasOne(x => x.TaxInvoice)
            .WithMany()
            .HasForeignKey(x => x.TaxInvoiceID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DebitNote>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerID)
            .OnDelete(DeleteBehavior.NoAction);

        // AdvancePaymentVoucher FKs (no cascade)
        modelBuilder.Entity<AdvancePaymentVoucher>()
            .HasOne(x => x.Customer).WithMany()
            .HasForeignKey(x => x.CustomerID).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<AdvancePaymentVoucher>()
            .HasOne(x => x.Inward).WithMany()
            .HasForeignKey(x => x.InwardID).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<AdvancePaymentVoucher>()
            .HasOne(x => x.AdjustedToInvoice).WithMany()
            .HasForeignKey(x => x.AdjustedToInvoiceID).OnDelete(DeleteBehavior.NoAction);

        // Quotation FKs (no cascade)
        modelBuilder.Entity<Quotation>()
            .HasOne(x => x.Customer).WithMany()
            .HasForeignKey(x => x.CustomerID).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Quotation>()
            .HasOne(x => x.ConvertedToInward).WithMany()
            .HasForeignKey(x => x.ConvertedToInwardID).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<QuotationItem>()
            .HasOne(x => x.Quotation).WithMany(q => q.Items)
            .HasForeignKey(x => x.QuotationID).OnDelete(DeleteBehavior.Cascade);

        // LabRoom + EnvironmentDailyRecord FKs
        modelBuilder.Entity<EnvironmentDailyRecord>()
            .HasOne(x => x.EnvironmentMonitoring).WithMany(m => m.DailyRecords)
            .HasForeignKey(x => x.EnvironmentMonitoringID).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NablEnvironmentMonitoring>()
            .HasOne(x => x.LabRoom).WithMany()
            .HasForeignKey(x => x.LabRoomId).OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<EquipmentMaster>()
            .HasOne(x => x.LabRoom).WithMany()
            .HasForeignKey(x => x.LabRoomID).OnDelete(DeleteBehavior.NoAction);

        // SamplePreparation FKs (no cascade — prevent multiple cascade paths)
        modelBuilder.Entity<SamplePreparation>(entity =>
        {
            entity.HasOne(x => x.Sample).WithMany().HasForeignKey(x => x.SampleID).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.SampleInward).WithMany().HasForeignKey(x => x.InwardID).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.AssignedTo).WithMany().HasForeignKey(x => x.AssignedToEmployeeID).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.PreparedBy).WithMany().HasForeignKey(x => x.PreparedByEmployeeID).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.VerifiedBy).WithMany().HasForeignKey(x => x.VerifiedByEmployeeID).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Equipment).WithMany().HasForeignKey(x => x.EquipmentID).OnDelete(DeleteBehavior.NoAction);
        });

        // SamplePreparationMaster FKs (no cascade)
        modelBuilder.Entity<SamplePreparationMaster>()
            .HasOne(x => x.LaboratoryTest)
            .WithMany()
            .HasForeignKey(x => x.LaboratoryTestID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SamplePreparationMaster>()
            .HasOne(x => x.TestMethodSpecification)
            .WithMany()
            .HasForeignKey(x => x.TestMethodStandardID)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        // MachiningChargeMaster FKs (no cascade)
        modelBuilder.Entity<MachiningChargeMaster>()
            .HasOne(x => x.LaboratoryTest)
            .WithMany()
            .HasForeignKey(x => x.LaboratoryTestID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MachiningChargeMaster>()
            .HasOne(x => x.TestMethodSpecification)
            .WithMany()
            .HasForeignKey(x => x.TestMethodStandardID)
            .OnDelete(DeleteBehavior.NoAction);

        // MachiningChargeItem → MachiningChargeMaster (optional FK, no cascade)
        modelBuilder.Entity<MachiningChargeItem>()
            .HasOne(x => x.MachiningChargeMaster)
            .WithMany()
            .HasForeignKey(x => x.MachiningChargeMasterID)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        // CuttingPriceVersion → CuttingPriceMaster (no cascade; versions soft-deleted with the master)
        modelBuilder.Entity<CuttingPriceVersion>()
            .HasOne(x => x.CuttingPriceMaster)
            .WithMany(m => m.Versions)
            .HasForeignKey(x => x.CuttingPriceMasterID)
            .OnDelete(DeleteBehavior.NoAction);

        // CuttingPriceVersion → FinancialYear (nullable derived field — no cascade)
        modelBuilder.Entity<CuttingPriceVersion>()
            .HasOne(x => x.FinancialYear)
            .WithMany()
            .HasForeignKey(x => x.FinancialYearId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        // One rate version per effective date per master
        modelBuilder.Entity<CuttingPriceVersion>()
            .HasIndex(x => new { x.CuttingPriceMasterID, x.EffectiveFrom })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        // MachiningChargeVersion → MachiningChargeMaster (no cascade; versions soft-deleted with the master)
        modelBuilder.Entity<MachiningChargeVersion>()
            .HasOne(x => x.MachiningChargeMaster)
            .WithMany(m => m.Versions)
            .HasForeignKey(x => x.MachiningChargeMasterID)
            .OnDelete(DeleteBehavior.NoAction);

        // MachiningChargeVersion → FinancialYear (nullable derived field — no cascade)
        modelBuilder.Entity<MachiningChargeVersion>()
            .HasOne(x => x.FinancialYear)
            .WithMany()
            .HasForeignKey(x => x.FinancialYearId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        // One price version per effective date per master
        modelBuilder.Entity<MachiningChargeVersion>()
            .HasIndex(x => new { x.MachiningChargeMasterID, x.EffectiveFrom })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        // SampleInward → FinancialYear (nullable — stamped from CollectionTime on create)
        modelBuilder.Entity<SampleInward>()
            .HasOne(x => x.FinancialYear)
            .WithMany()
            .HasForeignKey(x => x.FinancialYearId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        // One InvoiceCase price version per effective date per LaboratoryTest
        // (LaboratoryTest acts as the master; each InvoiceCase row is a date-effective version)
        modelBuilder.Entity<InvoiceCase>()
            .HasIndex(x => new { x.LaboratoryTestID, x.EffectiveFrom })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        // EquipmentReferenceMaterial → EquipmentMaster (no cascade)
        modelBuilder.Entity<EquipmentReferenceMaterial>()
            .HasOne(x => x.Equipment)
            .WithMany(e => e.ReferenceMaterials)
            .HasForeignKey(x => x.EquipmentMasterID)
            .OnDelete(DeleteBehavior.NoAction);

        // PurchaseOrderItem → CustomerPurchaseOrder (no cascade)
        modelBuilder.Entity<PurchaseOrderItem>()
            .HasOne(x => x.CustomerPurchaseOrder)
            .WithMany(p => p.Items)
            .HasForeignKey(x => x.CustomerPurchaseOrderID)
            .OnDelete(DeleteBehavior.NoAction);

        // TestUsageStats composite index for smart auto-suggest lookups
        modelBuilder.Entity<TestUsageStats>()
            .HasIndex(t => new { t.LaboratoryTestID, t.MetalClassificationID, t.ProductConditionID, t.CustomerID });

        // ── Dynamic Report Format System ──
        modelBuilder.Entity<ReportFormat>()
            .HasIndex(rf => rf.FormatCode)
            .HasFilter("[IsActive] = 1 AND [FormatCode] IS NOT NULL")
            .IsUnique()
            .HasDatabaseName("IX_ReportFormat_FormatCode");

        modelBuilder.Entity<ReportFormatSection>()
            .HasOne(s => s.ReportFormat)
            .WithMany(f => f.Sections)
            .HasForeignKey(s => s.ReportFormatID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReportFormatMapping>()
            .HasOne(m => m.ReportFormat)
            .WithMany(f => f.Mappings)
            .HasForeignKey(m => m.ReportFormatID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReportFormatMapping>()
            .HasOne(m => m.LaboratoryTest)
            .WithMany()
            .HasForeignKey(m => m.LaboratoryTestID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ReportFormatMapping>()
            .HasOne(m => m.TestMethod)
            .WithMany()
            .HasForeignKey(m => m.TestMethodID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<GeneratedReport>()
            .HasOne(g => g.ReportFormat)
            .WithMany()
            .HasForeignKey(g => g.ReportFormatID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<GeneratedReport>()
            .HasOne(g => g.Sample)
            .WithMany()
            .HasForeignKey(g => g.SampleID)
            .OnDelete(DeleteBehavior.Restrict);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        var activities = new List<SiteActivity>();
        var user = httpContext?.User?.Identity?.Name ?? "System";
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
        var browser = httpContext?.Request.Headers["User-Agent"].ToString();
        var url = httpContext?.Request?.Path;

        // Auto-set ModifiedBy for all AuditProperty entities on update
        var loggedInUser = Helpers.LoggedInUserProvider.CurrentUser;
        foreach (var auditEntry in ChangeTracker.Entries<Models.AuditProperty>()
            .Where(e => e.State == EntityState.Modified))
        {
            auditEntry.Entity.ModifiedOn = DateTime.UtcNow;
            auditEntry.Entity.ModifiedBy = loggedInUser?.EmployeeID ?? 0;
        }

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();


        foreach (var entry in entries)
        {
            var tableName = entry.Metadata.GetTableName();
            var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString();
            var action = entry.State.ToString();

            activities.Add(new SiteActivity
            {
                ModuleName = tableName,
                TraceId = primaryKey,
                Ipaddress = ipAddress,
                Browser = browser,
                Action = action,
                Description = $"{action} on {tableName} (ID: {primaryKey})",
                WebUrl = url,
                ModifiedBy = user,
                ModifiedOn = DateTime.UtcNow
            });

        }
        var result = await base.SaveChangesAsync(cancellationToken);
        foreach (var entry in entries)
        {
            var tableName = entry.Metadata.GetTableName();
            var newPrimaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString();
            var action = entry.State.ToString();

            var activity = activities.FirstOrDefault(a => a.ModuleName == tableName);
            if (activity != null && activity?.Action?.Trim().ToLower() == "added")
            {
                activity.TraceId = newPrimaryKey;

            }
        }

        // Store in HttpContext.Items temporarily
        // Save activity logs only if in a real HTTP request
        if (httpContext != null && activities.Any(a => a.ModuleName != "SiteActivity" && a.ModuleName != "SiteError"))
        {
            httpContext.Items["ActivityLog"] = activities;
        }

        return result;
    }

}
