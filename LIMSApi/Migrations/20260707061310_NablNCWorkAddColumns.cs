using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablNCWorkAddColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CloserDate",
                table: "NablNonConformingWorks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectiveAction",
                table: "NablNonConformingWorks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureTDQM",
                table: "NablNonConformingWorks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloserDate",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "CorrectiveAction",
                table: "NablNonConformingWorks");

            migrationBuilder.DropColumn(
                name: "SignatureTDQM",
                table: "NablNonConformingWorks");
        }
    }
}
