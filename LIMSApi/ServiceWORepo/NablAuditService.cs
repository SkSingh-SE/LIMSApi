using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LIMSApi.ServiceWORepo
{
    public class NablAuditService : INablAuditService
    {
        private readonly LIMSContext _context;
        private readonly ILogger<NablAuditService> _logger;
        private readonly LoggedInUserDTO loggedInUser;

        // FormType -> (FormCode, DisplayName)
        private static readonly Dictionary<string, (string Code, string Name)> FormTypeMap = new()
        {
            { "JobDescription", ("F-3", "Job Description") },
            { "ResponsibilityAuthority", ("F-4", "Responsibility & Authority") },
            { "EmployeeCompetence", ("F-7", "Employee Competence") },
            { "EmployeeAuthorization", ("F-5", "Employee Authorization") },
            { "CompetenceRequirement", ("F-4", "Competence Requirement") },
            { "InductionTraining", ("F-6", "Induction Training") },
            { "SkillMatrix", ("F-6", "Skill Matrix") },
            { "SkillMatrixDecision", ("F-6A", "Skill Matrix Decision") },
            { "TrainingPlan", ("F-8", "Training Plan") },
            { "TrainingAttendance", ("F-9", "Training Attendance") },
            { "TrainingEffectiveness", ("F-10", "Training Effectiveness") },
            { "EnvironmentMonitoring", ("F-11", "Environment Monitoring") },
            { "QualityControlPlan", ("F-12", "Quality Control Plan") },
            { "TestRequest", ("F-13", "Test Request") },
            { "TestMethod", ("F-14", "Test Method") },
            { "MethodVerification", ("F-15", "Method Verification") },
            { "MethodValidation", ("F-16", "Method Validation") },
            { "SampleInwardRegister", ("F-17", "Sample Inward Register") },
            { "SampleMusterRegister", ("F-18", "Sample Muster Register") },
            { "SampleLabel", ("F-19", "Sample Label") },
            { "TechnicalRawData", ("F-20", "Technical Raw Data") },
            { "TestReport", ("F-21", "Test Report") },
            { "EquipmentHistory", ("F-22", "Equipment History") },
            { "CalibrationReview", ("F-23", "Calibration Review") },
            { "IntermediateCheck", ("F-24", "Intermediate Check") },
            { "ReferenceMaterial", ("F-25", "Reference Material") },
            { "CrmConsumption", ("F-26", "CRM Consumption") },
            { "SupplierRegistration", ("F-27", "Supplier Registration") },
            { "SupplierEvaluation", ("F-28", "Supplier Evaluation") },
            { "ApprovedSupplier", ("F-29", "Approved Supplier") },
            { "SupplierConfidentiality", ("F-30", "Supplier Confidentiality") },
            { "IncomingMaterial", ("F-31", "Incoming Material") },
            { "ProductInspection", ("F-32", "Product Inspection") },
            { "PurchaseIndent", ("F-33", "Purchase Indent") },
            { "PurchaseOrder", ("F-34", "Purchase Order") },
            { "PurchaseMaterialVerification", ("F-35", "Purchase Material Verification") },
            { "Complaint", ("F-36", "Complaint") },
            { "CustomerFeedback", ("F-37", "Customer Feedback") },
            { "FeedbackAnalysis", ("F-38", "Feedback Analysis") },
            { "AuditPlan", ("F-39", "Audit Plan") },
            { "AuditChecklist", ("F-40", "Audit Checklist") },
            { "AuditSummary", ("F-41", "Audit Summary") },
            { "InternalAuditor", ("F-42", "Internal Auditor") },
            { "MeetingAgenda", ("F-43", "Meeting Agenda") },
            { "MeetingMinutes", ("F-44", "Meeting Minutes") },
            { "NonConformingWork", ("F-45", "Non Conforming Work") },
            { "NcCorrectiveAction", ("F-46", "NC Corrective Action") },
            { "Retesting", ("F-47", "Retesting") },
            { "RiskAssessment", ("F-48", "Risk Assessment") },
            { "DocumentChangeRequest", ("F-49", "Document Change Request") },
            { "DocumentReview", ("F-50", "Document Review") },
            { "MasterDocument", ("F-51", "Master Document") },
            { "MeasurementUncertainty", ("F-52", "Measurement Uncertainty") },
            { "PtIlcPlan", ("F-53", "PT / ILC Plan") },
        };

        public NablAuditService(LIMSContext context, ILogger<NablAuditService> logger)
        {
            _context = context;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<NablDashboardDto> GetNablDashboard()
        {
            var dashboard = new NablDashboardDto();
            var allSummary = await GetAuditSummary();

            dashboard.TotalForms = allSummary.Sum(s => s.Total);
            dashboard.DraftCount = allSummary.Sum(s => s.Draft);
            dashboard.SubmittedCount = allSummary.Sum(s => s.Submitted);
            dashboard.ApprovedCount = allSummary.Sum(s => s.Approved);
            dashboard.RejectedCount = allSummary.Sum(s => s.Rejected);
            dashboard.FormSummary = allSummary;

            // Pending Reviews: forms with status "Submitted" (awaiting review)
            dashboard.PendingReviews = await GetPendingReviews();

            // Upcoming review dates (within next 30 days)
            dashboard.UpcomingReviews = await GetUpcomingReviews(30);

            // Expiring documents (review date passed or within 7 days)
            dashboard.ExpiringDocuments = await GetExpiringDocuments(7);

            return dashboard;
        }

        public async Task<List<AuditSummaryItem>> GetAuditSummary()
        {
            var results = new List<AuditSummaryItem>();

            foreach (var kvp in FormTypeMap)
            {
                var item = await GetSummaryForType(kvp.Key, kvp.Value.Code, kvp.Value.Name);
                if (item != null)
                    results.Add(item);
            }

            return results;
        }

        public async Task<byte[]> GenerateAuditPackage(List<string> formTypes, DateTime? from, DateTime? to)
        {
            var formDataSections = new List<(string FormName, string FormCode, List<AuditFormRow> Rows)>();

            foreach (var formType in formTypes)
            {
                if (!FormTypeMap.ContainsKey(formType))
                    continue;

                var (code, name) = FormTypeMap[formType];
                var rows = await GetFormRows(formType, from, to);
                if (rows.Any())
                    formDataSections.Add((name, code, rows));
            }

            return GenerateAuditPdf(formDataSections, from, to);
        }

        // ─── Private helpers ──────────────────────────────────────────────

        private async Task<AuditSummaryItem?> GetSummaryForType(string formType, string code, string name)
        {
            try
            {
                var counts = await GetStatusCounts(formType);
                if (counts == null) return null;

                return new AuditSummaryItem
                {
                    FormType = formType,
                    FormCode = code,
                    Total = counts.Total,
                    Draft = counts.Draft,
                    Submitted = counts.Submitted,
                    Reviewed = counts.Reviewed,
                    Approved = counts.Approved,
                    Rejected = counts.Rejected
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting summary for form type {FormType}", formType);
                return null;
            }
        }

        private async Task<StatusCounts?> GetStatusCounts(string formType)
        {
            return formType switch
            {
                "JobDescription" => await CountStatuses<NablJobDescription>(),
                "ResponsibilityAuthority" => await CountStatuses<NablResponsibilityAuthority>(),
                "EmployeeCompetence" => await CountStatuses<NablEmployeeCompetence>(),
                "EmployeeAuthorization" => await CountStatuses<NablEmployeeAuthorization>(),
                "CompetenceRequirement" => await CountStatuses<NablCompetenceRequirement>(),
                "InductionTraining" => await CountStatuses<NablInductionTraining>(),
                "SkillMatrix" => await CountStatuses<NablSkillMatrix>(),
                "SkillMatrixDecision" => await CountStatuses<NablSkillMatrixDecision>(),
                "TrainingPlan" => await CountStatuses<NablTrainingPlan>(),
                "TrainingAttendance" => await CountStatuses<NablTrainingAttendance>(),
                "TrainingEffectiveness" => await CountStatuses<NablTrainingEffectiveness>(),
                "EnvironmentMonitoring" => await CountStatuses<NablEnvironmentMonitoring>(),
                "QualityControlPlan" => await CountStatuses<NablQualityControlPlan>(),
                "TestRequest" => await CountStatuses<NablTestRequest>(),
                "TestMethod" => await CountStatuses<NablTestMethod>(),
                "MethodVerification" => await CountStatuses<NablMethodVerification>(),
                "MethodValidation" => await CountStatuses<NablMethodValidation>(),
                "SampleInwardRegister" => await CountStatuses<NablSampleInwardRegister>(),
                "SampleMusterRegister" => await CountStatuses<NablSampleMusterRegister>(),
                "SampleLabel" => await CountStatuses<NablSampleLabel>(),
                "TechnicalRawData" => await CountStatuses<NablTechnicalRawData>(),
                "TestReport" => await CountStatuses<NablTestReport>(),
                "EquipmentHistory" => await CountStatuses<NablEquipmentHistory>(),
                "CalibrationReview" => await CountStatuses<NablCalibrationReview>(),
                "IntermediateCheck" => await CountStatuses<NablIntermediateCheck>(),
                "ReferenceMaterial" => await CountStatuses<NablReferenceMaterial>(),
                "CrmConsumption" => await CountStatuses<NablCrmConsumption>(),
                "SupplierRegistration" => await CountStatuses<NablSupplierRegistration>(),
                "SupplierEvaluation" => await CountStatuses<NablSupplierEvaluation>(),
                "ApprovedSupplier" => await CountStatuses<NablApprovedSupplier>(),
                "SupplierConfidentiality" => await CountStatuses<NablSupplierConfidentiality>(),
                "IncomingMaterial" => await CountStatuses<NablIncomingMaterial>(),
                "ProductInspection" => await CountStatuses<NablProductInspection>(),
                "PurchaseIndent" => await CountStatuses<NablPurchaseIndent>(),
                "PurchaseOrder" => await CountStatuses<NablPurchaseOrder>(),
                "PurchaseMaterialVerification" => await CountStatuses<NablPurchaseMaterialVerification>(),
                "Complaint" => await CountStatuses<NablComplaint>(),
                "CustomerFeedback" => await CountStatuses<NablCustomerFeedback>(),
                "FeedbackAnalysis" => await CountStatuses<NablFeedbackAnalysis>(),
                "AuditPlan" => await CountStatuses<NablAuditPlan>(),
                "AuditChecklist" => await CountStatuses<NablAuditChecklist>(),
                "AuditSummary" => await CountStatuses<NablAuditSummary>(),
                "InternalAuditor" => await CountStatuses<NablInternalAuditor>(),
                "MeetingAgenda" => await CountStatuses<NablMeetingAgenda>(),
                "MeetingMinutes" => await CountStatuses<NablMeetingMinutes>(),
                "NonConformingWork" => await CountStatuses<NablNonConformingWork>(),
                "NcCorrectiveAction" => await CountStatuses<NablNcCorrectiveAction>(),
                "Retesting" => await CountStatuses<NablRetesting>(),
                "RiskAssessment" => await CountStatuses<NablRiskAssessment>(),
                "DocumentChangeRequest" => await CountStatuses<NablDocumentChangeRequest>(),
                "DocumentReview" => await CountStatuses<NablDocumentReview>(),
                "MasterDocument" => await CountStatuses<NablMasterDocument>(),
                "MeasurementUncertainty" => await CountStatuses<NablMeasurementUncertainty>(),
                "PtIlcPlan" => await CountStatuses<NablPtIlcPlan>(),
                _ => null
            };
        }

        private async Task<StatusCounts> CountStatuses<T>() where T : NablFormBase
        {
            var query = _context.Set<T>()
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            var groups = await query
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return new StatusCounts
            {
                Total = groups.Sum(g => g.Count),
                Draft = groups.Where(g => g.Status == "Draft").Sum(g => g.Count),
                Submitted = groups.Where(g => g.Status == "Submitted").Sum(g => g.Count),
                Reviewed = groups.Where(g => g.Status == "Reviewed").Sum(g => g.Count),
                Approved = groups.Where(g => g.Status == "Approved").Sum(g => g.Count),
                Rejected = groups.Where(g => g.Status == "Rejected").Sum(g => g.Count)
            };
        }

        private async Task<List<NablPendingReviewItem>> GetPendingReviews()
        {
            var results = new List<NablPendingReviewItem>();

            foreach (var kvp in FormTypeMap)
            {
                try
                {
                    var items = await GetPendingForType(kvp.Key, kvp.Value.Code);
                    results.AddRange(items);
                }
                catch { /* skip types that fail */ }
            }

            return results.OrderBy(x => x.Date).Take(20).ToList();
        }

        private async Task<List<NablPendingReviewItem>> GetPendingForType(string formType, string formCode)
        {
            return formType switch
            {
                "JobDescription" => await GetPendingTyped<NablJobDescription>(formType, formCode),
                "ResponsibilityAuthority" => await GetPendingTyped<NablResponsibilityAuthority>(formType, formCode),
                "EmployeeCompetence" => await GetPendingTyped<NablEmployeeCompetence>(formType, formCode),
                "EmployeeAuthorization" => await GetPendingTyped<NablEmployeeAuthorization>(formType, formCode),
                "CompetenceRequirement" => await GetPendingTyped<NablCompetenceRequirement>(formType, formCode),
                "InductionTraining" => await GetPendingTyped<NablInductionTraining>(formType, formCode),
                "SkillMatrix" => await GetPendingTyped<NablSkillMatrix>(formType, formCode),
                "TrainingPlan" => await GetPendingTyped<NablTrainingPlan>(formType, formCode),
                "TrainingAttendance" => await GetPendingTyped<NablTrainingAttendance>(formType, formCode),
                "TrainingEffectiveness" => await GetPendingTyped<NablTrainingEffectiveness>(formType, formCode),
                "EnvironmentMonitoring" => await GetPendingTyped<NablEnvironmentMonitoring>(formType, formCode),
                "TestRequest" => await GetPendingTyped<NablTestRequest>(formType, formCode),
                "NonConformingWork" => await GetPendingTyped<NablNonConformingWork>(formType, formCode),
                "NcCorrectiveAction" => await GetPendingTyped<NablNcCorrectiveAction>(formType, formCode),
                "AuditPlan" => await GetPendingTyped<NablAuditPlan>(formType, formCode),
                "AuditChecklist" => await GetPendingTyped<NablAuditChecklist>(formType, formCode),
                "DocumentChangeRequest" => await GetPendingTyped<NablDocumentChangeRequest>(formType, formCode),
                "RiskAssessment" => await GetPendingTyped<NablRiskAssessment>(formType, formCode),
                _ => new List<NablPendingReviewItem>()
            };
        }

        private async Task<List<NablPendingReviewItem>> GetPendingTyped<T>(string formType, string formCode)
            where T : NablFormBase
        {
            return await _context.Set<T>()
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode && x.Status == "Submitted")
                .OrderBy(x => x.Date)
                .Take(5)
                .Select(x => new NablPendingReviewItem
                {
                    Id = x.ID,
                    FormType = formType,
                    FormCode = formCode,
                    DocumentNo = x.DocumentNo,
                    Status = x.Status,
                    Date = x.Date,
                    PreparedBy = x.PreparedBy
                })
                .ToListAsync();
        }

        private async Task<List<NablUpcomingReviewItem>> GetUpcomingReviews(int daysAhead)
        {
            var results = new List<NablUpcomingReviewItem>();
            var cutoff = DateTime.UtcNow.AddDays(daysAhead);

            foreach (var kvp in FormTypeMap)
            {
                try
                {
                    var items = await GetUpcomingForType(kvp.Key, kvp.Value.Code, cutoff);
                    results.AddRange(items);
                }
                catch { /* skip */ }
            }

            return results.OrderBy(x => x.NextReviewDate).Take(20).ToList();
        }

        private async Task<List<NablUpcomingReviewItem>> GetUpcomingForType(string formType, string formCode, DateTime cutoff)
        {
            // Only handle a representative subset to avoid too large a switch
            return formType switch
            {
                "JobDescription" => await GetUpcomingTyped<NablJobDescription>(formType, formCode, cutoff),
                "EnvironmentMonitoring" => await GetUpcomingTyped<NablEnvironmentMonitoring>(formType, formCode, cutoff),
                "TestMethod" => await GetUpcomingTyped<NablTestMethod>(formType, formCode, cutoff),
                "AuditPlan" => await GetUpcomingTyped<NablAuditPlan>(formType, formCode, cutoff),
                "DocumentReview" => await GetUpcomingTyped<NablDocumentReview>(formType, formCode, cutoff),
                "MasterDocument" => await GetUpcomingTyped<NablMasterDocument>(formType, formCode, cutoff),
                "CalibrationReview" => await GetUpcomingTyped<NablCalibrationReview>(formType, formCode, cutoff),
                "RiskAssessment" => await GetUpcomingTyped<NablRiskAssessment>(formType, formCode, cutoff),
                _ => new List<NablUpcomingReviewItem>()
            };
        }

        private async Task<List<NablUpcomingReviewItem>> GetUpcomingTyped<T>(string formType, string formCode, DateTime cutoff)
            where T : NablFormBase
        {
            var now = DateTime.UtcNow;
            return await _context.Set<T>()
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode
                    && x.NextReviewDate != null && x.NextReviewDate <= cutoff && x.NextReviewDate >= now
                    && !x.IsObsolete)
                .OrderBy(x => x.NextReviewDate)
                .Take(5)
                .Select(x => new NablUpcomingReviewItem
                {
                    Id = x.ID,
                    FormType = formType,
                    FormCode = formCode,
                    DocumentNo = x.DocumentNo,
                    NextReviewDate = x.NextReviewDate,
                    DaysUntilReview = x.NextReviewDate.HasValue ? (int)(x.NextReviewDate.Value - now).TotalDays : 0
                })
                .ToListAsync();
        }

        private async Task<List<NablExpiringDocItem>> GetExpiringDocuments(int daysThreshold)
        {
            var results = new List<NablExpiringDocItem>();
            var cutoff = DateTime.UtcNow.AddDays(daysThreshold);

            foreach (var kvp in FormTypeMap)
            {
                try
                {
                    var items = await GetExpiringForType(kvp.Key, kvp.Value.Code, cutoff);
                    results.AddRange(items);
                }
                catch { /* skip */ }
            }

            return results.OrderBy(x => x.NextReviewDate).Take(20).ToList();
        }

        private async Task<List<NablExpiringDocItem>> GetExpiringForType(string formType, string formCode, DateTime cutoff)
        {
            return formType switch
            {
                "MasterDocument" => await GetExpiringTyped<NablMasterDocument>(formType, formCode, cutoff),
                "DocumentReview" => await GetExpiringTyped<NablDocumentReview>(formType, formCode, cutoff),
                "CalibrationReview" => await GetExpiringTyped<NablCalibrationReview>(formType, formCode, cutoff),
                "SupplierEvaluation" => await GetExpiringTyped<NablSupplierEvaluation>(formType, formCode, cutoff),
                "RiskAssessment" => await GetExpiringTyped<NablRiskAssessment>(formType, formCode, cutoff),
                _ => new List<NablExpiringDocItem>()
            };
        }

        private async Task<List<NablExpiringDocItem>> GetExpiringTyped<T>(string formType, string formCode, DateTime cutoff)
            where T : NablFormBase
        {
            var now = DateTime.UtcNow;
            return await _context.Set<T>()
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode
                    && x.NextReviewDate != null && x.NextReviewDate <= cutoff
                    && !x.IsObsolete)
                .OrderBy(x => x.NextReviewDate)
                .Take(5)
                .Select(x => new NablExpiringDocItem
                {
                    Id = x.ID,
                    FormType = formType,
                    FormCode = formCode,
                    DocumentNo = x.DocumentNo,
                    EffectiveDate = x.EffectiveDate,
                    NextReviewDate = x.NextReviewDate,
                    IsOverdue = x.NextReviewDate < now
                })
                .ToListAsync();
        }

        private async Task<List<AuditFormRow>> GetFormRows(string formType, DateTime? from, DateTime? to)
        {
            return formType switch
            {
                "JobDescription" => await GetFormRowsTyped<NablJobDescription>(from, to),
                "ResponsibilityAuthority" => await GetFormRowsTyped<NablResponsibilityAuthority>(from, to),
                "EmployeeCompetence" => await GetFormRowsTyped<NablEmployeeCompetence>(from, to),
                "EmployeeAuthorization" => await GetFormRowsTyped<NablEmployeeAuthorization>(from, to),
                "CompetenceRequirement" => await GetFormRowsTyped<NablCompetenceRequirement>(from, to),
                "InductionTraining" => await GetFormRowsTyped<NablInductionTraining>(from, to),
                "SkillMatrix" => await GetFormRowsTyped<NablSkillMatrix>(from, to),
                "SkillMatrixDecision" => await GetFormRowsTyped<NablSkillMatrixDecision>(from, to),
                "TrainingPlan" => await GetFormRowsTyped<NablTrainingPlan>(from, to),
                "TrainingAttendance" => await GetFormRowsTyped<NablTrainingAttendance>(from, to),
                "TrainingEffectiveness" => await GetFormRowsTyped<NablTrainingEffectiveness>(from, to),
                "EnvironmentMonitoring" => await GetFormRowsTyped<NablEnvironmentMonitoring>(from, to),
                "QualityControlPlan" => await GetFormRowsTyped<NablQualityControlPlan>(from, to),
                "TestRequest" => await GetFormRowsTyped<NablTestRequest>(from, to),
                "TestMethod" => await GetFormRowsTyped<NablTestMethod>(from, to),
                "MethodVerification" => await GetFormRowsTyped<NablMethodVerification>(from, to),
                "MethodValidation" => await GetFormRowsTyped<NablMethodValidation>(from, to),
                "SampleInwardRegister" => await GetFormRowsTyped<NablSampleInwardRegister>(from, to),
                "SampleMusterRegister" => await GetFormRowsTyped<NablSampleMusterRegister>(from, to),
                "SampleLabel" => await GetFormRowsTyped<NablSampleLabel>(from, to),
                "TechnicalRawData" => await GetFormRowsTyped<NablTechnicalRawData>(from, to),
                "TestReport" => await GetFormRowsTyped<NablTestReport>(from, to),
                "EquipmentHistory" => await GetFormRowsTyped<NablEquipmentHistory>(from, to),
                "CalibrationReview" => await GetFormRowsTyped<NablCalibrationReview>(from, to),
                "IntermediateCheck" => await GetFormRowsTyped<NablIntermediateCheck>(from, to),
                "ReferenceMaterial" => await GetFormRowsTyped<NablReferenceMaterial>(from, to),
                "CrmConsumption" => await GetFormRowsTyped<NablCrmConsumption>(from, to),
                "SupplierRegistration" => await GetFormRowsTyped<NablSupplierRegistration>(from, to),
                "SupplierEvaluation" => await GetFormRowsTyped<NablSupplierEvaluation>(from, to),
                "ApprovedSupplier" => await GetFormRowsTyped<NablApprovedSupplier>(from, to),
                "SupplierConfidentiality" => await GetFormRowsTyped<NablSupplierConfidentiality>(from, to),
                "IncomingMaterial" => await GetFormRowsTyped<NablIncomingMaterial>(from, to),
                "ProductInspection" => await GetFormRowsTyped<NablProductInspection>(from, to),
                "PurchaseIndent" => await GetFormRowsTyped<NablPurchaseIndent>(from, to),
                "PurchaseOrder" => await GetFormRowsTyped<NablPurchaseOrder>(from, to),
                "PurchaseMaterialVerification" => await GetFormRowsTyped<NablPurchaseMaterialVerification>(from, to),
                "Complaint" => await GetFormRowsTyped<NablComplaint>(from, to),
                "CustomerFeedback" => await GetFormRowsTyped<NablCustomerFeedback>(from, to),
                "FeedbackAnalysis" => await GetFormRowsTyped<NablFeedbackAnalysis>(from, to),
                "AuditPlan" => await GetFormRowsTyped<NablAuditPlan>(from, to),
                "AuditChecklist" => await GetFormRowsTyped<NablAuditChecklist>(from, to),
                "AuditSummary" => await GetFormRowsTyped<NablAuditSummary>(from, to),
                "InternalAuditor" => await GetFormRowsTyped<NablInternalAuditor>(from, to),
                "MeetingAgenda" => await GetFormRowsTyped<NablMeetingAgenda>(from, to),
                "MeetingMinutes" => await GetFormRowsTyped<NablMeetingMinutes>(from, to),
                "NonConformingWork" => await GetFormRowsTyped<NablNonConformingWork>(from, to),
                "NcCorrectiveAction" => await GetFormRowsTyped<NablNcCorrectiveAction>(from, to),
                "Retesting" => await GetFormRowsTyped<NablRetesting>(from, to),
                "RiskAssessment" => await GetFormRowsTyped<NablRiskAssessment>(from, to),
                "DocumentChangeRequest" => await GetFormRowsTyped<NablDocumentChangeRequest>(from, to),
                "DocumentReview" => await GetFormRowsTyped<NablDocumentReview>(from, to),
                "MasterDocument" => await GetFormRowsTyped<NablMasterDocument>(from, to),
                "MeasurementUncertainty" => await GetFormRowsTyped<NablMeasurementUncertainty>(from, to),
                "PtIlcPlan" => await GetFormRowsTyped<NablPtIlcPlan>(from, to),
                _ => new List<AuditFormRow>()
            };
        }

        private async Task<List<AuditFormRow>> GetFormRowsTyped<T>(DateTime? from, DateTime? to) where T : NablFormBase
        {
            var query = _context.Set<T>()
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (from.HasValue)
                query = query.Where(x => x.Date >= from.Value);
            if (to.HasValue)
                query = query.Where(x => x.Date <= to.Value);

            return await query
                .OrderByDescending(x => x.Date)
                .Select(x => new AuditFormRow
                {
                    Id = x.ID,
                    DocumentNo = x.DocumentNo ?? "",
                    FormCode = x.FormCode,
                    IssueNo = x.IssueNo,
                    RevNo = x.RevNo,
                    Date = x.Date,
                    Status = x.Status ?? "Draft",
                    PreparedBy = x.PreparedBy ?? "",
                    ReviewedBy = x.ReviewedBy ?? "",
                    ApprovedBy = x.ApprovedBy ?? ""
                })
                .ToListAsync();
        }

        private byte[] GenerateAuditPdf(
            List<(string FormName, string FormCode, List<AuditFormRow> Rows)> sections,
            DateTime? from, DateTime? to)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("NABL Audit Package").FontSize(16).Bold().AlignCenter();
                        col.Item().Text(text =>
                        {
                            text.Span("Generated: ").SemiBold();
                            text.Span(DateTime.UtcNow.ToString("dd-MMM-yyyy HH:mm"));
                        });

                        if (from.HasValue || to.HasValue)
                        {
                            col.Item().Text(text =>
                            {
                                text.Span("Period: ").SemiBold();
                                text.Span($"{from?.ToString("dd-MMM-yyyy") ?? "Start"} to {to?.ToString("dd-MMM-yyyy") ?? "Present"}");
                            });
                        }

                        col.Item().PaddingBottom(10);
                    });

                    page.Content().Column(col =>
                    {
                        foreach (var section in sections)
                        {
                            col.Item().PaddingBottom(10).Column(sectionCol =>
                            {
                                sectionCol.Item().Background("#B71C1C").Padding(5)
                                    .Text($"{section.FormCode} - {section.FormName} ({section.Rows.Count} records)")
                                    .FontColor("#FFFFFF").Bold().FontSize(11);

                                sectionCol.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(30);   // #
                                        columns.RelativeColumn(2);    // Doc No
                                        columns.RelativeColumn(1);    // Form Code
                                        columns.RelativeColumn(1);    // Issue
                                        columns.RelativeColumn(1);    // Rev
                                        columns.RelativeColumn(1.5f); // Date
                                        columns.RelativeColumn(1);    // Status
                                        columns.RelativeColumn(1.5f); // Prepared By
                                        columns.RelativeColumn(1.5f); // Reviewed By
                                        columns.RelativeColumn(1.5f); // Approved By
                                    });

                                    // Header row
                                    table.Header(header =>
                                    {
                                        var headerStyle = TextStyle.Default.Bold().FontSize(8);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("#").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Document No").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Form Code").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Issue").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Rev").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Date").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Status").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Prepared By").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Reviewed By").Style(headerStyle);
                                        header.Cell().Background("#E0E0E0").Padding(3).Text("Approved By").Style(headerStyle);
                                    });

                                    // Data rows
                                    for (int i = 0; i < section.Rows.Count; i++)
                                    {
                                        var row = section.Rows[i];
                                        var bg = i % 2 == 0 ? "#FFFFFF" : "#F5F5F5";

                                        table.Cell().Background(bg).Padding(2).Text((i + 1).ToString());
                                        table.Cell().Background(bg).Padding(2).Text(row.DocumentNo);
                                        table.Cell().Background(bg).Padding(2).Text(row.FormCode);
                                        table.Cell().Background(bg).Padding(2).Text(row.IssueNo);
                                        table.Cell().Background(bg).Padding(2).Text(row.RevNo);
                                        table.Cell().Background(bg).Padding(2).Text(row.Date.ToString("dd-MMM-yyyy"));
                                        table.Cell().Background(bg).Padding(2).Text(row.Status);
                                        table.Cell().Background(bg).Padding(2).Text(row.PreparedBy);
                                        table.Cell().Background(bg).Padding(2).Text(row.ReviewedBy);
                                        table.Cell().Background(bg).Padding(2).Text(row.ApprovedBy);
                                    }
                                });
                            });
                        }

                        if (!sections.Any())
                        {
                            col.Item().PaddingTop(30).Text("No records found for the selected form types and date range.")
                                .FontSize(12).Italic().AlignCenter();
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }

        private class StatusCounts
        {
            public int Total { get; set; }
            public int Draft { get; set; }
            public int Submitted { get; set; }
            public int Reviewed { get; set; }
            public int Approved { get; set; }
            public int Rejected { get; set; }
        }
    }

    public class AuditFormRow
    {
        public long Id { get; set; }
        public string DocumentNo { get; set; } = "";
        public string FormCode { get; set; } = "";
        public string IssueNo { get; set; } = "";
        public string RevNo { get; set; } = "";
        public DateTime Date { get; set; }
        public string Status { get; set; } = "";
        public string PreparedBy { get; set; } = "";
        public string ReviewedBy { get; set; } = "";
        public string ApprovedBy { get; set; } = "";
    }
}
