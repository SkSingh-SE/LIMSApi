using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInwardCollectionTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CollectionTime",
                table: "SampleInwards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_CustomerID",
                table: "SampleInwards",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleInwards_Customers_CustomerID",
                table: "SampleInwards",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleInwards_Customers_CustomerID",
                table: "SampleInwards");

            migrationBuilder.DropIndex(
                name: "IX_SampleInwards_CustomerID",
                table: "SampleInwards");

            migrationBuilder.DropColumn(
                name: "CollectionTime",
                table: "SampleInwards");
        }
    }
}
