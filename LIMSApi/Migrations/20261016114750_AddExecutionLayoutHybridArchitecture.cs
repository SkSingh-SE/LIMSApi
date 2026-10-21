using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddExecutionLayoutHybridArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormulaVersion",
                table: "TestResultParameters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReportable",
                table: "TestResultParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReportableOverrideReason",
                table: "TestResultParameters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "TestResultParameters",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalHash",
                table: "TestResultHeaders",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "TestResultHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApprovedById",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutionExtraJson",
                table: "TestResultHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LayoutSnapshotJson",
                table: "TestResultHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LayoutTemplateId",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LayoutVersion",
                table: "TestResultHeaders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportSnapshotJson",
                table: "TestResultHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RetestOfHeaderId",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RetestReasonCode",
                table: "TestResultHeaders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "TestResultHeaders",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TestResultAuditLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityID = table.Column<long>(type: "bigint", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangedBy = table.Column<long>(type: "bigint", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultAuditLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultAuditLogs_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResultChecklistItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CheckedBy = table.Column<long>(type: "bigint", nullable: true),
                    CheckedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    IsVoided = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultChecklistItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultChecklistItems_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResultLocations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    IsRuntimeAdded = table.Column<bool>(type: "bit", nullable: false),
                    AddedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    IsVoided = table.Column<bool>(type: "bit", nullable: false),
                    VoidReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultLocations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultLocations_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResultObservations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    ObservationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObservationText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRuntimeAdded = table.Column<bool>(type: "bit", nullable: false),
                    AddedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    IsVoided = table.Column<bool>(type: "bit", nullable: false),
                    VoidReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultObservations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultObservations_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResultReadings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    TestResultParameterID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SpecimenCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsVoided = table.Column<bool>(type: "bit", nullable: false),
                    VoidReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultReadings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultReadings_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResultReadings_TestResultParameters_TestResultParameterID",
                        column: x => x.TestResultParameterID,
                        principalTable: "TestResultParameters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TestResultTraversePoints",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    SpecimenCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    HardnessValue = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    HardnessUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsBelowCutoff = table.Column<bool>(type: "bit", nullable: false),
                    IsVoided = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultTraversePoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultTraversePoints_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_LayoutTemplateId",
                table: "TestResultHeaders",
                column: "LayoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_RetestOfHeaderId",
                table: "TestResultHeaders",
                column: "RetestOfHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultAuditLogs_TestResultHeaderID",
                table: "TestResultAuditLogs",
                column: "TestResultHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultChecklistItems_TestResultHeaderID",
                table: "TestResultChecklistItems",
                column: "TestResultHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultLocations_TestResultHeaderID",
                table: "TestResultLocations",
                column: "TestResultHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultObservations_TestResultHeaderID",
                table: "TestResultObservations",
                column: "TestResultHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultReadings_TestResultHeaderID",
                table: "TestResultReadings",
                column: "TestResultHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultReadings_TestResultParameterID",
                table: "TestResultReadings",
                column: "TestResultParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultTraversePoints_TestResultHeaderID",
                table: "TestResultTraversePoints",
                column: "TestResultHeaderID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultHeaders_TestLayoutTemplates_LayoutTemplateId",
                table: "TestResultHeaders",
                column: "LayoutTemplateId",
                principalTable: "TestLayoutTemplates",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultHeaders_TestResultHeaders_RetestOfHeaderId",
                table: "TestResultHeaders",
                column: "RetestOfHeaderId",
                principalTable: "TestResultHeaders",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResultHeaders_TestLayoutTemplates_LayoutTemplateId",
                table: "TestResultHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResultHeaders_TestResultHeaders_RetestOfHeaderId",
                table: "TestResultHeaders");

            migrationBuilder.DropTable(
                name: "TestResultAuditLogs");

            migrationBuilder.DropTable(
                name: "TestResultChecklistItems");

            migrationBuilder.DropTable(
                name: "TestResultLocations");

            migrationBuilder.DropTable(
                name: "TestResultObservations");

            migrationBuilder.DropTable(
                name: "TestResultReadings");

            migrationBuilder.DropTable(
                name: "TestResultTraversePoints");

            migrationBuilder.DropIndex(
                name: "IX_TestResultHeaders_LayoutTemplateId",
                table: "TestResultHeaders");

            migrationBuilder.DropIndex(
                name: "IX_TestResultHeaders_RetestOfHeaderId",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "FormulaVersion",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "IsReportable",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "ReportableOverrideReason",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "ApprovalHash",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "ExecutionExtraJson",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "LayoutSnapshotJson",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "LayoutTemplateId",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "LayoutVersion",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "ReportSnapshotJson",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "RetestOfHeaderId",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "RetestReasonCode",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "TestResultHeaders");
        }
    }
}
