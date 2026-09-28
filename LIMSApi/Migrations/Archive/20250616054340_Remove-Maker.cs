using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMasters_MakerMasters_MakerID",
                table: "EquipmentMasters");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentMasters_MakerID",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "MakerID",
                table: "EquipmentMasters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MakerID",
                table: "EquipmentMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_MakerID",
                table: "EquipmentMasters",
                column: "MakerID");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMasters_MakerMasters_MakerID",
                table: "EquipmentMasters",
                column: "MakerID",
                principalTable: "MakerMasters",
                principalColumn: "ID");
        }
    }
}
