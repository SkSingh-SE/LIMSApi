using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class MeesageTemplateUpdateUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageTemplate_TemplateKey_Channel_Version",
                table: "MessageTemplates");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "MessageTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "MessageTemplates",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplate_TemplateKey_Channel_Version",
                table: "MessageTemplates",
                columns: new[] { "TemplateKey", "Type", "Version" },
                unique: true);
        }
    }
}
