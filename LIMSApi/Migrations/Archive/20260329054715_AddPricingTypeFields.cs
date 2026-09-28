using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingTypeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecificationLineLaboratoryTests");

            migrationBuilder.AddColumn<string>(
                name: "SelectedPricingType",
                table: "TestResultHeaders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultPricingType",
                table: "InvoiceCases",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedPricingType",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "DefaultPricingType",
                table: "InvoiceCases");

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
