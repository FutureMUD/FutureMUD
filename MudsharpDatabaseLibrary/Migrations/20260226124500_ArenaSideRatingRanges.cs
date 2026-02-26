using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
	public partial class ArenaSideRatingRanges : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<decimal>(
				name: "MaximumRating",
				table: "ArenaEventTypeSides",
				type: "decimal(58,29)",
				nullable: true);

			migrationBuilder.AddColumn<decimal>(
				name: "MinimumRating",
				table: "ArenaEventTypeSides",
				type: "decimal(58,29)",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "MaximumRating",
				table: "ArenaEventTypeSides");

			migrationBuilder.DropColumn(
				name: "MinimumRating",
				table: "ArenaEventTypeSides");
		}
	}
}
