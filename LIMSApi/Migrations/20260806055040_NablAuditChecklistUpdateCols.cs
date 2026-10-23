using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablAuditChecklistUpdateCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChecklistItemsJson",
                table: "NablAuditChecklists",
                newName: "Remarks");

            migrationBuilder.AddColumn<string>(
                name: "ChecklistStatus",
                table: "NablAuditChecklists",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ScheduleItemId",
                table: "NablAuditChecklists",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChecklistStatus",
                table: "NablAuditChecklists");

            migrationBuilder.DropColumn(
                name: "ScheduleItemId",
                table: "NablAuditChecklists");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "NablAuditChecklists",
                newName: "ChecklistItemsJson");
        }
    }
}
