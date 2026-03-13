using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NABLJobDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NablAuditLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormDataId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedBy = table.Column<long>(type: "bigint", nullable: false),
                    PerformedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAuditLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablFormAttachments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormDataId = table.Column<long>(type: "bigint", nullable: false),
                    FormType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFormAttachments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablFormRevisionHistory",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormDataId = table.Column<long>(type: "bigint", nullable: false),
                    FormType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RevisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevisedBy = table.Column<long>(type: "bigint", nullable: false),
                    RevisedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFormRevisionHistory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablJobDescriptions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationId = table.Column<long>(type: "bigint", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReportingTo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MinimumQualification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TechnicalTraining = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Experience = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrincipalAccountabilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorityStopTesting = table.Column<bool>(type: "bit", nullable: false),
                    AuthorityIssueReports = table.Column<bool>(type: "bit", nullable: false),
                    AuthorityAccessConfidential = table.Column<bool>(type: "bit", nullable: false),
                    AuthorityEquipmentCalibration = table.Column<bool>(type: "bit", nullable: false),
                    QmsResponsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfidentialityClause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreparedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmployeeAccepted = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_NablJobDescriptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablJobDescriptions_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NablJobDescriptions_DesignationMasters_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NablJobDescriptions_DepartmentId",
                table: "NablJobDescriptions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablJobDescriptions_DesignationId",
                table: "NablJobDescriptions",
                column: "DesignationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NablAuditLogs");

            migrationBuilder.DropTable(
                name: "NablFormAttachments");

            migrationBuilder.DropTable(
                name: "NablFormRevisionHistory");

            migrationBuilder.DropTable(
                name: "NablJobDescriptions");
        }
    }
}
