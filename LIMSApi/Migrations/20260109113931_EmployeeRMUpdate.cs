using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeRMUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMasters_EmployeeMasters_ReportingTo",
                table: "EmployeeMasters");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeMasters_ReportingTo",
                table: "EmployeeMasters");

            migrationBuilder.DropColumn(
                name: "ReportingTo",
                table: "EmployeeMasters");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_ReportingManagerID",
                table: "EmployeeMasters",
                column: "ReportingManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMasters_EmployeeMasters_ReportingManagerID",
                table: "EmployeeMasters",
                column: "ReportingManagerID",
                principalTable: "EmployeeMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMasters_EmployeeMasters_ReportingManagerID",
                table: "EmployeeMasters");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeMasters_ReportingManagerID",
                table: "EmployeeMasters");

            migrationBuilder.AddColumn<long>(
                name: "ReportingTo",
                table: "EmployeeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_ReportingTo",
                table: "EmployeeMasters",
                column: "ReportingTo");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMasters_EmployeeMasters_ReportingTo",
                table: "EmployeeMasters",
                column: "ReportingTo",
                principalTable: "EmployeeMasters",
                principalColumn: "ID");
        }
    }
}
