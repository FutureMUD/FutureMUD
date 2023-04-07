using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class NameCulturesChargenExpansion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UseForChargenSuggestionsProgId",
                table: "RandomNameProfiles",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RandomNameProfiles_UseForChargenSuggestionsProgId",
                table: "RandomNameProfiles",
                column: "UseForChargenSuggestionsProgId");

            migrationBuilder.AddForeignKey(
                name: "FK_RandomNameProfiles_FutureProgs_UseForChargenSuggestionsProgId",
                table: "RandomNameProfiles",
                column: "UseForChargenSuggestionsProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RandomNameProfiles_FutureProgs_UseForChargenSuggestionsProgId",
                table: "RandomNameProfiles");

            migrationBuilder.DropIndex(
                name: "IX_RandomNameProfiles_UseForChargenSuggestionsProgId",
                table: "RandomNameProfiles");

            migrationBuilder.DropColumn(
                name: "UseForChargenSuggestionsProgId",
                table: "RandomNameProfiles");
        }
    }
}
