using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class TestMethodSpecMetalParamsAndVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultVersionID",
                table: "TestMethodSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TestMethodSpecificationMetalClassifications",
                columns: table => new
                {
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodSpecificationMetalClassifications", x => new { x.TestMethodSpecificationID, x.MetalClassificationID });
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationMetalClassifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationMetalClassifications_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestMethodSpecificationParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterUnitEquivalentID = table.Column<long>(type: "bigint", nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodSpecificationParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationParameters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationParameters_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecifications_DefaultVersionID",
                table: "TestMethodSpecifications",
                column: "DefaultVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationMetalClassifications_MetalClassificationID",
                table: "TestMethodSpecificationMetalClassifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationParameters_ParameterID",
                table: "TestMethodSpecificationParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationParameters_ParameterUnitID",
                table: "TestMethodSpecificationParameters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationParameters_TestMethodSpecificationVersionID",
                table: "TestMethodSpecificationParameters",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestMethodSpecifications_TestMethodSpecificationVersions_DefaultVersionID",
                table: "TestMethodSpecifications",
                column: "DefaultVersionID",
                principalTable: "TestMethodSpecificationVersions",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestMethodSpecifications_TestMethodSpecificationVersions_DefaultVersionID",
                table: "TestMethodSpecifications");

            migrationBuilder.DropTable(
                name: "TestMethodSpecificationMetalClassifications");

            migrationBuilder.DropTable(
                name: "TestMethodSpecificationParameters");

            migrationBuilder.DropIndex(
                name: "IX_TestMethodSpecifications_DefaultVersionID",
                table: "TestMethodSpecifications");

            migrationBuilder.DropColumn(
                name: "DefaultVersionID",
                table: "TestMethodSpecifications");
        }
    }
}
