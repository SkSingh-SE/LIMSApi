using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablPtllcPlanAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivitiesJson",
                table: "NablPtIlcPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldOfAccreditation",
                table: "NablPtIlcPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaboratoryId",
                table: "NablPtIlcPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaboratoryName",
                table: "NablPtIlcPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEndDate",
                table: "NablPtIlcPlans",
                type: "datetime2",
                nullable: true);


            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStartDate",
                table: "NablPtIlcPlans",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivitiesJson",
                table: "NablPtIlcPlans");

            migrationBuilder.DropColumn(
                name: "FieldOfAccreditation",
                table: "NablPtIlcPlans");

            migrationBuilder.DropColumn(
                name: "LaboratoryId",
                table: "NablPtIlcPlans");

            migrationBuilder.DropColumn(
                name: "LaboratoryName",
                table: "NablPtIlcPlans");

            migrationBuilder.DropColumn(
                name: "PeriodEndDate",
                table: "NablPtIlcPlans");

            migrationBuilder.DropColumn(
                name: "PeriodStartDate",
                table: "NablPtIlcPlans");
        }
    }
}
