using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class ISOMasterUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GroupID",
                table: "SubGroupMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "DisciplineID",
                table: "GroupMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SubGroupMasters_GroupID",
                table: "SubGroupMasters",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMasters_DisciplineID",
                table: "GroupMasters",
                column: "DisciplineID");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMasters_DisciplineMasters_DisciplineID",
                table: "GroupMasters",
                column: "DisciplineID",
                principalTable: "DisciplineMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubGroupMasters_GroupMasters_GroupID",
                table: "SubGroupMasters",
                column: "GroupID",
                principalTable: "GroupMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMasters_DisciplineMasters_DisciplineID",
                table: "GroupMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_SubGroupMasters_GroupMasters_GroupID",
                table: "SubGroupMasters");

            migrationBuilder.DropIndex(
                name: "IX_SubGroupMasters_GroupID",
                table: "SubGroupMasters");

            migrationBuilder.DropIndex(
                name: "IX_GroupMasters_DisciplineID",
                table: "GroupMasters");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "SubGroupMasters");

            migrationBuilder.DropColumn(
                name: "DisciplineID",
                table: "GroupMasters");
        }
    }
}
