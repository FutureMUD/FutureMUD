using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class BanksV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Order",
                table: "TimeZoneInfos",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax",
                table: "ShopTransactionRecords",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PretaxValue",
                table: "ShopTransactionRecords",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalesTax",
                table: "ShopFinancialPeriodResults",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProfitsTax",
                table: "ShopFinancialPeriodResults",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NetRevenue",
                table: "ShopFinancialPeriodResults",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "GrossRevenue",
                table: "ShopFinancialPeriodResults",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PayAmount",
                table: "Paygrades",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BasePrice",
                table: "Merchandises",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AutoReorderPrice",
                table: "Merchandises",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SpendingLimit",
                table: "LineOfCreditAccountUsers",
                type: "decimal(58,29)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OutstandingBalance",
                table: "LineOfCreditAccounts",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AccountLimit",
                table: "LineOfCreditAccounts",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StandardFine",
                table: "Laws",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumFine",
                table: "Laws",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaximumFine",
                table: "Laws",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxesInCredits",
                table: "EconomicZoneShopTaxes",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OutstandingSalesTaxes",
                table: "EconomicZoneShopTaxes",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OutstandingProfitTaxes",
                table: "EconomicZoneShopTaxes",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalRevenueHeld",
                table: "EconomicZones",
                type: "decimal(58,29)",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)",
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<decimal>(
                name: "OutstandingTaxesOwed",
                table: "EconomicZones",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalTaxRevenue",
                table: "EconomicZoneRevenues",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BaseUnitConversionRate",
                table: "CurrencyDivisions",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "CurrencyDescriptionPatternElementSpecialValues",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Coins",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ClanMemberships_Backpay",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ChargenRoles_Currencies",
                type: "decimal(58,29)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Code = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PrimaryCurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MaximumBankAccountsPerCustomer = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_Currencies",
                        column: x => x.PrimaryCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Banks_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankAccountTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CustomerDescription = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    MaximumOverdrawAmount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    WithdrawalFleeFlat = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    WithdrawalFleeRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    DepositFeeFlat = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    DepositFeeRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TransferFeeFlat = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TransferFeeRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TransferFeeOtherBankFlat = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TransferFeeOtherBankRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    DailyFee = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    DailyInterestRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    OverdrawFeeFlat = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    OverdrawFeeRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    DailyOverdrawnFee = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    DailyOverdrawnInterestRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    BankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CanOpenAccountProgCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanOpenAccountProgClanId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanOpenAccountProgShopId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanCloseAccountProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccountTypes_Banks",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccountTypes_CanCloseProg",
                        column: x => x.CanCloseAccountProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccountTypes_CharacterProgs",
                        column: x => x.CanOpenAccountProgCharacterId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccountTypes_ClanProgs",
                        column: x => x.CanOpenAccountProgClanId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccountTypes_ShopProgs",
                        column: x => x.CanOpenAccountProgShopId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankBranches",
                columns: table => new
                {
                    BankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BankId, x.CellId });
                    table.ForeignKey(
                        name: "FK_BankBranches_Banks",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankBranches_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankCurrencyReserves",
                columns: table => new
                {
                    BankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BankId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_BankCurrencyReserves_Banks",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankCurrencyReserves_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankExchangeRates",
                columns: table => new
                {
                    BankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FromCurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ToCurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BankId, x.FromCurrencyId, x.ToCurrencyId });
                    table.ForeignKey(
                        name: "FK_BankExchangeRates_Banks",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankExchangeRates_Currencies_From",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankExchangeRates_Currencies_To",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankManagerAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DateTime = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Detail = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankManagerAuditLogs_Banks",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankManagerAuditLogs_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankManagers",
                columns: table => new
                {
                    BankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BankId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_BankManagers_Banks",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankManagers_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AccountNumber = table.Column<int>(type: "int(11)", nullable: false),
                    BankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BankAccountTypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    AccountOwnerCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AccountOwnerClanId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AccountOwnerShopId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NominatedBenefactorAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AccountCreationDate = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AccountStatus = table.Column<int>(type: "int(11)", nullable: false),
                    CurrentMonthInterest = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    CurrentMonthFees = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_BankAccounts",
                        column: x => x.NominatedBenefactorAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccounts_BankAccountTypes",
                        column: x => x.BankAccountTypeId,
                        principalTable: "BankAccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Banks",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Characters",
                        column: x => x.AccountOwnerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Clans",
                        column: x => x.AccountOwnerClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Shops",
                        column: x => x.AccountOwnerShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankAccountTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BankAccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TransactionType = table.Column<int>(type: "int(11)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TransactionTime = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    TransactionDescription = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AccountBalanceAfter = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccountTransactions_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AccountOwnerCharacterId",
                table: "BankAccounts",
                column: "AccountOwnerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AccountOwnerClanId",
                table: "BankAccounts",
                column: "AccountOwnerClanId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AccountOwnerShopId",
                table: "BankAccounts",
                column: "AccountOwnerShopId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_BankAccountTypeId",
                table: "BankAccounts",
                column: "BankAccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_BankId",
                table: "BankAccounts",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_NominatedBenefactorAccountId",
                table: "BankAccounts",
                column: "NominatedBenefactorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountTransactions_BankAccountId",
                table: "BankAccountTransactions",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountTypes_BankId",
                table: "BankAccountTypes",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountTypes_CanCloseAccountProgId",
                table: "BankAccountTypes",
                column: "CanCloseAccountProgId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountTypes_CanOpenAccountProgCharacterId",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountTypes_CanOpenAccountProgClanId",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgClanId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountTypes_CanOpenAccountProgShopId",
                table: "BankAccountTypes",
                column: "CanOpenAccountProgShopId");

            migrationBuilder.CreateIndex(
                name: "IX_BankBranches_CellId",
                table: "BankBranches",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_BankCurrencyReserves_CurrencyId",
                table: "BankCurrencyReserves",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankExchangeRates_FromCurrencyId",
                table: "BankExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankExchangeRates_ToCurrencyId",
                table: "BankExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankManagerAuditLogs_BankId",
                table: "BankManagerAuditLogs",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankManagerAuditLogs_CharacterId",
                table: "BankManagerAuditLogs",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_BankManagers_CharacterId",
                table: "BankManagers",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_EconomicZoneId",
                table: "Banks",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_PrimaryCurrencyId",
                table: "Banks",
                column: "PrimaryCurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccountTransactions");

            migrationBuilder.DropTable(
                name: "BankBranches");

            migrationBuilder.DropTable(
                name: "BankCurrencyReserves");

            migrationBuilder.DropTable(
                name: "BankExchangeRates");

            migrationBuilder.DropTable(
                name: "BankManagerAuditLogs");

            migrationBuilder.DropTable(
                name: "BankManagers");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "BankAccountTypes");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.AlterColumn<decimal>(
                name: "Order",
                table: "TimeZoneInfos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax",
                table: "ShopTransactionRecords",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PretaxValue",
                table: "ShopTransactionRecords",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalesTax",
                table: "ShopFinancialPeriodResults",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProfitsTax",
                table: "ShopFinancialPeriodResults",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NetRevenue",
                table: "ShopFinancialPeriodResults",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "GrossRevenue",
                table: "ShopFinancialPeriodResults",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PayAmount",
                table: "Paygrades",
                type: "decimal(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BasePrice",
                table: "Merchandises",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AutoReorderPrice",
                table: "Merchandises",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SpendingLimit",
                table: "LineOfCreditAccountUsers",
                type: "decimal(10,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OutstandingBalance",
                table: "LineOfCreditAccounts",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AccountLimit",
                table: "LineOfCreditAccounts",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StandardFine",
                table: "Laws",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumFine",
                table: "Laws",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaximumFine",
                table: "Laws",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxesInCredits",
                table: "EconomicZoneShopTaxes",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OutstandingSalesTaxes",
                table: "EconomicZoneShopTaxes",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OutstandingProfitTaxes",
                table: "EconomicZoneShopTaxes",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalRevenueHeld",
                table: "EconomicZones",
                type: "decimal(10,0)",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)",
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<decimal>(
                name: "OutstandingTaxesOwed",
                table: "EconomicZones",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalTaxRevenue",
                table: "EconomicZoneRevenues",
                type: "decimal(10,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BaseUnitConversionRate",
                table: "CurrencyDivisions",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "CurrencyDescriptionPatternElementSpecialValues",
                type: "decimal(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Coins",
                type: "decimal(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ClanMemberships_Backpay",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ChargenRoles_Currencies",
                type: "decimal(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(58,29)");
        }
    }
}
