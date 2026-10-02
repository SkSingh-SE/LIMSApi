using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablAuditPlanUpdateLeadAuditorIdCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablAuditPlans_EmployeeMasters_LeadAuditorId",
                table: "NablAuditPlans");

            migrationBuilder.DropIndex(
                name: "IX_NablAuditPlans_LeadAuditorId",
                table: "NablAuditPlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NablAuditPlans_LeadAuditorId",
                table: "NablAuditPlans",
                column: "LeadAuditorId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablAuditPlans_EmployeeMasters_LeadAuditorId",
                table: "NablAuditPlans",
                column: "LeadAuditorId",
                principalTable: "EmployeeMasters",
                principalColumn: "ID");
        }
    }
}
