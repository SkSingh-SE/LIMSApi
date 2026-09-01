using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMetalChemicalBillingAndTechniques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "MetalClassificationAnalysisTechniques",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetalClassificationAnalysisTechniques", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MetalClassificationAnalysisTechniques_AnalysisTechniqueMasters_AnalysisTechniqueID",
                        column: x => x.AnalysisTechniqueID,
                        principalTable: "AnalysisTechniqueMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MetalClassificationAnalysisTechniques_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationAnalysisTechniques_AnalysisTechniqueID",
                table: "MetalClassificationAnalysisTechniques",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationAnalysisTechniques_MetalClassificationID",
                table: "MetalClassificationAnalysisTechniques",
                column: "MetalClassificationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetalClassificationAnalysisTechniques");

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
        }
    }
}
