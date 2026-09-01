using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLinkedSpecAddVersionGrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMasterGradeConditionPriorities");

            migrationBuilder.DropTable(
                name: "ProductMasterLinkedSpecs");

            migrationBuilder.CreateTable(
                name: "ProductMasterVersionGrades",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterVersionID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_ProductMasterVersionGrades", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGrades_ProductMasterVersions_ProductMasterVersionID",
                        column: x => x.ProductMasterVersionID,
                        principalTable: "ProductMasterVersions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGrades_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterVersionGradeConditions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterVersionGradeID = table.Column<long>(type: "bigint", nullable: false),
                    ProductConditionID1 = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID2 = table.Column<long>(type: "bigint", nullable: true),
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_ProductMasterVersionGradeConditions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_HeatTreatmentMasters_HeatTreatmentID",
                        column: x => x.HeatTreatmentID,
                        principalTable: "HeatTreatmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_ProductConditionMasters_ProductConditionID1",
                        column: x => x.ProductConditionID1,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_ProductConditionMasters_ProductConditionID2",
                        column: x => x.ProductConditionID2,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_ProductMasterVersionGrades_ProductMasterVersionGradeID",
                        column: x => x.ProductMasterVersionGradeID,
                        principalTable: "ProductMasterVersionGrades",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_ProductSizeMasters_ProductSizeMasterID",
                        column: x => x.ProductSizeMasterID,
                        principalTable: "ProductSizeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_HeatTreatmentID",
                table: "ProductMasterVersionGradeConditions",
                column: "HeatTreatmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_ProductConditionID1",
                table: "ProductMasterVersionGradeConditions",
                column: "ProductConditionID1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_ProductConditionID2",
                table: "ProductMasterVersionGradeConditions",
                column: "ProductConditionID2");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_ProductMasterVersionGradeID",
                table: "ProductMasterVersionGradeConditions",
                column: "ProductMasterVersionGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_ProductSizeMasterID",
                table: "ProductMasterVersionGradeConditions",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGrades_ProductMasterVersionID",
                table: "ProductMasterVersionGrades",
                column: "ProductMasterVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGrades_SpecificationGradeID",
                table: "ProductMasterVersionGrades",
                column: "SpecificationGradeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMasterVersionGradeConditions");

            migrationBuilder.DropTable(
                name: "ProductMasterVersionGrades");

            migrationBuilder.CreateTable(
                name: "ProductMasterLinkedSpecs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterVersionID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID1 = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID2 = table.Column<long>(type: "bigint", nullable: true),
                    ProductMasterLinkedSpecID = table.Column<long>(type: "bigint", nullable: false),
                    ProductSizeMasterID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
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
        }
    }
}
