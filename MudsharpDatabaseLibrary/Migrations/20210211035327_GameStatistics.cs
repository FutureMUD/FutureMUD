using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class GameStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "HasBeenActiveInWeek",
                table: "Accounts",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");

            migrationBuilder.CreateTable(
                name: "PlayerActivitySnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    OnlinePlayers = table.Column<int>(type: "int(11)", nullable: false),
                    OnlineAdmins = table.Column<int>(type: "int(11)", nullable: false),
                    AvailableAdmins = table.Column<int>(type: "int(11)", nullable: false),
                    IdlePlayers = table.Column<int>(type: "int(11)", nullable: false),
                    UniquePCLocations = table.Column<int>(type: "int(11)", nullable: false),
                    OnlineGuests = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Start = table.Column<DateTime>(type: "datetime", nullable: false),
                    End = table.Column<DateTime>(type: "datetime", nullable: false),
                    TotalAccounts = table.Column<int>(type: "int(11)", nullable: false),
                    ActiveAccounts = table.Column<int>(type: "int(11)", nullable: false),
                    NewAccounts = table.Column<int>(type: "int(11)", nullable: false),
                    ApplicationsSubmitted = table.Column<int>(type: "int(11)", nullable: false),
                    ApplicationsApproved = table.Column<int>(type: "int(11)", nullable: false),
                    PlayerDeaths = table.Column<int>(type: "int(11)", nullable: false),
                    NonPlayerDeaths = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerActivitySnapshots");

            migrationBuilder.DropTable(
                name: "WeeklyStatistics");

            migrationBuilder.DropColumn(
                name: "HasBeenActiveInWeek",
                table: "Accounts");
        }
    }
}
