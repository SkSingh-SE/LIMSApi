using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddColinTrainingAttendanceTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenearalRemarks",
                table: "NablTrainingAttendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrainingDatetime",
                table: "NablTrainingAttendances",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenearalRemarks",
                table: "NablTrainingAttendances");

            migrationBuilder.DropColumn(
                name: "TrainingDatetime",
                table: "NablTrainingAttendances");
        }
    }
}
