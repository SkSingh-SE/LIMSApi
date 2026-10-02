using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablNonConformingWorkModifiedTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CorrectiveAction",
                table: "NablNonConformingWorks",
                newName: "ProblemDescription");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "NablNonConformingWorks",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "NablNonConformingWorks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStep",
                table: "NablNonConformingWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CustomerAffected",
                table: "NablNonConformingWorks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "NablNonConformingWorks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablNonConformingWorks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "NablNonConformingWorks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "NablNonConformingWorks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "NablNonConformingWorks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceModule",
                table: "NablNonConformingWorks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                table: "NablNonConformingWorks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReportedByEmployeeId",
                table: "NablNonConformingWorks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportedByEmployeeName",
                table: "NablNonConformingWorks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "NablNonConformingWorks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NablNonConformingWorkClosures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NablNonConformingWorkId = table.Column<long>(type: "bigint", nullable: false),
                    ClosureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedByEmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    ClosedByEmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FinalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablNonConformingWorkClosures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NablNonConformingWorkClosures_NablNonConformingWorks_NablNonConformingWorkId",
                        column: x => x.NablNonConformingWorkId,
                        principalTable: "NablNonConformingWorks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablNonConformingWorkCorrectiveActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NablNonConformingWorkId = table.Column<long>(type: "bigint", nullable: false),
                    ActionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsiblePersonId = table.Column<long>(type: "bigint", nullable: true),
                    ResponsiblePersonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResourcesRequired = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreventiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablNonConformingWorkCorrectiveActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NablNonConformingWorkCorrectiveActions_NablNonConformingWorks_NablNonConformingWorkId",
                        column: x => x.NablNonConformingWorkId,
                        principalTable: "NablNonConformingWorks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablNonConformingWorkInvestigations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NablNonConformingWorkId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedToEmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    AssignedToEmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvestigationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvestigationMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContributingFactors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvestigationDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedAction = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablNonConformingWorkInvestigations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NablNonConformingWorkInvestigations_NablNonConformingWorks_NablNonConformingWorkId",
                        column: x => x.NablNonConformingWorkId,
                        principalTable: "NablNonConformingWorks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablNonConformingWorkVerifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NablNonConformingWorkId = table.Column<long>(type: "bigint", nullable: false),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedByEmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    VerifiedByEmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VerificationMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablNonConformingWorkVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NablNonConformingWorkVerifications_NablNonConformingWorks_NablNonConformingWorkId",
                        column: x => x.NablNonConformingWorkId,
                        principalTable: "NablNonConformingWorks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NablNonConformingWorkClosures_NablNonConformingWorkId",
                table: "NablNonConformingWorkClosures",
                column: "NablNonConformingWorkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablNonConformingWorkCorrectiveActions_NablNonConformingWorkId",
                table: "NablNonConformingWorkCorrectiveActions",
                column: "NablNonConformingWorkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablNonConformingWorkInvestigations_NablNonConformingWorkId",
                table: "NablNonConformingWorkInvestigations",
                column: "NablNonConformingWorkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablNonConformingWorkVerifications_NablNonConformingWorkId",
                table: "NablNonConformingWorkVerifications",
                column: "NablNonConformingWorkId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NablNonConformingWorkClosures");

            migrationBuilder.DropTable(
                name: "NablNonConformingWorkCorrectiveActions");

            migrationBuilder.DropTable(
                name: "NablNonConformingWorkInvestigations");

            migrationBuilder.DropTable(
                name: "NablNonConformingWorkVerifications");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "CurrentStep",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "CustomerAffected",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "ReferenceModule",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "ReportedByEmployeeId",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "ReportedByEmployeeName",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "NablNonConformingWorks");

            migrationBuilder.RenameColumn(
                name: "ProblemDescription",
                table: "NablNonConformingWorks",
                newName: "CorrectiveAction");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "NablNonConformingWorks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
