using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablQCPlanAdddeptemployeeTestMId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablQualityControlPlanActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "NablQualityControlPlanActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestMethod",
                table: "NablQualityControlPlanActivities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablQualityControlPlanActivities");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "NablQualityControlPlanActivities");

            migrationBuilder.DropColumn(
                name: "TestMethod",
                table: "NablQualityControlPlanActivities");
        }
    }
}
