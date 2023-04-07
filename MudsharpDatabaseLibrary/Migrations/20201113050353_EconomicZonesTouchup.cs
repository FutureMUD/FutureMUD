using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class EconomicZonesTouchup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ControllingClanId",
                table: "EconomicZones",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZones_ControllingClans_idx",
                table: "EconomicZones",
                column: "ControllingClanId");

            migrationBuilder.AddForeignKey(
                name: "FK_EconomicZones_ControllingClans",
                table: "EconomicZones",
                column: "ControllingClanId",
                principalTable: "Clans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EconomicZones_ControllingClans",
                table: "EconomicZones");

            migrationBuilder.DropIndex(
                name: "FK_EconomicZones_ControllingClans_idx",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "ControllingClanId",
                table: "EconomicZones");
        }
    }
}
