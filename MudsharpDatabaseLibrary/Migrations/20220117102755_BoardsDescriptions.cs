using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class BoardsDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorFullDescription",
                table: "BoardPosts",
                type: "varchar(1000)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "BoardPosts",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "AuthorShortDescription",
                table: "BoardPosts",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorFullDescription",
                table: "BoardPosts");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "BoardPosts");

            migrationBuilder.DropColumn(
                name: "AuthorShortDescription",
                table: "BoardPosts");
        }
    }
}
