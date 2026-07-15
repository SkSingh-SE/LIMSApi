using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ParameterRestructure_InputType_Dropdown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_ParameterCategoryMasters_ParameterCategoryID",
                table: "ParameterMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_TestMethodSpecifications_DefaultTestMethodID",
                table: "ParameterMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterSpecimenOrientations_ParameterMasters_ParameterID",
                table: "ParameterSpecimenOrientations");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterSpecimenOrientations_SpecimenOrientationMasters_SpecimenOrientationID",
                table: "ParameterSpecimenOrientations");

            migrationBuilder.DropTable(
                name: "ParameterCategoryMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMaster_Code",
                table: "ParameterMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMasters_DefaultTestMethodID",
                table: "ParameterMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMasters_ParameterCategoryID",
                table: "ParameterMasters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParameterSpecimenOrientations",
                table: "ParameterSpecimenOrientations");

            migrationBuilder.DropColumn(
                name: "DefaultTestMethodID",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "MinReportableLimit",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "ParameterCategoryID",
                table: "ParameterMasters");

            migrationBuilder.RenameTable(
                name: "ParameterSpecimenOrientations",
                newName: "ParameterSpecimenOrientation");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "ParameterMasters",
                newName: "InputType");

            migrationBuilder.RenameColumn(
                name: "AliasName",
                table: "ParameterMasters",
                newName: "FormulaDisplay");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterSpecimenOrientations_SpecimenOrientationID",
                table: "ParameterSpecimenOrientation",
                newName: "IX_ParameterSpecimenOrientation_SpecimenOrientationID");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "ParameterMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ParameterMasters",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParameterSpecimenOrientation",
                table: "ParameterSpecimenOrientation",
                columns: new[] { "ParameterID", "SpecimenOrientationID" });

            migrationBuilder.CreateTable(
                name: "ParameterDropdownOptions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterDropdownOptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParameterDropdownOptions_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterDropdownOptions_ParameterID",
                table: "ParameterDropdownOptions",
                column: "ParameterID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterSpecimenOrientation_ParameterMasters_ParameterID",
                table: "ParameterSpecimenOrientation",
                column: "ParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterSpecimenOrientation_SpecimenOrientationMasters_SpecimenOrientationID",
                table: "ParameterSpecimenOrientation",
                column: "SpecimenOrientationID",
                principalTable: "SpecimenOrientationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterSpecimenOrientation_ParameterMasters_ParameterID",
                table: "ParameterSpecimenOrientation");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterSpecimenOrientation_SpecimenOrientationMasters_SpecimenOrientationID",
                table: "ParameterSpecimenOrientation");

            migrationBuilder.DropTable(
                name: "ParameterDropdownOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParameterSpecimenOrientation",
                table: "ParameterSpecimenOrientation");

            migrationBuilder.RenameTable(
                name: "ParameterSpecimenOrientation",
                newName: "ParameterSpecimenOrientations");

            migrationBuilder.RenameColumn(
                name: "InputType",
                table: "ParameterMasters",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "FormulaDisplay",
                table: "ParameterMasters",
                newName: "AliasName");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterSpecimenOrientation_SpecimenOrientationID",
                table: "ParameterSpecimenOrientations",
                newName: "IX_ParameterSpecimenOrientations_SpecimenOrientationID");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "ParameterMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ParameterMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<long>(
                name: "DefaultTestMethodID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinReportableLimit",
                table: "ParameterMasters",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParameterCategoryID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParameterSpecimenOrientations",
                table: "ParameterSpecimenOrientations",
                columns: new[] { "ParameterID", "SpecimenOrientationID" });

            migrationBuilder.CreateTable(
                name: "ParameterCategoryMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterCategoryMasters", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMaster_Code",
                table: "ParameterMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_DefaultTestMethodID",
                table: "ParameterMasters",
                column: "DefaultTestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_ParameterCategoryID",
                table: "ParameterMasters",
                column: "ParameterCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_ParameterCategoryMasters_ParameterCategoryID",
                table: "ParameterMasters",
                column: "ParameterCategoryID",
                principalTable: "ParameterCategoryMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_TestMethodSpecifications_DefaultTestMethodID",
                table: "ParameterMasters",
                column: "DefaultTestMethodID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterSpecimenOrientations_ParameterMasters_ParameterID",
                table: "ParameterSpecimenOrientations",
                column: "ParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterSpecimenOrientations_SpecimenOrientationMasters_SpecimenOrientationID",
                table: "ParameterSpecimenOrientations",
                column: "SpecimenOrientationID",
                principalTable: "SpecimenOrientationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
