using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "UserMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchCount",
                table: "Organizations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiBranch",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "DisciplineMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "DisciplineMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsHeadOffice = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Branches_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchDisciplines",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    DisciplineID = table.Column<long>(type: "bigint", nullable: false),
                    IsAccredited = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchDisciplines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BranchDisciplines_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchDisciplines_DisciplineMasters_DisciplineID",
                        column: x => x.DisciplineID,
                        principalTable: "DisciplineMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMasters_BranchID",
                table: "UserMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_BranchDisciplines_BranchID",
                table: "BranchDisciplines",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_BranchDisciplines_DisciplineID",
                table: "BranchDisciplines",
                column: "DisciplineID");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_OrganizationID",
                table: "Branches",
                column: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMasters_Branches_BranchID",
                table: "UserMasters",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID");

            migrationBuilder.Sql(@"
                -- 1. Create a default branch for each existing organization
                INSERT INTO Branches (OrganizationID, Name, Code, IsHeadOffice, CreatedBy, CreatedOn, CompanyCode, IsActive) 
                SELECT Id, LabName, LabCode, 1, 0, GETUTCDATE(), 'LIMS', 1 FROM Organizations;

                -- 2. Update Organizations status
                UPDATE Organizations SET IsMultiBranch = 0, BranchCount = 1;

                -- 3. Backfill BranchID for Users based on their OrganizationID (fallback to first branch if OrganizationID is NULL, assuming single-tenant mode for now)
                UPDATE UserMasters SET BranchID = (
                    SELECT TOP 1 ID FROM Branches WHERE Branches.OrganizationID = UserMasters.OrganizationID
                ) WHERE OrganizationID IS NOT NULL;

                UPDATE UserMasters SET BranchID = (
                    SELECT TOP 1 ID FROM Branches
                ) WHERE OrganizationID IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMasters_Branches_BranchID",
                table: "UserMasters");

            migrationBuilder.DropTable(
                name: "BranchDisciplines");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_UserMasters_BranchID",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "BranchCount",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "IsMultiBranch",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "DisciplineMasters");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "DisciplineMasters");
        }
    }
}
