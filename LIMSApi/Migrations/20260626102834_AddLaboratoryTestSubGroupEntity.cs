using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLaboratoryTestSubGroupEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTestSubTypes_LaboratoryTests_LaboratoryTestID",
                table: "LaboratoryTestSubTypes");

            migrationBuilder.RenameColumn(
                name: "LaboratoryTestID",
                table: "LaboratoryTestSubTypes",
                newName: "LaboratoryTestSubGroupID");

            migrationBuilder.RenameIndex(
                name: "IX_LaboratoryTestSubTypes_LaboratoryTestID",
                table: "LaboratoryTestSubTypes",
                newName: "IX_LaboratoryTestSubTypes_LaboratoryTestSubGroupID");

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroups_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroups_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroups_LaboratoryTestID",
                table: "LaboratoryTestSubGroups",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroups_MetalClassificationID",
                table: "LaboratoryTestSubGroups",
                column: "MetalClassificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestSubTypes_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubTypes",
                column: "LaboratoryTestSubGroupID",
                principalTable: "LaboratoryTestSubGroups",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTestSubTypes_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubTypes");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroups");

            migrationBuilder.RenameColumn(
                name: "LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubTypes",
                newName: "LaboratoryTestID");

            migrationBuilder.RenameIndex(
                name: "IX_LaboratoryTestSubTypes_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubTypes",
                newName: "IX_LaboratoryTestSubTypes_LaboratoryTestID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTestSubTypes_LaboratoryTests_LaboratoryTestID",
                table: "LaboratoryTestSubTypes",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID");
        }
    }
}
