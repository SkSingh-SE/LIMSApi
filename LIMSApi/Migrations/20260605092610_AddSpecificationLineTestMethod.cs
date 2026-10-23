using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecificationLineTestMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductSizeMasterID",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestCondition",
                table: "SpecificationLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestNote",
                table: "SpecificationLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentifierConfigJson",
                table: "SpecificationHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentifierValuesJson",
                table: "SpecificationGrades",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpecificationLineTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationLineID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    NumberOfTestSpecimen = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationLineTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationLineTestMethods_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLineTestMethods_SpecificationLines_SpecificationLineID",
                        column: x => x.SpecificationLineID,
                        principalTable: "SpecificationLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecificationLineTestMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ProductSizeMasterID",
                table: "SpecificationLines",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineTestMethods_LaboratoryTestID",
                table: "SpecificationLineTestMethods",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineTestMethods_SpecificationLineID",
                table: "SpecificationLineTestMethods",
                column: "SpecificationLineID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineTestMethods_TestMethodSpecificationID",
                table: "SpecificationLineTestMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationLines_ProductSizeMasters_ProductSizeMasterID",
                table: "SpecificationLines",
                column: "ProductSizeMasterID",
                principalTable: "ProductSizeMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationLines_ProductSizeMasters_ProductSizeMasterID",
                table: "SpecificationLines");

            migrationBuilder.DropTable(
                name: "SpecificationLineTestMethods");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationLines_ProductSizeMasterID",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "ProductSizeMasterID",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "TestCondition",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "TestNote",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "IdentifierConfigJson",
                table: "SpecificationHeaders");

            migrationBuilder.DropColumn(
                name: "IdentifierValuesJson",
                table: "SpecificationGrades");
        }
    }
}
