using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ClanBudgetsAndPayrollHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClanBudgets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BankAccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AmountPerPeriod = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    PeriodIntervalType = table.Column<int>(type: "int(11)", nullable: false),
                    PeriodIntervalModifier = table.Column<int>(type: "int(11)", nullable: false),
                    PeriodIntervalOther = table.Column<int>(type: "int(11)", nullable: false),
                    PeriodIntervalOtherSecondary = table.Column<int>(type: "int(11)", nullable: false),
                    PeriodIntervalFallback = table.Column<int>(type: "int(11)", nullable: false),
                    CurrentPeriodStart = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CurrentPeriodEnd = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CurrentPeriodDrawdown = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    IsActive = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClanBudgets_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanBudgets_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanBudgets_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanBudgets_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClanPayrollHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PaygradeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    EntryType = table.Column<int>(type: "int(11)", nullable: false),
                    DateTime = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClanPayrollHistories_Actors",
                        column: x => x.ActorId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClanPayrollHistories_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClanPayrollHistories_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanPayrollHistories_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanPayrollHistories_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanPayrollHistories_Paygrades",
                        column: x => x.PaygradeId,
                        principalTable: "Paygrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClanPayrollHistories_Ranks",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClanBudgetTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClanBudgetId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BankAccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TransactionTime = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PeriodStart = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PeriodEnd = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    BankBalanceAfter = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Reason = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClanBudgetTransactions_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanBudgetTransactions_Budgets",
                        column: x => x.ClanBudgetId,
                        principalTable: "ClanBudgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanBudgetTransactions_Characters",
                        column: x => x.ActorId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanBudgetTransactions_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_ClanBudgets_Appointments_idx",
                table: "ClanBudgets",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanBudgets_BankAccounts_idx",
                table: "ClanBudgets",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanBudgets_Clans_idx",
                table: "ClanBudgets",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanBudgets_Currencies_idx",
                table: "ClanBudgets",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanBudgetTransactions_Actors_idx",
                table: "ClanBudgetTransactions",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanBudgetTransactions_BankAccounts_idx",
                table: "ClanBudgetTransactions",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanBudgetTransactions_Budgets_idx",
                table: "ClanBudgetTransactions",
                column: "ClanBudgetId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanBudgetTransactions_Currencies_idx",
                table: "ClanBudgetTransactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanPayrollHistories_Actors_idx",
                table: "ClanPayrollHistories",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanPayrollHistories_Appointments_idx",
                table: "ClanPayrollHistories",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanPayrollHistories_Characters_idx",
                table: "ClanPayrollHistories",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanPayrollHistories_Clans_idx",
                table: "ClanPayrollHistories",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanPayrollHistories_Currencies_idx",
                table: "ClanPayrollHistories",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanPayrollHistories_Paygrades_idx",
                table: "ClanPayrollHistories",
                column: "PaygradeId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanPayrollHistories_Ranks_idx",
                table: "ClanPayrollHistories",
                column: "RankId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClanBudgetTransactions");

            migrationBuilder.DropTable(
                name: "ClanPayrollHistories");

            migrationBuilder.DropTable(
                name: "ClanBudgets");
        }
    }
}
