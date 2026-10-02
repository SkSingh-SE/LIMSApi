using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablDocChangereqUpdatecol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestById",
                table: "NablDocumentChangeRequests");

            migrationBuilder.RenameColumn(
                name: "RequestByName",
                table: "NablDocumentChangeRequests",
                newName: "ReviewedByName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewedByName",
                table: "NablDocumentChangeRequests",
                newName: "RequestByName");

            migrationBuilder.AddColumn<long>(
                name: "RequestById",
                table: "NablDocumentChangeRequests",
                type: "bigint",
                nullable: true);
        }
    }
}
