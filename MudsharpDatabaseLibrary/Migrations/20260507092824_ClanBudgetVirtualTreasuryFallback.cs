using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ClanBudgetVirtualTreasuryFallback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClanBudgets_BankAccounts",
                table: "ClanBudgets");

            migrationBuilder.DropForeignKey(
                name: "FK_ClanBudgetTransactions_BankAccounts",
                table: "ClanBudgetTransactions");

            migrationBuilder.AlterColumn<long>(
                name: "BankAccountId",
                table: "ClanBudgetTransactions",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AlterColumn<long>(
                name: "BankAccountId",
                table: "ClanBudgets",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AddForeignKey(
                name: "FK_ClanBudgets_BankAccounts",
                table: "ClanBudgets",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClanBudgetTransactions_BankAccounts",
                table: "ClanBudgetTransactions",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClanBudgets_BankAccounts",
                table: "ClanBudgets");

            migrationBuilder.DropForeignKey(
                name: "FK_ClanBudgetTransactions_BankAccounts",
                table: "ClanBudgetTransactions");

            migrationBuilder.AlterColumn<long>(
                name: "BankAccountId",
                table: "ClanBudgetTransactions",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BankAccountId",
                table: "ClanBudgets",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClanBudgets_BankAccounts",
                table: "ClanBudgets",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClanBudgetTransactions_BankAccounts",
                table: "ClanBudgetTransactions",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
