using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabScopeMasters_TestMethodMasters_TestMethodID",
                table: "LabScopeMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_SpecificationHeaders_MateriaSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGroupMappings_TestMethodMasters_TestMethodID",
                table: "TestGroupMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_TestMethodMasters_DepartmentMasters_LabDepartmentID",
                table: "TestMethodMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_TestMethodSubGroups_TestMethodMasters_TestMethodID",
                table: "TestMethodSubGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestMethodMasters",
                table: "TestMethodMasters");

            migrationBuilder.RenameTable(
                name: "TestMethodMasters",
                newName: "LaboratoryTests");

            migrationBuilder.RenameColumn(
                name: "MateriaSpecificationID",
                table: "ProductSpecifications",
                newName: "TestMethodSpecificationID");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSpecifications_MateriaSpecificationID",
                table: "ProductSpecifications",
                newName: "IX_ProductSpecifications_TestMethodSpecificationID");

            migrationBuilder.RenameIndex(
                name: "IX_TestMethodMasters_LabDepartmentID",
                table: "LaboratoryTests",
                newName: "IX_LaboratoryTests_LabDepartmentID");

            migrationBuilder.AlterColumn<string>(
                name: "SpecificationCode",
                table: "ProductSpecifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AliasName",
                table: "ProductSpecifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LaboratoryTestID",
                table: "ProductSpecifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MaterialSpecificationID",
                table: "ProductSpecifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MetalClassificationID",
                table: "ProductSpecifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "ProductSpecifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaboratoryTests",
                table: "LaboratoryTests",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_LaboratoryTestID",
                table: "ProductSpecifications",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_MaterialSpecificationID",
                table: "ProductSpecifications",
                column: "MaterialSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_MetalClassificationID",
                table: "ProductSpecifications",
                column: "MetalClassificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryTests_DepartmentMasters_LabDepartmentID",
                table: "LaboratoryTests",
                column: "LabDepartmentID",
                principalTable: "DepartmentMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LabScopeMasters_LaboratoryTests_TestMethodID",
                table: "LabScopeMasters",
                column: "TestMethodID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_LaboratoryTests_LaboratoryTestID",
                table: "ProductSpecifications",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_MetalClassificationMasters_MetalClassificationID",
                table: "ProductSpecifications",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_SpecificationHeaders_MaterialSpecificationID",
                table: "ProductSpecifications",
                column: "MaterialSpecificationID",
                principalTable: "SpecificationHeaders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_TestMethodSpecifications_TestMethodSpecificationID",
                table: "ProductSpecifications",
                column: "TestMethodSpecificationID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestGroupMappings_LaboratoryTests_TestMethodID",
                table: "TestGroupMappings",
                column: "TestMethodID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestMethodSubGroups_LaboratoryTests_TestMethodID",
                table: "TestMethodSubGroups",
                column: "TestMethodID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryTests_DepartmentMasters_LabDepartmentID",
                table: "LaboratoryTests");

            migrationBuilder.DropForeignKey(
                name: "FK_LabScopeMasters_LaboratoryTests_TestMethodID",
                table: "LabScopeMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_LaboratoryTests_LaboratoryTestID",
                table: "ProductSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_MetalClassificationMasters_MetalClassificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_SpecificationHeaders_MaterialSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecifications_TestMethodSpecifications_TestMethodSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGroupMappings_LaboratoryTests_TestMethodID",
                table: "TestGroupMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_TestMethodSubGroups_LaboratoryTests_TestMethodID",
                table: "TestMethodSubGroups");

            migrationBuilder.DropIndex(
                name: "IX_ProductSpecifications_LaboratoryTestID",
                table: "ProductSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_ProductSpecifications_MaterialSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_ProductSpecifications_MetalClassificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LaboratoryTests",
                table: "LaboratoryTests");

            migrationBuilder.DropColumn(
                name: "LaboratoryTestID",
                table: "ProductSpecifications");

            migrationBuilder.DropColumn(
                name: "MaterialSpecificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropColumn(
                name: "MetalClassificationID",
                table: "ProductSpecifications");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "ProductSpecifications");

            migrationBuilder.RenameTable(
                name: "LaboratoryTests",
                newName: "TestMethodMasters");

            migrationBuilder.RenameColumn(
                name: "TestMethodSpecificationID",
                table: "ProductSpecifications",
                newName: "MateriaSpecificationID");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSpecifications_TestMethodSpecificationID",
                table: "ProductSpecifications",
                newName: "IX_ProductSpecifications_MateriaSpecificationID");

            migrationBuilder.RenameIndex(
                name: "IX_LaboratoryTests_LabDepartmentID",
                table: "TestMethodMasters",
                newName: "IX_TestMethodMasters_LabDepartmentID");

            migrationBuilder.AlterColumn<string>(
                name: "SpecificationCode",
                table: "ProductSpecifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "AliasName",
                table: "ProductSpecifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestMethodMasters",
                table: "TestMethodMasters",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LabScopeMasters_TestMethodMasters_TestMethodID",
                table: "LabScopeMasters",
                column: "TestMethodID",
                principalTable: "TestMethodMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecifications_SpecificationHeaders_MateriaSpecificationID",
                table: "ProductSpecifications",
                column: "MateriaSpecificationID",
                principalTable: "SpecificationHeaders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestGroupMappings_TestMethodMasters_TestMethodID",
                table: "TestGroupMappings",
                column: "TestMethodID",
                principalTable: "TestMethodMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestMethodMasters_DepartmentMasters_LabDepartmentID",
                table: "TestMethodMasters",
                column: "LabDepartmentID",
                principalTable: "DepartmentMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestMethodSubGroups_TestMethodMasters_TestMethodID",
                table: "TestMethodSubGroups",
                column: "TestMethodID",
                principalTable: "TestMethodMasters",
                principalColumn: "ID");
        }
    }
}
