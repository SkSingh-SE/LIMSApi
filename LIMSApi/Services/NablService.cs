using Humanizer;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Text.Json;

namespace LIMSApi.Services
{
    public class NablService : INablService
    {
        private readonly INablRepository _repository;
        private readonly LIMSContext _context;
        private readonly ILogger<NablService> _logger;
        private readonly LoggedInUserDTO loggedInUser;
        private readonly IWebHostEnvironment _env;
        private readonly IFileUploadService _fileUploadService;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // FormType -> FormCode mapping
        private static readonly Dictionary<string, string> FormCodeMap = new()
        {
            { "JobDescription", "F-3" },
            { "ResponsibilityAuthority", "F-4" },
            { "EmployeeCompetence", "F-7A" },
            { "EmployeeAuthorization", "F-13" },
            { "CompetenceRequirement", "F-7C" },
            { "InductionTraining", "F-11" },
            { "SkillMatrix", "F-6" },
            { "SkillMatrixDecision", "F-6A" },
            { "TrainingPlan", "F-8" },
            { "TrainingAttendance", "F-9" },
            { "TrainingEffectiveness", "F-10" },
            { "EnvironmentMonitoring", "F-12" },
            { "QualityControlPlan", "F-37" },
            { "TestRequest", "F-27" },
            { "TestMethod", "F-28" },
            { "MethodVerification", "F-29" },
            { "MethodValidation", "F-30" },
            { "SampleInwardRegister", "F-31" },
            { "SampleMusterRegister", "F-32" },
            { "SampleLabel", "F-33" },
            { "TechnicalRawData", "F-34" },
            { "TestReport", "F-39" },
            { "EquipmentHistory", "F-22" },
            { "CalibrationReview", "F-23" },
            { "IntermediateCheck", "F-16" },
            { "ReferenceMaterial", "F-17" },
            { "CrmConsumption", "F-18" },
            { "SupplierRegistration", "F-19" },
            { "SupplierEvaluation", "F-26" },
            { "ApprovedSupplier", "F-20" },
            { "SupplierConfidentiality", "F-2" },
            { "IncomingMaterial", "F-24" },
            { "ProductInspection", "F-23" },
            { "PurchaseIndent", "F-21" },
            { "PurchaseOrder", "F-22" },
            { "PurchaseMaterialVerification", "F-25" },
            { "Complaint", "F-40" },
            { "CustomerFeedback", "F-47" },
            { "FeedbackAnalysis", "F-48" },
            { "AuditPlan", "F-50" },
            { "AuditChecklist", "F-51" },
            { "AuditSummary", "F-52" },
            { "InternalAuditor", "F-49" },
            { "MeetingAgenda", "F-53" },
            { "MeetingMinutes", "F-54" },
            { "NonConformingWork", "F-41" },
            { "NcCorrectiveAction", "F-42" },
            { "Retesting", "F-38" },
            { "RiskAssessment", "F-46" },
            { "DocumentChangeRequest", "F-44" },
            { "DocumentReview", "F-45" },
            { "MasterDocument", "F-43" },
            { "MeasurementUncertainty", "F-35" },
            { "PtIlcPlan", "F-36" },
            { "EmployeePerformanceRecord", "F-54" },
        };

        public NablService(INablRepository repository, LIMSContext context, ILogger<NablService> logger, IWebHostEnvironment env, IFileUploadService fileUploadService)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            _env=env;
            _fileUploadService=fileUploadService;
        }

        public async Task<PagedResponse<object>> FetchList(string formType, PageFilter filter)
        {
            return await _repository.GetAll(formType, filter);
        }

