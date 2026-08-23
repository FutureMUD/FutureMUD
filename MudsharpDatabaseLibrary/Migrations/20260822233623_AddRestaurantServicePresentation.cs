using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantServicePresentation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChefOpenEmote",
                table: "Restaurants",
                type: "varchar(1000)",
                nullable: false,
                defaultValue: "@ open|opens $0 for service.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ChefPlateEmote",
                table: "Restaurants",
                type: "varchar(1000)",
                nullable: false,
                defaultValue: "@ plate|plates $0 on $1.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ChefReadyEmote",
                table: "Restaurants",
                type: "varchar(1000)",
                nullable: false,
                defaultValue: "@ finish|finishes preparing $0 for service.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ChefStartEmote",
                table: "Restaurants",
                type: "varchar(1000)",
                nullable: false,
                defaultValue: "@ begin|begins preparing $0.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CleanupIntervalSeconds",
                table: "Restaurants",
                type: "int(11)",
                nullable: false,
                defaultValue: 120);

            migrationBuilder.AddColumn<string>(
                name: "ServerClearEmote",
                table: "Restaurants",
                type: "varchar(1000)",
                nullable: false,
                defaultValue: "@ clear|clears $0 from $1.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ServerReturnEmote",
                table: "Restaurants",
                type: "varchar(1000)",
                nullable: false,
                defaultValue: "@ put|puts $0 aside in the kitchen.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ServerServeEmote",
                table: "Restaurants",
                type: "varchar(1000)",
                nullable: false,
                defaultValue: "@ place|places $0 before $1 on $2.")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChefOpenEmote",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ChefPlateEmote",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ChefReadyEmote",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ChefStartEmote",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CleanupIntervalSeconds",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ServerClearEmote",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ServerReturnEmote",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ServerServeEmote",
                table: "Restaurants");
        }
    }
}
