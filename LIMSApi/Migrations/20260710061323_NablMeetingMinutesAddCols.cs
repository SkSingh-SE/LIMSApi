using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablMeetingMinutesAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinutesJson",
                table: "NablMeetingMinutes",
                newName: "OverallConclusion");

            migrationBuilder.AddColumn<string>(
                name: "ActionPlanJson",
                table: "NablMeetingMinutes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgendaItemsJson",
                table: "NablMeetingMinutes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MeetingId",
                table: "NablMeetingMinutes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetingNo",
                table: "NablMeetingMinutes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "MeetingTime",
                table: "NablMeetingMinutes",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetingVenue",
                table: "NablMeetingMinutes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionPlanJson",
                table: "NablMeetingMinutes");

            migrationBuilder.DropColumn(
                name: "AgendaItemsJson",
                table: "NablMeetingMinutes");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "NablMeetingMinutes");

            migrationBuilder.DropColumn(
                name: "MeetingNo",
                table: "NablMeetingMinutes");

            migrationBuilder.DropColumn(
                name: "MeetingTime",
                table: "NablMeetingMinutes");

            migrationBuilder.DropColumn(
                name: "MeetingVenue",
                table: "NablMeetingMinutes");

            migrationBuilder.RenameColumn(
                name: "OverallConclusion",
                table: "NablMeetingMinutes",
                newName: "MinutesJson");
        }
    }
}
