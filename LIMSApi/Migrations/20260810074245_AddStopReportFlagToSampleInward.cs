using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStopReportFlagToSampleInward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReportStopped",
                table: "SampleInwards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "StopReportBy",
                table: "SampleInwards",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StopReportOn",
                table: "SampleInwards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StopReportReason",
                table: "SampleInwards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReportStopped",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "StopReportBy",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "StopReportOn",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "StopReportReason",
                table: "SampleInwards");
        }
    }
}
