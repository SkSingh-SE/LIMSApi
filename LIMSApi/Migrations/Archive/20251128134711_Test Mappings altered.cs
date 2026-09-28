using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class TestMappingsaltered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_LaboratoryTests_LaboratoryTestID",
                table: "MaterialTestMappings");

            migrationBuilder.DropIndex(
                name: "IX_MaterialTestMappings_LaboratoryTestID",
                table: "MaterialTestMappings");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID",
                table: "MaterialTestMappings");

            migrationBuilder.CreateTable(
                name: "MappingLaboratoryTests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMappingID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    MaterialTestMappingID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingLaboratoryTests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MappingLaboratoryTests_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MappingLaboratoryTests_MaterialTestMappings_MaterialTestMappingID",
                        column: x => x.MaterialTestMappingID,
                        principalTable: "MaterialTestMappings",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MappingLaboratoryTests_LaboratoryTestID",
                table: "MappingLaboratoryTests",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_MappingLaboratoryTests_MaterialTestMappingID",
                table: "MappingLaboratoryTests",
                column: "MaterialTestMappingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MappingLaboratoryTests");

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID",
                table: "MaterialTestMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTestMappings_LaboratoryTestID",
                table: "MaterialTestMappings",
                column: "LaboratoryTestID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_LaboratoryTests_LaboratoryTestID",
                table: "MaterialTestMappings",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
