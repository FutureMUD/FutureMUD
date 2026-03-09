using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCharacterCaloriesWithSatiationReserve : Migration
    {
        /// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<double>(
				name: "SatiationReserve",
				table: "Characters",
				type: "double",
				nullable: false,
				defaultValue: 0.0);

			migrationBuilder.DropColumn(
				name: "Calories",
				table: "Characters");
		}

        /// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<double>(
				name: "Calories",
				table: "Characters",
				type: "double",
				nullable: false,
				defaultValue: 0.0);

			migrationBuilder.DropColumn(
				name: "SatiationReserve",
				table: "Characters");
		}
    }
}
