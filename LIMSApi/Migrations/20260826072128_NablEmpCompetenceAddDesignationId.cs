using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablEmpCompetenceAddDesignationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablEmployeeCompetences_EmployeeMasters_EmployeeId",
                table: "NablEmployeeCompetences");

            migrationBuilder.DropIndex(
                name: "IX_NablEmployeeCompetences_EmployeeId",
                table: "NablEmployeeCompetences");

            migrationBuilder.AddColumn<long>(
                name: "DesignationId",
                table: "NablEmployeeCompetences",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "NablEmployeeCompetences");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeCompetences_EmployeeId",
                table: "NablEmployeeCompetences",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablEmployeeCompetences_EmployeeMasters_EmployeeId",
                table: "NablEmployeeCompetences",
                column: "EmployeeId",
                principalTable: "EmployeeMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
