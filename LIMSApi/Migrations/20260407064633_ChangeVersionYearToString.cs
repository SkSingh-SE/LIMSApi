using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVersionYearToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ParameterMasters");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "TestMethodSpecificationVersions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ParameterUnitID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ParameterMasters",
                column: "ParameterUnitID",
                principalTable: "ParameterUnitMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ParameterMasters");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "TestMethodSpecificationVersions",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ParameterUnitID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ParameterMasters",
                column: "ParameterUnitID",
                principalTable: "ParameterUnitMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
