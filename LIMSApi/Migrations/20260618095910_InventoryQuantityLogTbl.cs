using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class InventoryQuantityLogTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryQuantityLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryId = table.Column<long>(type: "bigint", nullable: false),
                    AddedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedBy = table.Column<long>(type: "bigint", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryQuantityLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InventoryQuantityLogs_InventoryManagements_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "InventoryManagements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryQuantityLogs_InventoryId",
                table: "InventoryQuantityLogs",
                column: "InventoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryQuantityLogs");
        }
    }
}
