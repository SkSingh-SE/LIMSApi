using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class CascadeTestPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTests_TestPlans_SamplePlanID",
                table: "ChemicalTests");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTests_SamplePlanID",
                table: "ChemicalTests");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTests_SampleTestPlanID",
                table: "ChemicalTests",
                column: "SampleTestPlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTests_TestPlans_SampleTestPlanID",
                table: "ChemicalTests",
                column: "SampleTestPlanID",
                principalTable: "TestPlans",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTests_TestPlans_SampleTestPlanID",
                table: "ChemicalTests");

            migrationBuilder.DropIndex(
                name: "IX_ChemicalTests_SampleTestPlanID",
                table: "ChemicalTests");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTests_SamplePlanID",
                table: "ChemicalTests",
                column: "SamplePlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTests_TestPlans_SamplePlanID",
                table: "ChemicalTests",
                column: "SamplePlanID",
                principalTable: "TestPlans",
                principalColumn: "ID");
        }
    }
}
