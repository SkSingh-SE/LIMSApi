using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablDocumentReviewAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalRemarks",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentIssue",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentDoc",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "NablDocumentReviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "NablDocumentReviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentOwner",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneratedDcrChangeType",
                table: "NablDocumentReviews",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GeneratedDcrId",
                table: "NablDocumentReviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneratedDcrNo",
                table: "NablDocumentReviews",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImpactOfChange",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoChangeConclusion",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForChange",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewType",
                table: "NablDocumentReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SourceReviewId",
                table: "NablDocumentChangeRequests",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalRemarks",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "CurrentIssue",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "DepartmentDoc",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "DocumentOwner",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "GeneratedDcrChangeType",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "GeneratedDcrId",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "GeneratedDcrNo",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "ImpactOfChange",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "NoChangeConclusion",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "ReasonForChange",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "ReviewType",
                table: "NablDocumentReviews");

            migrationBuilder.DropColumn(
                name: "SourceReviewId",
                table: "NablDocumentChangeRequests");
        }
    }
}
