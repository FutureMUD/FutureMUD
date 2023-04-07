using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class MoreSpellStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CastingDifficulty",
                table: "MagicSpells",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "CastingTraitDefinitionId",
                table: "MagicSpells",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ResistingDifficulty",
                table: "MagicSpells",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ResistingTraitDefinitionId",
                table: "MagicSpells",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_MagicSpells_TraitDefinitions_Casting_idx",
                table: "MagicSpells",
                column: "CastingTraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicSpells_TraitDefinitions_Resisting_idx",
                table: "MagicSpells",
                column: "ResistingTraitDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MagicSpells_TraitDefinitions_Casting",
                table: "MagicSpells",
                column: "CastingTraitDefinitionId",
                principalTable: "TraitDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicSpells_TraitDefinitions_Resisting",
                table: "MagicSpells",
                column: "ResistingTraitDefinitionId",
                principalTable: "TraitDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicSpells_TraitDefinitions_Casting",
                table: "MagicSpells");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicSpells_TraitDefinitions_Resisting",
                table: "MagicSpells");

            migrationBuilder.DropIndex(
                name: "FK_MagicSpells_TraitDefinitions_Casting_idx",
                table: "MagicSpells");

            migrationBuilder.DropIndex(
                name: "FK_MagicSpells_TraitDefinitions_Resisting_idx",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "CastingDifficulty",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "CastingTraitDefinitionId",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "ResistingDifficulty",
                table: "MagicSpells");

            migrationBuilder.DropColumn(
                name: "ResistingTraitDefinitionId",
                table: "MagicSpells");
        }
    }
}
