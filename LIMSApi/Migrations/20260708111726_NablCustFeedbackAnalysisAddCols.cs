using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablCustFeedbackAnalysisAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionDetails",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionTaken",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AnalysisDate",
                table: "NablFeedbackAnalyses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnalysisNo",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "NablFeedbackAnalyses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectiveActionRequired",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CustomerID",
                table: "NablFeedbackAnalyses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerRemarks",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EffectivenessStatus",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FeedbackDate",
                table: "NablFeedbackAnalyses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalStatus",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImprovementOpportunity",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuesIdentified",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewRequirement",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverallConclusion",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverallCustomerSatisfaction",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverallGrade",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PositiveObservations",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RatingsJson",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsiblePerson",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootCause",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suggestions",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TargetCompletionDate",
                table: "NablFeedbackAnalyses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDate",
                table: "NablFeedbackAnalyses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationRemarks",
                table: "NablFeedbackAnalyses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionDetails",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "ActionTaken",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "AnalysisDate",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "AnalysisNo",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "CorrectiveActionRequired",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "CustomerRemarks",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "EffectivenessStatus",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "FeedbackDate",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "FinalStatus",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "ImprovementOpportunity",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "IssuesIdentified",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "NewRequirement",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "OverallConclusion",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "OverallCustomerSatisfaction",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "OverallGrade",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "PositiveObservations",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "RatingsJson",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "ResponsiblePerson",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "RootCause",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "Suggestions",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "TargetCompletionDate",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "VerificationRemarks",
                table: "NablFeedbackAnalyses");
        }
    }
}
