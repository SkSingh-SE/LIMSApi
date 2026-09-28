using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UniqueCaseNoandSampleNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleDispatchModes");

            migrationBuilder.AlterColumn<string>(
                name: "SampleNo",
                table: "SampleDetails",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "InwardDispatchModes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    DispatchModeID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InwardDispatchModes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InwardDispatchModes_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_CaseNo",
                table: "SampleInwards",
                column: "CaseNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_SampleNo",
                table: "SampleDetails",
                column: "SampleNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InwardDispatchModes_InwardID",
                table: "InwardDispatchModes",
                column: "InwardID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InwardDispatchModes");

            migrationBuilder.DropIndex(
                name: "IX_SampleInwards_CaseNo",
                table: "SampleInwards");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_SampleNo",
                table: "SampleDetails");

            migrationBuilder.AlterColumn<string>(
                name: "SampleNo",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "SampleDispatchModes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    DispatchModeID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleDispatchModes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SampleDispatchModes_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleDispatchModes_InwardID",
                table: "SampleDispatchModes",
                column: "InwardID");
        }
    }
}
