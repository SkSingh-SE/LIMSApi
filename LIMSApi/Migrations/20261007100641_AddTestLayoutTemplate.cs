using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTestLayoutTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestLayoutTemplates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LayoutCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LayoutName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Revision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: true),
                    LeafNode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ApplyToLeaf = table.Column<bool>(type: "bit", nullable: false),
                    SectionCount = table.Column<int>(type: "int", nullable: false),
                    ArchetypeSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LayoutConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputPayloadSchemaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLayoutTemplates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestLayoutTemplates_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestLayoutTemplates_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestLayoutTemplates_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestLayoutTemplates_LaboratoryTestAnalysisTypeID",
                table: "TestLayoutTemplates",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_TestLayoutTemplates_LaboratoryTestID",
                table: "TestLayoutTemplates",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_TestLayoutTemplates_LaboratoryTestSubGroupID",
                table: "TestLayoutTemplates",
                column: "LaboratoryTestSubGroupID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestLayoutTemplates");
        }
    }
}
