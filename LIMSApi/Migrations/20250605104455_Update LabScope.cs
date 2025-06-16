using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLabScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabScopeMasters_LaboratoryTests_TestMethodID",
                table: "LabScopeMasters");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LabScopeMasters");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "LabScopeMasters");

            migrationBuilder.RenameColumn(
                name: "TestMethodID",
                table: "LabScopeMasters",
                newName: "LaboratoryTestID");

            migrationBuilder.RenameIndex(
                name: "IX_LabScopeMasters_TestMethodID",
                table: "LabScopeMasters",
                newName: "IX_LabScopeMasters_LaboratoryTestID");

            migrationBuilder.CreateTable(
                name: "LabScopeSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabScopeID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabScopeSpecifications_LabScopeMasters_LabScopeID",
                        column: x => x.LabScopeID,
                        principalTable: "LabScopeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabScopeSpecificationParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabScopeSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: false),
                    QualitativeQuantitative = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUnderISO = table.Column<bool>(type: "bit", nullable: false),
                    LowerLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpperLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisciplineID = table.Column<long>(type: "bigint", nullable: true),
                    GroupID = table.Column<long>(type: "bigint", nullable: true),
                    SubGroupID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeSpecificationParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabScopeSpecificationParameters_LabScopeSpecifications_LabScopeSpecificationID",
                        column: x => x.LabScopeSpecificationID,
                        principalTable: "LabScopeSpecifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabScopeSpecificationParameterEquipments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabScopeSpecificationParameterID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeSpecificationParameterEquipments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabScopeSpecificationParameterEquipments_LabScopeSpecificationParameters_LabScopeSpecificationParameterID",
                        column: x => x.LabScopeSpecificationParameterID,
                        principalTable: "LabScopeSpecificationParameters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecificationParameterEquipments_LabScopeSpecificationParameterID",
                table: "LabScopeSpecificationParameterEquipments",
                column: "LabScopeSpecificationParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecificationParameters_LabScopeSpecificationID",
                table: "LabScopeSpecificationParameters",
                column: "LabScopeSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecifications_LabScopeID",
                table: "LabScopeSpecifications",
                column: "LabScopeID");

            migrationBuilder.AddForeignKey(
                name: "FK_LabScopeMasters_LaboratoryTests_LaboratoryTestID",
                table: "LabScopeMasters",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabScopeMasters_LaboratoryTests_LaboratoryTestID",
                table: "LabScopeMasters");

            migrationBuilder.DropTable(
                name: "LabScopeSpecificationParameterEquipments");

            migrationBuilder.DropTable(
                name: "LabScopeSpecificationParameters");

            migrationBuilder.DropTable(
                name: "LabScopeSpecifications");

            migrationBuilder.RenameColumn(
                name: "LaboratoryTestID",
                table: "LabScopeMasters",
                newName: "TestMethodID");

            migrationBuilder.RenameIndex(
                name: "IX_LabScopeMasters_LaboratoryTestID",
                table: "LabScopeMasters",
                newName: "IX_LabScopeMasters_TestMethodID");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LabScopeMasters",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "LabScopeMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_LabScopeMasters_LaboratoryTests_TestMethodID",
                table: "LabScopeMasters",
                column: "TestMethodID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
