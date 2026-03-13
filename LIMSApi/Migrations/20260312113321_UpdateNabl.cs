using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNabl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NablComplianceChecks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CheckType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReferenceEntityID = table.Column<long>(type: "bigint", nullable: true),
                    ReferenceEntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablComplianceChecks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablFormTemplates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SchemaJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFormTemplates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablFolders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParentFolderID = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    NablFormTemplateID = table.Column<long>(type: "bigint", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFolders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablFolders_NablFolders_ParentFolderID",
                        column: x => x.ParentFolderID,
                        principalTable: "NablFolders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NablFolders_NablFormTemplates_NablFormTemplateID",
                        column: x => x.NablFormTemplateID,
                        principalTable: "NablFormTemplates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "NablFormDataEntries",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NablFormTemplateID = table.Column<long>(type: "bigint", nullable: false),
                    DataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReviewedBy = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataSnapshotAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFormDataEntries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablFormDataEntries_NablFormTemplates_NablFormTemplateID",
                        column: x => x.NablFormTemplateID,
                        principalTable: "NablFormTemplates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NablFormAttachments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NablFormDataID = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    FileCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.ForeignKey(
                        name: "FK_NablFormAttachments_NablFormDataEntries_NablFormDataID",
                        column: x => x.NablFormDataID,
                        principalTable: "NablFormDataEntries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "NablFormTemplates",
                columns: new[] { "ID", "Category", "CompanyCode", "CreatedBy", "CreatedOn", "Description", "FormCode", "FormType", "IsActive", "ModifiedBy", "ModifiedOn", "SchemaJson", "SortOrder", "Title", "Version" },
                values: new object[,]
                {
                    { 1L, "Application", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-1", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 1, "Application for Accreditation", 1 },
                    { 2L, "Organization", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-2", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 2, "Organization Chart", 1 },
                    { 3L, "Personnel", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-3", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"EmployeeName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"EmployeeMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Employee Name\",\"Section\":\"Personnel\",\"SortOrder\":1},{\"FieldName\":\"Designation\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"DesignationMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Designation\",\"Section\":\"Personnel\",\"SortOrder\":2},{\"FieldName\":\"Department\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"DepartmentMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Department\",\"Section\":\"Personnel\",\"SortOrder\":3},{\"FieldName\":\"Qualification\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Qualification\",\"Section\":\"Personnel\",\"SortOrder\":4},{\"FieldName\":\"Experience\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Experience (Years)\",\"Section\":\"Personnel\",\"SortOrder\":5},{\"FieldName\":\"DateOfJoining\",\"FieldType\":\"date\",\"Required\":false,\"Label\":\"Date of Joining\",\"Section\":\"Personnel\",\"SortOrder\":6}]", 3, "List of Key Managerial and Technical Personnel", 1 },
                    { 4L, "Equipment", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-4", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"EquipmentName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"EquipmentMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Equipment Name\",\"Section\":\"Equipment\",\"SortOrder\":1},{\"FieldName\":\"EquipmentNo\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"EquipmentMaster\",\"AutoSourceField\":\"EquipmentNo\",\"Label\":\"Equipment No\",\"Section\":\"Equipment\",\"SortOrder\":2},{\"FieldName\":\"ModelNo\",\"FieldType\":\"auto\",\"Required\":false,\"AutoSource\":\"EquipmentMaster\",\"AutoSourceField\":\"ModelNo\",\"Label\":\"Model No\",\"Section\":\"Equipment\",\"SortOrder\":3},{\"FieldName\":\"CalibrationStatus\",\"FieldType\":\"auto\",\"Required\":false,\"AutoSource\":\"EquipmentMaster\",\"AutoSourceField\":\"CalibrationRequired\",\"Label\":\"Calibration Required\",\"Section\":\"Equipment\",\"SortOrder\":4},{\"FieldName\":\"NextCalibrationDue\",\"FieldType\":\"date\",\"Required\":false,\"Label\":\"Next Calibration Due\",\"Section\":\"Equipment\",\"SortOrder\":5}]", 4, "List of Major Equipment and Instruments", 1 },
                    { 5L, "Equipment", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-5", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"EquipmentName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"EquipmentMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Equipment\",\"Section\":\"Calibration\",\"SortOrder\":1},{\"FieldName\":\"CalibrationDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Calibration Date\",\"Section\":\"Calibration\",\"SortOrder\":2},{\"FieldName\":\"DueDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Due Date\",\"Section\":\"Calibration\",\"SortOrder\":3},{\"FieldName\":\"Agency\",\"FieldType\":\"auto\",\"Required\":false,\"AutoSource\":\"CalibrationAgencyMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Calibration Agency\",\"Section\":\"Calibration\",\"SortOrder\":4},{\"FieldName\":\"CertificateNo\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Certificate No\",\"Section\":\"Calibration\",\"SortOrder\":5},{\"FieldName\":\"InternalExternal\",\"FieldType\":\"dropdown\",\"Required\":true,\"Label\":\"Internal/External\",\"Section\":\"Calibration\",\"SortOrder\":6,\"Options\":[\"Internal\",\"External\"]}]", 5, "Details of Equipment Calibration", 1 },
                    { 6L, "Equipment", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-6", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"StandardName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"TestMethodStandard\",\"AutoSourceField\":\"Name\",\"Label\":\"Standard Name\",\"Section\":\"Standards\",\"SortOrder\":1},{\"FieldName\":\"StandardCode\",\"FieldType\":\"auto\",\"Required\":false,\"AutoSource\":\"TestMethodStandard\",\"AutoSourceField\":\"TestMethodCode\",\"Label\":\"Standard Code\",\"Section\":\"Standards\",\"SortOrder\":2},{\"FieldName\":\"Year\",\"FieldType\":\"auto\",\"Required\":false,\"AutoSource\":\"TestMethodStandard\",\"AutoSourceField\":\"Year\",\"Label\":\"Year\",\"Section\":\"Standards\",\"SortOrder\":3},{\"FieldName\":\"Organization\",\"FieldType\":\"auto\",\"Required\":false,\"AutoSource\":\"StandardOrganizationMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Organization\",\"Section\":\"Standards\",\"SortOrder\":4}]", 6, "List of Reference Standards and Reference Materials", 1 },
                    { 7L, "Testing", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-7", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"TestMethodName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"TestMethodStandard\",\"AutoSourceField\":\"Name\",\"Label\":\"Test Method\",\"Section\":\"Methods\",\"SortOrder\":1},{\"FieldName\":\"TestMethodCode\",\"FieldType\":\"auto\",\"Required\":false,\"AutoSource\":\"TestMethodStandard\",\"AutoSourceField\":\"TestMethodCode\",\"Label\":\"Method Code\",\"Section\":\"Methods\",\"SortOrder\":2},{\"FieldName\":\"TestCategory\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Test Category\",\"Section\":\"Methods\",\"SortOrder\":3},{\"FieldName\":\"UnderNABL\",\"FieldType\":\"checkbox\",\"Required\":false,\"Label\":\"Under NABL\",\"Section\":\"Methods\",\"SortOrder\":4}]", 7, "List of Standard Test Methods", 1 },
                    { 8L, "Testing", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-8", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"CaseNo\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"SampleInward\",\"AutoSourceField\":\"CaseNo\",\"Label\":\"Case No\",\"Section\":\"Sample\",\"SortOrder\":1},{\"FieldName\":\"CustomerName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"Customer\",\"AutoSourceField\":\"Name\",\"Label\":\"Customer\",\"Section\":\"Sample\",\"SortOrder\":2},{\"FieldName\":\"SampleDescription\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Sample Description\",\"Section\":\"Sample\",\"SortOrder\":3},{\"FieldName\":\"DateOfReceipt\",\"FieldType\":\"date\",\"Required\":false,\"Label\":\"Date of Receipt\",\"Section\":\"Sample\",\"SortOrder\":4},{\"FieldName\":\"Condition\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Condition of Sample\",\"Section\":\"Sample\",\"SortOrder\":5}]", 8, "Sample Record Format", 1 },
                    { 9L, "Testing", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-9", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"ProgramName\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"PT Program Name\",\"Section\":\"PT\",\"SortOrder\":1},{\"FieldName\":\"Organizer\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Organizer\",\"Section\":\"PT\",\"SortOrder\":2},{\"FieldName\":\"Parameter\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Parameter Tested\",\"Section\":\"PT\",\"SortOrder\":3},{\"FieldName\":\"DateOfParticipation\",\"FieldType\":\"date\",\"Required\":false,\"Label\":\"Date of Participation\",\"Section\":\"PT\",\"SortOrder\":4},{\"FieldName\":\"Result\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Result\",\"Section\":\"PT\",\"SortOrder\":5},{\"FieldName\":\"ZScore\",\"FieldType\":\"number\",\"Required\":false,\"Label\":\"Z-Score\",\"Section\":\"PT\",\"SortOrder\":6}]", 9, "Proficiency Testing and Inter-Laboratory Comparison", 1 },
                    { 10L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-10", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"ComplaintDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Date of Complaint\",\"Section\":\"Complaint\",\"SortOrder\":1},{\"FieldName\":\"CustomerName\",\"FieldType\":\"dropdown\",\"Required\":true,\"AutoSource\":\"Customer\",\"Label\":\"Customer\",\"Section\":\"Complaint\",\"SortOrder\":2},{\"FieldName\":\"Description\",\"FieldType\":\"textarea\",\"Required\":true,\"Label\":\"Complaint Description\",\"Section\":\"Complaint\",\"SortOrder\":3},{\"FieldName\":\"RootCause\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Root Cause\",\"Section\":\"Investigation\",\"SortOrder\":4},{\"FieldName\":\"CorrectiveAction\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Corrective Action\",\"Section\":\"Investigation\",\"SortOrder\":5},{\"FieldName\":\"Status\",\"FieldType\":\"dropdown\",\"Required\":true,\"Label\":\"Status\",\"Section\":\"Investigation\",\"SortOrder\":6,\"Options\":[\"Open\",\"InProgress\",\"Closed\"]}]", 10, "Customer Complaint Record", 1 },
                    { 11L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-11", "Mixed", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"AuditDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Audit Date\",\"Section\":\"Schedule\",\"SortOrder\":1},{\"FieldName\":\"Auditor\",\"FieldType\":\"dropdown\",\"Required\":true,\"AutoSource\":\"EmployeeMaster\",\"Label\":\"Auditor\",\"Section\":\"Schedule\",\"SortOrder\":2},{\"FieldName\":\"Department\",\"FieldType\":\"dropdown\",\"Required\":true,\"AutoSource\":\"DepartmentMaster\",\"Label\":\"Department\",\"Section\":\"Schedule\",\"SortOrder\":3},{\"FieldName\":\"Findings\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Findings\",\"Section\":\"Report\",\"SortOrder\":4},{\"FieldName\":\"NonConformities\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Non-Conformities\",\"Section\":\"Report\",\"SortOrder\":5},{\"FieldName\":\"CorrectiveActions\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Corrective Actions\",\"Section\":\"Report\",\"SortOrder\":6}]", 11, "Internal Audit Schedule and Report", 1 },
                    { 12L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-12", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"ReviewDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Review Date\",\"Section\":\"Review\",\"SortOrder\":1},{\"FieldName\":\"Attendees\",\"FieldType\":\"textarea\",\"Required\":true,\"Label\":\"Attendees\",\"Section\":\"Review\",\"SortOrder\":2},{\"FieldName\":\"AgendaItems\",\"FieldType\":\"textarea\",\"Required\":true,\"Label\":\"Agenda Items\",\"Section\":\"Review\",\"SortOrder\":3},{\"FieldName\":\"Decisions\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Decisions Taken\",\"Section\":\"Review\",\"SortOrder\":4},{\"FieldName\":\"ActionItems\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Action Items\",\"Section\":\"Review\",\"SortOrder\":5}]", 12, "Management Review Record", 1 },
                    { 13L, "Personnel", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-13", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"EmployeeName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"EmployeeMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Employee\",\"Section\":\"Training\",\"SortOrder\":1},{\"FieldName\":\"TrainingTopic\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Training Topic\",\"Section\":\"Training\",\"SortOrder\":2},{\"FieldName\":\"TrainingDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Training Date\",\"Section\":\"Training\",\"SortOrder\":3},{\"FieldName\":\"Trainer\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Trainer\",\"Section\":\"Training\",\"SortOrder\":4},{\"FieldName\":\"Duration\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Duration (Hours)\",\"Section\":\"Training\",\"SortOrder\":5},{\"FieldName\":\"Assessment\",\"FieldType\":\"dropdown\",\"Required\":false,\"Label\":\"Assessment Result\",\"Section\":\"Training\",\"SortOrder\":6,\"Options\":[\"Pass\",\"Fail\",\"NA\"]}]", 13, "Training Records", 1 },
                    { 14L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-14", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"DocumentNo\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Document No\",\"Section\":\"Document\",\"SortOrder\":1},{\"FieldName\":\"DocumentTitle\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Document Title\",\"Section\":\"Document\",\"SortOrder\":2},{\"FieldName\":\"RevisionNo\",\"FieldType\":\"number\",\"Required\":true,\"Label\":\"Revision No\",\"Section\":\"Document\",\"SortOrder\":3},{\"FieldName\":\"EffectiveDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Effective Date\",\"Section\":\"Document\",\"SortOrder\":4},{\"FieldName\":\"ApprovedBy\",\"FieldType\":\"dropdown\",\"Required\":false,\"AutoSource\":\"EmployeeMaster\",\"Label\":\"Approved By\",\"Section\":\"Document\",\"SortOrder\":5},{\"FieldName\":\"Location\",\"FieldType\":\"text\",\"Required\":false,\"Label\":\"Storage Location\",\"Section\":\"Document\",\"SortOrder\":6}]", 14, "Document Control Register", 1 },
                    { 15L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-15", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 15, "Quality Manual", 1 },
                    { 16L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-16", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 16, "Quality Policy Statement", 1 },
                    { 17L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-17", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 17, "SOP for Sample Receipt and Handling", 1 },
                    { 18L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-18", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 18, "SOP for Equipment Handling and Maintenance", 1 },
                    { 19L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-19", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 19, "SOP for Calibration", 1 },
                    { 20L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-20", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 20, "SOP for Measurement Uncertainty", 1 },
                    { 21L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-21", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 21, "SOP for Method Validation", 1 },
                    { 22L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-22", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 22, "SOP for Internal Audit", 1 },
                    { 23L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-23", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 23, "SOP for Management Review", 1 },
                    { 24L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-24", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 24, "SOP for Corrective and Preventive Actions", 1 },
                    { 25L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-25", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 25, "SOP for Document Control", 1 },
                    { 26L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-26", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 26, "SOP for Complaints Handling", 1 },
                    { 27L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-27", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 27, "SOP for Non-Conforming Work", 1 },
                    { 28L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-28", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 28, "SOP for Proficiency Testing", 1 },
                    { 29L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-29", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 29, "SOP for Training", 1 },
                    { 30L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-30", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 30, "SOP for Subcontracting", 1 },
                    { 31L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-31", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 31, "SOP for Purchasing Services and Supplies", 1 },
                    { 32L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-32", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 32, "SOP for Customer Service", 1 },
                    { 33L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-33", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"Date\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Date\",\"Section\":\"Environment\",\"SortOrder\":1},{\"FieldName\":\"Temperature\",\"FieldType\":\"number\",\"Required\":true,\"Label\":\"Temperature (C)\",\"Section\":\"Environment\",\"SortOrder\":2},{\"FieldName\":\"Humidity\",\"FieldType\":\"number\",\"Required\":true,\"Label\":\"Humidity (%)\",\"Section\":\"Environment\",\"SortOrder\":3},{\"FieldName\":\"RecordedBy\",\"FieldType\":\"dropdown\",\"Required\":true,\"AutoSource\":\"EmployeeMaster\",\"Label\":\"Recorded By\",\"Section\":\"Environment\",\"SortOrder\":4}]", 33, "Environmental Condition Monitoring Record", 1 },
                    { 34L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-34", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"SupplierName\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Supplier Name\",\"Section\":\"Supplier\",\"SortOrder\":1},{\"FieldName\":\"MaterialSupplied\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Material/Service Supplied\",\"Section\":\"Supplier\",\"SortOrder\":2},{\"FieldName\":\"EvaluationDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Evaluation Date\",\"Section\":\"Supplier\",\"SortOrder\":3},{\"FieldName\":\"Rating\",\"FieldType\":\"dropdown\",\"Required\":true,\"Label\":\"Rating\",\"Section\":\"Supplier\",\"SortOrder\":4,\"Options\":[\"Approved\",\"Conditional\",\"Rejected\"]}]", 34, "Supplier Evaluation Record", 1 },
                    { 35L, "Equipment", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-35", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"EquipmentName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"EquipmentMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Equipment\",\"Section\":\"Maintenance\",\"SortOrder\":1},{\"FieldName\":\"MaintenanceType\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Maintenance Type\",\"Section\":\"Maintenance\",\"SortOrder\":2},{\"FieldName\":\"Frequency\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Frequency\",\"Section\":\"Maintenance\",\"SortOrder\":3},{\"FieldName\":\"LastDone\",\"FieldType\":\"date\",\"Required\":false,\"Label\":\"Last Done\",\"Section\":\"Maintenance\",\"SortOrder\":4},{\"FieldName\":\"NextDue\",\"FieldType\":\"date\",\"Required\":false,\"Label\":\"Next Due\",\"Section\":\"Maintenance\",\"SortOrder\":5}]", 35, "Preventive Maintenance Schedule", 1 },
                    { 36L, "Equipment", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-36", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 36, "Intermediate Check Record", 1 },
                    { 37L, "Testing", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-37", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 37, "Method Validation Report", 1 },
                    { 38L, "Testing", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-38", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 38, "Measurement Uncertainty Report", 1 },
                    { 39L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-39", "Mixed", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 39, "Quality Control Charts", 1 },
                    { 40L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-40", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"NCDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Date\",\"Section\":\"NC\",\"SortOrder\":1},{\"FieldName\":\"Description\",\"FieldType\":\"textarea\",\"Required\":true,\"Label\":\"Description of NC\",\"Section\":\"NC\",\"SortOrder\":2},{\"FieldName\":\"RootCause\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Root Cause\",\"Section\":\"NC\",\"SortOrder\":3},{\"FieldName\":\"CorrectiveAction\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Corrective Action\",\"Section\":\"NC\",\"SortOrder\":4},{\"FieldName\":\"Status\",\"FieldType\":\"dropdown\",\"Required\":true,\"Label\":\"Status\",\"Section\":\"NC\",\"SortOrder\":5,\"Options\":[\"Open\",\"InProgress\",\"Closed\"]}]", 40, "Non-Conformance Report", 1 },
                    { 41L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-41", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 41, "Corrective Action Request", 1 },
                    { 42L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-42", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 42, "Preventive Action Record", 1 },
                    { 43L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-43", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 43, "Risk and Opportunity Assessment", 1 },
                    { 44L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-44", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"CustomerName\",\"FieldType\":\"dropdown\",\"Required\":true,\"AutoSource\":\"Customer\",\"Label\":\"Customer\",\"Section\":\"Feedback\",\"SortOrder\":1},{\"FieldName\":\"FeedbackDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Feedback Date\",\"Section\":\"Feedback\",\"SortOrder\":2},{\"FieldName\":\"ServiceRating\",\"FieldType\":\"dropdown\",\"Required\":true,\"Label\":\"Service Rating\",\"Section\":\"Feedback\",\"SortOrder\":3,\"Options\":[\"Excellent\",\"Good\",\"Average\",\"Poor\"]},{\"FieldName\":\"Comments\",\"FieldType\":\"textarea\",\"Required\":false,\"Label\":\"Comments\",\"Section\":\"Feedback\",\"SortOrder\":4}]", 44, "Customer Feedback Form", 1 },
                    { 45L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-45", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 45, "Confidentiality and Impartiality Statement", 1 },
                    { 46L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-46", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 46, "SOP for Data Integrity and Protection", 1 },
                    { 47L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-47", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 47, "Laboratory Safety Manual", 1 },
                    { 48L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-48", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 48, "Waste Disposal Record", 1 },
                    { 49L, "Procedures", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-49", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 49, "SOP for Sample Retention and Disposal", 1 },
                    { 50L, "Personnel", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-50", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[{\"FieldName\":\"EmployeeName\",\"FieldType\":\"auto\",\"Required\":true,\"AutoSource\":\"EmployeeMaster\",\"AutoSourceField\":\"Name\",\"Label\":\"Employee\",\"Section\":\"Competency\",\"SortOrder\":1},{\"FieldName\":\"AssessmentDate\",\"FieldType\":\"date\",\"Required\":true,\"Label\":\"Assessment Date\",\"Section\":\"Competency\",\"SortOrder\":2},{\"FieldName\":\"Skill\",\"FieldType\":\"text\",\"Required\":true,\"Label\":\"Skill/Competency Area\",\"Section\":\"Competency\",\"SortOrder\":3},{\"FieldName\":\"Rating\",\"FieldType\":\"dropdown\",\"Required\":true,\"Label\":\"Rating\",\"Section\":\"Competency\",\"SortOrder\":4,\"Options\":[\"Competent\",\"NeedsTraining\",\"NotCompetent\"]}]", 50, "Competency Assessment Record", 1 },
                    { 51L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-51", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 51, "Sub-Contractor Evaluation Record", 1 },
                    { 52L, "Testing", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-52", "Checklist", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 52, "Report Verification Checklist", 1 },
                    { 53L, "Testing", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-53", "Tabular", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 53, "Sample Chain of Custody Record", 1 },
                    { 54L, "Quality", "LIMS", 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "F-54", "Document", true, 0L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "[]", 54, "Annual Quality Objectives and Review", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NablFolders_NablFormTemplateID",
                table: "NablFolders",
                column: "NablFormTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_NablFolders_ParentFolderID",
                table: "NablFolders",
                column: "ParentFolderID");

            migrationBuilder.CreateIndex(
                name: "IX_NablFormAttachments_NablFormDataID",
                table: "NablFormAttachments",
                column: "NablFormDataID");

            migrationBuilder.CreateIndex(
                name: "IX_NablFormDataEntries_NablFormTemplateID",
                table: "NablFormDataEntries",
                column: "NablFormTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_NablFormTemplates_FormCode",
                table: "NablFormTemplates",
                column: "FormCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NablComplianceChecks");

            migrationBuilder.DropTable(
                name: "NablFolders");

            migrationBuilder.DropTable(
                name: "NablFormAttachments");

            migrationBuilder.DropTable(
                name: "NablFormDataEntries");

            migrationBuilder.DropTable(
                name: "NablFormTemplates");
        }
    }
}
