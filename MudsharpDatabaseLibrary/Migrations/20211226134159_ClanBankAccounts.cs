using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class ClanBankAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BankAccountId",
                table: "Clans",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clans_BankAccountId",
                table: "Clans",
                column: "BankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clans_BankAccounts",
                table: "Clans",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clans_BankAccounts",
                table: "Clans");

            migrationBuilder.DropIndex(
                name: "IX_Clans_BankAccountId",
                table: "Clans");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "Clans");
        }
    }
}
