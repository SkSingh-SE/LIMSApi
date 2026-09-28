using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class CalibrationAgency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Note",
                table: "CalibrationAgencyMasters",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "CalibrationAgencyMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupplierApproved",
                table: "CalibrationAgencyMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UploadReferenceID",
                table: "CalibrationAgencyMasters",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "CalibrationAgencyMasters");

            migrationBuilder.DropColumn(
                name: "SupplierApproved",
                table: "CalibrationAgencyMasters");

            migrationBuilder.DropColumn(
                name: "UploadReferenceID",
                table: "CalibrationAgencyMasters");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "CalibrationAgencyMasters",
                newName: "Note");
        }
    }
}
