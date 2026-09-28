using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecificationGradeLifecycleAndUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecificationGrades_SpecificationHeaderID",
                table: "SpecificationGrades");

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "SpecificationGrades",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "SpecificationGrades",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SpecificationGrades",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedBy",
                table: "SpecificationGrades",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "SpecificationGrades",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationGrades_SpecificationHeaderID_Grade",
                table: "SpecificationGrades",
                columns: new[] { "SpecificationHeaderID", "Grade" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecificationGrades_SpecificationHeaderID_Grade",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "SpecificationGrades");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationGrades_SpecificationHeaderID",
                table: "SpecificationGrades",
                column: "SpecificationHeaderID");
        }
    }
}
