using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddRacePainToleranceMultiplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PainToleranceMultiplier",
                table: "Races",
                type: "double",
                nullable: false,
                defaultValueSql: "'1'");

            migrationBuilder.Sql(
                "UPDATE `Races` SET `PainToleranceMultiplier` = `BodypartHealthMultiplier`;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PainToleranceMultiplier",
                table: "Races");
        }
    }
}
