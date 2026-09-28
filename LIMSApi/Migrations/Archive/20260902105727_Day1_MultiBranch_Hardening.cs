using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Day1_MultiBranch_Hardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchDisciplines_Branches_BranchID",
                table: "BranchDisciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchDisciplines_DisciplineMasters_DisciplineID",
                table: "BranchDisciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Organizations_OrganizationID",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMasters_Branches_BranchID",
                table: "UserMasters");

            migrationBuilder.DropIndex(
                name: "IX_Branches_OrganizationID",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_BranchDisciplines_BranchID",
                table: "BranchDisciplines");

            migrationBuilder.DropColumn(
                name: "BranchCount",
                table: "Organizations");

            migrationBuilder.AddColumn<bool>(
                name: "CanViewAllBranches",
                table: "UserMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "SampleInwards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "LabRooms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "EquipmentMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchID",
                table: "DepartmentMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "UserBranches",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CanView = table.Column<bool>(type: "bit", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    CanExecute = table.Column<bool>(type: "bit", nullable: false),
                    CanApprove = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranches", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserBranches_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBranches_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_BranchID",
                table: "SampleInwards",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_LabRooms_BranchID",
                table: "LabRooms",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_BranchID",
                table: "EquipmentMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMasters_BranchID",
                table: "DepartmentMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_OrganizationID_Code",
                table: "Branches",
                columns: new[] { "OrganizationID", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchDisciplines_BranchID_DisciplineID",
                table: "BranchDisciplines",
                columns: new[] { "BranchID", "DisciplineID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_BranchID",
                table: "UserBranches",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_UserID_BranchID",
                table: "UserBranches",
                columns: new[] { "UserID", "BranchID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_UserID_IsDefault",
                table: "UserBranches",
                column: "UserID",
                unique: true,
                filter: "[IsDefault] = 1");

            migrationBuilder.Sql(@"
                -- 1. Check for Ambiguity
                DECLARE @OrgCount int = (SELECT COUNT(*) FROM Organizations WHERE IsActive = 1);
                IF @OrgCount > 1
                BEGIN
                    THROW 51000, 'Ambiguous migration: Multiple active organizations detected. Manual mapping required.', 1;
                END

                -- 2. Dynamically resolve single active Organization and Branch
                DECLARE @ResolvedOrgID bigint = (SELECT TOP 1 Id FROM Organizations WHERE IsActive = 1);
                DECLARE @ResolvedBranchID bigint = (SELECT TOP 1 ID FROM Branches WHERE OrganizationID = @ResolvedOrgID AND IsActive = 1);

                -- If no branch exists, create one
                IF @ResolvedBranchID IS NULL AND @ResolvedOrgID IS NOT NULL
                BEGIN
                    INSERT INTO Branches (OrganizationID, Name, Code, IsHeadOffice, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    SELECT Id, LabName, LabCode, 1, 0, GETUTCDATE(), 'LIMS', 1 FROM Organizations WHERE Id = @ResolvedOrgID;

                    SET @ResolvedBranchID = SCOPE_IDENTITY();
                END

                IF @ResolvedBranchID IS NOT NULL
                BEGIN
                    -- Backfill UserMasters
                    UPDATE UserMasters SET BranchID = @ResolvedBranchID WHERE BranchID IS NULL OR BranchID = 0;

                    -- Backfill UserBranches for all existing users
                    INSERT INTO UserBranches (UserID, BranchID, IsDefault, CanView, CanCreate, CanEdit, CanExecute, CanApprove, CanDelete, CreatedBy, CreatedOn, CompanyCode, IsActive)
                    SELECT u.ID, @ResolvedBranchID, 1, 1, 1, 1, 1, 0, 0, 0, GETUTCDATE(), 'LIMS', 1
                    FROM UserMasters u
                    WHERE NOT EXISTS (SELECT 1 FROM UserBranches ub WHERE ub.UserID = u.ID AND ub.BranchID = @ResolvedBranchID);

                    -- Backfill transactional tables to valid branch ID
                    UPDATE SampleInwards SET BranchID = @ResolvedBranchID WHERE BranchID IS NULL OR BranchID = 0;
                    UPDATE EquipmentMasters SET BranchID = @ResolvedBranchID WHERE BranchID IS NULL OR BranchID = 0;
                    UPDATE DepartmentMasters SET BranchID = @ResolvedBranchID WHERE BranchID IS NULL OR BranchID = 0;
                    UPDATE LabRooms SET BranchID = @ResolvedBranchID WHERE BranchID IS NULL OR BranchID = 0;
                END
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchDisciplines_Branches_BranchID",
                table: "BranchDisciplines",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchDisciplines_DisciplineMasters_DisciplineID",
                table: "BranchDisciplines",
                column: "DisciplineID",
                principalTable: "DisciplineMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Organizations_OrganizationID",
                table: "Branches",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentMasters_Branches_BranchID",
                table: "DepartmentMasters",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMasters_Branches_BranchID",
                table: "EquipmentMasters",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabRooms_Branches_BranchID",
                table: "LabRooms",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SampleInwards_Branches_BranchID",
                table: "SampleInwards",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMasters_Branches_BranchID",
                table: "UserMasters",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchDisciplines_Branches_BranchID",
                table: "BranchDisciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchDisciplines_DisciplineMasters_DisciplineID",
                table: "BranchDisciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Organizations_OrganizationID",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentMasters_Branches_BranchID",
                table: "DepartmentMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMasters_Branches_BranchID",
                table: "EquipmentMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_LabRooms_Branches_BranchID",
                table: "LabRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleInwards_Branches_BranchID",
                table: "SampleInwards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMasters_Branches_BranchID",
                table: "UserMasters");

            migrationBuilder.DropTable(
                name: "UserBranches");

            migrationBuilder.DropIndex(
                name: "IX_SampleInwards_BranchID",
                table: "SampleInwards");

            migrationBuilder.DropIndex(
                name: "IX_LabRooms_BranchID",
                table: "LabRooms");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentMasters_BranchID",
                table: "EquipmentMasters");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentMasters_BranchID",
                table: "DepartmentMasters");

            migrationBuilder.DropIndex(
                name: "IX_Branches_OrganizationID_Code",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_BranchDisciplines_BranchID_DisciplineID",
                table: "BranchDisciplines");

            migrationBuilder.DropColumn(
                name: "CanViewAllBranches",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "DepartmentMasters");

            migrationBuilder.AddColumn<int>(
                name: "BranchCount",
                table: "Organizations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_OrganizationID",
                table: "Branches",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_BranchDisciplines_BranchID",
                table: "BranchDisciplines",
                column: "BranchID");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchDisciplines_Branches_BranchID",
                table: "BranchDisciplines",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchDisciplines_DisciplineMasters_DisciplineID",
                table: "BranchDisciplines",
                column: "DisciplineID",
                principalTable: "DisciplineMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Organizations_OrganizationID",
                table: "Branches",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMasters_Branches_BranchID",
                table: "UserMasters",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "ID");
        }
    }
}
