using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyForeignKeyUpdateJan24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CDPE_CurrencyDivisions",
                table: "CurrencyDescriptionPatternElements");

            migrationBuilder.AddForeignKey(
                name: "FK_CDPE_CurrencyDivisions",
                table: "CurrencyDescriptionPatternElements",
                column: "CurrencyDivisionId",
                principalTable: "CurrencyDivisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CDPE_CurrencyDivisions",
                table: "CurrencyDescriptionPatternElements");

            migrationBuilder.AddForeignKey(
                name: "FK_CDPE_CurrencyDivisions",
                table: "CurrencyDescriptionPatternElements",
                column: "CurrencyDivisionId",
                principalTable: "CurrencyDivisions",
                principalColumn: "Id");
        }
    }
}
