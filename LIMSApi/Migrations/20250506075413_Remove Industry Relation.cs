using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndustryRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_IndustryMasters_IndustryID",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_IndustryID",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndustryID",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IndustryID",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IndustryID",
                table: "Customers",
                column: "IndustryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_IndustryMasters_IndustryID",
                table: "Customers",
                column: "IndustryID",
                principalTable: "IndustryMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
