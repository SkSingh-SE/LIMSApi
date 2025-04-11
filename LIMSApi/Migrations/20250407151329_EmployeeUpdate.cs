using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDocuments_EmployeeMasters_EmployeeID",
                table: "EmployeeDocuments");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDocuments_EmployeeID",
                table: "EmployeeDocuments");

            migrationBuilder.DropColumn(
                name: "IsMarried",
                table: "EmployeeMasters");

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "EmployeeMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReportingManagerID",
                table: "EmployeeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeMasterID",
                table: "EmployeeDocuments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeMasterID",
                table: "EmployeeDocuments",
                column: "EmployeeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_UploadReferenceID",
                table: "EmployeeDocuments",
                column: "UploadReferenceID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDocuments_EmployeeMasters_EmployeeMasterID",
                table: "EmployeeDocuments",
                column: "EmployeeMasterID",
                principalTable: "EmployeeMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDocuments_UploadFiles_UploadReferenceID",
                table: "EmployeeDocuments",
                column: "UploadReferenceID",
                principalTable: "UploadFiles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDocuments_EmployeeMasters_EmployeeMasterID",
                table: "EmployeeDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDocuments_UploadFiles_UploadReferenceID",
                table: "EmployeeDocuments");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDocuments_EmployeeMasterID",
                table: "EmployeeDocuments");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDocuments_UploadReferenceID",
                table: "EmployeeDocuments");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "EmployeeMasters");

            migrationBuilder.DropColumn(
                name: "ReportingManagerID",
                table: "EmployeeMasters");

            migrationBuilder.DropColumn(
                name: "EmployeeMasterID",
                table: "EmployeeDocuments");

            migrationBuilder.AddColumn<bool>(
                name: "IsMarried",
                table: "EmployeeMasters",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeID",
                table: "EmployeeDocuments",
                column: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDocuments_EmployeeMasters_EmployeeID",
                table: "EmployeeDocuments",
                column: "EmployeeID",
                principalTable: "EmployeeMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
