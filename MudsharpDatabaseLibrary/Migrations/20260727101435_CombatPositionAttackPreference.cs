using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CombatPositionAttackPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "PreferToStandOverAttacking",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferToStandOverAttacking",
                table: "CharacterCombatSettings");
        }
    }
}
