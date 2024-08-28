using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CombatSettingsAugust24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttackUnarmedOrHelpless",
                table: "CharacterCombatSettings",
                newName: "AttackUnarmed");

            migrationBuilder.AddColumn<ulong>(
                name: "AttackHelpless",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttackHelpless",
                table: "CharacterCombatSettings");

            migrationBuilder.RenameColumn(
                name: "AttackUnarmed",
                table: "CharacterCombatSettings",
                newName: "AttackUnarmedOrHelpless");
        }
    }
}
