using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationCinWebsiteMobile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CIN",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "Organizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecifications_TestMethodSpecificationID",
                table: "LabScopeSpecifications",
                column: "TestMethodSpecificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LabScopeSpecifications_TestMethodSpecifications_TestMethodSpecificationID",
                table: "LabScopeSpecifications",
                column: "TestMethodSpecificationID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabScopeSpecifications_TestMethodSpecifications_TestMethodSpecificationID",
                table: "LabScopeSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_LabScopeSpecifications_TestMethodSpecificationID",
                table: "LabScopeSpecifications");

            migrationBuilder.DropColumn(
                name: "CIN",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Organizations");
        }
    }
}
