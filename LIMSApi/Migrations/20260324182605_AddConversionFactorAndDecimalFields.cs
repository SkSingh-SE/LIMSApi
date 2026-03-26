using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddConversionFactorAndDecimalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecificationLineLaboratoryTests");

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor",
                table: "TestResultParameters",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DecimalPrecision",
                table: "TestResultParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionFactor",
                table: "ParameterMasters",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversionFactor",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "DecimalPrecision",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "ConversionFactor",
                table: "ParameterMasters");

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
        }
    }
}
