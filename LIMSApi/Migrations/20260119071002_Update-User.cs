using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceUser",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "RemotLogin",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "SamplePrepare",
                table: "UserMasters");

            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                table: "UserMasters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AutoLockAfterAttempts",
                table: "UserMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "UserMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IpRestriction",
                table: "UserMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "UserMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RemoteLogin",
                table: "UserMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SessionTimeout",
                table: "UserMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "UserMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorMethod",
                table: "UserMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnlockMethod",
                table: "UserMasters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "UserMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "AutoLockAfterAttempts",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "IpRestriction",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "RemoteLogin",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "SessionTimeout",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "TwoFactorMethod",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "UnlockMethod",
                table: "UserMasters");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "UserMasters");

            migrationBuilder.AddColumn<bool>(
                name: "DeviceUser",
                table: "UserMasters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RemotLogin",
                table: "UserMasters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SamplePrepare",
                table: "UserMasters",
                type: "bit",
                nullable: true);
        }
    }
}
