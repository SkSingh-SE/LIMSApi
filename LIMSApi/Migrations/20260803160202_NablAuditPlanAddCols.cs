using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablAuditPlanAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanNo",
                table: "NablAuditPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "NablAuditPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDateFrom",
                table: "NablAuditPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDateTo",
                table: "NablAuditPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScheduleItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditPlanId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ISOClausesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditorId = table.Column<long>(type: "bigint", nullable: false),
                    AuditorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditeeId = table.Column<long>(type: "bigint", nullable: false),
                    AuditeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChecklistId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ScheduleItems_NablAuditPlans_AuditPlanId",
                        column: x => x.AuditPlanId,
                        principalTable: "NablAuditPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleItems_AuditPlanId",
                table: "ScheduleItems",
                column: "AuditPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleItems");

            migrationBuilder.DropColumn(
                name: "PlanNo",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "ScheduleDateFrom",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "ScheduleDateTo",
                table: "NablAuditPlans");
        }
    }
}
