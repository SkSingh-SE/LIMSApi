using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDefaultVersionFK_AddIsDefaultColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestMethodSpecifications_TestMethodSpecificationVersions_DefaultVersionID",
                table: "TestMethodSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_TestMethodSpecificationVersions_TestMethodSpecificationID",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropIndex(
                name: "IX_TestMethodSpecifications_DefaultVersionID",
                table: "TestMethodSpecifications");

            // Step 1: Add IsDefault column before dropping DefaultVersionID
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "TestMethodSpecificationVersions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Step 2: Copy existing default-version data into IsDefault
            migrationBuilder.Sql(@"
                UPDATE v SET v.IsDefault = 1
                FROM dbo.TestMethodSpecificationVersions v
                INNER JOIN dbo.TestMethodSpecifications s ON s.DefaultVersionID = v.ID
                WHERE s.DefaultVersionID IS NOT NULL;
            ");

            // Step 3: Now safe to drop DefaultVersionID
            migrationBuilder.DropColumn(
                name: "DefaultVersionID",
                table: "TestMethodSpecifications");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationVersions_TestMethodSpecificationID_IsDefault",
                table: "TestMethodSpecificationVersions",
                columns: new[] { "TestMethodSpecificationID", "IsDefault" },
                unique: true,
                filter: "[IsDefault] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestMethodSpecificationVersions_TestMethodSpecificationID_IsDefault",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "TestMethodSpecificationVersions");

            migrationBuilder.AddColumn<long>(
                name: "DefaultVersionID",
                table: "TestMethodSpecifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationVersions_TestMethodSpecificationID",
                table: "TestMethodSpecificationVersions",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecifications_DefaultVersionID",
                table: "TestMethodSpecifications",
                column: "DefaultVersionID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestMethodSpecifications_TestMethodSpecificationVersions_DefaultVersionID",
                table: "TestMethodSpecifications",
                column: "DefaultVersionID",
                principalTable: "TestMethodSpecificationVersions",
                principalColumn: "ID");
        }
    }
}
