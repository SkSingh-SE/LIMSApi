using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "SampleInwards",
                newName: "InwardStatus");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "SampleDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "SampleDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "SampleDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleStatus",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "SampleStatus",
                table: "SampleDetails");

            migrationBuilder.RenameColumn(
                name: "InwardStatus",
                table: "SampleInwards",
                newName: "Status");
        }
    }
}
