using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablQCPLanAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ReferenceMaterialId",
                table: "ReferenceMaterialConsumptionLogs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Discipline",
                table: "NablQualityControlPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "NablQualityControlPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "NablQualityControlPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabIncharge",
                table: "NablQualityControlPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialProductGroup",
                table: "NablQualityControlPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanNo",
                table: "NablQualityControlPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanYear",
                table: "NablQualityControlPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RetentionPeriod",
                table: "NablQualityControlPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NablQualityControlPlanActivities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QualityControlPlanId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodId = table.Column<long>(type: "bigint", nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true),
                    ReferenceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrequencyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrequencyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablQualityControlPlanActivities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablQualityControlPlanActivities_NablQualityControlPlans_QualityControlPlanId",
                        column: x => x.QualityControlPlanId,
                        principalTable: "NablQualityControlPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NablQualityControlPlanActivities_QualityControlPlanId",
                table: "NablQualityControlPlanActivities",
                column: "QualityControlPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NablQualityControlPlanActivities");

            migrationBuilder.DropColumn(
                name: "Discipline",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "LabIncharge",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "MaterialProductGroup",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "PlanNo",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "PlanYear",
                table: "NablQualityControlPlans");

            migrationBuilder.DropColumn(
                name: "RetentionPeriod",
                table: "NablQualityControlPlans");

            migrationBuilder.AlterColumn<long>(
                name: "ReferenceMaterialId",
                table: "ReferenceMaterialConsumptionLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
