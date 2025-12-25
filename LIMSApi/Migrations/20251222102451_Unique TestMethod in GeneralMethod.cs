using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UniqueTestMethodinGeneralMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TestMethodID",
                table: "GeneralTestMethods",
                newName: "LaboratoryTestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LaboratoryTestID",
                table: "GeneralTestMethods",
                newName: "TestMethodID");
        }
    }
}
