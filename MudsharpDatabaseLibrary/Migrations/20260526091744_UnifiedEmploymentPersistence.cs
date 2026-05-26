using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class UnifiedEmploymentPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuilderNotes",
                table: "NPCTemplates",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "UniqueName",
                table: "NPCTemplates",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "EmploymentHostStates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HostType = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    HostId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BoardId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentHostStates_Boards",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    LicenseStatus = table.Column<int>(type: "int(11)", nullable: false),
                    CanRentProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    LostPropertyRetention = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    OutstandingTaxes = table.Column<decimal>(type: "decimal(58,29)", nullable: false),
                    HotelDefinition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_BankAccounts",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Hotels_FutureProgs",
                        column: x => x.CanRentProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Hotels_Properties",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentActionPlans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentActionPlans_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentContracts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RuntimeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Role = table.Column<int>(type: "int(11)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    Authority = table.Column<long>(type: "bigint(20)", nullable: false),
                    FixedRateCurrencyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    FixedRateAmount = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    MarketBindingType = table.Column<int>(type: "int(11)", nullable: false),
                    MarketBindingValue = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    PayCadence = table.Column<int>(type: "int(11)", nullable: false),
                    MinimumEffectivePayCurrencyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MinimumEffectivePayAmount = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    EmployerPaymentSource = table.Column<int>(type: "int(11)", nullable: false),
                    ScheduleDescription = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ScheduleStartTicks = table.Column<long>(type: "bigint(20)", nullable: true),
                    ScheduleEndTicks = table.Column<long>(type: "bigint(20)", nullable: true),
                    DurationType = table.Column<int>(type: "int(11)", nullable: false),
                    DurationTicks = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymentMethodKind = table.Column<int>(type: "int(11)", nullable: false),
                    PaymentBankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymentItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymentItemType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PaymentNotes = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    StartedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndReason = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentContracts_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentJobOpenings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RuntimeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Role = table.Column<int>(type: "int(11)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    MaxPositions = table.Column<int>(type: "int(11)", nullable: false),
                    NpcApplicationsOnly = table.Column<ulong>(type: "bit(1)", nullable: false),
                    Authority = table.Column<long>(type: "bigint(20)", nullable: false),
                    FixedRateCurrencyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    FixedRateAmount = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    MarketBindingType = table.Column<int>(type: "int(11)", nullable: false),
                    MarketBindingValue = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    PayCadence = table.Column<int>(type: "int(11)", nullable: false),
                    MinimumEffectivePayCurrencyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MinimumEffectivePayAmount = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    EmployerPaymentSource = table.Column<int>(type: "int(11)", nullable: false),
                    ScheduleDescription = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ScheduleStartTicks = table.Column<long>(type: "bigint(20)", nullable: true),
                    ScheduleEndTicks = table.Column<long>(type: "bigint(20)", nullable: true),
                    DurationType = table.Column<int>(type: "int(11)", nullable: false),
                    DurationTicks = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymentMethodKind = table.Column<int>(type: "int(11)", nullable: false),
                    PaymentBankAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymentItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymentItemType = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PaymentNotes = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentJobOpenings_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CorrelationId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EntryType = table.Column<int>(type: "int(11)", nullable: false),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AmountCurrencyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RecordedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentLedgerEntries_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentRegisterEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CorrelationId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EntryType = table.Column<int>(type: "int(11)", nullable: false),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RecordedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentRegisterEntries_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentActionSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmploymentActionPlanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SortOrder = table.Column<int>(type: "int(11)", nullable: false),
                    StepType = table.Column<int>(type: "int(11)", nullable: false),
                    RequiredAuthority = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequiredCapabilities = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RequiresPaymentAuthorisation = table.Column<ulong>(type: "bit(1)", nullable: false),
                    IsFinancialStep = table.Column<ulong>(type: "bit(1)", nullable: false),
                    Description = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AmountCurrencyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    ExistingFinancialRecord = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DestinationCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ExecutionCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CommandName = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CommandArguments = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AccountName = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    BoardTitle = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    BoardText = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentActionSteps_Plans",
                        column: x => x.EmploymentActionPlanId,
                        principalTable: "EmploymentActionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentActiveTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentActionPlanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    AssignedEmployeeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BlockedReason = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CorrelationId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IdempotencyKey = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentActiveTasks_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentActiveTasks_Plans",
                        column: x => x.EmploymentActionPlanId,
                        principalTable: "EmploymentActionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentManagerGoals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RuntimeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GoalType = table.Column<int>(type: "int(11)", nullable: false),
                    RequiredAuthority = table.Column<long>(type: "bigint(20)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    ConfigurationDescription = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentActionPlanId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Priority = table.Column<int>(type: "int(11)", nullable: false),
                    EvaluationCadenceTicks = table.Column<long>(type: "bigint(20)", nullable: false),
                    LastEvaluatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastEvaluationResult = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CorrelationId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentManagerGoals_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentManagerGoals_Plans",
                        column: x => x.EmploymentActionPlanId,
                        principalTable: "EmploymentActionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentScheduledTaskRules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentHostStateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IdempotencyKey = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmploymentActionPlanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CooldownTicks = table.Column<long>(type: "bigint(20)", nullable: false),
                    LastSpawnedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentScheduledTaskRules_HostStates",
                        column: x => x.EmploymentHostStateId,
                        principalTable: "EmploymentHostStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentScheduledTaskRules_Plans",
                        column: x => x.EmploymentActionPlanId,
                        principalTable: "EmploymentActionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentApplications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RuntimeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EmploymentJobOpeningId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CandidateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    DecisionReason = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentApplications_Openings",
                        column: x => x.EmploymentJobOpeningId,
                        principalTable: "EmploymentJobOpenings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentJobOpeningRequirements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmploymentJobOpeningId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequirementType = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    NumericValue = table.Column<double>(type: "double", nullable: true),
                    Capability = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentJobOpeningRequirements_Openings",
                        column: x => x.EmploymentJobOpeningId,
                        principalTable: "EmploymentJobOpenings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentActiveTaskStepStates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmploymentActiveTaskId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SortOrder = table.Column<int>(type: "int(11)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentActiveTaskStepStates_Tasks",
                        column: x => x.EmploymentActiveTaskId,
                        principalTable: "EmploymentActiveTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmploymentTaskConditions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScheduledTaskRuleId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ManagerGoalId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SortOrder = table.Column<int>(type: "int(11)", nullable: false),
                    ConditionType = table.Column<int>(type: "int(11)", nullable: false),
                    Key = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ThresholdInt = table.Column<int>(type: "int(11)", nullable: true),
                    ThresholdDecimal = table.Column<decimal>(type: "decimal(58,29)", nullable: true),
                    BoolValue = table.Column<ulong>(type: "bit(1)", nullable: true),
                    EarliestTicks = table.Column<long>(type: "bigint(20)", nullable: true),
                    LatestTicks = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentTaskConditions_ManagerGoals",
                        column: x => x.ManagerGoalId,
                        principalTable: "EmploymentManagerGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentTaskConditions_ScheduledRules",
                        column: x => x.ScheduledTaskRuleId,
                        principalTable: "EmploymentScheduledTaskRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_NPCTemplates_UniqueName",
                table: "NPCTemplates",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentActionPlans_HostStates_idx",
                table: "EmploymentActionPlans",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentActionSteps_Plans_idx",
                table: "EmploymentActionSteps",
                column: "EmploymentActionPlanId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentActiveTasks_HostStates_idx",
                table: "EmploymentActiveTasks",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentActiveTasks_Plans_idx",
                table: "EmploymentActiveTasks",
                column: "EmploymentActionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentActiveTasks_PublicId",
                table: "EmploymentActiveTasks",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentActiveTaskStepStates_Tasks_idx",
                table: "EmploymentActiveTaskStepStates",
                column: "EmploymentActiveTaskId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentApplications_Openings_idx",
                table: "EmploymentApplications",
                column: "EmploymentJobOpeningId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentApplications_Candidate",
                table: "EmploymentApplications",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentApplications_Opening_Runtime",
                table: "EmploymentApplications",
                columns: new[] { "EmploymentJobOpeningId", "RuntimeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentContracts_HostStates_idx",
                table: "EmploymentContracts",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContracts_Employee",
                table: "EmploymentContracts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContracts_Host_Runtime",
                table: "EmploymentContracts",
                columns: new[] { "EmploymentHostStateId", "RuntimeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentHostStates_Boards_idx",
                table: "EmploymentHostStates",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentHostStates_Host",
                table: "EmploymentHostStates",
                columns: new[] { "HostType", "HostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentJobOpeningRequirements_Openings_idx",
                table: "EmploymentJobOpeningRequirements",
                column: "EmploymentJobOpeningId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentJobOpenings_HostStates_idx",
                table: "EmploymentJobOpenings",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentJobOpenings_Host_Runtime",
                table: "EmploymentJobOpenings",
                columns: new[] { "EmploymentHostStateId", "RuntimeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentLedgerEntries_HostStates_idx",
                table: "EmploymentLedgerEntries",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentLedgerEntries_Correlation",
                table: "EmploymentLedgerEntries",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentManagerGoals_HostStates_idx",
                table: "EmploymentManagerGoals",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentManagerGoals_Plans_idx",
                table: "EmploymentManagerGoals",
                column: "EmploymentActionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentManagerGoals_Host_Runtime",
                table: "EmploymentManagerGoals",
                columns: new[] { "EmploymentHostStateId", "RuntimeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentRegisterEntries_HostStates_idx",
                table: "EmploymentRegisterEntries",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRegisterEntries_Correlation",
                table: "EmploymentRegisterEntries",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentScheduledTaskRules_HostStates_idx",
                table: "EmploymentScheduledTaskRules",
                column: "EmploymentHostStateId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentScheduledTaskRules_Plans_idx",
                table: "EmploymentScheduledTaskRules",
                column: "EmploymentActionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentScheduledTaskRules_PublicId",
                table: "EmploymentScheduledTaskRules",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentTaskConditions_ManagerGoals_idx",
                table: "EmploymentTaskConditions",
                column: "ManagerGoalId");

            migrationBuilder.CreateIndex(
                name: "FK_EmploymentTaskConditions_ScheduledRules_idx",
                table: "EmploymentTaskConditions",
                column: "ScheduledTaskRuleId");

            migrationBuilder.CreateIndex(
                name: "FK_Hotels_BankAccounts_idx",
                table: "Hotels",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_Hotels_FutureProgs_idx",
                table: "Hotels",
                column: "CanRentProgId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_PropertyId",
                table: "Hotels",
                column: "PropertyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmploymentActionSteps");

            migrationBuilder.DropTable(
                name: "EmploymentActiveTaskStepStates");

            migrationBuilder.DropTable(
                name: "EmploymentApplications");

            migrationBuilder.DropTable(
                name: "EmploymentContracts");

            migrationBuilder.DropTable(
                name: "EmploymentJobOpeningRequirements");

            migrationBuilder.DropTable(
                name: "EmploymentLedgerEntries");

            migrationBuilder.DropTable(
                name: "EmploymentRegisterEntries");

            migrationBuilder.DropTable(
                name: "EmploymentTaskConditions");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "EmploymentActiveTasks");

            migrationBuilder.DropTable(
                name: "EmploymentJobOpenings");

            migrationBuilder.DropTable(
                name: "EmploymentManagerGoals");

            migrationBuilder.DropTable(
                name: "EmploymentScheduledTaskRules");

            migrationBuilder.DropTable(
                name: "EmploymentActionPlans");

            migrationBuilder.DropTable(
                name: "EmploymentHostStates");

            migrationBuilder.DropIndex(
                name: "IX_NPCTemplates_UniqueName",
                table: "NPCTemplates");

            migrationBuilder.DropColumn(
                name: "BuilderNotes",
                table: "NPCTemplates");

            migrationBuilder.DropColumn(
                name: "UniqueName",
                table: "NPCTemplates");
        }
    }
}
