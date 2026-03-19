using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class MergeTestMasterIntoLaboratoryTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTests_TestMasters_TestMasterID",
                table: "LaboratoryTests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGroupMappings_TestMasters_TestID",
                table: "TestGroupMappings");

            migrationBuilder.DropIndex(
                name: "IX_TestGroupMappings_TestID",
                table: "TestGroupMappings");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTests_TestMasterID",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "TestID",
                table: "TestGroupMappings");

            migrationBuilder.DropColumn(
                name: "TestMasterID",
                table: "LaboratoryTests");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCaption",
                table: "LaboratoryTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestCaption",
                table: "LaboratoryTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestDuration",
                table: "LaboratoryTests",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceCaption",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "TestCaption",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "TestDuration",
                table: "LaboratoryTests");

            migrationBuilder.AddColumn<long>(
                name: "TestID",
                table: "TestGroupMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TestMasterID",
                table: "LaboratoryTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestGroupMappings_TestID",
                table: "TestGroupMappings",
                column: "TestID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_TestMasterID",
                table: "LaboratoryTests",
                column: "TestMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTests_TestMasters_TestMasterID",
                table: "LaboratoryTests",
                column: "TestMasterID",
                principalTable: "TestMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestGroupMappings_TestMasters_TestID",
                table: "TestGroupMappings",
                column: "TestID",
                principalTable: "TestMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
