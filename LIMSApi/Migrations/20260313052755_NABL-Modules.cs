using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NABLModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NablApprovedSuppliers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemsApproved = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalValidUpto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PerformanceRating = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablApprovedSuppliers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablAuditPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditYear = table.Column<int>(type: "int", nullable: true),
                    AuditScheduleJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditObjective = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeadAuditorId = table.Column<long>(type: "bigint", nullable: true),
                    LeadAuditorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAuditPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablAuditPlans_EmployeeMasters_LeadAuditorId",
                        column: x => x.LeadAuditorId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablCalibrationReviews",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EquipmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalibrationAgencyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CalibrationDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalibrationResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CalibrationDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablCalibrationReviews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablCalibrationReviews_EquipmentMasters_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablCompetenceRequirements",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<long>(type: "bigint", nullable: true),
                    PositionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MinimumEducation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinimumExperience = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false),
                    RelatedActivity = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablCompetenceRequirements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablCompetenceRequirements_DesignationMasters_PositionId",
                        column: x => x.PositionId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablComplaints",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ComplaintDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComplaintDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComplaintCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SampleCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReportNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvestigationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreventiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerInformedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerSatisfied = table.Column<bool>(type: "bit", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablComplaints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablComplaints_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablCustomerFeedbacks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeedbackPeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeedbackPeriodTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OverallSatisfaction = table.Column<int>(type: "int", nullable: true),
                    TurnaroundRating = table.Column<int>(type: "int", nullable: true),
                    AccuracyRating = table.Column<int>(type: "int", nullable: true),
                    CommunicationRating = table.Column<int>(type: "int", nullable: true),
                    ServiceRating = table.Column<int>(type: "int", nullable: true),
                    CommentsSuggestions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WouldRecommend = table.Column<bool>(type: "bit", nullable: true),
                    CollectedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablCustomerFeedbacks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablCustomerFeedbacks_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablDocumentChangeRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChangeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForChange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UrgencyLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssessedImpact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssessmentBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Disposition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImplementationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablDocumentChangeRequests", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablDocumentReviews",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentRevision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeRequired = table.Column<bool>(type: "bit", nullable: true),
                    ChangeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablDocumentReviews", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablEmployeeAuthorizations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    PersonnelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Uid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestMethodAuthorization = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestAuthorization = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEmployeeAuthorizations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEmployeeAuthorizations_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablEmployeeAuthorizations_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablEmployeeCompetences",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EvaluationPeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationPeriodTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpecificTrainingRequired = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EvaluationDoneBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEmployeeCompetences", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEmployeeCompetences_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablEnvironmentMonitorings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MonitoringDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeOfReading = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AcceptableTemperatureMin = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AcceptableTemperatureMax = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AcceptableHumidityMin = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AcceptableHumidityMax = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    IsWithinLimits = table.Column<bool>(type: "bit", nullable: false),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEnvironmentMonitorings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEnvironmentMonitorings_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablEquipmentHistories",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EquipmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModelNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CalibrationFrequency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastCalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextCalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalibrationAgency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaintenanceRecordsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEquipmentHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEquipmentHistories_EquipmentMasters_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablFeedbackAnalyses",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalysisPeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnalysisPeriodTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalFeedbacks = table.Column<int>(type: "int", nullable: true),
                    AverageSatisfaction = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AverageTurnaround = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AverageAccuracy = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AverageCommunication = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AverageService = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AcceptanceCriteria = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MeetsAcceptanceCriteria = table.Column<bool>(type: "bit", nullable: true),
                    KeyStrengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreasForImprovement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnalysedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFeedbackAnalyses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablIncomingMaterials",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InspectionResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablIncomingMaterials", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablIncomingMaterials_SupplierMasters_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablInductionTrainings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainerDesignation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleRefNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Parameter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestMethodSop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationMode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EvalParameter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvalTestMethodSop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ObservedValue1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObservedValue2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObservedValueAverage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OriginalValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainerComments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablInductionTrainings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablInductionTrainings_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablIntermediateChecks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CheckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReferenceStandard = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ObservedValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    AcceptedValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Tolerance = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ResultStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CheckedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablIntermediateChecks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablIntermediateChecks_EquipmentMasters_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablInternalAuditors",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LeadAuditorCourse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LeadAuditorCertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InternalAuditorCourse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InternalAuditorCertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ISOClauses = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AuditExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizedAreas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthorizationValidUpto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthorizedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablInternalAuditors", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablInternalAuditors_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablMasterDocuments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentIssue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentRevision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentOwner = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ControlledCopiesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObsoleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMasterDocuments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablMeasurementUncertainties",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MatrixType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UncertaintyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SourcesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CombinedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ExpandedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CoverageFactor = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ConfidenceLevel = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValidatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMeasurementUncertainties", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablMeetingAgendas",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeetingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeetingVenue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChairpersonId = table.Column<long>(type: "bigint", nullable: true),
                    ChairpersonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AgendaItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttendeeIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttendeeNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousMOMRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMeetingAgendas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablMeetingAgendas_EmployeeMasters_ChairpersonId",
                        column: x => x.ChairpersonId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablMethodValidations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMatrix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValidationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidationScope = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SelectivityResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LinearityRange = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DetectionLimit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QuantificationLimit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PrecisionRSD = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    BiasPercentage = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    RobustnessResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UncertaintyResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OverallConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValidatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextValidationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMethodValidations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablMethodVerifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMatrix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LinearityResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrecisionResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BiasResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UncertaintyResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OverallConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextVerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMethodVerifications", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablNonConformingWorks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NCDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SampleCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NCDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NCSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DetectedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdentifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SuspendedWork = table.Column<bool>(type: "bit", nullable: true),
                    AffectedResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImmediateAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NCCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RootCauseAnalysis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablNonConformingWorks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablProductInspections",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleSize = table.Column<int>(type: "int", nullable: true),
                    DefectsFound = table.Column<int>(type: "int", nullable: true),
                    InspectionCriteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InspectionResultsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablProductInspections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablProductInspections_SupplierMasters_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablPtIlcPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanYear = table.Column<int>(type: "int", nullable: true),
                    PTType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrganizingBody = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ScheduleJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalParticipations = table.Column<int>(type: "int", nullable: true),
                    SatisfactoryResults = table.Column<int>(type: "int", nullable: true),
                    UnsatisfactoryResults = table.Column<int>(type: "int", nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OverallAssessment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablPtIlcPlans", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablPurchaseIndents",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Justification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablPurchaseIndents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablPurchaseIndents_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablQualityControlPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ControlType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FrequencyUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPerformedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionOnFailure = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablQualityControlPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablQualityControlPlans_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablReferenceMaterials",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RMCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RMName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StorageCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CertifiedValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Uncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    RemainingQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    QuantityUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablReferenceMaterials", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablResponsibilityAuthorities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationId = table.Column<long>(type: "bigint", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Responsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Authorities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeAccepted = table.Column<bool>(type: "bit", nullable: false),
                    AcceptanceTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeSignature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablResponsibilityAuthorities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablResponsibilityAuthorities_DesignationMasters_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablRetestings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OriginalTestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetestReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OriginalResult = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RetestResult = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RetestConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthorizedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablRetestings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablRiskAssessments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessArea = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RisksJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallRiskLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssessedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablRiskAssessments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSampleInwardRegisters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleInwardId = table.Column<long>(type: "bigint", nullable: true),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestsRequested = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TargetCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSampleInwardRegisters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSampleLabels",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SampleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LabelNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StorageCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LabelledBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSampleLabels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSampleMusterRegisters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SampleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MusteringDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MusteredBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NumberOfPieces = table.Column<int>(type: "int", nullable: true),
                    SampleDimensions = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CuttingInstructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreparedSamples = table.Column<int>(type: "int", nullable: true),
                    WasteGenerated = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisposalMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSampleMusterRegisters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSkillMatrices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationId = table.Column<long>(type: "bigint", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Decision = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SkillsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeSkillsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSkillMatrices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablSkillMatrices_DesignationMasters_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablSkillMatrixDecisions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationId = table.Column<long>(type: "bigint", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSkillMatrixDecisions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablSkillMatrixDecisions_DesignationMasters_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablSupplierConfidentialities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AgreementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgreementValidUpto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfidentialItems = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PenaltyClause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SupplierSignature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSupplierConfidentialities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSupplierEvaluations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalScore = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    MaxScore = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    PercentageScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    EvaluationResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextEvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSupplierEvaluations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSupplierRegistrations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SupplierCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemsSupplied = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationValidUpto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NablApproved = table.Column<bool>(type: "bit", nullable: false),
                    BankDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSupplierRegistrations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablTechnicalRawDatas",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    ObservationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObservationsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculatedResult = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Uncertainty = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RawDataFile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CheckedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTechnicalRawDatas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTechnicalRawDatas_EquipmentMasters_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodStandardId = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestMethodTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMatrix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Principle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApplicableStandard = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EquipmentRequired = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReagentsRequired = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SamplePreparation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Procedure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalibrationRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QualityControlRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UncertaintyStatement = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DetectionLimit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTestMethods_TestMethodStandards_TestMethodStandardId",
                        column: x => x.TestMethodStandardId,
                        principalTable: "TestMethodStandards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTestReports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestResultsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SamplingDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MethodReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Conclusion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Disclaimer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReportVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTestReports", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablTestRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SampleQuantity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReferenceStandard = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestPurpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTestRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTestRequests_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTrainingPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainingTopic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingObjective = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainerDesignation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VenueMode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NeedIdentifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompletionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTrainingPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTrainingPlans_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablAuditChecklists",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditPlanId = table.Column<long>(type: "bigint", nullable: true),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditorId = table.Column<long>(type: "bigint", nullable: true),
                    AuditorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditeeId = table.Column<long>(type: "bigint", nullable: true),
                    AuiteeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ISOClause = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChecklistItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NCCount = table.Column<int>(type: "int", nullable: true),
                    ObservationCount = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAuditChecklists", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablAuditChecklists_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablAuditChecklists_EmployeeMasters_AuditeeId",
                        column: x => x.AuditeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablAuditChecklists_EmployeeMasters_AuditorId",
                        column: x => x.AuditorId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablAuditChecklists_NablAuditPlans_AuditPlanId",
                        column: x => x.AuditPlanId,
                        principalTable: "NablAuditPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablAuditSummaries",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditPlanId = table.Column<long>(type: "bigint", nullable: true),
                    AuditDateFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditDateTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAudits = table.Column<int>(type: "int", nullable: true),
                    TotalNCs = table.Column<int>(type: "int", nullable: true),
                    MajorNCs = table.Column<int>(type: "int", nullable: true),
                    MinorNCs = table.Column<int>(type: "int", nullable: true),
                    Observations = table.Column<int>(type: "int", nullable: true),
                    FindingsSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositiveFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosureStatus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextAuditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SummaryBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAuditSummaries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablAuditSummaries_NablAuditPlans_AuditPlanId",
                        column: x => x.AuditPlanId,
                        principalTable: "NablAuditPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablMeetingMinutes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgendaId = table.Column<long>(type: "bigint", nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeetingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChairpersonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AttendeesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinutesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextMeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMeetingAgenda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionClosureStatus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMeetingMinutes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablMeetingMinutes_NablMeetingAgendas_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "NablMeetingAgendas",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablNcCorrectiveActions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NCId = table.Column<long>(type: "bigint", nullable: true),
                    NCRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CADate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreventiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImplementedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImplementationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EffectivenessEvaluated = table.Column<bool>(type: "bit", nullable: true),
                    EffectivenessResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Closed = table.Column<bool>(type: "bit", nullable: true),
                    ClosureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablNcCorrectiveActions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablNcCorrectiveActions_NablNonConformingWorks_NCId",
                        column: x => x.NCId,
                        principalTable: "NablNonConformingWorks",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablPurchaseOrders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PurchaseIndentId = table.Column<long>(type: "bigint", nullable: true),
                    PODate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablPurchaseOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablPurchaseOrders_NablPurchaseIndents_PurchaseIndentId",
                        column: x => x.PurchaseIndentId,
                        principalTable: "NablPurchaseIndents",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablPurchaseOrders_SupplierMasters_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablCrmConsumptions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceMaterialId = table.Column<long>(type: "bigint", nullable: true),
                    RMCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RMName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuantityUsed = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    QuantityUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PurposeOfUse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UsedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RemainingAfterUse = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsExhausted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablCrmConsumptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablCrmConsumptions_NablReferenceMaterials_ReferenceMaterialId",
                        column: x => x.ReferenceMaterialId,
                        principalTable: "NablReferenceMaterials",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTrainingAttendances",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainingPlanId = table.Column<long>(type: "bigint", nullable: true),
                    TrainingTopic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VenueMode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AttendeesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAttendees = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTrainingAttendances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTrainingAttendances_NablTrainingPlans_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "NablTrainingPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTrainingEffectiveness",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainingPlanId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainingTopic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KnowledgeScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    SkillScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    EffectivenessResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActionRequired = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReEvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTrainingEffectiveness", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTrainingEffectiveness_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablTrainingEffectiveness_NablTrainingPlans_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "NablTrainingPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablPurchaseMaterialVerifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: true),
                    PONumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemsVerificationJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GRNNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablPurchaseMaterialVerifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablPurchaseMaterialVerifications_NablPurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "NablPurchaseOrders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditChecklists_AuditeeId",
                table: "NablAuditChecklists",
                column: "AuditeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditChecklists_AuditorId",
                table: "NablAuditChecklists",
                column: "AuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditChecklists_AuditPlanId",
                table: "NablAuditChecklists",
                column: "AuditPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditChecklists_DepartmentId",
                table: "NablAuditChecklists",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditPlans_LeadAuditorId",
                table: "NablAuditPlans",
                column: "LeadAuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditSummaries_AuditPlanId",
                table: "NablAuditSummaries",
                column: "AuditPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NablCalibrationReviews_EquipmentId",
                table: "NablCalibrationReviews",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablCompetenceRequirements_PositionId",
                table: "NablCompetenceRequirements",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_NablComplaints_CustomerId",
                table: "NablComplaints",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NablCrmConsumptions_ReferenceMaterialId",
                table: "NablCrmConsumptions",
                column: "ReferenceMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_NablCustomerFeedbacks_CustomerId",
                table: "NablCustomerFeedbacks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeAuthorizations_DepartmentId",
                table: "NablEmployeeAuthorizations",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeAuthorizations_EmployeeId",
                table: "NablEmployeeAuthorizations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeCompetences_EmployeeId",
                table: "NablEmployeeCompetences",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEnvironmentMonitorings_DepartmentId",
                table: "NablEnvironmentMonitorings",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEquipmentHistories_EquipmentId",
                table: "NablEquipmentHistories",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablIncomingMaterials_SupplierId",
                table: "NablIncomingMaterials",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_NablInductionTrainings_EmployeeId",
                table: "NablInductionTrainings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablIntermediateChecks_EquipmentId",
                table: "NablIntermediateChecks",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablInternalAuditors_EmployeeId",
                table: "NablInternalAuditors",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablMeetingAgendas_ChairpersonId",
                table: "NablMeetingAgendas",
                column: "ChairpersonId");

            migrationBuilder.CreateIndex(
                name: "IX_NablMeetingMinutes_AgendaId",
                table: "NablMeetingMinutes",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_NablNcCorrectiveActions_NCId",
                table: "NablNcCorrectiveActions",
                column: "NCId");

            migrationBuilder.CreateIndex(
                name: "IX_NablProductInspections_SupplierId",
                table: "NablProductInspections",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseIndents_DepartmentId",
                table: "NablPurchaseIndents",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseMaterialVerifications_PurchaseOrderId",
                table: "NablPurchaseMaterialVerifications",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseOrders_PurchaseIndentId",
                table: "NablPurchaseOrders",
                column: "PurchaseIndentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseOrders_SupplierId",
                table: "NablPurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_NablQualityControlPlans_DepartmentId",
                table: "NablQualityControlPlans",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablResponsibilityAuthorities_DesignationId",
                table: "NablResponsibilityAuthorities",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablSkillMatrices_DesignationId",
                table: "NablSkillMatrices",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablSkillMatrixDecisions_DesignationId",
                table: "NablSkillMatrixDecisions",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTechnicalRawDatas_EquipmentId",
                table: "NablTechnicalRawDatas",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTestMethods_TestMethodStandardId",
                table: "NablTestMethods",
                column: "TestMethodStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTestRequests_CustomerId",
                table: "NablTestRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTrainingAttendances_TrainingPlanId",
                table: "NablTrainingAttendances",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTrainingEffectiveness_EmployeeId",
                table: "NablTrainingEffectiveness",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTrainingEffectiveness_TrainingPlanId",
                table: "NablTrainingEffectiveness",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTrainingPlans_EmployeeId",
                table: "NablTrainingPlans",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NablApprovedSuppliers");

            migrationBuilder.DropTable(
                name: "NablAuditChecklists");

            migrationBuilder.DropTable(
                name: "NablAuditSummaries");

            migrationBuilder.DropTable(
                name: "NablCalibrationReviews");

            migrationBuilder.DropTable(
                name: "NablCompetenceRequirements");

            migrationBuilder.DropTable(
                name: "NablComplaints");

            migrationBuilder.DropTable(
                name: "NablCrmConsumptions");

            migrationBuilder.DropTable(
                name: "NablCustomerFeedbacks");

            migrationBuilder.DropTable(
                name: "NablDocumentChangeRequests");

            migrationBuilder.DropTable(
                name: "NablDocumentReviews");

            migrationBuilder.DropTable(
                name: "NablEmployeeAuthorizations");

            migrationBuilder.DropTable(
                name: "NablEmployeeCompetences");

            migrationBuilder.DropTable(
                name: "NablEnvironmentMonitorings");

            migrationBuilder.DropTable(
                name: "NablEquipmentHistories");

            migrationBuilder.DropTable(
                name: "NablFeedbackAnalyses");

            migrationBuilder.DropTable(
                name: "NablIncomingMaterials");

            migrationBuilder.DropTable(
                name: "NablInductionTrainings");

            migrationBuilder.DropTable(
                name: "NablIntermediateChecks");

            migrationBuilder.DropTable(
                name: "NablInternalAuditors");

            migrationBuilder.DropTable(
                name: "NablMasterDocuments");

            migrationBuilder.DropTable(
                name: "NablMeasurementUncertainties");

            migrationBuilder.DropTable(
                name: "NablMeetingMinutes");

            migrationBuilder.DropTable(
                name: "NablMethodValidations");

            migrationBuilder.DropTable(
                name: "NablMethodVerifications");

            migrationBuilder.DropTable(
                name: "NablNcCorrectiveActions");

            migrationBuilder.DropTable(
                name: "NablProductInspections");

            migrationBuilder.DropTable(
                name: "NablPtIlcPlans");

            migrationBuilder.DropTable(
                name: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropTable(
                name: "NablQualityControlPlans");

            migrationBuilder.DropTable(
                name: "NablResponsibilityAuthorities");

            migrationBuilder.DropTable(
                name: "NablRetestings");

            migrationBuilder.DropTable(
                name: "NablRiskAssessments");

            migrationBuilder.DropTable(
                name: "NablSampleInwardRegisters");

            migrationBuilder.DropTable(
                name: "NablSampleLabels");

            migrationBuilder.DropTable(
                name: "NablSampleMusterRegisters");

            migrationBuilder.DropTable(
                name: "NablSkillMatrices");

            migrationBuilder.DropTable(
                name: "NablSkillMatrixDecisions");

            migrationBuilder.DropTable(
                name: "NablSupplierConfidentialities");

            migrationBuilder.DropTable(
                name: "NablSupplierEvaluations");

            migrationBuilder.DropTable(
                name: "NablSupplierRegistrations");

            migrationBuilder.DropTable(
                name: "NablTechnicalRawDatas");

            migrationBuilder.DropTable(
                name: "NablTestMethods");

            migrationBuilder.DropTable(
                name: "NablTestReports");

            migrationBuilder.DropTable(
                name: "NablTestRequests");

            migrationBuilder.DropTable(
                name: "NablTrainingAttendances");

            migrationBuilder.DropTable(
                name: "NablTrainingEffectiveness");

            migrationBuilder.DropTable(
                name: "NablAuditPlans");

            migrationBuilder.DropTable(
                name: "NablReferenceMaterials");

            migrationBuilder.DropTable(
                name: "NablMeetingAgendas");

            migrationBuilder.DropTable(
                name: "NablNonConformingWorks");

            migrationBuilder.DropTable(
                name: "NablPurchaseOrders");

            migrationBuilder.DropTable(
                name: "NablTrainingPlans");

            migrationBuilder.DropTable(
                name: "NablPurchaseIndents");
        }
    }
}
