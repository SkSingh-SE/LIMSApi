using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class FixNablModelFieldMismatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "NablTrainingPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoursesJson",
                table: "NablTrainingPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanDate",
                table: "NablTrainingPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanningYear",
                table: "NablTrainingPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBudget",
                table: "NablTrainingPlans",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankDetailsJson",
                table: "NablSupplierRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "NablSupplierRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentsSubmittedJson",
                table: "NablSupplierRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GstNo",
                table: "NablSupplierRegistrations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsoCertified",
                table: "NablSupplierRegistrations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IsoDetails",
                table: "NablSupplierRegistrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "NablSupplierRegistrations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NatureOfBusiness",
                table: "NablSupplierRegistrations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanNo",
                table: "NablSupplierRegistrations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductsServicesOffered",
                table: "NablSupplierRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordedBy",
                table: "NablSupplierRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationStatus",
                table: "NablSupplierRegistrations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "NablSupplierRegistrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                table: "NablSupplierRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "NablSupplierRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RatingsJson",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suggestions",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AreaDepartment",
                table: "NablAuditPlans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditType",
                table: "NablAuditPlans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditorName",
                table: "NablAuditPlans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "NablAuditPlans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDate",
                table: "NablAuditPlans",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "CoursesJson",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "PlanDate",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "PlanningYear",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "TotalBudget",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "BankDetailsJson",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "DocumentsSubmittedJson",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "GstNo",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "IsoCertified",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "IsoDetails",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "NatureOfBusiness",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "PanNo",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "ProductsServicesOffered",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "RecordedBy",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "RegistrationStatus",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "RatingsJson",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "Suggestions",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "AreaDepartment",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "AuditType",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "AuditorName",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "NablAuditPlans");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "NablAuditPlans");
        }
    }
}
