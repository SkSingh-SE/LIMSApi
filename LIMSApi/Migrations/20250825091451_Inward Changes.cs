using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class InwardChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InwardAddresses_SampleInwards_SampleID",
                table: "InwardAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_InwardContacts_SampleInwards_SampleInwardID",
                table: "InwardContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_SampleInwards_SampleInwardID",
                table: "SampleDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDispatchModes_SampleInwards_SampleInwardID",
                table: "SampleDispatchModes");

            migrationBuilder.DropIndex(
                name: "IX_SampleDispatchModes_SampleInwardID",
                table: "SampleDispatchModes");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_SampleInwardID",
                table: "SampleDetails");

            migrationBuilder.DropIndex(
                name: "IX_InwardContacts_SampleInwardID",
                table: "InwardContacts");

            migrationBuilder.DropColumn(
                name: "SampleInwardID",
                table: "SampleDispatchModes");

            migrationBuilder.DropColumn(
                name: "SampleInwardID",
                table: "SampleDetails");

            migrationBuilder.DropColumn(
                name: "SampleInwardID",
                table: "InwardContacts");

            migrationBuilder.RenameColumn(
                name: "SampleID",
                table: "SampleDispatchModes",
                newName: "InwardID");

            migrationBuilder.RenameColumn(
                name: "SampleID",
                table: "SampleDetails",
                newName: "InwardID");

            migrationBuilder.RenameColumn(
                name: "SampleID",
                table: "InwardContacts",
                newName: "InwardID");

            migrationBuilder.RenameColumn(
                name: "SampleID",
                table: "InwardAddresses",
                newName: "InwardID");

            migrationBuilder.RenameIndex(
                name: "IX_InwardAddresses_SampleID",
                table: "InwardAddresses",
                newName: "IX_InwardAddresses_InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDispatchModes_InwardID",
                table: "SampleDispatchModes",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_InwardID",
                table: "SampleDetails",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleAdditionalDetails_SampleID",
                table: "SampleAdditionalDetails",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_InwardContacts_InwardID",
                table: "InwardContacts",
                column: "InwardID");

            migrationBuilder.AddForeignKey(
                name: "FK_InwardAddresses_SampleInwards_InwardID",
                table: "InwardAddresses",
                column: "InwardID",
                principalTable: "SampleInwards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InwardContacts_SampleInwards_InwardID",
                table: "InwardContacts",
                column: "InwardID",
                principalTable: "SampleInwards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleID",
                table: "SampleAdditionalDetails",
                column: "SampleID",
                principalTable: "SampleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_SampleInwards_InwardID",
                table: "SampleDetails",
                column: "InwardID",
                principalTable: "SampleInwards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDispatchModes_SampleInwards_InwardID",
                table: "SampleDispatchModes",
                column: "InwardID",
                principalTable: "SampleInwards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InwardAddresses_SampleInwards_InwardID",
                table: "InwardAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_InwardContacts_SampleInwards_InwardID",
                table: "InwardContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleAdditionalDetails_SampleDetails_SampleID",
                table: "SampleAdditionalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDetails_SampleInwards_InwardID",
                table: "SampleDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleDispatchModes_SampleInwards_InwardID",
                table: "SampleDispatchModes");

            migrationBuilder.DropIndex(
                name: "IX_SampleDispatchModes_InwardID",
                table: "SampleDispatchModes");

            migrationBuilder.DropIndex(
                name: "IX_SampleDetails_InwardID",
                table: "SampleDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleAdditionalDetails_SampleID",
                table: "SampleAdditionalDetails");

            migrationBuilder.DropIndex(
                name: "IX_InwardContacts_InwardID",
                table: "InwardContacts");

            migrationBuilder.RenameColumn(
                name: "InwardID",
                table: "SampleDispatchModes",
                newName: "SampleID");

            migrationBuilder.RenameColumn(
                name: "InwardID",
                table: "SampleDetails",
                newName: "SampleID");

            migrationBuilder.RenameColumn(
                name: "InwardID",
                table: "InwardContacts",
                newName: "SampleID");

            migrationBuilder.RenameColumn(
                name: "InwardID",
                table: "InwardAddresses",
                newName: "SampleID");

            migrationBuilder.RenameIndex(
                name: "IX_InwardAddresses_InwardID",
                table: "InwardAddresses",
                newName: "IX_InwardAddresses_SampleID");

            migrationBuilder.AddColumn<long>(
                name: "SampleInwardID",
                table: "SampleDispatchModes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SampleInwardID",
                table: "SampleDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SampleInwardID",
                table: "InwardContacts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleDispatchModes_SampleInwardID",
                table: "SampleDispatchModes",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_SampleInwardID",
                table: "SampleDetails",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_InwardContacts_SampleInwardID",
                table: "InwardContacts",
                column: "SampleInwardID");

            migrationBuilder.AddForeignKey(
                name: "FK_InwardAddresses_SampleInwards_SampleID",
                table: "InwardAddresses",
                column: "SampleID",
                principalTable: "SampleInwards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InwardContacts_SampleInwards_SampleInwardID",
                table: "InwardContacts",
                column: "SampleInwardID",
                principalTable: "SampleInwards",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDetails_SampleInwards_SampleInwardID",
                table: "SampleDetails",
                column: "SampleInwardID",
                principalTable: "SampleInwards",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleDispatchModes_SampleInwards_SampleInwardID",
                table: "SampleDispatchModes",
                column: "SampleInwardID",
                principalTable: "SampleInwards",
                principalColumn: "ID");
        }
    }
}
