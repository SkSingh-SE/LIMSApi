using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMetalIdfromGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LongTermTests_SampleDetails_SampleID",
                table: "LongTermTests");

            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationGrades_MetalClassificationMasters_MetalCalssificationID",
                table: "SpecificationGrades");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationGrades_MetalCalssificationID",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "MetalCalssificationID",
                table: "SpecificationGrades");

            migrationBuilder.DropColumn(
                name: "MetalClassificationID",
                table: "SpecificationGrades");

            migrationBuilder.AddForeignKey(
                name: "FK_LongTermTests_SampleDetails_SampleID",
                table: "LongTermTests",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LongTermTests_SampleDetails_SampleID",
                table: "LongTermTests");

            migrationBuilder.AddColumn<long>(
                name: "MetalCalssificationID",
                table: "SpecificationGrades",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MetalClassificationID",
                table: "SpecificationGrades",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationGrades_MetalCalssificationID",
                table: "SpecificationGrades",
                column: "MetalCalssificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LongTermTests_SampleDetails_SampleID",
                table: "LongTermTests",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationGrades_MetalClassificationMasters_MetalCalssificationID",
                table: "SpecificationGrades",
                column: "MetalCalssificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");
        }
    }
}