        public async Task<object?> GetDetails(string formType, long id)
        {
            var data = await _repository.GetById(formType, id);

            if (data == null)
                return null;

            switch (formType)
            {
                case "SupplierEvaluation":
                    {
                        var result = data as NablSupplierEvaluation;

                        if (result != null && !string.IsNullOrEmpty(result.CriteriaJson))
                        {
                            result.Criteria = JsonSerializer.Deserialize<List<Criteria>>(result.CriteriaJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.POJson))
                        {
                            result.PurchaseOrders = JsonSerializer.Deserialize<List<PurchaseOrders>>(result.POJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.IncomingPlanJson))
                        {
                            result.IncomingPlan = JsonSerializer.Deserialize<List<IncomingPlan>>(result.IncomingPlanJson);
                        }

                        return result;
                    }

                case "EmployeeCompetence":
                    {
                        var result = data as NablEmployeeCompetence;

                        if (result != null && !string.IsNullOrEmpty(result.ParametersJson))
                        {
                            result.Parameters = JsonSerializer.Deserialize<List<CompetenceParameter>>(result.ParametersJson);
                        }

                        return result;
                    }
                case "TrainingAttendance":
                    {
                        var result = data as NablTrainingAttendance;
                        if (result != null && !string.IsNullOrEmpty(result.AttendeesJson))
                        {
                            result.Participants = JsonSerializer.Deserialize<List<Participates>>(result.AttendeesJson);
                        }
                        return result;
                    }
                case "SupplierRegistration":
                    {
                        var result = data as NablSupplierRegistration;
                        if (result != null && !string.IsNullOrEmpty(result.BankDetailsJson))
                        {
                            result.BankDetail = JsonSerializer.Deserialize<BankDetail>(result.BankDetailsJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.DocumentsSubmittedJson))
                        {
                            result.DocumentsSubmitted = JsonSerializer.Deserialize<DocumentsSubmitted>(result.DocumentsSubmittedJson);
                        }
                        return result;
                    }
                case "PurchaseOrder":
                    {
                        var result = data as NablPurchaseOrder;
                        if (result != null && !string.IsNullOrEmpty(result.ItemsJson))
                        {
                            result.Items = JsonSerializer.Deserialize<List<Items>>(result.ItemsJson);
                        }
                        return result;
                    }
                case "ProductInspection":
                    {
                        var result = data as NablProductInspection;
                        if (result != null && !string.IsNullOrEmpty(result.InspectionResultsJson))
                        {
                            result.Parameters = JsonSerializer.Deserialize<List<Inspectionparameters>>(result.InspectionResultsJson);
                        }
                        return result;
                    }
                case "IncomingMaterial":
                    {
                        var result = data as NablIncomingMaterial;
                        if (result != null && !string.IsNullOrEmpty(result.InspectionParameterJson))
                        {
                            result.InspectionParameters = JsonSerializer.Deserialize<List<InspectionParameters>>(result.InspectionParameterJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.ItemsParametersJson))
                        {
                            result.ItemsParameters = JsonSerializer.Deserialize<List<ItemsParameters>>(result.ItemsParametersJson);
                        }
                        return result;
                    }
                case "TestRequest":
                    {
                        var result = data as NablTestRequest;
                        if (result != null && !string.IsNullOrEmpty(result.DispatchModeJson))
                        {
                            result.DispatchModes = JsonSerializer.Deserialize<List<string>>(result.DispatchModeJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.TestParametersJson))
                        {
                            result.Samples = JsonSerializer.Deserialize<List<samples>>(result.TestParametersJson);
                        }
                        return result;
                    }
                case "PurchaseMaterialVerification":
                    {
                        var result = data as NablPurchaseMaterialVerification;
                        if (result != null && !string.IsNullOrEmpty(result.ItemsVerificationJson))
                        {
                            result.ItemsParameters = JsonSerializer.Deserialize<List<DescriptionParameters>>(result.ItemsVerificationJson);
                        }
                        return result;
                    }
                case "TestMethod":
                    {
                        var result = data as NablTestMethod;
                        if (result != null && !string.IsNullOrEmpty(result.TestMethodJson))
                        {
                            result.TestMethod = JsonSerializer.Deserialize<List<TestMethod>>(result.TestMethodJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.OrginDocJson))
                        {
                            result.DocEntries = JsonSerializer.Deserialize<List<DocEntries>>(result.OrginDocJson);
                        }
                        return result;
                    }
                case "MethodVerification":
                    {
                        var result = data as NablMethodVerification;
                        if (result != null && !string.IsNullOrEmpty(result.CrmParametersJson))
                        {
                            result.CrmParameters = JsonSerializer.Deserialize<List<CrmParameters>>(result.CrmParametersJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.VerificationDataJson))
                        {
                            result.VerificationData = JsonSerializer.Deserialize<List<VerificationData>>(result.VerificationDataJson);
                        }
                        return result;
                    }
                case "MethodValidation":
                    {
                        var result = data as NablMethodValidation;
                        if (result != null && !string.IsNullOrEmpty(result.AccuracyStudyJson))
                        {
                            result.AccuracyStudy = JsonSerializer.Deserialize<List<AccuracyStudy>>(result.AccuracyStudyJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.CrmMaterialParametersJson))
                        {
                            result.CrmMaterialParameters = JsonSerializer.Deserialize<List<CrmMaterialParameters>>(result.CrmMaterialParametersJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.PrecisionStudyJson))
                        {
                            result.PrecisionStudy = JsonSerializer.Deserialize<List<PrecisionStudy>>(result.PrecisionStudyJson);
                        }
                        return result;
                    }
                case "PtIlcPlan":
                    {
                        var result = data as NablPtIlcPlan;
                        if (result != null && !string.IsNullOrEmpty(result.ActivitiesJson))
                        {
                            result.Activities = JsonSerializer.Deserialize<List<PtilcActivity>>(result.ActivitiesJson);
                        }
                        return result;
                    }
                case "ReferenceMaterial":
                    {
                        var result = data as NablReferenceMaterial;
                        if (result != null && !string.IsNullOrEmpty(result.ParameterJson))
                        {
                            result.Parameters = JsonSerializer.Deserialize<List<Parameters>>(result.ParameterJson);
                        }
                        return result;
                    }
                case "CustomerFeedback":
                    {
                        var result = data as NablCustomerFeedback;
                        if (result != null && !string.IsNullOrEmpty(result.RatingsJson))
                        {
                            result.Ratings = JsonSerializer.Deserialize<List<Ratings>>(result.RatingsJson);
                        }
                        return result;
                    }
                case "MeetingAgenda":
                    {
                        var result = data as NablMeetingAgenda;
                        if (result != null && !string.IsNullOrEmpty(result.AgendaItemsJson))
                        {
                            result.AgendaItems = JsonSerializer.Deserialize<List<AgendaItems>>(result.AgendaItemsJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.ParticipantsJson))
                        {
                            result.Participants= JsonSerializer.Deserialize<List<Participants>>(result.ParticipantsJson);
                        }
                        return result;
                    }
                case "MeetingMinutes":
                    {
                        var result = data as NablMeetingMinutes;
                        if (result != null && !string.IsNullOrEmpty(result.ActionPlanJson))
                        {
                            result.ActionItems = JsonSerializer.Deserialize<List<ActionItems>>(result.ActionPlanJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.AttendeesJson))
                        {
                            result.ParticipantItems= JsonSerializer.Deserialize<List<ParticipantItems>>(result.AttendeesJson);
                        }
                        if (result != null && !string.IsNullOrEmpty(result.AgendaItemsJson))
                        {
                            result.AgendaList= JsonSerializer.Deserialize<List<AgendaList>>(result.AgendaItemsJson);
                        }
                        return result;
                    }
                case "MeasurementUncertainty":
                    {
                        var result = data as NablMeasurementUncertainty;
                        if (result != null && !string.IsNullOrEmpty(result.SourcesJson))
                        {
                            result.UncertaintySources = JsonSerializer.Deserialize<List<UncertaintySources>>(result.SourcesJson);
                        }

                        return result;
                    }
                case "RiskAssessment":
                    {
                        var result = data as NablRiskAssessment;
                        if (result != null && !string.IsNullOrEmpty(result.RisksJson))
                        {
                            result.ActionPlans = JsonSerializer.Deserialize<List<ActionPlans>>(result.RisksJson);
                        }

                        return result;
                    }


                default:
                    return data;
            }
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
                "InventoryMaster" => await SaveInventoryMaster(body),

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
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
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
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate =model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
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
                model.ParametersJson = JsonSerializer.Serialize(model.Parameters);

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
                existing.ParametersJson = JsonSerializer.Serialize(model.Parameters);
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedBy = model.ApprovedBy;

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
                var existing = await _context.NablEmployeeAuthorizations.Include(c => c.LabTestAuth).Include(c => c.TestMethodAuth).Include(c => c.EmployeeEquipmentAuth)
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing.EmployeeEquipmentAuth != null)
                {
                    var toRemove = existing.EmployeeEquipmentAuth.Where(existing => !model.EmployeeEquipmentAuth.Any(m => m.EquipmentId == existing.EquipmentId)).ToList();
                    foreach (var employee in toRemove)
                    {
                        existing.EmployeeEquipmentAuth.Remove(employee);
                    }
                }
                if (model.EmployeeEquipmentAuth != null && model.EmployeeEquipmentAuth.Any())
                {
                    foreach (var Newemp in model.EmployeeEquipmentAuth)
                    {
                        Newemp.EmployeeAuthorazitionId = model.ID;
                        var existemployee = existing.EmployeeEquipmentAuth.FirstOrDefault(c => c.EquipmentId == Newemp.EquipmentId);
                        if (existemployee == null)
                        {
                            existing.EmployeeEquipmentAuth.Add(Newemp);
                        }
                        else
                        {
                            existemployee.UID = Newemp.UID;
                            existemployee.EquipmentName = Newemp.EquipmentName;

                        }
                    }
                }
                if (existing.LabTestAuth != null)
                {

                    var toRemove = existing.LabTestAuth.Where(existing => !model.LabTestAuth.Any(m => m.LabTestId == existing.LabTestId)).ToList();
                    foreach (var lab in toRemove)
                    {
                        existing.LabTestAuth.Remove(lab);
                    }
                }
                if (model.LabTestAuth != null && model.LabTestAuth.Any())
                {
                    foreach (var newlab in model.LabTestAuth)
                    {
                        newlab.EmployeeAuthorizationId = model.ID;
                        var existlab = existing.LabTestAuth.FirstOrDefault(c => c.LabTestId == newlab.LabTestId);
                        if (existlab == null)
                        {
                            existing.LabTestAuth.Add(newlab);
                        }
                        else
                        {
                            existlab.LabTestName = newlab.LabTestName;
                        }
                    }
                }
                if (existing.TestMethodAuth != null)
                {
                    var toRemove = existing.TestMethodAuth.Where(existing => !model.TestMethodAuth.Any(m => m.TestMethodId == existing.TestMethodId)).ToList();
                    foreach (var testMethod in toRemove)
                    {
                        existing.TestMethodAuth.Remove(testMethod);
                    }
                }
                if (model.TestMethodAuth != null && model.TestMethodAuth.Any())
                {
                    foreach (var newtestMethod in model.TestMethodAuth)
                    {
                        newtestMethod.EmployeeAuthorizationId = model.ID;
                        var extisttestMethod = existing.TestMethodAuth.FirstOrDefault(c => c.TestMethodId == newtestMethod.TestMethodId);
                        if (extisttestMethod == null)
                        {
                            existing.TestMethodAuth.Add(newtestMethod);
                        }
                        else
                        {
                            extisttestMethod.TestMethodName = newtestMethod.TestMethodName;
                        }
                    }
                }

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
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedDate = model.PreparedDate;
                //existing.LabTestAuth = model.LabTestAuth;
                //existing.TestMethodAuth = model.TestMethodAuth;
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
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedDate = model.PreparedDate;
                existing.PreparedBy = model.PreparedBy;
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
                existing.PerformanceLevel = model.PerformanceLevel;
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
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
                existing.TotalBudget = model.TotalBudget;
                existing.ApprovalStatus = model.ApprovalStatus;
                existing.CoursesJson = model.CoursesJson;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.Agency = model.Agency;
                existing.TargetAudience = model.TargetAudience;
                existing.Provider = model.Provider;
                existing.PlanMonth = model.PlanMonth;
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
                model.TrainingDate = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";
                model.AttendeesJson = JsonSerializer.Serialize(model.Participants);
                model.TotalAttendees = model.Participants?.Count;
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
                existing.AttendeesJson = JsonSerializer.Serialize(model.Participants);
                existing.TotalAttendees = model.Participants?.Count;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.NextReviewDate = model.ReviewedDate;
                existing.TrainingDatetime = model.TrainingDatetime;
                existing.GenearalRemarks = model.GenearalRemarks;

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
                if (model.Activities != null && model.Activities.Any())
                {
                    foreach (var activity in model.Activities)
                    {
                        activity.IsActive = true;
                        activity.EffectiveFrom = activity.EffectiveFrom;
                        activity.EffectiveTo = model.EffectiveTo;
                        activity.NextDueDate = CalculateNextDuaDate(activity.FrequencyType, activity.EffectiveFrom);
                    }
                }

