using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class MoreSpellStuff2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CastingEmote",
                table: "MagicSpells",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<int>(
                name: "CastingEmoteFlags",
                table: "MagicSpells",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "EffectDurationExpressionId",
                table: "MagicSpells",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailCastingEmote",
                table: "MagicSpells",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "TargetEmote",
                table: "MagicSpells",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<int>(
                name: "TargetEmoteFlags",
                table: "MagicSpells",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TargetResistedEmote",
                table: "MagicSpells",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateIndex(
                name: "FK_MagicSpells_TraitExpressions_idx",
                table: "MagicSpells",
                column: "EffectDurationExpressionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MagicSpells_TraitExpressions",
                table: "MagicSpells",
                column: "EffectDurationExpressionId",
                principalTable: "TraitExpression",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicSpells_TraitExpressions",
                table: "MagicSpells");

            migrationBuilder.DropIndex(
                name: "FK_MagicSpells_TraitExpressions_idx",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "CastingEmote",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "CastingEmoteFlags",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "EffectDurationExpressionId",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "FailCastingEmote",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "TargetEmote",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "TargetEmoteFlags",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "TargetResistedEmote",
                table: "MagicSpells");
        }
    }
}
