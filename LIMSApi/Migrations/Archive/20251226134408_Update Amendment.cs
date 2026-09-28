using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAmendment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportingDocumentsJson",
                table: "AmendmentRequests");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "AmendmentRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "AmendmentRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "UploadReferenceID",
                table: "AmendmentRequests",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "AmendmentRequests");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "AmendmentRequests");

            migrationBuilder.DropColumn(
                name: "UploadReferenceID",
                table: "AmendmentRequests");

            migrationBuilder.AddColumn<string>(
                name: "SupportingDocumentsJson",
                table: "AmendmentRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
