using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class LongTermTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "TestResultHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "TestResultHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StartedBy",
                table: "TestResultHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LongTermTests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LongTermTests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LongTermTests_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LongTermTests_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LongTermRecords",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LongTermTestID = table.Column<long>(type: "bigint", nullable: false),
                    DataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LongTermRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LongTermRecords_LongTermTests_LongTermTestID",
                        column: x => x.LongTermTestID,
                        principalTable: "LongTermTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_MetalClassificationID",
                table: "SampleDetails",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ProductConditionID",
                table: "SampleDetails",
                column: "ProductConditionID");

            migrationBuilder.CreateIndex(
                name: "IX_LongTermRecords_LongTermTestID",
                table: "LongTermRecords",
                column: "LongTermTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LongTermTests_SampleID",
                table: "LongTermTests",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_LongTermTests_TestResultHeaderID",
                table: "LongTermTests",
                column: "TestResultHeaderID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_MetalClassificationMasters_MetalClassificationID",
                table: "SampleDetails",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_ProductConditionMasters_ProductConditionID",
                table: "SampleDetails",
                column: "ProductConditionID",
                principalTable: "ProductConditionMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_MetalClassificationMasters_MetalClassificationID",
                table: "SampleDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_ProductConditionMasters_ProductConditionID",
                table: "SampleDetails");

            migrationBuilder.DropTable(
                name: "LongTermRecords");

            migrationBuilder.DropTable(
                name: "LongTermTests");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_MetalClassificationID",
                table: "SampleDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_ProductConditionID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "StartedBy",
                table: "TestResultHeaders");
        }
    }
}
