using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablComplaintAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComplainantName",
                table: "NablComplaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComplaintNo",
                table: "NablComplaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MonthYear",
                table: "NablComplaints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutcomeOfInvestigation",
                table: "NablComplaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReferenceNoDate",
                table: "NablComplaints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureQM",
                table: "NablComplaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationOfComplaint",
                table: "NablComplaints",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplainantName",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "ComplaintNo",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "MonthYear",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "OutcomeOfInvestigation",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "ReferenceNoDate",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "SignatureQM",
                table: "NablComplaints");

            migrationBuilder.DropColumn(
                name: "ValidationOfComplaint",
                table: "NablComplaints");
        }
    }
}
