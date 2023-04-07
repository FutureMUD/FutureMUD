using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class ShopBankAccountsAndFinance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BankAccountId",
                table: "Shops",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CashBalance",
                table: "Shops",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Shops_BankAccountId",
                table: "Shops",
                column: "BankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_BankAccounts",
                table: "Shops",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_BankAccounts",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_BankAccountId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "CashBalance",
                table: "Shops");
        }
    }
}
