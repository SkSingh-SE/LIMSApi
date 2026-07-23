using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablSupplierRegisterAddCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankDetails",
                table: "NablSupplierRegistrations");

            migrationBuilder.AddColumn<bool>(
                name: "EvaluationRequired",
                table: "NablSupplierRegistrations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisterNo",
                table: "NablSupplierRegistrations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvaluationRequired",
                table: "NablSupplierRegistrations");

            migrationBuilder.DropColumn(
                name: "RegisterNo",
                table: "NablSupplierRegistrations");

            migrationBuilder.AddColumn<string>(
                name: "BankDetails",
                table: "NablSupplierRegistrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
