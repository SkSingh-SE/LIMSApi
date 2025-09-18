using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Createworkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specimen",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestInstructions",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    AssignedToType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedToValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AutoAdvance = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_Workflows_WorkflowID",
                        column: x => x.WorkflowID,
                        principalTable: "Workflows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowActionLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowID = table.Column<long>(type: "bigint", nullable: false),
                    InstanceID = table.Column<long>(type: "bigint", nullable: false),
                    StepID = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeID = table.Column<long>(type: "bigint", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowActionLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkflowActionLogs_EmployeeMasters_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowActionLogs_WorkflowSteps_StepID",
                        column: x => x.StepID,
                        principalTable: "WorkflowSteps",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WorkflowActionLogs_Workflows_WorkflowID",
                        column: x => x.WorkflowID,
                        principalTable: "Workflows",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowID = table.Column<long>(type: "bigint", nullable: false),
                    EntityID = table.Column<long>(type: "bigint", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStepID = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_WorkflowSteps_CurrentStepID",
                        column: x => x.CurrentStepID,
                        principalTable: "WorkflowSteps",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_Workflows_WorkflowID",
                        column: x => x.WorkflowID,
                        principalTable: "Workflows",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTransitions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepID = table.Column<long>(type: "bigint", nullable: false),
                    FromStepID = table.Column<long>(type: "bigint", nullable: false),
                    ToStepID = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTransitions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_WorkflowSteps_StepID",
                        column: x => x.StepID,
                        principalTable: "WorkflowSteps",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActionLogs_EmployeeID",
                table: "WorkflowActionLogs",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActionLogs_StepID",
                table: "WorkflowActionLogs",
                column: "StepID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActionLogs_WorkflowID",
                table: "WorkflowActionLogs",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_CurrentStepID",
                table: "WorkflowInstances",
                column: "CurrentStepID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowID",
                table: "WorkflowInstances",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_WorkflowID",
                table: "WorkflowSteps",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_StepID",
                table: "WorkflowTransitions",
                column: "StepID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowActionLogs");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "WorkflowTransitions");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "Workflows");

            migrationBuilder.DropColumn(
                name: "Specimen",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "TestInstructions",
                table: "SampleDetails");
        }
    }
}
