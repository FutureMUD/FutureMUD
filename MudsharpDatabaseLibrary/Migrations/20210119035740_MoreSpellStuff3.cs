using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class MoreSpellStuff3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicSpells_TraitDefinitions_Casting",
                table: "MagicSpells");

            migrationBuilder.AlterColumn<long>(
                name: "CastingTraitDefinitionId",
                table: "MagicSpells",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AddForeignKey(
                name: "FK_MagicSpells_TraitDefinitions_Casting",
                table: "MagicSpells",
                column: "CastingTraitDefinitionId",
                principalTable: "TraitDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicSpells_TraitDefinitions_Casting",
                table: "MagicSpells");

            migrationBuilder.AlterColumn<long>(
                name: "CastingTraitDefinitionId",
                table: "MagicSpells",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicSpells_TraitDefinitions_Casting",
                table: "MagicSpells",
                column: "CastingTraitDefinitionId",
                principalTable: "TraitDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
