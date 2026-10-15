using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalDefaultAndMultiTestMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLayoutTemplates_LaboratoryTests_LaboratoryTestID",
                table: "TestLayoutTemplates");

            migrationBuilder.AlterColumn<long>(
                name: "LaboratoryTestID",
                table: "TestLayoutTemplates",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsGlobalDefault",
                table: "TestLayoutTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TestLayoutTemplateMappings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestLayoutTemplateID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLayoutTemplateMappings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestLayoutTemplateMappings_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestLayoutTemplateMappings_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestLayoutTemplateMappings_TestLayoutTemplates_TestLayoutTemplateID",
                        column: x => x.TestLayoutTemplateID,
                        principalTable: "TestLayoutTemplates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestLayoutTemplateMappings_LaboratoryTestID",
                table: "TestLayoutTemplateMappings",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_TestLayoutTemplateMappings_LaboratoryTestSubGroupID",
                table: "TestLayoutTemplateMappings",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TestLayoutTemplateMappings_TestLayoutTemplateID",
                table: "TestLayoutTemplateMappings",
                column: "TestLayoutTemplateID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLayoutTemplates_LaboratoryTests_LaboratoryTestID",
                table: "TestLayoutTemplates",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLayoutTemplates_LaboratoryTests_LaboratoryTestID",
                table: "TestLayoutTemplates");

            migrationBuilder.DropTable(
                name: "TestLayoutTemplateMappings");

            migrationBuilder.DropColumn(
                name: "IsGlobalDefault",
                table: "TestLayoutTemplates");

            migrationBuilder.AlterColumn<long>(
                name: "LaboratoryTestID",
                table: "TestLayoutTemplates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestLayoutTemplates_LaboratoryTests_LaboratoryTestID",
                table: "TestLayoutTemplates",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
