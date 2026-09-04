using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecimenPreparationTestItemsAndExtendMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID')
                    ALTER TABLE [dbo].[MachiningChargeMasters] DROP CONSTRAINT [FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID];
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID')
                    ALTER TABLE [dbo].[MachiningChargeMasters] DROP CONSTRAINT [FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MachiningChargeMasters_LaboratoryTestID' AND object_id = OBJECT_ID('MachiningChargeMasters'))
                    DROP INDEX [IX_MachiningChargeMasters_LaboratoryTestID] ON [dbo].[MachiningChargeMasters];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MachiningChargeMasters_TestMethodStandardID' AND object_id = OBJECT_ID('MachiningChargeMasters'))
                    DROP INDEX [IX_MachiningChargeMasters_TestMethodStandardID] ON [dbo].[MachiningChargeMasters];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'MetalClassificationParameter')
                BEGIN
                    IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_MetalClassificationParameter')
                        ALTER TABLE [dbo].[MetalClassificationParameter] DROP CONSTRAINT [PK_MetalClassificationParameter];
                    EXEC sp_rename 'MetalClassificationParameter', 'MetalClassificationParameters';
                END
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MetalClassificationParameter_ParameterID' AND object_id = OBJECT_ID('MetalClassificationParameters'))
                    EXEC sp_rename 'MetalClassificationParameters.IX_MetalClassificationParameter_ParameterID', 'IX_MetalClassificationParameters_ParameterID', 'INDEX';
            ");

            migrationBuilder.AddColumn<decimal>(
                name: "CutThickness",
                table: "SamplePreparations",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EdmCutting",
                table: "SamplePreparations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EdmCuttingCharge",
                table: "SamplePreparations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GasCutting",
                table: "SamplePreparations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GasCuttingCharge",
                table: "SamplePreparations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCuts",
                table: "SamplePreparations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialCutting",
                table: "SamplePreparations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecialCuttingCharge",
                table: "SamplePreparations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WaterJetCuttingMins",
                table: "SamplePreparations",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CuttingInstructions",
                table: "MachiningChargeMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CuttingRequired",
                table: "MachiningChargeMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MachiningInstructions",
                table: "MachiningChargeMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MachiningRequired",
                table: "MachiningChargeMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "MetalClassificationID",
                table: "MachiningChargeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OtherPreparation",
                table: "MachiningChargeMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PreparationRequired",
                table: "MachiningChargeMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ProductMasterID",
                table: "MachiningChargeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecimenQuantity",
                table: "MachiningChargeMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetalClassificationParameters",
                table: "MetalClassificationParameters",
                columns: new[] { "MetalClassificationID", "ParameterID" });

            migrationBuilder.CreateTable(
                name: "SamplePreparationTestItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SamplePreparationID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    TestPlanID = table.Column<long>(type: "bigint", nullable: true),
                    PlannedTestType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlannedTestMethodID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    SpecimenPreparationMasterID = table.Column<long>(type: "bigint", nullable: true),
                    SpecimenSize = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SpecimenRawMaterialSize = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DrawingFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CuttingRequired = table.Column<bool>(type: "bit", nullable: false),
                    MachiningRequired = table.Column<bool>(type: "bit", nullable: false),
                    NoTesting = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedCuttingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ResolvedMachiningRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CuttingTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MachiningTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedByEmployeeID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamplePreparationTestItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SamplePreparationTestItems_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparationTestItems_MachiningChargeMasters_SpecimenPreparationMasterID",
                        column: x => x.SpecimenPreparationMasterID,
                        principalTable: "MachiningChargeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparationTestItems_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparationTestItems_SamplePreparations_SamplePreparationID",
                        column: x => x.SamplePreparationID,
                        principalTable: "SamplePreparations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeMasters_MetalClassificationID",
                table: "MachiningChargeMasters",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeMasters_ProductMasterID",
                table: "MachiningChargeMasters",
                column: "ProductMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparationTestItems_LaboratoryTestID",
                table: "SamplePreparationTestItems",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparationTestItems_SampleID",
                table: "SamplePreparationTestItems",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparationTestItems_SamplePreparationID",
                table: "SamplePreparationTestItems",
                column: "SamplePreparationID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparationTestItems_SpecimenPreparationMasterID",
                table: "SamplePreparationTestItems",
                column: "SpecimenPreparationMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_MachiningChargeMasters_MetalClassificationMasters_MetalClassificationID",
                table: "MachiningChargeMasters",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MachiningChargeMasters_ProductMasters_ProductMasterID",
                table: "MachiningChargeMasters",
                column: "ProductMasterID",
                principalTable: "ProductMasters",
                principalColumn: "ID");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MetalClassificationParameters_MetalClassificationMasters_MetalClassificationID')
                    ALTER TABLE [dbo].[MetalClassificationParameters] ADD CONSTRAINT [FK_MetalClassificationParameters_MetalClassificationMasters_MetalClassificationID] FOREIGN KEY ([MetalClassificationID]) REFERENCES [MetalClassificationMasters] ([ID]) ON DELETE NO ACTION;
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MetalClassificationParameters_ParameterMasters_ParameterID')
                    ALTER TABLE [dbo].[MetalClassificationParameters] ADD CONSTRAINT [FK_MetalClassificationParameters_ParameterMasters_ParameterID] FOREIGN KEY ([ParameterID]) REFERENCES [ParameterMasters] ([ID]) ON DELETE NO ACTION;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachiningChargeMasters_MetalClassificationMasters_MetalClassificationID",
                table: "MachiningChargeMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_MachiningChargeMasters_ProductMasters_ProductMasterID",
                table: "MachiningChargeMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameters_MetalClassificationMasters_MetalClassificationID",
                table: "MetalClassificationParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameters_ParameterMasters_ParameterID",
                table: "MetalClassificationParameters");

            migrationBuilder.DropTable(
                name: "SamplePreparationTestItems");

            migrationBuilder.DropIndex(
                name: "IX_MachiningChargeMasters_MetalClassificationID",
                table: "MachiningChargeMasters");

            migrationBuilder.DropIndex(
                name: "IX_MachiningChargeMasters_ProductMasterID",
                table: "MachiningChargeMasters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetalClassificationParameters",
                table: "MetalClassificationParameters");

            migrationBuilder.DropColumn(
                name: "CutThickness",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "EdmCutting",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "EdmCuttingCharge",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "GasCutting",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "GasCuttingCharge",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "NumberOfCuts",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "SpecialCutting",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "SpecialCuttingCharge",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "WaterJetCuttingMins",
                table: "SamplePreparations");

            migrationBuilder.DropColumn(
                name: "CuttingInstructions",
                table: "MachiningChargeMasters");

            migrationBuilder.DropColumn(
                name: "CuttingRequired",
                table: "MachiningChargeMasters");

            migrationBuilder.DropColumn(
                name: "MachiningInstructions",
                table: "MachiningChargeMasters");

            migrationBuilder.DropColumn(
                name: "MachiningRequired",
                table: "MachiningChargeMasters");

            migrationBuilder.DropColumn(
                name: "MetalClassificationID",
                table: "MachiningChargeMasters");

            migrationBuilder.DropColumn(
                name: "OtherPreparation",
                table: "MachiningChargeMasters");

            migrationBuilder.DropColumn(
                name: "PreparationRequired",
                table: "MachiningChargeMasters");

            migrationBuilder.DropColumn(
                name: "ProductMasterID",
                table: "MachiningChargeMasters");

            migrationBuilder.DropColumn(
                name: "SpecimenQuantity",
                table: "MachiningChargeMasters");

            migrationBuilder.RenameTable(
                name: "MetalClassificationParameters",
                newName: "MetalClassificationParameter");

            migrationBuilder.RenameIndex(
                name: "IX_MetalClassificationParameters_ParameterID",
                table: "MetalClassificationParameter",
                newName: "IX_MetalClassificationParameter_ParameterID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetalClassificationParameter",
                table: "MetalClassificationParameter",
                columns: new[] { "MetalClassificationID", "ParameterID" });

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeMasters_LaboratoryTestID",
                table: "MachiningChargeMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeMasters_TestMethodStandardID",
                table: "MachiningChargeMasters",
                column: "TestMethodStandardID");

            migrationBuilder.AddForeignKey(
                name: "FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID",
                table: "MachiningChargeMasters",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID",
                table: "MachiningChargeMasters",
                column: "TestMethodStandardID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameter_MetalClassificationMasters_MetalClassificationID",
                table: "MetalClassificationParameter",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameter_ParameterMasters_ParameterID",
                table: "MetalClassificationParameter",
                column: "ParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
