using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApprovedSupplierTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "NablApprovedSuppliers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NablApprovedSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnlistmentDate",
                table: "NablApprovedSuppliers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPresentStatus",
                table: "NablApprovedSuppliers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastScore",
                table: "NablApprovedSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "NablApprovedSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ProductApproved",
                table: "NablApprovedSuppliers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceProviderName",
                table: "NablApprovedSuppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "EnlistmentDate",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "IsPresentStatus",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "LastScore",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "ProductApproved",
                table: "NablApprovedSuppliers");

            migrationBuilder.DropColumn(
                name: "ServiceProviderName",
                table: "NablApprovedSuppliers");
        }
    }
}
