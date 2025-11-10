using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CombatArenaSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arenas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    VirtualBalance = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arenas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Arenas_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Arenas_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Arenas_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaCells",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Role = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaCells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaCells_Arenas",
                        column: x => x.ArenaId,
                        principalTable: "Arenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaCells_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaCombatantClasses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EligibilityProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AdminNpcLoaderProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ResurrectNpcOnDeath = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    DefaultStageNameTemplate = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DefaultSignatureColour = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaCombatantClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaCombatantClasses_AdminNpcLoaderProg",
                        column: x => x.AdminNpcLoaderProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArenaCombatantClasses_Arenas",
                        column: x => x.ArenaId,
                        principalTable: "Arenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaCombatantClasses_EligibilityProg",
                        column: x => x.EligibilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaEventTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    BringYourOwn = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    RegistrationDurationSeconds = table.Column<int>(type: "int(11)", nullable: false),
                    PreparationDurationSeconds = table.Column<int>(type: "int(11)", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "int(11)", nullable: true),
                    BettingModel = table.Column<int>(type: "int(11)", nullable: false),
                    AppearanceFee = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    VictoryFee = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    IntroProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ScoringProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ResolutionOverrideProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaEventTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaEventTypes_Arenas",
                        column: x => x.ArenaId,
                        principalTable: "Arenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaEventTypes_IntroProg",
                        column: x => x.IntroProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArenaEventTypes_ResolutionProg",
                        column: x => x.ResolutionOverrideProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArenaEventTypes_ScoringProg",
                        column: x => x.ScoringProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaManagers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaManagers_Arenas",
                        column: x => x.ArenaId,
                        principalTable: "Arenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaManagers_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaRatings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CombatantClassId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaRatings_Arenas",
                        column: x => x.ArenaId,
                        principalTable: "Arenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaRatings_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaRatings_CombatantClasses",
                        column: x => x.CombatantClassId,
                        principalTable: "ArenaCombatantClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ArenaEventTypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    State = table.Column<int>(type: "int(11)", nullable: false),
                    BringYourOwn = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    RegistrationDurationSeconds = table.Column<int>(type: "int(11)", nullable: false),
                    PreparationDurationSeconds = table.Column<int>(type: "int(11)", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "int(11)", nullable: true),
                    BettingModel = table.Column<int>(type: "int(11)", nullable: false),
                    AppearanceFee = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    VictoryFee = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    RegistrationOpensAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    AbortedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CancellationReason = table.Column<string>(type: "varchar(4000)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaEvents_Arenas",
                        column: x => x.ArenaId,
                        principalTable: "Arenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaEvents_EventTypes",
                        column: x => x.ArenaEventTypeId,
                        principalTable: "ArenaEventTypes",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaEventTypeSides",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaEventTypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Index = table.Column<int>(type: "int(11)", nullable: false),
                    Capacity = table.Column<int>(type: "int(11)", nullable: false),
                    Policy = table.Column<int>(type: "int(11)", nullable: false),
                    AllowNpcSignup = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    AutoFillNpc = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    OutfitProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NpcLoaderProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaEventTypeSides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaEventTypeSides_EventTypes",
                        column: x => x.ArenaEventTypeId,
                        principalTable: "ArenaEventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaEventTypeSides_NpcLoaderProg",
                        column: x => x.NpcLoaderProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArenaEventTypeSides_OutfitProg",
                        column: x => x.OutfitProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaBetPayouts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    IsBlocked = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CollectedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaBetPayouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaBetPayouts_ArenaEvents",
                        column: x => x.ArenaEventId,
                        principalTable: "ArenaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaBetPayouts_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaBetPools",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SideIndex = table.Column<int>(type: "int(11)", nullable: true),
                    TotalStake = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TakeRate = table.Column<decimal>(type: "decimal(58,29)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaBetPools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaBetPools_ArenaEvents",
                        column: x => x.ArenaEventId,
                        principalTable: "ArenaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaBets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SideIndex = table.Column<int>(type: "int(11)", nullable: true),
                    Stake = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    FixedDecimalOdds = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    ModelSnapshot = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IsCancelled = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    PlacedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaBets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaBets_ArenaEvents",
                        column: x => x.ArenaEventId,
                        principalTable: "ArenaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaBets_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaEventSides",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SideIndex = table.Column<int>(type: "int(11)", nullable: false),
                    Capacity = table.Column<int>(type: "int(11)", nullable: false),
                    Policy = table.Column<int>(type: "int(11)", nullable: false),
                    AllowNpcSignup = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    AutoFillNpc = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    OutfitProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NpcLoaderProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaEventSides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaEventSides_ArenaEvents",
                        column: x => x.ArenaEventId,
                        principalTable: "ArenaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaEventSides_NpcLoaderProg",
                        column: x => x.NpcLoaderProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArenaEventSides_OutfitProg",
                        column: x => x.OutfitProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaFinanceSnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ArenaEventId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Period = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Revenue = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Costs = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    TaxWithheld = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaFinanceSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaFinanceSnapshots_ArenaEvents",
                        column: x => x.ArenaEventId,
                        principalTable: "ArenaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArenaFinanceSnapshots_Arenas",
                        column: x => x.ArenaId,
                        principalTable: "Arenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaReservations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SideIndex = table.Column<int>(type: "int(11)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ReservedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaReservations_ArenaEvents",
                        column: x => x.ArenaEventId,
                        principalTable: "ArenaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaReservations_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArenaReservations_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaEventTypeSideAllowedClasses",
                columns: table => new
                {
                    ArenaEventTypeSideId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ArenaCombatantClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaEventTypeSideAllowedClasses", x => new { x.ArenaEventTypeSideId, x.ArenaCombatantClassId });
                    table.ForeignKey(
                        name: "FK_ArenaEventTypeSideAllowedClasses_Classes",
                        column: x => x.ArenaCombatantClassId,
                        principalTable: "ArenaCombatantClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaEventTypeSideAllowedClasses_Sides",
                        column: x => x.ArenaEventTypeSideId,
                        principalTable: "ArenaEventTypeSides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaSignups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CombatantClassId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SideIndex = table.Column<int>(type: "int(11)", nullable: false),
                    IsNpc = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    StageName = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    SignatureColour = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    StartingRating = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    SignedUpAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ArenaReservationId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaSignups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaSignups_ArenaEvents",
                        column: x => x.ArenaEventId,
                        principalTable: "ArenaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaSignups_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaSignups_CombatantClasses",
                        column: x => x.CombatantClassId,
                        principalTable: "ArenaCombatantClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaSignups_Reservations",
                        column: x => x.ArenaReservationId,
                        principalTable: "ArenaReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArenaEliminations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArenaEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ArenaSignupId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Reason = table.Column<int>(type: "int(11)", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaEliminations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaEliminations_ArenaEvents",
                        column: x => x.ArenaEventId,
                        principalTable: "ArenaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaEliminations_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArenaEliminations_Signups",
                        column: x => x.ArenaSignupId,
                        principalTable: "ArenaSignups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaBetPayouts_ArenaEvents",
                table: "ArenaBetPayouts",
                column: "ArenaEventId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaBetPayouts_Characters",
                table: "ArenaBetPayouts",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaBetPools_ArenaEvents",
                table: "ArenaBetPools",
                column: "ArenaEventId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaBets_ArenaEvents",
                table: "ArenaBets",
                column: "ArenaEventId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaBets_Characters",
                table: "ArenaBets",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaCells_Arenas",
                table: "ArenaCells",
                column: "ArenaId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaCells_Cells",
                table: "ArenaCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaCombatantClasses_AdminNpcLoaderProg",
                table: "ArenaCombatantClasses",
                column: "AdminNpcLoaderProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaCombatantClasses_Arenas",
                table: "ArenaCombatantClasses",
                column: "ArenaId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaCombatantClasses_EligibilityProg",
                table: "ArenaCombatantClasses",
                column: "EligibilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEliminations_ArenaEvents",
                table: "ArenaEliminations",
                column: "ArenaEventId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEliminations_Characters",
                table: "ArenaEliminations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEliminations_Signups",
                table: "ArenaEliminations",
                column: "ArenaSignupId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEvents_Arenas",
                table: "ArenaEvents",
                column: "ArenaId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEvents_EventTypes",
                table: "ArenaEvents",
                column: "ArenaEventTypeId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventSides_ArenaEvents",
                table: "ArenaEventSides",
                column: "ArenaEventId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventSides_NpcLoaderProg",
                table: "ArenaEventSides",
                column: "NpcLoaderProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventSides_OutfitProg",
                table: "ArenaEventSides",
                column: "OutfitProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventTypes_Arenas",
                table: "ArenaEventTypes",
                column: "ArenaId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventTypes_IntroProg",
                table: "ArenaEventTypes",
                column: "IntroProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventTypes_ResolutionProg",
                table: "ArenaEventTypes",
                column: "ResolutionOverrideProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventTypes_ScoringProg",
                table: "ArenaEventTypes",
                column: "ScoringProgId");

            migrationBuilder.CreateIndex(
                name: "IX_ArenaEventTypeSideAllowedClasses_ArenaCombatantClassId",
                table: "ArenaEventTypeSideAllowedClasses",
                column: "ArenaCombatantClassId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventTypeSides_EventTypes",
                table: "ArenaEventTypeSides",
                column: "ArenaEventTypeId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventTypeSides_NpcLoaderProg",
                table: "ArenaEventTypeSides",
                column: "NpcLoaderProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaEventTypeSides_OutfitProg",
                table: "ArenaEventTypeSides",
                column: "OutfitProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaFinanceSnapshots_ArenaEvents",
                table: "ArenaFinanceSnapshots",
                column: "ArenaEventId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaFinanceSnapshots_Arenas",
                table: "ArenaFinanceSnapshots",
                column: "ArenaId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaManagers_Arenas",
                table: "ArenaManagers",
                column: "ArenaId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaManagers_Characters",
                table: "ArenaManagers",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaRatings_Arenas",
                table: "ArenaRatings",
                column: "ArenaId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaRatings_Characters",
                table: "ArenaRatings",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaRatings_CombatantClasses",
                table: "ArenaRatings",
                column: "CombatantClassId");

            migrationBuilder.CreateIndex(
                name: "UX_ArenaRatings_UniqueParticipant",
                table: "ArenaRatings",
                columns: new[] { "ArenaId", "CharacterId", "CombatantClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_ArenaReservations_ArenaEvents",
                table: "ArenaReservations",
                column: "ArenaEventId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaReservations_Characters",
                table: "ArenaReservations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaReservations_Clans",
                table: "ArenaReservations",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "FK_Arenas_BankAccounts",
                table: "Arenas",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_Arenas_Currencies",
                table: "Arenas",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_Arenas_EconomicZones",
                table: "Arenas",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaSignups_ArenaEvents",
                table: "ArenaSignups",
                column: "ArenaEventId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaSignups_Characters",
                table: "ArenaSignups",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaSignups_CombatantClasses",
                table: "ArenaSignups",
                column: "CombatantClassId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaSignups_Reservations",
                table: "ArenaSignups",
                column: "ArenaReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArenaBetPayouts");

            migrationBuilder.DropTable(
                name: "ArenaBetPools");

            migrationBuilder.DropTable(
                name: "ArenaBets");

            migrationBuilder.DropTable(
                name: "ArenaCells");

            migrationBuilder.DropTable(
                name: "ArenaEliminations");

            migrationBuilder.DropTable(
                name: "ArenaEventSides");

            migrationBuilder.DropTable(
                name: "ArenaEventTypeSideAllowedClasses");

            migrationBuilder.DropTable(
                name: "ArenaFinanceSnapshots");

            migrationBuilder.DropTable(
                name: "ArenaManagers");

            migrationBuilder.DropTable(
                name: "ArenaRatings");

            migrationBuilder.DropTable(
                name: "ArenaSignups");

            migrationBuilder.DropTable(
                name: "ArenaEventTypeSides");

            migrationBuilder.DropTable(
                name: "ArenaCombatantClasses");

            migrationBuilder.DropTable(
                name: "ArenaReservations");

            migrationBuilder.DropTable(
                name: "ArenaEvents");

            migrationBuilder.DropTable(
                name: "ArenaEventTypes");

            migrationBuilder.DropTable(
                name: "Arenas");
        }
    }
}
