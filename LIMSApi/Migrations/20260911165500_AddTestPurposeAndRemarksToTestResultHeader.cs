using LIMSApi.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(LIMSContext))]
    [Migration("20260911165500_AddTestPurposeAndRemarksToTestResultHeader")]
    public partial class AddTestPurposeAndRemarksToTestResultHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestPurpose",
                table: "TestResultHeaders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TestResultHeaders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestPurpose",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TestResultHeaders");
        }
    }
}
