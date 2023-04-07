using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class PlayerBoards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CalendarId",
                table: "Boards",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "AuthorIsCharacter",
                table: "BoardPosts",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "InGameDateTime",
                table: "BoardPosts",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateIndex(
                name: "IX_Boards_CalendarId",
                table: "Boards",
                column: "CalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Calendars",
                table: "Boards",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Calendars",
                table: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_Boards_CalendarId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "CalendarId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "AuthorIsCharacter",
                table: "BoardPosts");

            migrationBuilder.DropColumn(
                name: "InGameDateTime",
                table: "BoardPosts");
        }
    }
}
