using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldinPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTest_TestPlans_SamplePlanID",
                table: "ChemicalTest");

            migrationBuilder.AlterColumn<long>(
                name: "SamplePlanID",
                table: "ChemicalTest",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "SampleTestPlanID",
                table: "ChemicalTest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTest_TestPlans_SamplePlanID",
                table: "ChemicalTest",
                column: "SamplePlanID",
                principalTable: "TestPlans",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemicalTest_TestPlans_SamplePlanID",
                table: "ChemicalTest");

            migrationBuilder.DropColumn(
                name: "SampleTestPlanID",
                table: "ChemicalTest");

            migrationBuilder.AlterColumn<long>(
                name: "SamplePlanID",
                table: "ChemicalTest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemicalTest_TestPlans_SamplePlanID",
                table: "ChemicalTest",
                column: "SamplePlanID",
                principalTable: "TestPlans",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