                var id = await _repository.Add("QualityControlPlan", model);
                await LogAudit("QualityControlPlan", id, "Created", null, body.GetRawText());
                _logger.LogInformation("QualityControlPlan created with ID {Id}.", id);
                return id;
            }
            else
            {
                var existing = await _context.NablQualityControlPlans
                    .FirstOrDefaultAsync(x => x.ID == model.ID
                        && x.IsActive
                        && x.CompanyCode == loggedInUser.CompanyCode);

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
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.PlanNo = model.PlanNo;
                existing.RetentionPeriod = model.RetentionPeriod;
                existing.LabIncharge = model.LabIncharge;
                existing.MaterialProductGroup = model.MaterialProductGroup;
                existing.Discipline = model.Discipline;
                existing.PlanYear = model.PlanYear;
                existing.EffectiveFrom = model.EffectiveFrom;
                existing.EffectiveTo = model.EffectiveTo;

                var dbActivities = await _context.NablQualityControlPlanActivities
                    .Where(x => x.QualityControlPlanId == existing.ID && x.IsActive)
                    .ToListAsync();

                var modelActivities = model.Activities ?? new List<NablQualityControlPlanActivity>();

                var modelActivityIds = modelActivities
                    .Where(x => x.ID > 0)
                    .Select(x => x.ID)
                    .ToList();

                foreach (var dbActivity in dbActivities)
                {
                    if (!modelActivityIds.Contains(dbActivity.ID))
                    {
                        dbActivity.IsActive = false;
                    }
                }

                foreach (var activity in modelActivities)
                {
                    if (activity.ID > 0)
                    {
                        var dbActivity = dbActivities.FirstOrDefault(x => x.ID == activity.ID);

                        if (dbActivity == null)
                            continue;

                        dbActivity.ActivityName = activity.ActivityName;
                        dbActivity.DepartmentID = activity.DepartmentID;
                        dbActivity.TestMethodId = activity.TestMethodId;
                        dbActivity.ReferenceType = activity.ReferenceType;
                        dbActivity.ReferenceId = activity.ReferenceId;
                        dbActivity.ReferenceName = activity.ReferenceName;
                        dbActivity.FrequencyType = activity.FrequencyType;
                        dbActivity.FrequencyName = activity.FrequencyName;
                        dbActivity.EmployeeId = activity.EmployeeId;
                        dbActivity.AcceptanceCriteria = activity.AcceptanceCriteria;
                        dbActivity.ResultStatus = activity.ResultStatus;
                        dbActivity.Remarks = activity.Remarks;
                        dbActivity.EffectiveFrom = existing.EffectiveFrom;
                        dbActivity.EffectiveTo = existing.EffectiveTo;
                        dbActivity.DepartmentName = activity.DepartmentName;
                        dbActivity.TestMethod = activity.TestMethod;
                        dbActivity.EmployeeName = activity.EmployeeName;
                        dbActivity.IsActive = true;
                        dbActivity.NextDueDate = CalculateNextDuaDate(activity.FrequencyType, activity.EffectiveFrom);
                    }
                    else
                    {
                        var newActivity = new NablQualityControlPlanActivity
                        {
                            QualityControlPlanId = existing.ID,
                            ActivityName = activity.ActivityName,
                            DepartmentID = activity.DepartmentID,
                            TestMethodId = activity.TestMethodId,
                            ReferenceType = activity.ReferenceType,
                            ReferenceId = activity.ReferenceId,
                            ReferenceName = activity.ReferenceName,
                            FrequencyType = activity.FrequencyType,
                            FrequencyName = activity.FrequencyName,
                            EmployeeId = activity.EmployeeId,
                            AcceptanceCriteria = activity.AcceptanceCriteria,
                            ResultStatus = activity.ResultStatus,
                            Remarks = activity.Remarks,
                            EffectiveFrom = existing.EffectiveFrom,
                            EffectiveTo = existing.EffectiveTo,
                            DepartmentName = activity.DepartmentName,
                            TestMethod = activity.TestMethod,
                            EmployeeName = activity.EmployeeName,
                            NextDueDate = CalculateNextDuaDate(activity.FrequencyType, activity.EffectiveFrom),
                            IsActive = true
                        };

                        _context.NablQualityControlPlanActivities.Add(newActivity);
                    }
                }

                await _context.SaveChangesAsync();

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
                model.TestParametersJson =  JsonSerializer.Serialize(model.Samples);
                model.DispatchModeJson =  JsonSerializer.Serialize(model.DispatchModes);
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
                existing.TestParametersJson =  JsonSerializer.Serialize(model.Samples);
                existing.DispatchModeJson =  JsonSerializer.Serialize(model.DispatchModes);
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.Urgent = model.Urgent;
                existing.HoldTesting = model.HoldTesting;
                existing.ReturnSample = model.ReturnSample;
                existing.BillRequired = model.BillRequired;
                existing.ConfirmityRequired = model.ConfirmityRequired;
                existing.GstNo = model.GstNo;
                existing.Remarks =  model.Remarks;
                existing.Address = model.Address;
                existing.PoNumber = model.PoNumber;
                existing.Note = model.Note;
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
                model.OrginDocJson =  JsonSerializer.Serialize(model.DocEntries);
                model.TestMethodJson =  JsonSerializer.Serialize(model.TestMethod);
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
                existing.OrginDocJson =  JsonSerializer.Serialize(model.DocEntries);
                existing.TestMethodJson =  JsonSerializer.Serialize(model.TestMethod);
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
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
                model.CrmParametersJson =  JsonSerializer.Serialize(model.CrmParameters);
                model.VerificationDataJson =  JsonSerializer.Serialize(model.VerificationData);

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
                existing.TestMethodName = model.TestMethodName;
                existing.RevIssue = model.RevIssue;
                existing.ReferenceStandard = model.ReferenceStandard;
                existing.Humidity = model.Humidity;
                existing.Temperature = model.Temperature;
                existing.EquipmentId = model.EquipmentId;
                existing.EquipmentName = model.EquipmentName;
                existing.Conclusion = model.Conclusion;
                existing.VerificationStatus = model.VerificationStatus;
                existing.ReasonNotVerified = model.ReasonNotVerified;
                existing.RecoveryMax = model.RecoveryMax;
                existing.RecoveryMin = model.RecoveryMin;
                existing.RsdMax = model.RsdMax;
                existing.BiasMax = model.BiasMax;
                existing.CalibrationDueDate = model.CalibrationDueDate;
                existing.CrmParametersJson =  JsonSerializer.Serialize(model.CrmParameters);
                existing.VerificationDataJson =  JsonSerializer.Serialize(model.VerificationData);
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;

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
                model.CrmMaterialParametersJson =  JsonSerializer.Serialize(model.CrmMaterialParameters);
                model.AccuracyStudyJson =  JsonSerializer.Serialize(model.AccuracyStudy);
                model.PrecisionStudyJson =  JsonSerializer.Serialize(model.PrecisionStudy);

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
                existing.CrmMaterialParametersJson =  JsonSerializer.Serialize(model.CrmMaterialParameters);
                existing.AccuracyStudyJson =  JsonSerializer.Serialize(model.AccuracyStudy);
                existing.PrecisionStudyJson =  JsonSerializer.Serialize(model.PrecisionStudy);
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ValidationType = model.ValidationType;
                existing.ValidStatus = model.ValidStatus;
                existing.TestMethodName = model.TestMethodName;
                existing.RevIssue = model.RevIssue;
                existing.ReferenceStandard = model.ReferenceStandard;
                existing.Humidity = model.Humidity;
                existing.Temperature = model.Temperature;
                existing.EquipmentId = model.EquipmentId;
                existing.EquipmentName = model.EquipmentName;
                existing.Conclusion = model.Conclusion;
                existing.ReasonForValidation = model.ReasonForValidation;
                existing.ReasonNotValid = model.ReasonNotValid;
                existing.Recovery = model.Recovery;
                existing.RecoveryMax = model.RecoveryMax;
                existing.RecoveryMin= model.RecoveryMin;
                existing.RsdMax= model.RsdMax;
                existing.BiasMax= model.BiasMax;
                existing.ConfidenceLevel= model.ConfidenceLevel;
                existing.CoverageFactor= model.CoverageFactor;
                existing.ExpandedUncertainty= model.ExpandedUncertainty;
                existing.Measurement= model.Measurement;
                existing.MeasurementUncertainty= model.MeasurementUncertainty;
                existing.Precision= model.Precision;
                existing.Repeatability= model.Repeatability;
                existing.Accuracy= model.Accuracy;
                existing.Robustness = model.Robustness;

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
                model.ParameterJson = JsonSerializer.Serialize(model.Parameters);

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
                existing.StorageCondition = null;
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
                existing.ApprovedBy = model.ApprovedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ParameterJson = JsonSerializer.Serialize(model.Parameters);
                existing.MaterialDescription = model.MaterialDescription;
                existing.Type = model.Type;
                existing.Supplier = model.Supplier;
                existing.MatrixType = model.MatrixType;
                existing.StorageLocation= model.StorageLocation;
                existing.Traceability= model.Traceability;
                existing.CertificationDate= model.CertificationDate;
                existing.ValidityDate= model.ValidityDate;
                existing.InitialQuantity= model.InitialQuantity;
                existing.AvailableQuantity= model.AvailableQuantity;
                existing.MinimumQuantity= model.MinimumQuantity;
                existing.UnitOfMeasure= model.UnitOfMeasure;
                existing.Specifications = model.Specifications;
                existing.ItemId = model.ItemId;
                existing.DepartmentID = model.DepartmentID;
                existing.InventoryId = model.InventoryId;
                existing.ItemCode = model.ItemCode;
                existing.ItemName = model.ItemName;

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

            if (model.ReferenceMaterialId <= 0)
                throw new ArgumentException("Reference Material not found.");

            if (model.ID == 0)
            {
                model.FormCode = FormCodeMap["CrmConsumption"];
                await AssignDocumentNumber(model, "CrmConsumption");
                model.Status = "Draft";
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";
                if (model.Logs != null && model.Logs.Any())
                {
                    foreach (var log in model.Logs)
                    {
                        log.ReferenceMaterialId = model.ReferenceMaterialId;
                        log.ReferenceMaterialConsumptionId = 0;
                        log.IsActive = true;
                        log.CreatedBy = loggedInUser.EmployeeID;
                        log.CreatedDate = DateTime.UtcNow;

                    }
                }
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
                existing.OpeningQuantity = model.OpeningQuantity;
                existing.TotalConsumed = model.TotalConsumed;
                existing.RemainingQuantity = model.RemainingQuantity;
                existing.Notes = model.Notes;
                existing.PreparedBy = model.PreparedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedBy = model.ApprovedBy;
                if (model.Logs != null && model.Logs.Any())
                {
                    foreach (var log in model.Logs)
                    {
                        if (log.Id > 0)
                            continue;

                        var newlog = new ReferenceMaterialConsumptionLog
                        {
                            ReferenceMaterialConsumptionId = existing.ID,
                            ReferenceMaterialId = existing.ReferenceMaterialId,
                            ConsumptionDate = log.ConsumptionDate,
                            QuantityConsumed = log.QuantityConsumed,
                            //PreviousBalanceQty = log.PreviousBalanceQty,
                            BalanceQty = log.BalanceQty,
                            Purpose = log.Purpose,
                            EquipmentOrTest = log.EquipmentOrTest,
                            UsedBy = log.UsedBy,
                            Remarks = log.Remarks,
                            IsActive = true,
                            CreatedBy = loggedInUser.EmployeeID,
                            CreatedDate = DateTime.UtcNow
                        };
                        existing.Logs.Add(newlog);
                    }
                }

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
                model.DocumentsSubmittedJson = JsonSerializer.Serialize(model.DocumentsSubmitted);
                model.BankDetailsJson = JsonSerializer.Serialize(model.BankDetail);
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
                existing.Designation = model.Designation;
                existing.MobileNo = model.MobileNo;
                existing.Website = model.Website;
                existing.NatureOfBusiness = model.NatureOfBusiness;
                existing.ProductsServicesOffered = model.ProductsServicesOffered;
                existing.GstNo = model.GstNo;
                existing.PanNo = model.PanNo;
                existing.IsoCertified = model.IsoCertified;
                existing.IsoDetails = model.IsoDetails;
                existing.DocumentsSubmittedJson = JsonSerializer.Serialize(model.DocumentsSubmitted);
                existing.BankDetailsJson = JsonSerializer.Serialize(model.BankDetail);
                existing.RegistrationStatus = model.RegistrationStatus;
                existing.Remarks = model.Remarks;
                existing.RecordedBy = model.RecordedBy;
                existing.VerifiedBy = model.VerifiedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.ApprovedBy = model.ApprovedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.RegisterNo = model.RegisterNo;

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
                model.CriteriaJson = JsonSerializer.Serialize(model.Criteria);
                model.POJson = JsonSerializer.Serialize(model.PurchaseOrders);
                model.IncomingPlanJson = JsonSerializer.Serialize(model.IncomingPlan);

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
                existing.CriteriaJson = JsonSerializer.Serialize(model.Criteria);
                existing.POJson = JsonSerializer.Serialize(model.PurchaseOrders);
                existing.IncomingPlanJson = JsonSerializer.Serialize(model.IncomingPlan);
                existing.SupplierRegisterId = model.SupplierRegisterId;
                existing.Email = model.Email;
                existing.MobileNo = model.MobileNo;
                existing.NatureOfBusiness= model.NatureOfBusiness;
                existing.EvaluatingPeriodFrom = model.EvaluatingPeriodFrom;
                existing.EvaluatingPeriodTo = model.EvaluatingPeriodTo;
                existing.PresentStatus = model.PresentStatus;
                existing.ProductsServicesOffered = model.ProductsServicesOffered;
                existing.ToContinued = model.ToContinued;
                existing.ToRemoved = model.ToRemoved;
                existing.Recommendation = model.Recommendation;
                existing.RegisterNo = model.RegisterNo;
                existing.GstNo = model.GstNo;
                existing.AcceptableLimitMin = model.AcceptableLimitMin;
                existing.Address = model.Address;
                existing.ContactPerson = model.ContactPerson;
                existing.ServiceProvider = model.ServiceProvider;
                existing.ApprovedBy = model.ApprovedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedDate = model.ApprovedDate;
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
                existing.ApprovedDate = model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.IsPresentStatus = model.IsPresentStatus;
                existing.EnlistmentDate = model.EnlistmentDate;
                existing.LastScore = model.LastScore;
                existing.ContactPerson = model.ContactPerson;
                existing.Email = model.Email;
                existing.MobileNo = model.MobileNo;
                existing.ProductApproved = model.ProductApproved;
                existing.ServiceProviderName = model.ServiceProviderName;
                existing.PreparedBy = model.PreparedBy;
                existing.AgreementDate = model.AgreementDate;
                existing.IsBlacklisted = model.IsBlacklisted;
                existing.BlacklistDate = model.BlacklistDate;
                existing.BlacklistReason = model.BlacklistReason;
                existing.SupplierRegisterId = model.SupplierRegisterId;
                existing.Remarks = model.Remarks;
                existing.RegisterNo = model.RegisterNo;
                existing.GstNo = model.GstNo;
                existing.Address = model.Address;
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
                //model.Status = "Draft";
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
                existing.Status = model.Status;
                existing.ApprovedDate = model.ApprovedDate;
                existing.Address = model.Address;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedById = model.PreparedById;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedDate = model.PreparedDate;

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
                model.InspectionParameterJson =JsonSerializer.Serialize(model.InspectionParameters);
                model.ItemsParametersJson = JsonSerializer.Serialize(model.ItemsParameters);

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
                existing.InspectionParameterJson =JsonSerializer.Serialize(model.InspectionParameters);
                existing.ItemsParametersJson = JsonSerializer.Serialize(model.ItemsParameters);
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReceivedBy = model.ReceivedBy;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.LotNo = model.LotNo;
                existing.MaterialCode = model.MaterialCode;
                existing.MaterialName = model.MaterialName;
                existing.InvoiceNo = model.InvoiceNo;
                existing.GrnNo = model.GrnNo;
                existing.Deviations = model.Deviations;
                existing.CorrectiveActions = model.CorrectiveActions;
                existing.RiskLevel = model.RiskLevel;
                existing.InspectionStage = model.InspectionStage;
                existing.ProductName = model.ProductName;
                existing.ProductCode = model.ProductCode;
                existing.Category = model.Category;
                existing.InspectionPlanNo = model.InspectionPlanNo;
                existing.PoNo = model.PoNo;
                existing.Email = model.Email;
                existing.PhoneNo = model.PhoneNo;
                existing.Address = model.Address;
                existing.GstNo = model.GstNo;
                existing.GeneralRemarks = model.GeneralRemarks;
                existing.OrderType = model.OrderType;
                existing.IndentNoPoNo = model.IndentNoPoNo;
                existing.InspectionPlanNoName = model.InspectionPlanNoName;


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
                model.InspectionResultsJson =  JsonSerializer.Serialize(model.Parameters);
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
                existing.OverallResult = model.OverallResult;
                existing.CorrectiveAction = model.CorrectiveAction;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.InspectionResultsJson =  JsonSerializer.Serialize(model.Parameters);
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ProductCode = model.ProductCode;
                existing.ProductName = model.ProductName;
                existing.InspectionStage = model.InspectionStage;
                existing.Category = model.Category;
                existing.Remarks =  model.Remarks;
                existing.PlanNo = model.PlanNo;
                existing.Risklevel = model.Risklevel;



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
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.Quantity = model.Quantity;
                existing.TechnicalSpecification = model.TechnicalSpecification;
                existing.ExpectedDate = model.ExpectedDate;
                existing.Remarks = model.Remarks;
                existing.IndentorName = model.IndentorName;
                existing.UnitOfMeasure = model.UnitOfMeasure;
                existing.PINo = model.PINo;
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
                model.ItemsJson = JsonSerializer.Serialize(model.Items);
                model.PODate = DateTime.UtcNow;
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
                existing.TotalAmount = model.TotalAmount;
                existing.Currency = model.Currency;
                existing.SpecialInstructions = model.SpecialInstructions;
                existing.IssuedBy = model.IssuedBy;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.GstAmount = model.GstAmount;
                existing.GSTNo = model.GSTNo;
                existing.GstPercentage = model.GstPercentage;
                existing.OrderType = model.OrderType;
                existing.GrandTotal = model.GrandTotal;
                existing.TearmCondition = model.TearmCondition;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ItemsJson = JsonSerializer.Serialize(model.Items);
                existing.SupplierAddress = model.SupplierAddress;
                existing.PONo = model.PONo;
                existing.Email = model.Email;
                existing.PhoneNo = model.PONo;
                existing.AuthorizedBy = model.AuthorizedBy;
                existing.ReferenceIndentNo = model.ReferenceIndentNo;
                existing.RequestedQuantity = model.RequestedQuantity;
                existing.ApprovedSupplierId = model.ApprovedSupplierId;
                existing.ReferenceIndentName = model.ReferenceIndentName;
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
                model.ItemsVerificationJson = JsonSerializer.Serialize(model.ItemsParameters);

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
                existing.ItemsVerificationJson = JsonSerializer.Serialize(model.ItemsParameters);
                existing.OverallStatus = model.OverallStatus;
                existing.GRNNumber = model.GRNNumber;
                existing.Remarks = model.Remarks;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.GstNo= model.GstNo;
                existing.SupplierName= model.SupplierName;
                existing.Email= model.Email;
                existing.Address= model.Address;
                existing.PhoneNo= model.PhoneNo;
                existing.InspectionBy= model.InspectionBy;
                existing.PODate= model.PODate;
                existing.InvoiceNo= model.InvoiceNo;
                existing.PODate= model.PODate;
                existing.InvoiceDate= model.InvoiceDate;
                existing.OrderType= model.OrderType;
                existing.CorrectiveActions= model.CorrectiveActions;
                existing.Deviations= model.Deviations;
                existing.PurchaseOrderNo= model.PurchaseOrderNo;
                existing.PoNo= model.PoNo;

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
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.MonthYear = model.MonthYear;
                existing.ReferenceNoDate = model.ReferenceNoDate;
                existing.ComplainantName = model.ComplainantName;
                existing.ComplaintNo = model.ComplaintNo;
                existing.ValidationOfComplaint = model.ValidationOfComplaint;
                existing.OutcomeOfInvestigation = model.OutcomeOfInvestigation;
                existing.SignatureQM = model.SignatureQM;
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
                model.RatingsJson = JsonSerializer.Serialize(model.Ratings);

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
                existing.Suggestions = model.Suggestions;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.RatingsJson = JsonSerializer.Serialize(model.Ratings);
                existing.ContactPerson = model.ContactPerson;
                existing.CompanyAddress = model.CompanyAddress;
                existing.Email = model.Email;
                existing.Note= model.Note;
                existing.MobileNo= model.MobileNo;
                existing.CompanyName= model.CompanyName;
                existing.Designation= model.Designation;
                existing.ReportedBy = model.ReportedBy;

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
                model.RatingsJson = JsonSerializer.Serialize(model.FeedbackRatings);

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
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.CustomerID = model.CustomerID;
                existing.RatingsJson = JsonSerializer.Serialize(model.FeedbackRatings);
                existing.ActionDetails  = model.ActionDetails;
                existing.ActionTaken = model.ActionTaken;
                existing.Address = model.Address;
                existing.AnalysisNo = model.AnalysisNo;
                existing.ContactPerson = model.ContactPerson;
                existing.CorrectiveActionRequired = model.CorrectiveActionRequired;
                existing.CustomerName = model.CustomerName;
                existing.CustomerRemarks = model.CustomerRemarks;
                existing.EffectivenessStatus = model.EffectivenessStatus;
                existing.Email = model.Email;
                existing.FinalStatus = model.FinalStatus;
                existing.Suggestions = model.Suggestions;
                existing.ImprovementOpportunity = model.ImprovementOpportunity;
                existing.IssuesIdentified = model.IssuesIdentified;
                existing.MobileNo = model.MobileNo;
                existing.RootCause =model.RootCause;
                existing.ResponsiblePerson =model.ResponsiblePerson;
                existing.NewRequirement = model.NewRequirement;
                existing.OverallConclusion = model.OverallConclusion;
                existing.OverallCustomerSatisfaction = model.OverallCustomerSatisfaction;
                existing.OverallGrade = model.OverallGrade;
                existing.AverageRating = model.AverageRating;
                existing.PositiveObservations = model.PositiveObservations;
                existing.VerificationRemarks = model.VerificationRemarks;
                existing.AnalysisDate = model.AnalysisDate;
                existing.TargetCompletionDate = model.TargetCompletionDate;
                existing.VerificationDate = model.VerificationDate;
                existing.FeedbackDate = model.FeedbackDate;


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
                model.AgendaItemsJson = JsonSerializer.Serialize(model.AgendaItems);
                model.ParticipantsJson = JsonSerializer.Serialize(model.Participants);
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
                existing.AgendaItemsJson = JsonSerializer.Serialize(model.AgendaItems);
                existing.ParticipantsJson = JsonSerializer.Serialize(model.Participants);
                existing.MeetingNo = model.MeetingNo;
                existing.MeetingTime = model.MeetingTime;
                existing.ApprovedBy = model.ApprovedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedDate = model.ApprovedDate;
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
                model.AgendaItemsJson = JsonSerializer.Serialize(model.AgendaList);
                model.AttendeesJson = JsonSerializer.Serialize(model.ParticipantItems);
                model.ActionPlanJson = JsonSerializer.Serialize(model.ActionItems);

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
                existing.NextMeetingDate = model.NextMeetingDate;
                existing.NextMeetingAgenda = model.NextMeetingAgenda;
                existing.ActionClosureStatus = model.ActionClosureStatus;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.AgendaItemsJson = JsonSerializer.Serialize(model.AgendaList);
                existing.AttendeesJson = JsonSerializer.Serialize(model.ParticipantItems);
                existing.ActionPlanJson = JsonSerializer.Serialize(model.ActionItems);
                existing.ApprovedBy = model.ApprovedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.MeetingId = model.MeetingId;
                existing.MeetingNo = model.MeetingNo;
                existing.MeetingVenue = model.MeetingVenue;
                existing.MeetingTime = model.MeetingTime;
                existing.OverallConclusion = model.OverallConclusion;

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

                // First saved tab
                model.CurrentStep = model.RequestStep;

                var id = await _repository.Add("NonConformingWork", model);

                await LogAudit("NonConformingWork", id, "Created", null, body.GetRawText());

                _logger.LogInformation("NonConformingWork created with ID {Id}.", id);

                return id;
            }
            else
            {
                var existing = await _context.NablNonConformingWorks
                    .FirstOrDefaultAsync(x =>
                        x.ID == model.ID &&
                        x.IsActive &&
                        x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("NonConformingWork not found!");

                await SaveRevisionSnapshot("NonConformingWork", existing);

                // ===========================
                // Common Fields
                // ===========================

                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;

                existing.RequestStep = model.RequestStep;

                if (existing.CurrentStep < model.RequestStep)
                {
                    existing.CurrentStep = model.RequestStep;
                }

                if (existing.CurrentStep >= 5)
                {
                    existing.Status = "Completed";
                }

                // ===========================
                // Update Main Table only when
                // General tab is submitted
                // ===========================

                if (model.RequestStep == 1)
                {
                    existing.NCDate = model.NCDate;
                    existing.Date = model.Date;

                    existing.SampleCode = model.SampleCode;
                    existing.TestParameter = model.TestParameter;
                    existing.NCDescription = model.NCDescription;
                    existing.NCSource = model.NCSource;

                    existing.DetectedBy = model.DetectedBy;
                    existing.IdentifiedBy = model.IdentifiedBy;

                    existing.SuspendedWork = model.SuspendedWork;
                    existing.AffectedResults = model.AffectedResults;

                    existing.NCCategory = model.NCCategory;
                    existing.RootCauseAnalysis = model.RootCauseAnalysis;

                    existing.DepartmentId = model.DepartmentId;
                    existing.DepartmentName = model.DepartmentName;

                    existing.ReportedByEmployeeId = model.ReportedByEmployeeId;
                    existing.ReportedByEmployeeName = model.ReportedByEmployeeName;

                    existing.NcNo = model.NcNo;

                    existing.Source = model.Source;
                    existing.Category = model.Category;
                    existing.Priority = model.Priority;

                    existing.ReferenceModule = model.ReferenceModule;
                    existing.ReferenceId = model.ReferenceId;
                    existing.ReferenceNo = model.ReferenceNo;

                    existing.CustomerAffected = model.CustomerAffected;

                    existing.Description = model.Description;
                    existing.ImmediateAction = model.ImmediateAction;
                    existing.ProblemDescription = model.ProblemDescription;

                    existing.PreparedDate = model.PreparedDate;
                    existing.ReviewedDate = model.ReviewedDate;
                    existing.ApprovedDate = model.ApprovedDate;

                    existing.ReviewedBy = model.ReviewedBy;
                    existing.ApprovedBy = model.ApprovedBy;

                    existing.CloserDate = model.CloserDate;
                    existing.SignatureTDQM = model.SignatureTDQM;
                }

                // Keep latest workflow values in request model
                model.CurrentStep = existing.CurrentStep;
                model.Status = existing.Status;
                model.ModifiedOn = existing.ModifiedOn;
                model.ModifiedBy = existing.ModifiedBy;
                model.CompanyCode = existing.CompanyCode;
                model.CreatedOn = existing.CreatedOn;
                model.CreatedBy = existing.CreatedBy;
                model.FormCode = existing.FormCode;
                model.DocumentNo = existing.DocumentNo;

                // Repository receives ORIGINAL request model
                await _repository.Update("NonConformingWork", model);

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
                existing.ApprovedBy = model.ApprovedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ActivityAssessed = model.ActivityAssessed;
                existing.AuditNo = model.AuditNo;
                existing.Auditee = model.Auditee;
                existing.Auditor = model.Auditor;
                existing.CorrectiveActionProposed = model.CorrectiveActionProposed;
                existing.DepartmentID = model.DepartmentID;
                existing.ImplementedById = model.ImplementedById;
                existing.ObservedByID = model.ObservedByID;
                existing.ProposedById = model.ProposedById;
                existing.SignOfAuditorID = model.SignOfAuditorID;
                existing.SignatureOfQMID = model.SignatureOfQMID;
                existing.VerifiedById = model.VerifiedById;
                existing.ClauseNo = model.ClauseNo;
                existing.VerifiedByName = model.VerifiedByName;
                existing.SignatureOfQMName = model.SignatureOfQMName;
                existing.TimeRequirement = model.TimeRequirement;
                existing.ProposedByName = model.ProposedByName;
                existing.ObservedByName = model.ObservedByName;
                existing.SignOfAuditorName = model.SignOfAuditorName;
                existing.ImplementedByName = model.ImplementedByName;
                existing.DepartmentName = model.DepartmentName;
                existing.EffectivenessOfAction = model.EffectivenessOfAction;
                existing.NcNo = model.NcNo;
                existing.NcObserved = model.NcObserved;
                existing.CorrectiveActionDate = model.CorrectiveActionDate;
                existing.ImplementedDate = model.ImplementedDate;
                existing.VerifiedDate = model.VerifiedDate;
                existing.CorrectiveActionTaken= model.CorrectiveActionTaken;


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

                foreach (var log in model.InitialTestingLogs)
                {
                    //log.NablRetesting = model;

                    log.LatestResultPrefix = log.ResultPrefix;
                    log.LatestResultValue = log.ResultValue;

                    log.ModifiedDate = DateTime.UtcNow;
                }
                var id = await _repository.Add("Retesting", model);

                await LogAudit("Retesting", id, "Created", null, body.GetRawText());
                _logger.LogInformation("Retesting created with ID {Id}.", id);

                return id;
            }
            else
            {
                var existing = await _context.NablRetestings
                    .Include(x => x.InitialTestingLogs)
                    .Include(x => x.RetestingLogs)
                    .FirstOrDefaultAsync(x =>
                        x.ID == model.ID &&
                        x.IsActive &&
                        x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("Retesting not found!");

                await SaveRevisionSnapshot("Retesting", existing);

                existing.SampleCode = model.SampleCode;
                existing.OriginalTestDate = model.OriginalTestDate;
                existing.RetestReason = model.RetestReason;
                existing.RetestDate = model.RetestDate;
                existing.TestParameter = model.TestParameter;
                existing.TestMethodName = model.TestMethodName;
                existing.OriginalResult = model.OriginalResult;
                existing.RetestResult = model.RetestResult;
                existing.Unit = model.Unit;
                existing.AcceptanceCriteria = model.AcceptanceCriteria;
                existing.RetestConclusion = model.RetestConclusion;
                existing.TestedBy = model.TestedBy;
                existing.AuthorizedBy = model.AuthorizedBy;
                existing.Remarks = model.Remarks;

                existing.QcPlanNoId = model.QcPlanNoId;
                existing.QcPlanActivityId = model.QcPlanActivityId;
                existing.PlanNo = model.PlanNo;
                existing.PlanYear = model.PlanYear;
                existing.Discipline = model.Discipline;
                existing.MaterialProductGroup = model.MaterialProductGroup;
                existing.LabIncharge = model.LabIncharge;
                existing.QcActivity = model.QcActivity;
                existing.DepartmentName = model.DepartmentName;
                existing.ReferenceType = model.ReferenceType;
                existing.ReferenceName = model.ReferenceName;
                existing.FrequencyType = model.FrequencyType;
                existing.ResponsibleEmployee = model.ResponsibleEmployee;
                existing.EffectiveFrom = model.EffectiveFrom;
                existing.EffectiveTo = model.EffectiveTo;
                existing.NextDueDate = model.NextDueDate;
                existing.ApprovedBy = model.ApprovedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.ReviewedDate = model.ReviewedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                foreach (var log in model.InitialTestingLogs)
                {
                    var existingLog = existing.InitialTestingLogs
                        .FirstOrDefault(x => x.Id == log.Id);

                    if (existingLog == null)
                    {
                        log.RetestingRetainedSampleId = existing.ID;

                        log.LatestResultPrefix = log.ResultPrefix;
                        log.LatestResultValue = log.ResultValue;
                        log.ModifiedDate = DateTime.UtcNow;

                        existing.InitialTestingLogs.Add(log);
                    }
                    else
                    {
                        existingLog.DateOfTesting = log.DateOfTesting;
                        existingLog.SampleId = log.SampleId;

                        existingLog.ResultPrefix = log.ResultPrefix;
                        existingLog.ResultValue = log.ResultValue;

                        existingLog.TestedById = log.TestedById;
                        existingLog.TestedByName = log.TestedByName;

                        existingLog.Remarks = log.Remarks;
                    }
                }
                foreach (var log in model.RetestingLogs)
                {
                    var existingLog = existing.RetestingLogs
                        .FirstOrDefault(x => x.Id == log.Id);

                    if (existingLog == null)
                    {
                        log.RetestingRetainedSampleId = existing.ID;

                        existing.RetestingLogs.Add(log);

                        var initial = existing.InitialTestingLogs
                            .FirstOrDefault(x => x.Id == log.InitialTestLogId);

                        if (initial != null)
                        {
                            bool previousChanged =
                                initial.ResultPrefix != log.PreviousPrefix ||
                                initial.ResultValue != log.PreviousValue;

                            // User ne Previous Test Result edit kiya
                            if (previousChanged)
                            {
                                initial.ResultPrefix = log.PreviousPrefix;
                                initial.ResultValue = log.PreviousValue;
                                initial.ModifiedDate = DateTime.UtcNow;
                            }

                            // Latest Test Result hamesha Retesting Result hoga
                            initial.LatestResultPrefix = log.RetestPrefix;
                            initial.LatestResultValue = log.RetestValue;
                        }
                    }
                }

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
                model.RisksJson = JsonSerializer.Serialize(model.ActionPlans);
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
                existing.RisksJson = JsonSerializer.Serialize(model.ActionPlans);
                existing.OverallRiskLevel = model.OverallRiskLevel;
                existing.AssessedBy = model.AssessedBy;
                existing.RiskDate= model.RiskDate;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.RiskNo = model.RiskNo;
                existing.DepartmentName = model.DepartmentName;
                existing.Impact = model.Impact;
                existing.Type = model.Type;
                existing.Likelihood = model.Likelihood;
                existing.Category = model.Category;
                existing.IdentifiedByName = model.IdentifiedByName;
                existing.RiskLevel = model.RiskLevel;
                existing.DepartmentId = model.DepartmentId;
                existing.IdentifiedById = model.IdentifiedById;
                existing.RiskScore = model.RiskScore;
                existing.Opportunity = model.Opportunity;
                existing.ExistingSituation = model.ExistingSituation;
                existing.ExpectedBenefit = model.ExpectedBenefit;
                existing.Title = model.Title;
                existing.ExistingControls = model.ExistingControls;
                existing.RiskOwner = model.RiskOwner;
                existing.EffectivenessRemarks = model.EffectivenessRemarks;
                existing.Effectiveness = model.Effectiveness;
                existing.RiskRemarks = model.RiskRemarks;


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
                model.SourcesJson = JsonSerializer.Serialize(model.UncertaintySources);

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
                existing.TestMethodName = model.TestMethodName;
                existing.MatrixType = model.MatrixType;
                existing.UncertaintyType = model.UncertaintyType;
                existing.SourcesJson = JsonSerializer.Serialize(model.UncertaintySources);
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
                existing.EffectiveDate = model.EffectiveDate;
                existing.MUCode = model.MUCode;
                existing.LaboratoryTestID = model.LaboratoryTestID;
                existing.TestMethodID = model.TestMethodID;
                existing.EquipmentID = model.EquipmentID;
                existing.EquipmentName = model.EquipmentName;
                existing.LaboratoryTestName = model.LaboratoryTestName;
                existing.Version = model.Version;
                existing.Remarks = model.Remarks;
                existing.SumOfSquares = model.SumOfSquares;
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
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
                model.ActivitiesJson = JsonSerializer.Serialize(model.Activities);
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
                existing.LaboratoryId= model.LaboratoryId;
                existing.LaboratoryName= model.LaboratoryName;
                existing.FieldOfAccreditation= model.FieldOfAccreditation;
                existing.Date = model.Date;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.ActivitiesJson = JsonSerializer.Serialize(model.Activities);
                existing.ApprovedBy = model.ApprovedBy;
                existing.ApprovedDate = model.ApprovedDate;
                existing.ReviewedBy = model.ReviewedBy;
                existing.ReviewedDate = model.ReviewedDate;
                existing.PreparedBy = model.PreparedBy;
                existing.PreparedDate = model.PreparedDate;
                existing.Note = model.Note;

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

        // ─── Form Defaults & Suggested Reviewers ────────────────────────

        public async Task<object> GetFormDefaults(string formType)
        {
            var org = await _context.Organizations.FirstOrDefaultAsync();
            var nabl = await _context.NablAccreditations.FirstOrDefaultAsync();
            var employee = await _context.EmployeeMasters
                .FirstOrDefaultAsync(e => e.ID == loggedInUser.EmployeeID);

            var formCode = FormCodeMap.TryGetValue(formType, out var code) ? code : formType;

            return new
            {
                formCode,
                companyName = org?.LabName ?? "",
                companyAddress = org?.LabAddress ?? "",
                contactEmail = org?.ContactEmail ?? "",
                contactPhone = org?.ContactPhone ?? "",
                organizationLogo = org?.OrganizationLogo ?? "",
                nablTcNumber = nabl?.CertificateNumber ?? "",
                nablLogo = nabl?.LogoPath ?? "",
                preparedBy = employee?.Name ?? loggedInUser.Name ?? "",
                preparedById = loggedInUser.EmployeeID
            };
        }

        public async Task<object> GetSuggestedReviewers()
        {
            // Get employees with Reviewer or Approver roles, or Lab Manager / Quality Manager designations
            var employees = await _context.EmployeeMasters
                .Where(e => e.IsActive && e.CompanyCode == loggedInUser.CompanyCode)
                .Select(e => new
                {
                    id = e.ID,
                    name = e.Name,
                    designation = e.Designation != null ? e.Designation.Name : "",
                    department = e.Department != null ? e.Department.Name : ""
                })
                .ToListAsync();

            // For now, return all active employees as potential reviewers/approvers
            // The frontend can filter by role/designation as needed
            return new
            {
                reviewers = employees,
                approvers = employees
            };
        }
        public async Task<List<DropdwonSelector>> GetTraningPlanDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetTraningPlanDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> Roomdropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.Roomdropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> Supplierlist(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.Supplierlist(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> AllSupplierlist(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.AllSupplierlist(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> Alltestmethodlist(string formType, string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.Alltestmethodlist(formType, searchTerm, pageNo, pageSize);
        }
        public async Task<UploadFile> UploadSignatureAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, Dtos.FileType.Nabl, null, string.Empty);
            var relativePath = uploaded.FilePath;
            return uploaded;
        }
        public async Task<string> GetNextRegisterNo()
        {
            var year = DateTime.Now.Year;

            var lastRecord = await _context.NablSupplierRegistrations
                .Where(x => x.RegisterNo != null && x.RegisterNo.StartsWith($"SUP-{year}-"))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null && !string.IsNullOrEmpty(lastRecord.RegisterNo))
            {
                var lastNumberText = lastRecord.RegisterNo.Split('-').Last();

                if (int.TryParse(lastNumberText, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"SUP-{year}-{nextNumber:D3}";
        }
        public async Task<string> GetNextIndentNo()
        {
            var year = DateTime.Now.Year;
            string companyCode = loggedInUser.CompanyCode ?? "LIMS";

            // Expected Format: LIMS-PI-YYYY-XXX
            var prefix = $"{companyCode}-PI-{year}-";

            var lastRecord = await _context.NablPurchaseIndents
                .Where(x => x.PINo != null && x.PINo.StartsWith(prefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null && !string.IsNullOrWhiteSpace(lastRecord.PINo))
            {
                // Example: LIMS-PI-2026-001
                // Split -> ["LIMS", "PI", "2026", "001"]
                var lastNumberText = lastRecord.PINo.Split('-').Last();

                if (int.TryParse(lastNumberText, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            // Final Format: LIMS-PI-2026-001
            return $"{companyCode}-PI-{year}-{nextNumber:D3}";
        }
        public async Task<string> GetNextPlanNo()
        {
            var year = DateTime.Now.Year;
            string companyCode = loggedInUser.CompanyCode ?? "LIMS";

            string prefix = $"{companyCode}-PSIP-{year}-";

            var lastRecord = await _context.NablProductInspections
                .Where(x => x.PlanNo != null && x.PlanNo.StartsWith(prefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null && !string.IsNullOrEmpty(lastRecord.PlanNo))
            {
                var lastNumberText = lastRecord.PlanNo.Split('-').Last();

                if (int.TryParse(lastNumberText, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D3}";
        }
        public async Task<List<DropdwonSelector>> IndentNoList(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.IndentNoList(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> ApprovedSupplierlist(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.ApprovedSupplierlist(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> PlanNoDetailslist(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.PlanNoDetailslist(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> PONoListDetailslist(string? formType, string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.PONoListDetailslist(formType, searchTerm, pageNo, pageSize);
        }
        public async Task<SupplierEvaluationDetailsDto> SupplierEvaluationDetails(string supplierName, DateTime? fromDate, DateTime? toDate)
        {
            return await _repository.SupplierEvaluationDetails(supplierName, fromDate, toDate);
        }
        public async Task<List<Items>> PoitemsDetails(string poNo, string supplierName)
        {
            return await _repository.PoitemsDetails(poNo, supplierName);
        }
        public async Task<List<CombinedPoItemDto>> ReceivedItemsDetails(string poNo, string supplierName)
        {
            return await _repository.ReceivedItemsDetails(poNo, supplierName);
        }
        public async Task<List<InspectionParameters>> InspectionPlanDetails(string inspectionPlanNo)
        {
            return await _repository.InspectionPlanDetails(inspectionPlanNo);
        }
        public async Task<NablPurchaseIndentDto> IndentDetails(string indentNo)
        {
            return await _repository.IndentDetails(indentNo);
        }
        public async Task<NablTestMethodValidationDto> TestMethodDetails(string testmethodCode)
        {
            return await _repository.TestMethodDetails(testmethodCode);
        }
        public async Task<string> GetNextMaterialNo()
        {
            const string prefix = "CRM";

            var lastRecord = await _context.NablReferenceMaterials
                .Where(x => !string.IsNullOrEmpty(x.RMCode)
                            && x.RMCode.StartsWith(prefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null)
            {
                var parts = lastRecord.RMCode.Split('-');

                if (parts.Length > 1 &&
                    int.TryParse(parts[1], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}-{nextNumber:D3}";
        }
        public async Task<List<DropdwonSelector>> GetSupplierDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetSupplierDropdown(searchTerm, pageNo, pageSize);
        }
        private async Task<long> SaveInventoryMaster(JsonElement body)
        {
            var model = JsonSerializer.Deserialize<InventoryManagement>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid Inventory Management data.");

            if (model.ID == 0)
            {
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode ?? "LIMS";
                var id = await _repository.Add("InventoryMaster", model);
                return id;
            }
            else
            {
                var existing = await _context.InventoryManagements
                    .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

                if (existing == null)
                    throw new InvalidOperationException("Inventory Management not found!");


                existing.ItemCode = model.ItemCode;
                existing.ItemName= model.ItemName;
                existing.ItemCategory= model.ItemCategory;
                existing.Quantity = model.Quantity;
                existing.MinimumQuantity= model.MinimumQuantity;
                existing.ItemDescription= model.ItemDescription;
                existing.DepartmentID= model.DepartmentID;
                existing.SupplierId= model.SupplierId;
                existing.Manufacturer = model.Manufacturer;
                existing.BatchNo= model.BatchNo;
                existing.Unit= model.Unit;
                existing.StorageLocation= model.StorageLocation;
                existing.Date= model.Date;
                existing.Remarks= model.Remarks;
                existing.ModifiedOn = DateTime.UtcNow;
                existing.ModifiedBy = loggedInUser.EmployeeID;
                existing.SupplierName = model.SupplierName;
                await _repository.Update("InventoryMaster", existing);
                await LogAudit("InventoryMaster", existing.ID, "Updated", null, body.GetRawText());
                _logger.LogInformation("InventoryMaster ID {Id} updated.", existing.ID);
                return existing.ID;
            }
        }

        public async Task<InventoryQuantityLog> Addquantity(string? formType, JsonElement body)
        {
            var model = JsonSerializer.Deserialize<AddInventoryQuantityDto>(body.GetRawText(), _jsonOptions)
                  ?? throw new ArgumentException("Invalid Inventory Management data.");
            if (model.InventoryId <= 0)
                throw new InvalidOperationException("Inventory Management not found!");
            if (model.AddedQuantity <= 0)
                throw new InvalidOperationException("Inventory Management not found!");

            var existing = await _context.InventoryManagements
                     .FirstOrDefaultAsync(x => x.ID == model.InventoryId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            var previousQty = existing.Quantity;
            var newQty = previousQty + model.AddedQuantity;
            existing.Quantity = newQty;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repository.Update("InventoryMaster", existing);
            var log = new InventoryQuantityLog
            {
                InventoryId = existing.ID,   // FK stored here
                AddedQuantity = model.AddedQuantity,
                PreviousQuantity = previousQty,
                NewQuantity = newQty,
                AddedDate = DateTime.UtcNow,
                AddedBy = loggedInUser.EmployeeID,
                IsActive = true
            };
            await _repository.AddQuantityLog(log);
            return log;
        }
        public async Task<List<InventoryQuantityLog>?> GetQuantityLogs(string formType, long inventoryId)
        {
            var existing = await _context.InventoryQuantityLogs
                     .Where(x => x.InventoryId == inventoryId && x.IsActive).ToListAsync();

            return existing;
        }
        public async Task<List<DropdwonSelector>> GetMaterialData(string formType, string type)
        {
            return await _repository.GetMaterialData(formType, type);
        }
        public async Task<InventoryManagementDto> GetInventoryDetails(string itemCode, string itemName)
        {
            return await _repository.GetInventoryDetails(itemCode, itemName);
        }
        public async Task<ReferenceMaterialConsumptionLog> AddConsumption(string? formType, JsonElement body)
        {
            var model = JsonSerializer.Deserialize<CrmConsumptionLogDto>(body.GetRawText(), _jsonOptions)
                ?? throw new ArgumentException("Invalid Reference Material data.");

            if (model.ReferenceMaterialId <= 0)
                throw new InvalidOperationException("Reference Material not found!");

            if (model.QuantityConsumed <= 0)
                throw new InvalidOperationException("Consumed quantity must be greater than 0.");

            var referenceMaterial = await _context.NablReferenceMaterials
                .FirstOrDefaultAsync(x =>
                    x.ID == model.ReferenceMaterialId &&
                    x.IsActive &&
                    x.CompanyCode == loggedInUser.CompanyCode);

            if (referenceMaterial == null)
                throw new InvalidOperationException("Reference Material not found!");

            var lastLog = await _context.ReferenceMaterialConsumptionLogs
                .Where(x =>
                    x.ReferenceMaterialId == model.ReferenceMaterialId &&
                    x.IsActive)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            decimal previousQty;

            if (lastLog == null)
            {
                previousQty = referenceMaterial.InitialQuantity;
            }
            else
            {
                previousQty = lastLog.BalanceQty;
            }

            if (previousQty <= 0)
                throw new InvalidOperationException(
                    $"No quantity available for consumption. Current balance is {previousQty} {referenceMaterial.Unit}."
                );

            if (model.QuantityConsumed > previousQty)
                throw new InvalidOperationException(
                    $"Consumed quantity ({model.QuantityConsumed} {referenceMaterial.Unit}) cannot be greater than available balance ({previousQty} {referenceMaterial.Unit})."
                );

            var newQty = previousQty - model.QuantityConsumed;

            var log = new ReferenceMaterialConsumptionLog
            {
                ReferenceMaterialId = referenceMaterial.ID,
                QuantityConsumed = model.QuantityConsumed,
                PreviousBalanceQty = previousQty,
                BalanceQty = newQty,
                ConsumptionDate = model.ConsumptionDate,
                UsedBy = model.UsedBy,
                EquipmentOrTest = model.EquipmentOrTest,
                Purpose = model.Purpose,
                Remarks = model.Remarks,
                IsActive = true,
            };

            await _context.ReferenceMaterialConsumptionLogs.AddAsync(log);
            await _context.SaveChangesAsync();

            return log;
        }
        public async Task<List<DropdwonSelector>> GetEmployeesDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetEmployeesDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> GetReferenceOptions(string? referenceType)
        {
            return await _repository.GetReferenceOptions(referenceType);
        }
        public async Task<string> GetNextQCPlanNo()
        {
            var year = DateTime.Now.Year;
            var prefix = $"QC-PLAN-{year}";

            var lastRecord = await _context.NablQualityControlPlans
                .Where(x => x.IsActive
                            && !string.IsNullOrEmpty(x.PlanNo)
                            && x.PlanNo.StartsWith(prefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null)
            {
                var parts = lastRecord.PlanNo.Split('-');

                if (parts.Length >= 4 &&
                    int.TryParse(parts[3], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}-{nextNumber:D3}";
        }
        private DateTime? CalculateNextDuaDate(string frequencyType, DateTime? effectiveForm)
        {
            if (effectiveForm == null || string.IsNullOrEmpty(frequencyType))
                return null;

            return frequencyType switch
            {
                "Daily" => effectiveForm.Value.AddDays(1),
                "Weekly" => effectiveForm.Value.AddDays(7),
                "Monthly" => effectiveForm.Value.AddMonths(1),
                "Quarterly" => effectiveForm.Value.AddMonths(3),
                "Half-Yearly" => effectiveForm.Value.AddMonths(6),
                "Yearly" => effectiveForm.Value.AddYears(1),
                _ => null
            };
        }
        public async Task<List<DropdwonSelector>> GetQcplannoDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetQcplannoDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<RetestingQcPlanDetailsDto> QCDetails(long id)
        {
            return await _repository.QCDetails(id);
        }
        public async Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetCustomerDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<CustomerFeedbackAnalysisDto> GetFeedbackDetails(long id)
        {
            return await _repository.GetFeedbackDetails(id);
        }
        public async Task<string> GetNextAnalysisNo()
        {
            var year = DateTime.Now.Year;
            const string companyCode = "CFA";

            // Format: CFA-2026-
            var prefix = $"{companyCode}-{year}-";

            var lastRecord = await _context.NablFeedbackAnalyses
                .Where(x => x.AnalysisNo != null && x.AnalysisNo.StartsWith(prefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null)
            {
                var lastNumberText = lastRecord.AnalysisNo.Split('-').Last();

                if (int.TryParse(lastNumberText, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            // Final Format: CFA-2026-001
            return $"{companyCode}-{year}-{nextNumber:D3}";
        }
        public async Task<string> GetNextMeetingNo()
        {
            var year = DateTime.Now.Year;
            const string companyCode = "MRM";

            // Format: CFA-2026-
            var prefix = $"{companyCode}-{year}-";

            var lastRecord = await _context.NablMeetingAgendas
                .Where(x => x.MeetingNo != null && x.MeetingNo.StartsWith(prefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null)
            {
                var lastNumberText = lastRecord.MeetingNo.Split('-').Last();

                if (int.TryParse(lastNumberText, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{companyCode}-{year}-{nextNumber:D3}";
        }

        public async Task<List<DropdwonSelector>> GetMeetinglist(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetMeetinglist(searchTerm, pageNo, pageSize);
        }
        public async Task<MeetingAgendaDto> GetMeetingDetails(string meetingNo)
        {
            return await _repository.GetMeetingDetails(meetingNo);
        }
        public async Task<List<PurchaseMaterialVerificationPrintDto>> GetPurchaseMaterialVerificationPrintList()
        {
            return await _repository.GetPurchaseMaterialVerificationPrintList();
        }
        public async Task<string> GetNextNCNo()
        {
            var year = DateTime.Now.Year;
            const string prefix = "NC";

            // Format: NC-2026-001
            var codePrefix = $"{prefix}-{year}-";

            var lastRecord = await _context.NablNonConformingWorks
                .Where(x => !string.IsNullOrEmpty(x.NcNo) &&
                            x.NcNo.StartsWith(codePrefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null)
            {
                var lastPart = lastRecord.NcNo.Split('-').Last();

                if (int.TryParse(lastPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}-{year}-{nextNumber:D3}";
        }
        public async Task<string> GetNextActionNo()
        {
            var year = DateTime.Now.Year;
            const string prefix = "CA";

            // Format : CA-2026-001
            var codePrefix = $"{prefix}-{year}-";

            var lastRecord = await _context.NablNonConformingWorkCorrectiveActions
                .Where(x => !string.IsNullOrWhiteSpace(x.ActionNo)
                         && x.ActionNo.StartsWith(codePrefix))
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null)
            {
                var lastPart = lastRecord.ActionNo.Split('-').LastOrDefault();

                if (int.TryParse(lastPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}-{year}-{nextNumber:D3}";
        }
        public async Task<PagedResponse<object>> NcPrintList(PageFilter filter)
        {
            return await _repository.NcPrintList(filter);
        }
        public async Task<string> GetNextMUNo()
        {
            var year = DateTime.Now.Year;
            const string prefix = "MU";

            // Format : CA-2026-001
            var codePrefix = $"{prefix}-{year}-";

            var lastRecord = await _context.NablMeasurementUncertainties
                .Where(x => !string.IsNullOrWhiteSpace(x.MUCode)
                         && x.MUCode.StartsWith(codePrefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null)
            {
                var lastPart = lastRecord.MUCode.Split('-').LastOrDefault();

                if (int.TryParse(lastPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}-{year}-{nextNumber:D3}";
        }
        public async Task<string> GetNextRiskNo()
        {
            var year = DateTime.Now.Year;
            const string prefix = "RSK";

            // Format : CA-2026-001
            var codePrefix = $"{prefix}-{year}-";

            var lastRecord = await _context.NablRiskAssessments
                .Where(x => !string.IsNullOrWhiteSpace(x.RiskNo)
                         && x.RiskNo.StartsWith(codePrefix))
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastRecord != null)
            {
                var lastPart = lastRecord.RiskNo.Split('-').LastOrDefault();

                if (int.TryParse(lastPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}-{year}-{nextNumber:D3}";
        }
    }
}
