using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSpecificationGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TestMethodSpecificationID",
                table: "SpecificationGrades",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationGrades_TestMethodSpecificationID",
                table: "SpecificationGrades",
                column: "TestMethodSpecificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationGrades_TestMethodSpecifications_TestMethodSpecificationID",
                table: "SpecificationGrades",
                column: "TestMethodSpecificationID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationGrades_TestMethodSpecifications_TestMethodSpecificationID",
                table: "SpecificationGrades");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationGrades_TestMethodSpecificationID",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "TestMethodSpecificationID",
                table: "SpecificationGrades");
        }
    }
}
