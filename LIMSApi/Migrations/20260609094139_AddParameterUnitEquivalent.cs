using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddParameterUnitEquivalent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParameterUnitEquivalents",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseParameterUnitID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterUnitEquivalents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParameterUnitEquivalents_ParameterUnitMasters_BaseParameterUnitID",
                        column: x => x.BaseParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterUnitEquivalents_BaseParameterUnitID",
                table: "ParameterUnitEquivalents",
                column: "BaseParameterUnitID");

            // Data migration: copy each non-empty inline SimilarUnit1-7 into a child row.
            for (int i = 1; i <= 7; i++)
            {
                migrationBuilder.Sql($@"
INSERT INTO ParameterUnitEquivalents (BaseParameterUnitID, Name, ConversionFactor, DisplayOrder, IsActive)
SELECT ID, LTRIM(RTRIM(SimilarUnit{i})), ConversionFactor{i}, {i}, 1
FROM ParameterUnitMasters
WHERE SimilarUnit{i} IS NOT NULL AND LTRIM(RTRIM(SimilarUnit{i})) <> '';");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParameterUnitEquivalents");
        }
    }
}
