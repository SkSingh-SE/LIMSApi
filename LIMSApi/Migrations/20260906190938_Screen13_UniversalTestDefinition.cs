using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Screen13_UniversalTestDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add Code and Description to LaboratoryTests
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "LaboratoryTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LaboratoryTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // 2. Create LaboratoryTestParameters with ON DELETE NO ACTION
            migrationBuilder.CreateTable(
                name: "LaboratoryTestParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "LIMS"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestParameters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            // 3. Create LaboratoryTestMethods with ON DELETE NO ACTION
            migrationBuilder.CreateTable(
                name: "LaboratoryTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "LIMS"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestMethods_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            // 4. Create LaboratoryTestConditions with ON DELETE NO ACTION
            migrationBuilder.CreateTable(
                name: "LaboratoryTestConditions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    ConditionMasterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "LIMS"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestConditions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestConditions_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestConditions_ConditionMasters_ConditionMasterID",
                        column: x => x.ConditionMasterID,
                        principalTable: "ConditionMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            // 5. Unique Indexes
            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_CompanyCode_Code",
                table: "LaboratoryTests",
                columns: new[] { "CompanyCode", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestParameters_LaboratoryTestID_ParameterID",
                table: "LaboratoryTestParameters",
                columns: new[] { "LaboratoryTestID", "ParameterID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestParameters_ParameterID",
                table: "LaboratoryTestParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestMethods_LaboratoryTestID_TestMethodSpecificationID",
                table: "LaboratoryTestMethods",
                columns: new[] { "LaboratoryTestID", "TestMethodSpecificationID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestMethods_TestMethodSpecificationID",
                table: "LaboratoryTestMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestConditions_LaboratoryTestID_ConditionMasterID",
                table: "LaboratoryTestConditions",
                columns: new[] { "LaboratoryTestID", "ConditionMasterID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestConditions_ConditionMasterID",
                table: "LaboratoryTestConditions",
                column: "ConditionMasterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "LaboratoryTestConditions");
            migrationBuilder.DropTable(name: "LaboratoryTestMethods");
            migrationBuilder.DropTable(name: "LaboratoryTestParameters");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTests_CompanyCode_Code",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "LaboratoryTests");
        }
    }
}
