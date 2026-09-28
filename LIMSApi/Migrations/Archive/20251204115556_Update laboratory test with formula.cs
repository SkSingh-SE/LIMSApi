using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Updatelaboratorytestwithformula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Formula",
                table: "ParameterMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCalculated",
                table: "ParameterMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LaboratoryTestParameter",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestParameter", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestParameter_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestParameter_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestParameter_LaboratoryTestID",
                table: "LaboratoryTestParameter",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestParameter_ParameterID",
                table: "LaboratoryTestParameter",
                column: "ParameterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaboratoryTestParameter");

            migrationBuilder.DropColumn(
                name: "Formula",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "IsCalculated",
                table: "ParameterMasters");
        }
    }
}
