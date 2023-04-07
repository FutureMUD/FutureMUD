using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class JournalUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CharacterId",
                table: "AccountNotes",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_AccountNotes_Characters_idx",
                table: "AccountNotes",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountNotes_Characters",
                table: "AccountNotes",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountNotes_Characters",
                table: "AccountNotes");

            migrationBuilder.DropIndex(
                name: "FK_AccountNotes_Characters_idx",
                table: "AccountNotes");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "AccountNotes");
        }
    }
}
