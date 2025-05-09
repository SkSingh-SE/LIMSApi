using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class MaterialSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerDispatchModes_CustomerDispatchModes_CustomerDispatchModeID",
                table: "CustomerDispatchModes");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationLines_UOMMasters_UOMID",
                table: "SpecificationLines");

            migrationBuilder.DropIndex(
                name: "IX_CustomerDispatchModes_CustomerDispatchModeID",
                table: "CustomerDispatchModes");

            migrationBuilder.DropColumn(
                name: "LowerLimit",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "UpperLimit",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "CustomerDispatchModeID",
                table: "CustomerDispatchModes");

            migrationBuilder.RenameColumn(
                name: "UOMID",
                table: "SpecificationLines",
                newName: "ParameterUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_SpecificationLines_UOMID",
                table: "SpecificationLines",
                newName: "IX_SpecificationLines_ParameterUnitID");

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID1",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID2",
                table: "SpecificationLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxTolerance",
                table: "SpecificationLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValueEquation",
                table: "SpecificationLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinTolerance",
                table: "SpecificationLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValueEquation",
                table: "SpecificationLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MetalCalssificationID",
                table: "SpecificationHeaders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MetalClassificationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetalClassificationMasters", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaders_MetalCalssificationID",
                table: "SpecificationHeaders",
                column: "MetalCalssificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationHeaders_MetalClassificationMasters_MetalCalssificationID",
                table: "SpecificationHeaders",
                column: "MetalCalssificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationLines_ParameterUnitMasters_ParameterUnitID",
                table: "SpecificationLines",
                column: "ParameterUnitID",
                principalTable: "ParameterUnitMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationHeaders_MetalClassificationMasters_MetalCalssificationID",
                table: "SpecificationHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationLines_ParameterUnitMasters_ParameterUnitID",
                table: "SpecificationLines");

            migrationBuilder.DropTable(
                name: "MetalClassificationMasters");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationHeaders_MetalCalssificationID",
                table: "SpecificationHeaders");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID1",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID2",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "MaxTolerance",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "MaxValueEquation",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "MinTolerance",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "MinValueEquation",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "MetalCalssificationID",
                table: "SpecificationHeaders");

            migrationBuilder.RenameColumn(
                name: "ParameterUnitID",
                table: "SpecificationLines",
                newName: "UOMID");

            migrationBuilder.RenameIndex(
                name: "IX_SpecificationLines_ParameterUnitID",
                table: "SpecificationLines",
                newName: "IX_SpecificationLines_UOMID");

            migrationBuilder.AddColumn<string>(
                name: "LowerLimit",
                table: "SpecificationLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpperLimit",
                table: "SpecificationLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CustomerDispatchModeID",
                table: "CustomerDispatchModes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDispatchModes_CustomerDispatchModeID",
                table: "CustomerDispatchModes",
                column: "CustomerDispatchModeID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerDispatchModes_CustomerDispatchModes_CustomerDispatchModeID",
                table: "CustomerDispatchModes",
                column: "CustomerDispatchModeID",
                principalTable: "CustomerDispatchModes",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationLines_UOMMasters_UOMID",
                table: "SpecificationLines",
                column: "UOMID",
                principalTable: "UOMMasters",
                principalColumn: "ID");
        }
    }
}
