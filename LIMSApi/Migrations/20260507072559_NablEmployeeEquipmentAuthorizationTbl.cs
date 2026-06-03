using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablEmployeeEquipmentAuthorizationTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NablEmployeeEquipmentAuthrizations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeAuthorazitionId = table.Column<long>(type: "bigint", nullable: false),
                    UID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEmployeeEquipmentAuthrizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NablEmployeeEquipmentAuthrizations_NablEmployeeAuthorizations_EmployeeAuthorazitionId",
                        column: x => x.EmployeeAuthorazitionId,
                        principalTable: "NablEmployeeAuthorizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablEmployeeLaborartyTestAuthorizations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeAuthorizationId = table.Column<long>(type: "bigint", nullable: false),
                    LabTestId = table.Column<long>(type: "bigint", nullable: false),
                    LabTestName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEmployeeLaborartyTestAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NablEmployeeLaborartyTestAuthorizations_NablEmployeeAuthorizations_EmployeeAuthorizationId",
                        column: x => x.EmployeeAuthorizationId,
                        principalTable: "NablEmployeeAuthorizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablEmployeeTestMethodAuthorizations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeAuthorizationId = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodId = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEmployeeTestMethodAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NablEmployeeTestMethodAuthorizations_NablEmployeeAuthorizations_EmployeeAuthorizationId",
                        column: x => x.EmployeeAuthorizationId,
                        principalTable: "NablEmployeeAuthorizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeEquipmentAuthrizations_EmployeeAuthorazitionId",
                table: "NablEmployeeEquipmentAuthrizations",
                column: "EmployeeAuthorazitionId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeLaborartyTestAuthorizations_EmployeeAuthorizationId",
                table: "NablEmployeeLaborartyTestAuthorizations",
                column: "EmployeeAuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeTestMethodAuthorizations_EmployeeAuthorizationId",
                table: "NablEmployeeTestMethodAuthorizations",
                column: "EmployeeAuthorizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NablEmployeeEquipmentAuthrizations");

            migrationBuilder.DropTable(
                name: "NablEmployeeLaborartyTestAuthorizations");

            migrationBuilder.DropTable(
                name: "NablEmployeeTestMethodAuthorizations");
        }
    }
}
