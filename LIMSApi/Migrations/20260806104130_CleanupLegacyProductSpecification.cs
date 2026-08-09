using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class CleanupLegacyProductSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fail-safe Foreign Keys Drop
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LaboratoryTestAnalysisTypeSpecifications_ProductSpecifications_ProductSpecificationID')
                BEGIN
                    ALTER TABLE [LaboratoryTestAnalysisTypeSpecifications] DROP CONSTRAINT [FK_LaboratoryTestAnalysisTypeSpecifications_ProductSpecifications_ProductSpecificationID];
                END
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LaboratoryTestSubGroupSpecifications_ProductSpecifications_ProductSpecificationID')
                BEGIN
                    ALTER TABLE [LaboratoryTestSubGroupSpecifications] DROP CONSTRAINT [FK_LaboratoryTestSubGroupSpecifications_ProductSpecifications_ProductSpecificationID];
                END
            ");

            // Fail-safe Tables Drop
            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.ProductSpecificationGrades', 'U') IS NOT NULL DROP TABLE dbo.ProductSpecificationGrades;
                IF OBJECT_ID('dbo.ProductTestGroups', 'U') IS NOT NULL DROP TABLE dbo.ProductTestGroups;
                IF OBJECT_ID('dbo.ProductSpecifications', 'U') IS NOT NULL DROP TABLE dbo.ProductSpecifications;
            ");

            // Fail-safe Indexes Drop
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LaboratoryTestSubGroupSpecifications_ProductSpecificationID' AND object_id = OBJECT_ID('LaboratoryTestSubGroupSpecifications'))
                BEGIN
                    DROP INDEX [IX_LaboratoryTestSubGroupSpecifications_ProductSpecificationID] ON [LaboratoryTestSubGroupSpecifications];
                END
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LaboratoryTestAnalysisTypeSpecifications_ProductSpecificationID' AND object_id = OBJECT_ID('LaboratoryTestAnalysisTypeSpecifications'))
                BEGIN
                    DROP INDEX [IX_LaboratoryTestAnalysisTypeSpecifications_ProductSpecificationID] ON [LaboratoryTestAnalysisTypeSpecifications];
                END
            ");

            // Fail-safe Columns Drop
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'ProductSpecificationID' AND object_id = OBJECT_ID('LaboratoryTestSubGroupSpecifications'))
                BEGIN
                    ALTER TABLE [LaboratoryTestSubGroupSpecifications] DROP COLUMN [ProductSpecificationID];
                END
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'ProductSpecificationID' AND object_id = OBJECT_ID('LaboratoryTestAnalysisTypeSpecifications'))
                BEGIN
                    ALTER TABLE [LaboratoryTestAnalysisTypeSpecifications] DROP COLUMN [ProductSpecificationID];
                END
            ");

            // Fail-safe ProductMasterID Columns Add
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ProductMasterID' AND object_id = OBJECT_ID('LaboratoryTestSubGroupSpecifications'))
                BEGIN
                    ALTER TABLE [LaboratoryTestSubGroupSpecifications] ADD [ProductMasterID] bigint NULL;
                END
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ProductMasterID' AND object_id = OBJECT_ID('LaboratoryTestAnalysisTypeSpecifications'))
                BEGIN
                    ALTER TABLE [LaboratoryTestAnalysisTypeSpecifications] ADD [ProductMasterID] bigint NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductSpecificationID",
                table: "LaboratoryTestSubGroupSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductSpecificationID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true),
                    AliasName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsCustom = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecificationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpecificationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_SpecificationGrades_GradeID",
                        column: x => x.GradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecificationGrades",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: false),
                    AliasName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecificationGrades", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductSpecificationGrades_ProductSpecifications_ProductSpecificationID",
                        column: x => x.ProductSpecificationID,
                        principalTable: "ProductSpecifications",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductSpecificationGrades_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProductTestGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    ProductSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodStandardID = table.Column<long>(type: "bigint", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPerBatch = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTestGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductTestGroups_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductTestGroups_ProductSpecifications_ProductSpecificationID",
                        column: x => x.ProductSpecificationID,
                        principalTable: "ProductSpecifications",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductTestGroups_TestMethodSpecifications_TestMethodStandardID",
                        column: x => x.TestMethodStandardID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_ProductSpecificationID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_ProductSpecificationID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationGrades_ProductSpecificationID",
                table: "ProductSpecificationGrades",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationGrades_SpecificationGradeID",
                table: "ProductSpecificationGrades",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_GradeID",
                table: "ProductSpecifications",
                column: "GradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_LaboratoryTestID",
                table: "ProductSpecifications",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_MetalClassificationID",
                table: "ProductSpecifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_TestMethodSpecificationID",
                table: "ProductSpecifications",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_TestMethodSpecificationVersionID",
                table: "ProductSpecifications",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTestGroups_LaboratoryTestID",
                table: "ProductTestGroups",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTestGroups_ProductSpecificationID",
                table: "ProductTestGroups",
                column: "ProductSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTestGroups_TestMethodStandardID",
                table: "ProductTestGroups",
                column: "TestMethodStandardID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestAnalysisTypeSpecifications_ProductSpecifications_ProductSpecificationID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "ProductSpecificationID",
                principalTable: "ProductSpecifications",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestSubGroupSpecifications_ProductSpecifications_ProductSpecificationID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "ProductSpecificationID",
                principalTable: "ProductSpecifications",
                principalColumn: "ID");
        }
    }
}
