using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicReportFormatSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportFormats",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormatCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormatName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PageLayout = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PageSize = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HeaderConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFormats", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedReports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportFormatID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificateNo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GeneratedReports_ReportFormats_ReportFormatID",
                        column: x => x.ReportFormatID,
                        principalTable: "ReportFormats",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_GeneratedReports_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportFormatMappings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportFormatID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodID = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFormatMappings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportFormatMappings_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReportFormatMappings_ReportFormats_ReportFormatID",
                        column: x => x.ReportFormatID,
                        principalTable: "ReportFormats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportFormatMappings_TestMethodSpecifications_TestMethodID",
                        column: x => x.TestMethodID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReportFormatSections",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportFormatID = table.Column<long>(type: "bigint", nullable: false),
                    SectionType = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFormatSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportFormatSections_ReportFormats_ReportFormatID",
                        column: x => x.ReportFormatID,
                        principalTable: "ReportFormats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_ReportFormatID",
                table: "GeneratedReports",
                column: "ReportFormatID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_SampleID",
                table: "GeneratedReports",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormatMappings_LaboratoryTestID",
                table: "ReportFormatMappings",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormatMappings_ReportFormatID",
                table: "ReportFormatMappings",
                column: "ReportFormatID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormatMappings_TestMethodID",
                table: "ReportFormatMappings",
                column: "TestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormat_FormatCode",
                table: "ReportFormats",
                column: "FormatCode",
                unique: true,
                filter: "[IsActive] = 1 AND [FormatCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormatSections_ReportFormatID",
                table: "ReportFormatSections",
                column: "ReportFormatID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneratedReports");

            migrationBuilder.DropTable(
                name: "ReportFormatMappings");

            migrationBuilder.DropTable(
                name: "ReportFormatSections");

            migrationBuilder.DropTable(
                name: "ReportFormats");
        }
    }
}
