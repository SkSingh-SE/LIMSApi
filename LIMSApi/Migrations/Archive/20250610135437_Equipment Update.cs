using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMasters_EquipmentTypeMasters_EquipmentTypeID",
                table: "EquipmentMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMasters_TestTypeMasters_TestTypeID",
                table: "EquipmentMasters");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentMasters_TestTypeID",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "TestTypeID",
                table: "EquipmentMasters");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "SupplierMasters",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "OEMMasters",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "EquipmentMasters",
                newName: "MaintenanceInterval");

            migrationBuilder.RenameColumn(
                name: "IdentificationNo",
                table: "EquipmentMasters",
                newName: "EquipmentNo");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "EquipmentMasters",
                newName: "InternalExternal");

            migrationBuilder.AddColumn<string>(
                name: "Part",
                table: "TestMethodSpecifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "OEMMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UploadReferenceID",
                table: "OEMMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EquipmentTypeID",
                table: "EquipmentMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CalibrationRequired",
                table: "EquipmentMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentID",
                table: "EquipmentMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IntermediateCheckInterval",
                table: "EquipmentMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IntermediateCheckRequired",
                table: "EquipmentMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MaintenanceRequired",
                table: "EquipmentMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextCalibrationDueDate",
                table: "EquipmentMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextMaintenanceDueDate",
                table: "EquipmentMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OEMID",
                table: "EquipmentMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "EquipmentMasters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "EquipmentCalibration",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    CalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CalibrationDueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Certificate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    CalibrationAgencyID = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentMasterID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCalibration", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentCalibration_EquipmentMasters_EquipmentMasterID",
                        column: x => x.EquipmentMasterID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMaintenance",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    MaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Certificate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentMasterID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMaintenance", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentMaintenance_EquipmentMasters_EquipmentMasterID",
                        column: x => x.EquipmentMasterID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCalibration_EquipmentMasterID",
                table: "EquipmentCalibration",
                column: "EquipmentMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaintenance_EquipmentMasterID",
                table: "EquipmentMaintenance",
                column: "EquipmentMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMasters_EquipmentTypeMasters_EquipmentTypeID",
                table: "EquipmentMasters",
                column: "EquipmentTypeID",
                principalTable: "EquipmentTypeMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMasters_EquipmentTypeMasters_EquipmentTypeID",
                table: "EquipmentMasters");

            migrationBuilder.DropTable(
                name: "EquipmentCalibration");

            migrationBuilder.DropTable(
                name: "EquipmentMaintenance");

            migrationBuilder.DropColumn(
                name: "Part",
                table: "TestMethodSpecifications");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "OEMMasters");

            migrationBuilder.DropColumn(
                name: "UploadReferenceID",
                table: "OEMMasters");

            migrationBuilder.DropColumn(
                name: "CalibrationRequired",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "IntermediateCheckInterval",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "IntermediateCheckRequired",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "MaintenanceRequired",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "NextCalibrationDueDate",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "NextMaintenanceDueDate",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "OEMID",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "EquipmentMasters");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "SupplierMasters",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "OEMMasters",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "MaintenanceInterval",
                table: "EquipmentMasters",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "InternalExternal",
                table: "EquipmentMasters",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "EquipmentNo",
                table: "EquipmentMasters",
                newName: "IdentificationNo");

            migrationBuilder.AlterColumn<long>(
                name: "EquipmentTypeID",
                table: "EquipmentMasters",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "TestTypeID",
                table: "EquipmentMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_TestTypeID",
                table: "EquipmentMasters",
                column: "TestTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMasters_EquipmentTypeMasters_EquipmentTypeID",
                table: "EquipmentMasters",
                column: "EquipmentTypeID",
                principalTable: "EquipmentTypeMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMasters_TestTypeMasters_TestTypeID",
                table: "EquipmentMasters",
                column: "TestTypeID",
                principalTable: "TestTypeMasters",
                principalColumn: "ID");
        }
    }
}
