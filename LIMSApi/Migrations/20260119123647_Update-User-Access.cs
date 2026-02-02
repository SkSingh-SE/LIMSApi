using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ForcePasswordChange",
                table: "UserMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLoginEnabled",
                table: "UserMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TwoFactorLastSentAt",
                table: "UserMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorOtp",
                table: "UserMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TwoFactorOtpExpiry",
                table: "UserMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TwoFactorSendCount",
                table: "UserMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForcePasswordChange",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "IsLoginEnabled",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "TwoFactorLastSentAt",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "TwoFactorOtp",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "TwoFactorOtpExpiry",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "TwoFactorSendCount",
                table: "UserMasters");
        }
    }
}
