using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class HardenAccountCredentialRecovery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CredentialVersion",
                table: "Accounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginAttemptWindowUtc",
                table: "Accounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoginAttempts",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginBlockedUntilUtc",
                table: "Accounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecoveryAttempts",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "RecoveryCredentialVersion",
                table: "Accounts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecoveryExpiresUtc",
                table: "Accounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecoveryIssuedUtc",
                table: "Accounts",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CredentialVersion",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LoginAttemptWindowUtc",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LoginAttempts",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LoginBlockedUntilUtc",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "RecoveryAttempts",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "RecoveryCredentialVersion",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "RecoveryExpiresUtc",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "RecoveryIssuedUtc",
                table: "Accounts");
        }
    }
}
