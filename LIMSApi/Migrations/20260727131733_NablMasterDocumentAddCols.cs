using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablMasterDocumentAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "NablMasterDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "NablMasterDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DocumentOwnerId",
                table: "NablMasterDocuments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "NablMasterDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "NablMasterDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UploadReferenceID",
                table: "NablMasterDocuments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedOn",
                table: "NablMasterDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablMasterDocuments_UploadReferenceID",
                table: "NablMasterDocuments",
                column: "UploadReferenceID");

            migrationBuilder.AddForeignKey(
                name: "FK_NablMasterDocuments_UploadFiles_UploadReferenceID",
                table: "NablMasterDocuments",
                column: "UploadReferenceID",
                principalTable: "UploadFiles",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablMasterDocuments_UploadFiles_UploadReferenceID",
                table: "NablMasterDocuments");

            migrationBuilder.DropIndex(
                name: "IX_NablMasterDocuments_UploadReferenceID",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "DocumentOwnerId",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "UploadReferenceID",
                table: "NablMasterDocuments");

            migrationBuilder.DropColumn(
                name: "UploadedOn",
                table: "NablMasterDocuments");
        }
    }
}
