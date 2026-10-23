using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTestMethodStandardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablTestMethods_TestMethodStandards_TestMethodStandardId",
                table: "NablTestMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_TestMethodStandards_DefaultTestMethodID",
                table: "ParameterMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTestGroups_TestMethodStandards_TestMethodStandardID",
                table: "ProductTestGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SamplePreparationMasters_TestMethodStandards_TestMethodStandardID",
                table: "SamplePreparationMasters");

            migrationBuilder.DropTable(
                name: "TestMethodStandards");

            migrationBuilder.AddForeignKey(
                name: "FK_NablTestMethods_TestMethodSpecifications_TestMethodStandardId",
                table: "NablTestMethods",
                column: "TestMethodStandardId",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_TestMethodSpecifications_DefaultTestMethodID",
                table: "ParameterMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTestGroups_TestMethodSpecifications_TestMethodStandardID",
                table: "ProductTestGroups",
                column: "TestMethodStandardID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SamplePreparationMasters_TestMethodSpecifications_TestMethodStandardID",
                table: "SamplePreparationMasters",
                column: "TestMethodStandardID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablTestMethods_TestMethodSpecifications_TestMethodStandardId",
                table: "NablTestMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_TestMethodSpecifications_DefaultTestMethodID",
                table: "ParameterMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTestGroups_TestMethodSpecifications_TestMethodStandardID",
                table: "ProductTestGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SamplePreparationMasters_TestMethodSpecifications_TestMethodStandardID",
                table: "SamplePreparationMasters");

            migrationBuilder.CreateTable(
                name: "TestMethodStandards",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    StandardOrganisationID = table.Column<long>(type: "bigint", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Group = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ParameterUnits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubGroup = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TestCategory = table.Column<string>(type: "varchar(20)", nullable: false),
                    TestMethodCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnderNABL = table.Column<bool>(type: "bit", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodStandards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodStandards_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestMethodStandards_StandardOrganizationMasters_StandardOrganisationID",
                        column: x => x.StandardOrganisationID,
                        principalTable: "StandardOrganizationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodStandards_EquipmentID",
                table: "TestMethodStandards",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodStandards_StandardOrganisationID",
                table: "TestMethodStandards",
                column: "StandardOrganisationID");

            migrationBuilder.AddForeignKey(
                name: "FK_NablTestMethods_TestMethodStandards_TestMethodStandardId",
                table: "NablTestMethods",
                column: "TestMethodStandardId",
                principalTable: "TestMethodStandards",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_TestMethodStandards_DefaultTestMethodID",
                table: "ParameterMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodStandards",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTestGroups_TestMethodStandards_TestMethodStandardID",
                table: "ProductTestGroups",
                column: "TestMethodStandardID",
                principalTable: "TestMethodStandards",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SamplePreparationMasters_TestMethodStandards_TestMethodStandardID",
                table: "SamplePreparationMasters",
                column: "TestMethodStandardID",
                principalTable: "TestMethodStandards",
                principalColumn: "ID");
        }
    }
}
