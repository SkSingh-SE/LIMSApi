using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                table: "UserMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UOMID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_ParameterUnitID",
                table: "ParameterMasters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_UOMID",
                table: "ParameterMasters",
                column: "UOMID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ParameterMasters",
                column: "ParameterUnitID",
                principalTable: "ParameterUnitMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_UOMMasters_UOMID",
                table: "ParameterMasters",
                column: "UOMID",
                principalTable: "UOMMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ParameterMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_UOMMasters_UOMID",
                table: "ParameterMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMasters_ParameterUnitID",
                table: "ParameterMasters");

            migrationBuilder.DropIndex(
                name: "IX_ParameterMasters_UOMID",
                table: "ParameterMasters");

            migrationBuilder.DropColumn(
                name: "RoleName",
                table: "UserMasters");

            migrationBuilder.AlterColumn<int>(
                name: "UOMID",
                table: "ParameterMasters",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
