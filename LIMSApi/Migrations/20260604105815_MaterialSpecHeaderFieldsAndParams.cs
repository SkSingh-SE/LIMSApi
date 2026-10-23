using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class MaterialSpecHeaderFieldsAndParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayTitle",
                table: "SpecificationHeaders",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecificationNo",
                table: "SpecificationHeaders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SpecificationHeaders",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "SpecificationHeaders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpecificationHeaderParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationHeaderParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationHeaderParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationHeaderParameters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationHeaderParameters_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaderParameters_ParameterID",
                table: "SpecificationHeaderParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaderParameters_ParameterUnitID",
                table: "SpecificationHeaderParameters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaderParameters_SpecificationHeaderID",
                table: "SpecificationHeaderParameters",
                column: "SpecificationHeaderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecificationHeaderParameters");

            migrationBuilder.DropColumn(
                name: "DisplayTitle",
                table: "SpecificationHeaders");

            migrationBuilder.DropColumn(
                name: "SpecificationNo",
                table: "SpecificationHeaders");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "SpecificationHeaders");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SpecificationHeaders");
        }
    }
}
