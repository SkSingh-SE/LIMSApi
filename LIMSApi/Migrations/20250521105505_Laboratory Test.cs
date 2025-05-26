using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class LaboratoryTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedTimeDuration",
                table: "TestMethodSubGroups");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "ParameterMasters");

            migrationBuilder.AddColumn<string>(
                name: "ElementType",
                table: "ParameterMasters",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElementType",
                table: "ParameterMasters");

            migrationBuilder.AddColumn<int>(
                name: "FixedTimeDuration",
                table: "TestMethodSubGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "ParameterMasters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
