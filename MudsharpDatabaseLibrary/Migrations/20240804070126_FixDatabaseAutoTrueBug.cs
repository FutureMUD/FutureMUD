using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class FixDatabaseAutoTrueBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "PreferToFightArmed",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldDefaultValueSql: "b'0'");

            migrationBuilder.AlterColumn<ulong>(
                name: "PreferShieldUse",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldDefaultValueSql: "b'0'");

            migrationBuilder.AlterColumn<ulong>(
                name: "PreferNonContactClinchBreaking",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldDefaultValueSql: "b'1'");

            migrationBuilder.AlterColumn<ulong>(
                name: "MoveToMeleeIfCannotEngageInRangedCombat",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldDefaultValueSql: "b'1'");

            migrationBuilder.AlterColumn<ulong>(
                name: "SafeQuit",
                table: "CellOverlays",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldDefaultValueSql: "b'1'");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsCore",
                table: "BodypartProto",
                type: "bit(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldDefaultValueSql: "b'1'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "PreferToFightArmed",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'",
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "PreferShieldUse",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'",
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "PreferNonContactClinchBreaking",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'",
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "MoveToMeleeIfCannotEngageInRangedCombat",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'",
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "SafeQuit",
                table: "CellOverlays",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'",
                oldClrType: typeof(ulong),
                oldType: "bit(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsCore",
                table: "BodypartProto",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'",
                oldClrType: typeof(ulong),
                oldType: "bit(1)");
        }
    }
}
