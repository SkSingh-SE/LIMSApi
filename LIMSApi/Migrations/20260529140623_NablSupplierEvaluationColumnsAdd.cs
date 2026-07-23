using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablSupplierEvaluationColumnsAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AcceptableLimitMin",
                table: "NablSupplierEvaluations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CriteriaJson",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EvaluatingPeriodFrom",
                table: "NablSupplierEvaluations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EvaluatingPeriodTo",
                table: "NablSupplierEvaluations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GstNo",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomingPlanJson",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NatureOfBusiness",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POJson",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentStatus",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductsServicesOffered",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Recommendation",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisterNo",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceProvider",
                table: "NablSupplierEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SupplierRegisterId",
                table: "NablSupplierEvaluations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ToContinued",
                table: "NablSupplierEvaluations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ToRemoved",
                table: "NablSupplierEvaluations",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptableLimitMin",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "CriteriaJson",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "EvaluatingPeriodFrom",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "EvaluatingPeriodTo",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "GstNo",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "IncomingPlanJson",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "NatureOfBusiness",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "POJson",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "PresentStatus",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "ProductsServicesOffered",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "Recommendation",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "RegisterNo",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "ServiceProvider",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "SupplierRegisterId",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "ToContinued",
                table: "NablSupplierEvaluations");

            migrationBuilder.DropColumn(
                name: "ToRemoved",
                table: "NablSupplierEvaluations");
        }
    }
}
