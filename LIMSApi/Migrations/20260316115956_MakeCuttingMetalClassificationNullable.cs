using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeCuttingMetalClassificationNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuttingChargeSamples_MetalClassificationMasters_MetalClassificationID",
                table: "CuttingChargeSamples");

            migrationBuilder.AddColumn<bool>(
                name: "IsBillable",
                table: "TestResultParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "MetalClassificationID",
                table: "CuttingChargeSamples",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "NablEmployeePerformanceRecords",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReviewPeriod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TechnicalRating = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    BehavioralRating = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    OverallRating = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    ReviewerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_NablEmployeePerformanceRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEmployeePerformanceRecords_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeePerformanceRecords_EmployeeId",
                table: "NablEmployeePerformanceRecords",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CuttingChargeSamples_MetalClassificationMasters_MetalClassificationID",
                table: "CuttingChargeSamples",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuttingChargeSamples_MetalClassificationMasters_MetalClassificationID",
                table: "CuttingChargeSamples");

            migrationBuilder.DropTable(
                name: "NablEmployeePerformanceRecords");

            migrationBuilder.DropColumn(
                name: "IsBillable",
                table: "TestResultParameters");

            migrationBuilder.AlterColumn<long>(
                name: "MetalClassificationID",
                table: "CuttingChargeSamples",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CuttingChargeSamples_MetalClassificationMasters_MetalClassificationID",
                table: "CuttingChargeSamples",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
