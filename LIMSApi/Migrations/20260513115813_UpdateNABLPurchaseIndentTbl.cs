using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNABLPurchaseIndentTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDate",
                table: "NablPurchaseIndents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndentorName",
                table: "NablPurchaseIndents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "NablPurchaseIndents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "NablPurchaseIndents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalSpecification",
                table: "NablPurchaseIndents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedDate",
                table: "NablPurchaseIndents");

            migrationBuilder.DropColumn(
                name: "IndentorName",
                table: "NablPurchaseIndents");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "NablPurchaseIndents");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "NablPurchaseIndents");

            migrationBuilder.DropColumn(
                name: "TechnicalSpecification",
                table: "NablPurchaseIndents");
        }
    }
}
