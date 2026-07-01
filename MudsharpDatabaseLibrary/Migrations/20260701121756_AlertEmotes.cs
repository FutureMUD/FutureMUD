using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AlertEmotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultAlertEmote",
                table: "Races",
                type: "varchar(500)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "DefaultDistantAlertEmote",
                table: "Races",
                type: "varchar(500)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "CustomAlertEmote",
                table: "Characters",
                type: "varchar(500)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "CustomDistantAlertEmote",
                table: "Characters",
                type: "varchar(500)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultAlertEmote",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "DefaultDistantAlertEmote",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "CustomAlertEmote",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CustomDistantAlertEmote",
                table: "Characters");
        }
    }
}
