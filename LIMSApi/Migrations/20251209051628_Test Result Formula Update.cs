using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class TestResultFormulaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Formula",
                table: "TestResultParameters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "TestMethod",
                table: "ChemicalTests",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_LaboratoryTestID",
                table: "TestResultHeaders",
                column: "LaboratoryTestID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultHeaders_LaboratoryTests_LaboratoryTestID",
                table: "TestResultHeaders",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResultHeaders_LaboratoryTests_LaboratoryTestID",
                table: "TestResultHeaders");

            migrationBuilder.DropIndex(
                name: "IX_TestResultHeaders_LaboratoryTestID",
                table: "TestResultHeaders");

            migrationBuilder.AlterColumn<string>(
                name: "Formula",
                table: "TestResultParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TestMethod",
                table: "ChemicalTests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
