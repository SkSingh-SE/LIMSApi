using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddScopeLimitValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LowerLimitValue",
                table: "LabScopeSpecificationParameters",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UpperLimitValue",
                table: "LabScopeSpecificationParameters",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LowerLimitValue",
                table: "LabScopeSpecificationParameters");

            migrationBuilder.DropColumn(
                name: "UpperLimitValue",
                table: "LabScopeSpecificationParameters");
        }
    }
}
