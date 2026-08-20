using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablAuditChecklistAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuiteeName",
                table: "NablAuditChecklists",
                newName: "AuditeeName");

            migrationBuilder.AddColumn<string>(
                name: "AuditPlanNo",
                table: "NablAuditChecklists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChecklistNo",
                table: "NablAuditChecklists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditChecklistItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistId = table.Column<long>(type: "bigint", nullable: false),
                    IsoClauseId = table.Column<int>(type: "int", nullable: true),
                    IsoClauseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AuditQuestion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ObjectiveEvidence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FindingType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NcId = table.Column<long>(type: "bigint", nullable: true),
                    NcNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditChecklistItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AuditChecklistItems_NablAuditChecklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "NablAuditChecklists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditChecklistItems_ChecklistId",
                table: "AuditChecklistItems",
                column: "ChecklistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditChecklistItems");

            migrationBuilder.DropColumn(
                name: "AuditPlanNo",
                table: "NablAuditChecklists");

            migrationBuilder.DropColumn(
                name: "ChecklistNo",
                table: "NablAuditChecklists");

            migrationBuilder.RenameColumn(
                name: "AuditeeName",
                table: "NablAuditChecklists",
                newName: "AuiteeName");
        }
    }
}
