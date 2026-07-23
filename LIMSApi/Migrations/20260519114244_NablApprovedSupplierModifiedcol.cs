using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablApprovedSupplierModifiedcol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvaluationRequired",
                table: "NablSupplierRegistrations");

            migrationBuilder.AddColumn<DateTime>(
                name: "AgreementDate",
                table: "NablApprovedSuppliers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlacklistDate",
                table: "NablApprovedSuppliers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlacklistReason",
                table: "NablApprovedSuppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlacklisted",
                table: "NablApprovedSuppliers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "NablApprovedSuppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SupplierRegisterId",
                table: "NablApprovedSuppliers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablApprovedSuppliers_SupplierRegisterId",
                table: "NablApprovedSuppliers",
                column: "SupplierRegisterId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablApprovedSuppliers_NablSupplierRegistrations_SupplierRegisterId",
                table: "NablApprovedSuppliers",
                column: "SupplierRegisterId",
                principalTable: "NablSupplierRegistrations",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablApprovedSuppliers_NablSupplierRegistrations_SupplierRegisterId",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_NablApprovedSuppliers_SupplierRegisterId",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "AgreementDate",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "BlacklistDate",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "BlacklistReason",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "IsBlacklisted",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "SupplierRegisterId",
                table: "NablApprovedSuppliers");

            migrationBuilder.AddColumn<bool>(
                name: "EvaluationRequired",
                table: "NablSupplierRegistrations",
                type: "bit",
                nullable: true);
        }
    }
}
