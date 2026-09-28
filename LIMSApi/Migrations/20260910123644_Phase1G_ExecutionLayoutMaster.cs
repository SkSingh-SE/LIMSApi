using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase1G_ExecutionLayoutMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExecutionLayoutMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RendererType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionLayoutMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionLayoutSections",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionLayoutID = table.Column<long>(type: "bigint", nullable: false),
                    SectionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SectionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    IsCollapsible = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionLayoutSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExecutionLayoutSections_ExecutionLayoutMasters_ExecutionLayoutID",
                        column: x => x.ExecutionLayoutID,
                        principalTable: "ExecutionLayoutMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionLayoutItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionLayoutSectionID = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    DisplayLabel = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionLayoutItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ExecutionLayoutItems_ExecutionLayoutSections_ExecutionLayoutSectionID",
                        column: x => x.ExecutionLayoutSectionID,
                        principalTable: "ExecutionLayoutSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionLayoutItems_Section_RefType_RefId",
                table: "ExecutionLayoutItems",
                columns: new[] { "ExecutionLayoutSectionID", "ReferenceType", "ReferenceID" },
                unique: true,
                filter: "[ReferenceID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionLayoutMasters_CompanyCode_Code",
                table: "ExecutionLayoutMasters",
                columns: new[] { "CompanyCode", "Code" },
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionLayoutSections_Layout_SectionCode",
                table: "ExecutionLayoutSections",
                columns: new[] { "ExecutionLayoutID", "SectionCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExecutionLayoutItems");

            migrationBuilder.DropTable(
                name: "ExecutionLayoutSections");

            migrationBuilder.DropTable(
                name: "ExecutionLayoutMasters");
        }
    }
}
