using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class RemovingChildClans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clans_Parent",
                table: "Clans");

            migrationBuilder.RenameColumn(
                name: "ParentClanId",
                table: "Clans",
                newName: "ClanId");

            migrationBuilder.RenameIndex(
                name: "FK_Clans_Parent",
                table: "Clans",
                newName: "IX_Clans_ClanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clans_Clans_ClanId",
                table: "Clans",
                column: "ClanId",
                principalTable: "Clans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clans_Clans_ClanId",
                table: "Clans");

            migrationBuilder.RenameColumn(
                name: "ClanId",
                table: "Clans",
                newName: "ParentClanId");

            migrationBuilder.RenameIndex(
                name: "IX_Clans_ClanId",
                table: "Clans",
                newName: "FK_Clans_Parent");

            migrationBuilder.AddForeignKey(
                name: "FK_Clans_Parent",
                table: "Clans",
                column: "ParentClanId",
                principalTable: "Clans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
