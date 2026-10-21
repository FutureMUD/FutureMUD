using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurableRaceAgeDescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AgeDescriptionProgId",
                table: "Races",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Races_AgeDescriptionProgId",
                table: "Races",
                column: "AgeDescriptionProgId");

            migrationBuilder.AddForeignKey(
                name: "FK_Races_AgeDescriptionProg",
                table: "Races",
                column: "AgeDescriptionProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Races_AgeDescriptionProg",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Races_AgeDescriptionProgId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "AgeDescriptionProgId",
                table: "Races");
        }
    }
}
