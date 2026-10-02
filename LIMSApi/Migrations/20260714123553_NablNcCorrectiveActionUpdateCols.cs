using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablNcCorrectiveActionUpdateCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AuditeeId",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AuditorId",
                table: "NablNcCorrectiveActions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectiveActionTaken",
                table: "NablNcCorrectiveActions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditeeId",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "AuditorId",
                table: "NablNcCorrectiveActions");

            migrationBuilder.DropColumn(
                name: "CorrectiveActionTaken",
                table: "NablNcCorrectiveActions");
        }
    }
}
