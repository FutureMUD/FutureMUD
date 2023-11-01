using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MagicResourceColours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BottomColour",
                table: "MagicResources",
                type: "varchar(100)",
                nullable: true,
                defaultValue: "[35m",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "MidColour",
                table: "MagicResources",
                type: "varchar(100)",
                nullable: true,
                defaultValue: "[1;35m",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "MagicResources",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "TopColour",
                table: "MagicResources",
                type: "varchar(100)",
                nullable: true,
                defaultValue: "[0m[38;5;171m",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BottomColour",
                table: "MagicResources");

            migrationBuilder.DropColumn(
                name: "MidColour",
                table: "MagicResources");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "MagicResources");

            migrationBuilder.DropColumn(
                name: "TopColour",
                table: "MagicResources");
        }
    }
}
