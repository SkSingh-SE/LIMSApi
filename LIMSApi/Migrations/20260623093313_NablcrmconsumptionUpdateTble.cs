using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablcrmconsumptionUpdateTble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablCrmConsumptions_NablReferenceMaterials_ReferenceMaterialId",
                table: "NablCrmConsumptions");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantity",
                table: "NablCrmConsumptions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BatchNo",
                table: "NablCrmConsumptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateNo",
                table: "NablCrmConsumptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialClassification",
                table: "NablCrmConsumptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "NablCrmConsumptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningQuantity",
                table: "NablCrmConsumptions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingQuantity",
                table: "NablCrmConsumptions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalConsumed",
                table: "NablCrmConsumptions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "NablCrmConsumptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidityDate",
                table: "NablCrmConsumptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReferenceMaterialConsumptionLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceMaterialConsumptionId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceMaterialId = table.Column<long>(type: "bigint", nullable: false),
                    ConsumptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityConsumed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousBalanceQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentOrTest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceMaterialConsumptionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferenceMaterialConsumptionLogs_NablCrmConsumptions_ReferenceMaterialConsumptionId",
                        column: x => x.ReferenceMaterialConsumptionId,
                        principalTable: "NablCrmConsumptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferenceMaterialConsumptionLogs_NablReferenceMaterials_ReferenceMaterialId",
                        column: x => x.ReferenceMaterialId,
                        principalTable: "NablReferenceMaterials",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceMaterialConsumptionLogs_ReferenceMaterialConsumptionId",
                table: "ReferenceMaterialConsumptionLogs",
                column: "ReferenceMaterialConsumptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceMaterialConsumptionLogs_ReferenceMaterialId",
                table: "ReferenceMaterialConsumptionLogs",
                column: "ReferenceMaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablCrmConsumptions_NablReferenceMaterials_ReferenceMaterialId",
                table: "NablCrmConsumptions",
                column: "ReferenceMaterialId",
                principalTable: "NablReferenceMaterials",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablCrmConsumptions_NablReferenceMaterials_ReferenceMaterialId",
                table: "NablCrmConsumptions");

            migrationBuilder.DropTable(
                name: "ReferenceMaterialConsumptionLogs");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "BatchNo",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "CertificateNo",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "MaterialClassification",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "OpeningQuantity",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "RemainingQuantity",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "TotalConsumed",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "NablCrmConsumptions");

            migrationBuilder.DropColumn(
                name: "ValidityDate",
                table: "NablCrmConsumptions");

            migrationBuilder.AddForeignKey(
                name: "FK_NablCrmConsumptions_NablReferenceMaterials_ReferenceMaterialId",
                table: "NablCrmConsumptions",
                column: "ReferenceMaterialId",
                principalTable: "NablReferenceMaterials",
                principalColumn: "ID");
        }
    }
}
