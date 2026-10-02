using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablCustomerFeedbackAddColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyAddress",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "NablCustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyAddress",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "NablCustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "NablCustomerFeedbacks");
        }
    }
}
