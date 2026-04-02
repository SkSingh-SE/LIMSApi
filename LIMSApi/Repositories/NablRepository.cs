using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class NablRepository : INablRepository
    {
        private readonly LIMSContext _context;
        private readonly LoggedInUserDTO loggedInUser;

        public NablRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<PagedResponse<object>> GetAll(string formType, PageFilter filter)
        {
            return formType switch
            {
                "JobDescription" => await GetAllTyped<NablJobDescription>(filter),
                "ResponsibilityAuthority" => await GetAllTyped<NablResponsibilityAuthority>(filter),
                "EmployeeCompetence" => await GetAllTyped<NablEmployeeCompetence>(filter),
                "EmployeePerformanceRecord" => await GetAllTyped<NablEmployeePerformanceRecord>(filter),
                "EmployeeAuthorization" => await GetAllTyped<NablEmployeeAuthorization>(filter),
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
                "EmployeeAuthorization" => await GetByIdTyped<NablEmployeeAuthorization>(id),
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
                "SupplierEvaluation" => await AddTyped((NablSupplierEvaluation)model),
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
                    await UpdateTyped((NablSupplierEvaluation)model);
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

            // Global search
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    (x.DocumentNo != null && x.DocumentNo.ToLower().Contains(search)) ||
                    (x.FormCode != null && x.FormCode.ToLower().Contains(search)) ||
                    (x.Status != null && x.Status.ToLower().Contains(search)));
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

        private async Task<long> AddTyped<T>(T model) where T : NablFormBase
        {
            await _context.Set<T>().AddAsync(model);
            await _context.SaveChangesAsync();
            return model.ID;
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
    }
}
