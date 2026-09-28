using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PurchaseDate",
                table: "EquipmentMasters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ModelNo",
                table: "EquipmentMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquipmentSOP",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentMasterID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSOP", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentSOP_EquipmentMasters_EquipmentMasterID",
                        column: x => x.EquipmentMasterID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSOP_EquipmentMasterID",
                table: "EquipmentSOP",
                column: "EquipmentMasterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentSOP");

            migrationBuilder.DropColumn(
                name: "ModelNo",
                table: "EquipmentMasters");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "EquipmentMasters",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
