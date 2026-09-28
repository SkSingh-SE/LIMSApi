using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPanLogoApplicable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PanNumber",
                table: "GstConfigs",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "NablAccreditations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ApplicableFor",
                table: "AuthorizedSignatories",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PanNumber",
                table: "GstConfigs");

            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "NablAccreditations");

            migrationBuilder.DropColumn(
                name: "ApplicableFor",
                table: "AuthorizedSignatories");
        }
    }
}
