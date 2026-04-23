using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;

#nullable disable

namespace MudSharp.Migrations
{
	[DbContext(typeof(FuturemudDatabaseContext))]
	[Migration("20260423093000_CharacterBodyTraumaMode")]
	public partial class CharacterBodyTraumaMode : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "TraumaMode",
				table: "CharacterBodies",
				type: "int(11)",
				nullable: false,
				defaultValueSql: "'0'");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "TraumaMode",
				table: "CharacterBodies");
		}
	}
}
