using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;

#nullable disable

namespace MudSharp.Migrations
{
	[DbContext(typeof(FuturemudDatabaseContext))]
	[Migration("20260424000000_RaceAttributeAlterations")]
	public partial class RaceAttributeAlterations : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Races_AttributeBonusProg",
				table: "Races");

			migrationBuilder.DropIndex(
				name: "FK_Races_AttributeBonusProg",
				table: "Races");

			migrationBuilder.DropColumn(
				name: "AttributeBonusProgId",
				table: "Races");

			migrationBuilder.AddColumn<double>(
				name: "AttributeBonus",
				table: "Races_Attributes",
				type: "double",
				nullable: false,
				defaultValue: 0.0);

			migrationBuilder.AddColumn<string>(
				name: "DiceExpression",
				table: "Races_Attributes",
				type: "varchar(255)",
				maxLength: 255,
				nullable: true,
				collation: "utf8_general_ci")
				.Annotation("MySql:CharSet", "utf8");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AttributeBonus",
				table: "Races_Attributes");

			migrationBuilder.DropColumn(
				name: "DiceExpression",
				table: "Races_Attributes");

			migrationBuilder.AddColumn<long>(
				name: "AttributeBonusProgId",
				table: "Races",
				type: "bigint(20)",
				nullable: false,
				defaultValue: 1L);

			migrationBuilder.CreateIndex(
				name: "FK_Races_AttributeBonusProg",
				table: "Races",
				column: "AttributeBonusProgId");

			migrationBuilder.AddForeignKey(
				name: "FK_Races_AttributeBonusProg",
				table: "Races",
				column: "AttributeBonusProgId",
				principalTable: "FutureProgs",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
