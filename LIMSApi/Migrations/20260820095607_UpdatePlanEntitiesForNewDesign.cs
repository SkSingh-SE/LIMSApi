using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlanEntitiesForNewDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedGradeID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedGradeNote",
                table: "SampleDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnknownSample",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ProductMasterID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductSizeMasterID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpecificationGradeID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SpecificationLineID",
                table: "ChemicalTestElements",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestElements",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "ChemicalTestElements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChemicalTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChemicalTestID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UlrNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cancel = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChemicalTestMethods_ChemicalTests_ChemicalTestID",
                        column: x => x.ChemicalTestID,
                        principalTable: "ChemicalTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChemicalTestMethods_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChemicalTestMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_AssignedGradeID",
                table: "SampleDetails",
                column: "AssignedGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ProductMasterID",
                table: "SampleDetails",
                column: "ProductMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ProductSizeMasterID",
                table: "SampleDetails",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_SpecificationGradeID",
                table: "SampleDetails",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestElements_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestElements",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestElements_SpecificationLineID",
                table: "ChemicalTestElements",
                column: "SpecificationLineID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestMethods_ChemicalTestID",
                table: "ChemicalTestMethods",
                column: "ChemicalTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestMethods_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestMethods",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestMethods_TestMethodSpecificationID",
                table: "ChemicalTestMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestElements_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestElements",
                column: "LaboratoryTestAnalysisTypeID",
                principalTable: "LaboratoryTestAnalysisTypes",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTestElements_SpecificationLines_SpecificationLineID",
                table: "ChemicalTestElements",
                column: "SpecificationLineID",
                principalTable: "SpecificationLines",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_ProductMasters_ProductMasterID",
                table: "SampleDetails",
                column: "ProductMasterID",
                principalTable: "ProductMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_ProductSizeMasters_ProductSizeMasterID",
                table: "SampleDetails",
                column: "ProductSizeMasterID",
                principalTable: "ProductSizeMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_SpecificationGrades_AssignedGradeID",
                table: "SampleDetails",
                column: "AssignedGradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_SpecificationGrades_SpecificationGradeID",
                table: "SampleDetails",
                column: "SpecificationGradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestElements_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestElements");

            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTestElements_SpecificationLines_SpecificationLineID",
                table: "ChemicalTestElements");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_ProductMasters_ProductMasterID",
                table: "SampleDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_ProductSizeMasters_ProductSizeMasterID",
                table: "SampleDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_SpecificationGrades_AssignedGradeID",
                table: "SampleDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_SpecificationGrades_SpecificationGradeID",
                table: "SampleDetails");

            migrationBuilder.DropTable(
                name: "ChemicalTestMethods");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_AssignedGradeID",
                table: "SampleDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_ProductMasterID",
                table: "SampleDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_ProductSizeMasterID",
                table: "SampleDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_SpecificationGradeID",
                table: "SampleDetails");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTestElements_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestElements");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTestElements_SpecificationLineID",
                table: "ChemicalTestElements");

            migrationBuilder.DropColumn(
                name: "AssignedGradeID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "AssignedGradeNote",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "IsUnknownSample",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "ProductMasterID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "ProductSizeMasterID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "SpecificationGradeID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestElements");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "ChemicalTestElements");

            migrationBuilder.AlterColumn<long>(
                name: "SpecificationLineID",
                table: "ChemicalTestElements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
