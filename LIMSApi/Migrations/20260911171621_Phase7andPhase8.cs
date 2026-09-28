using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase7andPhase8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniversalTestResults",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestExecutionID = table.Column<long>(type: "bigint", nullable: false),
                    UniversalTestGroupID = table.Column<long>(type: "bigint", nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: false),
                    RevisionNo = table.Column<int>(type: "int", nullable: false),
                    ResultStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OverallDecision = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SnapshotHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExecutionConfigSnapshotID = table.Column<long>(type: "bigint", nullable: true),
                    DecisionRule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AcceptanceCriteriaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CalculationTraceJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComplianceSummaryJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalizedBy = table.Column<long>(type: "bigint", nullable: true),
                    FinalizedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewerID = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewRemarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalTestResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UniversalTestResults_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalTestResults_TestExecutions_TestExecutionID",
                        column: x => x.TestExecutionID,
                        principalTable: "TestExecutions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalTestResults_UniversalTestGroups_UniversalTestGroupID",
                        column: x => x.UniversalTestGroupID,
                        principalTable: "UniversalTestGroups",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "UniversalResultAudits",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversalTestResultID = table.Column<long>(type: "bigint", nullable: false),
                    TestExecutionID = table.Column<long>(type: "bigint", nullable: false),
                    RevisionNo = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActorID = table.Column<long>(type: "bigint", nullable: false),
                    ActorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EventOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SnapshotHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalResultAudits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UniversalResultAudits_UniversalTestResults_UniversalTestResultID",
                        column: x => x.UniversalTestResultID,
                        principalTable: "UniversalTestResults",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniversalReviewFindings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversalTestResultID = table.Column<long>(type: "bigint", nullable: false),
                    TestExecutionID = table.Column<long>(type: "bigint", nullable: false),
                    FindingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsBlocking = table.Column<bool>(type: "bit", nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResolvedBy = table.Column<long>(type: "bigint", nullable: true),
                    ResolvedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalReviewFindings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UniversalReviewFindings_UniversalTestResults_UniversalTestResultID",
                        column: x => x.UniversalTestResultID,
                        principalTable: "UniversalTestResults",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniversalTestResultParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversalTestResultID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterMasterID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InputType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DecimalPrecision = table.Column<int>(type: "int", nullable: false),
                    IsCalculated = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    RawValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RawNumericValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    AppliedFactorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppliedFactorOperation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FactoredValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Formula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubstitutionTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculatedValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ComplianceValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    DisplayValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReportedValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpecMin = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    SpecMax = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    SpecTarget = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MinTolerance = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MaxTolerance = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    RequirementStatus = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Verdict = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CombinedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ExpandedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CoverageFactor = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    GuardBandApplied = table.Column<bool>(type: "bit", nullable: false),
                    EvaluationNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalTestResultParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UniversalTestResultParameters_UniversalTestResults_UniversalTestResultID",
                        column: x => x.UniversalTestResultID,
                        principalTable: "UniversalTestResults",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniversalResultAudits_TestExecutionID_EventType",
                table: "UniversalResultAudits",
                columns: new[] { "TestExecutionID", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_UniversalResultAudits_UniversalTestResultID",
                table: "UniversalResultAudits",
                column: "UniversalTestResultID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalReviewFindings_UniversalTestResultID",
                table: "UniversalReviewFindings",
                column: "UniversalTestResultID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalReviewFindings_UniversalTestResultID_Status",
                table: "UniversalReviewFindings",
                columns: new[] { "UniversalTestResultID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestResultParameters_UniversalTestResultID",
                table: "UniversalTestResultParameters",
                column: "UniversalTestResultID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestResultParameters_UniversalTestResultID_ParameterMasterID",
                table: "UniversalTestResultParameters",
                columns: new[] { "UniversalTestResultID", "ParameterMasterID" });

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestResults_BranchID",
                table: "UniversalTestResults",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestResults_CompanyCode_BranchID",
                table: "UniversalTestResults",
                columns: new[] { "CompanyCode", "BranchID" });

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestResults_Execution_Revision",
                table: "UniversalTestResults",
                columns: new[] { "TestExecutionID", "RevisionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestResults_ResultStatus",
                table: "UniversalTestResults",
                column: "ResultStatus");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestResults_UniversalTestGroupID",
                table: "UniversalTestResults",
                column: "UniversalTestGroupID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniversalResultAudits");

            migrationBuilder.DropTable(
                name: "UniversalReviewFindings");

            migrationBuilder.DropTable(
                name: "UniversalTestResultParameters");

            migrationBuilder.DropTable(
                name: "UniversalTestResults");
        }
    }
}
