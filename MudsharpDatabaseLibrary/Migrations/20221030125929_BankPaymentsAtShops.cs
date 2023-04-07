using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class BankPaymentsAtShops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPermittedPaymentItems",
                table: "BankAccountTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "PaymentItemPrototypeId",
                table: "BankAccountTypes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorisedBankPaymentItems",
                table: "BankAccounts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPermittedPaymentItems",
                table: "BankAccountTypes");

            migrationBuilder.DropColumn(
                name: "PaymentItemPrototypeId",
                table: "BankAccountTypes");

            migrationBuilder.DropColumn(
                name: "AuthorisedBankPaymentItems",
                table: "BankAccounts");
        }
    }
}
