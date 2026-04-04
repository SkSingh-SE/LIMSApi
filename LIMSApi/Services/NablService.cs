using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class NablService : INablService
    {
        private readonly INablRepository _repository;
        private readonly LIMSContext _context;
        private readonly ILogger<NablService> _logger;
        private readonly LoggedInUserDTO loggedInUser;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // FormType -> FormCode mapping
        private static readonly Dictionary<string, string> FormCodeMap = new()
        {
            { "JobDescription", "F-3" },
            { "ResponsibilityAuthority", "F-4" },
            { "EmployeeCompetence", "F-7" },
            { "EmployeeAuthorization", "F-5" },
            { "CompetenceRequirement", "F-4" },
            { "InductionTraining", "F-6" },
            { "SkillMatrix", "F-6" },
            { "SkillMatrixDecision", "F-6A" },
            { "TrainingPlan", "F-8" },
            { "TrainingAttendance", "F-9" },
            { "TrainingEffectiveness", "F-10" },
            { "EnvironmentMonitoring", "F-11" },
            { "QualityControlPlan", "F-12" },
            { "TestRequest", "F-13" },
            { "TestMethod", "F-14" },
            { "MethodVerification", "F-15" },
            { "MethodValidation", "F-16" },
            { "SampleInwardRegister", "F-17" },
            { "SampleMusterRegister", "F-18" },
            { "SampleLabel", "F-19" },
            { "TechnicalRawData", "F-20" },
            { "TestReport", "F-21" },
            { "EquipmentHistory", "F-22" },
            { "CalibrationReview", "F-23" },
            { "IntermediateCheck", "F-24" },
            { "ReferenceMaterial", "F-25" },
            { "CrmConsumption", "F-26" },
            { "SupplierRegistration", "F-27" },
            { "SupplierEvaluation", "F-28" },
            { "ApprovedSupplier", "F-29" },
            { "SupplierConfidentiality", "F-30" },
            { "IncomingMaterial", "F-31" },
            { "ProductInspection", "F-32" },
            { "PurchaseIndent", "F-33" },
            { "PurchaseOrder", "F-34" },
            { "PurchaseMaterialVerification", "F-35" },
            { "Complaint", "F-36" },
            { "CustomerFeedback", "F-37" },
            { "FeedbackAnalysis", "F-38" },
            { "AuditPlan", "F-39" },
            { "AuditChecklist", "F-40" },
            { "AuditSummary", "F-41" },
            { "InternalAuditor", "F-42" },
            { "MeetingAgenda", "F-43" },
            { "MeetingMinutes", "F-44" },
            { "NonConformingWork", "F-45" },
            { "NcCorrectiveAction", "F-46" },
            { "Retesting", "F-47" },
            { "RiskAssessment", "F-48" },
            { "DocumentChangeRequest", "F-49" },
            { "DocumentReview", "F-50" },
            { "MasterDocument", "F-51" },
            { "MeasurementUncertainty", "F-52" },
            { "PtIlcPlan", "F-53" },
            { "EmployeePerformanceRecord", "F-54" },
        };

        public NablService(INablRepository repository, LIMSContext context, ILogger<NablService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<PagedResponse<object>> FetchList(string formType, PageFilter filter)
        {
            return await _repository.GetAll(formType, filter);
        }

        public async Task<object?> GetDetails(string formType, long id)
        {
            return await _repository.GetById(formType, id);
        }

        public async Task<object?> GetByDesignationId(string formType, long designationId)
        {
            return formType switch
            {
                "JobDescription" => await _context.NablJobDescriptions
                    .FirstOrDefaultAsync(x => x.DesignationId == designationId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "ResponsibilityAuthority" => await _context.NablResponsibilityAuthorities
                    .FirstOrDefaultAsync(x => x.DesignationId == designationId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "CompetenceRequirement" => await _context.NablCompetenceRequirements
                    .FirstOrDefaultAsync(x => x.PositionId == designationId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SkillMatrix" => await _context.NablSkillMatrices
                    .FirstOrDefaultAsync(x => x.DesignationId == designationId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SkillMatrixDecision" => await _context.NablSkillMatrixDecisions
                    .FirstOrDefaultAsync(x => x.DesignationId == designationId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "EmployeeCompetence" => null,
                "EmployeePerformanceRecord" => null,
                "EmployeeAuthorization" => null,
                "InductionTraining" => null,
                _ => throw new ArgumentException($"Unknown form type: {formType}")
            };
        }

        public async Task<long> Save(string formType, JsonElement body)
        {
            return formType switch
            {
                "JobDescription" => await SaveJobDescription(body),
                "ResponsibilityAuthority" => await SaveResponsibilityAuthority(body),
                "EmployeeCompetence" => await SaveEmployeeCompetence(body),
                "EmployeePerformanceRecord" => await SaveEmployeePerformanceRecord(body),
                "EmployeeAuthorization" => await SaveEmployeeAuthorization(body),
                "CompetenceRequirement" => await SaveCompetenceRequirement(body),
                "InductionTraining" => await SaveInductionTraining(body),
                "SkillMatrix" => await SaveSkillMatrix(body),
                "SkillMatrixDecision" => await SaveSkillMatrixDecision(body),
                "TrainingPlan" => await SaveTrainingPlan(body),
                "TrainingAttendance" => await SaveTrainingAttendance(body),
                "TrainingEffectiveness" => await SaveTrainingEffectiveness(body),
                "EnvironmentMonitoring" => await SaveEnvironmentMonitoring(body),
                "QualityControlPlan" => await SaveQualityControlPlan(body),
                "TestRequest" => await SaveTestRequest(body),
                "TestMethod" => await SaveTestMethod(body),
                "MethodVerification" => await SaveMethodVerification(body),
                "MethodValidation" => await SaveMethodValidation(body),
                "SampleInwardRegister" => await SaveSampleInwardRegister(body),
                "SampleMusterRegister" => await SaveSampleMusterRegister(body),
                "SampleLabel" => await SaveSampleLabel(body),
                "TechnicalRawData" => await SaveTechnicalRawData(body),
                "TestReport" => await SaveTestReport(body),
                "EquipmentHistory" => await SaveEquipmentHistory(body),
                "CalibrationReview" => await SaveCalibrationReview(body),
                "IntermediateCheck" => await SaveIntermediateCheck(body),
                "ReferenceMaterial" => await SaveReferenceMaterial(body),
                "CrmConsumption" => await SaveCrmConsumption(body),
                "SupplierRegistration" => await SaveSupplierRegistration(body),
                "SupplierEvaluation" => await SaveSupplierEvaluation(body),
                "ApprovedSupplier" => await SaveApprovedSupplier(body),
                "SupplierConfidentiality" => await SaveSupplierConfidentiality(body),
                "IncomingMaterial" => await SaveIncomingMaterial(body),
                "ProductInspection" => await SaveProductInspection(body),
                "PurchaseIndent" => await SavePurchaseIndent(body),
                "PurchaseOrder" => await SavePurchaseOrder(body),
                "PurchaseMaterialVerification" => await SavePurchaseMaterialVerification(body),
                "Complaint" => await SaveComplaint(body),
                "CustomerFeedback" => await SaveCustomerFeedback(body),
                "FeedbackAnalysis" => await SaveFeedbackAnalysis(body),
                "AuditPlan" => await SaveAuditPlan(body),
                "AuditChecklist" => await SaveAuditChecklist(body),
                "AuditSummary" => await SaveAuditSummary(body),
                "InternalAuditor" => await SaveInternalAuditor(body),
                "MeetingAgenda" => await SaveMeetingAgenda(body),
                "MeetingMinutes" => await SaveMeetingMinutes(body),
                "NonConformingWork" => await SaveNonConformingWork(body),
                "NcCorrectiveAction" => await SaveNcCorrectiveAction(body),
                "Retesting" => await SaveRetesting(body),
                "RiskAssessment" => await SaveRiskAssessment(body),
                "DocumentChangeRequest" => await SaveDocumentChangeRequest(body),
                "DocumentReview" => await SaveDocumentReview(body),
                "MasterDocument" => await SaveMasterDocument(body),
                "MeasurementUncertainty" => await SaveMeasurementUncertainty(body),
                "PtIlcPlan" => await SavePtIlcPlan(body),
                _ => throw new ArgumentException($"Unknown form type: {formType}")
            };
        }

        public async Task Remove(string formType, long id)
        {
            // Log audit before deletion
            await LogAudit(formType, id, "Deleted", null, null);
            await _repository.Delete(formType, id);
            _logger.LogInformation("{FormType} with ID {Id} deleted.", formType, id);
        }

        // ─── Workflow ────────────────────────────────────────────────────

        public async Task Submit(string formType, long id)
        {
            var entity = await GetEntityAsBase(formType, id);
            if (entity == null)
                throw new InvalidOperationException($"{formType} with ID {id} not found.");

            if (entity.Status != "Draft")
                throw new InvalidOperationException($"Only Draft records can be submitted. Current status: {entity.Status}");

            entity.Status = "Submitted";
            entity.PreparedById = loggedInUser.EmployeeID;
            entity.PreparedBy = loggedInUser.Name;
            entity.PreparedDate = DateTime.UtcNow;
            entity.ModifiedOn = DateTime.UtcNow;
            entity.ModifiedBy = loggedInUser.EmployeeID;

            await _context.SaveChangesAsync();
            await LogAudit(formType, id, "Submitted", null, null);
            _logger.LogInformation("{FormType} ID {Id} submitted by {User}.", formType, id, loggedInUser.Name);
        }

        public async Task Review(string formType, long id)
        {
            var entity = await GetEntityAsBase(formType, id);
            if (entity == null)
                throw new InvalidOperationException($"{formType} with ID {id} not found.");

            if (entity.Status != "Submitted")
                throw new InvalidOperationException($"Only Submitted records can be reviewed. Current status: {entity.Status}");

            entity.Status = "Reviewed";
            entity.ReviewedById = loggedInUser.EmployeeID;
            entity.ReviewedBy = loggedInUser.Name;
            entity.ReviewedDate = DateTime.UtcNow;
            entity.ModifiedOn = DateTime.UtcNow;
            entity.ModifiedBy = loggedInUser.EmployeeID;

            await _context.SaveChangesAsync();
            await LogAudit(formType, id, "Reviewed", null, null);
            _logger.LogInformation("{FormType} ID {Id} reviewed by {User}.", formType, id, loggedInUser.Name);
        }

        public async Task Approve(string formType, long id)
        {
            var entity = await GetEntityAsBase(formType, id);
            if (entity == null)
                throw new InvalidOperationException($"{formType} with ID {id} not found.");

            if (entity.Status != "Reviewed")
                throw new InvalidOperationException($"Only Reviewed records can be approved. Current status: {entity.Status}");

            entity.Status = "Approved";
            entity.ApprovedById = loggedInUser.EmployeeID;
            entity.ApprovedBy = loggedInUser.Name;
            entity.ApprovedDate = DateTime.UtcNow;
            entity.EffectiveDate = DateTime.UtcNow;
            entity.NextReviewDate = DateTime.UtcNow.AddMonths(entity.ReviewFrequencyMonths);
            entity.ModifiedOn = DateTime.UtcNow;
            entity.ModifiedBy = loggedInUser.EmployeeID;

            await _context.SaveChangesAsync();
            await LogAudit(formType, id, "Approved", null, null);
            _logger.LogInformation("{FormType} ID {Id} approved by {User}.", formType, id, loggedInUser.Name);
        }

        public async Task Reject(string formType, long id, string? remarks)
        {
            var entity = await GetEntityAsBase(formType, id);
            if (entity == null)
                throw new InvalidOperationException($"{formType} with ID {id} not found.");

            if (entity.Status == "Draft" || entity.Status == "Rejected")
                throw new InvalidOperationException($"Cannot reject a record with status: {entity.Status}");

            entity.Status = "Rejected";
            entity.RejectionRemarks = remarks;
            entity.ModifiedOn = DateTime.UtcNow;
            entity.ModifiedBy = loggedInUser.EmployeeID;

            await _context.SaveChangesAsync();
            await LogAudit(formType, id, "Rejected", null, JsonSerializer.Serialize(new { remarks }));
            _logger.LogInformation("{FormType} ID {Id} rejected by {User}.", formType, id, loggedInUser.Name);
        }

        // ─── History & Audit ─────────────────────────────────────────────

        public async Task<List<object>> GetRevisionHistory(string formType, long id)
        {
            var history = await _context.Set<NablFormRevisionHistory>()
                .Where(x => x.FormType == formType && x.FormDataId == id && x.IsActive)
                .OrderByDescending(x => x.RevisionDate)
                .ToListAsync();

            return history.Cast<object>().ToList();
        }

        public async Task<List<object>> GetAuditLog(string formType, long id)
        {
            var logs = await _context.Set<NablAuditLog>()
                .Where(x => x.FormType == formType && x.FormDataId == id)
                .OrderByDescending(x => x.PerformedOn)
                .ToListAsync();

            return logs.Cast<object>().ToList();
        }

        // ─── Private: Form-specific save logic ──────────────────────────

        private async Task<long> SaveJobDescription(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablJobDescription>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid JobDescription data.");

            if (model.ID == 0)
            {
                // CREATE
                model.FormCode = FormCodeMap["JobDescription"];
                await AssignDocumentNumber(model, "JobDescription");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("JobDescription", model);
                await LogAudit("JobDescription", id, "Created", null, body.GetRawText());
                _logger.LogInformation("JobDescription created with ID {Id}.", id);
                return id;
            }
            else
            {
                // UPDATE - get existing entity to preserve audit fields
                var existing = await _context.NablJobDescriptions
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("JobDescription not found!");

                // Save revision history (snapshot of previous version)
                await SaveRevisionSnapshot("JobDescription", existing);

                // Update fields
                existing.DesignationId = model.DesignationId;
                existing.DesignationName = model.DesignationName;
                existing.DepartmentId = model.DepartmentId;
                existing.DepartmentName = model.DepartmentName;
                existing.ReportingTo = model.ReportingTo;
                existing.MinimumQualification = model.MinimumQualification;
                existing.TechnicalTraining = model.TechnicalTraining;
                existing.Experience = model.Experience;
                existing.PrincipalAccountabilities = model.PrincipalAccountabilities;
                existing.AuthorityStopTesting = model.AuthorityStopTesting;
                existing.AuthorityIssueReports = model.AuthorityIssueReports;
                existing.AuthorityAccessConfidential = model.AuthorityAccessConfidential;
                existing.AuthorityEquipmentCalibration = model.AuthorityEquipmentCalibration;
                existing.QmsResponsibilities = model.QmsResponsibilities;
                existing.ConfidentialityClause = model.ConfidentialityClause;
                existing.PreparedByName = model.PreparedByName;
                existing.ApprovedByName = model.ApprovedByName;
                existing.EmployeeAccepted = model.EmployeeAccepted;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("JobDescription", existing);
                await LogAudit("JobDescription", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("JobDescription ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveResponsibilityAuthority(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablResponsibilityAuthority>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid ResponsibilityAuthority data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["ResponsibilityAuthority"];
                await AssignDocumentNumber(model, "ResponsibilityAuthority");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("ResponsibilityAuthority", model);
                await LogAudit("ResponsibilityAuthority", id, "Created", null, body.GetRawText());
                _logger.LogInformation("ResponsibilityAuthority created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablResponsibilityAuthorities
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("ResponsibilityAuthority not found!");

                await SaveRevisionSnapshot("ResponsibilityAuthority", existing);

                existing.DesignationId = model.DesignationId;
                existing.DesignationName = model.DesignationName;
                existing.Responsibilities = model.Responsibilities;
                existing.Authorities = model.Authorities;
                existing.EmployeeAccepted = model.EmployeeAccepted;
                existing.AcceptanceTimestamp = model.AcceptanceTimestamp;
                existing.EmployeeSignature = model.EmployeeSignature;
                existing.IssuedBy = model.IssuedBy;
                existing.ReviewedApprovedBy = model.ReviewedApprovedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("ResponsibilityAuthority", existing);
                await LogAudit("ResponsibilityAuthority", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("ResponsibilityAuthority ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveEmployeeCompetence(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablEmployeeCompetence>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid EmployeeCompetence data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["EmployeeCompetence"];
                await AssignDocumentNumber(model, "EmployeeCompetence");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("EmployeeCompetence", model);
                await LogAudit("EmployeeCompetence", id, "Created", null, body.GetRawText());
                _logger.LogInformation("EmployeeCompetence created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablEmployeeCompetences
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("EmployeeCompetence not found!");

                await SaveRevisionSnapshot("EmployeeCompetence", existing);

                existing.EmployeeId = model.EmployeeId;
                existing.EmployeeName = model.EmployeeName;
                existing.DesignationName = model.DesignationName;
                existing.EvaluationPeriodFrom = model.EvaluationPeriodFrom;
                existing.EvaluationPeriodTo = model.EvaluationPeriodTo;
                existing.ParametersJson = model.ParametersJson;
                existing.OverallRating = model.OverallRating;
                existing.SpecificTrainingRequired = model.SpecificTrainingRequired;
                existing.EvaluationDoneBy = model.EvaluationDoneBy;
                existing.EvaluationDate = model.EvaluationDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("EmployeeCompetence", existing);
                await LogAudit("EmployeeCompetence", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("EmployeeCompetence ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveEmployeePerformanceRecord(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablEmployeePerformanceRecord>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid EmployeePerformanceRecord data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["EmployeePerformanceRecord"];
                await AssignDocumentNumber(model, "EmployeePerformanceRecord");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("EmployeePerformanceRecord", model);
                await LogAudit("EmployeePerformanceRecord", id, "Created", null, body.GetRawText());
                _logger.LogInformation("EmployeePerformanceRecord created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablEmployeePerformanceRecords
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("EmployeePerformanceRecord not found!");

                await SaveRevisionSnapshot("EmployeePerformanceRecord", existing);

                existing.EmployeeId = model.EmployeeId;
                existing.EmployeeName = model.EmployeeName;
                existing.DesignationName = model.DesignationName;
                existing.ReviewPeriod = model.ReviewPeriod;
                existing.TechnicalRating = model.TechnicalRating;
                existing.BehavioralRating = model.BehavioralRating;
                existing.OverallRating = model.OverallRating;
                existing.ReviewerName = model.ReviewerName;
                existing.ReviewerId = model.ReviewerId;
                existing.ReviewDate = model.ReviewDate;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("EmployeePerformanceRecord", existing);
                await LogAudit("EmployeePerformanceRecord", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("EmployeePerformanceRecord ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveEmployeeAuthorization(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablEmployeeAuthorization>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid EmployeeAuthorization data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["EmployeeAuthorization"];
                await AssignDocumentNumber(model, "EmployeeAuthorization");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("EmployeeAuthorization", model);
                await LogAudit("EmployeeAuthorization", id, "Created", null, body.GetRawText());
                _logger.LogInformation("EmployeeAuthorization created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablEmployeeAuthorizations
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("EmployeeAuthorization not found!");

                await SaveRevisionSnapshot("EmployeeAuthorization", existing);

                existing.DepartmentId = model.DepartmentId;
                existing.DepartmentName = model.DepartmentName;
                existing.EmployeeId = model.EmployeeId;
                existing.PersonnelName = model.PersonnelName;
                existing.Uid = model.Uid;
                existing.Equipment = model.Equipment;
                existing.TestMethodAuthorization = model.TestMethodAuthorization;
                existing.TestAuthorization = model.TestAuthorization;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("EmployeeAuthorization", existing);
                await LogAudit("EmployeeAuthorization", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("EmployeeAuthorization ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveCompetenceRequirement(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablCompetenceRequirement>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid CompetenceRequirement data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["CompetenceRequirement"];
                await AssignDocumentNumber(model, "CompetenceRequirement");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("CompetenceRequirement", model);
                await LogAudit("CompetenceRequirement", id, "Created", null, body.GetRawText());
                _logger.LogInformation("CompetenceRequirement created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablCompetenceRequirements
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("CompetenceRequirement not found!");

                await SaveRevisionSnapshot("CompetenceRequirement", existing);

                existing.PositionId = model.PositionId;
                existing.PositionName = model.PositionName;
                existing.MinimumEducation = model.MinimumEducation;
                existing.MinimumExperience = model.MinimumExperience;
                existing.IsExternal = model.IsExternal;
                existing.RelatedActivity = model.RelatedActivity;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("CompetenceRequirement", existing);
                await LogAudit("CompetenceRequirement", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("CompetenceRequirement ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveInductionTraining(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablInductionTraining>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid InductionTraining data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["InductionTraining"];
                await AssignDocumentNumber(model, "InductionTraining");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("InductionTraining", model);
                await LogAudit("InductionTraining", id, "Created", null, body.GetRawText());
                _logger.LogInformation("InductionTraining created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablInductionTrainings
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("InductionTraining not found!");

                await SaveRevisionSnapshot("InductionTraining", existing);

                existing.EmployeeId = model.EmployeeId;
                existing.EmployeeName = model.EmployeeName;
                existing.Qualification = model.Qualification;
                existing.DateOfJoining = model.DateOfJoining;
                existing.Position = model.Position;
                existing.TrainingDate = model.TrainingDate;
                existing.TrainerName = model.TrainerName;
                existing.TrainerDesignation = model.TrainerDesignation;
                existing.SampleName = model.SampleName;
                existing.SampleRefNo = model.SampleRefNo;
                existing.Parameter = model.Parameter;
                existing.TestMethodSop = model.TestMethodSop;
                existing.EvaluationDate = model.EvaluationDate;
                existing.EvaluationMode = model.EvaluationMode;
                existing.EvalParameter = model.EvalParameter;
                existing.EvalTestMethodSop = model.EvalTestMethodSop;
                existing.ObservedValue1 = model.ObservedValue1;
                existing.ObservedValue2 = model.ObservedValue2;
                existing.ObservedValueAverage = model.ObservedValueAverage;
                existing.OriginalValue = model.OriginalValue;
                existing.Remarks = model.Remarks;
                existing.TrainerComments = model.TrainerComments;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("InductionTraining", existing);
                await LogAudit("InductionTraining", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("InductionTraining ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveSkillMatrix(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablSkillMatrix>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid SkillMatrix data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["SkillMatrix"];
                await AssignDocumentNumber(model, "SkillMatrix");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("SkillMatrix", model);
                await LogAudit("SkillMatrix", id, "Created", null, body.GetRawText());
                _logger.LogInformation("SkillMatrix created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablSkillMatrices
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("SkillMatrix not found!");

                await SaveRevisionSnapshot("SkillMatrix", existing);

                existing.DesignationId = model.DesignationId;
                existing.DesignationName = model.DesignationName;
                existing.Title = model.Title;
                existing.Decision = model.Decision;
                existing.SkillsJson = model.SkillsJson;
                existing.EmployeeSkillsJson = model.EmployeeSkillsJson;
                existing.IssuedBy = model.IssuedBy;
                existing.ReviewedApprovedBy = model.ReviewedApprovedBy;
                existing.LastUpdated = model.LastUpdated;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("SkillMatrix", existing);
                await LogAudit("SkillMatrix", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("SkillMatrix ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveSkillMatrixDecision(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablSkillMatrixDecision>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid SkillMatrixDecision data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["SkillMatrixDecision"];
                await AssignDocumentNumber(model, "SkillMatrixDecision");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("SkillMatrixDecision", model);
                await LogAudit("SkillMatrixDecision", id, "Created", null, body.GetRawText());
                _logger.LogInformation("SkillMatrixDecision created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablSkillMatrixDecisions
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("SkillMatrixDecision not found!");

                await SaveRevisionSnapshot("SkillMatrixDecision", existing);

                existing.DesignationId = model.DesignationId;
                existing.DesignationName = model.DesignationName;
                existing.Title = model.Title;
                existing.RowsJson = model.RowsJson;
                existing.IssuedBy = model.IssuedBy;
                existing.ReviewedApprovedBy = model.ReviewedApprovedBy;
                existing.LastUpdated = model.LastUpdated;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("SkillMatrixDecision", existing);
                await LogAudit("SkillMatrixDecision", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("SkillMatrixDecision ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        // ─── Private: Phase 3-15 save methods ───────────────────────────

        private async Task<long> SaveTrainingPlan(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablTrainingPlan>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid TrainingPlan data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["TrainingPlan"];
                await AssignDocumentNumber(model, "TrainingPlan");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("TrainingPlan", model);
                await LogAudit("TrainingPlan", id, "Created", null, body.GetRawText());
                _logger.LogInformation("TrainingPlan created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablTrainingPlans
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("TrainingPlan not found!");

                await SaveRevisionSnapshot("TrainingPlan", existing);

                existing.EmployeeId = model.EmployeeId;
                existing.EmployeeName = model.EmployeeName;
                existing.Department = model.Department;
                existing.TrainingTopic = model.TrainingTopic;
                existing.TrainingObjective = model.TrainingObjective;
                existing.TrainingType = model.TrainingType;
                existing.PlannedDate = model.PlannedDate;
                existing.ActualDate = model.ActualDate;
                existing.TrainerName = model.TrainerName;
                existing.TrainerDesignation = model.TrainerDesignation;
                existing.Duration = model.Duration;
                existing.VenueMode = model.VenueMode;
                existing.NeedIdentifiedBy = model.NeedIdentifiedBy;
                existing.TrainingStatus = model.TrainingStatus;
                existing.CompletionRemarks = model.CompletionRemarks;
                existing.PlanningYear = model.PlanningYear;
                existing.PlanDate = model.PlanDate;
                existing.TotalBudget = model.TotalBudget;
                existing.ApprovalStatus = model.ApprovalStatus;
                existing.CoursesJson = model.CoursesJson;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("TrainingPlan", existing);
                await LogAudit("TrainingPlan", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("TrainingPlan ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveTrainingAttendance(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablTrainingAttendance>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid TrainingAttendance data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["TrainingAttendance"];
                await AssignDocumentNumber(model, "TrainingAttendance");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("TrainingAttendance", model);
                await LogAudit("TrainingAttendance", id, "Created", null, body.GetRawText());
                _logger.LogInformation("TrainingAttendance created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablTrainingAttendances
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("TrainingAttendance not found!");

                await SaveRevisionSnapshot("TrainingAttendance", existing);

                existing.TrainingPlanId = model.TrainingPlanId;
                existing.TrainingTopic = model.TrainingTopic;
                existing.TrainingDate = model.TrainingDate;
                existing.TrainerName = model.TrainerName;
                existing.VenueMode = model.VenueMode;
                existing.AttendeesJson = model.AttendeesJson;
                existing.TotalAttendees = model.TotalAttendees;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("TrainingAttendance", existing);
                await LogAudit("TrainingAttendance", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("TrainingAttendance ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveTrainingEffectiveness(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablTrainingEffectiveness>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid TrainingEffectiveness data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["TrainingEffectiveness"];
                await AssignDocumentNumber(model, "TrainingEffectiveness");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("TrainingEffectiveness", model);
                await LogAudit("TrainingEffectiveness", id, "Created", null, body.GetRawText());
                _logger.LogInformation("TrainingEffectiveness created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablTrainingEffectivenesses
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("TrainingEffectiveness not found!");

                await SaveRevisionSnapshot("TrainingEffectiveness", existing);

                existing.TrainingPlanId = model.TrainingPlanId;
                existing.EmployeeId = model.EmployeeId;
                existing.EmployeeName = model.EmployeeName;
                existing.TrainingTopic = model.TrainingTopic;
                existing.TrainingDate = model.TrainingDate;
                existing.EvaluationMethod = model.EvaluationMethod;
                existing.EvaluationDate = model.EvaluationDate;
                existing.KnowledgeScore = model.KnowledgeScore;
                existing.SkillScore = model.SkillScore;
                existing.OverallScore = model.OverallScore;
                existing.EffectivenessResult = model.EffectivenessResult;
                existing.ActionRequired = model.ActionRequired;
                existing.ReEvaluationDate = model.ReEvaluationDate;
                existing.EvaluatedBy = model.EvaluatedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("TrainingEffectiveness", existing);
                await LogAudit("TrainingEffectiveness", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("TrainingEffectiveness ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveEnvironmentMonitoring(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablEnvironmentMonitoring>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid EnvironmentMonitoring data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["EnvironmentMonitoring"];
                await AssignDocumentNumber(model, "EnvironmentMonitoring");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("EnvironmentMonitoring", model);
                await LogAudit("EnvironmentMonitoring", id, "Created", null, body.GetRawText());
                _logger.LogInformation("EnvironmentMonitoring created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablEnvironmentMonitorings
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("EnvironmentMonitoring not found!");

                await SaveRevisionSnapshot("EnvironmentMonitoring", existing);

                existing.DepartmentId = model.DepartmentId;
                existing.DepartmentName = model.DepartmentName;
                existing.LabRoomId = model.LabRoomId;
                existing.RoomName = model.RoomName;
                existing.MonitoringMonth = model.MonitoringMonth;
                existing.MonitoringYear = model.MonitoringYear;
                existing.MonitoringDate = model.MonitoringDate;
                existing.TimeOfReading = model.TimeOfReading;
                existing.Temperature = model.Temperature;
                existing.Humidity = model.Humidity;
                existing.AcceptableTemperatureMin = model.AcceptableTemperatureMin;
                existing.AcceptableTemperatureMax = model.AcceptableTemperatureMax;
                existing.AcceptableHumidityMin = model.AcceptableHumidityMin;
                existing.AcceptableHumidityMax = model.AcceptableHumidityMax;
                existing.IsWithinLimits = model.IsWithinLimits;
                existing.CorrectiveAction = model.CorrectiveAction;
                existing.RecordedBy = model.RecordedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                // Save daily records if provided
                if (model.DailyRecords?.Any() == true)
                {
                    // Remove existing daily records for this header and re-add
                    var existingRecords = await _context.EnvironmentDailyRecords
                        .Where(d => d.EnvironmentMonitoringID == existing.ID)
                        .ToListAsync();
                    _context.EnvironmentDailyRecords.RemoveRange(existingRecords);

                    foreach (var record in model.DailyRecords)
                    {
                        record.EnvironmentMonitoringID = existing.ID;
                        record.CreatedOn = DateTime.UtcNow;
                        record.CreatedBy = loggedInUser.EmployeeID;
                        record.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";
                        record.IsActive = true;
                        _context.EnvironmentDailyRecords.Add(record);
                    }
                }

                await _repository.Update("EnvironmentMonitoring", existing);
                await LogAudit("EnvironmentMonitoring", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("EnvironmentMonitoring ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveQualityControlPlan(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablQualityControlPlan>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid QualityControlPlan data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["QualityControlPlan"];
                await AssignDocumentNumber(model, "QualityControlPlan");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("QualityControlPlan", model);
                await LogAudit("QualityControlPlan", id, "Created", null, body.GetRawText());
                _logger.LogInformation("QualityControlPlan created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablQualityControlPlans
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("QualityControlPlan not found!");

                await SaveRevisionSnapshot("QualityControlPlan", existing);

                existing.DepartmentId = model.DepartmentId;
                existing.DepartmentName = model.DepartmentName;
                existing.TestParameter = model.TestParameter;
                existing.TestMethod = model.TestMethod;
                existing.ControlType = model.ControlType;
                existing.Frequency = model.Frequency;
                existing.FrequencyUnit = model.FrequencyUnit;
                existing.ResponsiblePerson = model.ResponsiblePerson;
                existing.AcceptanceCriteria = model.AcceptanceCriteria;
                existing.NextDueDate = model.NextDueDate;
                existing.LastPerformedDate = model.LastPerformedDate;
                existing.ActionOnFailure = model.ActionOnFailure;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("QualityControlPlan", existing);
                await LogAudit("QualityControlPlan", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("QualityControlPlan ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveTestRequest(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablTestRequest>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid TestRequest data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["TestRequest"];
                await AssignDocumentNumber(model, "TestRequest");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("TestRequest", model);
                await LogAudit("TestRequest", id, "Created", null, body.GetRawText());
                _logger.LogInformation("TestRequest created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablTestRequests
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("TestRequest not found!");

                await SaveRevisionSnapshot("TestRequest", existing);

                existing.CustomerId = model.CustomerId;
                existing.CustomerName = model.CustomerName;
                existing.SampleDescription = model.SampleDescription;
                existing.SampleQuantity = model.SampleQuantity;
                existing.SampleCondition = model.SampleCondition;
                existing.RequestDate = model.RequestDate;
                existing.RequiredByDate = model.RequiredByDate;
                existing.TestParametersJson = model.TestParametersJson;
                existing.SpecialRequirements = model.SpecialRequirements;
                existing.ContactPerson = model.ContactPerson;
                existing.ContactPhone = model.ContactPhone;
                existing.ContactEmail = model.ContactEmail;
                existing.ReferenceStandard = model.ReferenceStandard;
                existing.TestPurpose = model.TestPurpose;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("TestRequest", existing);
                await LogAudit("TestRequest", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("TestRequest ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveTestMethod(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablTestMethod>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid TestMethod data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["TestMethod"];
                await AssignDocumentNumber(model, "TestMethod");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("TestMethod", model);
                await LogAudit("TestMethod", id, "Created", null, body.GetRawText());
                _logger.LogInformation("TestMethod created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablTestMethods
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("TestMethod not found!");

                await SaveRevisionSnapshot("TestMethod", existing);

                existing.TestMethodStandardId = model.TestMethodStandardId;
                existing.TestMethodCode = model.TestMethodCode;
                existing.TestMethodTitle = model.TestMethodTitle;
                existing.TestParameter = model.TestParameter;
                existing.TestMatrix = model.TestMatrix;
                existing.Scope = model.Scope;
                existing.Principle = model.Principle;
                existing.ApplicableStandard = model.ApplicableStandard;
                existing.EquipmentRequired = model.EquipmentRequired;
                existing.ReagentsRequired = model.ReagentsRequired;
                existing.SamplePreparation = model.SamplePreparation;
                existing.Procedure = model.Procedure;
                existing.CalibrationRequirements = model.CalibrationRequirements;
                existing.QualityControlRequirements = model.QualityControlRequirements;
                existing.AcceptanceCriteria = model.AcceptanceCriteria;
                existing.UncertaintyStatement = model.UncertaintyStatement;
                existing.DetectionLimit = model.DetectionLimit;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("TestMethod", existing);
                await LogAudit("TestMethod", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("TestMethod ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveMethodVerification(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablMethodVerification>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid MethodVerification data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["MethodVerification"];
                await AssignDocumentNumber(model, "MethodVerification");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("MethodVerification", model);
                await LogAudit("MethodVerification", id, "Created", null, body.GetRawText());
                _logger.LogInformation("MethodVerification created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablMethodVerifications
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("MethodVerification not found!");

                await SaveRevisionSnapshot("MethodVerification", existing);

                existing.TestMethodCode = model.TestMethodCode;
                existing.TestParameter = model.TestParameter;
                existing.TestMatrix = model.TestMatrix;
                existing.VerificationDate = model.VerificationDate;
                existing.VerificationType = model.VerificationType;
                existing.LinearityResults = model.LinearityResults;
                existing.PrecisionResults = model.PrecisionResults;
                existing.BiasResults = model.BiasResults;
                existing.UncertaintyResults = model.UncertaintyResults;
                existing.OverallConclusion = model.OverallConclusion;
                existing.VerifiedBy = model.VerifiedBy;
                existing.NextVerificationDate = model.NextVerificationDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("MethodVerification", existing);
                await LogAudit("MethodVerification", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("MethodVerification ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveMethodValidation(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablMethodValidation>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid MethodValidation data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["MethodValidation"];
                await AssignDocumentNumber(model, "MethodValidation");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("MethodValidation", model);
                await LogAudit("MethodValidation", id, "Created", null, body.GetRawText());
                _logger.LogInformation("MethodValidation created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablMethodValidations
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("MethodValidation not found!");

                await SaveRevisionSnapshot("MethodValidation", existing);

                existing.TestMethodCode = model.TestMethodCode;
                existing.TestParameter = model.TestParameter;
                existing.TestMatrix = model.TestMatrix;
                existing.ValidationDate = model.ValidationDate;
                existing.ValidationScope = model.ValidationScope;
                existing.SelectivityResults = model.SelectivityResults;
                existing.LinearityRange = model.LinearityRange;
                existing.DetectionLimit = model.DetectionLimit;
                existing.QuantificationLimit = model.QuantificationLimit;
                existing.PrecisionRSD = model.PrecisionRSD;
                existing.BiasPercentage = model.BiasPercentage;
                existing.RobustnessResults = model.RobustnessResults;
                existing.UncertaintyResults = model.UncertaintyResults;
                existing.OverallConclusion = model.OverallConclusion;
                existing.ValidatedBy = model.ValidatedBy;
                existing.NextValidationDate = model.NextValidationDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("MethodValidation", existing);
                await LogAudit("MethodValidation", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("MethodValidation ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveSampleInwardRegister(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablSampleInwardRegister>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid SampleInwardRegister data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["SampleInwardRegister"];
                await AssignDocumentNumber(model, "SampleInwardRegister");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("SampleInwardRegister", model);
                await LogAudit("SampleInwardRegister", id, "Created", null, body.GetRawText());
                _logger.LogInformation("SampleInwardRegister created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablSampleInwardRegisters
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("SampleInwardRegister not found!");

                await SaveRevisionSnapshot("SampleInwardRegister", existing);

                existing.SampleInwardId = model.SampleInwardId;
                existing.SampleCode = model.SampleCode;
                existing.CustomerName = model.CustomerName;
                existing.SampleDescription = model.SampleDescription;
                existing.ReceivedDate = model.ReceivedDate;
                existing.ReceivedBy = model.ReceivedBy;
                existing.SampleCondition = model.SampleCondition;
                existing.StorageLocation = model.StorageLocation;
                existing.TestsRequested = model.TestsRequested;
                existing.TargetCompletionDate = model.TargetCompletionDate;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("SampleInwardRegister", existing);
                await LogAudit("SampleInwardRegister", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("SampleInwardRegister ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveSampleMusterRegister(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablSampleMusterRegister>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid SampleMusterRegister data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["SampleMusterRegister"];
                await AssignDocumentNumber(model, "SampleMusterRegister");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("SampleMusterRegister", model);
                await LogAudit("SampleMusterRegister", id, "Created", null, body.GetRawText());
                _logger.LogInformation("SampleMusterRegister created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablSampleMusterRegisters
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("SampleMusterRegister not found!");

                await SaveRevisionSnapshot("SampleMusterRegister", existing);

                existing.SampleCode = model.SampleCode;
                existing.SampleDescription = model.SampleDescription;
                existing.MusteringDate = model.MusteringDate;
                existing.MusteredBy = model.MusteredBy;
                existing.NumberOfPieces = model.NumberOfPieces;
                existing.SampleDimensions = model.SampleDimensions;
                existing.CuttingInstructions = model.CuttingInstructions;
                existing.PreparedSamples = model.PreparedSamples;
                existing.WasteGenerated = model.WasteGenerated;
                existing.DisposalMethod = model.DisposalMethod;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("SampleMusterRegister", existing);
                await LogAudit("SampleMusterRegister", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("SampleMusterRegister ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveSampleLabel(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablSampleLabel>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid SampleLabel data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["SampleLabel"];
                await AssignDocumentNumber(model, "SampleLabel");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("SampleLabel", model);
                await LogAudit("SampleLabel", id, "Created", null, body.GetRawText());
                _logger.LogInformation("SampleLabel created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablSampleLabels
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("SampleLabel not found!");

                await SaveRevisionSnapshot("SampleLabel", existing);

                existing.SampleCode = model.SampleCode;
                existing.SampleDescription = model.SampleDescription;
                existing.CustomerName = model.CustomerName;
                existing.ReceivedDate = model.ReceivedDate;
                existing.TestParameter = model.TestParameter;
                existing.LabelNo = model.LabelNo;
                existing.StorageCondition = model.StorageCondition;
                existing.ExpiryDate = model.ExpiryDate;
                existing.LabelledBy = model.LabelledBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("SampleLabel", existing);
                await LogAudit("SampleLabel", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("SampleLabel ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveTechnicalRawData(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablTechnicalRawData>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid TechnicalRawData data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["TechnicalRawData"];
                await AssignDocumentNumber(model, "TechnicalRawData");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("TechnicalRawData", model);
                await LogAudit("TechnicalRawData", id, "Created", null, body.GetRawText());
                _logger.LogInformation("TechnicalRawData created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablTechnicalRawDatas
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("TechnicalRawData not found!");

                await SaveRevisionSnapshot("TechnicalRawData", existing);

                existing.SampleCode = model.SampleCode;
                existing.TestParameter = model.TestParameter;
                existing.TestMethod = model.TestMethod;
                existing.EquipmentId = model.EquipmentId;
                existing.ObservationDate = model.ObservationDate;
                existing.ObservationsJson = model.ObservationsJson;
                existing.CalculatedResult = model.CalculatedResult;
                existing.Unit = model.Unit;
                existing.Uncertainty = model.Uncertainty;
                existing.RawDataFile = model.RawDataFile;
                existing.TestedBy = model.TestedBy;
                existing.CheckedBy = model.CheckedBy;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("TechnicalRawData", existing);
                await LogAudit("TechnicalRawData", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("TechnicalRawData ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveTestReport(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablTestReport>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid TestReport data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["TestReport"];
                await AssignDocumentNumber(model, "TestReport");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("TestReport", model);
                await LogAudit("TestReport", id, "Created", null, body.GetRawText());
                _logger.LogInformation("TestReport created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablTestReports
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("TestReport not found!");

                await SaveRevisionSnapshot("TestReport", existing);

                existing.SampleCode = model.SampleCode;
                existing.CustomerName = model.CustomerName;
                existing.ReportDate = model.ReportDate;
                existing.TestResultsJson = model.TestResultsJson;
                existing.SamplingDetails = model.SamplingDetails;
                existing.MethodReference = model.MethodReference;
                existing.Conclusion = model.Conclusion;
                existing.Disclaimer = model.Disclaimer;
                existing.IssuedBy = model.IssuedBy;
                existing.IssuedDate = model.IssuedDate;
                existing.ReportVersion = model.ReportVersion;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("TestReport", existing);
                await LogAudit("TestReport", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("TestReport ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveEquipmentHistory(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablEquipmentHistory>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid EquipmentHistory data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["EquipmentHistory"];
                await AssignDocumentNumber(model, "EquipmentHistory");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("EquipmentHistory", model);
                await LogAudit("EquipmentHistory", id, "Created", null, body.GetRawText());
                _logger.LogInformation("EquipmentHistory created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablEquipmentHistories
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("EquipmentHistory not found!");

                await SaveRevisionSnapshot("EquipmentHistory", existing);

                existing.EquipmentId = model.EquipmentId;
                existing.EquipmentCode = model.EquipmentCode;
                existing.EquipmentName = model.EquipmentName;
                existing.Manufacturer = model.Manufacturer;
                existing.ModelNo = model.ModelNo;
                existing.SerialNo = model.SerialNo;
                existing.PurchaseDate = model.PurchaseDate;
                existing.InstallationDate = model.InstallationDate;
                existing.Location = model.Location;
                existing.CalibrationFrequency = model.CalibrationFrequency;
                existing.LastCalibrationDate = model.LastCalibrationDate;
                existing.NextCalibrationDate = model.NextCalibrationDate;
                existing.CalibrationAgency = model.CalibrationAgency;
                existing.MaintenanceRecordsJson = model.MaintenanceRecordsJson;
                existing.CurrentStatus = model.CurrentStatus;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("EquipmentHistory", existing);
                await LogAudit("EquipmentHistory", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("EquipmentHistory ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveCalibrationReview(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablCalibrationReview>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid CalibrationReview data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["CalibrationReview"];
                await AssignDocumentNumber(model, "CalibrationReview");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("CalibrationReview", model);
                await LogAudit("CalibrationReview", id, "Created", null, body.GetRawText());
                _logger.LogInformation("CalibrationReview created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablCalibrationReviews
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("CalibrationReview not found!");

                await SaveRevisionSnapshot("CalibrationReview", existing);

                existing.EquipmentId = model.EquipmentId;
                existing.EquipmentCode = model.EquipmentCode;
                existing.EquipmentName = model.EquipmentName;
                existing.CalibrationDate = model.CalibrationDate;
                existing.CalibrationAgencyName = model.CalibrationAgencyName;
                existing.CertificateNo = model.CertificateNo;
                existing.CalibrationDueDate = model.CalibrationDueDate;
                existing.CalibrationResult = model.CalibrationResult;
                existing.CalibrationDataJson = model.CalibrationDataJson;
                existing.ReviewedByName = model.ReviewedByName;
                existing.ReviewConclusion = model.ReviewConclusion;
                existing.CorrectiveAction = model.CorrectiveAction;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("CalibrationReview", existing);
                await LogAudit("CalibrationReview", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("CalibrationReview ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveIntermediateCheck(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablIntermediateCheck>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid IntermediateCheck data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["IntermediateCheck"];
                await AssignDocumentNumber(model, "IntermediateCheck");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("IntermediateCheck", model);
                await LogAudit("IntermediateCheck", id, "Created", null, body.GetRawText());
                _logger.LogInformation("IntermediateCheck created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablIntermediateChecks
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("IntermediateCheck not found!");

                await SaveRevisionSnapshot("IntermediateCheck", existing);

                existing.EquipmentId = model.EquipmentId;
                existing.EquipmentCode = model.EquipmentCode;
                existing.CheckDate = model.CheckDate;
                existing.CheckMethod = model.CheckMethod;
                existing.ReferenceStandard = model.ReferenceStandard;
                existing.ObservedValue = model.ObservedValue;
                existing.AcceptedValue = model.AcceptedValue;
                existing.Tolerance = model.Tolerance;
                existing.ResultStatus = model.ResultStatus;
                existing.CheckedBy = model.CheckedBy;
                existing.CorrectiveAction = model.CorrectiveAction;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("IntermediateCheck", existing);
                await LogAudit("IntermediateCheck", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("IntermediateCheck ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveReferenceMaterial(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablReferenceMaterial>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid ReferenceMaterial data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["ReferenceMaterial"];
                await AssignDocumentNumber(model, "ReferenceMaterial");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("ReferenceMaterial", model);
                await LogAudit("ReferenceMaterial", id, "Created", null, body.GetRawText());
                _logger.LogInformation("ReferenceMaterial created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablReferenceMaterials
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("ReferenceMaterial not found!");

                await SaveRevisionSnapshot("ReferenceMaterial", existing);

                existing.RMCode = model.RMCode;
                existing.RMName = model.RMName;
                existing.Manufacturer = model.Manufacturer;
                existing.BatchNo = model.BatchNo;
                existing.CertificateNo = model.CertificateNo;
                existing.ReceivedDate = model.ReceivedDate;
                existing.ExpiryDate = model.ExpiryDate;
                existing.StorageCondition = model.StorageCondition;
                existing.CertifiedValue = model.CertifiedValue;
                existing.Uncertainty = model.Uncertainty;
                existing.Unit = model.Unit;
                existing.Purpose = model.Purpose;
                existing.SupplierId = model.SupplierId;
                existing.RemainingQuantity = model.RemainingQuantity;
                existing.QuantityUnit = model.QuantityUnit;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("ReferenceMaterial", existing);
                await LogAudit("ReferenceMaterial", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("ReferenceMaterial ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveCrmConsumption(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablCrmConsumption>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid CrmConsumption data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["CrmConsumption"];
                await AssignDocumentNumber(model, "CrmConsumption");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("CrmConsumption", model);
                await LogAudit("CrmConsumption", id, "Created", null, body.GetRawText());
                _logger.LogInformation("CrmConsumption created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablCrmConsumptions
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("CrmConsumption not found!");

                await SaveRevisionSnapshot("CrmConsumption", existing);

                existing.ReferenceMaterialId = model.ReferenceMaterialId;
                existing.RMCode = model.RMCode;
                existing.RMName = model.RMName;
                existing.UsageDate = model.UsageDate;
                existing.QuantityUsed = model.QuantityUsed;
                existing.QuantityUnit = model.QuantityUnit;
                existing.PurposeOfUse = model.PurposeOfUse;
                existing.UsedBy = model.UsedBy;
                existing.RemainingAfterUse = model.RemainingAfterUse;
                existing.IsExhausted = model.IsExhausted;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("CrmConsumption", existing);
                await LogAudit("CrmConsumption", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("CrmConsumption ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveSupplierRegistration(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablSupplierRegistration>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid SupplierRegistration data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["SupplierRegistration"];
                await AssignDocumentNumber(model, "SupplierRegistration");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("SupplierRegistration", model);
                await LogAudit("SupplierRegistration", id, "Created", null, body.GetRawText());
                _logger.LogInformation("SupplierRegistration created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablSupplierRegistrations
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("SupplierRegistration not found!");

                await SaveRevisionSnapshot("SupplierRegistration", existing);

                existing.SupplierId = model.SupplierId;
                existing.SupplierName = model.SupplierName;
                existing.SupplierCode = model.SupplierCode;
                existing.ContactPerson = model.ContactPerson;
                existing.Address = model.Address;
                existing.Phone = model.Phone;
                existing.Email = model.Email;
                existing.SupplierCategory = model.SupplierCategory;
                existing.ItemsSupplied = model.ItemsSupplied;
                existing.RegistrationDate = model.RegistrationDate;
                existing.RegistrationValidUpto = model.RegistrationValidUpto;
                existing.NablApproved = model.NablApproved;
                existing.BankDetails = model.BankDetails;
                existing.Designation = model.Designation;
                existing.MobileNo = model.MobileNo;
                existing.Website = model.Website;
                existing.NatureOfBusiness = model.NatureOfBusiness;
                existing.ProductsServicesOffered = model.ProductsServicesOffered;
                existing.GstNo = model.GstNo;
                existing.PanNo = model.PanNo;
                existing.IsoCertified = model.IsoCertified;
                existing.IsoDetails = model.IsoDetails;
                existing.BankDetailsJson = model.BankDetailsJson;
                existing.DocumentsSubmittedJson = model.DocumentsSubmittedJson;
                existing.RegistrationStatus = model.RegistrationStatus;
                existing.Remarks = model.Remarks;
                existing.RecordedBy = model.RecordedBy;
                existing.VerifiedBy = model.VerifiedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("SupplierRegistration", existing);
                await LogAudit("SupplierRegistration", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("SupplierRegistration ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveSupplierEvaluation(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablSupplierEvaluation>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid SupplierEvaluation data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["SupplierEvaluation"];
                await AssignDocumentNumber(model, "SupplierEvaluation");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("SupplierEvaluation", model);
                await LogAudit("SupplierEvaluation", id, "Created", null, body.GetRawText());
                _logger.LogInformation("SupplierEvaluation created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablSupplierEvaluations
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("SupplierEvaluation not found!");

                await SaveRevisionSnapshot("SupplierEvaluation", existing);

                existing.SupplierId = model.SupplierId;
                existing.SupplierName = model.SupplierName;
                existing.EvaluationDate = model.EvaluationDate;
                existing.EvaluationCriteria = model.EvaluationCriteria;
                existing.TotalScore = model.TotalScore;
                existing.MaxScore = model.MaxScore;
                existing.PercentageScore = model.PercentageScore;
                existing.EvaluationResult = model.EvaluationResult;
                existing.EvaluatedBy = model.EvaluatedBy;
                existing.NextEvaluationDate = model.NextEvaluationDate;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("SupplierEvaluation", existing);
                await LogAudit("SupplierEvaluation", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("SupplierEvaluation ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveApprovedSupplier(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablApprovedSupplier>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid ApprovedSupplier data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["ApprovedSupplier"];
                await AssignDocumentNumber(model, "ApprovedSupplier");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("ApprovedSupplier", model);
                await LogAudit("ApprovedSupplier", id, "Created", null, body.GetRawText());
                _logger.LogInformation("ApprovedSupplier created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablApprovedSuppliers
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("ApprovedSupplier not found!");

                await SaveRevisionSnapshot("ApprovedSupplier", existing);

                existing.SupplierId = model.SupplierId;
                existing.SupplierName = model.SupplierName;
                existing.ItemsApproved = model.ItemsApproved;
                existing.ApprovalDate = model.ApprovalDate;
                existing.ApprovalValidUpto = model.ApprovalValidUpto;
                existing.ApprovalCategory = model.ApprovalCategory;
                existing.PerformanceRating = model.PerformanceRating;
                existing.LastReviewDate = model.LastReviewDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("ApprovedSupplier", existing);
                await LogAudit("ApprovedSupplier", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("ApprovedSupplier ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveSupplierConfidentiality(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablSupplierConfidentiality>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid SupplierConfidentiality data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["SupplierConfidentiality"];
                await AssignDocumentNumber(model, "SupplierConfidentiality");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("SupplierConfidentiality", model);
                await LogAudit("SupplierConfidentiality", id, "Created", null, body.GetRawText());
                _logger.LogInformation("SupplierConfidentiality created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablSupplierConfidentialities
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("SupplierConfidentiality not found!");

                await SaveRevisionSnapshot("SupplierConfidentiality", existing);

                existing.SupplierId = model.SupplierId;
                existing.SupplierName = model.SupplierName;
                existing.ContactPerson = model.ContactPerson;
                existing.AgreementDate = model.AgreementDate;
                existing.AgreementValidUpto = model.AgreementValidUpto;
                existing.ConfidentialItems = model.ConfidentialItems;
                existing.PenaltyClause = model.PenaltyClause;
                existing.SignedBy = model.SignedBy;
                existing.SupplierSignature = model.SupplierSignature;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("SupplierConfidentiality", existing);
                await LogAudit("SupplierConfidentiality", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("SupplierConfidentiality ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveIncomingMaterial(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablIncomingMaterial>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid IncomingMaterial data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["IncomingMaterial"];
                await AssignDocumentNumber(model, "IncomingMaterial");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("IncomingMaterial", model);
                await LogAudit("IncomingMaterial", id, "Created", null, body.GetRawText());
                _logger.LogInformation("IncomingMaterial created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablIncomingMaterials
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("IncomingMaterial not found!");

                await SaveRevisionSnapshot("IncomingMaterial", existing);

                existing.SupplierId = model.SupplierId;
                existing.SupplierName = model.SupplierName;
                existing.PurchaseOrderNo = model.PurchaseOrderNo;
                existing.ReceivedDate = model.ReceivedDate;
                existing.ReceivedBy = model.ReceivedBy;
                existing.ItemDescription = model.ItemDescription;
                existing.Quantity = model.Quantity;
                existing.Unit = model.Unit;
                existing.BatchNo = model.BatchNo;
                existing.InspectionDate = model.InspectionDate;
                existing.InspectionBy = model.InspectionBy;
                existing.InspectionResult = model.InspectionResult;
                existing.Remarks = model.Remarks;
                existing.StorageLocation = model.StorageLocation;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("IncomingMaterial", existing);
                await LogAudit("IncomingMaterial", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("IncomingMaterial ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveProductInspection(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablProductInspection>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid ProductInspection data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["ProductInspection"];
                await AssignDocumentNumber(model, "ProductInspection");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("ProductInspection", model);
                await LogAudit("ProductInspection", id, "Created", null, body.GetRawText());
                _logger.LogInformation("ProductInspection created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablProductInspections
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("ProductInspection not found!");

                await SaveRevisionSnapshot("ProductInspection", existing);

                existing.SupplierId = model.SupplierId;
                existing.SupplierName = model.SupplierName;
                existing.ItemDescription = model.ItemDescription;
                existing.PurchaseOrderNo = model.PurchaseOrderNo;
                existing.InspectionDate = model.InspectionDate;
                existing.InspectionBy = model.InspectionBy;
                existing.SampleSize = model.SampleSize;
                existing.DefectsFound = model.DefectsFound;
                existing.InspectionCriteria = model.InspectionCriteria;
                existing.InspectionResultsJson = model.InspectionResultsJson;
                existing.OverallResult = model.OverallResult;
                existing.CorrectiveAction = model.CorrectiveAction;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("ProductInspection", existing);
                await LogAudit("ProductInspection", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("ProductInspection ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SavePurchaseIndent(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablPurchaseIndent>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid PurchaseIndent data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["PurchaseIndent"];
                await AssignDocumentNumber(model, "PurchaseIndent");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("PurchaseIndent", model);
                await LogAudit("PurchaseIndent", id, "Created", null, body.GetRawText());
                _logger.LogInformation("PurchaseIndent created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablPurchaseIndents
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("PurchaseIndent not found!");

                await SaveRevisionSnapshot("PurchaseIndent", existing);

                existing.IndentDate = model.IndentDate;
                existing.RequestedBy = model.RequestedBy;
                existing.DepartmentId = model.DepartmentId;
                existing.DepartmentName = model.DepartmentName;
                existing.ItemsJson = model.ItemsJson;
                existing.Justification = model.Justification;
                existing.RequiredByDate = model.RequiredByDate;
                existing.Priority = model.Priority;
                existing.ApprovedBy = model.ApprovedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("PurchaseIndent", existing);
                await LogAudit("PurchaseIndent", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("PurchaseIndent ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SavePurchaseOrder(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablPurchaseOrder>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid PurchaseOrder data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["PurchaseOrder"];
                await AssignDocumentNumber(model, "PurchaseOrder");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("PurchaseOrder", model);
                await LogAudit("PurchaseOrder", id, "Created", null, body.GetRawText());
                _logger.LogInformation("PurchaseOrder created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablPurchaseOrders
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("PurchaseOrder not found!");

                await SaveRevisionSnapshot("PurchaseOrder", existing);

                existing.SupplierId = model.SupplierId;
                existing.SupplierName = model.SupplierName;
                existing.PurchaseIndentId = model.PurchaseIndentId;
                existing.PODate = model.PODate;
                existing.DeliveryDate = model.DeliveryDate;
                existing.PaymentTerms = model.PaymentTerms;
                existing.ItemsJson = model.ItemsJson;
                existing.TotalAmount = model.TotalAmount;
                existing.Currency = model.Currency;
                existing.SpecialInstructions = model.SpecialInstructions;
                existing.IssuedBy = model.IssuedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("PurchaseOrder", existing);
                await LogAudit("PurchaseOrder", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("PurchaseOrder ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SavePurchaseMaterialVerification(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablPurchaseMaterialVerification>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid PurchaseMaterialVerification data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["PurchaseMaterialVerification"];
                await AssignDocumentNumber(model, "PurchaseMaterialVerification");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("PurchaseMaterialVerification", model);
                await LogAudit("PurchaseMaterialVerification", id, "Created", null, body.GetRawText());
                _logger.LogInformation("PurchaseMaterialVerification created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablPurchaseMaterialVerifications
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("PurchaseMaterialVerification not found!");

                await SaveRevisionSnapshot("PurchaseMaterialVerification", existing);

                existing.PurchaseOrderId = model.PurchaseOrderId;
                existing.PONumber = model.PONumber;
                existing.ReceivedDate = model.ReceivedDate;
                existing.VerificationDate = model.VerificationDate;
                existing.VerifiedBy = model.VerifiedBy;
                existing.ItemsVerificationJson = model.ItemsVerificationJson;
                existing.OverallStatus = model.OverallStatus;
                existing.GRNNumber = model.GRNNumber;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("PurchaseMaterialVerification", existing);
                await LogAudit("PurchaseMaterialVerification", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("PurchaseMaterialVerification ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveComplaint(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablComplaint>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid Complaint data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["Complaint"];
                await AssignDocumentNumber(model, "Complaint");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("Complaint", model);
                await LogAudit("Complaint", id, "Created", null, body.GetRawText());
                _logger.LogInformation("Complaint created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablComplaints
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("Complaint not found!");

                await SaveRevisionSnapshot("Complaint", existing);

                existing.CustomerId = model.CustomerId;
                existing.CustomerName = model.CustomerName;
                existing.ComplaintDate = model.ComplaintDate;
                existing.ComplaintDescription = model.ComplaintDescription;
                existing.ComplaintCategory = model.ComplaintCategory;
                existing.SampleCode = model.SampleCode;
                existing.ReportNo = model.ReportNo;
                existing.ReceivedBy = model.ReceivedBy;
                existing.InvestigationDate = model.InvestigationDate;
                existing.RootCause = model.RootCause;
                existing.CorrectiveAction = model.CorrectiveAction;
                existing.PreventiveAction = model.PreventiveAction;
                existing.ClosureDate = model.ClosureDate;
                existing.ClosedBy = model.ClosedBy;
                existing.CustomerInformedDate = model.CustomerInformedDate;
                existing.CustomerSatisfied = model.CustomerSatisfied;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("Complaint", existing);
                await LogAudit("Complaint", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("Complaint ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveCustomerFeedback(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablCustomerFeedback>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid CustomerFeedback data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["CustomerFeedback"];
                await AssignDocumentNumber(model, "CustomerFeedback");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("CustomerFeedback", model);
                await LogAudit("CustomerFeedback", id, "Created", null, body.GetRawText());
                _logger.LogInformation("CustomerFeedback created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablCustomerFeedbacks
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("CustomerFeedback not found!");

                await SaveRevisionSnapshot("CustomerFeedback", existing);

                existing.CustomerId = model.CustomerId;
                existing.CustomerName = model.CustomerName;
                existing.FeedbackDate = model.FeedbackDate;
                existing.FeedbackPeriodFrom = model.FeedbackPeriodFrom;
                existing.FeedbackPeriodTo = model.FeedbackPeriodTo;
                existing.OverallSatisfaction = model.OverallSatisfaction;
                existing.TurnaroundRating = model.TurnaroundRating;
                existing.AccuracyRating = model.AccuracyRating;
                existing.CommunicationRating = model.CommunicationRating;
                existing.ServiceRating = model.ServiceRating;
                existing.CommentsSuggestions = model.CommentsSuggestions;
                existing.WouldRecommend = model.WouldRecommend;
                existing.CollectedBy = model.CollectedBy;
                existing.ContactPerson = model.ContactPerson;
                existing.RatingsJson = model.RatingsJson;
                existing.Suggestions = model.Suggestions;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("CustomerFeedback", existing);
                await LogAudit("CustomerFeedback", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("CustomerFeedback ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveFeedbackAnalysis(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablFeedbackAnalysis>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid FeedbackAnalysis data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["FeedbackAnalysis"];
                await AssignDocumentNumber(model, "FeedbackAnalysis");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("FeedbackAnalysis", model);
                await LogAudit("FeedbackAnalysis", id, "Created", null, body.GetRawText());
                _logger.LogInformation("FeedbackAnalysis created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablFeedbackAnalyses
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("FeedbackAnalysis not found!");

                await SaveRevisionSnapshot("FeedbackAnalysis", existing);

                existing.AnalysisPeriodFrom = model.AnalysisPeriodFrom;
                existing.AnalysisPeriodTo = model.AnalysisPeriodTo;
                existing.TotalFeedbacks = model.TotalFeedbacks;
                existing.AverageSatisfaction = model.AverageSatisfaction;
                existing.AverageTurnaround = model.AverageTurnaround;
                existing.AverageAccuracy = model.AverageAccuracy;
                existing.AverageCommunication = model.AverageCommunication;
                existing.AverageService = model.AverageService;
                existing.OverallScore = model.OverallScore;
                existing.AcceptanceCriteria = model.AcceptanceCriteria;
                existing.MeetsAcceptanceCriteria = model.MeetsAcceptanceCriteria;
                existing.KeyStrengths = model.KeyStrengths;
                existing.AreasForImprovement = model.AreasForImprovement;
                existing.ActionPlan = model.ActionPlan;
                existing.AnalysedBy = model.AnalysedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("FeedbackAnalysis", existing);
                await LogAudit("FeedbackAnalysis", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("FeedbackAnalysis ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveAuditPlan(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablAuditPlan>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid AuditPlan data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["AuditPlan"];
                await AssignDocumentNumber(model, "AuditPlan");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("AuditPlan", model);
                await LogAudit("AuditPlan", id, "Created", null, body.GetRawText());
                _logger.LogInformation("AuditPlan created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablAuditPlans
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("AuditPlan not found!");

                await SaveRevisionSnapshot("AuditPlan", existing);

                existing.AuditYear = model.AuditYear;
                existing.AuditScheduleJson = model.AuditScheduleJson;
                existing.AuditObjective = model.AuditObjective;
                existing.AuditCriteria = model.AuditCriteria;
                existing.AuditScope = model.AuditScope;
                existing.LeadAuditorId = model.LeadAuditorId;
                existing.LeadAuditorName = model.LeadAuditorName;
                existing.AuditType = model.AuditType;
                existing.Period = model.Period;
                existing.AreaDepartment = model.AreaDepartment;
                existing.AuditorName = model.AuditorName;
                existing.ScheduleDate = model.ScheduleDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("AuditPlan", existing);
                await LogAudit("AuditPlan", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("AuditPlan ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveAuditChecklist(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablAuditChecklist>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid AuditChecklist data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["AuditChecklist"];
                await AssignDocumentNumber(model, "AuditChecklist");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("AuditChecklist", model);
                await LogAudit("AuditChecklist", id, "Created", null, body.GetRawText());
                _logger.LogInformation("AuditChecklist created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablAuditChecklists
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("AuditChecklist not found!");

                await SaveRevisionSnapshot("AuditChecklist", existing);

                existing.AuditPlanId = model.AuditPlanId;
                existing.AuditDate = model.AuditDate;
                existing.DepartmentId = model.DepartmentId;
                existing.DepartmentName = model.DepartmentName;
                existing.AuditorId = model.AuditorId;
                existing.AuditorName = model.AuditorName;
                existing.AuditeeId = model.AuditeeId;
                existing.AuiteeName = model.AuiteeName;
                existing.ISOClause = model.ISOClause;
                existing.ChecklistItemsJson = model.ChecklistItemsJson;
                existing.NCCount = model.NCCount;
                existing.ObservationCount = model.ObservationCount;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("AuditChecklist", existing);
                await LogAudit("AuditChecklist", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("AuditChecklist ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveAuditSummary(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablAuditSummary>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid AuditSummary data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["AuditSummary"];
                await AssignDocumentNumber(model, "AuditSummary");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("AuditSummary", model);
                await LogAudit("AuditSummary", id, "Created", null, body.GetRawText());
                _logger.LogInformation("AuditSummary created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablAuditSummaries
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("AuditSummary not found!");

                await SaveRevisionSnapshot("AuditSummary", existing);

                existing.AuditPlanId = model.AuditPlanId;
                existing.AuditDateFrom = model.AuditDateFrom;
                existing.AuditDateTo = model.AuditDateTo;
                existing.TotalAudits = model.TotalAudits;
                existing.TotalNCs = model.TotalNCs;
                existing.MajorNCs = model.MajorNCs;
                existing.MinorNCs = model.MinorNCs;
                existing.Observations = model.Observations;
                existing.FindingsSummary = model.FindingsSummary;
                existing.PositiveFindings = model.PositiveFindings;
                existing.ClosureStatus = model.ClosureStatus;
                existing.NextAuditDate = model.NextAuditDate;
                existing.SummaryBy = model.SummaryBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("AuditSummary", existing);
                await LogAudit("AuditSummary", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("AuditSummary ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveInternalAuditor(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablInternalAuditor>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid InternalAuditor data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["InternalAuditor"];
                await AssignDocumentNumber(model, "InternalAuditor");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("InternalAuditor", model);
                await LogAudit("InternalAuditor", id, "Created", null, body.GetRawText());
                _logger.LogInformation("InternalAuditor created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablInternalAuditors
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("InternalAuditor not found!");

                await SaveRevisionSnapshot("InternalAuditor", existing);

                existing.EmployeeId = model.EmployeeId;
                existing.EmployeeName = model.EmployeeName;
                existing.Qualification = model.Qualification;
                existing.LeadAuditorCourse = model.LeadAuditorCourse;
                existing.LeadAuditorCertDate = model.LeadAuditorCertDate;
                existing.InternalAuditorCourse = model.InternalAuditorCourse;
                existing.InternalAuditorCertDate = model.InternalAuditorCertDate;
                existing.ISOClauses = model.ISOClauses;
                existing.AuditExperience = model.AuditExperience;
                existing.AuthorizedAreas = model.AuthorizedAreas;
                existing.AuthorizationDate = model.AuthorizationDate;
                existing.AuthorizationValidUpto = model.AuthorizationValidUpto;
                existing.AuthorizedBy = model.AuthorizedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("InternalAuditor", existing);
                await LogAudit("InternalAuditor", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("InternalAuditor ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveMeetingAgenda(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablMeetingAgenda>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid MeetingAgenda data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["MeetingAgenda"];
                await AssignDocumentNumber(model, "MeetingAgenda");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("MeetingAgenda", model);
                await LogAudit("MeetingAgenda", id, "Created", null, body.GetRawText());
                _logger.LogInformation("MeetingAgenda created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablMeetingAgendas
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("MeetingAgenda not found!");

                await SaveRevisionSnapshot("MeetingAgenda", existing);

                existing.MeetingDate = model.MeetingDate;
                existing.MeetingType = model.MeetingType;
                existing.MeetingVenue = model.MeetingVenue;
                existing.ChairpersonId = model.ChairpersonId;
                existing.ChairpersonName = model.ChairpersonName;
                existing.AgendaItemsJson = model.AgendaItemsJson;
                existing.AttendeeIds = model.AttendeeIds;
                existing.AttendeeNames = model.AttendeeNames;
                existing.PreviousMOMRef = model.PreviousMOMRef;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("MeetingAgenda", existing);
                await LogAudit("MeetingAgenda", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("MeetingAgenda ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveMeetingMinutes(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablMeetingMinutes>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid MeetingMinutes data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["MeetingMinutes"];
                await AssignDocumentNumber(model, "MeetingMinutes");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("MeetingMinutes", model);
                await LogAudit("MeetingMinutes", id, "Created", null, body.GetRawText());
                _logger.LogInformation("MeetingMinutes created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablMeetingMinutes
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("MeetingMinutes not found!");

                await SaveRevisionSnapshot("MeetingMinutes", existing);

                existing.AgendaId = model.AgendaId;
                existing.MeetingDate = model.MeetingDate;
                existing.MeetingType = model.MeetingType;
                existing.ChairpersonName = model.ChairpersonName;
                existing.AttendeesJson = model.AttendeesJson;
                existing.MinutesJson = model.MinutesJson;
                existing.NextMeetingDate = model.NextMeetingDate;
                existing.NextMeetingAgenda = model.NextMeetingAgenda;
                existing.ActionClosureStatus = model.ActionClosureStatus;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("MeetingMinutes", existing);
                await LogAudit("MeetingMinutes", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("MeetingMinutes ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveNonConformingWork(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablNonConformingWork>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid NonConformingWork data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["NonConformingWork"];
                await AssignDocumentNumber(model, "NonConformingWork");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("NonConformingWork", model);
                await LogAudit("NonConformingWork", id, "Created", null, body.GetRawText());
                _logger.LogInformation("NonConformingWork created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablNonConformingWorks
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("NonConformingWork not found!");

                await SaveRevisionSnapshot("NonConformingWork", existing);

                existing.NCDate = model.NCDate;
                existing.SampleCode = model.SampleCode;
                existing.TestParameter = model.TestParameter;
                existing.NCDescription = model.NCDescription;
                existing.NCSource = model.NCSource;
                existing.DetectedBy = model.DetectedBy;
                existing.IdentifiedBy = model.IdentifiedBy;
                existing.SuspendedWork = model.SuspendedWork;
                existing.AffectedResults = model.AffectedResults;
                existing.ImmediateAction = model.ImmediateAction;
                existing.NCCategory = model.NCCategory;
                existing.RootCauseAnalysis = model.RootCauseAnalysis;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("NonConformingWork", existing);
                await LogAudit("NonConformingWork", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("NonConformingWork ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveNcCorrectiveAction(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablNcCorrectiveAction>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid NcCorrectiveAction data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["NcCorrectiveAction"];
                await AssignDocumentNumber(model, "NcCorrectiveAction");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("NcCorrectiveAction", model);
                await LogAudit("NcCorrectiveAction", id, "Created", null, body.GetRawText());
                _logger.LogInformation("NcCorrectiveAction created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablNcCorrectiveActions
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("NcCorrectiveAction not found!");

                await SaveRevisionSnapshot("NcCorrectiveAction", existing);

                existing.NCId = model.NCId;
                existing.NCRef = model.NCRef;
                existing.CADate = model.CADate;
                existing.RootCause = model.RootCause;
                existing.CorrectiveAction = model.CorrectiveAction;
                existing.PreventiveAction = model.PreventiveAction;
                existing.ImplementedBy = model.ImplementedBy;
                existing.ImplementationDate = model.ImplementationDate;
                existing.VerificationDate = model.VerificationDate;
                existing.VerifiedBy = model.VerifiedBy;
                existing.EffectivenessEvaluated = model.EffectivenessEvaluated;
                existing.EffectivenessResult = model.EffectivenessResult;
                existing.Closed = model.Closed;
                existing.ClosureDate = model.ClosureDate;
                existing.ClosedBy = model.ClosedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("NcCorrectiveAction", existing);
                await LogAudit("NcCorrectiveAction", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("NcCorrectiveAction ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveRetesting(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablRetesting>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid Retesting data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["Retesting"];
                await AssignDocumentNumber(model, "Retesting");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("Retesting", model);
                await LogAudit("Retesting", id, "Created", null, body.GetRawText());
                _logger.LogInformation("Retesting created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablRetestings
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("Retesting not found!");

                await SaveRevisionSnapshot("Retesting", existing);

                existing.SampleCode = model.SampleCode;
                existing.OriginalTestDate = model.OriginalTestDate;
                existing.RetestReason = model.RetestReason;
                existing.RetestDate = model.RetestDate;
                existing.TestParameter = model.TestParameter;
                existing.TestMethod = model.TestMethod;
                existing.OriginalResult = model.OriginalResult;
                existing.RetestResult = model.RetestResult;
                existing.Unit = model.Unit;
                existing.AcceptanceCriteria = model.AcceptanceCriteria;
                existing.RetestConclusion = model.RetestConclusion;
                existing.TestedBy = model.TestedBy;
                existing.AuthorizedBy = model.AuthorizedBy;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("Retesting", existing);
                await LogAudit("Retesting", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("Retesting ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveRiskAssessment(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablRiskAssessment>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid RiskAssessment data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["RiskAssessment"];
                await AssignDocumentNumber(model, "RiskAssessment");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("RiskAssessment", model);
                await LogAudit("RiskAssessment", id, "Created", null, body.GetRawText());
                _logger.LogInformation("RiskAssessment created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablRiskAssessments
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("RiskAssessment not found!");

                await SaveRevisionSnapshot("RiskAssessment", existing);

                existing.AssessmentDate = model.AssessmentDate;
                existing.ProcessArea = model.ProcessArea;
                existing.RisksJson = model.RisksJson;
                existing.OverallRiskLevel = model.OverallRiskLevel;
                existing.AssessedBy = model.AssessedBy;
                existing.ReviewDate = model.ReviewDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("RiskAssessment", existing);
                await LogAudit("RiskAssessment", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("RiskAssessment ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveDocumentChangeRequest(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablDocumentChangeRequest>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid DocumentChangeRequest data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["DocumentChangeRequest"];
                await AssignDocumentNumber(model, "DocumentChangeRequest");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("DocumentChangeRequest", model);
                await LogAudit("DocumentChangeRequest", id, "Created", null, body.GetRawText());
                _logger.LogInformation("DocumentChangeRequest created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablDocumentChangeRequests
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("DocumentChangeRequest not found!");

                await SaveRevisionSnapshot("DocumentChangeRequest", existing);

                existing.DocumentRef = model.DocumentRef;
                existing.DocumentTitle = model.DocumentTitle;
                existing.DocumentType = model.DocumentType;
                existing.CurrentVersion = model.CurrentVersion;
                existing.ChangeDescription = model.ChangeDescription;
                existing.ReasonForChange = model.ReasonForChange;
                existing.RequestedBy = model.RequestedBy;
                existing.RequestDate = model.RequestDate;
                existing.UrgencyLevel = model.UrgencyLevel;
                existing.AssessedImpact = model.AssessedImpact;
                existing.AssessmentBy = model.AssessmentBy;
                existing.AssessmentDate = model.AssessmentDate;
                existing.Disposition = model.Disposition;
                existing.ImplementationDate = model.ImplementationDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("DocumentChangeRequest", existing);
                await LogAudit("DocumentChangeRequest", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("DocumentChangeRequest ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveDocumentReview(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablDocumentReview>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid DocumentReview data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["DocumentReview"];
                await AssignDocumentNumber(model, "DocumentReview");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("DocumentReview", model);
                await LogAudit("DocumentReview", id, "Created", null, body.GetRawText());
                _logger.LogInformation("DocumentReview created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablDocumentReviews
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("DocumentReview not found!");

                await SaveRevisionSnapshot("DocumentReview", existing);

                existing.DocumentRef = model.DocumentRef;
                existing.DocumentTitle = model.DocumentTitle;
                existing.DocumentType = model.DocumentType;
                existing.CurrentRevision = model.CurrentRevision;
                existing.ReviewDate = model.ReviewDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewFindings = model.ReviewFindings;
                existing.ChangeRequired = model.ChangeRequired;
                existing.ChangeDescription = model.ChangeDescription;
                existing.NextReviewDate = model.NextReviewDate;
                existing.ReviewConclusion = model.ReviewConclusion;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("DocumentReview", existing);
                await LogAudit("DocumentReview", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("DocumentReview ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveMasterDocument(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablMasterDocument>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid MasterDocument data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["MasterDocument"];
                await AssignDocumentNumber(model, "MasterDocument");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("MasterDocument", model);
                await LogAudit("MasterDocument", id, "Created", null, body.GetRawText());
                _logger.LogInformation("MasterDocument created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablMasterDocuments
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("MasterDocument not found!");

                await SaveRevisionSnapshot("MasterDocument", existing);

                existing.DocumentCode = model.DocumentCode;
                existing.DocumentTitle = model.DocumentTitle;
                existing.DocumentType = model.DocumentType;
                existing.CurrentIssue = model.CurrentIssue;
                existing.CurrentRevision = model.CurrentRevision;
                existing.EffectiveDate = model.EffectiveDate;
                existing.ReviewFrequency = model.ReviewFrequency;
                existing.DocumentOwner = model.DocumentOwner;
                existing.StorageLocation = model.StorageLocation;
                existing.ControlledCopiesJson = model.ControlledCopiesJson;
                existing.ObsoleteDate = model.ObsoleteDate;
                existing.ObsoleteReason = model.ObsoleteReason;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("MasterDocument", existing);
                await LogAudit("MasterDocument", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("MasterDocument ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SaveMeasurementUncertainty(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablMeasurementUncertainty>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid MeasurementUncertainty data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["MeasurementUncertainty"];
                await AssignDocumentNumber(model, "MeasurementUncertainty");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("MeasurementUncertainty", model);
                await LogAudit("MeasurementUncertainty", id, "Created", null, body.GetRawText());
                _logger.LogInformation("MeasurementUncertainty created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablMeasurementUncertainties
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("MeasurementUncertainty not found!");

                await SaveRevisionSnapshot("MeasurementUncertainty", existing);

                existing.TestParameter = model.TestParameter;
                existing.TestMethod = model.TestMethod;
                existing.MatrixType = model.MatrixType;
                existing.UncertaintyType = model.UncertaintyType;
                existing.SourcesJson = model.SourcesJson;
                existing.CombinedUncertainty = model.CombinedUncertainty;
                existing.ExpandedUncertainty = model.ExpandedUncertainty;
                existing.CoverageFactor = model.CoverageFactor;
                existing.ConfidenceLevel = model.ConfidenceLevel;
                existing.Unit = model.Unit;
                existing.ValidatedBy = model.ValidatedBy;
                existing.ReviewDate = model.ReviewDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("MeasurementUncertainty", existing);
                await LogAudit("MeasurementUncertainty", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("MeasurementUncertainty ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        private async Task<long> SavePtIlcPlan(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<NablPtIlcPlan>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid PtIlcPlan data.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["PtIlcPlan"];
                await AssignDocumentNumber(model, "PtIlcPlan");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";

                var id = await _repository.Add("PtIlcPlan", model);
                await LogAudit("PtIlcPlan", id, "Created", null, body.GetRawText());
                _logger.LogInformation("PtIlcPlan created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablPtIlcPlans
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("PtIlcPlan not found!");

                await SaveRevisionSnapshot("PtIlcPlan", existing);

                existing.PlanYear = model.PlanYear;
                existing.PTType = model.PTType;
                existing.OrganizingBody = model.OrganizingBody;
                existing.ScheduleJson = model.ScheduleJson;
                existing.TotalParticipations = model.TotalParticipations;
                existing.SatisfactoryResults = model.SatisfactoryResults;
                existing.UnsatisfactoryResults = model.UnsatisfactoryResults;
                existing.CorrectiveActions = model.CorrectiveActions;
                existing.ResponsiblePerson = model.ResponsiblePerson;
                existing.OverallAssessment = model.OverallAssessment;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                await _repository.Update("PtIlcPlan", existing);
                await LogAudit("PtIlcPlan", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("PtIlcPlan ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        // ─── Private: Auto-numbering ────────────────────────────────────

        private async Task AssignDocumentNumber(NablFormBase model, string formType)
        {
            var formCode = FormCodeMap.GetValueOrDefault(formType, formType);
            var moduleName = $"NABL-{formCode}";

            var config = await _context.NumberingConfigs
                .FirstOrDefaultAsync(x => x.ModuleName == moduleName && x.CompanyCode == loggedInUser.CompanyCode);

            if (config == null)
            {
                // Auto-create numbering config for this form type
                config = new NumberingConfig
                {
                    ModuleName = moduleName,
                    Prefix = formCode,
                    StartNumber = 1,
                    CurrentNumber = 0,
                    OrganizationId = 0,
                    CompanyCode = loggedInUser.CompanyCode ?? "LIMS",
                    CreatedBy = loggedInUser.EmployeeID,
                    CreatedOn = DateTime.UtcNow
                };
                _context.NumberingConfigs.Add(config);
            }

            config.CurrentNumber++;
            var year = DateTime.UtcNow.Year;
            var seq = config.CurrentNumber.ToString("D3");

            model.IssueNo = "01";
            model.RevNo = "00";
            model.DocumentNo = $"{formCode}/{model.IssueNo}/{year}-{seq}";

            await _context.SaveChangesAsync();
        }

        // ─── Private: Revision history ──────────────────────────────────

        private async Task SaveRevisionSnapshot(string formType, NablFormBase entity)
        {
            var snapshot = JsonSerializer.Serialize(entity, entity.GetType(), _jsonOptions);

            var revision = new NablFormRevisionHistory
            {
                FormDataId = entity.ID,
                FormType = formType,
                IssueNo = entity.IssueNo,
                RevNo = entity.RevNo,
                SnapshotJson = snapshot,
                RevisionDate = DateTime.UtcNow,
                RevisedBy = loggedInUser.EmployeeID,
                RevisedByName = loggedInUser.Name,
                CreatedBy = loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = loggedInUser.CompanyCode ?? "LIMS"
            };

            await _context.NablFormRevisionHistory.AddAsync(revision);
            await _context.SaveChangesAsync();
        }

        // ─── Private: Audit logging ─────────────────────────────────────

        private async Task LogAudit(string formType, long formDataId, string action, string? oldValues, string? newValues)
        {
            var auditLog = new NablAuditLog
            {
                FormType = formType,
                FormDataId = formDataId,
                Action = action,
                OldValues = oldValues,
                NewValues = newValues,
                PerformedBy = loggedInUser.EmployeeID,
                PerformedByName = loggedInUser.Name,
                PerformedOn = DateTime.UtcNow,
                CompanyCode = loggedInUser.CompanyCode ?? "LIMS"
            };

            await _context.NablAuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        // ─── Private: Get entity as NablFormBase for workflow ────────────

        private async Task<NablFormBase?> GetEntityAsBase(string formType, long id)
        {
            return formType switch
            {
                "JobDescription" => await _context.NablJobDescriptions
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "ResponsibilityAuthority" => await _context.NablResponsibilityAuthorities
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "EmployeeCompetence" => await _context.NablEmployeeCompetences
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "EmployeePerformanceRecord" => await _context.NablEmployeePerformanceRecords
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "EmployeeAuthorization" => await _context.NablEmployeeAuthorizations
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "CompetenceRequirement" => await _context.NablCompetenceRequirements
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "InductionTraining" => await _context.NablInductionTrainings
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SkillMatrix" => await _context.NablSkillMatrices
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SkillMatrixDecision" => await _context.NablSkillMatrixDecisions
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "TrainingPlan" => await _context.NablTrainingPlans
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "TrainingAttendance" => await _context.NablTrainingAttendances
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "TrainingEffectiveness" => await _context.NablTrainingEffectivenesses
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "EnvironmentMonitoring" => await _context.NablEnvironmentMonitorings
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "QualityControlPlan" => await _context.NablQualityControlPlans
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "TestRequest" => await _context.NablTestRequests
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "TestMethod" => await _context.NablTestMethods
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "MethodVerification" => await _context.NablMethodVerifications
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "MethodValidation" => await _context.NablMethodValidations
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SampleInwardRegister" => await _context.NablSampleInwardRegisters
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SampleMusterRegister" => await _context.NablSampleMusterRegisters
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SampleLabel" => await _context.NablSampleLabels
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "TechnicalRawData" => await _context.NablTechnicalRawDatas
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "TestReport" => await _context.NablTestReports
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "EquipmentHistory" => await _context.NablEquipmentHistories
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "CalibrationReview" => await _context.NablCalibrationReviews
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "IntermediateCheck" => await _context.NablIntermediateChecks
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "ReferenceMaterial" => await _context.NablReferenceMaterials
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "CrmConsumption" => await _context.NablCrmConsumptions
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SupplierRegistration" => await _context.NablSupplierRegistrations
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SupplierEvaluation" => await _context.NablSupplierEvaluations
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "ApprovedSupplier" => await _context.NablApprovedSuppliers
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "SupplierConfidentiality" => await _context.NablSupplierConfidentialities
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "IncomingMaterial" => await _context.NablIncomingMaterials
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "ProductInspection" => await _context.NablProductInspections
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "PurchaseIndent" => await _context.NablPurchaseIndents
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "PurchaseOrder" => await _context.NablPurchaseOrders
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "PurchaseMaterialVerification" => await _context.NablPurchaseMaterialVerifications
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "Complaint" => await _context.NablComplaints
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "CustomerFeedback" => await _context.NablCustomerFeedbacks
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "FeedbackAnalysis" => await _context.NablFeedbackAnalyses
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "AuditPlan" => await _context.NablAuditPlans
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "AuditChecklist" => await _context.NablAuditChecklists
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "AuditSummary" => await _context.NablAuditSummaries
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "InternalAuditor" => await _context.NablInternalAuditors
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "MeetingAgenda" => await _context.NablMeetingAgendas
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "MeetingMinutes" => await _context.NablMeetingMinutes
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "NonConformingWork" => await _context.NablNonConformingWorks
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "NcCorrectiveAction" => await _context.NablNcCorrectiveActions
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "Retesting" => await _context.NablRetestings
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "RiskAssessment" => await _context.NablRiskAssessments
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "DocumentChangeRequest" => await _context.NablDocumentChangeRequests
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "DocumentReview" => await _context.NablDocumentReviews
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "MasterDocument" => await _context.NablMasterDocuments
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "MeasurementUncertainty" => await _context.NablMeasurementUncertainties
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                "PtIlcPlan" => await _context.NablPtIlcPlans
                    .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode),
                _ => throw new ArgumentException($"Unknown form type: {formType}")
            };
        }
    }
}
