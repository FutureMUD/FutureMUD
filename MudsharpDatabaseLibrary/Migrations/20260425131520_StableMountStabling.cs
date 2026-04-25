using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class StableMountStabling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsTrading = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul),
                    LodgeFee = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    DailyFee = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    LodgeFeeProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DailyFeeProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanStableProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WhyCannotStableProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    EmployeeRecords = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stables_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Stables_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stables_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stables_FutureProgs_Can",
                        column: x => x.CanStableProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Stables_FutureProgs_Daily",
                        column: x => x.DailyFeeProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Stables_FutureProgs_Lodge",
                        column: x => x.LodgeFeeProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Stables_FutureProgs_Why",
                        column: x => x.WhyCannotStableProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StableAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StableId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccountName = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AccountOwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccountOwnerName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Balance = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    IsSuspended = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StableAccounts_Characters",
                        column: x => x.AccountOwnerId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StableAccounts_Stables",
                        column: x => x.StableId,
                        principalTable: "Stables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StableStays",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StableId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OriginalOwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OriginalOwnerName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LodgedDateTime = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LastDailyFeeDateTime = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ClosedDateTime = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    TicketItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TicketToken = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AmountOwing = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StableStays_Characters_Mount",
                        column: x => x.MountId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StableStays_Characters_Owner",
                        column: x => x.OriginalOwnerId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StableStays_GameItems_Ticket",
                        column: x => x.TicketItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StableStays_Stables",
                        column: x => x.StableId,
                        principalTable: "Stables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StableAccountUsers",
                columns: table => new
                {
                    StableAccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccountUserId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccountUserName = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    SpendingLimit = table.Column<decimal>(type: "decimal(58,29)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.StableAccountId, x.AccountUserId });
                    table.ForeignKey(
                        name: "FK_StableAccountUsers_Characters",
                        column: x => x.AccountUserId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StableAccountUsers_StableAccounts",
                        column: x => x.StableAccountId,
                        principalTable: "StableAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StableStayLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StableStayId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EntryType = table.Column<int>(type: "int(11)", nullable: false),
                    MudDateTime = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ActorName = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Note = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StableStayLedgerEntries_Characters",
                        column: x => x.ActorId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StableStayLedgerEntries_StableStays",
                        column: x => x.StableStayId,
                        principalTable: "StableStays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_StableAccounts_Characters_idx",
                table: "StableAccounts",
                column: "AccountOwnerId");

            migrationBuilder.CreateIndex(
                name: "FK_StableAccounts_Stables_idx",
                table: "StableAccounts",
                column: "StableId");

            migrationBuilder.CreateIndex(
                name: "FK_StableAccountUsers_Characters_idx",
                table: "StableAccountUsers",
                column: "AccountUserId");

            migrationBuilder.CreateIndex(
                name: "FK_Stables_BankAccounts_idx",
                table: "Stables",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_Stables_Cells_idx",
                table: "Stables",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_Stables_EconomicZones_idx",
                table: "Stables",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_Stables_FutureProgs_Can_idx",
                table: "Stables",
                column: "CanStableProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Stables_FutureProgs_Daily_idx",
                table: "Stables",
                column: "DailyFeeProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Stables_FutureProgs_Lodge_idx",
                table: "Stables",
                column: "LodgeFeeProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Stables_FutureProgs_Why_idx",
                table: "Stables",
                column: "WhyCannotStableProgId");

            migrationBuilder.CreateIndex(
                name: "FK_StableStayLedgerEntries_Characters_idx",
                table: "StableStayLedgerEntries",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "FK_StableStayLedgerEntries_StableStays_idx",
                table: "StableStayLedgerEntries",
                column: "StableStayId");

            migrationBuilder.CreateIndex(
                name: "FK_StableStays_Characters_Mount_idx",
                table: "StableStays",
                column: "MountId");

            migrationBuilder.CreateIndex(
                name: "FK_StableStays_Characters_Owner_idx",
                table: "StableStays",
                column: "OriginalOwnerId");

            migrationBuilder.CreateIndex(
                name: "FK_StableStays_GameItems_Ticket_idx",
                table: "StableStays",
                column: "TicketItemId");

            migrationBuilder.CreateIndex(
                name: "FK_StableStays_Stables_idx",
                table: "StableStays",
                column: "StableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StableAccountUsers");

            migrationBuilder.DropTable(
                name: "StableStayLedgerEntries");

            migrationBuilder.DropTable(
                name: "StableAccounts");

            migrationBuilder.DropTable(
                name: "StableStays");

            migrationBuilder.DropTable(
                name: "Stables");

        }
    }
}
