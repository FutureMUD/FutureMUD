using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CraftPhaseExertionAndStamina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExertionLevel",
                table: "CraftPhases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "StaminaUsage",
                table: "CraftPhases",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExertionLevel",
                table: "CraftPhases");

            migrationBuilder.DropColumn(
                name: "StaminaUsage",
                table: "CraftPhases");
        }
    }
}
