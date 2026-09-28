using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillReportDeliveryAddress",
                table: "ContactPersons");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ContactPersons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "AreaID",
                table: "ContactPersons",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "City",
                table: "ContactPersons",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Country",
                table: "ContactPersons",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PinCode",
                table: "ContactPersons",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "State",
                table: "ContactPersons",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "AreaID",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "City",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ContactPersons");

            migrationBuilder.AddColumn<string>(
                name: "BillReportDeliveryAddress",
                table: "ContactPersons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
