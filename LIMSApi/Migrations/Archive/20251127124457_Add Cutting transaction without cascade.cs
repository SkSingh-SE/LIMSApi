using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCuttingtransactionwithoutcascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CuttingChargeHeaders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingChargeHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingChargeHeaders_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuttingChargeSamples",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuttingChargeHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    SampleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false),
                    SampleTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingChargeSamples", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingChargeSamples_CuttingChargeHeaders_CuttingChargeHeaderID",
                        column: x => x.CuttingChargeHeaderID,
                        principalTable: "CuttingChargeHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuttingChargeSamples_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuttingChargeDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuttingChargeSampleID = table.Column<long>(type: "bigint", nullable: false),
                    CuttingID = table.Column<long>(type: "bigint", nullable: false),
                    CuttingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingChargeDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingChargeDetails_CuttingChargeSamples_CuttingChargeSampleID",
                        column: x => x.CuttingChargeSampleID,
                        principalTable: "CuttingChargeSamples",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuttingChargeDetails_CuttingPriceMasters_CuttingID",
                        column: x => x.CuttingID,
                        principalTable: "CuttingPriceMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeDetails_CuttingChargeSampleID",
                table: "CuttingChargeDetails",
                column: "CuttingChargeSampleID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeDetails_CuttingID",
                table: "CuttingChargeDetails",
                column: "CuttingID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeHeaders_InwardID",
                table: "CuttingChargeHeaders",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeSamples_CuttingChargeHeaderID",
                table: "CuttingChargeSamples",
                column: "CuttingChargeHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeSamples_MetalClassificationID",
                table: "CuttingChargeSamples",
                column: "MetalClassificationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuttingChargeDetails");

            migrationBuilder.DropTable(
                name: "CuttingChargeSamples");

            migrationBuilder.DropTable(
                name: "CuttingChargeHeaders");
        }
    }
}
