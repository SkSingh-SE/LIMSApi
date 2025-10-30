using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTestMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_MetalClassificationMasters_MetalClassificationID",
                table: "MaterialTestMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_ProductConditionMasters_ProductConditionID",
                table: "MaterialTestMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_SpecificationGrades_GradeID",
                table: "MaterialTestMappings");

            migrationBuilder.AlterColumn<long>(
                name: "ProductConditionID",
                table: "MaterialTestMappings",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "MetalClassificationID",
                table: "MaterialTestMappings",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "GradeID",
                table: "MaterialTestMappings",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "MaterialTestMappings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID",
                table: "MaterialTestMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_MetalClassificationMasters_MetalClassificationID",
                table: "MaterialTestMappings",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_ProductConditionMasters_ProductConditionID",
                table: "MaterialTestMappings",
                column: "ProductConditionID",
                principalTable: "ProductConditionMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_SpecificationGrades_GradeID",
                table: "MaterialTestMappings",
                column: "GradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_MetalClassificationMasters_MetalClassificationID",
                table: "MaterialTestMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_ProductConditionMasters_ProductConditionID",
                table: "MaterialTestMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTestMappings_SpecificationGrades_GradeID",
                table: "MaterialTestMappings");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "MaterialTestMappings");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID",
                table: "MaterialTestMappings");

            migrationBuilder.AlterColumn<long>(
                name: "ProductConditionID",
                table: "MaterialTestMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MetalClassificationID",
                table: "MaterialTestMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "GradeID",
                table: "MaterialTestMappings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_MetalClassificationMasters_MetalClassificationID",
                table: "MaterialTestMappings",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_ProductConditionMasters_ProductConditionID",
                table: "MaterialTestMappings",
                column: "ProductConditionID",
                principalTable: "ProductConditionMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTestMappings_SpecificationGrades_GradeID",
                table: "MaterialTestMappings",
                column: "GradeID",
                principalTable: "SpecificationGrades",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
