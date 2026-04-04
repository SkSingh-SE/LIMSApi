using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLabRoomAndEnvironmentDailyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LabRoomId",
                table: "NablEnvironmentMonitorings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonitoringMonth",
                table: "NablEnvironmentMonitorings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonitoringYear",
                table: "NablEnvironmentMonitorings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "NablEnvironmentMonitorings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LabRoomID",
                table: "EquipmentMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EnvironmentDailyRecords",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnvironmentMonitoringID = table.Column<long>(type: "bigint", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Pressure = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AirQuality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObservedTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ObservedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsWithinLimits = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentDailyRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EnvironmentDailyRecords_NablEnvironmentMonitorings_EnvironmentMonitoringID",
                        column: x => x.EnvironmentMonitoringID,
                        principalTable: "NablEnvironmentMonitorings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabRooms",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DepartmentID = table.Column<long>(type: "bigint", nullable: true),
                    Building = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DefaultTempMin = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    DefaultTempMax = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    DefaultHumidityMin = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    DefaultHumidityMax = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    MonitoringEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabRooms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabRooms_DepartmentMasters_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NablEnvironmentMonitorings_LabRoomId",
                table: "NablEnvironmentMonitorings",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_LabRoomID",
                table: "EquipmentMasters",
                column: "LabRoomID");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentDailyRecords_EnvironmentMonitoringID",
                table: "EnvironmentDailyRecords",
                column: "EnvironmentMonitoringID");

            migrationBuilder.CreateIndex(
                name: "IX_LabRooms_DepartmentID",
                table: "LabRooms",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMasters_LabRooms_LabRoomID",
                table: "EquipmentMasters",
                column: "LabRoomID",
                principalTable: "LabRooms",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_NablEnvironmentMonitorings_LabRooms_LabRoomId",
                table: "NablEnvironmentMonitorings",
                column: "LabRoomId",
                principalTable: "LabRooms",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMasters_LabRooms_LabRoomID",
                table: "EquipmentMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_NablEnvironmentMonitorings_LabRooms_LabRoomId",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropTable(
                name: "EnvironmentDailyRecords");

            migrationBuilder.DropTable(
                name: "LabRooms");

            migrationBuilder.DropIndex(
                name: "IX_NablEnvironmentMonitorings_LabRoomId",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentMasters_LabRoomID",
                table: "EquipmentMasters");

            migrationBuilder.DropColumn(
                name: "LabRoomId",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropColumn(
                name: "MonitoringMonth",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropColumn(
                name: "MonitoringYear",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "NablEnvironmentMonitorings");

            migrationBuilder.DropColumn(
                name: "LabRoomID",
                table: "EquipmentMasters");
        }
    }
}
