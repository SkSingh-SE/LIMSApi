using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountFieldsAndFinancialYearChangeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "TaxInvoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "TaxInvoices",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "ProformaInvoiceHeader",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "ProformaInvoiceHeader",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablTrainingPlans",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablTrainingPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablTrainingPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablTrainingEffectiveness",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablTrainingEffectiveness",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablTrainingEffectiveness",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablTrainingAttendances",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablTrainingAttendances",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablTrainingAttendances",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablTestRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablTestRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablTestRequests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablTestReports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablTestReports",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablTestReports",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablTestMethods",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablTestMethods",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablTestMethods",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablTechnicalRawDatas",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablTechnicalRawDatas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablTechnicalRawDatas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablSupplierRegistrations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablSupplierRegistrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablSupplierRegistrations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablSupplierEvaluations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablSupplierEvaluations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablSupplierEvaluations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablSupplierConfidentialities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablSupplierConfidentialities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablSupplierConfidentialities",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablSkillMatrixDecisions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablSkillMatrixDecisions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablSkillMatrixDecisions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablSkillMatrices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablSkillMatrices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablSkillMatrices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablSampleMusterRegisters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablSampleMusterRegisters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablSampleMusterRegisters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablSampleLabels",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablSampleLabels",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablSampleLabels",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablSampleInwardRegisters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablSampleInwardRegisters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablSampleInwardRegisters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablRiskAssessments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablRiskAssessments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablRiskAssessments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablRetestings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablRetestings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablRetestings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablResponsibilityAuthorities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablResponsibilityAuthorities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablResponsibilityAuthorities",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablReferenceMaterials",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablReferenceMaterials",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablReferenceMaterials",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablQualityControlPlans",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablQualityControlPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablQualityControlPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablPurchaseOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablPurchaseOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablPurchaseOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablPurchaseMaterialVerifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablPurchaseMaterialVerifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablPurchaseIndents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablPurchaseIndents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablPurchaseIndents",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablPtIlcPlans",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablPtIlcPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablPtIlcPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablProductInspections",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablProductInspections",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablProductInspections",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablNonConformingWorks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablNonConformingWorks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablNonConformingWorks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablMethodVerifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablMethodVerifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablMethodVerifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablMethodValidations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablMethodValidations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablMethodValidations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablMeetingMinutes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablMeetingMinutes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablMeetingMinutes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablMeetingAgendas",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablMeetingAgendas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablMeetingAgendas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablMeasurementUncertainties",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablMeasurementUncertainties",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablMeasurementUncertainties",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablMasterDocuments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablMasterDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablMasterDocuments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablJobDescriptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablJobDescriptions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablJobDescriptions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablInternalAuditors",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablInternalAuditors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablInternalAuditors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablIntermediateChecks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablIntermediateChecks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablIntermediateChecks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablInductionTrainings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformanceLevel",
                table: "NablInductionTrainings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablInductionTrainings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablInductionTrainings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablIncomingMaterials",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablIncomingMaterials",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablIncomingMaterials",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablFormRevisionHistory",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablFeedbackAnalyses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablEquipmentHistories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablEquipmentHistories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablEquipmentHistories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablEnvironmentMonitorings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablEnvironmentMonitorings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablEnvironmentMonitorings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablEmployeePerformanceRecords",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablEmployeePerformanceRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablEmployeePerformanceRecords",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablEmployeeCompetences",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablEmployeeCompetences",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablEmployeeCompetences",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablEmployeeAuthorizations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablEmployeeAuthorizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablEmployeeAuthorizations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablDocumentReviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablDocumentReviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablDocumentReviews",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablDocumentChangeRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablCustomerFeedbacks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablCrmConsumptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablCrmConsumptions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablCrmConsumptions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablComplaints",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablComplaints",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablComplaints",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablCompetenceRequirements",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablCompetenceRequirements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablCompetenceRequirements",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablCalibrationReviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablCalibrationReviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablCalibrationReviews",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablAuditSummaries",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablAuditSummaries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablAuditSummaries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablAuditPlans",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablAuditPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablAuditPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablAuditChecklists",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablAuditChecklists",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablAuditChecklists",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "NablApprovedSuppliers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AgreementDate",
                table: "NablApprovedSuppliers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlacklistDate",
                table: "NablApprovedSuppliers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentDocumentId",
                table: "NablApprovedSuppliers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionReason",
                table: "NablApprovedSuppliers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionType",
                table: "NablApprovedSuppliers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinancialYearChangeLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: false),
                    OldYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NewYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedById = table.Column<long>(type: "bigint", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialYearChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialYearChangeLogs_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinancialYearChangeLogs_UserMasters_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "UserMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialYearChangeLogs_ChangedById",
                table: "FinancialYearChangeLogs",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialYearChangeLogs_FinancialYearId",
                table: "FinancialYearChangeLogs",
                column: "FinancialYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialYearChangeLogs");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "ProformaInvoiceHeader");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "ProformaInvoiceHeader");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablTrainingEffectiveness");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablTrainingEffectiveness");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablTrainingEffectiveness");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablTrainingAttendances");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablTrainingAttendances");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablTrainingAttendances");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablTestReports");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablTestReports");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablTestReports");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablTestMethods");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablTestMethods");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablTestMethods");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablTechnicalRawDatas");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablTechnicalRawDatas");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablTechnicalRawDatas");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablSupplierConfidentialities");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablSupplierConfidentialities");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablSupplierConfidentialities");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablSkillMatrixDecisions");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablSkillMatrixDecisions");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablSkillMatrixDecisions");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablSkillMatrices");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablSampleMusterRegisters");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablSampleMusterRegisters");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablSampleMusterRegisters");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablSampleLabels");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablSampleLabels");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablSampleLabels");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablSampleInwardRegisters");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablSampleInwardRegisters");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablSampleInwardRegisters");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablRiskAssessments");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablResponsibilityAuthorities");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablResponsibilityAuthorities");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablResponsibilityAuthorities");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablPurchaseIndents");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablPurchaseIndents");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablPurchaseIndents");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablPtIlcPlans");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablPtIlcPlans");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablPtIlcPlans");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablProductInspections");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablProductInspections");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablProductInspections");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablMethodVerifications");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablMethodValidations");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablMeetingMinutes");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablMeetingMinutes");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablMeetingMinutes");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablMeetingAgendas");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablMeetingAgendas");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablMeetingAgendas");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablMeasurementUncertainties");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablJobDescriptions");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablJobDescriptions");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablJobDescriptions");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablInternalAuditors");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablIntermediateChecks");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablInductionTrainings");

            migrationBuilder.DropColumn(
                name: "PerformanceLevel",
                table: "NablInductionTrainings");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablInductionTrainings");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablInductionTrainings");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablIncomingMaterials");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablFormRevisionHistory");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablEquipmentHistories");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablEquipmentHistories");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablEquipmentHistories");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablEmployeePerformanceRecords");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablEmployeePerformanceRecords");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablEmployeePerformanceRecords");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablEmployeeCompetences");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablEmployeeCompetences");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablEmployeeCompetences");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablEmployeeAuthorizations");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablEmployeeAuthorizations");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablEmployeeAuthorizations");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablCompetenceRequirements");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablCompetenceRequirements");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablCompetenceRequirements");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablCalibrationReviews");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablCalibrationReviews");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablCalibrationReviews");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablAuditSummaries");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablAuditSummaries");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablAuditSummaries");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablAuditChecklists");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablAuditChecklists");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablAuditChecklists");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "AgreementDate",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "BlacklistDate",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "RevisionReason",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "RevisionType",
                table: "NablApprovedSuppliers");
        }
    }
}
