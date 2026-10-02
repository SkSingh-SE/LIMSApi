using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablJobDecriptionUpdateReportToIdCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablJobDescriptions_DepartmentMasters_DepartmentId",
                table: "NablJobDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_NablJobDescriptions_DesignationMasters_DesignationId",
                table: "NablJobDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_NablJobDescriptions_DepartmentId",
                table: "NablJobDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_NablJobDescriptions_DesignationId",
                table: "NablJobDescriptions");

            migrationBuilder.AlterColumn<long>(
                name: "ReportingToId",
                table: "NablJobDescriptions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DepartmentId",
                table: "NablJobDescriptions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReportingToId",
                table: "NablJobDescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DepartmentId",
                table: "NablJobDescriptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablJobDescriptions_DepartmentId",
                table: "NablJobDescriptions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablJobDescriptions_DesignationId",
                table: "NablJobDescriptions",
                column: "DesignationId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablJobDescriptions_DepartmentMasters_DepartmentId",
                table: "NablJobDescriptions",
                column: "DepartmentId",
                principalTable: "DepartmentMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NablJobDescriptions_DesignationMasters_DesignationId",
                table: "NablJobDescriptions",
                column: "DesignationId",
                principalTable: "DesignationMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
