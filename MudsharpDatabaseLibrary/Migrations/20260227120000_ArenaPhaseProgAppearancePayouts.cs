using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
	public partial class ArenaPhaseProgAppearancePayouts : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<long>(
				name: "OnArenaEventPhaseProgId",
				table: "Arenas",
				type: "bigint(20)",
				nullable: true);

			migrationBuilder.AddColumn<ulong>(
				name: "PayNpcAppearanceFee",
				table: "ArenaEventTypes",
				type: "bit(1)",
				nullable: false,
				defaultValueSql: "b'0'");

			migrationBuilder.AddColumn<ulong>(
				name: "PayNpcAppearanceFee",
				table: "ArenaEvents",
				type: "bit(1)",
				nullable: false,
				defaultValueSql: "b'0'");

			migrationBuilder.AddColumn<int>(
				name: "PayoutType",
				table: "ArenaBetPayouts",
				type: "int(11)",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateIndex(
				name: "FK_Arenas_OnArenaEventPhaseProg",
				table: "Arenas",
				column: "OnArenaEventPhaseProgId");

			migrationBuilder.AddForeignKey(
				name: "FK_Arenas_OnArenaEventPhaseProg",
				table: "Arenas",
				column: "OnArenaEventPhaseProgId",
				principalTable: "FutureProgs",
				principalColumn: "Id",
				onDelete: ReferentialAction.SetNull);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Arenas_OnArenaEventPhaseProg",
				table: "Arenas");

			migrationBuilder.DropIndex(
				name: "FK_Arenas_OnArenaEventPhaseProg",
				table: "Arenas");

			migrationBuilder.DropColumn(
				name: "OnArenaEventPhaseProgId",
				table: "Arenas");

			migrationBuilder.DropColumn(
				name: "PayNpcAppearanceFee",
				table: "ArenaEventTypes");

			migrationBuilder.DropColumn(
				name: "PayNpcAppearanceFee",
				table: "ArenaEvents");

			migrationBuilder.DropColumn(
				name: "PayoutType",
				table: "ArenaBetPayouts");
		}
	}
}
