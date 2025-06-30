using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLabTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceCase",
                table: "LaboratoryTests");

            migrationBuilder.AddColumn<string>(
                name: "Equation",
                table: "LaboratoryTests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboratoryTestInvoiceCase",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestID = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestInvoiceCase", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestInvoiceCase_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestInvoiceCase_LaboratoryTestID",
                table: "LaboratoryTestInvoiceCase",
                column: "LaboratoryTestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaboratoryTestInvoiceCase");

            migrationBuilder.DropColumn(
                name: "Equation",
                table: "LaboratoryTests");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCase",
                table: "LaboratoryTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
