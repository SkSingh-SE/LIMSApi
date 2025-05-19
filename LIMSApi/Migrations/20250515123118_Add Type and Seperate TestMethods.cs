using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeandSeperateTestMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationLines_ProductConditionMasters_ProductConditionID1",
                table: "SpecificationLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationLines_ProductConditionMasters_ProductConditionID2",
                table: "SpecificationLines");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationLines_ProductConditionID1",
                table: "SpecificationLines");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationLines_ProductConditionID2",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID1",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID2",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "ProductConditionID1",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "ProductConditionID2",
                table: "SpecificationLines");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SpecificationHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SpecificationLineLaboratoryTests",
                columns: table => new
                {
                    SpecificationLineID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationLineLaboratoryTests", x => new { x.SpecificationLineID, x.LaboratoryTestID });
                    table.ForeignKey(
                        name: "FK_SpecificationLineLaboratoryTests_SpecificationLines_SpecificationLineID",
                        column: x => x.SpecificationLineID,
                        principalTable: "SpecificationLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationLineProductConditions",
                columns: table => new
                {
                    SpecificationLineID = table.Column<long>(type: "bigint", nullable: false),
                    ProductConditionID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationLineProductConditions", x => new { x.SpecificationLineID, x.ProductConditionID });
                    table.ForeignKey(
                        name: "FK_SpecificationLineProductConditions_SpecificationLines_SpecificationLineID",
                        column: x => x.SpecificationLineID,
                        principalTable: "SpecificationLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecificationLineLaboratoryTests");

            migrationBuilder.DropTable(
                name: "SpecificationLineProductConditions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SpecificationHeaders");

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID1",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID2",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductConditionID1",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductConditionID2",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ProductConditionID1",
                table: "SpecificationLines",
                column: "ProductConditionID1");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ProductConditionID2",
                table: "SpecificationLines",
                column: "ProductConditionID2");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationLines_ProductConditionMasters_ProductConditionID1",
                table: "SpecificationLines",
                column: "ProductConditionID1",
                principalTable: "ProductConditionMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationLines_ProductConditionMasters_ProductConditionID2",
                table: "SpecificationLines",
                column: "ProductConditionID2",
                principalTable: "ProductConditionMasters",
                principalColumn: "ID");
        }
    }
}
