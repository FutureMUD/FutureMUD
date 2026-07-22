using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MultiTargetCombatActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaximumTargets",
                table: "WeaponAttacks",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'1'");

            migrationBuilder.AddColumn<int>(
                name: "MaximumTargets",
                table: "CombatActions",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "'1'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumTargets",
                table: "WeaponAttacks");

            migrationBuilder.DropColumn(
                name: "MaximumTargets",
                table: "CombatActions");
        }
    }
}
