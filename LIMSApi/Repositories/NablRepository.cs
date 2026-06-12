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
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;

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
                "DocumentReview" => await GetAllTyped<NablDocumentReview>(filter),
                "MasterDocument" => await GetAllTyped<NablMasterDocument>(filter),
                "MeasurementUncertainty" => await GetAllTyped<NablMeasurementUncertainty>(filter),
                "PtIlcPlan" => await GetAllTyped<NablPtIlcPlan>(filter),
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
                "QualityControlPlan" => await GetByIdTyped<NablQualityControlPlan>(id),
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
                "CrmConsumption" => await GetByIdTyped<NablCrmConsumption>(id),
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
                "AuditPlan" => await GetByIdTyped<NablAuditPlan>(id),
                "AuditChecklist" => await GetByIdTyped<NablAuditChecklist>(id),
                "AuditSummary" => await GetByIdTyped<NablAuditSummary>(id),
                "InternalAuditor" => await GetByIdTyped<NablInternalAuditor>(id),
                "MeetingAgenda" => await GetByIdTyped<NablMeetingAgenda>(id),
                "MeetingMinutes" => await GetByIdTyped<NablMeetingMinutes>(id),
                "NonConformingWork" => await GetByIdTyped<NablNonConformingWork>(id),
                "NcCorrectiveAction" => await GetByIdTyped<NablNcCorrectiveAction>(id),
                "Retesting" => await GetByIdTyped<NablRetesting>(id),
                "RiskAssessment" => await GetByIdTyped<NablRiskAssessment>(id),
                "DocumentChangeRequest" => await GetByIdTyped<NablDocumentChangeRequest>(id),
                "DocumentReview" => await GetByIdTyped<NablDocumentReview>(id),
                "MasterDocument" => await GetByIdTyped<NablMasterDocument>(id),
                "MeasurementUncertainty" => await GetByIdTyped<NablMeasurementUncertainty>(id),
                "PtIlcPlan" => await GetByIdTyped<NablPtIlcPlan>(id),
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
                "NonConformingWork" => await AddTyped((NablNonConformingWork)model),
                "NcCorrectiveAction" => await AddTyped((NablNcCorrectiveAction)model),
                "Retesting" => await AddTyped((NablRetesting)model),
                "RiskAssessment" => await AddTyped((NablRiskAssessment)model),
                "DocumentChangeRequest" => await AddTyped((NablDocumentChangeRequest)model),
                "DocumentReview" => await AddTyped((NablDocumentReview)model),
                "MasterDocument" => await AddTyped((NablMasterDocument)model),
                "MeasurementUncertainty" => await AddTyped((NablMeasurementUncertainty)model),
                "PtIlcPlan" => await AddTyped((NablPtIlcPlan)model),
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
                    await UpdateTyped((NablNonConformingWork)model);
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
                {"Address",x.Address ?? "" }
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

    }
}
