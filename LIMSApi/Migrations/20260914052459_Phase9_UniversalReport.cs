using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase9_UniversalReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniversalReports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestExecutionID = table.Column<long>(type: "bigint", nullable: false),
                    UniversalTestResultID = table.Column<long>(type: "bigint", nullable: false),
                    ResultRevisionNo = table.Column<int>(type: "int", nullable: false),
                    ReportRevisionNo = table.Column<int>(type: "int", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SnapshotHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ResultRevisionHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReportDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportDataHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdfHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeneratedBy = table.Column<long>(type: "bigint", nullable: true),
                    ReleasedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UniversalReports_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalReports_TestExecutions_TestExecutionID",
                        column: x => x.TestExecutionID,
                        principalTable: "TestExecutions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalReports_UniversalTestResults_UniversalTestResultID",
                        column: x => x.UniversalTestResultID,
                        principalTable: "UniversalTestResults",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniversalReports_BranchID",
                table: "UniversalReports",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalReports_CompanyCode_BranchID",
                table: "UniversalReports",
                columns: new[] { "CompanyCode", "BranchID" });

            migrationBuilder.CreateIndex(
                name: "IX_UniversalReports_Execution_ResultRev_ReportRev",
                table: "UniversalReports",
                columns: new[] { "TestExecutionID", "ResultRevisionNo", "ReportRevisionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversalReports_ReportNo",
                table: "UniversalReports",
                column: "ReportNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversalReports_Status",
                table: "UniversalReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalReports_UniversalTestResultID",
                table: "UniversalReports",
                column: "UniversalTestResultID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniversalReports");
        }
    }
}
