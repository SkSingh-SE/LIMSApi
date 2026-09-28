using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class PhaseB_GroupA_Foundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The MachiningChargeMasters foreign keys and indexes were already dropped in a previous manual migration (20260827014600_DropMachiningChargeMasterForeignKeyConstraints).

            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameter_MetalClassificationMasters_MetalClassificationID",
                table: "MetalClassificationParameter");

            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameter_ParameterMasters_ParameterID",
                table: "MetalClassificationParameter");
            
            migrationBuilder.DropPrimaryKey(
                name: "PK_MetalClassificationParameter",
                table: "MetalClassificationParameter");

            migrationBuilder.RenameTable(
                name: "MetalClassificationParameter",
                newName: "MetalClassificationParameters");

            migrationBuilder.RenameIndex(
                name: "IX_MetalClassificationParameter_ParameterID",
                table: "MetalClassificationParameters",
                newName: "IX_MetalClassificationParameters_ParameterID");

            migrationBuilder.AddColumn<long>(
                name: "OrganizationID",
                table: "UserMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("UPDATE UserMasters SET OrganizationID = 7 WHERE OrganizationID IS NULL;");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ParameterMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DisciplineID",
                table: "LaboratoryTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetalClassificationParameters",
                table: "MetalClassificationParameters",
                columns: new[] { "MetalClassificationID", "ParameterID" });

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameters_MetalClassificationMasters_MetalClassificationID",
                table: "MetalClassificationParameters",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameters_ParameterMasters_ParameterID",
                table: "MetalClassificationParameters",
                column: "ParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameters_MetalClassificationMasters_MetalClassificationID",
                table: "MetalClassificationParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_MetalClassificationParameters_ParameterMasters_ParameterID",
                table: "MetalClassificationParameters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetalClassificationParameters",
                table: "MetalClassificationParameters");

            migrationBuilder.DropColumn(
                name: "OrganizationID",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "DisciplineID",
                table: "LaboratoryTests");

            migrationBuilder.RenameTable(
                name: "MetalClassificationParameters",
                newName: "MetalClassificationParameter");

            migrationBuilder.RenameIndex(
                name: "IX_MetalClassificationParameters_ParameterID",
                table: "MetalClassificationParameter",
                newName: "IX_MetalClassificationParameter_ParameterID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetalClassificationParameter",
                table: "MetalClassificationParameter",
                columns: new[] { "MetalClassificationID", "ParameterID" });

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeMasters_LaboratoryTestID",
                table: "MachiningChargeMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeMasters_TestMethodStandardID",
                table: "MachiningChargeMasters",
                column: "TestMethodStandardID");

            migrationBuilder.AddForeignKey(
                name: "FK_MachiningChargeMasters_LaboratoryTests_LaboratoryTestID",
                table: "MachiningChargeMasters",
                column: "LaboratoryTestID",
                principalTable: "LaboratoryTests",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MachiningChargeMasters_TestMethodSpecifications_TestMethodStandardID",
                table: "MachiningChargeMasters",
                column: "TestMethodStandardID",
                principalTable: "TestMethodSpecifications",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameter_MetalClassificationMasters_MetalClassificationID",
                table: "MetalClassificationParameter",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MetalClassificationParameter_ParameterMasters_ParameterID",
                table: "MetalClassificationParameter",
                column: "ParameterID",
                principalTable: "ParameterMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
