using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNametoUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_SubGroup",
                table: "LaboratoryTests",
                column: "SubGroup",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCaseConfigurations_Name",
                table: "InvoiceCaseConfigurations",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LaboratoryTests_SubGroup",
                table: "LaboratoryTests");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceCaseConfigurations_Name",
                table: "InvoiceCaseConfigurations");
        }
    }
}
