using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Tier3_MultiBranch_Gaps_Hardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "NumberingConfigs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "NablAccreditations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "LabScopeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "EmployeeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "BankMasters",
                type: "bigint",
                nullable: true);

            // ── Data Cleansing & Multi-Branch Record Migration ──
            migrationBuilder.Sql("DELETE FROM BankMasters WHERE BankName LIKE '_TEST_%' OR BankName = 'No-Bank';");
            migrationBuilder.Sql("UPDATE BankMasters SET BranchID = 1 WHERE BranchID IS NULL;");
            migrationBuilder.Sql(@"
                UPDATE e
                SET e.BranchID = COALESCE(d.BranchID, 1)
                FROM EmployeeMasters e
                LEFT JOIN DepartmentMasters d ON e.DepartmentID = d.ID
                WHERE e.BranchID IS NULL;
            ");
            migrationBuilder.Sql("UPDATE NablAccreditations SET BranchID = 1 WHERE BranchID IS NULL;");
            migrationBuilder.Sql("UPDATE LabScopeMasters SET BranchID = 1 WHERE BranchID IS NULL;");
            migrationBuilder.Sql("UPDATE NumberingConfigs SET OrganizationId = 7, BranchId = 1 WHERE OrganizationId = 0 OR BranchId IS NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_NumberingConfigs_BranchId",
                table: "NumberingConfigs",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberingConfigs_OrganizationId",
                table: "NumberingConfigs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAccreditations_BranchID",
                table: "NablAccreditations",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_NablAccreditations_OrganizationId",
                table: "NablAccreditations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeMasters_BranchID",
                table: "LabScopeMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_BranchID",
                table: "EmployeeMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_BankMasters_BranchID",
                table: "BankMasters",
                column: "BranchID");

            migrationBuilder.AddForeignKey(
                name: "FK_BankMasters_Branches_BranchID",
                table: "BankMasters",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMasters_Branches_BranchID",
                table: "EmployeeMasters",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabScopeMasters_Branches_BranchID",
                table: "LabScopeMasters",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NablAccreditations_Branches_BranchID",
                table: "NablAccreditations",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NablAccreditations_Organizations_OrganizationId",
                table: "NablAccreditations",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NumberingConfigs_Branches_BranchId",
                table: "NumberingConfigs",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NumberingConfigs_Organizations_OrganizationId",
                table: "NumberingConfigs",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankMasters_Branches_BranchID",
                table: "BankMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMasters_Branches_BranchID",
                table: "EmployeeMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_LabScopeMasters_Branches_BranchID",
                table: "LabScopeMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_NablAccreditations_Branches_BranchID",
                table: "NablAccreditations");

            migrationBuilder.DropForeignKey(
                name: "FK_NablAccreditations_Organizations_OrganizationId",
                table: "NablAccreditations");

            migrationBuilder.DropForeignKey(
                name: "FK_NumberingConfigs_Branches_BranchId",
                table: "NumberingConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_NumberingConfigs_Organizations_OrganizationId",
                table: "NumberingConfigs");

            migrationBuilder.DropIndex(
                name: "IX_NumberingConfigs_BranchId",
                table: "NumberingConfigs");

            migrationBuilder.DropIndex(
                name: "IX_NumberingConfigs_OrganizationId",
                table: "NumberingConfigs");

            migrationBuilder.DropIndex(
                name: "IX_NablAccreditations_BranchID",
                table: "NablAccreditations");

            migrationBuilder.DropIndex(
                name: "IX_NablAccreditations_OrganizationId",
                table: "NablAccreditations");

            migrationBuilder.DropIndex(
                name: "IX_LabScopeMasters_BranchID",
                table: "LabScopeMasters");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeMasters_BranchID",
                table: "EmployeeMasters");

            migrationBuilder.DropIndex(
                name: "IX_BankMasters_BranchID",
                table: "BankMasters");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "NumberingConfigs");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "NablAccreditations");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "LabScopeMasters");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "EmployeeMasters");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "BankMasters");
        }
    }
}
