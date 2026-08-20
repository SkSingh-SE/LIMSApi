using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Migrations;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LIMSApi.Repositories
{
    public class NablRepository : INablRepository
    {
        private readonly LIMSContext _context;
        private readonly LoggedInUserDTO loggedInUser;

        // Cache of searchable string property names per NABL form type (excluding sensitive/audit fields)
        private static readonly ConcurrentDictionary<Type, string[]> _searchablePropsCache = new();

        // Audit/sensitive string fields excluded from reflection-based search
        private static readonly HashSet<string> _excludedProps = new(StringComparer.OrdinalIgnoreCase)
        {
            "CompanyCode", "RejectionRemarks"
        };

        public NablRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        private static string[] GetSearchableStringProps(Type t)
        {
            return _searchablePropsCache.GetOrAdd(t, type =>
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType == typeof(string)
                                && p.GetCustomAttribute<NotMappedAttribute>() == null
                                && !_excludedProps.Contains(p.Name))
                    .Select(p => p.Name)
                    .ToArray());
        }

        public async Task<PagedResponse<object>> GetAll(string formType, PageFilter filter)
        {
            return formType switch
            {
                "JobDescription" => await GetAllTyped<NablJobDescription>(filter),
                "ResponsibilityAuthority" => await GetAllTyped<NablResponsibilityAuthority>(filter),
                "EmployeeCompetence" => await GetAllTyped<NablEmployeeCompetence>(filter),
                "EmployeePerformanceRecord" => await GetAllTyped<NablEmployeePerformanceRecord>(filter),
                "EmployeeAuthorization" => await GetEquipmentAuthorizationList(filter),
                "CompetenceRequirement" => await GetAllTyped<NablCompetenceRequirement>(filter),
                "InductionTraining" => await GetAllTyped<NablInductionTraining>(filter),
                "SkillMatrix" => await GetAllTyped<NablSkillMatrix>(filter),
                "SkillMatrixDecision" => await GetAllTyped<NablSkillMatrixDecision>(filter),
                "TrainingPlan" => await GetAllTyped<NablTrainingPlan>(filter),
                "TrainingAttendance" => await GetAllTyped<NablTrainingAttendance>(filter),
                "TrainingEffectiveness" => await GetAllTyped<NablTrainingEffectiveness>(filter),
                "EnvironmentMonitoring" => await GetAllTyped<NablEnvironmentMonitoring>(filter),
                "QualityControlPlan" => await GetAllTyped<NablQualityControlPlan>(filter),
                "TestRequest" => await GetAllTyped<NablTestRequest>(filter),
                "TestMethod" => await GetAllTyped<NablTestMethod>(filter),
                "MethodVerification" => await GetAllTyped<NablMethodVerification>(filter),
                "MethodValidation" => await GetAllTyped<NablMethodValidation>(filter),
                "SampleInwardRegister" => await GetAllTyped<NablSampleInwardRegister>(filter),
                "SampleMusterRegister" => await GetAllTyped<NablSampleMusterRegister>(filter),
                "SampleLabel" => await GetAllTyped<NablSampleLabel>(filter),
                "TechnicalRawData" => await GetAllTyped<NablTechnicalRawData>(filter),
                "TestReport" => await GetAllTyped<NablTestReport>(filter),
                "EquipmentHistory" => await GetAllTyped<NablEquipmentHistory>(filter),
                "CalibrationReview" => await GetAllTyped<NablCalibrationReview>(filter),
                "IntermediateCheck" => await GetAllTyped<NablIntermediateCheck>(filter),
                "ReferenceMaterial" => await GetAllTyped<NablReferenceMaterial>(filter),
                "CrmConsumption" => await GetAllTyped<NablCrmConsumption>(filter),
                "SupplierRegistration" => await GetAllTyped<NablSupplierRegistration>(filter),
                "SupplierEvaluation" => await GetAllTyped<NablSupplierEvaluation>(filter),
                "ApprovedSupplier" => await GetAllTyped<NablApprovedSupplier>(filter),
                "SupplierConfidentiality" => await GetAllTyped<NablSupplierConfidentiality>(filter),
                "IncomingMaterial" => await GetAllTyped<NablIncomingMaterial>(filter),
                "ProductInspection" => await GetAllTyped<NablProductInspection>(filter),
                "PurchaseIndent" => await GetAllTyped<NablPurchaseIndent>(filter),
                "PurchaseOrder" => await GetAllTyped<NablPurchaseOrder>(filter),
                "PurchaseMaterialVerification" => await GetAllTyped<NablPurchaseMaterialVerification>(filter),
                "Complaint" => await GetAllTyped<NablComplaint>(filter),
                "CustomerFeedback" => await GetAllTyped<NablCustomerFeedback>(filter),
                "FeedbackAnalysis" => await GetAllTyped<NablFeedbackAnalysis>(filter),
                "AuditPlan" => await GetAllTyped<NablAuditPlan>(filter),
                "AuditChecklist" => await GetAllTyped<NablAuditChecklist>(filter),
                "AuditSummary" => await GetAllTyped<NablAuditSummary>(filter),
                "InternalAuditor" => await GetAllTyped<NablInternalAuditor>(filter),
                "MeetingAgenda" => await GetAllTyped<NablMeetingAgenda>(filter),
                "MeetingMinutes" => await GetAllTyped<NablMeetingMinutes>(filter),
                "NonConformingWork" => await GetAllTyped<NablNonConformingWork>(filter),
                "NcCorrectiveAction" => await GetAllTyped<NablNcCorrectiveAction>(filter),
                "Retesting" => await GetAllTyped<NablRetesting>(filter),
                "RiskAssessment" => await GetAllTyped<NablRiskAssessment>(filter),
                "DocumentChangeRequest" => await GetAllTyped<NablDocumentChangeRequest>(filter),
                "DocumentReview" => await GetDocumentReviewList(filter),
                "MasterDocument" => await GetAllTyped<NablMasterDocument>(filter),
                "MeasurementUncertainty" => await GetAllTyped<NablMeasurementUncertainty>(filter),
                "PtIlcPlan" => await GetAllTyped<NablPtIlcPlan>(filter),
                "InventoryMaster" => await GetInventoryMasterList(filter),
                _ => throw new ArgumentException($"Unknown form type: {formType}")
            };
        }

        public async Task<object?> GetById(string formType, long id)
        {
            return formType switch
            {
                "JobDescription" => await GetByIdTyped<NablJobDescription>(id),
                "ResponsibilityAuthority" => await GetByIdTyped<NablResponsibilityAuthority>(id),
                "EmployeeCompetence" => await GetByIdTyped<NablEmployeeCompetence>(id),
                "EmployeePerformanceRecord" => await GetByIdTyped<NablEmployeePerformanceRecord>(id),
                "EmployeeAuthorization" => await GetEmployeeAuthorization(id),
                "CompetenceRequirement" => await GetByIdTyped<NablCompetenceRequirement>(id),
                "InductionTraining" => await GetByIdTyped<NablInductionTraining>(id),
                "SkillMatrix" => await GetByIdTyped<NablSkillMatrix>(id),
                "SkillMatrixDecision" => await GetByIdTyped<NablSkillMatrixDecision>(id),
                "TrainingPlan" => await GetByIdTyped<NablTrainingPlan>(id),
                "TrainingAttendance" => await GetByIdTyped<NablTrainingAttendance>(id),
                "TrainingEffectiveness" => await GetByIdTyped<NablTrainingEffectiveness>(id),
                "EnvironmentMonitoring" => await GetByIdTyped<NablEnvironmentMonitoring>(id),
                "QualityControlPlan" => await GetQualityControlPlanByIdTyped(id),
                "TestRequest" => await GetByIdTyped<NablTestRequest>(id),
                "TestMethod" => await GetByIdTyped<NablTestMethod>(id),
                "MethodVerification" => await GetByIdTyped<NablMethodVerification>(id),
                "MethodValidation" => await GetByIdTyped<NablMethodValidation>(id),
                "SampleInwardRegister" => await GetByIdTyped<NablSampleInwardRegister>(id),
                "SampleMusterRegister" => await GetByIdTyped<NablSampleMusterRegister>(id),
                "SampleLabel" => await GetByIdTyped<NablSampleLabel>(id),
                "TechnicalRawData" => await GetByIdTyped<NablTechnicalRawData>(id),
                "TestReport" => await GetByIdTyped<NablTestReport>(id),
                "EquipmentHistory" => await GetByIdTyped<NablEquipmentHistory>(id),
                "CalibrationReview" => await GetByIdTyped<NablCalibrationReview>(id),
                "IntermediateCheck" => await GetByIdTyped<NablIntermediateCheck>(id),
                "ReferenceMaterial" => await GetByIdTyped<NablReferenceMaterial>(id),
                "CrmConsumption" => await GetByReferenceMaterialId(id),
                "SupplierRegistration" => await GetByIdTyped<NablSupplierRegistration>(id),
                "SupplierEvaluation" => await GetByIdTyped<NablSupplierEvaluation>(id),
                "ApprovedSupplier" => await GetByIdTyped<NablApprovedSupplier>(id),
                "SupplierConfidentiality" => await GetByIdTyped<NablSupplierConfidentiality>(id),
                "IncomingMaterial" => await GetByIdTyped<NablIncomingMaterial>(id),
                "ProductInspection" => await GetByIdTyped<NablProductInspection>(id),
                "PurchaseIndent" => await GetByIdTyped<NablPurchaseIndent>(id),
                "PurchaseOrder" => await GetByIdTyped<NablPurchaseOrder>(id),
                "PurchaseMaterialVerification" => await GetByIdTyped<NablPurchaseMaterialVerification>(id),
                "Complaint" => await GetByIdTyped<NablComplaint>(id),
                "CustomerFeedback" => await GetByIdTyped<NablCustomerFeedback>(id),
                "FeedbackAnalysis" => await GetByIdTyped<NablFeedbackAnalysis>(id),
                "AuditPlan" => await GetByAuditPlanIdTyped(id),
                "AuditChecklist" => await GetByAuditChecklistTyped(id),
                "AuditSummary" => await GetByIdTyped<NablAuditSummary>(id),
                "InternalAuditor" => await GetByIdTyped<NablInternalAuditor>(id),
                "MeetingAgenda" => await GetByIdTyped<NablMeetingAgenda>(id),
                "MeetingMinutes" => await GetByIdTyped<NablMeetingMinutes>(id),
                "NonConformingWork" => await GetByIdNonConformingWork(id),
                "NcCorrectiveAction" => await GetByIdTyped<NablNcCorrectiveAction>(id),
                "Retesting" => await GetNablRetesting(id),
                "RiskAssessment" => await GetByIdTyped<NablRiskAssessment>(id),
                "DocumentChangeRequest" => await GetByIdTyped<NablDocumentChangeRequest>(id),
                "DocumentReview" => await GetByDocumentReviewId(id),
                "MasterDocument" => await GetMasterDocumentById(id),
                "MeasurementUncertainty" => await GetByIdTyped<NablMeasurementUncertainty>(id),
                "PtIlcPlan" => await GetByIdTyped<NablPtIlcPlan>(id),
                "InventoryMaster" => await GetInventoryMaster(id),
                _ => throw new ArgumentException($"Unknown form type: {formType}")
            };
        }

        public async Task<long> Add(string formType, object model)
        {
            return formType switch
            {
                "JobDescription" => await AddTyped((NablJobDescription)model),
                "ResponsibilityAuthority" => await AddTyped((NablResponsibilityAuthority)model),
                "EmployeeCompetence" => await AddTyped((NablEmployeeCompetence)model),
                "EmployeePerformanceRecord" => await AddTyped((NablEmployeePerformanceRecord)model),
                "EmployeeAuthorization" => await AddTyped((NablEmployeeAuthorization)model),
                "CompetenceRequirement" => await AddTyped((NablCompetenceRequirement)model),
                "InductionTraining" => await AddTyped((NablInductionTraining)model),
                "SkillMatrix" => await AddTyped((NablSkillMatrix)model),
                "SkillMatrixDecision" => await AddTyped((NablSkillMatrixDecision)model),
                "TrainingPlan" => await AddTyped((NablTrainingPlan)model),
                "TrainingAttendance" => await AddTyped((NablTrainingAttendance)model),
                "TrainingEffectiveness" => await AddTyped((NablTrainingEffectiveness)model),
                "EnvironmentMonitoring" => await AddTyped((NablEnvironmentMonitoring)model),
                "QualityControlPlan" => await AddTyped((NablQualityControlPlan)model),
                "TestRequest" => await AddTyped((NablTestRequest)model),
                "TestMethod" => await AddTyped((NablTestMethod)model),
                "MethodVerification" => await AddTyped((NablMethodVerification)model),
                "MethodValidation" => await AddTyped((NablMethodValidation)model),
                "SampleInwardRegister" => await AddTyped((NablSampleInwardRegister)model),
                "SampleMusterRegister" => await AddTyped((NablSampleMusterRegister)model),
                "SampleLabel" => await AddTyped((NablSampleLabel)model),
                "TechnicalRawData" => await AddTyped((NablTechnicalRawData)model),
                "TestReport" => await AddTyped((NablTestReport)model),
                "EquipmentHistory" => await AddTyped((NablEquipmentHistory)model),
                "CalibrationReview" => await AddTyped((NablCalibrationReview)model),
                "IntermediateCheck" => await AddTyped((NablIntermediateCheck)model),
                "ReferenceMaterial" => await AddTyped((NablReferenceMaterial)model),
                "CrmConsumption" => await AddTyped((NablCrmConsumption)model),
                "SupplierRegistration" => await AddTyped((NablSupplierRegistration)model),
                "SupplierEvaluation" => await AddSupplierEvaluation((NablSupplierEvaluation)model),
                "ApprovedSupplier" => await AddTyped((NablApprovedSupplier)model),
                "SupplierConfidentiality" => await AddTyped((NablSupplierConfidentiality)model),
                "IncomingMaterial" => await AddTyped((NablIncomingMaterial)model),
                "ProductInspection" => await AddTyped((NablProductInspection)model),
                "PurchaseIndent" => await AddTyped((NablPurchaseIndent)model),
                "PurchaseOrder" => await AddTyped((NablPurchaseOrder)model),
                "PurchaseMaterialVerification" => await AddTyped((NablPurchaseMaterialVerification)model),
                "Complaint" => await AddTyped((NablComplaint)model),
                "CustomerFeedback" => await AddTyped((NablCustomerFeedback)model),
                "FeedbackAnalysis" => await AddTyped((NablFeedbackAnalysis)model),
                "AuditPlan" => await AddTyped((NablAuditPlan)model),
                "AuditChecklist" => await AddTyped((NablAuditChecklist)model),
                "AuditSummary" => await AddTyped((NablAuditSummary)model),
                "InternalAuditor" => await AddTyped((NablInternalAuditor)model),
                "MeetingAgenda" => await AddTyped((NablMeetingAgenda)model),
                "MeetingMinutes" => await AddTyped((NablMeetingMinutes)model),
                "NonConformingWork" => await AddNonConformingWork((NablNonConformingWork)model),
                "NcCorrectiveAction" => await AddTyped((NablNcCorrectiveAction)model),
                "Retesting" => await AddTyped((NablRetesting)model),
                "RiskAssessment" => await AddTyped((NablRiskAssessment)model),
                "DocumentChangeRequest" => await AddTyped((NablDocumentChangeRequest)model),
                "DocumentReview" => await AddTyped((NablDocumentReview)model),
                "MasterDocument" => await AddTyped((NablMasterDocument)model),
                "MeasurementUncertainty" => await AddTyped((NablMeasurementUncertainty)model),
                "PtIlcPlan" => await AddTyped((NablPtIlcPlan)model),
                "InventoryMaster" => await AddInventoryMaster((InventoryManagement)model),
                _ => throw new ArgumentException($"Unknown form type: {formType}")
            };
        }

        public async Task Update(string formType, object model)
        {
            switch (formType)
            {
                case "JobDescription":
                    await UpdateTyped((NablJobDescription)model);
                    break;
                case "ResponsibilityAuthority":
                    await UpdateTyped((NablResponsibilityAuthority)model);
                    break;
                case "EmployeeCompetence":
                    await UpdateTyped((NablEmployeeCompetence)model);
                    break;
                case "EmployeePerformanceRecord":
                    await UpdateTyped((NablEmployeePerformanceRecord)model);
                    break;
                case "EmployeeAuthorization":
                    await UpdateTyped((NablEmployeeAuthorization)model);
                    break;
                case "CompetenceRequirement":
                    await UpdateTyped((NablCompetenceRequirement)model);
                    break;
                case "InductionTraining":
                    await UpdateTyped((NablInductionTraining)model);
                    break;
                case "SkillMatrix":
                    await UpdateTyped((NablSkillMatrix)model);
                    break;
                case "SkillMatrixDecision":
                    await UpdateTyped((NablSkillMatrixDecision)model);
                    break;
                case "TrainingPlan":
                    await UpdateTyped((NablTrainingPlan)model);
                    break;
                case "TrainingAttendance":
                    await UpdateTyped((NablTrainingAttendance)model);
                    break;
                case "TrainingEffectiveness":
                    await UpdateTyped((NablTrainingEffectiveness)model);
                    break;
                case "EnvironmentMonitoring":
                    await UpdateTyped((NablEnvironmentMonitoring)model);
                    break;
                case "QualityControlPlan":
                    await UpdateTyped((NablQualityControlPlan)model);
                    break;
                case "TestRequest":
                    await UpdateTyped((NablTestRequest)model);
                    break;
                case "TestMethod":
                    await UpdateTyped((NablTestMethod)model);
                    break;
                case "MethodVerification":
                    await UpdateTyped((NablMethodVerification)model);
                    break;
                case "MethodValidation":
                    await UpdateTyped((NablMethodValidation)model);
                    break;
                case "SampleInwardRegister":
                    await UpdateTyped((NablSampleInwardRegister)model);
                    break;
                case "SampleMusterRegister":
                    await UpdateTyped((NablSampleMusterRegister)model);
                    break;
                case "SampleLabel":
                    await UpdateTyped((NablSampleLabel)model);
                    break;
                case "TechnicalRawData":
                    await UpdateTyped((NablTechnicalRawData)model);
                    break;
                case "TestReport":
                    await UpdateTyped((NablTestReport)model);
                    break;
                case "EquipmentHistory":
                    await UpdateTyped((NablEquipmentHistory)model);
                    break;
                case "CalibrationReview":
                    await UpdateTyped((NablCalibrationReview)model);
                    break;
                case "IntermediateCheck":
                    await UpdateTyped((NablIntermediateCheck)model);
                    break;
                case "ReferenceMaterial":
                    await UpdateTyped((NablReferenceMaterial)model);
                    break;
                case "CrmConsumption":
                    await UpdateTyped((NablCrmConsumption)model);
                    break;
                case "SupplierRegistration":
                    await UpdateTyped((NablSupplierRegistration)model);
                    break;
                case "SupplierEvaluation":
                    await UpdateSupplierEvaluation((NablSupplierEvaluation)model);
                    break;
                case "ApprovedSupplier":
                    await UpdateTyped((NablApprovedSupplier)model);
                    break;
                case "SupplierConfidentiality":
                    await UpdateTyped((NablSupplierConfidentiality)model);
                    break;
                case "IncomingMaterial":
                    await UpdateTyped((NablIncomingMaterial)model);
                    break;
                case "ProductInspection":
                    await UpdateTyped((NablProductInspection)model);
                    break;
                case "PurchaseIndent":
                    await UpdateTyped((NablPurchaseIndent)model);
                    break;
                case "PurchaseOrder":
                    await UpdateTyped((NablPurchaseOrder)model);
                    break;
                case "PurchaseMaterialVerification":
                    await UpdateTyped((NablPurchaseMaterialVerification)model);
                    break;
                case "Complaint":
                    await UpdateTyped((NablComplaint)model);
                    break;
                case "CustomerFeedback":
                    await UpdateTyped((NablCustomerFeedback)model);
                    break;
                case "FeedbackAnalysis":
                    await UpdateTyped((NablFeedbackAnalysis)model);
                    break;
                case "AuditPlan":
                    await UpdateTyped((NablAuditPlan)model);
                    break;
                case "AuditChecklist":
                    await UpdateTyped((NablAuditChecklist)model);
                    break;
                case "AuditSummary":
                    await UpdateTyped((NablAuditSummary)model);
                    break;
                case "InternalAuditor":
                    await UpdateTyped((NablInternalAuditor)model);
                    break;
                case "MeetingAgenda":
                    await UpdateTyped((NablMeetingAgenda)model);
                    break;
                case "MeetingMinutes":
                    await UpdateTyped((NablMeetingMinutes)model);
                    break;
                case "NonConformingWork":
                    await UpdateNonConformingWork((NablNonConformingWork)model);
                    break;
                case "NcCorrectiveAction":
                    await UpdateTyped((NablNcCorrectiveAction)model);
                    break;
                case "Retesting":
                    await UpdateTyped((NablRetesting)model);
                    break;
                case "RiskAssessment":
                    await UpdateTyped((NablRiskAssessment)model);
                    break;
                case "DocumentChangeRequest":
                    await UpdateTyped((NablDocumentChangeRequest)model);
                    break;
                case "DocumentReview":
                    await UpdateTyped((NablDocumentReview)model);
                    break;
                case "MasterDocument":
                    await UpdateTyped((NablMasterDocument)model);
                    break;
                case "MeasurementUncertainty":
                    await UpdateTyped((NablMeasurementUncertainty)model);
                    break;
                case "PtIlcPlan":
                    await UpdateTyped((NablPtIlcPlan)model);
                    break;
                case "InventoryMaster":
                    await UpdateInventoryMaster((InventoryManagement)model);
                    break;
                default:
                    throw new ArgumentException($"Unknown form type: {formType}");
            }
        }

        public async Task Delete(string formType, long id)
        {
            switch (formType)
            {
                case "JobDescription":
                    await DeleteTyped<NablJobDescription>(id);
                    break;
                case "ResponsibilityAuthority":
                    await DeleteTyped<NablResponsibilityAuthority>(id);
                    break;
                case "EmployeeCompetence":
                    await DeleteTyped<NablEmployeeCompetence>(id);
                    break;
                case "EmployeePerformanceRecord":
                    await DeleteTyped<NablEmployeePerformanceRecord>(id);
                    break;
                case "EmployeeAuthorization":
                    await DeleteTyped<NablEmployeeAuthorization>(id);
                    break;
                case "CompetenceRequirement":
                    await DeleteTyped<NablCompetenceRequirement>(id);
                    break;
                case "InductionTraining":
                    await DeleteTyped<NablInductionTraining>(id);
                    break;
                case "SkillMatrix":
                    await DeleteTyped<NablSkillMatrix>(id);
                    break;
                case "SkillMatrixDecision":
                    await DeleteTyped<NablSkillMatrixDecision>(id);
                    break;
                case "TrainingPlan":
                    await DeleteTyped<NablTrainingPlan>(id);
                    break;
                case "TrainingAttendance":
                    await DeleteTyped<NablTrainingAttendance>(id);
                    break;
                case "TrainingEffectiveness":
                    await DeleteTyped<NablTrainingEffectiveness>(id);
                    break;
                case "EnvironmentMonitoring":
                    await DeleteTyped<NablEnvironmentMonitoring>(id);
                    break;
                case "QualityControlPlan":
                    await DeleteTyped<NablQualityControlPlan>(id);
                    break;
                case "TestRequest":
                    await DeleteTyped<NablTestRequest>(id);
                    break;
                case "TestMethod":
                    await DeleteTyped<NablTestMethod>(id);
                    break;
                case "MethodVerification":
                    await DeleteTyped<NablMethodVerification>(id);
                    break;
                case "MethodValidation":
                    await DeleteTyped<NablMethodValidation>(id);
                    break;
                case "SampleInwardRegister":
                    await DeleteTyped<NablSampleInwardRegister>(id);
                    break;
                case "SampleMusterRegister":
                    await DeleteTyped<NablSampleMusterRegister>(id);
                    break;
                case "SampleLabel":
                    await DeleteTyped<NablSampleLabel>(id);
                    break;
                case "TechnicalRawData":
                    await DeleteTyped<NablTechnicalRawData>(id);
                    break;
                case "TestReport":
                    await DeleteTyped<NablTestReport>(id);
                    break;
                case "EquipmentHistory":
                    await DeleteTyped<NablEquipmentHistory>(id);
                    break;
                case "CalibrationReview":
                    await DeleteTyped<NablCalibrationReview>(id);
                    break;
                case "IntermediateCheck":
                    await DeleteTyped<NablIntermediateCheck>(id);
                    break;
                case "ReferenceMaterial":
                    await DeleteTyped<NablReferenceMaterial>(id);
                    break;
                case "CrmConsumption":
                    await DeleteTyped<NablCrmConsumption>(id);
                    break;
                case "SupplierRegistration":
                    await DeleteTyped<NablSupplierRegistration>(id);
                    break;
                case "SupplierEvaluation":
                    await DeleteTyped<NablSupplierEvaluation>(id);
                    break;
                case "ApprovedSupplier":
                    await DeleteTyped<NablApprovedSupplier>(id);
                    break;
                case "SupplierConfidentiality":
                    await DeleteTyped<NablSupplierConfidentiality>(id);
                    break;
                case "IncomingMaterial":
                    await DeleteTyped<NablIncomingMaterial>(id);
                    break;
                case "ProductInspection":
                    await DeleteTyped<NablProductInspection>(id);
                    break;
                case "PurchaseIndent":
                    await DeleteTyped<NablPurchaseIndent>(id);
                    break;
                case "PurchaseOrder":
                    await DeleteTyped<NablPurchaseOrder>(id);
                    break;
                case "PurchaseMaterialVerification":
                    await DeleteTyped<NablPurchaseMaterialVerification>(id);
                    break;
                case "Complaint":
                    await DeleteTyped<NablComplaint>(id);
                    break;
                case "CustomerFeedback":
                    await DeleteTyped<NablCustomerFeedback>(id);
                    break;
                case "FeedbackAnalysis":
                    await DeleteTyped<NablFeedbackAnalysis>(id);
                    break;
                case "AuditPlan":
                    await DeleteTyped<NablAuditPlan>(id);
                    break;
                case "AuditChecklist":
                    await DeleteTyped<NablAuditChecklist>(id);
                    break;
                case "AuditSummary":
                    await DeleteTyped<NablAuditSummary>(id);
                    break;
                case "InternalAuditor":
                    await DeleteTyped<NablInternalAuditor>(id);
                    break;
                case "MeetingAgenda":
                    await DeleteTyped<NablMeetingAgenda>(id);
                    break;
                case "MeetingMinutes":
                    await DeleteTyped<NablMeetingMinutes>(id);
                    break;
                case "NonConformingWork":
                    await DeleteTyped<NablNonConformingWork>(id);
                    break;
                case "NcCorrectiveAction":
                    await DeleteTyped<NablNcCorrectiveAction>(id);
                    break;
                case "Retesting":
                    await DeleteTyped<NablRetesting>(id);
                    break;
                case "RiskAssessment":
                    await DeleteTyped<NablRiskAssessment>(id);
                    break;
                case "DocumentChangeRequest":
                    await DeleteTyped<NablDocumentChangeRequest>(id);
                    break;
                case "DocumentReview":
                    await DeleteTyped<NablDocumentReview>(id);
                    break;
                case "MasterDocument":
                    await DeleteTyped<NablMasterDocument>(id);
                    break;
                case "MeasurementUncertainty":
                    await DeleteTyped<NablMeasurementUncertainty>(id);
                    break;
                case "PtIlcPlan":
                    await DeleteTyped<NablPtIlcPlan>(id);
                    break;
                case "InventoryMaster":
                    await DeleteInventoryMaster<InventoryManagement>(id);
                    break;
                default:
                    throw new ArgumentException($"Unknown form type: {formType}");
            }
        }

        // ─── Generic typed methods ───────────────────────────────────────

        private async Task<PagedResponse<object>> GetAllTyped<T>(PageFilter filter)
            where T : NablFormBase
        {
            var query = _context.Set<T>()
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            // Apply column filters using existing FilterHelper
            query = query.AsQueryable().ApplyFilters(filter.Filter);

            // Global search: reflection-based OR-chain across all string properties of T
            // (covers inherited NablFormBase fields + entity-specific fields, excludes CompanyCode/RejectionRemarks)
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                var props = GetSearchableStringProps(typeof(T));
                if (props.Length > 0)
                {
                    // Rely on SQL Server CI collation — no LOWER() wrap (keeps columns sargable).
                    var predicate = string.Join(" || ",
                        props.Select(p => $"({p} != null && {p}.Contains(@0))"));
                    query = query.AsQueryable().Where(predicate, search).OfType<T>();
                }
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }

            return await query.Cast<object>().ToPagedAsync(filter);
        }

        private async Task<T?> GetByIdTyped<T>(long id) where T : NablFormBase
        {
            return await _context.Set<T>()
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        private async Task<NablEmployeeAuthorization> GetEmployeeAuthorization(long id)
        {
            var data = await _context.NablEmployeeAuthorizations.Include(c => c.EmployeeEquipmentAuth).Include(c => c.LabTestAuth).Include(c => c.TestMethodAuth).FirstOrDefaultAsync(c => c.ID == id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
            return data;
        }
        private async Task<InventoryManagement> GetInventoryMaster(long id)
        {
            var data = await _context.InventoryManagements.FirstOrDefaultAsync(c => c.ID == id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
            return data;
        }
        private async Task<NablRetesting> GetNablRetesting(long id)
        {
            var data = await _context.NablRetestings.Include(c => c.InitialTestingLogs).Include(c => c.RetestingLogs).FirstOrDefaultAsync(c => c.ID == id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
            return data;
        }
        private async Task<PagedResponse<object>> GetEquipmentAuthorizationList(PageFilter filter)
        {
            var _query = _context.NablEmployeeAuthorizations.Include(x => x.EmployeeEquipmentAuth)
                   .Include(c => c.LabTestAuth)
                   .Include(c => c.TestMethodAuth).Where(c => c.IsActive)
                   .AsQueryable()
                   .ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.PersonnelName!= null && x.PersonnelName.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            var data = await _query.Select(x => new
            {
                x.DocumentNo,
                x.ID,
                x.PersonnelName,
                x.Date,
                x.DepartmentName,
                UID = x.EmployeeEquipmentAuth.Select(e => e.UID).ToList(),
                Equipments = x.EmployeeEquipmentAuth.Select(e => e.EquipmentName).ToList(),
                TestMethodAuth = x.TestMethodAuth.Select(c => c.TestMethodName).ToList(),
                LabTests = x.LabTestAuth.Select(l => l.LabTestName).ToList()
            }).Cast<object>().ToPagedAsync(filter);

            return data;

        }

        private async Task<long> AddTyped<T>(T model) where T : NablFormBase
        {
            await _context.Set<T>().AddAsync(model);
            await _context.SaveChangesAsync();
            return model.ID;
        }

        private async Task<long> AddSupplierEvaluation(NablSupplierEvaluation obj)
        {
            if (obj.ToRemoved == true)
            {
                var supplier = await _context.NablApprovedSuppliers.FirstOrDefaultAsync(c => c.SupplierName.Contains(obj.SupplierName));
                supplier.IsPresentStatus = false;
                supplier.EnlistmentDate = null;
                _context.NablApprovedSuppliers.Update(supplier);
                await _context.SaveChangesAsync();
                obj.PresentStatus = "Delisted";
            }
            await _context.NablSupplierEvaluations.AddAsync(obj);
            await _context.SaveChangesAsync();
            return obj.ID;
        }

        private async Task UpdateSupplierEvaluation(NablSupplierEvaluation obj)
        {
            if (obj.ToRemoved == true)
            {
                var supplier = await _context.NablApprovedSuppliers.FirstOrDefaultAsync(c => c.SupplierName.Contains(obj.SupplierName));
                supplier.IsPresentStatus = false;
                supplier.EnlistmentDate = null;
                _context.NablApprovedSuppliers.Update(supplier);
                await _context.SaveChangesAsync();
                obj.PresentStatus = "Delisted";
            }
            _context.NablSupplierEvaluations.Update(obj);
            await _context.SaveChangesAsync();
        }
        private async Task UpdateTyped<T>(T model) where T : NablFormBase
        {
            _context.Set<T>().Update(model);
            await _context.SaveChangesAsync();
        }

        private async Task DeleteTyped<T>(long id) where T : NablFormBase
        {
            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (entity != null)
            {
                entity.IsActive = false;
                entity.ModifiedOn = DateTime.UtcNow;
                entity.ModifiedBy = loggedInUser.EmployeeID;
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<DropdwonSelector>> GetTraningPlanDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.NablTrainingPlans
                         where a.IsActive
                         select a;


            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.TrainingTopic));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.TrainingTopic.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.TrainingTopic
                })
                .ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> Roomdropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.LabRooms
                         where a.IsActive
                         select a;


            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.Name));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.Name.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.Name
                })
                .ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> Supplierlist(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0)
                pageNo = 0;


            var query = _context.NablSupplierRegistrations
                .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.SupplierName));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    query = query.Where(x => x.SupplierName.Contains(search));
                }
            }


            var supplierList = await query.ToListAsync();

            supplierList = supplierList
                .Where(x =>
                {
                    if (string.IsNullOrWhiteSpace(x.DocumentsSubmittedJson))
                        return false;

                    try
                    {
                        var documents = JsonConvert.DeserializeObject<DocumentsSubmitted>(x.DocumentsSubmittedJson);

                        return documents?.SupplierApproved == true;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .ToList();

            var skip = pageNo * pageSize;

            var data = supplierList
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.SupplierName,
                    AdditionalValues = new Dictionary<string, object>
                    {
                { "ContactPerson", x.ContactPerson ?? "" },
                { "MobileNo", x.MobileNo ?? "" },
                { "Email", x.Email ?? "" },
                { "RegisterNo", x.RegisterNo ?? "" },
                {"GSTNo",x.GstNo?? "" },
                {"Address",x.Address ?? "" },
                {"ProductsServicesOffered",x.ProductsServicesOffered ?? "" }
                    }
                })
                .ToList();
            return data;
        }

        public async Task<List<DropdwonSelector>> AllSupplierlist(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0)
                pageNo = 0;

            var query = _context.NablApprovedSuppliers
                .Where(x =>
                    x.IsActive &&
                    !string.IsNullOrWhiteSpace(x.SupplierName) &&
                    x.IsBlacklisted == false && x.IsPresentStatus == true); // Enlisted

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    query = query.Where(x => x.SupplierName.Contains(search));
                }
            }

            var approvedSuppliers = await query
                .OrderBy(x => x.SupplierName)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var supplierNames = approvedSuppliers
                .Select(x => x.SupplierName.Trim())
                .ToList();

            var registrationList = await _context.NablSupplierRegistrations
                .Where(x =>
                    x.IsActive &&
                    !string.IsNullOrWhiteSpace(x.SupplierName) &&
                    supplierNames.Contains(x.SupplierName.Trim()))
                .ToListAsync();

            var data = approvedSuppliers.Select(x =>
            {
                var reg = registrationList.FirstOrDefault(r =>
                    r.SupplierName.Trim().ToLower() == x.SupplierName.Trim().ToLower());

                return new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.SupplierName,
                    AdditionalValues = new Dictionary<string, object>
                {
                { "RegisterNo", x.RegisterNo ?? reg?.RegisterNo ?? "" },
                { "ContactPerson", x.ContactPerson ?? reg?.ContactPerson ?? "" },
                { "MobileNo", x.MobileNo ?? reg?.MobileNo ?? "" },
                { "Email", reg?.Email ?? "" },
                { "GSTNo", x.GstNo ?? reg?.GstNo ?? "" },
                { "Address", x.Address ?? reg?.Address ?? "" },

                { "PresentStatus", x.IsPresentStatus == true ? "Enlisted" : "Delisted" },
                { "IsBlacklisted", x.IsBlacklisted },

                { "NatureOfBusiness", reg?.NatureOfBusiness ?? "" },
                { "ProductsServicesOffered", reg?.ProductsServicesOffered ?? "" },
                { "ServiceProvider", x?.ServiceProviderName ?? "" }
                }
                };
            }).ToList();

            return data;
        }
        public async Task<List<DropdwonSelector>> IndentNoList(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.NablPurchaseIndents
                         where a.IsActive
                         select a;


            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.PINo));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.PINo.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = (x.PINo ?? "") + "/" + (x.IndentorName ?? ""),
                    AdditionalValues = new Dictionary<string, object>
                    {
                        {"Qaulity",x.Quantity ?? 0 },
                        {"IndetorName",x.IndentorName ?? "" },
                        {"ReferenceIndentorName",x.PINo ?? "" + "/" + (x.IndentorName ?? "") }
                    }
                })
                .ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> ApprovedSupplierlist(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0)
                pageNo = 0;


            var query = _context.NablApprovedSuppliers
                .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.SupplierName));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    query = query.Where(x => x.SupplierName.Contains(search));
                }
            }
            var supplierList = await query.ToListAsync();
            supplierList = supplierList.Where(x => x.IsBlacklisted == false && x.IsPresentStatus == true).ToList();

            var skip = pageNo * pageSize;

            var data = supplierList
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.SupplierName,
                    AdditionalValues = new Dictionary<string, object>
                    {
                { "ContactPerson", x.ContactPerson ?? "" },
                { "MobileNo", x.MobileNo ?? "" },
                { "Email", x.Email ?? "" },
                { "RegisterNo", x.RegisterNo ?? "" },
                {"GSTNo",x.GstNo?? "" },
                {"Address",x.Address ?? "" }
                    }
                })
                .ToList();
            return data;
        }
        public async Task<List<DropdwonSelector>> Alltestmethodlist(string formType, string? searchTerm, int pageNo = 0, int pageSize = 20)
        {


            if (pageNo < 0) pageNo = 0;
            switch (formType)
            {
                case "MethodVerification":
                    var query = _context.NablTestMethods
                        .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.TestMethodJson));

                    var allMethods = new List<TestMethodEntryDto>();
                    foreach (var method in await query.ToListAsync())
                    {

                        var methods = System.Text.Json.JsonSerializer.Deserialize<List<TestMethodEntryDto>>(
                    method.TestMethodJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                        if (methods != null && methods.Any())
                        {
                            allMethods.AddRange(methods);
                        }
                        allMethods = allMethods.Where(x => x.Status == "Active").ToList();
                    }


                    var skip = pageNo * pageSize;


                    var data = allMethods
                        .Skip(skip)
                        .Take(pageSize)
                        .Select((x, index) => new DropdwonSelector
                        {
                            Id = skip + index +1,
                            Name = x.SpecificationCode,
                            AdditionalValues = new Dictionary<string, object>
                        {
                { "MethodName", x.MethodName ?? "" },
                { "SpecificationCode", x.SpecificationCode ?? "" },
                { "ReferenceStandard", x.ReferenceStandard ?? "" },
                { "RevisionNo", x.RevisionNo ?? "" },
                { "EffectiveDate", x.EffectiveDate },
                { "Status", x.Status ?? "" },
                        }
                        })
                        .ToList();
                    return data;

                case "MethodValidation":
                    var _query = await _context.NablMethodVerifications.Where(x => x.IsActive && x.VerificationStatus == "Verified").ToListAsync();
                    //_query = _query.Where(x => !string.IsNullOrEmpty(x.VerificationStatus) && x.VerificationStatus == "Verified");

                    var _skip = pageNo * pageSize;


                    var data1 = _query
                        .Skip(_skip)
                        .Take(pageSize)
                        .Select((x, index) => new DropdwonSelector
                        {
                            Id = x.ID,
                            Name = x.TestMethodCode
                        })
                        .ToList();
                    return data1;

            }
            return null;
        }
        public async Task<List<DropdwonSelector>> PlanNoDetailslist(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {

            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.NablProductInspections
                         where a.IsActive
                         select a;


            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.PlanNo));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.PlanNo.Contains(search));
                }
            }
            var skip = pageNo * pageSize;

            var data = _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.PlanNo ?? "",
                    AdditionalValues = new Dictionary<string, object>
                    {
                { "ProductName", x.ProductName ?? "" },
                { "ProductCode", x.ProductCode ?? "" },
                { "Category", x.Category ?? "" },
                { "InspectionStage", x.InspectionStage ?? "" },
                {"Risklevel",x.Risklevel?? "" },
                {"PlanNoName",x.PlanNo?? "" },
                {"InspectionResultsJson",x.InspectionResultsJson ?? "" }
                    }
                })
                .ToList();
            return data;
        }
        public async Task<List<DropdwonSelector>> PONoListDetailslist(string? formType, string? searchTerm, int pageNo = 0, int pageSize = 20)
        {

            if (pageNo < 0) pageNo = 0;

            switch (formType)
            {
                case "IncomingMaterial":

                    var _query = from a in _context.NablPurchaseOrders
                                 where a.IsActive
                                 select a;


                    _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.PONo));

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                        {
                            _query = _query.Where(x => x.ID == exactId);
                        }
                        else
                        {
                            var search = searchTerm.Trim();
                            _query = _query.Where(x => x.PONo.Contains(search));
                        }
                    }
                    var skip = pageNo * pageSize;

                    var data = _query
                        .Skip(skip)
                        .Take(pageSize)
                        .Select(x => new DropdwonSelector
                        {
                            Id = x.ID,
                            Name = x.PONo ?? "",
                            AdditionalValues = new Dictionary<string, object>
                            {
                            { "ReferenceIndentNo", x.ReferenceIndentName ?? "" },
                            { "SupplierName", x.SupplierName ?? "" },
                            { "Email", x.Email?? "" },
                            { "PhoneNo", x.PhoneNo?? "" },
                            { "GSTNo", x.GSTNo?? "" },
                            { "OrderType", x.OrderType?? "" },
                            {"SupplierAddress" , x.SupplierAddress ?? "" },
                            {"ItemsJson",x.ItemsJson ?? "" },
                            {"PurchaseOrderNo",x.PONo?? "" }
                            }
                        })
                        .ToList();
                    return data;

                case "PurchaseMaterialVerification":
                    var query =
                        from im in _context.NablIncomingMaterials
                        join po in _context.NablPurchaseOrders
                            on im.PurchaseOrderNo equals po.PONo
                        where im.IsActive
                              && po.IsActive
                              && !string.IsNullOrEmpty(im.PurchaseOrderNo)
                        select new
                        {
                            Incoming = im,
                            PurchaseOrder = po
                        };

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                        {
                            query = query.Where(x => x.Incoming.ID == exactId);
                        }
                        else
                        {
                            var search = searchTerm.Trim();

                            query = query.Where(x =>
                                x.Incoming.PurchaseOrderNo.Contains(search) ||
                                x.PurchaseOrder.PONo.Contains(search) ||
                                x.PurchaseOrder.SupplierName.Contains(search)
                            );
                        }
                    }

                    var skipped = pageNo * pageSize;

                    var obj = await query
                        .OrderByDescending(x => x.Incoming.ID)
                        .Skip(skipped)
                        .Take(pageSize)
                        .Select(x => new DropdwonSelector
                        {
                            Id = x.Incoming.ID,


                            Name = x.Incoming.PurchaseOrderNo ?? "",

                            AdditionalValues = new Dictionary<string, object>
                            {
                            // From Purchase Order table
                            { "Deviations", x.Incoming.Deviations?? "" },
                            { "CorrectiveActions", x.Incoming.CorrectiveActions?? "" },
                            { "SupplierName", x.PurchaseOrder.SupplierName ?? "" },
                            { "Email", x.PurchaseOrder.Email ?? "" },
                            { "PhoneNo", x.PurchaseOrder.PhoneNo ?? "" },
                            { "GSTNo", x.PurchaseOrder.GSTNo ?? "" },
                            { "OrderType", x.PurchaseOrder.OrderType ?? "" },
                            { "SupplierAddress", x.PurchaseOrder.SupplierAddress ?? "" },
                            { "PurchaseOrderNo", x.PurchaseOrder.PONo ?? "" },
                            { "PODate", x.PurchaseOrder.PODate },
                            { "ItemsJson", x.Incoming.ItemsParametersJson ?? "" },
                            { "InspectionBy", x.Incoming.InspectionBy ?? "" },
                            { "ReceivedBy", x.Incoming.ReceivedBy ?? "" }
                            }
                        }).ToListAsync();
                    return obj;

            }
            return null;

        }
        public async Task<List<CombinedPoItemDto>> ReceivedItemsDetails(string poNo, string supplierName)
        {

            var po = await _context.NablPurchaseOrders
                .FirstOrDefaultAsync(x =>
                    x.IsActive &&
                    x.PONo == poNo &&
                    x.SupplierName == supplierName);
            var incoming = await _context.NablIncomingMaterials
                .FirstOrDefaultAsync(x =>
                    x.IsActive &&
                    x.PurchaseOrderNo == poNo &&
                    x.SupplierName == supplierName);
            var verification = await _context.NablPurchaseMaterialVerifications
                .FirstOrDefaultAsync(x =>
                    x.IsActive &&
                    x.PurchaseOrderNo == poNo &&
                    x.SupplierName == supplierName);

            List<Items> poItems = new();

            if (!string.IsNullOrWhiteSpace(po?.ItemsJson))
            {
                poItems = JsonConvert.DeserializeObject<List<Items>>(po.ItemsJson)
                          ?? new List<Items>();
            }
            List<ItemsParameters> incomingItems = new();

            if (!string.IsNullOrWhiteSpace(incoming?.ItemsParametersJson))
            {
                incomingItems = JsonConvert.DeserializeObject<List<ItemsParameters>>
                                (incoming.ItemsParametersJson)
                                ?? new List<ItemsParameters>();
            }

            // Verification item list
            List<DescriptionParameters> verificationItems = new();

            if (!string.IsNullOrWhiteSpace(verification?.ItemsVerificationJson))
            {
                verificationItems = JsonConvert.DeserializeObject<List<DescriptionParameters>>(verification.ItemsVerificationJson)
                                    ?? new List<DescriptionParameters>();
            }

            var result = new List<CombinedPoItemDto>();

            foreach (var poItem in poItems)
            {
                var incomingItem = incomingItems.FirstOrDefault(x =>
                    x.MaterialName == poItem.Description);

                var verificationItem = verificationItems.FirstOrDefault(x =>
                    x.MaterialName == poItem.Description);
                var row = new CombinedPoItemDto
                {
                    ItemName = poItem.Description ?? "",
                    OrderedQty = incomingItem.OrderedQty ?? 0,
                    UnitPrice = poItem.UnitPrice ?? 0,
                    Amount = poItem.TotalAmount,
                    ReceivedQty = incomingItem?.ReceviceQty,
                    BatchNo = incomingItem?.BatchNo ?? "",
                    LotNo = incomingItem?.LotNo ?? "",
                    InvoiceNo = incomingItem?.InvoiceNo ?? "",
                    ApprovedQty = verificationItem?.ApprovedQty,
                    RejectedQty = verificationItem?.RejectedQty,
                    VerificationStatus = verificationItem?.InspectionQtyStatus ?? "",
                    VerificationDone= verificationItem?.VerificationDone ?? "",
                    VerificationDetails = verificationItem?.VerificationDetails ?? ""
                };

                result.Add(row);
            }

            return result;
        }
        public async Task<List<InspectionParameters>> InspectionPlanDetails(string inspectionPlanNo)
        {

            var planNo = await _context.NablProductInspections
                .FirstOrDefaultAsync(x =>
                    x.IsActive &&
                    x.PlanNo == inspectionPlanNo);

            List<InspectionParameters> planDetails = new();

            if (!string.IsNullOrWhiteSpace(planNo?.InspectionResultsJson))
            {
                planDetails = JsonConvert.DeserializeObject<List<InspectionParameters>>(planNo.InspectionResultsJson)
                          ?? new List<InspectionParameters>();
            }


            // Verification item list

            var result = new List<InspectionParameters>();

            foreach (var inspectionPlanDetails in planDetails)
            {
                var row = new InspectionParameters
                {
                    ParameterName = inspectionPlanDetails.ParameterName ?? "",
                    Requirement = inspectionPlanDetails.Requirement ?? "",
                    ReferenceStandard = inspectionPlanDetails.ReferenceStandard ?? "",
                    MethodOfCheck = inspectionPlanDetails.MethodOfCheck,
                    Frequency = inspectionPlanDetails.Frequency ?? "",
                    AcceptanceCriteria = inspectionPlanDetails.AcceptanceCriteria ?? "",

                };

                result.Add(row);
            }

            return result;
        }
        public async Task<NablPurchaseIndentDto> IndentDetails(string indentNo)
        {

            var indentDetails = await _context.NablPurchaseIndents
                .Where(x =>
                    x.IsActive &&
                    x.PINo == indentNo).Select(c => new NablPurchaseIndentDto
                    {
                        Description = c.Justification,
                        UnitOfMeasure = c.UnitOfMeasure,
                        Priority = c.Priority,
                        TechnicalSpecification = c.TechnicalSpecification,
                        ExpectedDate = c.ExpectedDate,
                        IndentorName = c.IndentorName,
                        PurchaseIndentNo = c.PINo,
                        Quantity = c.Quantity
                    }).FirstOrDefaultAsync();

            return indentDetails;
        }
        public async Task<NablTestMethodValidationDto> TestMethodDetails(string testmethodCode)
        {

            var data = await _context.NablMethodVerifications
                .Where(x =>
                    x.IsActive &&
                    x.TestMethodCode == testmethodCode).Select(c => new
                    {
                        c.TestMethodName,
                        c.RevIssue,
                        c.ReferenceStandard,
                        c.Humidity,
                        c.Temperature,
                        c.EquipmentId,
                        c.EquipmentName,
                        c.CrmParametersJson,
                        c.VerificationDate,
                        c.VerifiedBy,
                    }).FirstOrDefaultAsync();
            var crmlist = new List<CrmParameters>();
            if (!string.IsNullOrEmpty(data.CrmParametersJson))
            {
                crmlist = System.Text.Json.JsonSerializer.Deserialize<List<CrmParameters>>(data.CrmParametersJson) ?? new List<CrmParameters>();
            }
            var res = new NablTestMethodValidationDto
            {
                TestMethodName = data.TestMethodName,
                RevIssue = data.RevIssue,
                ReferenceStandard = data.ReferenceStandard,
                Humidity = data.Humidity,
                Temperature = data.Temperature,
                EquipmentId = data.EquipmentId,
                EquipmentName = data.EquipmentName,
                VerifiedBy = data.VerifiedBy,
                VerificationDate = data.VerificationDate,
                CrmMaterialParameters = crmlist
            };
            return res;
        }
        public async Task<List<Items>> PoitemsDetails(string poNo, string supplierName)
        {

            var po = await _context.NablPurchaseOrders
                .FirstOrDefaultAsync(x =>
                    x.IsActive &&
                    x.PONo == poNo &&
                    x.SupplierName == supplierName);

            List<Items> poItems = new();

            if (!string.IsNullOrWhiteSpace(po?.ItemsJson))
            {
                poItems = JsonConvert.DeserializeObject<List<Items>>(po.ItemsJson)
                          ?? new List<Items>();
            }

            var result = new List<Items>();

            foreach (var poItem in poItems)
            {

                var row = new Items
                {
                    Description = poItem.Description ?? "",
                    Quantity = poItem.Quantity?? 0,
                    UnitPrice = poItem.UnitPrice ?? 0,
                    TotalAmount = poItem.TotalAmount,
                    UnitOfMeasure = poItem.UnitOfMeasure,
                };

                result.Add(row);
            }

            return result;
        }
        public async Task<SupplierEvaluationDetailsDto> SupplierEvaluationDetails(string? supplierName, DateTime? fromDate, DateTime? toDate)
        {
            var poQuery = _context.NablPurchaseOrders
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(supplierName))
            {
                poQuery = poQuery
                    .Where(x => x.SupplierName == supplierName);
            }

            if (fromDate != null && toDate != null)
            {
                poQuery = poQuery
                    .Where(x =>
                        x.PODate >= fromDate &&
                        x.PODate <= toDate);
            }

            var purchaseOrders = await poQuery
                .OrderBy(x => x.PODate)
                .Select(x => new NablPurchaseOrderDto
                {
                    Id = x.ID,
                    PONo = x.PONo ?? "",
                    PODate = x.PODate,
                    DeliveryDate = x.DeliveryDate,
                    SupplierName = x.SupplierName ?? "",
                    ReferenceIndentNo = x.ReferenceIndentName ?? "",
                })
                .ToListAsync();

            var poNos = purchaseOrders
                .Where(x => !string.IsNullOrWhiteSpace(x.PONo))
                .Select(x => x.PONo)
                .ToList();

            var incomingMaterials = await _context.NablIncomingMaterials
                .Where(x =>
                    x.IsActive &&
                    poNos.Contains(x.PurchaseOrderNo!))
                .Select(x => new NablIncomingMaterialDto
                {
                    Id = x.ID,
                    PurchaseOrderNo = x.PurchaseOrderNo ?? "",
                    SupplierName = x.SupplierName ?? "",
                    InspectionPlanNoName = x.InspectionPlanNoName ?? "",
                    Date = x.Date,
                    InspectionResult = x.InspectionResult ?? "",
                })
                .ToListAsync();

            var result = new SupplierEvaluationDetailsDto
            {
                PurchaseOrders = purchaseOrders,
                IncomingMaterials = incomingMaterials
            };

            return result;
        }
        public async Task<List<DropdwonSelector>> GetSupplierDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.NablSupplierRegistrations
                         where a.IsActive
                         select a;


            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.SupplierName));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.SupplierName.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.SupplierName
                })
                .ToListAsync();

            return data;
        }
        private async Task<long> AddInventoryMaster(InventoryManagement obj)
        {
            await _context.InventoryManagements.AddAsync(obj);
            await _context.SaveChangesAsync();

            return obj.ID;
        }
        private async Task UpdateInventoryMaster(InventoryManagement obj)
        {
            _context.InventoryManagements.Update(obj);
            await _context.SaveChangesAsync();
        }
        private async Task<PagedResponse<object>> GetInventoryMasterList(PageFilter filter)
        {
            var query = _context.InventoryManagements
                .Where(c => c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);

            if (filter.Filter != null)
            {
                query = query.AsQueryable().ApplyFilters(filter.Filter);
            }

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();

                decimal? qty = decimal.TryParse(search, out var parsedQty) ? parsedQty : (decimal?)null;

                query = query.Where(x =>
                    (x.ItemName != null && x.ItemName.Contains(search)) ||
                    (x.ItemCode != null && x.ItemCode.Contains(search)) ||
                    (x.ItemCategory != null && x.ItemCategory.Contains(search)) ||
                    (x.Unit!= null && x.Unit.Contains(search)) ||
                    (qty != null && x.Quantity == qty)
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                string direction = filter.SortOrder?.ToLower() == "asc" ? "ascending" : "descending";
                query = query.OrderBy($"{filter.SortByColumn} {direction}");
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }

            return await query.Cast<object>().ToPagedAsync(filter);
        }

        private async Task DeleteInventoryMaster<T>(long id)
        {
            var entity = await _context.InventoryManagements
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (entity != null)
            {
                entity.IsActive = false;
                entity.ModifiedOn = DateTime.UtcNow;
                entity.ModifiedBy = loggedInUser.EmployeeID;
                _context.InventoryManagements.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddQuantityLog(InventoryQuantityLog log)
        {
            await _context.InventoryQuantityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
        public async Task<List<DropdwonSelector>> GetMaterialData(string formType, string type)
        {
            string? searchTerm; int pageNo = 0; int pageSize = 20;
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.InventoryManagements
                         where a.IsActive
                         select a;


            _query = _query.Where(x => x.ItemCategory == type);


            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = (x.ItemCode ?? "") + "/" + (x.ItemName)
                })
                .ToListAsync();

            return data;
        }
        public async Task<InventoryManagementDto> GetInventoryDetails(string itemCode, string itemName)
        {
            var data = await _context.InventoryManagements.Where(x => x.IsActive && x.ItemCode == itemCode && x.ItemName == itemName).FirstOrDefaultAsync();

            var res = new InventoryManagementDto
            {
                ItemName = data.ItemName,
                ItemCode = data.ItemCode,
                Unit = data.Unit,
                BatchNo = data.BatchNo,
                Date = data.Date,
                ItemCategory = data.ItemCategory,
                Manufacturer = data.Manufacturer,
                MinimumQuantity = data.MinimumQuantity,
                Quantity = data.Quantity,
                StorageLocation = data.StorageLocation,
                SupplierId = data.SupplierId,
                InventoryId =data.ID,
                DepartmentID =data.DepartmentID,
                SupplierName = data.SupplierName


            };
            return res;
        }
        private async Task<CrmConsumptionDetailsDto> GetByReferenceMaterialId(long id)
        {

            var crm = await _context.NablReferenceMaterials.FirstOrDefaultAsync(c => c.ID == id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
            if (crm == null)
            {
                throw new ArgumentException("Reference Material not found.");
            }
            var consumption = await _context.NablCrmConsumptions
                .Include(c => c.Logs.Where(c => c.IsActive)).
                FirstOrDefaultAsync(c => c.ReferenceMaterialId== id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
            return new CrmConsumptionDetailsDto
            {
                CrmDetails = new CrmDetailsDto
                {
                    ReferenceMaterialId = crm.ID,
                    DocumentNo = crm.DocumentNo,
                    BatchNo = crm.BatchNo,
                    CertificateNo = crm.CertificateNo,
                    Manufacturer = crm.Manufacturer,
                    MaterialClassification = crm.MatrixType,
                    MinimumQuantity = crm.MinimumQuantity,
                    Quantity = crm.InitialQuantity,
                    RmCode = crm.RMCode,
                    RmName = crm.RMName,
                    Type = crm.Type,
                    Unit = crm.Unit,
                    ValidityDate = crm.ValidityDate,
                    PreparedBy = crm.PreparedBy,
                    PreparedDate = crm.PreparedDate,
                    ApprovedBy = crm.ApprovedBy,
                    ReviewedBy = crm.ReviewedBy,
                    ApprovedDate = crm.ApprovedDate,
                    ReceivedDate = crm.ReceivedDate,
                    Date = crm.Date,


                },
                ConsumptionHeader = consumption == null
            ? null
            : new CrmConsumptionHeaderDto
            {
                Id = consumption.ID,
                ReferenceMaterialId = consumption.ReferenceMaterialId,
                FormNo = consumption.FormCode,
                DocumentNo = consumption.DocumentNo,
                IssueNo = consumption.IssueNo,
                RevNo = consumption.RevNo,
                RecordDate = consumption.Date,
                OpeningQuantity = consumption.OpeningQuantity,
                TotalConsumed = consumption.TotalConsumed,
                RemainingQuantity = consumption.RemainingQuantity,
                Notes = consumption.Notes,
                PreparedBy = consumption.PreparedBy,
                ReviewedBy = consumption.ReviewedBy,
                ApprovedBy = consumption.ApprovedBy
            },

                Logs = consumption?.Logs?
            .OrderByDescending(x => x.ConsumptionDate)
            .Select(x => new CrmConsumptionLogDto
            {
                Id = x.Id,
                ConsumptionDate = x.ConsumptionDate,
                QuantityConsumed = x.QuantityConsumed,
                PreviousBalanceQty = x.PreviousBalanceQty,
                BalanceQty = x.BalanceQty,
                Purpose = x.Purpose,
                EquipmentOrTest = x.EquipmentOrTest,
                UsedBy = x.UsedBy,
                Remarks = x.Remarks
            })
            .ToList() ?? new List<CrmConsumptionLogDto>()
            };
        }
        private async Task<NablQualityControlPlan> GetQualityControlPlanByIdTyped(long id)
        {

            var qcPlan = await _context.NablQualityControlPlans.Include(c => c.Activities.Where(a => a.IsActive)).FirstOrDefaultAsync(c => c.ID == id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
            if (qcPlan == null)
            {
                throw new ArgumentException("Reference Material not found.");
            }
            return qcPlan;
        }
        public async Task<List<DropdwonSelector>> GetEmployeesDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.EmployeeMasters
                         where a.IsActive
                         select a;


            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.Name));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.Name.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.Name
                })
                .ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> GetReferenceOptions(string referenceType)
        {
            string? searchTerm = null;
            int pageNo = 0;
            int pageSize = 20;
            if (pageNo < 0) pageNo = 0;
            if (pageSize <= 0) pageSize = 20;

            var skip = pageNo * pageSize;

            if (referenceType == "CRM")
            {
                var query = _context.NablReferenceMaterials
                    .Where(x => x.IsActive);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var search = searchTerm.Trim();

                    if (FilterHelper.IsExactIdSearch(search, out long exactId))
                    {
                        query = query.Where(x => x.ID == exactId);
                    }
                    else
                    {
                        query = query.Where(x =>
                            x.RMCode.Contains(search) ||
                            x.RMName.Contains(search));
                    }
                }

                return await query
                    .OrderBy(x => x.RMCode)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(x => new DropdwonSelector
                    {
                        Id = x.ID,
                        Name = x.RMCode + " - " + x.RMName
                    })
                    .ToListAsync();
            }

            if (referenceType == "Equipment")
            {
                var query = _context.EquipmentMasters
                    .Where(x => x.IsActive);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var search = searchTerm.Trim();

                    if (FilterHelper.IsExactIdSearch(search, out long exactId))
                    {
                        query = query.Where(x => x.ID == exactId);
                    }
                    else
                    {
                        query = query.Where(x =>
                            x.EquipmentNo.Contains(search) ||
                            x.Name.Contains(search));
                    }
                }

                return await query
                    .OrderBy(x => x.EquipmentNo)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(x => new DropdwonSelector
                    {
                        Id = x.ID,
                        Name = x.EquipmentNo + " - " + x.Name
                    })
                    .ToListAsync();
            }

            return new List<DropdwonSelector>();
        }
        public async Task<List<DropdwonSelector>> GetQcplannoDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = _context.NablQualityControlPlans.Where(c => c.IsActive && c.Activities.Any(c => c.IsActive && c.ActivityName == "Retesting of Retained Sample"));

            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.PlanNo));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.PlanNo.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.PlanNo
                })
                .ToListAsync();

            return data;
        }
        public async Task<RetestingQcPlanDetailsDto?> QCDetails(long id)
        {
            var plan = await _context.NablQualityControlPlans.Include(c => c.Activities).FirstOrDefaultAsync(c => c.ID == id && c.IsActive);

            if (plan == null)
                return null;

            var activity = plan.Activities.FirstOrDefault(c => c.QualityControlPlanId == id && c.ActivityName == "Retesting of Retained Sample" && c.IsActive);
            if (activity == null)
                return null;

            return new RetestingQcPlanDetailsDto
            {
                QCPlanId = plan.ID,
                QCPlanActivityId = activity.ID,

                PlanNo = plan.PlanNo,
                PlanYear = plan.PlanYear,
                Discipline = plan.Discipline,
                MaterialProductGroup = plan.MaterialProductGroup,
                LabIncharge = plan.LabIncharge,
                EffectiveFrom = activity.EffectiveFrom,
                EffectiveTo = activity.EffectiveTo,

                QCActivity = activity.ActivityName,
                DepartmentName = activity.DepartmentName,
                TestMethodName = activity.TestMethod,
                ReferenceType = activity.ReferenceType,
                ReferenceName = activity.ReferenceName,
                FrequencyType = activity.FrequencyType,
                ResponsibleEmployee = activity.EmployeeName,
                AcceptanceCriteria = activity.AcceptanceCriteria,
                NextDueDate = activity.NextDueDate


            };
        }
        public async Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = _context.NablCustomerFeedbacks.Where(c => c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);

            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.CompanyName));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.CompanyName.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.CompanyName
                })
                .ToListAsync();

            return data;
        }
        public async Task<CustomerFeedbackAnalysisDto?> GetFeedbackDetails(long id)
        {
            var customerFeedback = await _context.NablCustomerFeedbacks.FirstOrDefaultAsync(c => c.ID == id && c.IsActive);

            if (customerFeedback == null)
                return null;
            var ratings = string.IsNullOrWhiteSpace(customerFeedback.RatingsJson) ? new List<FeedbackRatingDto>() : JsonSerializer.Deserialize<List<FeedbackRatingDto>>(customerFeedback.RatingsJson)
        ?? new List<FeedbackRatingDto>();
            var validRating = ratings.Where(c => c.Rating.HasValue).Select(c => c.Rating!.Value).ToList();
            decimal averageRating = validRating.Any() ? Math.Round((decimal)validRating.Average(), 2) : 0;


            return new CustomerFeedbackAnalysisDto
            {
                CustomerId = customerFeedback.ID,
                CustomerName = customerFeedback.CustomerName,
                CompanyName = customerFeedback.CompanyName,
                Address = customerFeedback.CompanyAddress,
                ContactPerson = customerFeedback.ContactPerson,
                Designation = customerFeedback.Designation,
                FeedbackDate = customerFeedback.FeedbackDate,
                Email = customerFeedback.Email,
                MobileNo = customerFeedback.MobileNo,
                Suggestions = customerFeedback.Suggestions,
                NewRequirement = customerFeedback.CommentsSuggestions,
                Ratings = ratings,
                AverageRating = averageRating
            };

        }
        public async Task<List<DropdwonSelector>> GetMeetinglist(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.NablMeetingAgendas
                         where a.IsActive
                         select a;


            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.MeetingNo));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.MeetingNo.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = (x.MeetingNo ?? "")
                })
                .ToListAsync();

            return data;
        }
        public async Task<MeetingAgendaDto> GetMeetingDetails(string meetingNo)
        {
            var meetingAgenda = await _context.NablMeetingAgendas.FirstOrDefaultAsync(c => c.MeetingNo == meetingNo && c.IsActive);

            if (meetingAgenda == null)
                return null;
            var participants = string.IsNullOrWhiteSpace(meetingAgenda.ParticipantsJson) ? new List<ParticipantsDto>() : JsonSerializer.Deserialize<List<ParticipantsDto>>(meetingAgenda.ParticipantsJson)
        ?? new List<ParticipantsDto>();

            var agendaItems = string.IsNullOrWhiteSpace(meetingAgenda.AgendaItemsJson) ? new List<AgendaItemsDto>() : JsonSerializer.Deserialize<List<AgendaItemsDto>>(meetingAgenda.AgendaItemsJson)
        ?? new List<AgendaItemsDto>();


            return new MeetingAgendaDto
            {
                MeetingId = meetingAgenda.ID,
                ParticipantItems = participants,
                Agendalist = agendaItems,
                MeetingNo = meetingAgenda.MeetingNo,
                MeetingTime = meetingAgenda.MeetingTime,
                ChairpersonName = meetingAgenda.ChairpersonName,
                MeetingDate = meetingAgenda.MeetingDate,
                MeetingType = meetingAgenda.MeetingType,
                MeetingVenue = meetingAgenda.MeetingVenue
            };

        }

        public async Task<List<PurchaseMaterialVerificationPrintDto>> GetPurchaseMaterialVerificationPrintList()
        {
            var purchaseVerifications = await _context.NablPurchaseMaterialVerifications
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            if (!purchaseVerifications.Any())
                return new List<PurchaseMaterialVerificationPrintDto>();

            var result = new List<PurchaseMaterialVerificationPrintDto>();

            foreach (var verification in purchaseVerifications)
            {
                var materialDetails = string.IsNullOrWhiteSpace(verification.ItemsVerificationJson)
      ? new List<PurchaseMaterialVerificationItemDto>()
      : JsonSerializer.Deserialize<List<PurchaseMaterialVerificationItemDto>>
          (verification.ItemsVerificationJson)
          ?? new List<PurchaseMaterialVerificationItemDto>();

                result.Add(new PurchaseMaterialVerificationPrintDto
                {
                    Date = verification.Date,
                    PONo = verification.PurchaseOrderNo,
                    SupplierName = verification.SupplierName,

                    MaterialDetails = materialDetails
                });
            }

            return result;
        }

        private async Task<long> AddNonConformingWork(NablNonConformingWork model)
        {
            switch (model.RequestStep)
            {
                case 1:

                    return await AddTyped(model);

                case 2:

                    model.Investigation.NablNonConformingWorkId = model.ID;

                    await _context.NablNonConformingWorkInvestigations.AddAsync(model.Investigation);

                    await _context.SaveChangesAsync();

                    return model.ID;

                case 3:

                    model.CorrectiveAction.NablNonConformingWorkId = model.ID;

                    await _context.NablNonConformingWorkCorrectiveActions.AddAsync(model.CorrectiveAction);

                    await _context.SaveChangesAsync();

                    return model.ID;

                case 4:

                    model.Verification.NablNonConformingWorkId = model.ID;

                    await _context.NablNonConformingWorkVerifications.AddAsync(model.Verification);

                    await _context.SaveChangesAsync();

                    return model.ID;

                case 5:

                    model.Closure.NablNonConformingWorkId = model.ID;

                    await _context.NablNonConformingWorkClosures.AddAsync(model.Closure);

                    await _context.SaveChangesAsync();

                    return model.ID;

                default:

                    throw new Exception("Invalid Request Step");
            }
        }
        private async Task UpdateNonConformingWork(NablNonConformingWork model)
        {
            switch (model.RequestStep)
            {
                //==================================================
                // General
                //==================================================

                case 1:

                    var general = await _context.NablNonConformingWorks
                        .FirstOrDefaultAsync(x => x.ID == model.ID);

                    if (general == null)
                        throw new Exception("Record not found.");

                    general.NCDate = model.NCDate;
                    general.SampleCode = model.SampleCode;
                    general.TestParameter = model.TestParameter;
                    general.NCDescription = model.NCDescription;
                    general.NCSource = model.NCSource;

                    general.DetectedBy = model.DetectedBy;
                    general.IdentifiedBy = model.IdentifiedBy;

                    general.SuspendedWork = model.SuspendedWork;
                    general.AffectedResults = model.AffectedResults;

                    general.NCCategory = model.NCCategory;
                    general.RootCauseAnalysis = model.RootCauseAnalysis;

                    general.DepartmentId = model.DepartmentId;
                    general.DepartmentName = model.DepartmentName;

                    general.ReportedByEmployeeId = model.ReportedByEmployeeId;
                    general.ReportedByEmployeeName = model.ReportedByEmployeeName;

                    general.Source = model.Source;
                    general.Category = model.Category;
                    general.Priority = model.Priority;

                    general.ReferenceModule = model.ReferenceModule;
                    general.ReferenceId = model.ReferenceId;
                    general.ReferenceNo = model.ReferenceNo;

                    general.CustomerAffected = model.CustomerAffected;

                    general.Description = model.Description;
                    general.ImmediateAction = model.ImmediateAction;
                    general.ProblemDescription = model.ProblemDescription;

                    general.PreparedDate = model.PreparedDate;
                    general.ReviewedDate = model.ReviewedDate;
                    general.ApprovedDate = model.ApprovedDate;

                    general.ReviewedBy = model.ReviewedBy;
                    general.ApprovedBy = model.ApprovedBy;

                    general.CloserDate = model.CloserDate;
                    general.SignatureTDQM = model.SignatureTDQM;

                    general.ModifiedOn = model.ModifiedOn;
                    general.ModifiedBy = model.ModifiedBy;

                    break;

                //==================================================
                // Investigation
                //==================================================

                case 2:

                    if (model.Investigation == null)
                        break;

                    var investigation = await _context.NablNonConformingWorkInvestigations
                        .FirstOrDefaultAsync(x => x.NablNonConformingWorkId == model.ID);

                    if (investigation == null)
                    {
                        model.Investigation.NablNonConformingWorkId = model.ID;

                        await _context.NablNonConformingWorkInvestigations
                            .AddAsync(model.Investigation);
                    }
                    else
                    {
                        investigation.AssignedToEmployeeId = model.Investigation.AssignedToEmployeeId;
                        investigation.AssignedToEmployeeName = model.Investigation.AssignedToEmployeeName;
                        investigation.InvestigationDate = model.Investigation.InvestigationDate;
                        investigation.InvestigationMethod = model.Investigation.InvestigationMethod;
                        investigation.RootCause = model.Investigation.RootCause;
                        investigation.ContributingFactors = model.Investigation.ContributingFactors;
                        investigation.InvestigationDetails = model.Investigation.InvestigationDetails;
                        investigation.RecommendedAction = model.Investigation.RecommendedAction;
                    }

                    break;

                //==================================================
                // Corrective Action
                //==================================================

                case 3:

                    if (model.CorrectiveAction == null)
                        break;

                    var correctiveAction = await _context.NablNonConformingWorkCorrectiveActions
                        .FirstOrDefaultAsync(x => x.NablNonConformingWorkId == model.ID);

                    if (correctiveAction == null)
                    {
                        model.CorrectiveAction.NablNonConformingWorkId = model.ID;

                        await _context.NablNonConformingWorkCorrectiveActions
                            .AddAsync(model.CorrectiveAction);
                    }
                    else
                    {
                        correctiveAction.ActionNo = model.CorrectiveAction.ActionNo;
                        correctiveAction.CorrectiveAction = model.CorrectiveAction.CorrectiveAction;
                        correctiveAction.ResponsiblePersonId = model.CorrectiveAction.ResponsiblePersonId;
                        correctiveAction.ResponsiblePersonName = model.CorrectiveAction.ResponsiblePersonName;
                        correctiveAction.TargetDate = model.CorrectiveAction.TargetDate;
                        correctiveAction.CompletionDate = model.CorrectiveAction.CompletionDate;
                        correctiveAction.ResourcesRequired = model.CorrectiveAction.ResourcesRequired;
                        correctiveAction.PreventiveAction = model.CorrectiveAction.PreventiveAction;
                    }

                    break;

                //==================================================
                // Verification
                //==================================================

                case 4:

                    if (model.Verification == null)
                        break;

                    var verification = await _context.NablNonConformingWorkVerifications
                        .FirstOrDefaultAsync(x => x.NablNonConformingWorkId == model.ID);

                    if (verification == null)
                    {
                        model.Verification.NablNonConformingWorkId = model.ID;

                        await _context.NablNonConformingWorkVerifications
                            .AddAsync(model.Verification);
                    }
                    else
                    {
                        verification.VerificationDate = model.Verification.VerificationDate;
                        verification.VerifiedByEmployeeId = model.Verification.VerifiedByEmployeeId;
                        verification.VerifiedByEmployeeName = model.Verification.VerifiedByEmployeeName;
                        verification.VerificationMethod = model.Verification.VerificationMethod;
                        verification.Observation = model.Verification.Observation;
                        verification.Result = model.Verification.Result;
                        verification.Remarks = model.Verification.Remarks;
                    }

                    break;

                //==================================================
                // Closure
                //==================================================

                case 5:

                    if (model.Closure == null)
                        break;

                    var closure = await _context.NablNonConformingWorkClosures
                        .FirstOrDefaultAsync(x => x.NablNonConformingWorkId == model.ID);

                    if (closure == null)
                    {
                        model.Closure.NablNonConformingWorkId = model.ID;

                        await _context.NablNonConformingWorkClosures
                            .AddAsync(model.Closure);
                    }
                    else
                    {
                        closure.ClosureDate = model.Closure.ClosureDate;
                        closure.ClosedByEmployeeId = model.Closure.ClosedByEmployeeId;
                        closure.ClosedByEmployeeName = model.Closure.ClosedByEmployeeName;
                        closure.FinalRemarks = model.Closure.FinalRemarks;
                        closure.Status = model.Closure.Status;
                    }

                    break;

                default:

                    throw new Exception("Invalid Request Step");
            }

            await _context.SaveChangesAsync();
        }

        private async Task<NablNonConformingWork> GetByIdNonConformingWork(long id)
        {
            var data = await _context.NablNonConformingWorks.Include(c => c.Investigation).Include(c => c.CorrectiveAction).Include(c => c.Verification).Include(c => c.Closure).FirstOrDefaultAsync(c => c.ID == id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
            return data;
        }
        public async Task<PagedResponse<object>> NcPrintList(PageFilter filter)
        {
            var query = _context.NablNonConformingWorks
                .Where(x => x.IsActive &&
                            x.CompanyCode == loggedInUser.CompanyCode);

            if (filter.Filter != null)
            {
                query = query.ApplyFilters(filter.Filter);
            }

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();

                query = query.Where(x =>
                    (x.NcNo != null && x.NcNo.ToLower().Contains(search)) ||
                    (x.Description != null && x.Description.ToLower().Contains(search)) ||
                    (x.RootCauseAnalysis != null && x.RootCauseAnalysis.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                string direction = filter.SortOrder?.ToLower() == "asc"
                    ? "ascending"
                    : "descending";

                query = query.OrderBy($"{filter.SortByColumn} {direction}");
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }

            var result = query.Select(x => new NonConformingWorkPrintDto
            {
                Id = x.ID,

                NcNo = x.NcNo,

                NcDate = x.NCDate,

                Description = x.Description,

                RootCauseAnalysis = x.Investigation != null
                    ? x.Investigation.RootCause
                    : string.Empty,

                CorrectiveAction = x.CorrectiveAction != null
                    ? x.CorrectiveAction.CorrectiveAction
                    : string.Empty,

                ClosureDate = x.Closure != null
                    ? x.Closure.ClosureDate
                    : null,

                SignatureTDQM = x.SignatureTDQM
            });

            return await result.Cast<object>().ToPagedAsync(filter);

        }
        public async Task<List<DropdwonSelector>> Documentlist(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0)
                pageNo = 0;


            var query = _context.NablMasterDocuments
                .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.DocumentCode));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    query = query.Where(x => x.DocumentCode.Contains(search));
                }
            }



            var skip = pageNo * pageSize;

            var data = query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = (x.DocumentCode ?? "")  + "/" + (x.DocumentTitle ?? ""),
                    AdditionalValues = new Dictionary<string, object>
                    {
                { "DocumentType", x.DocumentType ?? "" },
                { "DepartmentName", x.DepartmentName ?? "" },
                { "CurrentIssue", x.CurrentIssue ?? "" },
                { "CurrentRevision", x.CurrentRevision ?? "" },
                {"EffectiveDate",x.EffectiveDate},
                {"NextReviewDate",x.NextReviewDate},
                {"DocumentOwner",x.DocumentOwner},
                {"DocumentName",(x.DocumentCode) + "/" + (x.DocumentTitle)}
                    }
                })
                .ToList();
            return data;
        }
        public async Task<List<DropdwonSelector>> GetAuditorsDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.NablInternalAuditors
                         where a.IsActive
                         select a;


            _query = _query.Where(x => !string.IsNullOrWhiteSpace(x.EmployeeName));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.EmployeeName.Contains(search));
                }
            }

            var skip = pageNo * pageSize;

            var data = await _query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.EmployeeName
                })
                .ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> GetEligibleAuditors(
         long departmentId,
         string isoClauseIds,
         DateTime scheduleDate)
        {
            int pageNo = 0;
            int pageSize = 20;

            var selectedClauseIds = isoClauseIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x.Trim(), out var id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (selectedClauseIds.Count == 0)
                return new List<DropdwonSelector>();

            // First fetch only active and date-valid auditors.
            // Department eligibility will be checked from DepartmentListJson.
            var auditors = await _context.NablInternalAuditors
                .Where(x =>
                    x.IsActive &&
                    x.EmployeeId.HasValue &&
                    x.AuthorizationValidUpto.HasValue &&
                    x.AuthorizationValidUpto.Value.Date >= scheduleDate.Date &&
                    !string.IsNullOrWhiteSpace(x.EmployeeName) &&
                    !string.IsNullOrWhiteSpace(x.ISOClausesJson) &&
                    !string.IsNullOrWhiteSpace(x.DepartmentListJson))
                .ToListAsync();

            foreach (var auditor in auditors)
            {
                auditor.IsoClauses =
                    !string.IsNullOrWhiteSpace(auditor.ISOClausesJson)
                        ? JsonSerializer.Deserialize<List<IsoClauses>>(
                            auditor.ISOClausesJson
                          ) ?? new List<IsoClauses>()
                        : new List<IsoClauses>();

                auditor.DepartmentList =
                    !string.IsNullOrWhiteSpace(auditor.DepartmentListJson)
                        ? JsonSerializer.Deserialize<List<DepartmentList>>(
                            auditor.DepartmentListJson
                          ) ?? new List<DepartmentList>()
                        : new List<DepartmentList>();
            }

            var eligibleAuditors = auditors.Where(a =>
            {
                if (a.DepartmentList == null || a.DepartmentList.Count == 0)
                    return false;

                if (a.IsoClauses == null || a.IsoClauses.Count == 0)
                    return false;

                // Selected department must exist in auditor's Authorized Areas list.
                var departmentMatched = a.DepartmentList.Any(d =>
                    d.DepartmentId.HasValue &&
                    d.DepartmentId.Value == departmentId);

                if (!departmentMatched)
                    return false;

                // Auditor must be authorized for all selected ISO clauses.
                var auditorClauseIds = a.IsoClauses
                    .Where(x => x.ClauseId.HasValue)
                    .Select(x => x.ClauseId!.Value)
                    .Distinct()
                    .ToHashSet();

                return selectedClauseIds.All(id =>
                    auditorClauseIds.Contains(id));
            });

            return eligibleAuditors
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.EmployeeId!.Value,
                    Name = x.EmployeeName!
                })
                .ToList();
        }
        private async Task<NablAuditPlan> GetByAuditPlanIdTyped(long id)
        {
            var auditPlan = await _context.NablAuditPlans
                .Include(x => x.ScheduleItems)
                .FirstOrDefaultAsync(x =>
                    x.ID == id &&
                    x.IsActive &&
                    x.CompanyCode == loggedInUser.CompanyCode
                );

            if (auditPlan == null)
                throw new ArgumentException("Audit Plan not found.");

            foreach (var scheduleItem in auditPlan.ScheduleItems)
            {
                scheduleItem.IsoClauses =
                    !string.IsNullOrWhiteSpace(scheduleItem.ISOClausesJson)
                        ? JsonSerializer.Deserialize<List<AuditScheduleIsoClause>>(
                            scheduleItem.ISOClausesJson
                          ) ?? new List<AuditScheduleIsoClause>()
                        : new List<AuditScheduleIsoClause>();
            }

            return auditPlan;
        }
        public async Task<AuditChecklistDto?> GetScheduleSession(long scheduleItemId)
        {
            var schedule = await _context.ScheduleItems
                .FirstOrDefaultAsync(x => x.ID == scheduleItemId && x.IsActive == true);
            var scheduleplan = await _context.NablAuditPlans.FirstOrDefaultAsync(x => x.ID == schedule.AuditPlanId && x.IsActive == true);
            if (schedule == null)
                return null;

            var dto = new AuditChecklistDto
            {
                AuditPlanId = schedule.AuditPlanId,
                ScheduleItemId = schedule.ID,
                AuditPlanNo = schedule.AuditPlan?.PlanNo,
                DepartmentId = schedule.DepartmentId,
                DepartmentName = schedule.DepartmentName,
                AuditorId = schedule.AuditorId,
                AuditorName = schedule.AuditorName,
                AuditeeId = schedule.AuditeeId,
                AuditeeName = schedule.AuditeeName,
                ScheduleDate = schedule.ScheduleDate,
                PlanNo= scheduleplan.PlanNo,
            };

            if (!string.IsNullOrWhiteSpace(schedule.ISOClausesJson))
            {
                dto.IsoClauses =
                    JsonSerializer.Deserialize<List<AuditChecklistIsoClauseDto>>(
                        schedule.ISOClausesJson
                    ) ?? new List<AuditChecklistIsoClauseDto>();
            }

            return dto;
        }
        private async Task<NablAuditChecklist> GetByAuditChecklistTyped(long id)
        {
            var auditChecklist = await _context.NablAuditChecklists
      .Include(x => x.Items.Where(item => item.IsActive)) // Filters the related Items collection
      .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (auditChecklist == null)
                throw new ArgumentException("Audit checklist not found.");

            var ncIds = auditChecklist.Items
      .Where(x =>
          x.IsActive &&
          x.NcId.HasValue
      )
      .Select(x => x.NcId!.Value)
      .Distinct()
      .ToList();

            if (ncIds.Any())
            {
                var ncrData = await _context.NablNonConformingWorks
                    .Where(x =>
                        ncIds.Contains(x.ID) &&
                        x.IsActive
                    )
                    .Select(x => new
                    {
                        x.ID,
                        x.CurrentStep,
                        x.Status
                    })
                    .ToListAsync();

                foreach (var item in auditChecklist.Items)
                {
                    if (!item.NcId.HasValue)
                        continue;

                    var ncr = ncrData.FirstOrDefault(x =>
                        x.ID == item.NcId.Value
                    );

                    if (ncr != null)
                    {
                        item.NcCurrentStep = ncr.CurrentStep;
                        item.NcStatus = ncr.Status;
                    }
                }
            }
            return auditChecklist;
        }
        public async Task<AuditChecklistNcrDto?> GetAuditChecklistNcr(long checklistItemId)
        {
            var item = await _context.AuditChecklistItems
                .Include(x => x.Checklist)
                .FirstOrDefaultAsync(x =>
                    x.ID == checklistItemId &&
                    x.IsActive
                );

            if (item == null || item.Checklist == null)
                return null;

            var checklist = item.Checklist;

            var dto = new AuditChecklistNcrDto
            {
                ChecklistId = checklist.ID,
                ChecklistItemId = item.ID,
                ChecklistNo = checklist.ChecklistNo,
                AuditPlanId = checklist.AuditPlanId,
                ScheduleItemId = checklist.ScheduleItemId,
                DepartmentId = checklist.DepartmentId,
                DepartmentName = checklist.DepartmentName,
                AuditorId = checklist.AuditorId,
                AuditorName = checklist.AuditorName,
                FindingType = item.FindingType,
                AuditQuestion = item.AuditQuestion,
                ObjectiveEvidence = item.ObjectiveEvidence,
            };

            return dto;
        }
        public async Task<AuditSummaryDto?> GetAuditplan(long auditPlanId)
        {
            var auditPlan = await _context.NablAuditPlans
                .Include(x => x.ScheduleItems)
                .FirstOrDefaultAsync(x =>
                    x.ID == auditPlanId &&
                    x.IsActive
                );

            if (auditPlan == null)
                return null;

            var scheduleItems = auditPlan.ScheduleItems
                .Where(x => x.IsActive == true)
                .ToList();

            // Schedule Item IDs
            var scheduleItemIds = scheduleItems
                .Select(x => x.ID)
                .ToList();

            // Linked Checklists
            var checklists = await _context.NablAuditChecklists
                .Where(x =>
                    scheduleItemIds.Contains(x.ScheduleItemId) &&
                    x.IsActive
                )
                .ToListAsync();

            var checklistIds = checklists
                .Select(x => x.ID)
                .ToList();

            // Checklist Items
            var checklistItems = await _context.AuditChecklistItems
                .Where(x =>
                    checklistIds.Contains(x.ChecklistId) &&
                    x.IsActive
                )
                .ToListAsync();

            // NCR IDs
            var ncIds = checklistItems
                .Where(x => x.NcId.HasValue)
                .Select(x => x.NcId!.Value)
                .Distinct()
                .ToList();

            // NCR Records
            var ncrs = await _context.NablNonConformingWorks
                .Where(x =>
                    ncIds.Contains(x.ID) &&
                    x.IsActive
                )
                .ToListAsync();

            // Counts
            var totalAudits = scheduleItems.Count;

            var completed = scheduleItems.Count(x =>
                x.Status == "Completed"
            );

            var inProgress = scheduleItems.Count(x =>
                x.Status == "InProgress"
            );

            var scheduled = scheduleItems.Count(x =>
                x.Status == "Scheduled"
            );

            var majorNcrs = checklistItems.Count(x =>
                x.FindingType == "Major NC"
            );

            var minorNcrs = checklistItems.Count(x =>
                x.FindingType == "Minor NC"
            );

            var observations = checklistItems.Count(x =>
                x.FindingType == "Observation"
            );

            var totalNcrs =
                majorNcrs + minorNcrs;

            var closedNcrs = ncrs.Count(x =>
                x.Status == "Completed"
            );

            var pendingNcrs = ncrs.Count(x =>
                x.Status != "Completed"
            );

            var departmentSummary = new List<AuditDepartmentSummaryDto>();

            var departmentGroups = scheduleItems
                .GroupBy(x => new
                {
                    x.DepartmentId,
                    x.DepartmentName
                });

            foreach (var group in departmentGroups)
            {
                var departmentSchedules = group.ToList();

                var departmentScheduleIds = departmentSchedules
                    .Select(x => x.ID)
                    .ToList();

                // Department ke schedule items se linked checklists
                var departmentChecklists = checklists
                    .Where(x =>
                        departmentScheduleIds.Contains(x.ScheduleItemId)
                    )
                    .ToList();

                // Department ke checklist IDs
                var departmentChecklistIds = departmentChecklists
                    .Select(x => x.ID)
                    .ToList();

                // Department ke checklist items
                var departmentItems = checklistItems
                    .Where(x =>
                        departmentChecklistIds.Contains(x.ChecklistId)
                    )
                    .ToList();

                departmentSummary.Add(
                    new AuditDepartmentSummaryDto
                    {
                        DepartmentId = group.Key.DepartmentId,
                        DepartmentName = group.Key.DepartmentName,

                        TotalAudits = departmentSchedules.Count,

                        Completed = departmentSchedules.Count(x =>
                            x.Status == "Completed"
                        ),

                        InProgress = departmentSchedules.Count(x =>
                            x.Status == "InProgress"
                        ),

                        Scheduled = departmentSchedules.Count(x =>
                            x.Status == "Scheduled"
                        ),

                        MajorNcrs = departmentItems.Count(x =>
                            x.FindingType == "Major NC"
                        ),

                        MinorNcrs = departmentItems.Count(x =>
                            x.FindingType == "Minor NC"
                        ),

                        Observations = departmentItems.Count(x =>
                            x.FindingType == "Observation"
                        )
                    }
                );
            }
            var dto = new AuditSummaryDto
            {
                AuditPlanId = auditPlan.ID,

                AuditPlanNo = auditPlan.PlanNo,
                AuditType = auditPlan.AuditType,
                PlanningYear = auditPlan.AuditYear,

                LeadAuditor = auditPlan.LeadAuditorName,

                AuditFrom = auditPlan.ScheduleDateFrom,
                AuditTo = auditPlan.ScheduleDateTo,

                AuditCriteria = auditPlan.AuditCriteria,
                ScopeOfAudit = auditPlan.AuditScope,
                AuditObjective = auditPlan.AuditObjective,

                TotalAudits = totalAudits,
                Completed = completed,
                InProgress = inProgress,
                Scheduled = scheduled,

                TotalNcrs = totalNcrs,
                MajorNcrs = majorNcrs,
                MinorNcrs = minorNcrs,
                Observations = observations,

                ClosedNcrs = closedNcrs,
                PendingNcrs = pendingNcrs,
                DepartmentSummary = departmentSummary
            };

            return dto;
        }
        private async Task<NablMasterDocument> GetMasterDocumentById(long id)
        {
            var data = await _context.NablMasterDocuments.FirstOrDefaultAsync(c => c.ID == id && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode);
            if (data == null)
                return null;

            var review = await _context.NablDocumentReviews.Where(x => x.DocumentId == data.ID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode).OrderByDescending(x => x.ID).FirstOrDefaultAsync();
            if (review != null)
            {
                data.HasReview = true;
                data.ReviewId = review.ID;
                data.ReviewStatus = review.Status;
            }
            return data;
        }
        private async Task<NablDocumentReview?> GetByDocumentReviewId(long id)
        {
            var data = await _context.NablDocumentReviews
                .FirstOrDefaultAsync(c =>
                    c.ID == id &&
                    c.IsActive &&
                    c.CompanyCode == loggedInUser.CompanyCode
                );

            if (data == null)
                return null;


            // Change Required = No
            // No reviewer restriction
            if (data.ChangeRequired != true)
            {
                data.CanEditReview = true;
                return data;
            }


            // Change Required = Yes
            // Check linked DCR
            var dcr = await _context.NablDocumentChangeRequests
                .FirstOrDefaultAsync(x =>
                    x.SourceReviewId == data.ID &&
                    x.IsActive &&
                    x.CompanyCode == loggedInUser.CompanyCode
                );


            // DCR abhi create nahi hua
            // Review editable rahega
            if (dcr == null)
            {
                data.CanEditReview = true;
                return data;
            }


            // DCR created
            // Selected reviewer must match logged-in user
            data.CanEditReview =
                dcr.ReviewedById == loggedInUser.EmployeeID;


            return data;
        }
        public async Task<List<DropdwonSelector>> GetDocumentsAvailableForReview(
    string? searchTerm,
    int pageNo = 0,
    int pageSize = 20)
        {
            if (pageNo < 0)
                pageNo = 0;

            var query = _context.NablMasterDocuments
                .Where(x =>
                    x.IsActive &&
                    !string.IsNullOrWhiteSpace(x.DocumentCode) &&

                    !_context.NablDocumentReviews.Any(r =>
                        r.DocumentId == x.ID &&
                        r.IsActive
                    )
                );

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();

                    query = query.Where(x =>
                        x.DocumentCode.Contains(search)
                    );
                }
            }

            var skip = pageNo * pageSize;

            var data = query
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,

                    Name =
                        (x.DocumentCode ?? "") +
                        "/" +
                        (x.DocumentTitle ?? ""),

                    AdditionalValues =
                        new Dictionary<string, object>
                        {
                    { "DocumentType", x.DocumentType ?? "" },
                    { "DepartmentName", x.DepartmentName ?? "" },
                    { "CurrentIssue", x.CurrentIssue ?? "" },
                    { "CurrentRevision", x.CurrentRevision ?? "" },
                    { "EffectiveDate", x.EffectiveDate },
                    { "NextReviewDate", x.NextReviewDate },
                    { "DocumentOwner", x.DocumentOwner ?? "" },
                    {
                        "DocumentName",
                        (x.DocumentCode ?? "") +
                        "/" +
                        (x.DocumentTitle ?? "")
                    }
                        }
                })
                .ToList();

            return data;
        }
        private async Task<PagedResponse<object>> GetDocumentReviewList(PageFilter filter)
        {
            var query = _context.NablDocumentReviews
                .Where(c =>
                    c.IsActive &&
                    c.CompanyCode == loggedInUser.CompanyCode
                );

            if (filter.Filter != null)
            {
                query = query
                    .AsQueryable()
                    .ApplyFilters(filter.Filter);
            }

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();

                query = query.Where(x =>
                    (x.ReviewNo != null && x.ReviewNo.Contains(search)) ||
                    (x.ReviewType != null && x.ReviewType.Contains(search)) ||
                    (x.DocumentName != null && x.DocumentName.Contains(search)) ||
                    (x.DepartmentName != null && x.DepartmentName.Contains(search)) ||
                    (x.Status != null && x.Status.Contains(search))
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                string direction =
                    filter.SortOrder?.ToLower() == "asc"
                        ? "ascending"
                        : "descending";

                query = query.OrderBy(
                    $"{filter.SortByColumn} {direction}"
                );
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }

            var resultQuery = query.Select(x => new NablDocumentReview
            {
                ID = x.ID,
                ReviewNo = x.ReviewNo,
                ReviewType = x.ReviewType,
                DocumentName = x.DocumentName,
                DepartmentName = x.DepartmentName,
                ChangeRequired = x.ChangeRequired,
                NextReviewDate = x.NextReviewDate,
                Status = x.Status,

                CanEditReview =
         // Case 1: Change Required = No
         x.ChangeRequired != true

         ||

         // Case 2: Change Required = Yes,
         // but DCR abhi create hi nahi hua
         !_context.NablDocumentChangeRequests.Any(dcr =>
             dcr.SourceReviewId == x.ID &&
             dcr.IsActive &&
             dcr.CompanyCode == loggedInUser.CompanyCode
         )

         ||

         // Case 3: DCR created hai
         // and selected reviewer = logged-in user
         _context.NablDocumentChangeRequests.Any(dcr =>
             dcr.SourceReviewId == x.ID &&
             dcr.IsActive &&
             dcr.CompanyCode == loggedInUser.CompanyCode &&
             dcr.ReviewedById == loggedInUser.EmployeeID
         )
            });

            return await resultQuery
                .Cast<object>()
                .ToPagedAsync(filter);
        }
        public async Task<List<MasterDocumentPrintDto>> GetMasterDocumentPrintList()
        {
            var documents = await _context.NablMasterDocuments
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

            if (!documents.Any())
                return new List<MasterDocumentPrintDto>();

            var result = new List<MasterDocumentPrintDto>();

            foreach (var document in documents)
            {
                var controlledCopies = string.IsNullOrWhiteSpace(document.ControlledCopiesJson)
                    ? new List<ControlledCopyPrintDto>()
                    : JsonSerializer.Deserialize<List<ControlledCopyPrintDto>>(
                        document.ControlledCopiesJson
                      ) ?? new List<ControlledCopyPrintDto>();

                var copyHolders = string.Join(
                    ", ",
                    controlledCopies
                        .Where(x => !string.IsNullOrWhiteSpace(x.HolderName))
                        .Select(x => x.HolderName!.Trim())
                );

                result.Add(new MasterDocumentPrintDto
                {
                   
                    DocumentCode = document.DocumentCode,
                    DocumentTitle = document.DocumentTitle,
                    DocumentNo = document.DocumentNo,
                    DocumentType = document.DocumentType,
                    DocumentOwner = document.DocumentOwner,
                    IssueNo = document.IssueNo,
                    RevNo = document.RevNo,
                    CopyHolders = copyHolders
                });
            }

            return result;
        }
    }

}
