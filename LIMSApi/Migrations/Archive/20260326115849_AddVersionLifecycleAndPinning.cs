using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionLifecycleAndPinning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add new columns BEFORE dropping Default
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TestMethodSpecificationVersions",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "Draft");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "TestMethodSpecificationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDate",
                table: "TestMethodSpecificationVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SupersededDate",
                table: "TestMethodSpecificationVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewDate",
                table: "TestMethodSpecificationVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeReason",
                table: "TestMethodSpecificationVersions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "TestMethodSpecificationVersions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TestMethodSpecificationVersions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            // Step 2: Migrate data from Default → Status
            // VersionStatus enum: Draft=0, Active=1, Superseded=2, Withdrawn=3
            // Stored as enum name string in varchar(20) column
            migrationBuilder.Sql(@"
                UPDATE TestMethodSpecificationVersions SET Status = 'Active', EffectiveDate = GETUTCDATE(), CreatedOn = GETUTCDATE() WHERE [Default] = 1;
                UPDATE TestMethodSpecificationVersions SET Status = 'Superseded', CreatedOn = GETUTCDATE() WHERE [Default] = 0;
            ");

            // Step 3: Now safe to drop Default
            migrationBuilder.DropColumn(
                name: "Default",
                table: "TestMethodSpecificationVersions");

            // Step 4: Add version pinning FKs to downstream tables
            migrationBuilder.AddColumn<long>(
                name: "TestMethodSpecificationVersionID",
                table: "ProductSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TestMethodSpecificationVersionID",
                table: "LabScopeSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_TestMethodSpecificationVersionID",
                table: "ProductSpecifications",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecifications_TestMethodSpecificationVersionID",
                table: "LabScopeSpecifications",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "ProductSpecifications",
                column: "TestMethodSpecificationVersionID",
                principalTable: "TestMethodSpecificationVersions",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LabScopeSpecifications_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "LabScopeSpecifications",
                column: "TestMethodSpecificationVersionID",
                principalTable: "TestMethodSpecificationVersions",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "ProductSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_LabScopeSpecifications_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                table: "LabScopeSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_ProductSpecifications_TestMethodSpecificationVersionID",
                table: "ProductSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_LabScopeSpecifications_TestMethodSpecificationVersionID",
                table: "LabScopeSpecifications");

            // Re-add Default column
            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "TestMethodSpecificationVersions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Migrate data back: Active → Default=true, others → Default=false
            migrationBuilder.Sql(@"
                UPDATE TestMethodSpecificationVersions SET [Default] = 1 WHERE Status = 'Active';
                UPDATE TestMethodSpecificationVersions SET [Default] = 0 WHERE Status != 'Active';
            ");

            migrationBuilder.DropColumn(
                name: "ChangeReason",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "ReviewDate",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "SupersededDate",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationVersionID",
                table: "ProductSpecifications");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationVersionID",
                table: "LabScopeSpecifications");
        }
    }
}
