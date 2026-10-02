using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNablTestRequestTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "NablTestRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BillRequired",
                table: "NablTestRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmityRequired",
                table: "NablTestRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispatchModeJson",
                table: "NablTestRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GstNo",
                table: "NablTestRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HoldTesting",
                table: "NablTestRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoNumber",
                table: "NablTestRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "NablTestRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReturnSample",
                table: "NablTestRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Urgent",
                table: "NablTestRequests",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "BillRequired",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "ConfirmityRequired",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "DispatchModeJson",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "GstNo",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "HoldTesting",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "PoNumber",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "ReturnSample",
                table: "NablTestRequests");

            migrationBuilder.DropColumn(
                name: "Urgent",
                table: "NablTestRequests");
        }
    }
}
