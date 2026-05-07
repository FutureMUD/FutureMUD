using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VirtualCashLedgerAndBanklessSettlement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionHouses_BankAccounts",
                table: "AuctionHouses");

            migrationBuilder.AlterColumn<long>(
                name: "ProfitsBankAccountId",
                table: "AuctionHouses",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.CreateTable(
                name: "VirtualCashBalances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerType = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    OwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualCashBalances_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VirtualCashLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerType = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    OwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RealDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    MudDateTime = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ActorName = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CounterpartyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CounterpartyType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CounterpartyName = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    SourceKind = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DestinationKind = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LinkedBankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ReferenceType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ReferenceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Reference = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Reason = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualCashLedgerEntries_BankAccounts",
                        column: x => x.LinkedBankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VirtualCashLedgerEntries_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_VirtualCashBalances_Currencies_idx",
                table: "VirtualCashBalances",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualCashBalances_Owner_Currency",
                table: "VirtualCashBalances",
                columns: new[] { "OwnerType", "OwnerId", "CurrencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VirtualCashLedgerEntries_BankAccounts_idx",
                table: "VirtualCashLedgerEntries",
                column: "LinkedBankAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_VirtualCashLedgerEntries_Currencies_idx",
                table: "VirtualCashLedgerEntries",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualCashLedgerEntries_Owner_Date",
                table: "VirtualCashLedgerEntries",
                columns: new[] { "OwnerType", "OwnerId", "RealDateTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionHouses_BankAccounts",
                table: "AuctionHouses",
                column: "ProfitsBankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionHouses_BankAccounts",
                table: "AuctionHouses");

            migrationBuilder.DropTable(
                name: "VirtualCashBalances");

            migrationBuilder.DropTable(
                name: "VirtualCashLedgerEntries");

            migrationBuilder.AlterColumn<long>(
                name: "ProfitsBankAccountId",
                table: "AuctionHouses",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionHouses_BankAccounts",
                table: "AuctionHouses",
                column: "ProfitsBankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
