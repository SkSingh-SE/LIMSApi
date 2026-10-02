using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablMeetingAgendaAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeetingNo",
                table: "NablMeetingAgendas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "MeetingTime",
                table: "NablMeetingAgendas",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantsJson",
                table: "NablMeetingAgendas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingNo",
                table: "NablMeetingAgendas");

            migrationBuilder.DropColumn(
                name: "MeetingTime",
                table: "NablMeetingAgendas");

            migrationBuilder.DropColumn(
                name: "ParticipantsJson",
                table: "NablMeetingAgendas");
        }
    }
}
