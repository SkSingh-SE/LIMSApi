using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablDocumentChangeReqAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChangeType",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentIssue",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentRevision",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentDoc",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "NablDocumentChangeRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionOfChange",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DocumentId",
                table: "NablDocumentChangeRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentOwner",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImpactOfChange",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RequestById",
                table: "NablDocumentChangeRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestByName",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestNo",
                table: "NablDocumentChangeRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeType",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "CurrentIssue",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "CurrentRevision",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "DepartmentDoc",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "DescriptionOfChange",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "DocumentOwner",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "ImpactOfChange",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "RequestById",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "RequestByName",
                table: "NablDocumentChangeRequests");

            migrationBuilder.DropColumn(
                name: "RequestNo",
                table: "NablDocumentChangeRequests");
        }
    }
}
