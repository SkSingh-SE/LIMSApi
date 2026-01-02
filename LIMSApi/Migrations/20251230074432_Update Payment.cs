using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAmendmentAllowed",
                table: "SampleInwards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReportUnlocked",
                table: "SampleInwards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PIReceived",
                table: "SampleInwards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReportUnlocked",
                table: "SampleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAmendmentAllowed",
                table: "Reports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: true),
                    CaseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleID = table.Column<long>(type: "bigint", nullable: true),
                    ReportID = table.Column<long>(type: "bigint", nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazorpayOrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazorpayPaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RazorpaySignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false),
                    WhatsAppSent = table.Column<bool>(type: "bit", nullable: false),
                    SMSSent = table.Column<bool>(type: "bit", nullable: false),
                    LinkSentOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_CustomerId",
                table: "PaymentOrders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "IsAmendmentAllowed",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "IsReportUnlocked",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "PIReceived",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "IsReportUnlocked",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "IsAmendmentAllowed",
                table: "Reports");
        }
    }
}
