using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Wave1Enhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcceptanceCriteria",
                table: "TestResultParameters",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DependsOnParamsJson",
                table: "TestResultParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormulaExpression",
                table: "TestResultParameters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStandalone",
                table: "TestResultParameters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResultStatus",
                table: "TestResultParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SourceTestMethodId",
                table: "TestResultParameters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecMaxValue",
                table: "TestResultParameters",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecMinValue",
                table: "TestResultParameters",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CalculatedPrice",
                table: "TestResultHeaders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentIdsJson",
                table: "TestResultHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LabRoomId",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OverrideById",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverridePrice",
                table: "TestResultHeaders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverrideReason",
                table: "TestResultHeaders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PerformedById",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformedByName",
                table: "TestResultHeaders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PriceOverridden",
                table: "TestResultHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RoomHumidity",
                table: "TestResultHeaders",
                type: "decimal(6,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RoomTemperature",
                table: "TestResultHeaders",
                type: "decimal(6,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TestEndTime",
                table: "TestResultHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TestStartTime",
                table: "TestResultHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "TestPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApprovedById",
                table: "TestPlans",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByName",
                table: "TestPlans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanStatus",
                table: "TestPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReplanCount",
                table: "TestPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "TestPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "RoleID",
                table: "DesignationMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlanHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedById = table.Column<long>(type: "bigint", nullable: false),
                    ChangedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FieldChangesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanHistories_TestPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "TestPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReplanRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedById = table.Column<long>(type: "bigint", nullable: false),
                    RequestedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplanRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplanRequests_TestPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "TestPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DesignationMasters_RoleID",
                table: "DesignationMasters",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanHistories_PlanId",
                table: "PlanHistories",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplanRequests_PlanId",
                table: "ReplanRequests",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignationMasters_RoleMasters_RoleID",
                table: "DesignationMasters",
                column: "RoleID",
                principalTable: "RoleMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DesignationMasters_RoleMasters_RoleID",
                table: "DesignationMasters");

            migrationBuilder.DropTable(
                name: "PlanHistories");

            migrationBuilder.DropTable(
                name: "ReplanRequests");

            migrationBuilder.DropIndex(
                name: "IX_DesignationMasters_RoleID",
                table: "DesignationMasters");

            migrationBuilder.DropColumn(
                name: "AcceptanceCriteria",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "DependsOnParamsJson",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "FormulaExpression",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "IsStandalone",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "ResultStatus",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "SourceTestMethodId",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "SpecMaxValue",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "SpecMinValue",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "CalculatedPrice",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "EquipmentIdsJson",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "LabRoomId",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "OverrideById",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "OverridePrice",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "OverrideReason",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "PerformedById",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "PerformedByName",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "PriceOverridden",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "RoomHumidity",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "RoomTemperature",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "TestEndTime",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "TestStartTime",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "TestPlans");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "TestPlans");

            migrationBuilder.DropColumn(
                name: "ApprovedByName",
                table: "TestPlans");

            migrationBuilder.DropColumn(
                name: "PlanStatus",
                table: "TestPlans");

            migrationBuilder.DropColumn(
                name: "ReplanCount",
                table: "TestPlans");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "TestPlans");

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "DesignationMasters");
        }
    }
}
