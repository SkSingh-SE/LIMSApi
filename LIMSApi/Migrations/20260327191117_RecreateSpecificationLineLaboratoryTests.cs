using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RecreateSpecificationLineLaboratoryTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MappingLaboratoryTests");

            migrationBuilder.DropTable(
                name: "MaterialTestMappings");

            // Recreate SpecificationLineLaboratoryTests which was erroneously dropped in migration
            // 20260324182605_AddConversionFactorAndDecimalFields
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SpecificationLineLaboratoryTests')
                BEGIN
                    CREATE TABLE [SpecificationLineLaboratoryTests] (
                        [SpecificationLineID] bigint NOT NULL,
                        [LaboratoryTestID] bigint NOT NULL,
                        CONSTRAINT [PK_SpecificationLineLaboratoryTests] PRIMARY KEY ([SpecificationLineID], [LaboratoryTestID]),
                        CONSTRAINT [FK_SpecificationLineLaboratoryTests_SpecificationLines_SpecificationLineID]
                            FOREIGN KEY ([SpecificationLineID]) REFERENCES [SpecificationLines] ([ID]) ON DELETE CASCADE
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SpecificationLineLaboratoryTests')
                BEGIN
                    DROP TABLE [SpecificationLineLaboratoryTests];
                END
            ");

            migrationBuilder.CreateTable(
                name: "MaterialTestMappings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeID = table.Column<long>(type: "bigint", nullable: true),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID = table.Column<long>(type: "bigint", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTestMappings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialTestMappings_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MaterialTestMappings_ProductConditionMasters_ProductConditionID",
                        column: x => x.ProductConditionID,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MaterialTestMappings_SpecificationGrades_GradeID",
                        column: x => x.GradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MappingLaboratoryTests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    MaterialTestMappingID = table.Column<long>(type: "bigint", nullable: true),
                    TestMappingID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingLaboratoryTests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MappingLaboratoryTests_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MappingLaboratoryTests_MaterialTestMappings_MaterialTestMappingID",
                        column: x => x.MaterialTestMappingID,
                        principalTable: "MaterialTestMappings",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MappingLaboratoryTests_LaboratoryTestID",
                table: "MappingLaboratoryTests",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_MappingLaboratoryTests_MaterialTestMappingID",
                table: "MappingLaboratoryTests",
                column: "MaterialTestMappingID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTestMappings_GradeID",
                table: "MaterialTestMappings",
                column: "GradeID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTestMappings_MetalClassificationID",
                table: "MaterialTestMappings",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTestMappings_ProductConditionID",
                table: "MaterialTestMappings",
                column: "ProductConditionID");
        }
    }
}
