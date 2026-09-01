using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ChemicalSampleCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChemicalSampleCategoryID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChemicalSampleCategories",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalSampleCategories", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ChemicalSampleCategoryID",
                table: "SampleDetails",
                column: "ChemicalSampleCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_ChemicalSampleCategories_ChemicalSampleCategoryID",
                table: "SampleDetails",
                column: "ChemicalSampleCategoryID",
                principalTable: "ChemicalSampleCategories",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_ChemicalSampleCategories_ChemicalSampleCategoryID",
                table: "SampleDetails");

            migrationBuilder.DropTable(
                name: "ChemicalSampleCategories");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_ChemicalSampleCategoryID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "ChemicalSampleCategoryID",
                table: "SampleDetails");
        }
    }
}
