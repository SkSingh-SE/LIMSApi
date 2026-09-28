using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class WorkflowUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromStepID",
                table: "WorkflowTransitions");

            migrationBuilder.RenameColumn(
                name: "AutoAdvance",
                table: "WorkflowSteps",
                newName: "IsActive");

            migrationBuilder.AlterColumn<long>(
                name: "ToStepID",
                table: "WorkflowTransitions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "WorkflowTransitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WorkflowTransitions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "WorkflowTransitions");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "WorkflowSteps",
                newName: "AutoAdvance");

            migrationBuilder.AlterColumn<long>(
                name: "ToStepID",
                table: "WorkflowTransitions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FromStepID",
                table: "WorkflowTransitions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
