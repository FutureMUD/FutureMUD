using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RangedWeaponSecondaryDamage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PainBonusExpression",
                table: "RangedWeaponTypes",
                type: "text",
                nullable: false,
                defaultValue: "0",
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StunBonusExpression",
                table: "RangedWeaponTypes",
                type: "text",
                nullable: false,
                defaultValue: "0",
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PainBonusExpression",
                table: "RangedWeaponTypes");

            migrationBuilder.DropColumn(
                name: "StunBonusExpression",
                table: "RangedWeaponTypes");
        }
    }
}
