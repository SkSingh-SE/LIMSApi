using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablRetestingAddFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LatestResultPrefix",
                table: "RetestingInitialTestLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LatestResultValue",
                table: "RetestingInitialTestLogs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "RetestingInitialTestLogs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestResultPrefix",
                table: "RetestingInitialTestLogs");

            migrationBuilder.DropColumn(
                name: "LatestResultValue",
                table: "RetestingInitialTestLogs");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "RetestingInitialTestLogs");
        }
    }
}
