using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablRetestingTblAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TestMethod",
                table: "NablRetestings",
                newName: "TestMethodName");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discipline",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "NablRetestings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "NablRetestings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrequencyType",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabIncharge",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialProductGroup",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextDueDate",
                table: "NablRetestings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanNo",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanYear",
                table: "NablRetestings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QcActivity",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QcPlanActivityId",
                table: "NablRetestings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QcPlanNoId",
                table: "NablRetestings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceName",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsibleEmployee",
                table: "NablRetestings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RetestingComparisonLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetestingRetainedSampleId = table.Column<long>(type: "bigint", nullable: false),
                    InitialTestLogId = table.Column<long>(type: "bigint", nullable: true),
                    QcMonth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfRetesting = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SampleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousPrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RetestPrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetestValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Difference = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AcceptableLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ResultStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestedById = table.Column<int>(type: "int", nullable: true),
                    TestedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QmSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetestingComparisonLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetestingComparisonLogs_NablRetestings_RetestingRetainedSampleId",
                        column: x => x.RetestingRetainedSampleId,
                        principalTable: "NablRetestings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RetestingInitialTestLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetestingRetainedSampleId = table.Column<long>(type: "bigint", nullable: false),
                    DateOfTesting = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SampleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultPrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TestedById = table.Column<int>(type: "int", nullable: true),
                    TestedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetestingInitialTestLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetestingInitialTestLogs_NablRetestings_RetestingRetainedSampleId",
                        column: x => x.RetestingRetainedSampleId,
                        principalTable: "NablRetestings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RetestingComparisonLogs_RetestingRetainedSampleId",
                table: "RetestingComparisonLogs",
                column: "RetestingRetainedSampleId");

            migrationBuilder.CreateIndex(
                name: "IX_RetestingInitialTestLogs_RetestingRetainedSampleId",
                table: "RetestingInitialTestLogs",
                column: "RetestingRetainedSampleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetestingComparisonLogs");

            migrationBuilder.DropTable(
                name: "RetestingInitialTestLogs");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "Discipline",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "FrequencyType",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "LabIncharge",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "MaterialProductGroup",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "NextDueDate",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "PlanNo",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "PlanYear",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "QcActivity",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "QcPlanActivityId",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "QcPlanNoId",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "ReferenceName",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "NablRetestings");

            migrationBuilder.DropColumn(
                name: "ResponsibleEmployee",
                table: "NablRetestings");

            migrationBuilder.RenameColumn(
                name: "TestMethodName",
                table: "NablRetestings",
                newName: "TestMethod");
        }
    }
}
