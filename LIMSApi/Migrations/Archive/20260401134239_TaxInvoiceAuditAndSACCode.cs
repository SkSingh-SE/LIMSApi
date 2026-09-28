using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class TaxInvoiceAuditAndSACCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "TaxInvoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "TaxInvoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TaxInvoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "TaxInvoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "TaxInvoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SACCode",
                table: "TaxInvoices",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "SACCode",
                table: "TaxInvoices");
        }
    }
}
