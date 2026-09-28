using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProductMasterFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSizeMasterID = table.Column<long>(type: "bigint", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GradePrefix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsSizeApplicable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasters_ProductSizeMasters_ProductSizeMasterID",
                        column: x => x.ProductSizeMasterID,
                        principalTable: "ProductSizeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterMetalClassifications",
                columns: table => new
                {
                    ProductMasterID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterMetalClassifications", x => new { x.ProductMasterID, x.MetalClassificationID });
                    table.ForeignKey(
                        name: "FK_ProductMasterMetalClassifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterMetalClassifications_ProductMasters_ProductMasterID",
                        column: x => x.ProductMasterID,
                        principalTable: "ProductMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterID = table.Column<long>(type: "bigint", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SpecificationFilePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    StandardOrganizationID = table.Column<long>(type: "bigint", nullable: true),
                    SpecStdNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PartSection = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ProductCaption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActiveVersion = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersions_ProductMasters_ProductMasterID",
                        column: x => x.ProductMasterID,
                        principalTable: "ProductMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersions_StandardOrganizationMasters_StandardOrganizationID",
                        column: x => x.StandardOrganizationID,
                        principalTable: "StandardOrganizationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterLinkedSpecs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterVersionID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterLinkedSpecs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasterLinkedSpecs_ProductMasterVersions_ProductMasterVersionID",
                        column: x => x.ProductMasterVersionID,
                        principalTable: "ProductMasterVersions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMasterLinkedSpecs_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterGradeConditionPriorities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterLinkedSpecID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: false),
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID1 = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID2 = table.Column<long>(type: "bigint", nullable: true),
                    ProductSizeMasterID = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterGradeConditionPriorities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasterGradeConditionPriorities_HeatTreatmentMasters_HeatTreatmentID",
                        column: x => x.HeatTreatmentID,
                        principalTable: "HeatTreatmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterGradeConditionPriorities_ProductConditionMasters_ProductConditionID1",
                        column: x => x.ProductConditionID1,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterGradeConditionPriorities_ProductConditionMasters_ProductConditionID2",
                        column: x => x.ProductConditionID2,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterGradeConditionPriorities_ProductMasterLinkedSpecs_ProductMasterLinkedSpecID",
                        column: x => x.ProductMasterLinkedSpecID,
                        principalTable: "ProductMasterLinkedSpecs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMasterGradeConditionPriorities_ProductSizeMasters_ProductSizeMasterID",
                        column: x => x.ProductSizeMasterID,
                        principalTable: "ProductSizeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterGradeConditionPriorities_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterGradeConditionPriorities_HeatTreatmentID",
                table: "ProductMasterGradeConditionPriorities",
                column: "HeatTreatmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterGradeConditionPriorities_ProductConditionID1",
                table: "ProductMasterGradeConditionPriorities",
                column: "ProductConditionID1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterGradeConditionPriorities_ProductConditionID2",
                table: "ProductMasterGradeConditionPriorities",
                column: "ProductConditionID2");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterGradeConditionPriorities_ProductMasterLinkedSpecID",
                table: "ProductMasterGradeConditionPriorities",
                column: "ProductMasterLinkedSpecID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterGradeConditionPriorities_ProductSizeMasterID",
                table: "ProductMasterGradeConditionPriorities",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterGradeConditionPriorities_SpecificationGradeID",
                table: "ProductMasterGradeConditionPriorities",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterLinkedSpecs_ProductMasterVersionID",
                table: "ProductMasterLinkedSpecs",
                column: "ProductMasterVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterLinkedSpecs_SpecificationHeaderID",
                table: "ProductMasterLinkedSpecs",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterMetalClassifications_MetalClassificationID",
                table: "ProductMasterMetalClassifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasters_ProductSizeMasterID",
                table: "ProductMasters",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersions_ProductMasterID",
                table: "ProductMasterVersions",
                column: "ProductMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersions_StandardOrganizationID",
                table: "ProductMasterVersions",
                column: "StandardOrganizationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMasterGradeConditionPriorities");

            migrationBuilder.DropTable(
                name: "ProductMasterMetalClassifications");

            migrationBuilder.DropTable(
                name: "ProductMasterLinkedSpecs");

            migrationBuilder.DropTable(
                name: "ProductMasterVersions");

            migrationBuilder.DropTable(
                name: "ProductMasters");
        }
    }
}
