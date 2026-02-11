using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AIStorytellerEventSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SubscribeToCrimeEvents",
                table: "AIStorytellers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SubscribeToSpeechEvents",
                table: "AIStorytellers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SubscribeToStateEvents",
                table: "AIStorytellers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscribeToCrimeEvents",
                table: "AIStorytellers");

            migrationBuilder.DropColumn(
                name: "SubscribeToSpeechEvents",
                table: "AIStorytellers");

            migrationBuilder.DropColumn(
                name: "SubscribeToStateEvents",
                table: "AIStorytellers");
        }
    }
}
