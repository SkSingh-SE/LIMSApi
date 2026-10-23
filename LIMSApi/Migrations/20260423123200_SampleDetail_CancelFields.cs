using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class SampleDetail_CancelFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Disabled",
                table: "SampleDetails",
                newName: "IsCancelled");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "SampleDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CancelledBy",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledOn",
                table: "SampleDetails",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "CancelledOn",
                table: "SampleDetails");

            migrationBuilder.RenameColumn(
                name: "IsCancelled",
                table: "SampleDetails",
                newName: "Disabled");
        }
    }
}
