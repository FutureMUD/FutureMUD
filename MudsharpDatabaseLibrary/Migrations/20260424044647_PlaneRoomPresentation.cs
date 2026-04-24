using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class PlaneRoomPresentation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomDescriptionAddendum",
                table: "Planes",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "RoomNameFormat",
                table: "Planes",
                type: "varchar(500)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomDescriptionAddendum",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "RoomNameFormat",
                table: "Planes");
        }
    }
}
