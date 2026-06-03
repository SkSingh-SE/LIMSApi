using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrainingPlanTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanDate",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "ApprovedByName",
                table: "NablJobDescriptions");

            migrationBuilder.DropColumn(
                name: "PreparedByName",
                table: "NablJobDescriptions");

            migrationBuilder.AddColumn<string>(
                name: "Agency",
                table: "NablTrainingPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Objectives",
                table: "NablTrainingPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanMonth",
                table: "NablTrainingPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "NablTrainingPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetAudience",
                table: "NablTrainingPlans",
                type: "nvarchar(max)",
                nullable: true);

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agency",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "Objectives",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "PlanMonth",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "NablTrainingPlans");

            migrationBuilder.DropColumn(
                name: "TargetAudience",
                table: "NablTrainingPlans");

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanDate",
                table: "NablTrainingPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByName",
                table: "NablJobDescriptions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparedByName",
                table: "NablJobDescriptions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
