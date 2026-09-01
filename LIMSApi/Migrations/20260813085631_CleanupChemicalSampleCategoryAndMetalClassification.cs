using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class CleanupChemicalSampleCategoryAndMetalClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ChemicalBillingGroup",
                table: "MetalClassificationMasters");

            migrationBuilder.DropColumn(
                name: "HasSpectroSpecialSurcharge",
                table: "MetalClassificationMasters");

            migrationBuilder.DropColumn(
                name: "SpectroElementThreshold",
                table: "MetalClassificationMasters");

            migrationBuilder.DropColumn(
                name: "SurchargeAppliesFromElement",
                table: "MetalClassificationMasters");

            migrationBuilder.AlterColumn<long>(
                name: "ContactPersonID",
                table: "InwardAddresses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChemicalSampleCategoryID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChemicalBillingGroup",
                table: "MetalClassificationMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasSpectroSpecialSurcharge",
                table: "MetalClassificationMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SpectroElementThreshold",
                table: "MetalClassificationMasters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SurchargeAppliesFromElement",
                table: "MetalClassificationMasters",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ContactPersonID",
                table: "InwardAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ChemicalSampleCategories",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
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
    }
}
