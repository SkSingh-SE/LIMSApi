using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AllEnhancements_Phase1To7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CombinedUncertainty",
                table: "TestResultParameters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CoverageFactor",
                table: "TestResultParameters",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpandedUncertainty",
                table: "TestResultParameters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWithinNablScope",
                table: "TestResultParameters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LabScopeSpecParamId",
                table: "TestResultParameters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NablMeasurementUncertaintyId",
                table: "TestResultParameters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NablScopeStatus",
                table: "TestResultParameters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PreparationDataMissing",
                table: "TestResultHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDueDate",
                table: "TaxInvoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerms",
                table: "TaxInvoices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PurchaseOrderId",
                table: "TaxInvoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PurchaseOrderId",
                table: "SampleInwards",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PriceDimensionTypeId",
                table: "InvoiceCaseConfigurations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerPurchaseOrders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PODate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    POAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UtilizedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Terms = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPurchaseOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerPurchaseOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MachineDataLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    TestResultHeaderId = table.Column<long>(type: "bigint", nullable: true),
                    RawPayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchedCount = table.Column<int>(type: "int", nullable: false),
                    UnmatchedCount = table.Column<int>(type: "int", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MachineSerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineDataLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PriceDimensionTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsRange = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceDimensionTypes", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxInvoices_PurchaseOrderId",
                table: "TaxInvoices",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_PurchaseOrderId",
                table: "SampleInwards",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCaseConfigurations_PriceDimensionTypeId",
                table: "InvoiceCaseConfigurations",
                column: "PriceDimensionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchaseOrders_CustomerId",
                table: "CustomerPurchaseOrders",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceCaseConfigurations_PriceDimensionTypes_PriceDimensionTypeId",
                table: "InvoiceCaseConfigurations",
                column: "PriceDimensionTypeId",
                principalTable: "PriceDimensionTypes",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleInwards_CustomerPurchaseOrders_PurchaseOrderId",
                table: "SampleInwards",
                column: "PurchaseOrderId",
                principalTable: "CustomerPurchaseOrders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxInvoices_CustomerPurchaseOrders_PurchaseOrderId",
                table: "TaxInvoices",
                column: "PurchaseOrderId",
                principalTable: "CustomerPurchaseOrders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceCaseConfigurations_PriceDimensionTypes_PriceDimensionTypeId",
                table: "InvoiceCaseConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleInwards_CustomerPurchaseOrders_PurchaseOrderId",
                table: "SampleInwards");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxInvoices_CustomerPurchaseOrders_PurchaseOrderId",
                table: "TaxInvoices");

            migrationBuilder.DropTable(
                name: "CustomerPurchaseOrders");

            migrationBuilder.DropTable(
                name: "MachineDataLogs");

            migrationBuilder.DropTable(
                name: "PriceDimensionTypes");

            migrationBuilder.DropIndex(
                name: "IX_TaxInvoices_PurchaseOrderId",
                table: "TaxInvoices");

            migrationBuilder.DropIndex(
                name: "IX_SampleInwards_PurchaseOrderId",
                table: "SampleInwards");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceCaseConfigurations_PriceDimensionTypeId",
                table: "InvoiceCaseConfigurations");

            migrationBuilder.DropColumn(
                name: "CombinedUncertainty",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "CoverageFactor",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "ExpandedUncertainty",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "IsWithinNablScope",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "LabScopeSpecParamId",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "NablMeasurementUncertaintyId",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "NablScopeStatus",
                table: "TestResultParameters");

            migrationBuilder.DropColumn(
                name: "PreparationDataMissing",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "PaymentDueDate",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "TaxInvoices");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "PriceDimensionTypeId",
                table: "InvoiceCaseConfigurations");
        }
    }
}
