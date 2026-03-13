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
    public virtual DbSet<CurrencyMaster> CurrencyMasters { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<CustomerCompanyCategory> CustomerCompanyCategories { get; set; }
    public virtual DbSet<CustomerDispatchMode> CustomerDispatchModes { get; set; }
    public virtual DbSet<CuttingPriceMaster> CuttingPriceMasters { get; set; }
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
    public virtual DbSet<MakerMaster> MakerMasters { get; set; }
    public virtual DbSet<MenuMaster> MenuMasters { get; set; }
    public virtual DbSet<MetalClassificationMaster> MetalClassificationMasters { get; set; }
    public virtual DbSet<OEMMaster> OEMMasters { get; set; }
    public virtual DbSet<OrganisationMaster> OrganisationMasters { get; set; }
    public virtual DbSet<ParameterMaster> ParameterMasters { get; set; }
    public virtual DbSet<ParameterUnitMaster> ParameterUnitMasters { get; set; }
    public virtual DbSet<ProductConditionMaster> ProductConditionMasters { get; set; }
    public virtual DbSet<ProductSpecification> ProductSpecifications { get; set; }
    public virtual DbSet<RemarkMaster> RemarkMasters { get; set; }
    public virtual DbSet<RoleMaster> RoleMasters { get; set; }
    public virtual DbSet<RoleMenuMapping> RoleMenuMappings { get; set; }
    public virtual DbSet<SiteActivity> SiteActivities { get; set; }
    public virtual DbSet<SiteError> SiteErrors { get; set; }
    public virtual DbSet<SpecificationHeader> SpecificationHeaders { get; set; }
    public virtual DbSet<SpecificationGrade> SpecificationGrades { get; set; }
    public virtual DbSet<SpecificationLine> SpecificationLines { get; set; }
    public virtual DbSet<SpecificationLineLaboratoryTest> SpecificationLineLaboratoryTests { get; set; }

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
    public virtual DbSet<SampleAdditionalDetail> SampleAdditionalDetails { get; set; }
    public virtual DbSet<SampleTestPlan> TestPlans { get; set; }
    public virtual DbSet<TaxMaster> TaxMasters { get; set; }
    public virtual DbSet<TestGroup> TestGroups { get; set; }
    public virtual DbSet<TestGroupMapping> TestGroupMappings { get; set; }
    public virtual DbSet<TestMaster> TestMasters { get; set; }
    public virtual DbSet<LaboratoryTest> LaboratoryTests { get; set; }
    public virtual DbSet<LaboratoryTestInvoiceCase> LaboratoryTestInvoiceCase { get; set; }
    public virtual DbSet<TestMethodStandard> TestMethodStandards { get; set; }
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
    public virtual DbSet<Workflow> Workflows { get; set; }
    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }
    public virtual DbSet<WorkflowTransition> WorkflowTransitions { get; set; }
    public virtual DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public virtual DbSet<WorkflowActionLog> WorkflowActionLogs { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<UserPushSubscription> UserPushSubscriptions { get; set; }
    public virtual DbSet<JobExecutionLog> JobExecutionLogs { get; set; }
    public virtual DbSet<MaterialTestMapping> MaterialTestMappings { get; set; }

    public DbSet<CuttingChargeHeader> CuttingChargeHeaders { get; set; }
    public DbSet<CuttingChargeSample> CuttingChargeSamples { get; set; }
    public DbSet<CuttingChargeDetail> CuttingChargeDetails { get; set; }
    public DbSet<TestMappingLaboratoryTest> MappingLaboratoryTests { get; set; }
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
    public DbSet<CustomerAmendment> CustomerAmendments { get; set; }
    public DbSet<TaxInvoice> TaxInvoices { get; set; }
    public DbSet<ChargeEvent> ChargeEvents { get; set; }
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
    public DbSet<NablEmployeeAuthorization> NablEmployeeAuthorizations => Set<NablEmployeeAuthorization>();
    public DbSet<NablCompetenceRequirement> NablCompetenceRequirements => Set<NablCompetenceRequirement>();
    public DbSet<NablInductionTraining> NablInductionTrainings => Set<NablInductionTraining>();
    public DbSet<NablSkillMatrix> NablSkillMatrices => Set<NablSkillMatrix>();
    public DbSet<NablSkillMatrixDecision> NablSkillMatrixDecisions => Set<NablSkillMatrixDecision>();
    public DbSet<NablTrainingPlan> NablTrainingPlans => Set<NablTrainingPlan>();
    public DbSet<NablTrainingAttendance> NablTrainingAttendances => Set<NablTrainingAttendance>();
    public DbSet<NablTrainingEffectiveness> NablTrainingEffectivenesses => Set<NablTrainingEffectiveness>();
    public DbSet<NablEnvironmentMonitoring> NablEnvironmentMonitorings => Set<NablEnvironmentMonitoring>();
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
    public DbSet<AuthorizedSignatory> AuthorizedSignatories => Set<AuthorizedSignatory>();

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the BankName= syntax to read it from _configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=LIMS_Backup;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SpecificationLineLaboratoryTest>()
      .HasKey(x => new { x.SpecificationLineID, x.LaboratoryTestID });

        modelBuilder.Entity<MetalClassificationParameter>().HasKey(x => new { x.MetalClassificationID, x.ParameterID });

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
