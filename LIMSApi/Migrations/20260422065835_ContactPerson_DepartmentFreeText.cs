using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ContactPerson_DepartmentFreeText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactPersons_DepartmentMasters_DepartmentID",
                table: "ContactPersons");

            migrationBuilder.DropIndex(
                name: "IX_ContactPersons_DepartmentID",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "ContactPersons");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "ContactPersons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "ContactPersons");

            migrationBuilder.AddColumn<long>(
                name: "DepartmentID",
                table: "ContactPersons",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_DepartmentID",
                table: "ContactPersons",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactPersons_DepartmentMasters_DepartmentID",
                table: "ContactPersons",
                column: "DepartmentID",
                principalTable: "DepartmentMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
