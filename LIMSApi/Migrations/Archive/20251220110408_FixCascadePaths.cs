using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportHeaders_SampleDetails_SampleID",
                table: "ReportHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_SampleDetails_SampleID",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplates_LaboratoryTests_TestTypeID",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "WorkflowInstanceId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ReportHeaders");

            migrationBuilder.RenameColumn(
                name: "SampleID",
                table: "Reports",
                newName: "ReportHeaderID");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_SampleID",
                table: "Reports",
                newName: "IX_Reports_ReportHeaderID");

            migrationBuilder.AlterColumn<long>(
                name: "TestTypeID",
                table: "ReportTemplates",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "TestType",
                table: "ReportTemplates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "ReportTemplates",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ConditionExpression",
                table: "ReportTemplateBlocks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AmendmentRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupportingDocumentsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmendmentRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AmendmentRequests_ReportHeaders_ReportHeaderID",
                        column: x => x.ReportHeaderID,
                        principalTable: "ReportHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_SampleID",
                table: "TestResultHeaders",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_AmendmentRequests_ReportHeaderID",
                table: "AmendmentRequests",
                column: "ReportHeaderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportHeaders_SampleDetails_SampleID",
                table: "ReportHeaders",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportHeaders_ReportHeaderID",
                table: "Reports",
                column: "ReportHeaderID",
                principalTable: "ReportHeaders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTemplates_LaboratoryTests_TestTypeID",
                table: "ReportTemplates",
                column: "TestTypeID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultHeaders_SampleDetails_SampleID",
                table: "TestResultHeaders",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportHeaders_SampleDetails_SampleID",
                table: "ReportHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportHeaders_ReportHeaderID",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplates_LaboratoryTests_TestTypeID",
                table: "ReportTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResultHeaders_SampleDetails_SampleID",
                table: "TestResultHeaders");

            migrationBuilder.DropTable(
                name: "AmendmentRequests");

            migrationBuilder.DropIndex(
                name: "IX_TestResultHeaders_SampleID",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "ConditionExpression",
                table: "ReportTemplateBlocks");

            migrationBuilder.RenameColumn(
                name: "ReportHeaderID",
                table: "Reports",
                newName: "SampleID");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReportHeaderID",
                table: "Reports",
                newName: "IX_Reports_SampleID");

            migrationBuilder.AlterColumn<long>(
                name: "TestTypeID",
                table: "ReportTemplates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TestType",
                table: "ReportTemplates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "ReportTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<long>(
                name: "WorkflowInstanceId",
                table: "Reports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "ReportHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportHeaders_SampleDetails_SampleID",
                table: "ReportHeaders",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_SampleDetails_SampleID",
                table: "Reports",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTemplates_LaboratoryTests_TestTypeID",
                table: "ReportTemplates",
                column: "TestTypeID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
