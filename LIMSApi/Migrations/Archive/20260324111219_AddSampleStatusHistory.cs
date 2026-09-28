using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestResultHeaders_SampleID",
                table: "TestResultHeaders");

            migrationBuilder.CreateTable(
                name: "SampleStatusHistories",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EntityID = table.Column<long>(type: "bigint", nullable: false),
                    PreviousStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedBy = table.Column<long>(type: "bigint", nullable: false),
                    ChangedByName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChangedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleStatusHistories", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_SampleID_LaboratoryTestID_TestPlanID",
                table: "TestResultHeaders",
                columns: new[] { "SampleID", "LaboratoryTestID", "TestPlanID" },
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_TestResultHeaders_SampleID_LaboratoryTestID_TestPlanID",
                table: "TestResultHeaders");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_SampleID",
                table: "TestResultHeaders",
                column: "SampleID");
        }
    }
}
