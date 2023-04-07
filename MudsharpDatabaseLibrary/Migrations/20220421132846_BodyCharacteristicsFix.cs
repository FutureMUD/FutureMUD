using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class BodyCharacteristicsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characteristics_Bodies",
                table: "Characteristics");

            migrationBuilder.AddForeignKey(
                name: "FK_Characteristics_Bodies",
                table: "Characteristics",
                column: "BodyId",
                principalTable: "Bodies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characteristics_Bodies",
                table: "Characteristics");

            migrationBuilder.AddForeignKey(
                name: "FK_Characteristics_Bodies",
                table: "Characteristics",
                column: "BodyId",
                principalTable: "Bodies",
                principalColumn: "Id");
        }
    }
}
