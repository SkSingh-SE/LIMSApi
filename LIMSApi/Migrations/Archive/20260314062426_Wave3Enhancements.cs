using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Wave3Enhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerFeedbackId",
                table: "NablFeedbackAnalyses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NablFeedbackAnalyses_CustomerFeedbackId",
                table: "NablFeedbackAnalyses",
                column: "CustomerFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_NablFeedbackAnalyses_NablCustomerFeedbacks_CustomerFeedbackId",
                table: "NablFeedbackAnalyses",
                column: "CustomerFeedbackId",
                principalTable: "NablCustomerFeedbacks",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NablFeedbackAnalyses_NablCustomerFeedbacks_CustomerFeedbackId",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_NablFeedbackAnalyses_CustomerFeedbackId",
                table: "NablFeedbackAnalyses");

            migrationBuilder.DropColumn(
                name: "CustomerFeedbackId",
                table: "NablFeedbackAnalyses");
        }
    }
}
