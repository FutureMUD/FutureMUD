using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class JournalUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InGameTimeStamp",
                table: "AccountNotes",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<ulong>(
                name: "IsJournalEntry",
                table: "AccountNotes",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InGameTimeStamp",
                table: "AccountNotes");

            migrationBuilder.DropColumn(
                name: "IsJournalEntry",
                table: "AccountNotes");
        }
    }
}
