using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class SeasonsDisplayUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Seasons");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Seasons",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "SeasonGroup",
                table: "Seasons",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "SeasonGroup",
                table: "Seasons");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Seasons",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }
    }
}
