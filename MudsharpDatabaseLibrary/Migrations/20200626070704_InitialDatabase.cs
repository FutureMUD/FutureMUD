using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmmunitionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    SpecificType = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    RangedWeaponTypes = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    BaseAccuracy = table.Column<double>(nullable: false),
                    Loudness = table.Column<int>(type: "int(11)", nullable: false),
                    BreakChanceOnHit = table.Column<double>(nullable: false),
                    BreakChanceOnMiss = table.Column<double>(nullable: false),
                    BaseBlockDifficulty = table.Column<int>(type: "int(11)", nullable: false),
                    BaseDodgeDifficulty = table.Column<int>(type: "int(11)", nullable: false),
                    DamageExpression = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    StunExpression = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    PainExpression = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    DamageType = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmmunitionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArmourTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    MinimumPenetrationDegree = table.Column<int>(type: "int(11)", nullable: false),
                    BaseDifficultyDegrees = table.Column<int>(type: "int(11)", nullable: false),
                    StackedDifficultyDegrees = table.Column<int>(type: "int(11)", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArmourTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtificialIntelligences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtificialIntelligences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorityGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AuthorityLevel = table.Column<int>(type: "int(11)", nullable: false),
                    InformationLevel = table.Column<int>(type: "int(11)", nullable: false),
                    AccountsLevel = table.Column<int>(type: "int(11)", nullable: false),
                    CharactersLevel = table.Column<int>(type: "int(11)", nullable: false),
                    CharacterApprovalLevel = table.Column<int>(type: "int(11)", nullable: false),
                    CharacterApprovalRisk = table.Column<int>(type: "int(11)", nullable: false),
                    ItemsLevel = table.Column<int>(type: "int(11)", nullable: false),
                    PlanesLevel = table.Column<int>(type: "int(11)", nullable: false),
                    RoomsLevel = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorityGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutobuilderAreaTemplates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TemplateType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutobuilderAreaTemplates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AutobuilderRoomTemplates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TemplateType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutobuilderRoomTemplates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BloodModels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BloodtypeAntigens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodtypeAntigens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bloodtypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bloodtypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShowOnLogin = table.Column<bool>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BodypartGroupDescribers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DescribedAs = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Comment = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodypartGroupDescribers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BodypartShape",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodypartShape", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "bodypartshapecountview",
                columns: table => new
                {
                    BodypartGroupDescriptionRuleId = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    DescribedAs = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    MinCount = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    MaxCount = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    TargetId = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    Name = table.Column<sbyte>(type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Date = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FeedClockId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Celestials",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Minutes = table.Column<int>(type: "int(11)", nullable: false),
                    FeedClockId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CelestialYear = table.Column<int>(type: "int(11)", nullable: false),
                    LastYearBump = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Celestials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    Pattern = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ParentId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ChargenDisplayType = table.Column<int>(type: "int(11)", nullable: true),
                    Model = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'standard'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacteristicDefinitions_Parent",
                        column: x => x.ParentId,
                        principalTable: "CharacteristicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChargenResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PluralName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Alias = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MinimumTimeBetweenAwards = table.Column<int>(type: "int(11)", nullable: false),
                    MaximumNumberAwardedPerAward = table.Column<int>(type: "int(11)", nullable: false),
                    PermissionLevelRequiredToAward = table.Column<int>(type: "int(11)", nullable: false),
                    PermissionLevelRequiredToCircumventMinimumTime = table.Column<int>(type: "int(11)", nullable: false),
                    ShowToPlayerInScore = table.Column<bool>(type: "bit(1)", nullable: false),
                    TextDisplayedToPlayerOnAward = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TextDisplayedToPlayerOnDeduct = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MaximumResourceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MaximumResourceFormula = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargenResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CheckMethod = table.Column<string>(type: "varchar(25)", nullable: false, defaultValueSql: "'Standard'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ImproveTraits = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    FailIfTraitMissingMode = table.Column<short>(type: "smallint(6)", nullable: false),
                    CanBranchIfTraitMissing = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClimateModels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MinuteProcessingInterval = table.Column<int>(type: "int(11)", nullable: false),
                    MinimumMinutesBetweenFlavourEchoes = table.Column<int>(type: "int(11)", nullable: false),
                    MinuteFlavourEchoChance = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClimateModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clocks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Seconds = table.Column<int>(type: "int(11)", nullable: false),
                    Minutes = table.Column<int>(type: "int(11)", nullable: false),
                    Hours = table.Column<int>(type: "int(11)", nullable: false),
                    PrimaryTimezoneId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Basic = table.Column<int>(type: "int(11)", nullable: false),
                    Red = table.Column<int>(type: "int(11)", nullable: false),
                    Green = table.Column<int>(type: "int(11)", nullable: false),
                    Blue = table.Column<int>(type: "int(11)", nullable: false),
                    Fancy = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CorpseModels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Definition = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorpseModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CultureInfos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DisplayName = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Order = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CultureInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DamagePatterns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DamageType = table.Column<int>(type: "int(11)", nullable: false),
                    Dice = table.Column<int>(type: "int(11)", nullable: false),
                    Sides = table.Column<int>(type: "int(11)", nullable: false),
                    Bonus = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamagePatterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drugs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    DrugTypes = table.Column<int>(type: "int(11)", nullable: false),
                    DrugVectors = table.Column<int>(type: "int(11)", nullable: false),
                    IntensityPerGram = table.Column<double>(nullable: false),
                    RelativeMetabolisationRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drugs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EditableItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    RevisionStatus = table.Column<int>(type: "int(11)", nullable: false),
                    BuilderAccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReviewerAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BuilderComment = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ReviewerComment = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BuilderDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ReviewerDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ObsoleteDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditableItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    TemplateType = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Subject = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ReturnAddress = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.TemplateType);
                });

            migrationBuilder.CreateTable(
                name: "EntityDescriptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShortDescription = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FullDescription = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DisplaySex = table.Column<short>(type: "smallint(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityDescriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Keywords1 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Keywords2 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CellId1 = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId2 = table.Column<long>(type: "bigint(20)", nullable: false),
                    DoorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Direction1 = table.Column<int>(type: "int(11)", nullable: false),
                    Direction2 = table.Column<int>(type: "int(11)", nullable: false),
                    TimeMultiplier = table.Column<double>(nullable: false),
                    InboundDescription1 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    InboundDescription2 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OutboundDescription1 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OutboundDescription2 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    InboundTarget1 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    InboundTarget2 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OutboundTarget1 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OutboundTarget2 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Verb1 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Verb2 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PrimaryKeyword1 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PrimaryKeyword2 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AcceptsDoor = table.Column<bool>(type: "bit(1)", nullable: false),
                    DoorSize = table.Column<int>(type: "int(11)", nullable: true),
                    MaximumSizeToEnter = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'12'"),
                    MaximumSizeToEnterUpright = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'12'"),
                    FallCell = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsClimbExit = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    ClimbDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    BlockedLayers = table.Column<string>(type: "varchar(255)", nullable: false, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FutureProgs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FunctionName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FunctionComment = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FunctionText = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci"),
                    ReturnType = table.Column<long>(type: "bigint(20)", nullable: false),
                    Category = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Subcategory = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Public = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    AcceptsAnyParameters = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    StaticType = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureProgs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gameitemeditingview",
                columns: table => new
                {
                    Id = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    Name = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    MaterialId = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    ProtoMaterial = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    Quality = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    Size = table.Column<sbyte>(type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "GameItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quality = table.Column<int>(type: "int(11)", nullable: false),
                    GameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    RoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    Condition = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    MaterialId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Size = table.Column<int>(type: "int(11)", nullable: false),
                    ContainerId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PositionId = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    PositionModifier = table.Column<int>(type: "int(11)", nullable: false),
                    PositionTargetId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PositionTargetType = table.Column<string>(type: "varchar(45)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PositionEmote = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MorphTimeRemaining = table.Column<int>(type: "int(11)", nullable: true),
                    EffectData = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameItems_GameItems_Containers",
                        column: x => x.ContainerId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Grids",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GridType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grids", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupAITemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAITemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthStrategies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci"),
                    Type = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthStrategies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HearingProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    SurveyDescription = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HearingProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeightWeightModels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MeanHeight = table.Column<double>(nullable: false),
                    MeanBMI = table.Column<double>(nullable: false),
                    StddevHeight = table.Column<double>(nullable: false),
                    StddevBMI = table.Column<double>(nullable: false),
                    BMIMultiplier = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeightWeightModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hooks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci"),
                    Type = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Category = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TargetEventType = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hooks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Improvers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Improvers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci"),
                    Keywords = table.Column<string>(type: "varchar(1000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageDifficultyModels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Definition = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageDifficultyModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Style = table.Column<int>(type: "int(11)", nullable: false),
                    Strength = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MagicGenerators",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicGenerators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MagicResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MagicResourceType = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MagicSchools",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ParentSchoolId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SchoolVerb = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    SchoolAdjective = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PowerListColour = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicSchools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicSchools_MagicSchools",
                        column: x => x.ParentSchoolId,
                        principalTable: "MagicSchools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MaterialDescription = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Density = table.Column<double>(nullable: false),
                    Organic = table.Column<bool>(type: "bit(1)", nullable: false),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    BehaviourType = table.Column<int>(type: "int(11)", nullable: true),
                    ThermalConductivity = table.Column<double>(nullable: false),
                    ElectricalConductivity = table.Column<double>(nullable: false),
                    SpecificHeatCapacity = table.Column<double>(nullable: false),
                    SolidFormId = table.Column<long>(type: "bigint(20)", nullable: true),
                    LiquidFormId = table.Column<long>(type: "bigint(20)", nullable: true),
                    GasFormId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Viscosity = table.Column<double>(nullable: true),
                    MeltingPoint = table.Column<double>(nullable: true),
                    BoilingPoint = table.Column<double>(nullable: true),
                    IgnitionPoint = table.Column<double>(nullable: true),
                    HeatDamagePoint = table.Column<double>(nullable: true),
                    ImpactFracture = table.Column<double>(nullable: true),
                    ImpactYield = table.Column<double>(nullable: true),
                    ImpactStrainAtYield = table.Column<double>(nullable: true),
                    ShearFracture = table.Column<double>(nullable: true),
                    ShearYield = table.Column<double>(nullable: true),
                    ShearStrainAtYield = table.Column<double>(nullable: true),
                    YoungsModulus = table.Column<double>(nullable: true),
                    SolventId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SolventVolumeRatio = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    ResidueSdesc = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ResidueDesc = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ResidueColour = table.Column<string>(type: "varchar(45)", nullable: true, defaultValueSql: "'white'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Absorbency = table.Column<double>(nullable: false, defaultValueSql: "'0.25'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Merits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    MeritType = table.Column<int>(type: "int(11)", nullable: false),
                    MeritScope = table.Column<int>(type: "int(11)", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ParentId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Merits_Merits",
                        column: x => x.ParentId,
                        principalTable: "Merits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "NameCulture",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameCulture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonCardinalExitTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OriginOutboundPreface = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OriginInboundPreface = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DestinationOutboundPreface = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DestinationInboundPreface = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OutboundVerb = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    InboundVerb = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonCardinalExitTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PopulationBloodModels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PopulationBloodModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RangedCovers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    CoverType = table.Column<int>(type: "int(11)", nullable: false),
                    CoverExtent = table.Column<int>(type: "int(11)", nullable: false),
                    HighestPositionState = table.Column<int>(type: "int(11)", nullable: false),
                    DescriptionString = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ActionDescriptionString = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    MaximumSimultaneousCovers = table.Column<int>(type: "int(11)", nullable: false),
                    CoverStaysWhileMoving = table.Column<bool>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangedCovers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionalClimates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ClimateModelId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalClimates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkyDescriptionTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkyDescriptionTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StackDecorators",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "varchar(10000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(1000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StackDecorators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticConfigurations",
                columns: table => new
                {
                    SettingName = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SettingName);
                });

            migrationBuilder.CreateTable(
                name: "StaticStrings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Text = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticStrings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeZoneInfos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Display = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Order = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZoneInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TraitDecorators",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Contents = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraitDecorators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TraitExpression",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Expression = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, defaultValueSql: "'Unnamed Expression'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraitExpression", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasure",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PrimaryAbbreviation = table.Column<string>(type: "varchar(45)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Abbreviations = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BaseMultiplier = table.Column<double>(nullable: false),
                    PreMultiplierBaseOffset = table.Column<double>(nullable: false),
                    PostMultiplierBaseOffset = table.Column<double>(nullable: false),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    Describer = table.Column<bool>(type: "bit(1)", nullable: false),
                    SpaceBetween = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    System = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DefaultUnitForSystem = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VariableDefaults",
                columns: table => new
                {
                    OwnerType = table.Column<long>(type: "bigint(20)", nullable: false),
                    Property = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DefaultValue = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.OwnerType, x.Property });
                });

            migrationBuilder.CreateTable(
                name: "VariableDefinitions",
                columns: table => new
                {
                    OwnerType = table.Column<long>(type: "bigint(20)", nullable: false),
                    Property = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ContainedType = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.OwnerType, x.Property });
                });

            migrationBuilder.CreateTable(
                name: "VariableValues",
                columns: table => new
                {
                    ReferenceType = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReferenceProperty = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ValueDefinition = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ValueType = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ReferenceType, x.ReferenceId, x.ReferenceProperty });
                });

            migrationBuilder.CreateTable(
                name: "WearableSizes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OneSizeFitsAll = table.Column<bool>(type: "bit(1)", nullable: false),
                    Height = table.Column<double>(nullable: true),
                    Weight = table.Column<double>(nullable: true),
                    TraitValue = table.Column<double>(nullable: true),
                    BodyPrototypeId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WearableSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WearProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BodyPrototypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WearStringInventory = table.Column<string>(type: "varchar(255)", nullable: false, defaultValueSql: "'worn on'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WearAction1st = table.Column<string>(type: "varchar(255)", nullable: false, defaultValueSql: "'put'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WearAction3rd = table.Column<string>(type: "varchar(255)", nullable: false, defaultValueSql: "'puts'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WearAffix = table.Column<string>(type: "varchar(255)", nullable: false, defaultValueSql: "'on'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WearlocProfiles = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false, defaultValueSql: "'Direct'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RequireContainerIsEmpty = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WearProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WeatherEventType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WeatherDescription = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WeatherRoomAddendum = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TemperatureEffect = table.Column<double>(nullable: false),
                    Precipitation = table.Column<int>(type: "int(11)", nullable: false),
                    Wind = table.Column<int>(type: "int(11)", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PrecipitationTemperatureEffect = table.Column<double>(nullable: false),
                    WindTemperatureEffect = table.Column<double>(nullable: false),
                    LightLevelMultiplier = table.Column<double>(nullable: false),
                    ObscuresViewOfSky = table.Column<bool>(type: "bit(1)", nullable: false),
                    PermittedAtNight = table.Column<bool>(type: "bit(1)", nullable: false),
                    PermittedAtDawn = table.Column<bool>(type: "bit(1)", nullable: false),
                    PermittedAtMorning = table.Column<bool>(type: "bit(1)", nullable: false),
                    PermittedAtAfternoon = table.Column<bool>(type: "bit(1)", nullable: false),
                    PermittedAtDusk = table.Column<bool>(type: "bit(1)", nullable: false),
                    CountsAsId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherEvents_WeatherEvents",
                        column: x => x.CountsAsId,
                        principalTable: "WeatherEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Password = table.Column<string>(type: "varchar(1000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Salt = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccessStatus = table.Column<int>(type: "int(11)", nullable: false),
                    AuthorityGroupId = table.Column<long>(type: "bigint(20)", nullable: true, defaultValueSql: "'0'"),
                    Email = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LastLoginTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastLoginIP = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FormatLength = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'110'"),
                    InnerFormatLength = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'80'"),
                    UseMXP = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    UseMSP = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    UseMCCP = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    ActiveCharactersAllowed = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    UseUnicode = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    TimeZoneId = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CultureName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RegistrationCode = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    IsRegistered = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    RecoveryCode = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    UnitPreference = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PageLength = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'22'"),
                    PromptType = table.Column<int>(type: "int(11)", nullable: false),
                    TabRoomDescriptions = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    CodedRoomDescriptionAdditionsOnNewLine = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    CharacterNameOverlaySetting = table.Column<int>(type: "int(11)", nullable: false),
                    AppendNewlinesBetweenMultipleEchoesPerPrompt = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AuthorityGroups",
                        column: x => x.AuthorityGroupId,
                        principalTable: "AuthorityGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BloodModels_Bloodtypes",
                columns: table => new
                {
                    BloodModelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BloodtypeId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BloodModelId, x.BloodtypeId });
                    table.ForeignKey(
                        name: "FK_BloodModels_Bloodtypes_BloodModels",
                        column: x => x.BloodModelId,
                        principalTable: "BloodModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BloodModels_Bloodtypes_Bloodtypes",
                        column: x => x.BloodtypeId,
                        principalTable: "Bloodtypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bloodtypes_BloodtypeAntigens",
                columns: table => new
                {
                    BloodtypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BloodtypeAntigenId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BloodtypeId, x.BloodtypeAntigenId });
                    table.ForeignKey(
                        name: "FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens",
                        column: x => x.BloodtypeAntigenId,
                        principalTable: "BloodtypeAntigens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bloodtypes_BloodtypeAntigens_Bloodtypes",
                        column: x => x.BloodtypeId,
                        principalTable: "Bloodtypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BodypartGroupDescribers_ShapeCount",
                columns: table => new
                {
                    BodypartGroupDescriptionRuleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MinCount = table.Column<int>(type: "int(11)", nullable: false),
                    MaxCount = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodypartGroupDescriptionRuleId, x.TargetId });
                    table.ForeignKey(
                        name: "FK_BGD_ShapeCount_BodypartGroupDescribers",
                        column: x => x.BodypartGroupDescriptionRuleId,
                        principalTable: "BodypartGroupDescribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BGD_ShapeCount_BodypartShape",
                        column: x => x.TargetId,
                        principalTable: "BodypartShape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CelestialDayOnset = table.Column<int>(type: "int(11)", nullable: false),
                    CelestialId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seasons_Celestials",
                        column: x => x.CelestialId,
                        principalTable: "Celestials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TargetDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacteristicProfiles_CharacteristicDefinitions",
                        column: x => x.TargetDefinitionId,
                        principalTable: "CharacteristicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckTemplateDifficulties",
                columns: table => new
                {
                    CheckTemplateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Difficulty = table.Column<int>(type: "int(11)", nullable: false),
                    Modifier = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Difficulty, x.CheckTemplateId });
                    table.ForeignKey(
                        name: "FK_CheckTemplateDifficulties_CheckTemplates",
                        column: x => x.CheckTemplateId,
                        principalTable: "CheckTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Timezones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OffsetMinutes = table.Column<int>(type: "int(11)", nullable: false),
                    OffsetHours = table.Column<int>(type: "int(11)", nullable: false),
                    ClockId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timezones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timezones_Clocks",
                        column: x => x.ClockId,
                        principalTable: "Clocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShortDescription = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FullDescription = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Value = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    GeneralForm = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PluralWord = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coins_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyDivisions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BaseUnitConversionRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyDivisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyDivisions_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalAuthorities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalAuthorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalAuthorities_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrugsIntensities",
                columns: table => new
                {
                    DrugId = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DrugType = table.Column<int>(type: "int(11)", nullable: false),
                    RelativeIntensity = table.Column<double>(nullable: false),
                    AdditionalEffects = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.DrugId, x.DrugType });
                    table.ForeignKey(
                        name: "FK_Drugs_DrugIntensities",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CellOverlayPackages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_CellOverlayPackages_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisfigurementTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ShortDescription = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FullDescription = table.Column<string>(type: "varchar(5000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_DisfigurementTemplates_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForagableProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_ForagableProfiles_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Foragables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ForagableTypes = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ForageDifficulty = table.Column<int>(type: "int(11)", nullable: false),
                    RelativeChance = table.Column<int>(type: "int(11)", nullable: false),
                    MinimumOutcome = table.Column<int>(type: "int(11)", nullable: false),
                    MaximumOutcome = table.Column<int>(type: "int(11)", nullable: false),
                    QuantityDiceExpression = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OnForageProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanForageProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_Foragables_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameItemComponentProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_GameItemComponentProtos_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NPCTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    Type = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_NPCTemplates_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_Projects_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChannelName = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ChannelListenerProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChannelSpeakerProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AnnounceChannelJoiners = table.Column<bool>(type: "bit(1)", nullable: false),
                    ChannelColour = table.Column<string>(type: "char(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Mode = table.Column<int>(type: "int(11)", nullable: false),
                    AnnounceMissedListeners = table.Column<bool>(type: "bit(1)", nullable: false),
                    AddToPlayerCommandTree = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    AddToGuideCommandTree = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_FutureProgs_Listener",
                        column: x => x.ChannelListenerProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Channels_FutureProgs_Speaker",
                        column: x => x.ChannelSpeakerProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterIntroTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ResolutionPriority = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    AppliesToCharacterProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Order = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterIntroTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterIntroTemplates_FutureProgs",
                        column: x => x.AppliesToCharacterProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Value = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Default = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    AdditionalValue = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Pluralisation = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacteristicValues_CharacteristicDefinitions",
                        column: x => x.DefinitionId,
                        principalTable: "CharacteristicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacteristicValues_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChargenAdvices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChargenStage = table.Column<int>(type: "int(11)", nullable: false),
                    AdviceTitle = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AdviceText = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShouldShowAdviceProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargenAdvices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_FutureProgs",
                        column: x => x.ShouldShowAdviceProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CombatMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    Outcome = table.Column<int>(type: "int(11)", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Priority = table.Column<int>(type: "int(11)", nullable: false),
                    Verb = table.Column<int>(type: "int(11)", nullable: true),
                    Chance = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    FailureMessage = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatMessages_FutureProgs",
                        column: x => x.ProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyDescriptionPatterns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NegativePrefix = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Order = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyDescriptionPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyDescriptionPatterns_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyDescriptionPatterns_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dreams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    CanDreamProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OnDreamProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OnWakeDuringDreamingProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OnlyOnce = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    Priority = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'100'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dreams_FutureProgs_CanDream",
                        column: x => x.CanDreamProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Dreams_FutureProgs_OnDream",
                        column: x => x.OnDreamProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Dreams_FutureProgs_OnWake",
                        column: x => x.OnWakeDuringDreamingProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EntityDescriptionPatterns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Pattern = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    ApplicabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RelativeWeight = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityDescriptionPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityDescriptionPatterns_FutureProgs",
                        column: x => x.ApplicabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FutureProgs_Parameters",
                columns: table => new
                {
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ParameterIndex = table.Column<int>(type: "int(11)", nullable: false),
                    ParameterType = table.Column<long>(type: "bigint(20)", nullable: false),
                    ParameterName = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.FutureProgId, x.ParameterIndex });
                    table.ForeignKey(
                        name: "FK_FutureProgs_Parameters_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Helpfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Category = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Subcategory = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TagLine = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PublicText = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RuleId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Keywords = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LastEditedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LastEditedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Helpfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Helpfiles_FutureProgs",
                        column: x => x.RuleId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "knowledges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LongDescription = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Subtype = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LearnableType = table.Column<int>(type: "int(11)", nullable: false),
                    LearnDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'7'"),
                    TeachDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'7'"),
                    LearningSessionsRequired = table.Column<int>(type: "int(11)", nullable: false),
                    CanAcquireProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanLearnProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_knowledges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE",
                        column: x => x.CanAcquireProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KNOWLEDGES_FUTUREPROGS_LEARN",
                        column: x => x.CanLearnProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProgSchedules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    IntervalType = table.Column<int>(type: "int(11)", nullable: false),
                    IntervalModifier = table.Column<int>(type: "int(11)", nullable: false),
                    IntervalOther = table.Column<int>(type: "int(11)", nullable: false),
                    ReferenceTime = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ReferenceDate = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgSchedules_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Socials",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    NoTargetEcho = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OneTargetEcho = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DirectionTargetEcho = table.Column<string>(type: "varchar(1000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MultiTargetEcho = table.Column<string>(type: "varchar(1000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Socials_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ParentId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ShouldSeeProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Parent",
                        column: x => x.ParentId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tags_Futureprogs",
                        column: x => x.ShouldSeeProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WitnessProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    IdentityKnownProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReportingMultiplierProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReportingReliability = table.Column<double>(nullable: false),
                    MinimumSkillToDetermineTimeOfDay = table.Column<double>(nullable: false),
                    MinimumSkillToDetermineBiases = table.Column<double>(nullable: false),
                    BaseReportingChanceNight = table.Column<double>(nullable: false),
                    BaseReportingChanceDawn = table.Column<double>(nullable: false),
                    BaseReportingChanceMorning = table.Column<double>(nullable: false),
                    BaseReportingChanceAfternoon = table.Column<double>(nullable: false),
                    BaseReportingChanceDusk = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WitnessProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WitnessProfiles_IdentityProg",
                        column: x => x.IdentityKnownProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WitnessProfiles_MultiplierProg",
                        column: x => x.ReportingMultiplierProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameItemComponents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameItemComponentProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemComponentProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameItemComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameItemComponents_GameItems",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupAIs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    GroupAITemplateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Data = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAIs_GroupAITemplates",
                        column: x => x.GroupAITemplateId,
                        principalTable: "GroupAITemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultHooks",
                columns: table => new
                {
                    HookId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PerceivableType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.HookId, x.PerceivableType, x.FutureProgId });
                    table.ForeignKey(
                        name: "FK_DefaultHooks_Futureprogs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultHooks_Hooks",
                        column: x => x.HookId,
                        principalTable: "Hooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameItemProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Keywords = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MaterialId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Size = table.Column<int>(type: "int(11)", nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    ReadOnly = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    LongDescription = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ItemGroupId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OnDestroyedGameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    HealthStrategyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BaseItemQuality = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    CustomColour = table.Column<string>(type: "varchar(45)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    HighPriority = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    MorphGameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MorphTimeSeconds = table.Column<int>(type: "int(11)", nullable: false),
                    MorphEmote = table.Column<string>(type: "varchar(1000)", nullable: false, defaultValueSql: "'$0 $?1|morphs into $1|decays into nothing$.'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShortDescription = table.Column<string>(type: "varchar(1000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FullDescription = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_GameItemProtos_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameItemProtos_ItemGroups",
                        column: x => x.ItemGroupId,
                        principalTable: "ItemGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ItemGroupForms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemGroupId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGroupForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemGroupForms_ItemGroups",
                        column: x => x.ItemGroupId,
                        principalTable: "ItemGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Style = table.Column<int>(type: "int(11)", nullable: false),
                    IsOpen = table.Column<bool>(type: "bit(1)", nullable: false),
                    LockedWith = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doors_Locks",
                        column: x => x.LockedWith,
                        principalTable: "Locks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameItems_MagicResources",
                columns: table => new
                {
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MagicResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.GameItemId, x.MagicResourceId });
                    table.ForeignKey(
                        name: "FK_GameItems_MagicResources_GameItems",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameItems_MagicResources_MagicResources",
                        column: x => x.MagicResourceId,
                        principalTable: "MagicResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MagicCapabilities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CapabilityModel = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PowerLevel = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MagicSchoolId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicCapabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicCapabilities_MagicSchools",
                        column: x => x.MagicSchoolId,
                        principalTable: "MagicSchools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MagicPowers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Blurb = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShowHelp = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PowerModel = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MagicSchoolId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicPowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicPowers_MagicSchools",
                        column: x => x.MagicSchoolId,
                        principalTable: "MagicSchools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Liquids",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LongDescription = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TasteText = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    VagueTasteText = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    SmellText = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    VagueSmellText = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TasteIntensity = table.Column<double>(nullable: false, defaultValueSql: "'100'"),
                    SmellIntensity = table.Column<double>(nullable: false, defaultValueSql: "'10'"),
                    AlcoholLitresPerLitre = table.Column<double>(nullable: false),
                    WaterLitresPerLitre = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    FoodSatiatedHoursPerLitre = table.Column<double>(nullable: false),
                    DrinkSatiatedHoursPerLitre = table.Column<double>(nullable: false, defaultValueSql: "'12'"),
                    CaloriesPerLitre = table.Column<double>(nullable: false),
                    Viscosity = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    Density = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    Organic = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    ThermalConductivity = table.Column<double>(nullable: false, defaultValueSql: "'0.609'"),
                    ElectricalConductivity = table.Column<double>(nullable: false, defaultValueSql: "'0.005'"),
                    SpecificHeatCapacity = table.Column<double>(nullable: false, defaultValueSql: "'4181'"),
                    IgnitionPoint = table.Column<double>(nullable: true),
                    FreezingPoint = table.Column<double>(nullable: true, defaultValueSql: "'273.15'"),
                    BoilingPoint = table.Column<double>(nullable: true, defaultValueSql: "'373.15'"),
                    DraughtProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SolventId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CountAsId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CountAsQuality = table.Column<int>(type: "int(11)", nullable: false),
                    DisplayColour = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'blue'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DampDescription = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WetDescription = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DrenchedDescription = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DampShortDescription = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WetShortDescription = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DrenchedShortDescription = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    SolventVolumeRatio = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    DriedResidueId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DrugId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DrugGramsPerUnitVolume = table.Column<double>(nullable: false),
                    InjectionConsequence = table.Column<int>(type: "int(11)", nullable: false),
                    ResidueVolumePercentage = table.Column<double>(nullable: false, defaultValueSql: "'0.05'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liquids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Liquids_Liquids_CountasAs",
                        column: x => x.CountAsId,
                        principalTable: "Liquids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Liquids_Materials",
                        column: x => x.DriedResidueId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Liquids_Drugs",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Liquids_Liquids",
                        column: x => x.SolventId,
                        principalTable: "Liquids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Merits_ChargenResources",
                columns: table => new
                {
                    MeritId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequirementOnly = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    Amount = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.MeritId, x.ChargenResourceId, x.RequirementOnly });
                    table.ForeignKey(
                        name: "FK_Merits_ChargenResources_ChargenResources",
                        column: x => x.ChargenResourceId,
                        principalTable: "ChargenResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Merits_ChargenResources_Merits",
                        column: x => x.MeritId,
                        principalTable: "Merits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cultures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    NameCultureId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PersonWordMale = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PersonWordFemale = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PersonWordNeuter = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PersonWordIndeterminate = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PrimaryCalendarId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SkillStartingValueProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AvailabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TolerableTemperatureFloorEffect = table.Column<double>(nullable: false),
                    TolerableTemperatureCeilingEffect = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cultures_AvailabilityProg",
                        column: x => x.AvailabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Cultures_NameCulture",
                        column: x => x.NameCultureId,
                        principalTable: "NameCulture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cultures_SkillStartingProg",
                        column: x => x.SkillStartingValueProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RandomNameProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Gender = table.Column<int>(type: "int(11)", nullable: false),
                    NameCultureId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RandomNameProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RandomNameProfiles_NameCulture",
                        column: x => x.NameCultureId,
                        principalTable: "NameCulture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PopulationBloodModels_Bloodtypes",
                columns: table => new
                {
                    BloodtypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PopulationBloodModelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Weight = table.Column<double>(nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BloodtypeId, x.PopulationBloodModelId });
                    table.ForeignKey(
                        name: "FK_PopulationBloodModels_Bloodtypes_Bloodtypes",
                        column: x => x.BloodtypeId,
                        principalTable: "Bloodtypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels",
                        column: x => x.PopulationBloodModelId,
                        principalTable: "PopulationBloodModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MinimumTerrestrialLux = table.Column<double>(nullable: false),
                    SkyDescriptionTemplateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SphericalRadiusMetres = table.Column<double>(nullable: false, defaultValueSql: "'6371000'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shards_SkyDescriptionTemplates",
                        column: x => x.SkyDescriptionTemplateId,
                        principalTable: "SkyDescriptionTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkyDescriptionTemplates_Values",
                columns: table => new
                {
                    SkyDescriptionTemplateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LowerBound = table.Column<double>(nullable: false),
                    UpperBound = table.Column<double>(nullable: false),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.SkyDescriptionTemplateId, x.LowerBound });
                    table.ForeignKey(
                        name: "FK_SkyDescriptionTemplates_Values_SkyDescriptionTemplates",
                        column: x => x.SkyDescriptionTemplateId,
                        principalTable: "SkyDescriptionTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checks",
                columns: table => new
                {
                    Type = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TraitExpressionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CheckTemplateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MaximumDifficultyForImprovement = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'10'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Type);
                    table.ForeignKey(
                        name: "FK_Checks_CheckTemplates",
                        column: x => x.CheckTemplateId,
                        principalTable: "CheckTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checks_TraitExpression",
                        column: x => x.TraitExpressionId,
                        principalTable: "TraitExpression",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TraitDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    DecoratorId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TraitGroup = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DerivedType = table.Column<int>(type: "int(11)", nullable: false),
                    ExpressionId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ImproverId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Hidden = table.Column<bool>(type: "bit(1)", nullable: true, defaultValueSql: "b'0'"),
                    ChargenBlurb = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BranchMultiplier = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    Alias = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AvailabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TeachableProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    LearnableProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TeachDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'7'"),
                    LearnDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'7'"),
                    ValueExpression = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraitDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraitDefinitions_AvailabilityProg",
                        column: x => x.AvailabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TraitDefinitions_TraitExpression",
                        column: x => x.ExpressionId,
                        principalTable: "TraitExpression",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TraitDefinitions_LearnableProg",
                        column: x => x.LearnableProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TraitDefinitions_TeachableProg",
                        column: x => x.TeachableProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AccountNotes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Subject = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TimeStamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountNotes_Accounts",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountNotes_Author",
                        column: x => x.AuthorId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accounts_ChargenResources",
                columns: table => new
                {
                    AccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<int>(type: "int(11)", nullable: false),
                    LastAwardDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.AccountId, x.ChargenResourceId });
                    table.ForeignKey(
                        name: "FK_Accounts_ChargenResources_Accounts",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_ChargenResources_ChargenResources",
                        column: x => x.ChargenResourceId,
                        principalTable: "ChargenResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IpMask = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BannerAccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Reason = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Expiry = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bans_Accounts",
                        column: x => x.BannerAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BoardPosts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BoardId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Content = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AuthorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PostTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardsPosts_Accounts",
                        column: x => x.AuthorId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BoardPosts_Boards",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    PosterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MaximumNumberAlive = table.Column<int>(type: "int(11)", nullable: false),
                    MaximumNumberTotal = table.Column<int>(type: "int(11)", nullable: false),
                    ChargenBlurb = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AvailabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Expired = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    MinimumAuthorityToApprove = table.Column<int>(type: "int(11)", nullable: false),
                    MinimumAuthorityToView = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargenRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargenRoles_FutureProgs",
                        column: x => x.AvailabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Accounts",
                        column: x => x.PosterId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Chargens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(12000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    SubmitTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    MinimumApprovalAuthority = table.Column<int>(type: "int(11)", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint(20)", nullable: true),
                    ApprovalTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chargens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chargens_Accounts",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoginIPs",
                columns: table => new
                {
                    IpAddress = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AccountId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FirstDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountRegisteredOnThisIP = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.IpAddress, x.AccountId });
                    table.ForeignKey(
                        name: "FK_LoginIPs_Accounts",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegionalClimates_Seasons",
                columns: table => new
                {
                    RegionalClimateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TemperatureInfo = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RegionalClimateId, x.SeasonId });
                    table.ForeignKey(
                        name: "FK_RegionalClimates_Seasons_RegionalClimates",
                        column: x => x.RegionalClimateId,
                        principalTable: "RegionalClimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionalClimates_Seasons_Seasons",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeatherControllers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FeedClockId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FeedClockTimeZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RegionalClimateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrentWeatherEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrentSeasonId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ConsecutiveUnchangedPeriods = table.Column<int>(type: "int(11)", nullable: false),
                    MinutesCounter = table.Column<int>(type: "int(11)", nullable: false),
                    CelestialId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Elevation = table.Column<double>(nullable: false),
                    Radius = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    HighestRecentPrecipitationLevel = table.Column<int>(type: "int(11)", nullable: false),
                    PeriodsSinceHighestPrecipitation = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherControllers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherControllers_Celestials",
                        column: x => x.CelestialId,
                        principalTable: "Celestials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeatherControllers_Seasons",
                        column: x => x.CurrentSeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeatherControllers_WeatherEvents",
                        column: x => x.CurrentWeatherEventId,
                        principalTable: "WeatherEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeatherControllers_Clocks",
                        column: x => x.FeedClockId,
                        principalTable: "Clocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeatherControllers_TimeZones",
                        column: x => x.FeedClockTimeZoneId,
                        principalTable: "Timezones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeatherControllers_RegionalClimates",
                        column: x => x.RegionalClimateId,
                        principalTable: "RegionalClimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyDivisionAbbreviations",
                columns: table => new
                {
                    Pattern = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CurrencyDivisionId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Pattern, x.CurrencyDivisionId });
                    table.ForeignKey(
                        name: "FK_CurrencyDivisionAbbreviations_CurrencyDivisions",
                        column: x => x.CurrencyDivisionId,
                        principalTable: "CurrencyDivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnforcementAuthorities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Priority = table.Column<int>(type: "int(11)", nullable: false),
                    CanAccuse = table.Column<bool>(type: "bit(1)", nullable: false),
                    CanForgive = table.Column<bool>(type: "bit(1)", nullable: false),
                    CanConvict = table.Column<bool>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnforcementAuthorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Laws",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CrimeType = table.Column<int>(type: "int(11)", nullable: false),
                    ActivePeriod = table.Column<double>(nullable: false),
                    EnforcementStrategy = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LawAppliesProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    EnforcementPriority = table.Column<int>(type: "int(11)", nullable: false),
                    MinimumFine = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    StandardFine = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    MaximumFine = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    CanBeAppliedAutomatically = table.Column<bool>(type: "bit(1)", nullable: false),
                    CanBeArrested = table.Column<bool>(type: "bit(1)", nullable: false),
                    CanBeOfferedBail = table.Column<bool>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laws", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Laws_FutureProgs",
                        column: x => x.LawAppliesProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Laws_LegalAuthority",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalClasses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassPriority = table.Column<int>(type: "int(11)", nullable: false),
                    MembershipProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CanBeDetainedUntilFinesPaid = table.Column<bool>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalClasses_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalClasses_FutureProgs",
                        column: x => x.MembershipProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForagableProfiles_Foragables",
                columns: table => new
                {
                    ForagableProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ForagableProfileRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    ForagableId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ForagableProfileId, x.ForagableProfileRevisionNumber, x.ForagableId });
                    table.ForeignKey(
                        name: "FK_ForagableProfiles_Foragables_ForagableProfiles",
                        columns: x => new { x.ForagableProfileId, x.ForagableProfileRevisionNumber },
                        principalTable: "ForagableProfiles",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForagableProfiles_HourlyYieldGains",
                columns: table => new
                {
                    ForagableProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ForagableProfileRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    ForageType = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Yield = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ForagableProfileId, x.ForagableProfileRevisionNumber, x.ForageType });
                    table.ForeignKey(
                        name: "FK_ForagableProfiles_HourlyYieldGains_ForagableProfiles",
                        columns: x => new { x.ForagableProfileId, x.ForagableProfileRevisionNumber },
                        principalTable: "ForagableProfiles",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForagableProfiles_MaximumYields",
                columns: table => new
                {
                    ForagableProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ForagableProfileRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    ForageType = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Yield = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ForagableProfileId, x.ForagableProfileRevisionNumber, x.ForageType });
                    table.ForeignKey(
                        name: "FK_ForagableProfiles_MaximumYields_ForagableProfiles",
                        columns: x => new { x.ForagableProfileId, x.ForagableProfileRevisionNumber },
                        principalTable: "ForagableProfiles",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NPCTemplates_ArtificalIntelligences",
                columns: table => new
                {
                    NPCTemplateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AIId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NPCTemplateRevisionNumber = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.NPCTemplateRevisionNumber, x.NPCTemplateId, x.AIId });
                    table.ForeignKey(
                        name: "FK_NTAI_ArtificalIntelligences",
                        column: x => x.AIId,
                        principalTable: "ArtificialIntelligences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NTAI_NPCTemplates",
                        columns: x => new { x.NPCTemplateId, x.NPCTemplateRevisionNumber },
                        principalTable: "NPCTemplates",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPhases",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProjectRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    PhaseNumber = table.Column<int>(type: "int(11)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectPhases_Projects",
                        columns: x => new { x.ProjectId, x.ProjectRevisionNumber },
                        principalTable: "Projects",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelCommandWords",
                columns: table => new
                {
                    Word = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ChannelId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Word);
                    table.ForeignKey(
                        name: "FK_ChannelCommandWords_Channels",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelIgnorers",
                columns: table => new
                {
                    ChannelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccountId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChannelId, x.AccountId });
                    table.ForeignKey(
                        name: "FK_ChannelIgnorers_Accounts",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelIgnorers_Channels",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyDescriptionPatternElements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Pattern = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Order = table.Column<int>(type: "int(11)", nullable: false),
                    ShowIfZero = table.Column<bool>(type: "bit(1)", nullable: false),
                    CurrencyDivisionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyDescriptionPatternId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PluraliseWord = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AlternatePattern = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RoundingMode = table.Column<int>(type: "int(11)", nullable: false),
                    SpecialValuesOverrideFormat = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyDescriptionPatternElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CDPE_CurrencyDescriptionPatterns",
                        column: x => x.CurrencyDescriptionPatternId,
                        principalTable: "CurrencyDescriptionPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CDPE_CurrencyDivisions",
                        column: x => x.CurrencyDivisionId,
                        principalTable: "CurrencyDivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dream_Phases",
                columns: table => new
                {
                    DreamId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PhaseId = table.Column<int>(type: "int(11)", nullable: false),
                    DreamerText = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    DreamerCommand = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    WaitSeconds = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'30'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.DreamId, x.PhaseId });
                    table.ForeignKey(
                        name: "FK_Dream_Phases_Dreams",
                        column: x => x.DreamId,
                        principalTable: "Dreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityDescriptionPatterns_EntityDescriptions",
                columns: table => new
                {
                    PatternId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EntityDescriptionId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.PatternId, x.EntityDescriptionId });
                    table.ForeignKey(
                        name: "FK_EDP_EntityDescriptions_EntityDescriptions",
                        column: x => x.EntityDescriptionId,
                        principalTable: "EntityDescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EDP_EntityDescriptions_EntityDescriptionPatterns",
                        column: x => x.PatternId,
                        principalTable: "EntityDescriptionPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Helpfiles_ExtraTexts",
                columns: table => new
                {
                    HelpfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RuleId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.HelpfileId, x.DisplayOrder });
                    table.ForeignKey(
                        name: "FK_Helpfiles_ExtraTexts_Helpfiles",
                        column: x => x.HelpfileId,
                        principalTable: "Helpfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Helpfiles_ExtraTexts_FutureProgs",
                        column: x => x.RuleId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Scripts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    KnownScriptDescription = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    UnknownScriptDescription = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    KnowledgeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DocumentLengthModifier = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    InkUseModifier = table.Column<double>(nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scripts_Knowledges",
                        column: x => x.KnowledgeId,
                        principalTable: "knowledges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurgicalProcedures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ProcedureName = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Procedure = table.Column<int>(type: "int(11)", nullable: false),
                    BaseCheckBonus = table.Column<double>(nullable: false),
                    Check = table.Column<int>(type: "int(11)", nullable: false),
                    MedicalSchool = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    KnowledgeRequiredId = table.Column<long>(type: "bigint(20)", nullable: true),
                    UsabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WhyCannotUseProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CompletionProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AbortProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ProcedureBeginEmote = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ProcedureDescriptionEmote = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    ProcedureGerund = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Definition = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurgicalProcedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurgicalProcedures_FutureProgs_AbortProg",
                        column: x => x.AbortProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SurgicalProcedures_FutureProgs_CompletionProg",
                        column: x => x.CompletionProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SurgicalProcedures_Knowledges",
                        column: x => x.KnowledgeRequiredId,
                        principalTable: "knowledges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SurgicalProcedures_FutureProgs_Usability",
                        column: x => x.UsabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg",
                        column: x => x.WhyCannotUseProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Materials_Tags",
                columns: table => new
                {
                    MaterialId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TagId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.MaterialId, x.TagId });
                    table.ForeignKey(
                        name: "Materials_Tags_Materials",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Materials_Tags_Tags",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceButcheryProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Verb = table.Column<int>(type: "int(11)", nullable: false),
                    RequiredToolTagId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DifficultySkin = table.Column<int>(type: "int(11)", nullable: false),
                    CanButcherProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WhyCannotButcherProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceButcheryProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_FutureProgs_Can",
                        column: x => x.CanButcherProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_Tags",
                        column: x => x.RequiredToolTagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_FutureProgs_Why",
                        column: x => x.WhyCannotButcherProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WitnessProfiles_CooperatingAuthorities",
                columns: table => new
                {
                    WitnessProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.WitnessProfileId, x.LegalAuthorityId });
                    table.ForeignKey(
                        name: "FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WitnessProfiles_CooperatingAuthorities_WitnessProfiles",
                        column: x => x.WitnessProfileId,
                        principalTable: "WitnessProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameItemProtos_DefaultVariables",
                columns: table => new
                {
                    GameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VariableName = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    GameItemProtoRevNum = table.Column<int>(type: "int(11)", nullable: false),
                    VariableValue = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.GameItemProtoId, x.GameItemProtoRevNum, x.VariableName });
                    table.ForeignKey(
                        name: "FK_GameItemProtos_DefaultValues_GameItemProtos",
                        columns: x => new { x.GameItemProtoId, x.GameItemProtoRevNum },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameItemProtos_GameItemComponentProtos",
                columns: table => new
                {
                    GameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemComponentProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    GameItemComponentRevision = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.GameItemProtoId, x.GameItemComponentProtoId, x.GameItemProtoRevision, x.GameItemComponentRevision });
                    table.ForeignKey(
                        name: "FK_GIPGICP_GameItemComponentProtos",
                        columns: x => new { x.GameItemComponentProtoId, x.GameItemComponentRevision },
                        principalTable: "GameItemComponentProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GIPGICP_GameItemProtos",
                        columns: x => new { x.GameItemProtoId, x.GameItemProtoRevision },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameItemProtos_OnLoadProgs",
                columns: table => new
                {
                    GameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemProtoRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.GameItemProtoId, x.GameItemProtoRevisionNumber, x.FutureProgId });
                    table.ForeignKey(
                        name: "FK_GameItemProtos_OnLoadProgs_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameItemProtos_OnLoadProgs_GameItemProtos",
                        columns: x => new { x.GameItemProtoId, x.GameItemProtoRevisionNumber },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameItemProtos_Tags",
                columns: table => new
                {
                    GameItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TagId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemProtoRevisionNumber = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.GameItemProtoId, x.TagId, x.GameItemProtoRevisionNumber });
                    table.ForeignKey(
                        name: "FK_GameItemProtos_Tags_Tags",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameItemProtos_Tags_GameItemProtos",
                        columns: x => new { x.GameItemProtoId, x.GameItemProtoRevisionNumber },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gases",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Description = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Density = table.Column<double>(nullable: false, defaultValueSql: "'0.001205'"),
                    ThermalConductivity = table.Column<double>(nullable: false, defaultValueSql: "'0.0257'"),
                    ElectricalConductivity = table.Column<double>(nullable: false, defaultValueSql: "'0.000005'"),
                    Organic = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    SpecificHeatCapacity = table.Column<double>(nullable: false, defaultValueSql: "'1.005'"),
                    BoilingPoint = table.Column<double>(nullable: false, defaultValueSql: "'5'"),
                    CountAsId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CountsAsQuality = table.Column<int>(type: "int(11)", nullable: true),
                    DisplayColour = table.Column<string>(type: "varchar(40)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    PrecipitateId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SmellIntensity = table.Column<double>(nullable: false),
                    SmellText = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    VagueSmellText = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Viscosity = table.Column<double>(nullable: false, defaultValueSql: "'15'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gases_Gases",
                        column: x => x.CountAsId,
                        principalTable: "Gases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Gases_Liquids",
                        column: x => x.PrecipitateId,
                        principalTable: "Liquids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Liquids_Tags",
                columns: table => new
                {
                    LiquidId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TagId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LiquidId, x.TagId });
                    table.ForeignKey(
                        name: "FK_Liquids_Tags_Liquids",
                        column: x => x.LiquidId,
                        principalTable: "Liquids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Liquids_Tags_Tags",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenAdvices_Cultures",
                columns: table => new
                {
                    ChargenAdviceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CultureId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenAdviceId, x.CultureId });
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_Cultures_ChargenAdvices",
                        column: x => x.ChargenAdviceId,
                        principalTable: "ChargenAdvices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_Cultures_Cultures",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cultures_ChargenResources",
                columns: table => new
                {
                    CultureId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequirementOnly = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    Amount = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CultureId, x.ChargenResourceId, x.RequirementOnly });
                    table.ForeignKey(
                        name: "FK_Cultures_ChargenResources_ChargenResources",
                        column: x => x.ChargenResourceId,
                        principalTable: "ChargenResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cultures_ChargenResources_Races",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RandomNameProfiles_DiceExpressions",
                columns: table => new
                {
                    RandomNameProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NameUsage = table.Column<int>(type: "int(11)", nullable: false),
                    DiceExpression = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RandomNameProfileId, x.NameUsage });
                    table.ForeignKey(
                        name: "FK_RandomNameProfiles_DiceExpressions_RandomNameProfiles",
                        column: x => x.RandomNameProfileId,
                        principalTable: "RandomNameProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RandomNameProfiles_Elements",
                columns: table => new
                {
                    RandomNameProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NameUsage = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Weighting = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RandomNameProfileId, x.NameUsage, x.Name });
                    table.ForeignKey(
                        name: "FK_RandomNameProfiles_Elements_RandomNameProfiles",
                        column: x => x.RandomNameProfileId,
                        principalTable: "RandomNameProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shards_Calendars",
                columns: table => new
                {
                    ShardId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CalendarId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ShardId, x.CalendarId });
                    table.ForeignKey(
                        name: "FK_Shards_Calendars_Shards",
                        column: x => x.ShardId,
                        principalTable: "Shards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shards_Celestials",
                columns: table => new
                {
                    ShardId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CelestialId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ShardId, x.CelestialId });
                    table.ForeignKey(
                        name: "FK_Shards_Celestials_Shards",
                        column: x => x.ShardId,
                        principalTable: "Shards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shards_Clocks",
                columns: table => new
                {
                    ShardId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ClockId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ShardId, x.ClockId });
                    table.ForeignKey(
                        name: "FK_Shards_Clocks_Shards",
                        column: x => x.ShardId,
                        principalTable: "Shards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Crafts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Blurb = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ActionDescription = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Category = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Interruptable = table.Column<bool>(type: "bit(1)", nullable: false),
                    ToolQualityWeighting = table.Column<double>(nullable: false),
                    InputQualityWeighting = table.Column<double>(nullable: false),
                    CheckQualityWeighting = table.Column<double>(nullable: false),
                    FreeSkillChecks = table.Column<int>(type: "int(11)", nullable: false),
                    FailThreshold = table.Column<int>(type: "int(11)", nullable: false),
                    CheckTraitId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CheckDifficulty = table.Column<int>(type: "int(11)", nullable: false),
                    FailPhase = table.Column<int>(type: "int(11)", nullable: false),
                    QualityFormula = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AppearInCraftsListProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanUseProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WhyCannotUseProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OnUseProgStartId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OnUseProgCompleteId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OnUseProgCancelId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ActiveCraftItemSDesc = table.Column<string>(type: "varchar(200)", nullable: false, defaultValueSql: "'a craft in progress'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_Crafts_FutureProgs_AppearInCraftsListProg",
                        column: x => x.AppearInCraftsListProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crafts_FutureProgs_CanUseProg",
                        column: x => x.CanUseProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Crafts_TraitDefinitions",
                        column: x => x.CheckTraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crafts_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Crafts_FutureProgs_OnUseProgCancel",
                        column: x => x.OnUseProgCancelId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Crafts_FutureProgs_OnUseProgComplete",
                        column: x => x.OnUseProgCompleteId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Crafts_FutureProgs_OnUseProgStart",
                        column: x => x.OnUseProgStartId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Crafts_FutureProgs_WhyCannotUseProg",
                        column: x => x.WhyCannotUseProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RangedWeaponTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Classification = table.Column<int>(type: "int(11)", nullable: false),
                    FireTraitId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OperateTraitId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FireableInMelee = table.Column<bool>(type: "bit(1)", nullable: false),
                    DefaultRangeInRooms = table.Column<int>(type: "int(11)", nullable: false),
                    AccuracyBonusExpression = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    DamageBonusExpression = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    AmmunitionLoadType = table.Column<int>(type: "int(11)", nullable: false),
                    SpecificAmmunitionGrade = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    AmmunitionCapacity = table.Column<int>(type: "int(11)", nullable: false),
                    RangedWeaponType = table.Column<int>(type: "int(11)", nullable: false),
                    StaminaToFire = table.Column<double>(nullable: false),
                    StaminaPerLoadStage = table.Column<double>(nullable: false),
                    CoverBonus = table.Column<double>(nullable: false),
                    BaseAimDifficulty = table.Column<int>(type: "int(11)", nullable: false),
                    LoadDelay = table.Column<double>(nullable: false, defaultValueSql: "'0.5'"),
                    ReadyDelay = table.Column<double>(nullable: false, defaultValueSql: "'0.1'"),
                    FireDelay = table.Column<double>(nullable: false, defaultValueSql: "'0.5'"),
                    AimBonusLostPerShot = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    RequiresFreeHandToReady = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    AlwaysRequiresTwoHandsToWield = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangedWeaponTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RangedWeaponTypes_TraitDefinitions_Fire",
                        column: x => x.FireTraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RangedWeaponTypes_TraitDefinitions_Operate",
                        column: x => x.OperateTraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShieldTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    BlockTraitId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BlockBonus = table.Column<double>(nullable: false),
                    StaminaPerBlock = table.Column<double>(nullable: false),
                    EffectiveArmourTypeId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShieldTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShieldTypes_TraitDefinitions",
                        column: x => x.BlockTraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShieldTypes_ArmourTypes",
                        column: x => x.EffectiveArmourTypeId,
                        principalTable: "ArmourTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TraitDefinitions_ChargenResources",
                columns: table => new
                {
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequirementOnly = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    Amount = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.TraitDefinitionId, x.ChargenResourceId, x.RequirementOnly });
                    table.ForeignKey(
                        name: "FK_TraitDefinitions_ChargenResources_ChargenResources",
                        column: x => x.ChargenResourceId,
                        principalTable: "ChargenResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraitDefinitions_ChargenResources_Races",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraitExpressionParameters",
                columns: table => new
                {
                    TraitExpressionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Parameter = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CanImprove = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    CanBranch = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Parameter, x.TraitExpressionId });
                    table.ForeignKey(
                        name: "FK_TraitExpressionParameters_TraitDefinitions",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TraitExpressionParameters_TraitExpression",
                        column: x => x.TraitExpressionId,
                        principalTable: "TraitExpression",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeaponTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Classification = table.Column<int>(type: "int(11)", nullable: false),
                    AttackTraitId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ParryTraitId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ParryBonus = table.Column<int>(type: "int(11)", nullable: false),
                    Reach = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    StaminaPerParry = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponTypes_TraitDefinitions_Attack",
                        column: x => x.AttackTraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WeaponTypes_TraitDefinitions_Parry",
                        column: x => x.ParryTraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WearableSizeParameterRule",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MinHeightFactor = table.Column<double>(nullable: false),
                    MaxHeightFactor = table.Column<double>(nullable: false),
                    MinWeightFactor = table.Column<double>(nullable: false),
                    MaxWeightFactor = table.Column<double>(nullable: true),
                    MinTraitFactor = table.Column<double>(nullable: true),
                    MaxTraitFactor = table.Column<double>(nullable: true),
                    TraitId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BodyProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IgnoreTrait = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    WeightVolumeRatios = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TraitVolumeRatios = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    HeightLinearRatios = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WearableSizeParameterRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WearableSizeParameterRule_TraitDefinitions",
                        column: x => x.TraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChargenAdvices_ChargenRoles",
                columns: table => new
                {
                    ChargenAdviceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenAdviceId, x.ChargenRoleId });
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_ChargenRoles_ChargenAdvices",
                        column: x => x.ChargenAdviceId,
                        principalTable: "ChargenAdvices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_ChargenRoles_ChargenRoles",
                        column: x => x.ChargenRoleId,
                        principalTable: "ChargenRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenRoles_Approvers",
                columns: table => new
                {
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ApproverId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenRoleId, x.ApproverId });
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Approvers_Accounts",
                        column: x => x.ApproverId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Approvers_ChargenRoles",
                        column: x => x.ChargenRoleId,
                        principalTable: "ChargenRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenRoles_Costs",
                columns: table => new
                {
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequirementOnly = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    Amount = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenRoleId, x.ChargenResourceId, x.RequirementOnly });
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Costs_ChargenResources",
                        column: x => x.ChargenResourceId,
                        principalTable: "ChargenResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Costs_ChargenRoles",
                        column: x => x.ChargenRoleId,
                        principalTable: "ChargenRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenRoles_Currencies",
                columns: table => new
                {
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenRoleId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Currencies_ChargenRoles",
                        column: x => x.ChargenRoleId,
                        principalTable: "ChargenRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Currencies_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenRoles_Merits",
                columns: table => new
                {
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MeritId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenRoleId, x.MeritId });
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Merits_ChargenRoles",
                        column: x => x.ChargenRoleId,
                        principalTable: "ChargenRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Merits_Merits",
                        column: x => x.MeritId,
                        principalTable: "Merits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenRoles_Traits",
                columns: table => new
                {
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TraitId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    GiveIfDoesntHave = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenRoleId, x.TraitId });
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Traits_ChargenRoles",
                        column: x => x.ChargenRoleId,
                        principalTable: "ChargenRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenRoles_Traits_Currencies",
                        column: x => x.TraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WeatherControllerId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_WeatherControllers",
                        column: x => x.WeatherControllerId,
                        principalTable: "WeatherControllers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Terrains",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MovementRate = table.Column<double>(nullable: false),
                    DefaultTerrain = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    TerrainBehaviourMode = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    HideDifficulty = table.Column<int>(type: "int(11)", nullable: false),
                    SpotDifficulty = table.Column<int>(type: "int(11)", nullable: false),
                    StaminaCost = table.Column<double>(nullable: false),
                    ForagableProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    InfectionMultiplier = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    InfectionType = table.Column<int>(type: "int(11)", nullable: false),
                    InfectionVirulence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    AtmosphereId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AtmosphereType = table.Column<string>(type: "varchar(45)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TerrainEditorColour = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'#FFFFFFFF'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WeatherControllerId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terrains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terrains_WeatherControllers",
                        column: x => x.WeatherControllerId,
                        principalTable: "WeatherControllers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EnforcementAuthorities_ParentAuthorities",
                columns: table => new
                {
                    ParentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChildId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_ParentAuthorities_Child",
                        column: x => x.ChildId,
                        principalTable: "EnforcementAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_ParentAuthorities_Parent",
                        column: x => x.ParentId,
                        principalTable: "EnforcementAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnforcementAuthorities_AccusableClasses",
                columns: table => new
                {
                    EnforcementAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EnforcementAuthorityId, x.LegalClassId });
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_AccusableClasses_Enforce",
                        column: x => x.EnforcementAuthorityId,
                        principalTable: "EnforcementAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_AccusableClasses_LegalClasses",
                        column: x => x.LegalClassId,
                        principalTable: "LegalClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnforcementAuthorities_ArrestableClasses",
                columns: table => new
                {
                    EnforcementAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.EnforcementAuthorityId);
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_ArrestableClasses_Enforce",
                        column: x => x.EnforcementAuthorityId,
                        principalTable: "EnforcementAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_ArrestableClasses_LegalClasses",
                        column: x => x.LegalClassId,
                        principalTable: "LegalClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Laws_OffenderClasses",
                columns: table => new
                {
                    LawId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LawId, x.LegalClassId });
                    table.ForeignKey(
                        name: "FK_Laws_OffenderClasses_Laws",
                        column: x => x.LawId,
                        principalTable: "Laws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Laws_OffenderClasses_LegalClasses",
                        column: x => x.LegalClassId,
                        principalTable: "LegalClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Laws_VictimClasses",
                columns: table => new
                {
                    LawId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LawId, x.LegalClassId });
                    table.ForeignKey(
                        name: "FK_Laws_VictimClasses_Laws",
                        column: x => x.LawId,
                        principalTable: "Laws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Laws_VictimClasses_LegalClasses",
                        column: x => x.LegalClassId,
                        principalTable: "LegalClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WitnessProfiles_IgnoredCriminalClasses",
                columns: table => new
                {
                    WitnessProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.WitnessProfileId, x.LegalClassId });
                    table.ForeignKey(
                        name: "FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses",
                        column: x => x.LegalClassId,
                        principalTable: "LegalClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WitnessProfiles_IgnoredCriminalClasses_WitnessProfiles",
                        column: x => x.WitnessProfileId,
                        principalTable: "WitnessProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WitnessProfiles_IgnoredVictimClasses",
                columns: table => new
                {
                    WitnessProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.WitnessProfileId, x.LegalClassId });
                    table.ForeignKey(
                        name: "FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses",
                        column: x => x.LegalClassId,
                        principalTable: "LegalClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WitnessProfiles_IgnoredVictimClasses_WitnessProfiles",
                        column: x => x.WitnessProfileId,
                        principalTable: "WitnessProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    SortOrder = table.Column<int>(type: "int(11)", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ProjectPhaseId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectActions_ProjectPhases",
                        column: x => x.ProjectPhaseId,
                        principalTable: "ProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLabourRequirements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ProjectPhaseId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TotalProgressRequired = table.Column<double>(nullable: false),
                    MaximumSimultaneousWorkers = table.Column<int>(type: "int(11)", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLabourRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLabourRequirements_ProjectPhases",
                        column: x => x.ProjectPhaseId,
                        principalTable: "ProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMaterialRequirements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ProjectPhaseId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    IsMandatoryForProjectCompletion = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMaterialRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMaterialRequirements_ProjectPhases",
                        column: x => x.ProjectPhaseId,
                        principalTable: "ProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyDescriptionPatternElementSpecialValues",
                columns: table => new
                {
                    Value = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    CurrencyDescriptionPatternElementId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Text = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Value, x.CurrencyDescriptionPatternElementId });
                    table.ForeignKey(
                        name: "FK_CDPESV_CDPE",
                        column: x => x.CurrencyDescriptionPatternElementId,
                        principalTable: "CurrencyDescriptionPatternElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurgicalProcedurePhases",
                columns: table => new
                {
                    SurgicalProcedureId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PhaseNumber = table.Column<int>(type: "int(11)", nullable: false),
                    BaseLengthInSeconds = table.Column<double>(nullable: false),
                    PhaseEmote = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    PhaseSpecialEffects = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    OnPhaseProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    InventoryActionPlan = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.SurgicalProcedureId, x.PhaseNumber });
                    table.ForeignKey(
                        name: "FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg",
                        column: x => x.OnPhaseProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SurgicalProcedurePhases_SurgicalProcudres",
                        column: x => x.SurgicalProcedureId,
                        principalTable: "SurgicalProcedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceButcheryProfiles_BreakdownChecks",
                columns: table => new
                {
                    RaceButcheryProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Subcageory = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Difficulty = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceButcheryProfileId, x.Subcageory });
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_BreakdownChecks_RaceButcheryProfiles",
                        column: x => x.RaceButcheryProfileId,
                        principalTable: "RaceButcheryProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RaceButcheryProfiles_BreakdownEmotes",
                columns: table => new
                {
                    RaceButcheryProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Subcategory = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Order = table.Column<int>(type: "int(11)", nullable: false),
                    Emote = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Delay = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceButcheryProfileId, x.Subcategory, x.Order });
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_BreakdownEmotes_RaceButcheryProfiles",
                        column: x => x.RaceButcheryProfileId,
                        principalTable: "RaceButcheryProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceButcheryProfiles_SkinningEmotes",
                columns: table => new
                {
                    RaceButcheryProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Subcategory = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Order = table.Column<int>(type: "int(11)", nullable: false),
                    Emote = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Delay = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceButcheryProfileId, x.Subcategory, x.Order });
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_SkinningEmotes_RaceButcheryProfiles",
                        column: x => x.RaceButcheryProfileId,
                        principalTable: "RaceButcheryProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gases_Tags",
                columns: table => new
                {
                    GasId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TagId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.GasId, x.TagId });
                    table.ForeignKey(
                        name: "FK_Gases_Tags_Gases",
                        column: x => x.GasId,
                        principalTable: "Gases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gases_Tags_Tags",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CraftInputs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CraftId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CraftRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    InputType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    InputQualityWeight = table.Column<double>(nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OriginalAdditionTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftInputs_Crafts",
                        columns: x => new { x.CraftId, x.CraftRevisionNumber },
                        principalTable: "Crafts",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CraftPhases",
                columns: table => new
                {
                    CraftPhaseId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CraftPhaseRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    PhaseNumber = table.Column<int>(type: "int(11)", nullable: false),
                    PhaseLengthInSeconds = table.Column<double>(nullable: false),
                    Echo = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FailEcho = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CraftPhaseId, x.CraftPhaseRevisionNumber, x.PhaseNumber });
                    table.ForeignKey(
                        name: "FK_CraftPhases_Crafts",
                        columns: x => new { x.CraftPhaseId, x.CraftPhaseRevisionNumber },
                        principalTable: "Crafts",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CraftProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CraftId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CraftRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    ProductType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    OriginalAdditionTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsFailProduct = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    MaterialDefiningInputIndex = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftProducts_Crafts",
                        columns: x => new { x.CraftId, x.CraftRevisionNumber },
                        principalTable: "Crafts",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CraftTools",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CraftId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CraftRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    OriginalAdditionTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ToolType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ToolQualityWeight = table.Column<double>(nullable: false),
                    DesiredState = table.Column<int>(type: "int(11)", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftTools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftTools_Crafts",
                        columns: x => new { x.CraftId, x.CraftRevisionNumber },
                        principalTable: "Crafts",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponAttacks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WeaponTypeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Verb = table.Column<int>(type: "int(11)", nullable: false),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BaseAttackerDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    BaseBlockDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    BaseDodgeDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    BaseParryDifficulty = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    BaseAngleOfIncidence = table.Column<double>(nullable: false, defaultValueSql: "'1.5708'"),
                    RecoveryDifficultySuccess = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    RecoveryDifficultyFailure = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    MoveType = table.Column<int>(type: "int(11)", nullable: false),
                    Intentions = table.Column<long>(type: "bigint(20)", nullable: false),
                    ExertionLevel = table.Column<int>(type: "int(11)", nullable: false),
                    DamageType = table.Column<int>(type: "int(11)", nullable: false),
                    DamageExpressionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    StunExpressionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PainExpressionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Weighting = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    BodypartShapeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    StaminaCost = table.Column<double>(nullable: false),
                    BaseDelay = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    Name = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Orientation = table.Column<int>(type: "int(11)", nullable: false),
                    Alignment = table.Column<int>(type: "int(11)", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    HandednessOptions = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponAttacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponAttacks_TraitExpression_Damage",
                        column: x => x.DamageExpressionId,
                        principalTable: "TraitExpression",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponAttacks_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WeaponAttacks_TraitExpression_Pain",
                        column: x => x.PainExpressionId,
                        principalTable: "TraitExpression",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponAttacks_TraitExpression_Stun",
                        column: x => x.StunExpressionId,
                        principalTable: "TraitExpression",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponAttacks_WeaponTypes",
                        column: x => x.WeaponTypeId,
                        principalTable: "WeaponTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terrains_RangedCovers",
                columns: table => new
                {
                    TerrainId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RangedCoverId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.TerrainId, x.RangedCoverId });
                    table.ForeignKey(
                        name: "FK_Terrains_RangedCovers_RangedCovers",
                        column: x => x.RangedCoverId,
                        principalTable: "RangedCovers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Terrains_RangedCovers_Terrains",
                        column: x => x.TerrainId,
                        principalTable: "Terrains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLabourImpacts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Type = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ProjectLabourRequirementId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MinimumHoursForImpactToKickIn = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLabourImpacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLabourImpacts_ProjectLabourRequirements",
                        column: x => x.ProjectLabourRequirementId,
                        principalTable: "ProjectLabourRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CombatMessages_WeaponAttacks",
                columns: table => new
                {
                    CombatMessageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WeaponAttackId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CombatMessageId, x.WeaponAttackId });
                    table.ForeignKey(
                        name: "FK_CombatMessages_WeaponAttacks_CombatMessages",
                        column: x => x.CombatMessageId,
                        principalTable: "CombatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CombatMessages_WeaponAttacks_WeaponAttacks",
                        column: x => x.WeaponAttackId,
                        principalTable: "WeaponAttacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    DeathTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    State = table.Column<int>(type: "int(11)", nullable: false),
                    Gender = table.Column<short>(type: "smallint(6)", nullable: false),
                    Location = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CultureId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EffectData = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BirthdayDate = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BirthdayCalendarId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsAdminAvatar = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TotalMinutesPlayed = table.Column<int>(type: "int(11)", nullable: false),
                    AlcoholLitres = table.Column<double>(nullable: false),
                    WaterLitres = table.Column<double>(nullable: false),
                    FoodSatiatedHours = table.Column<double>(nullable: false),
                    DrinkSatiatedHours = table.Column<double>(nullable: false),
                    Calories = table.Column<double>(nullable: false),
                    NeedsModel = table.Column<string>(type: "varchar(50)", nullable: false, defaultValueSql: "'NoNeeds'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LongTermPlan = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShortTermPlan = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShownIntroductionMessage = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    IntroductionMessage = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ChargenId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrentCombatSettingId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PreferredDefenseType = table.Column<int>(type: "int(11)", nullable: false),
                    PositionId = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    PositionModifier = table.Column<int>(type: "int(11)", nullable: false),
                    PositionTargetId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PositionTargetType = table.Column<string>(type: "varchar(45)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PositionEmote = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CurrentLanguageId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrentAccentId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrentWritingLanguageId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WritingStyle = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'8256'"),
                    CurrentScriptId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DominantHandAlignment = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'3'"),
                    LastLoginTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CombatBrief = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    RoomBrief = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    LastLogoutTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Outfits = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CurrentProjectLabourId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrentProjectId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrentProjectHours = table.Column<double>(nullable: false),
                    NameInfo = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    NoMercy = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Accounts",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Chargens",
                        column: x => x.ChargenId,
                        principalTable: "Chargens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Characters_Cultures",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_ProjectLabourRequirements",
                        column: x => x.CurrentProjectLabourId,
                        principalTable: "ProjectLabourRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Characters_Scripts",
                        column: x => x.CurrentScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Allies",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AllyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Trusted = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.AllyId });
                    table.ForeignKey(
                        name: "FK_Allies_Characters_Target",
                        column: x => x.AllyId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allies_Characters_Owner",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterCombatSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    GlobalTemplate = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    AvailabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CharacterOwnerId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WeaponUsePercentage = table.Column<double>(nullable: false),
                    MagicUsePercentage = table.Column<double>(nullable: false),
                    PsychicUsePercentage = table.Column<double>(nullable: false),
                    NaturalWeaponPercentage = table.Column<double>(nullable: false),
                    AuxillaryPercentage = table.Column<double>(nullable: false),
                    PreferToFightArmed = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    PreferFavouriteWeapon = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    PreferShieldUse = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    ClassificationsAllowed = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    RequiredIntentions = table.Column<long>(type: "bigint(20)", nullable: false),
                    ForbiddenIntentions = table.Column<long>(type: "bigint(20)", nullable: false),
                    PreferredIntentions = table.Column<long>(type: "bigint(20)", nullable: false),
                    AttackUnarmedOrHelpless = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    FallbackToUnarmedIfNoWeapon = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    AttackCriticallyInjured = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    SkirmishToOtherLocations = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    PursuitMode = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    DefaultPreferredDefenseType = table.Column<int>(type: "int(11)", nullable: false),
                    PreferredMeleeMode = table.Column<int>(type: "int(11)", nullable: false),
                    PreferredRangedMode = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    AutomaticallyMoveTowardsTarget = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    PreferNonContactClinchBreaking = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    InventoryManagement = table.Column<int>(type: "int(11)", nullable: false),
                    MovementManagement = table.Column<int>(type: "int(11)", nullable: false),
                    RangedManagement = table.Column<int>(type: "int(11)", nullable: false),
                    ManualPositionManagement = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    MinimumStaminaToAttack = table.Column<double>(nullable: false),
                    MoveToMeleeIfCannotEngageInRangedCombat = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    PreferredWeaponSetup = table.Column<int>(type: "int(11)", nullable: false),
                    RequiredMinimumAim = table.Column<double>(nullable: false, defaultValueSql: "'0.5'"),
                    MeleeAttackOrderPreference = table.Column<string>(type: "varchar(100)", nullable: false, defaultValueSql: "'0 1 2 3 4'")
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    GrappleResponse = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterCombatSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterCombatSettings_FutureProgs",
                        column: x => x.AvailabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CharacterCombatSettings_Characters",
                        column: x => x.CharacterOwnerId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterKnowledges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    KnowledgeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WhenAcquired = table.Column<DateTime>(type: "datetime", nullable: false),
                    HowAcquired = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TimesTaught = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterKnowledges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CHARACTERKNOWLEDGES_CHARACTERS",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHARACTERKNOWLEDGES_KNOWLEDGES",
                        column: x => x.KnowledgeId,
                        principalTable: "knowledges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters_ChargenRoles",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.ChargenRoleId });
                    table.ForeignKey(
                        name: "FK_Characters_ChargenRoles_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_ChargenRoles_ChargenRoles",
                        column: x => x.ChargenRoleId,
                        principalTable: "ChargenRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters_MagicResources",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MagicResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.MagicResourceId });
                    table.ForeignKey(
                        name: "FK_Characters_MagicResources_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_MagicResources_MagicResources",
                        column: x => x.MagicResourceId,
                        principalTable: "MagicResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters_Scripts",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ScriptId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.ScriptId });
                    table.ForeignKey(
                        name: "FK_Characters_Scripts_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Scripts_Scripts",
                        column: x => x.ScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clans",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Alias = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FullName = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ParentClanId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PayIntervalType = table.Column<int>(type: "int(11)", nullable: false),
                    PayIntervalModifier = table.Column<int>(type: "int(11)", nullable: false),
                    PayIntervalOther = table.Column<int>(type: "int(11)", nullable: false),
                    CalendarId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PayIntervalReferenceDate = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PayIntervalReferenceTime = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    IsTemplate = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    ShowClanMembersInWho = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    PaymasterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaymasterItemProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OnPayProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MaximumPeriodsOfUncollectedBackPay = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clans_Calendars",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clans_FutureProgs",
                        column: x => x.OnPayProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Clans_Parent",
                        column: x => x.ParentClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clans_Characters",
                        column: x => x.PaymasterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Drawings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FullDescription = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ImplementType = table.Column<int>(type: "int(11)", nullable: false),
                    DrawingSkill = table.Column<double>(nullable: false),
                    DrawingSize = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drawings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drawings_Characters",
                        column: x => x.AuthorId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dreams_Already_Dreamt",
                columns: table => new
                {
                    DreamId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.DreamId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_Dreams_Dreamt_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dreams_Dreamt_Dreams",
                        column: x => x.DreamId,
                        principalTable: "Dreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dreams_Characters",
                columns: table => new
                {
                    DreamId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.DreamId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_Dreams_Characters_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dreams_Characters_Dreams",
                        column: x => x.DreamId,
                        principalTable: "Dreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dubs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Keywords = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TargetId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetType = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LastDescription = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LastUsage = table.Column<DateTime>(type: "datetime", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IntroducedName = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dubs_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_Guests_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NPCs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TemplateId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TemplateRevnum = table.Column<int>(type: "int(11)", nullable: false),
                    BodyguardCharacterId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NPCs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NPCs_Characters_Bodyguard",
                        column: x => x.BodyguardCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NPCs_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NPCs_NPCTemplates",
                        columns: x => new { x.TemplateId, x.TemplateRevnum },
                        principalTable: "NPCTemplates",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenRoles_ClanMemberships",
                columns: table => new
                {
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PaygradeId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenRoleId, x.ClanId });
                    table.ForeignKey(
                        name: "FK_ChargenRoles_ClanMemberships_ChargenRoles",
                        column: x => x.ChargenRoleId,
                        principalTable: "ChargenRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenRoles_ClanMemberships_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanMemberships",
                columns: table => new
                {
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PaygradeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    JoinDate = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ManagerId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PersonalName = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ClanId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Manager",
                        column: x => x.ManagerId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Paygrades",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Abbreviation = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PayAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paygrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paygrades_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Paygrades_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    InsigniaGameItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    InsigniaGameItemRevnum = table.Column<int>(type: "int(11)", nullable: true),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Privileges = table.Column<long>(type: "bigint(20)", nullable: false),
                    RankPath = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RankNumber = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ranks_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ranks_GameItemProtos",
                        columns: x => new { x.InsigniaGameItemId, x.InsigniaGameItemRevnum },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NPCs_ArtificialIntelligences",
                columns: table => new
                {
                    NPCId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ArtificialIntelligenceId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ArtificialIntelligenceId, x.NPCId });
                    table.ForeignKey(
                        name: "FK_NPCs_ArtificialIntelligences_ArtificialIntelligences",
                        column: x => x.ArtificialIntelligenceId,
                        principalTable: "ArtificialIntelligences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NPCs_ArtificialIntelligences_NPCs",
                        column: x => x.NPCId,
                        principalTable: "NPCs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenRoles_ClanMemberships_Appointments",
                columns: table => new
                {
                    ChargenRoleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenRoleId, x.ClanId, x.AppointmentId });
                    table.ForeignKey(
                        name: "FK_CRCMA_ChargenRoles_ClanMemberships",
                        columns: x => new { x.ChargenRoleId, x.ClanId },
                        principalTable: "ChargenRoles_ClanMemberships",
                        principalColumns: new[] { "ChargenRoleId", "ClanId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanMemberships_Backpay",
                columns: table => new
                {
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CurrencyId, x.ClanId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Backpay_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Backpay_ClanMemberships",
                        columns: x => new { x.ClanId, x.CharacterId },
                        principalTable: "ClanMemberships",
                        principalColumns: new[] { "ClanId", "CharacterId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MaximumSimultaneousHolders = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    MinimumRankId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ParentAppointmentId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PaygradeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    InsigniaGameItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    InsigniaGameItemRevnum = table.Column<int>(type: "int(11)", nullable: true),
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Privileges = table.Column<long>(type: "bigint(20)", nullable: false),
                    MinimumRankToAppointId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Ranks",
                        column: x => x.MinimumRankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Appointments_Ranks_2",
                        column: x => x.MinimumRankToAppointId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_ParentAppointment",
                        column: x => x.ParentAppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Paygrades",
                        column: x => x.PaygradeId,
                        principalTable: "Paygrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Appointments_GameItemProtos",
                        columns: x => new { x.InsigniaGameItemId, x.InsigniaGameItemRevnum },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ranks_Abbreviations",
                columns: table => new
                {
                    RankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Abbreviation = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Order = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RankId, x.Abbreviation });
                    table.ForeignKey(
                        name: "FK_Ranks_Abbreviations_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ranks_Abbreviations_Ranks",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ranks_Paygrades",
                columns: table => new
                {
                    RankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PaygradeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Order = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RankId, x.PaygradeId });
                    table.ForeignKey(
                        name: "FK_Ranks_Paygrades_Paygrades",
                        column: x => x.PaygradeId,
                        principalTable: "Paygrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ranks_Paygrades_Ranks",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ranks_Titles",
                columns: table => new
                {
                    RankId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Order = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RankId, x.Title });
                    table.ForeignKey(
                        name: "FK_Ranks_Titles_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ranks_Titles_Ranks",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments_Abbreviations",
                columns: table => new
                {
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Abbreviation = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Order = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Abbreviation, x.AppointmentId });
                    table.ForeignKey(
                        name: "FK_Appointments_Abbreviations_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Abbreviations_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointments_Titles",
                columns: table => new
                {
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FutureProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Order = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Title, x.AppointmentId });
                    table.ForeignKey(
                        name: "FK_Appointments_Titles_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Titles_FutureProgs",
                        column: x => x.FutureProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClanMemberships_Appointments",
                columns: table => new
                {
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AppointmentId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ClanId, x.CharacterId, x.AppointmentId });
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Appointments_Appointments",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Appointments_ClanMemberships",
                        columns: x => new { x.ClanId, x.CharacterId },
                        principalTable: "ClanMemberships",
                        principalColumns: new[] { "ClanId", "CharacterId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalClanControls",
                columns: table => new
                {
                    VassalClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LiegeClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ControlledAppointmentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ControllingAppointmentId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NumberOfAppointments = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.VassalClanId, x.LiegeClanId, x.ControlledAppointmentId });
                    table.ForeignKey(
                        name: "FK_ECC_Appointments_Controlled",
                        column: x => x.ControlledAppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ECC_Appointments_Controlling",
                        column: x => x.ControllingAppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ECC_Clans_Liege",
                        column: x => x.LiegeClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ECC_Clans_Vassal",
                        column: x => x.VassalClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalClanControls_Appointments",
                columns: table => new
                {
                    VassalClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LiegeClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ControlledAppointmentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.VassalClanId, x.LiegeClanId, x.ControlledAppointmentId });
                    table.ForeignKey(
                        name: "FK_ECC_Appointments_ClanMemberships",
                        columns: x => new { x.VassalClanId, x.CharacterId },
                        principalTable: "ClanMemberships",
                        principalColumns: new[] { "ClanId", "CharacterId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ECC_Appointments_ExternalClanControls",
                        columns: x => new { x.VassalClanId, x.LiegeClanId, x.ControlledAppointmentId },
                        principalTable: "ExternalClanControls",
                        principalColumns: new[] { "VassalClanId", "LiegeClanId", "ControlledAppointmentId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Characters_Accents",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AccentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Familiarity = table.Column<int>(type: "int(11)", nullable: false),
                    IsPreferred = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.AccentId });
                    table.ForeignKey(
                        name: "FK_Characters_Accents_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DifficultyModel = table.Column<long>(type: "bigint(20)", nullable: false),
                    LinkedTraitId = table.Column<long>(type: "bigint(20)", nullable: false),
                    UnknownLanguageDescription = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LanguageObfuscationFactor = table.Column<double>(nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DefaultLearnerAccentId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Languages_LanguageDifficultyModels",
                        column: x => x.DifficultyModel,
                        principalTable: "LanguageDifficultyModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Languages_TraitDefinitions",
                        column: x => x.LinkedTraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LanguageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Suffix = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    VagueSuffix = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Difficulty = table.Column<int>(type: "int(11)", nullable: false),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Group = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ChargenAvailabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accents_Languages",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Characters_Languages",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Characters_Languages_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Languages_Languages",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MutualIntelligabilities",
                columns: table => new
                {
                    ListenerLanguageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetLanguageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IntelligabilityDifficulty = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ListenerLanguageId, x.TargetLanguageId });
                    table.ForeignKey(
                        name: "FK_Languages_MutualIntelligabilities_Listener",
                        column: x => x.ListenerLanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Languages_MutualIntelligabilities_Target",
                        column: x => x.TargetLanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scripts_DesignedLanguages",
                columns: table => new
                {
                    ScriptId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ScriptId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Scripts_DesignedLanguages_Languages",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scripts_DesignedLanguages_Scripts",
                        column: x => x.ScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Writings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WritingType = table.Column<string>(type: "varchar(45)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Style = table.Column<int>(type: "int(11)", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ScriptId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TrueAuthorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    HandwritingSkill = table.Column<double>(nullable: false),
                    LiteracySkill = table.Column<double>(nullable: false),
                    ForgerySkill = table.Column<double>(nullable: false),
                    LanguageSkill = table.Column<double>(nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WritingColour = table.Column<long>(type: "bigint(20)", nullable: false),
                    ImplementType = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Writings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Writings_Characters_Author",
                        column: x => x.AuthorId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Writings_Languages",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Writings_Scripts",
                        column: x => x.ScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Writings_Characters_TrueAuthor",
                        column: x => x.TrueAuthorId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CharacterLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Command = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Time = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsPlayerCharacter = table.Column<bool>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterLog_Accounts",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLog_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveProjectLabours",
                columns: table => new
                {
                    ActiveProjectId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProjectLabourRequirementsId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Progress = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ActiveProjectId, x.ProjectLabourRequirementsId });
                    table.ForeignKey(
                        name: "FK_ActiveProjectLabours_ProjectLabourRequirements",
                        column: x => x.ProjectLabourRequirementsId,
                        principalTable: "ProjectLabourRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveProjectMaterials",
                columns: table => new
                {
                    ActiveProjectId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProjectMaterialRequirementsId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Progress = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ActiveProjectId, x.ProjectMaterialRequirementsId });
                    table.ForeignKey(
                        name: "FK_ActiveProjectMaterials_ProjectMaterialRequirements",
                        column: x => x.ProjectMaterialRequirementsId,
                        principalTable: "ProjectMaterialRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas_Rooms",
                columns: table => new
                {
                    AreaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RoomId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.AreaId, x.RoomId });
                    table.ForeignKey(
                        name: "FK_Areas_Rooms_Areas",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BodypartProto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BodypartType = table.Column<int>(type: "int(11)", nullable: false),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CountAsId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BodypartShapeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: true),
                    MaxLife = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'100'"),
                    SeveredThreshold = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'100'"),
                    PainModifier = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    BleedModifier = table.Column<double>(nullable: false, defaultValueSql: "'0.1'"),
                    RelativeHitChance = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'100'"),
                    Location = table.Column<int>(type: "int(11)", nullable: false),
                    Alignment = table.Column<int>(type: "int(11)", nullable: false),
                    Unary = table.Column<bool>(type: "bit(1)", nullable: true),
                    MaxSingleSize = table.Column<int>(type: "int(11)", nullable: true),
                    IsOrgan = table.Column<int>(type: "int(11)", nullable: false),
                    WeightLimit = table.Column<double>(nullable: false),
                    IsCore = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    StunModifier = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    DamageModifier = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    DefaultMaterialId = table.Column<long>(type: "bigint(20)", nullable: false, defaultValueSql: "'1'"),
                    Significant = table.Column<bool>(type: "bit(1)", nullable: false),
                    RelativeInfectability = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    HypoxiaDamagePerTick = table.Column<double>(nullable: false, defaultValueSql: "'0.2'"),
                    IsVital = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    ArmourTypeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ImplantSpace = table.Column<double>(nullable: false),
                    ImplantSpaceOccupied = table.Column<double>(nullable: false),
                    Size = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodypartProto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BodypartProto_ArmourTypes",
                        column: x => x.ArmourTypeId,
                        principalTable: "ArmourTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BodypartProto_BodypartShape",
                        column: x => x.BodypartShapeId,
                        principalTable: "BodypartShape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BodypartProto_BodypartProto",
                        column: x => x.CountAsId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BodypartProto_Materials",
                        column: x => x.DefaultMaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BodypartGroupDescribers_BodypartProtos",
                columns: table => new
                {
                    BodypartGroupDescriberId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Mandatory = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodypartGroupDescriberId, x.BodypartProtoId });
                    table.ForeignKey(
                        name: "FK_BGD_BodypartProtos_BodypartGroupDescribers",
                        column: x => x.BodypartGroupDescriberId,
                        principalTable: "BodypartGroupDescribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BGD_BodypartProtos_BodypartProto",
                        column: x => x.BodypartProtoId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BodypartInternalInfos",
                columns: table => new
                {
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    InternalPartId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsPrimaryOrganLocation = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    HitChance = table.Column<double>(nullable: false, defaultValueSql: "'5'"),
                    ProximityGroup = table.Column<string>(type: "varchar(45)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodypartProtoId, x.InternalPartId });
                    table.ForeignKey(
                        name: "FK_BodypartInternalInfos_BodypartProtos",
                        column: x => x.BodypartProtoId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BodypartInternalInfos_BodypartProtos_Internal",
                        column: x => x.InternalPartId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BodypartProto_AlignmentHits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Alignment = table.Column<int>(type: "int(11)", nullable: false),
                    HitChance = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodypartProto_AlignmentHits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BodypartProto_AlignmentHits_BodypartProto",
                        column: x => x.BodypartProtoId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BodypartProto_BodypartProto_Upstream",
                columns: table => new
                {
                    Child = table.Column<long>(type: "bigint(20)", nullable: false),
                    Parent = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Child, x.Parent });
                    table.ForeignKey(
                        name: "FKChild",
                        column: x => x.Child,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FKParent",
                        column: x => x.Parent,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BodypartProto_OrientationHits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Orientation = table.Column<int>(type: "int(11)", nullable: false),
                    HitChance = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodypartProto_OrientationHits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BodypartProto_OrientationHits_BodypartProto",
                        column: x => x.BodypartProtoId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BodyProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CountsAsId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WearSizeParameterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WielderDescriptionPlural = table.Column<string>(type: "varchar(4000)", nullable: false, defaultValueSql: "'hands'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WielderDescriptionSingle = table.Column<string>(type: "varchar(4000)", nullable: false, defaultValueSql: "'hand'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ConsiderString = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    StaminaRecoveryProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MinimumLegsToStand = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'2'"),
                    MinimumWingsToFly = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'2'"),
                    LegDescriptionSingular = table.Column<string>(type: "varchar(1000)", nullable: false, defaultValueSql: "'leg'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LegDescriptionPlural = table.Column<string>(type: "varchar(1000)", nullable: false, defaultValueSql: "'legs'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DefaultSmashingBodypartId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyProtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BodyPrototype_BodyPrototype",
                        column: x => x.CountsAsId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BodyPrototype_Bodyparts",
                        column: x => x.DefaultSmashingBodypartId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BodyPrototype_WearableSizeParameterRule",
                        column: x => x.WearSizeParameterId,
                        principalTable: "WearableSizeParameterRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BoneOrganCoverages",
                columns: table => new
                {
                    BoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OrganId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CoverageChance = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BoneId, x.OrganId });
                    table.ForeignKey(
                        name: "FK_BoneOrganCoverages_BodypartProto_Bone",
                        column: x => x.BoneId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoneOrganCoverages_BodypartProto_Organ",
                        column: x => x.OrganId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BodypartGroupDescribers_BodyProtos",
                columns: table => new
                {
                    BodypartGroupDescriberId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyProtoId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodypartGroupDescriberId, x.BodyProtoId });
                    table.ForeignKey(
                        name: "FK_BGD_BodyProtos_BodyProtos",
                        column: x => x.BodyProtoId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BGD_BodyProtos_BodypartGroupDescribers",
                        column: x => x.BodypartGroupDescriberId,
                        principalTable: "BodypartGroupDescribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BodyProtos_AdditionalBodyparts",
                columns: table => new
                {
                    BodyProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodypartId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Usage = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodyProtoId, x.BodypartId, x.Usage });
                    table.ForeignKey(
                        name: "FK_BodyProtos_AdditionalBodyparts_BodyProtos",
                        column: x => x.BodyProtoId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BodyProtos_AdditionalBodyparts_BodypartProto",
                        column: x => x.BodypartId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ButcheryProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    TargetBodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsPelt = table.Column<bool>(type: "bit(1)", nullable: false),
                    Subcategory = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CanProduceProgId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButcheryProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ButcheryProducts_FutureProgs",
                        column: x => x.CanProduceProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ButcheryProducts_BodyProtos",
                        column: x => x.TargetBodyId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Limbs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    RootBodypartId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LimbType = table.Column<int>(type: "int(11)", nullable: false),
                    RootBodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LimbDamageThresholdMultiplier = table.Column<double>(nullable: false),
                    LimbPainThresholdMultiplier = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Limbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Limbs_BodyProtos",
                        column: x => x.RootBodyId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Limbs_BodypartProto",
                        column: x => x.RootBodypartId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MoveSpeeds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BodyProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Multiplier = table.Column<double>(nullable: false),
                    Alias = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FirstPersonVerb = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ThirdPersonVerb = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PresentParticiple = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PositionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    StaminaMultiplier = table.Column<double>(nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveSpeeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoveSpeeds_BodyPrototype",
                        column: x => x.BodyProtoId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Races",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Description = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BaseBodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AllowedGenders = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ParentRaceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AttributeBonusProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AttributeTotalCap = table.Column<int>(type: "int(11)", nullable: false),
                    IndividualAttributeCap = table.Column<int>(type: "int(11)", nullable: false),
                    DiceExpression = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    IlluminationPerceptionMultiplier = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    AvailabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CorpseModelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DefaultHealthStrategyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CanUseWeapons = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    CanAttack = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    CanDefend = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    NaturalArmourTypeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NaturalArmourQuality = table.Column<long>(type: "bigint(20)", nullable: false),
                    NaturalArmourMaterialId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BloodLiquidId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NeedsToBreathe = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    SweatLiquidId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SweatRateInLitresPerMinute = table.Column<double>(nullable: false, defaultValueSql: "'0.8'"),
                    SizeStanding = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'6'"),
                    SizeProne = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'5'"),
                    SizeSitting = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'6'"),
                    CommunicationStrategyType = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'humanoid'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DefaultHandedness = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'3'"),
                    HandednessOptions = table.Column<string>(type: "varchar(100)", nullable: false, defaultValueSql: "'1 3'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MaximumDragWeightExpression = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MaximumLiftWeightExpression = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RaceButcheryProfileId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BloodModelId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RaceUsesStamina = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    CanEatCorpses = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    BiteWeight = table.Column<double>(nullable: false, defaultValueSql: "'1000'"),
                    EatCorpseEmoteText = table.Column<string>(type: "varchar(500)", nullable: false, defaultValueSql: "'@ eat|eats {0}$1'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CanEatMaterialsOptIn = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    TemperatureRangeFloor = table.Column<double>(nullable: false),
                    TemperatureRangeCeiling = table.Column<double>(nullable: false, defaultValueSql: "'40'"),
                    BodypartSizeModifier = table.Column<int>(type: "int(11)", nullable: false),
                    BodypartHealthMultiplier = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    BreathingVolumeExpression = table.Column<string>(type: "varchar(500)", nullable: false, defaultValueSql: "'7'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    HoldBreathLengthExpression = table.Column<string>(type: "varchar(500)", nullable: false, defaultValueSql: "'120'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CanClimb = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    CanSwim = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    MinimumSleepingPosition = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'4'"),
                    ChildAge = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'3'"),
                    YouthAge = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'10'"),
                    YoungAdultAge = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'16'"),
                    AdultAge = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'21'"),
                    ElderAge = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'55'"),
                    VenerableAge = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'75'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Races", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Races_AttributeBonusProg",
                        column: x => x.AttributeBonusProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_AvailabilityProg",
                        column: x => x.AvailabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Races_BodyProtos",
                        column: x => x.BaseBodyId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_Liquids_Blood",
                        column: x => x.BloodLiquidId,
                        principalTable: "Liquids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_BloodModels",
                        column: x => x.BloodModelId,
                        principalTable: "BloodModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Races_CorpseModels",
                        column: x => x.CorpseModelId,
                        principalTable: "CorpseModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_HealthStrategies",
                        column: x => x.DefaultHealthStrategyId,
                        principalTable: "HealthStrategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_Materials",
                        column: x => x.NaturalArmourMaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Races_ArmourTypes",
                        column: x => x.NaturalArmourTypeId,
                        principalTable: "ArmourTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Races_Races",
                        column: x => x.ParentRaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_RaceButcheryProfiles",
                        column: x => x.RaceButcheryProfileId,
                        principalTable: "RaceButcheryProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Races_Liqiuds_Sweat",
                        column: x => x.SweatLiquidId,
                        principalTable: "Liquids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ButcheryProductItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ButcheryProductId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NormalProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DamagedProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NormalQuantity = table.Column<int>(type: "int(11)", nullable: false),
                    DamagedQuantity = table.Column<int>(type: "int(11)", nullable: false),
                    ButcheryProductItemscol = table.Column<string>(type: "varchar(45)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    DamageThreshold = table.Column<double>(nullable: false, defaultValueSql: "'10'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButcheryProductItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ButcheryProductItems_ButcheryProducts",
                        column: x => x.ButcheryProductId,
                        principalTable: "ButcheryProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ButcheryProducts_BodypartProtos",
                columns: table => new
                {
                    ButcheryProductId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ButcheryProductId, x.BodypartProtoId });
                    table.ForeignKey(
                        name: "FK_ButcheryProducts_BodypartProtos_BodypartProtos",
                        column: x => x.BodypartProtoId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ButcheryProducts_BodypartProtos_ButcheryProducts",
                        column: x => x.ButcheryProductId,
                        principalTable: "ButcheryProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceButcheryProfiles_ButcheryProducts",
                columns: table => new
                {
                    RaceButcheryProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ButcheryProductId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceButcheryProfileId, x.ButcheryProductId });
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts",
                        column: x => x.ButcheryProductId,
                        principalTable: "ButcheryProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaceButcheryProfiles_ButcheryProducts_RaceButcheryProfiles",
                        column: x => x.RaceButcheryProfileId,
                        principalTable: "RaceButcheryProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Limbs_BodypartProto",
                columns: table => new
                {
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LimbId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodypartProtoId, x.LimbId });
                    table.ForeignKey(
                        name: "FK_Limbs_BodypartProto_BodypartProto",
                        column: x => x.BodypartProtoId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Limbs_BodypartProto_Limbs",
                        column: x => x.LimbId,
                        principalTable: "Limbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Limbs_SpinalParts",
                columns: table => new
                {
                    LimbId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LimbId, x.BodypartProtoId });
                    table.ForeignKey(
                        name: "FK_Limbs_SpinalParts_BodypartProtos",
                        column: x => x.BodypartProtoId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Limbs_SpinalParts_Limbs",
                        column: x => x.LimbId,
                        principalTable: "Limbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargenAdvices_Races",
                columns: table => new
                {
                    ChargenAdviceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenAdviceId, x.RaceId });
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_Races_ChargenAdvices",
                        column: x => x.ChargenAdviceId,
                        principalTable: "ChargenAdvices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_Races_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ethnicities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ChargenBlurb = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AvailabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ParentRaceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    EthnicGroup = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    EthnicSubgroup = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PopulationBloodModelId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TolerableTemperatureFloorEffect = table.Column<double>(nullable: false),
                    TolerableTemperatureCeilingEffect = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ethnicities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ethnicities_AvailabilityProg",
                        column: x => x.AvailabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ethnicities_Races",
                        column: x => x.ParentRaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ethnicities_PopulationBloodModels",
                        column: x => x.PopulationBloodModelId,
                        principalTable: "PopulationBloodModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RaceEdibleForagableYields",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    YieldType = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BiteYield = table.Column<double>(nullable: false),
                    CaloriesPerYield = table.Column<double>(nullable: false),
                    HungerPerYield = table.Column<double>(nullable: false),
                    WaterPerYield = table.Column<double>(nullable: false),
                    ThirstPerYield = table.Column<double>(nullable: false),
                    AlcoholPerYield = table.Column<double>(nullable: false),
                    EatEmote = table.Column<string>(type: "varchar(1000)", nullable: false, defaultValueSql: "'@ eat|eats {0} from the location.'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.YieldType });
                    table.ForeignKey(
                        name: "FK_RaceEdibleForagableYields_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races_AdditionalBodyparts",
                columns: table => new
                {
                    Usage = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    BodypartId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Usage, x.RaceId, x.BodypartId });
                    table.ForeignKey(
                        name: "FK_Races_AdditionalBodyparts_BodypartProto",
                        column: x => x.BodypartId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_AdditionalBodyparts_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races_AdditionalCharacteristics",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacteristicDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Usage = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.CharacteristicDefinitionId });
                    table.ForeignKey(
                        name: "FK_RAC_CharacteristicDefinitions",
                        column: x => x.CharacteristicDefinitionId,
                        principalTable: "CharacteristicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RAC_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races_Attributes",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AttributeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsHealthAttribute = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.AttributeId });
                    table.ForeignKey(
                        name: "FK_Races_Attributes_TraitDefinitions",
                        column: x => x.AttributeId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_Attributes_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races_BreathableGases",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GasId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Multiplier = table.Column<double>(nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.GasId });
                    table.ForeignKey(
                        name: "FK_Races_BreathableGases_Gases",
                        column: x => x.GasId,
                        principalTable: "Gases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_BreathableGases_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races_BreathableLiquids",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LiquidId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Multiplier = table.Column<double>(nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.LiquidId });
                    table.ForeignKey(
                        name: "FK_Races_BreathableLiquids_Liquids",
                        column: x => x.LiquidId,
                        principalTable: "Liquids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_BreathableLiquids_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races_ChargenResources",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequirementOnly = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    Amount = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.ChargenResourceId, x.RequirementOnly });
                    table.ForeignKey(
                        name: "FK_Races_ChargenResources_ChargenResources",
                        column: x => x.ChargenResourceId,
                        principalTable: "ChargenResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_ChargenResources_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races_EdibleMaterials",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MaterialId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CaloriesPerKilogram = table.Column<double>(nullable: false),
                    HungerPerKilogram = table.Column<double>(nullable: false),
                    ThirstPerKilogram = table.Column<double>(nullable: false),
                    WaterPerKilogram = table.Column<double>(nullable: false),
                    AlcoholPerKilogram = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_Races_EdibleMaterials_Materials",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_EdibleMaterials_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races_WeaponAttacks",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WeaponAttackId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodypartId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Quality = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.WeaponAttackId, x.BodypartId });
                    table.ForeignKey(
                        name: "FK_Races_WeaponAttacks_BodypartProto",
                        column: x => x.BodypartId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_WeaponAttacks_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_WeaponAttacks_WeaponAttacks",
                        column: x => x.WeaponAttackId,
                        principalTable: "WeaponAttacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bodies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BodyPrototypeID = table.Column<long>(type: "bigint(20)", nullable: false),
                    Height = table.Column<double>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    Position = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrentSpeed = table.Column<long>(type: "bigint(20)", nullable: true),
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrentStamina = table.Column<double>(nullable: false),
                    CurrentBloodVolume = table.Column<double>(nullable: false, defaultValueSql: "'-1'"),
                    EthnicityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BloodtypeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Gender = table.Column<short>(type: "smallint(6)", nullable: false),
                    ShortDescription = table.Column<string>(type: "varchar(1000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    FullDescription = table.Column<string>(type: "varchar(4000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShortDescriptionPatternId = table.Column<long>(type: "bigint(20)", nullable: true),
                    FullDescriptionPatternId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Tattoos = table.Column<string>(type: "mediumtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    HeldBreathLength = table.Column<int>(type: "int(11)", nullable: false),
                    EffectData = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Scars = table.Column<string>(type: "mediumtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bodies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bodies_Bloodtypes",
                        column: x => x.BloodtypeId,
                        principalTable: "Bloodtypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bodies_Ethnicities",
                        column: x => x.EthnicityId,
                        principalTable: "Ethnicities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bodies_EntityDescriptionPatterns_Full",
                        column: x => x.FullDescriptionPatternId,
                        principalTable: "EntityDescriptionPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bodies_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bodies_EntityDescriptionPatterns_Short",
                        column: x => x.ShortDescriptionPatternId,
                        principalTable: "EntityDescriptionPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChargenAdvices_Ethnicities",
                columns: table => new
                {
                    ChargenAdviceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EthnicityId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ChargenAdviceId, x.EthnicityId });
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_Ethnicities_ChargenAdvices",
                        column: x => x.ChargenAdviceId,
                        principalTable: "ChargenAdvices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargenAdvices_Ethnicities_Ethnicities",
                        column: x => x.EthnicityId,
                        principalTable: "Ethnicities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ethnicities_Characteristics",
                columns: table => new
                {
                    EthnicityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacteristicDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacteristicProfileId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EthnicityId, x.CharacteristicDefinitionId, x.CharacteristicProfileId });
                    table.ForeignKey(
                        name: "FK_Ethnicities_Characteristics_CharacteristicDefinitions",
                        column: x => x.CharacteristicDefinitionId,
                        principalTable: "CharacteristicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ethnicities_Characteristics_CharacteristicProfiles",
                        column: x => x.CharacteristicProfileId,
                        principalTable: "CharacteristicProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ethnicities_Characteristics_Ethnicities",
                        column: x => x.EthnicityId,
                        principalTable: "Ethnicities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ethnicities_ChargenResources",
                columns: table => new
                {
                    EthnicityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChargenResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RequirementOnly = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    Amount = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EthnicityId, x.ChargenResourceId, x.RequirementOnly });
                    table.ForeignKey(
                        name: "FK_Ethnicities_ChargenResources_ChargenResources",
                        column: x => x.ChargenResourceId,
                        principalTable: "ChargenResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ethnicities_ChargenResources_Ethnicities",
                        column: x => x.EthnicityId,
                        principalTable: "Ethnicities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bodies_DrugDoses",
                columns: table => new
                {
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DrugId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Active = table.Column<bool>(type: "bit(1)", nullable: false),
                    Grams = table.Column<double>(nullable: false),
                    OriginalVector = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodyId, x.DrugId, x.Active });
                    table.ForeignKey(
                        name: "FK_Bodies_DrugDoses_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bodies_DrugDoses_Drugs",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bodies_GameItems",
                columns: table => new
                {
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EquippedOrder = table.Column<int>(type: "int(11)", nullable: false),
                    WearProfile = table.Column<long>(type: "bigint(20)", nullable: true),
                    Wielded = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodyId, x.GameItemId });
                    table.ForeignKey(
                        name: "FK_Bodies_GameItems_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bodies_GameItems_GameItems",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bodies_Implants",
                columns: table => new
                {
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ImplantId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodyId, x.ImplantId });
                    table.ForeignKey(
                        name: "FK_Bodies_Implants_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bodies_Implants_GameItems",
                        column: x => x.ImplantId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bodies_Prosthetics",
                columns: table => new
                {
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProstheticId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodyId, x.ProstheticId });
                    table.ForeignKey(
                        name: "FK_Bodies_Prosthetics_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bodies_Prosthetics_GameItems",
                        column: x => x.ProstheticId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bodies_SeveredParts",
                columns: table => new
                {
                    BodiesId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodiesId, x.BodypartProtoId });
                    table.ForeignKey(
                        name: "FK_Bodies_SeveredParts_Bodies",
                        column: x => x.BodiesId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bodies_SeveredParts_BodypartProtos",
                        column: x => x.BodypartProtoId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characteristics",
                columns: table => new
                {
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Type = table.Column<int>(type: "int(11)", nullable: false),
                    CharacteristicId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodyId, x.Type });
                    table.ForeignKey(
                        name: "FK_Characteristics_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characteristics_CharacteristicValues",
                        column: x => x.CharacteristicId,
                        principalTable: "CharacteristicValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerceiverMerits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MeritId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerceiverMerits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerceiverMerits_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerceiverMerits_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: " FK_PerceiverMerits_Merits",
                        column: x => x.MeritId,
                        principalTable: "Merits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Traits",
                columns: table => new
                {
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Value = table.Column<double>(nullable: false),
                    AdditionalValue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodyId, x.TraitDefinitionId });
                    table.ForeignKey(
                        name: "FK_Traits_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Traits_TraitDefinitions",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wounds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OriginalDamage = table.Column<double>(nullable: false),
                    CurrentDamage = table.Column<double>(nullable: false),
                    CurrentPain = table.Column<double>(nullable: false),
                    CurrentShock = table.Column<double>(nullable: false),
                    CurrentStun = table.Column<double>(nullable: false),
                    LodgedItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DamageType = table.Column<int>(type: "int(11)", nullable: false),
                    Internal = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    BodypartProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ExtraInformation = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci"),
                    ActorOriginId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ToolOriginId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WoundType = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wounds_Characters",
                        column: x => x.ActorOriginId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Wounds_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wounds_GameItemOwner",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wounds_GameItems",
                        column: x => x.LodgedItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Wounds_GameItems_Tool",
                        column: x => x.ToolOriginId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Infections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InfectionType = table.Column<int>(type: "int(11)", nullable: false),
                    Virulence = table.Column<int>(type: "int(11)", nullable: false),
                    Intensity = table.Column<double>(nullable: false),
                    OwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WoundId = table.Column<long>(type: "bigint(20)", nullable: true),
                    BodypartId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Immunity = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Infections_Bodyparts",
                        column: x => x.BodypartId,
                        principalTable: "BodypartProto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Infections_Bodies",
                        column: x => x.OwnerId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Infections_Wounds",
                        column: x => x.WoundId,
                        principalTable: "Wounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hooks_Perceivables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HookId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ZoneId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ShardId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hooks_Perceivables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hooks_Perceivables_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hooks_Perceivables_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hooks_Perceivables_GameItems",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hooks_Perceivables_Hooks",
                        column: x => x.HookId,
                        principalTable: "Hooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hooks_Perceivables_Shards",
                        column: x => x.ShardId,
                        principalTable: "Shards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EconomicZones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    PreviousFinancialPeriodsToKeep = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'50'"),
                    ZoneForTimePurposesId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PermitTaxableLosses = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    OutstandingTaxesOwed = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrentFinancialPeriodId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ReferenceCalendarId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ReferenceClockId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReferenceTime = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    IntervalType = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'2'"),
                    IntervalModifier = table.Column<int>(type: "int(11)", nullable: false),
                    IntervalAmount = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EconomicZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EconomicZones_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EconomicZones_Calendars",
                        column: x => x.ReferenceCalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EconomicZones_Clocks",
                        column: x => x.ReferenceClockId,
                        principalTable: "Clocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EconomicZones_Timezones",
                        column: x => x.ReferenceClockId,
                        principalTable: "Timezones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialPeriods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime", nullable: false),
                    MudPeriodStart = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MudPeriodEnd = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialPeriods_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EconomicZoneRevenues",
                columns: table => new
                {
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FinancialPeriodId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TotalTaxRevenue = table.Column<decimal>(type: "decimal(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EconomicZoneId, x.FinancialPeriodId });
                    table.ForeignKey(
                        name: "FK_EconomicZoneRevenues",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EconomicZoneRevenues_FinancialPeriods",
                        column: x => x.FinancialPeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CellOverlays",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CellName = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CellDescription = table.Column<string>(type: "varchar(4000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CellOverlayPackageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellOverlayPackageRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    TerrainId = table.Column<long>(type: "bigint(20)", nullable: false),
                    HearingProfileId = table.Column<long>(type: "bigint(20)", nullable: true),
                    OutdoorsType = table.Column<int>(type: "int(11)", nullable: false),
                    AmbientLightFactor = table.Column<double>(nullable: false, defaultValueSql: "'1'"),
                    AddedLight = table.Column<double>(nullable: false),
                    AtmosphereId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AtmosphereType = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'gas'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellOverlays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CellOverlays_HearingProfiles",
                        column: x => x.HearingProfileId,
                        principalTable: "HearingProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CellOverlays_Terrains",
                        column: x => x.TerrainId,
                        principalTable: "Terrains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CellOverlays_CellOverlayPackages",
                        columns: x => new { x.CellOverlayPackageId, x.CellOverlayPackageRevisionNumber },
                        principalTable: "CellOverlayPackages",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CellOverlays_Exits",
                columns: table => new
                {
                    CellOverlayId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ExitId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CellOverlayId, x.ExitId });
                    table.ForeignKey(
                        name: "FK_CellOverlays_Exits_CellOverlays",
                        column: x => x.CellOverlayId,
                        principalTable: "CellOverlays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CellOverlays_Exits_Exits",
                        column: x => x.ExitId,
                        principalTable: "Exits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoomId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrentOverlayId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ForagableProfileId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Temporary = table.Column<bool>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    EffectData = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cells_CellOverlays",
                        column: x => x.CurrentOverlayId,
                        principalTable: "CellOverlays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ActiveProjects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProjectRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    CurrentPhaseId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveProjects_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActiveProjects_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActiveProjects_ProjectPhases",
                        column: x => x.CurrentPhaseId,
                        principalTable: "ProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveProjects_Projects",
                        columns: x => new { x.ProjectId, x.ProjectRevisionNumber },
                        principalTable: "Projects",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells_ForagableYields",
                columns: table => new
                {
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ForagableType = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .Annotation("MySql:Collation", "utf8mb4_unicode_ci"),
                    Yield = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CellId, x.ForagableType });
                    table.ForeignKey(
                        name: "FK_Cells_ForagableYields_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells_GameItems",
                columns: table => new
                {
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CellId, x.GameItemId });
                    table.ForeignKey(
                        name: "FK_Cells_GameItems_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cells_GameItems_GameItems",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells_MagicResources",
                columns: table => new
                {
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MagicResourceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CellId, x.MagicResourceId });
                    table.ForeignKey(
                        name: "FK_Cells_MagicResources_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cells_MagicResources_MagicResources",
                        column: x => x.MagicResourceId,
                        principalTable: "MagicResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells_RangedCovers",
                columns: table => new
                {
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RangedCoverId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CellId, x.RangedCoverId });
                    table.ForeignKey(
                        name: "FK_Cells_RangedCovers_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cells_RangedCovers_RangedCovers",
                        column: x => x.RangedCoverId,
                        principalTable: "RangedCovers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells_Tags",
                columns: table => new
                {
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TagId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CellId, x.TagId });
                    table.ForeignKey(
                        name: "FK_Cells_Tags_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cells_Tags_Tags",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clans_AdministrationCells",
                columns: table => new
                {
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ClanId, x.CellId });
                    table.ForeignKey(
                        name: "FK_Clans_AdministrationCells_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clans_AdministrationCells_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clans_TreasuryCells",
                columns: table => new
                {
                    ClanId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ClanId, x.CellId });
                    table.ForeignKey(
                        name: "FK_Clans_TreasuryCells_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clans_TreasuryCells_Clans",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Crimes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LawId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CriminalId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VictimId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TimeOfCrime = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    RealTimeOfCrime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LocationId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TimeOfReport = table.Column<string>(type: "varchar(200)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    AccuserId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CriminalShortDescription = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CriminalFullDescription = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    CriminalCharacteristics = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    IsKnownCrime = table.Column<bool>(type: "bit(1)", nullable: false),
                    IsStaleCrime = table.Column<bool>(type: "bit(1)", nullable: false),
                    IsFinalised = table.Column<bool>(type: "bit(1)", nullable: false),
                    ConvictionRecorded = table.Column<bool>(type: "bit(1)", nullable: false),
                    IsCriminalIdentityKnown = table.Column<bool>(type: "bit(1)", nullable: false),
                    BailHasBeenPosted = table.Column<bool>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Crimes_Accuser",
                        column: x => x.AccuserId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Crimes_Criminal",
                        column: x => x.CriminalId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Crimes_Laws",
                        column: x => x.LawId,
                        principalTable: "Laws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Crimes_Location",
                        column: x => x.LocationId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Crimes_Victim",
                        column: x => x.VictimId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    WorkshopCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    StockroomCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CanShopProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WhyCannotShopProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsTrading = table.Column<bool>(type: "bit(1)", nullable: false),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EmployeeRecords = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shops_FutureProgs_Can",
                        column: x => x.CanShopProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Shops_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shops_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shops_Cells_Stockroom",
                        column: x => x.StockroomCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Shops_FutureProgs_WhyCant",
                        column: x => x.WhyCannotShopProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Shops_Cells_Workshop",
                        column: x => x.WorkshopCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShardId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Elevation = table.Column<double>(nullable: false),
                    DefaultCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AmbientLightPollution = table.Column<double>(nullable: false),
                    ForagableProfileId = table.Column<long>(type: "bigint(20)", nullable: true),
                    WeatherControllerId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zones_Cells",
                        column: x => x.DefaultCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zones_Shards",
                        column: x => x.ShardId,
                        principalTable: "Shards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zones_WeatherControllers",
                        column: x => x.WeatherControllerId,
                        principalTable: "WeatherControllers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EconomicZoneShopTaxes",
                columns: table => new
                {
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OutstandingProfitTaxes = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    OutstandingSalesTaxes = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    TaxesInCredits = table.Column<decimal>(type: "decimal(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EconomicZoneId, x.ShopId });
                    table.ForeignKey(
                        name: "FK_EconomicZoneShopTaxes_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EconomicZoneShopTaxes_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Merchandises",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AutoReordering = table.Column<bool>(type: "bit(1)", nullable: false),
                    AutoReorderPrice = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    DefaultMerchandiseForItem = table.Column<bool>(type: "bit(1)", nullable: false),
                    ItemProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PreferredDisplayContainerId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ListDescription = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    MinimumStockLevels = table.Column<int>(type: "int(11)", nullable: false),
                    MinimumStockLevelsByWeight = table.Column<double>(nullable: false),
                    PreserveVariablesOnReorder = table.Column<bool>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchandises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Merchandises_GameItems",
                        column: x => x.PreferredDisplayContainerId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Merchandises_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopFinancialPeriodResults",
                columns: table => new
                {
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    FinancialPeriodId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GrossRevenue = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    NetRevenue = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    SalesTax = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    ProfitsTax = table.Column<decimal>(type: "decimal(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EconomicZoneId, x.ShopId, x.FinancialPeriodId });
                    table.ForeignKey(
                        name: "FK_ShopFinancialPeriodResults_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopFinancialPeriodResults_FinancialPeriods",
                        column: x => x.FinancialPeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopFinancialPeriodResults_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shops_StoreroomCells",
                columns: table => new
                {
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ShopId, x.CellId });
                    table.ForeignKey(
                        name: "FK_Shops_StoreroomCells_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shops_StoreroomCells_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopsTills",
                columns: table => new
                {
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GameItemId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ShopId, x.GameItemId });
                    table.ForeignKey(
                        name: "FK_ShopTills_GameItems",
                        column: x => x.GameItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopTills_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopTransactionRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CurrencyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PretaxValue = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(10,0)", nullable: false),
                    TransactionType = table.Column<int>(type: "int(11)", nullable: false),
                    ShopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ThirdPartyId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RealDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    MudDateTime = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopTransactionRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopTransactionRecords_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopTransactionRecords_Shops",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalAuthorities_Zones",
                columns: table => new
                {
                    ZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ZoneId, x.LegalAuthorityId });
                    table.ForeignKey(
                        name: "FK_LegalAuthorities_Zones_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalAuthorities_Zones_Zones",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    X = table.Column<int>(type: "int(11)", nullable: false),
                    Y = table.Column<int>(type: "int(11)", nullable: false),
                    Z = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Zones",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zones_Timezones",
                columns: table => new
                {
                    ZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ClockId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TimezoneId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ZoneId, x.ClockId, x.TimezoneId });
                    table.ForeignKey(
                        name: "FK_Zones_Timezones_Zones",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "FK_Accents_Languages",
                table: "Accents",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_AccountNotes_Accounts",
                table: "AccountNotes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "FK_AccountNotes_Author",
                table: "AccountNotes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "FK_Accounts_AuthorityGroups",
                table: "Accounts",
                column: "AuthorityGroupId");

            migrationBuilder.CreateIndex(
                name: "FK_Accounts_ChargenResources_ChargenResources",
                table: "Accounts_ChargenResources",
                column: "ChargenResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_ActiveProjectLabours_ProjectLabourRequirements_idx",
                table: "ActiveProjectLabours",
                column: "ProjectLabourRequirementsId");

            migrationBuilder.CreateIndex(
                name: "FK_ActiveProjectMaterials_ProjectMaterialRequirements_idx",
                table: "ActiveProjectMaterials",
                column: "ProjectMaterialRequirementsId");

            migrationBuilder.CreateIndex(
                name: "FK_ActiveProjects_Cells_idx",
                table: "ActiveProjects",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_ActiveProjects_Characters_idx",
                table: "ActiveProjects",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ActiveProjects_ProjectPhases_idx",
                table: "ActiveProjects",
                column: "CurrentPhaseId");

            migrationBuilder.CreateIndex(
                name: "FK_ActiveProjects_Projects_idx",
                table: "ActiveProjects",
                columns: new[] { "ProjectId", "ProjectRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_Allies_Characters_Target_idx",
                table: "Allies",
                column: "AllyId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_Clans",
                table: "Appointments",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_Ranks",
                table: "Appointments",
                column: "MinimumRankId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_Ranks_2",
                table: "Appointments",
                column: "MinimumRankToAppointId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_ParentAppointment",
                table: "Appointments",
                column: "ParentAppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_Paygrades",
                table: "Appointments",
                column: "PaygradeId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_GameItemProtos",
                table: "Appointments",
                columns: new[] { "InsigniaGameItemId", "InsigniaGameItemRevnum" });

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_Abbreviations_Appointments",
                table: "Appointments_Abbreviations",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_Abbreviations_FutureProgs",
                table: "Appointments_Abbreviations",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_Titles_Appointments",
                table: "Appointments_Titles",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_Appointments_Titles_FutureProgs",
                table: "Appointments_Titles",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Areas_WeatherControllers_idx",
                table: "Areas",
                column: "WeatherControllerId");

            migrationBuilder.CreateIndex(
                name: "FK_Areas_Rooms_Rooms_idx",
                table: "Areas_Rooms",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "FK_Bans_Accounts",
                table: "Bans",
                column: "BannerAccountId");

            migrationBuilder.CreateIndex(
                name: "FK_BloodModels_Bloodtypes_Bloodtypes_idx",
                table: "BloodModels_Bloodtypes",
                column: "BloodtypeId");

            migrationBuilder.CreateIndex(
                name: "FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens_idx",
                table: "Bloodtypes_BloodtypeAntigens",
                column: "BloodtypeAntigenId");

            migrationBuilder.CreateIndex(
                name: "FK_BoardsPosts_Accounts_idx",
                table: "BoardPosts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "FK_BoardPosts_Boards_idx",
                table: "BoardPosts",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_Bloodtypes_idx",
                table: "Bodies",
                column: "BloodtypeId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_Ethnicities_idx",
                table: "Bodies",
                column: "EthnicityId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_EntityDescriptionPatterns_Full_idx",
                table: "Bodies",
                column: "FullDescriptionPatternId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_Races",
                table: "Bodies",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_EntityDescriptionPatterns_Short_idx",
                table: "Bodies",
                column: "ShortDescriptionPatternId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_DrugDoses_Drugs_idx",
                table: "Bodies_DrugDoses",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_GameItems_GameItems",
                table: "Bodies_GameItems",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_Implants_GameItems_idx",
                table: "Bodies_Implants",
                column: "ImplantId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_Prosthetics_GameItems_idx",
                table: "Bodies_Prosthetics",
                column: "ProstheticId");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_SeveredParts_BodypartProtos_idx",
                table: "Bodies_SeveredParts",
                column: "BodypartProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_BGD_BodypartProtos_BodypartProto",
                table: "BodypartGroupDescribers_BodypartProtos",
                column: "BodypartProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_BGD_BodyProtos_BodyProtos",
                table: "BodypartGroupDescribers_BodyProtos",
                column: "BodyProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_BGD_ShapeCount_BodypartShape",
                table: "BodypartGroupDescribers_ShapeCount",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartInternalInfos_BodypartProtos_Internal_idx",
                table: "BodypartInternalInfos",
                column: "InternalPartId");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartProto_ArmourTypes_idx",
                table: "BodypartProto",
                column: "ArmourTypeId");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartProto_BodyPrototype",
                table: "BodypartProto",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartProto_BodypartShape",
                table: "BodypartProto",
                column: "BodypartShapeId");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartProto_BodypartProto_idx",
                table: "BodypartProto",
                column: "CountAsId");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartProto_Materials_idx",
                table: "BodypartProto",
                column: "DefaultMaterialId");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartProto_AlignmentHits_BodypartProto",
                table: "BodypartProto_AlignmentHits",
                column: "BodypartProtoId");

            migrationBuilder.CreateIndex(
                name: "FKParent",
                table: "BodypartProto_BodypartProto_Upstream",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "FK_BodypartProto_OrientationHits_BodypartProto",
                table: "BodypartProto_OrientationHits",
                column: "BodypartProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_BodyPrototype_BodyPrototype_idx",
                table: "BodyProtos",
                column: "CountsAsId");

            migrationBuilder.CreateIndex(
                name: "FK_BodyPrototype_Bodyparts_idx",
                table: "BodyProtos",
                column: "DefaultSmashingBodypartId");

            migrationBuilder.CreateIndex(
                name: "FK_BodyPrototype_WearableSizeParameterRule",
                table: "BodyProtos",
                column: "WearSizeParameterId");

            migrationBuilder.CreateIndex(
                name: "FK_BodyProtos_AdditionalBodyparts_BodypartProto",
                table: "BodyProtos_AdditionalBodyparts",
                column: "BodypartId");

            migrationBuilder.CreateIndex(
                name: "FK_BoneOrganCoverages_BodypartProto_Organ_idx",
                table: "BoneOrganCoverages",
                column: "OrganId");

            migrationBuilder.CreateIndex(
                name: "FK_ButcheryProductItems_ButcheryProducts_idx",
                table: "ButcheryProductItems",
                column: "ButcheryProductId");

            migrationBuilder.CreateIndex(
                name: "FK_ButcheryProducts_FutureProgs_idx",
                table: "ButcheryProducts",
                column: "CanProduceProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ButcheryProducts_BodyProtos_idx",
                table: "ButcheryProducts",
                column: "TargetBodyId");

            migrationBuilder.CreateIndex(
                name: "FK_ButcheryProducts_BodypartProtos_BodypartProtos_idx",
                table: "ButcheryProducts_BodypartProtos",
                column: "BodypartProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_CellOverlayPackages_EditableItems",
                table: "CellOverlayPackages",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_CellOverlays_Cells",
                table: "CellOverlays",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_CellOverlays_HearingProfiles",
                table: "CellOverlays",
                column: "HearingProfileId");

            migrationBuilder.CreateIndex(
                name: "FK_CellOverlays_Terrains",
                table: "CellOverlays",
                column: "TerrainId");

            migrationBuilder.CreateIndex(
                name: "FK_CellOverlays_CellOverlayPackages",
                table: "CellOverlays",
                columns: new[] { "CellOverlayPackageId", "CellOverlayPackageRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_CellOverlays_Exits_Exits",
                table: "CellOverlays_Exits",
                column: "ExitId");

            migrationBuilder.CreateIndex(
                name: "FK_Cells_CellOverlays",
                table: "Cells",
                column: "CurrentOverlayId");

            migrationBuilder.CreateIndex(
                name: "FK_Cells_Rooms",
                table: "Cells",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "FK_Cells_GameItems_GameItems",
                table: "Cells_GameItems",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_Cells_MagicResources_MagicResources_idx",
                table: "Cells_MagicResources",
                column: "MagicResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_Cells_RangedCovers_RangedCovers_idx",
                table: "Cells_RangedCovers",
                column: "RangedCoverId");

            migrationBuilder.CreateIndex(
                name: "FK_Cells_Tags_Tags_idx",
                table: "Cells_Tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "FK_ChannelCommandWords_Channels",
                table: "ChannelCommandWords",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "FK_ChannelIgnorers_Accounts",
                table: "ChannelIgnorers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "FK_Channels_FutureProgs_Listener",
                table: "Channels",
                column: "ChannelListenerProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Channels_FutureProgs_Speaker",
                table: "Channels",
                column: "ChannelSpeakerProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterCombatSettings_FutureProgs_idx",
                table: "CharacterCombatSettings",
                column: "AvailabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterCombatSettings_Characters_idx",
                table: "CharacterCombatSettings",
                column: "CharacterOwnerId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterIntroTemplates_FutureProgs_idx",
                table: "CharacterIntroTemplates",
                column: "AppliesToCharacterProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacteristicDefinitions_Parent",
                table: "CharacteristicDefinitions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacteristicProfiles_CharacteristicDefinitions",
                table: "CharacteristicProfiles",
                column: "TargetDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_Characteristics_CharacteristicValues",
                table: "Characteristics",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacteristicValues_CharacteristicDefinitions",
                table: "CharacteristicValues",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacteristicValues_FutureProgs",
                table: "CharacteristicValues",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CHARACTERKNOWLEDGES_CHARACTERS",
                table: "CharacterKnowledges",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_CHARACTERKNOWLEDGES_KNOWLEDGES_idx",
                table: "CharacterKnowledges",
                column: "KnowledgeId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterLog_Accounts_idx",
                table: "CharacterLog",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterLog_Cells_idx",
                table: "CharacterLog",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_CharacterLog_Characters_idx",
                table: "CharacterLog",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Accounts",
                table: "Characters",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Bodies",
                table: "Characters",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Chargens_idx",
                table: "Characters",
                column: "ChargenId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Cultures",
                table: "Characters",
                column: "CultureId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Currencies",
                table: "Characters",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Accents_idx",
                table: "Characters",
                column: "CurrentAccentId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Languages_idx",
                table: "Characters",
                column: "CurrentLanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_ActiveProjects_idx",
                table: "Characters",
                column: "CurrentProjectId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_ProjectLabourRequirements_idx",
                table: "Characters",
                column: "CurrentProjectLabourId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Scripts_idx",
                table: "Characters",
                column: "CurrentScriptId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Languages_Written_idx",
                table: "Characters",
                column: "CurrentWritingLanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Cells",
                table: "Characters",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Accents_Accents_idx",
                table: "Characters_Accents",
                column: "AccentId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_ChargenRoles_ChargenRoles",
                table: "Characters_ChargenRoles",
                column: "ChargenRoleId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Languages_Languages_idx",
                table: "Characters_Languages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_MagicResources_MagicResources_idx",
                table: "Characters_MagicResources",
                column: "MagicResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_Scripts_Scripts_idx",
                table: "Characters_Scripts",
                column: "ScriptId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenAdvices_FutureProgs_idx",
                table: "ChargenAdvices",
                column: "ShouldShowAdviceProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenAdvices_ChargenRoles_ChargenRoles_idx",
                table: "ChargenAdvices_ChargenRoles",
                column: "ChargenRoleId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenAdvices_Cultures_Cultures_idx",
                table: "ChargenAdvices_Cultures",
                column: "CultureId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenAdvices_Ethnicities_Ethnicities_idx",
                table: "ChargenAdvices_Ethnicities",
                column: "EthnicityId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenAdvices_Races_Races_idx",
                table: "ChargenAdvices_Races",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenRoles_FutureProgs",
                table: "ChargenRoles",
                column: "AvailabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenRoles_Accounts",
                table: "ChargenRoles",
                column: "PosterId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenRoles_Approvers_Accounts",
                table: "ChargenRoles_Approvers",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenRoles_ClanMemberships_Clans",
                table: "ChargenRoles_ClanMemberships",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenRoles_Costs_ChargenResources",
                table: "ChargenRoles_Costs",
                column: "ChargenResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenRoles_Currencies_Currencies",
                table: "ChargenRoles_Currencies",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenRoles_Merits_Merits_idx",
                table: "ChargenRoles_Merits",
                column: "MeritId");

            migrationBuilder.CreateIndex(
                name: "FK_ChargenRoles_Traits_Currencies",
                table: "ChargenRoles_Traits",
                column: "TraitId");

            migrationBuilder.CreateIndex(
                name: "FK_Chargens_Accounts",
                table: "Chargens",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "FK_Checks_CheckTemplates",
                table: "Checks",
                column: "CheckTemplateId");

            migrationBuilder.CreateIndex(
                name: "FK_Checks_TraitExpression",
                table: "Checks",
                column: "TraitExpressionId");

            migrationBuilder.CreateIndex(
                name: "FK_CheckTemplateDifficulties_CheckTemplates",
                table: "CheckTemplateDifficulties",
                column: "CheckTemplateId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanMemberships_Characters",
                table: "ClanMemberships",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanMemberships_Manager",
                table: "ClanMemberships",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanMemberships_Appointments_Appointments",
                table: "ClanMemberships_Appointments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_ClanMemberships_Backpay_ClanMemberships",
                table: "ClanMemberships_Backpay",
                columns: new[] { "ClanId", "CharacterId" });

            migrationBuilder.CreateIndex(
                name: "FK_Clans_Calendars",
                table: "Clans",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "FK_Clans_FutureProgs_idx",
                table: "Clans",
                column: "OnPayProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Clans_Parent",
                table: "Clans",
                column: "ParentClanId");

            migrationBuilder.CreateIndex(
                name: "FK_Clans_Characters_idx",
                table: "Clans",
                column: "PaymasterId");

            migrationBuilder.CreateIndex(
                name: "FK_Clans_AdministrationCells_Cells_idx",
                table: "Clans_AdministrationCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_Clans_TreasuryCells_Cells_idx",
                table: "Clans_TreasuryCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_Coins_Currencies",
                table: "Coins",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_CombatMessages_FutureProgs_idx",
                table: "CombatMessages",
                column: "ProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CombatMessages_WeaponAttacks_WeaponAttacks_idx",
                table: "CombatMessages_WeaponAttacks",
                column: "WeaponAttackId");

            migrationBuilder.CreateIndex(
                name: "FK_CraftInputs_Crafts_idx",
                table: "CraftInputs",
                columns: new[] { "CraftId", "CraftRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_CraftProducts_Crafts_idx",
                table: "CraftProducts",
                columns: new[] { "CraftId", "CraftRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_Crafts_FutureProgs_AppearInCraftsListProg_idx",
                table: "Crafts",
                column: "AppearInCraftsListProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Crafts_FutureProgs_CanUseProg_idx",
                table: "Crafts",
                column: "CanUseProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Crafts_TraitDefinitions_idx",
                table: "Crafts",
                column: "CheckTraitId");

            migrationBuilder.CreateIndex(
                name: "FK_Crafts_EditableItems_idx",
                table: "Crafts",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_Crafts_FutureProgs_OnUseProgCancel_idx",
                table: "Crafts",
                column: "OnUseProgCancelId");

            migrationBuilder.CreateIndex(
                name: "FK_Crafts_FutureProgs_OnUseProgComplete_idx",
                table: "Crafts",
                column: "OnUseProgCompleteId");

            migrationBuilder.CreateIndex(
                name: "FK_Crafts_FutureProgs_OnUseProgStart_idx",
                table: "Crafts",
                column: "OnUseProgStartId");

            migrationBuilder.CreateIndex(
                name: "FK_Crafts_FutureProgs_WhyCannotUseProg_idx",
                table: "Crafts",
                column: "WhyCannotUseProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CraftTools_Crafts_idx",
                table: "CraftTools",
                columns: new[] { "CraftId", "CraftRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_Crimes_Accuser_idx",
                table: "Crimes",
                column: "AccuserId");

            migrationBuilder.CreateIndex(
                name: "FK_Crimes_Criminal_idx",
                table: "Crimes",
                column: "CriminalId");

            migrationBuilder.CreateIndex(
                name: "FK_Crimes_Laws_idx",
                table: "Crimes",
                column: "LawId");

            migrationBuilder.CreateIndex(
                name: "FK_Crimes_Location_idx",
                table: "Crimes",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "FK_Crimes_Victim_idx",
                table: "Crimes",
                column: "VictimId");

            migrationBuilder.CreateIndex(
                name: "FK_Cultures_AvailabilityProg",
                table: "Cultures",
                column: "AvailabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Cultures_NameCulture",
                table: "Cultures",
                column: "NameCultureId");

            migrationBuilder.CreateIndex(
                name: "FK_Cultures_SkillStartingProg",
                table: "Cultures",
                column: "SkillStartingValueProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Cultures_ChargenResources_ChargenResources",
                table: "Cultures_ChargenResources",
                column: "ChargenResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_CDPE_CurrencyDescriptionPatterns",
                table: "CurrencyDescriptionPatternElements",
                column: "CurrencyDescriptionPatternId");

            migrationBuilder.CreateIndex(
                name: "FK_CDPE_CurrencyDivisions",
                table: "CurrencyDescriptionPatternElements",
                column: "CurrencyDivisionId");

            migrationBuilder.CreateIndex(
                name: "FK_CDPESV_CDPE",
                table: "CurrencyDescriptionPatternElementSpecialValues",
                column: "CurrencyDescriptionPatternElementId");

            migrationBuilder.CreateIndex(
                name: "FK_CurrencyDescriptionPatterns_Currencies",
                table: "CurrencyDescriptionPatterns",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_CurrencyDescriptionPatterns_FutureProgs",
                table: "CurrencyDescriptionPatterns",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CurrencyDivisionAbbreviations_CurrencyDivisions",
                table: "CurrencyDivisionAbbreviations",
                column: "CurrencyDivisionId");

            migrationBuilder.CreateIndex(
                name: "FK_CurrencyDivisions_Currencies",
                table: "CurrencyDivisions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_DefaultHooks_Futureprogs_idx",
                table: "DefaultHooks",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_DisfigurementTemplates_EditableItems_idx",
                table: "DisfigurementTemplates",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_Doors_Locks",
                table: "Doors",
                column: "LockedWith");

            migrationBuilder.CreateIndex(
                name: "FK_Drawings_Characters_idx",
                table: "Drawings",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "FK_Dreams_FutureProgs_CanDream_idx",
                table: "Dreams",
                column: "CanDreamProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Dreams_FutureProgs_OnDream_idx",
                table: "Dreams",
                column: "OnDreamProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Dreams_FutureProgs_OnWake_idx",
                table: "Dreams",
                column: "OnWakeDuringDreamingProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Dreams_Dreamt_Characters_idx",
                table: "Dreams_Already_Dreamt",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_Dreams_Characters_Characters_idx",
                table: "Dreams_Characters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_Dubs_Characters",
                table: "Dubs",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZoneRevenues_FinancialPeriods_idx",
                table: "EconomicZoneRevenues",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZones_Currencies_idx",
                table: "EconomicZones",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZones_FinancialPeriods_idx",
                table: "EconomicZones",
                column: "CurrentFinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZones_Calendars_idx",
                table: "EconomicZones",
                column: "ReferenceCalendarId");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZones_Timezones_idx",
                table: "EconomicZones",
                column: "ReferenceClockId");

            migrationBuilder.CreateIndex(
                name: "FK_EconomicZoneShopTaxes_Shops_idx",
                table: "EconomicZoneShopTaxes",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "FK_EnforcementAuthorities_LegalAuthorities_idx",
                table: "EnforcementAuthorities",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_EnforcementAuthorities_AccusableClasses_LegalClasses_idx",
                table: "EnforcementAuthorities_AccusableClasses",
                column: "LegalClassId");

            migrationBuilder.CreateIndex(
                name: "FK_EnforcementAuthorities_ArrestableClasses_LegalClasses_idx",
                table: "EnforcementAuthorities_ArrestableClasses",
                column: "LegalClassId");

            migrationBuilder.CreateIndex(
                name: "FK_EnforcementAuthorities_ParentAuthorities_Child_idx",
                table: "EnforcementAuthorities_ParentAuthorities",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "FK_EntityDescriptionPatterns_FutureProgs",
                table: "EntityDescriptionPatterns",
                column: "ApplicabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_EDP_EntityDescriptions_EntityDescriptions",
                table: "EntityDescriptionPatterns_EntityDescriptions",
                column: "EntityDescriptionId");

            migrationBuilder.CreateIndex(
                name: "FK_Ethnicities_AvailabilityProg",
                table: "Ethnicities",
                column: "AvailabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Ethnicities_Races_idx",
                table: "Ethnicities",
                column: "ParentRaceId");

            migrationBuilder.CreateIndex(
                name: "FK_Ethnicities_PopulationBloodModels_idx",
                table: "Ethnicities",
                column: "PopulationBloodModelId");

            migrationBuilder.CreateIndex(
                name: "FK_Ethnicities_Characteristics_CharacteristicDefinitions",
                table: "Ethnicities_Characteristics",
                column: "CharacteristicDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_Ethnicities_Characteristics_CharacteristicProfiles",
                table: "Ethnicities_Characteristics",
                column: "CharacteristicProfileId");

            migrationBuilder.CreateIndex(
                name: "FK_Ethnicities_ChargenResources_ChargenResources",
                table: "Ethnicities_ChargenResources",
                column: "ChargenResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_ECC_Appointments_Controlled",
                table: "ExternalClanControls",
                column: "ControlledAppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_ECC_Appointments_Controlling",
                table: "ExternalClanControls",
                column: "ControllingAppointmentId");

            migrationBuilder.CreateIndex(
                name: "FK_ECC_Clans_Liege",
                table: "ExternalClanControls",
                column: "LiegeClanId");

            migrationBuilder.CreateIndex(
                name: "FK_ECC_Appointments_ClanMemberships",
                table: "ExternalClanControls_Appointments",
                columns: new[] { "VassalClanId", "CharacterId" });

            migrationBuilder.CreateIndex(
                name: "FK_ECC_Appointments_ExternalClanControls",
                table: "ExternalClanControls_Appointments",
                columns: new[] { "VassalClanId", "LiegeClanId", "ControlledAppointmentId" });

            migrationBuilder.CreateIndex(
                name: "FK_FinancialPeriods_EconomicZones_idx",
                table: "FinancialPeriods",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_ForagableProfiles_EditableItems_idx",
                table: "ForagableProfiles",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_Foragables_EditableItems",
                table: "Foragables",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_GameItemComponentProtos_EditableItems",
                table: "GameItemComponentProtos",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_GameItemComponents_GameItems",
                table: "GameItemComponents",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_GameItemProtos_EditableItems",
                table: "GameItemProtos",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_GameItemProtos_ItemGroups_idx",
                table: "GameItemProtos",
                column: "ItemGroupId");

            migrationBuilder.CreateIndex(
                name: "FK_GIPGICP_GameItemComponentProtos",
                table: "GameItemProtos_GameItemComponentProtos",
                columns: new[] { "GameItemComponentProtoId", "GameItemComponentRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_GIPGICP_GameItemProtos",
                table: "GameItemProtos_GameItemComponentProtos",
                columns: new[] { "GameItemProtoId", "GameItemProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_GameItemProtos_OnLoadProgs_FutureProgs_idx",
                table: "GameItemProtos_OnLoadProgs",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_GameItemProtos_Tags_Tags",
                table: "GameItemProtos_Tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "FK_GameItemProtos_Tags_GameItemProtos",
                table: "GameItemProtos_Tags",
                columns: new[] { "GameItemProtoId", "GameItemProtoRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_GameItems_GameItems_Containers_idx",
                table: "GameItems",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "FK_GameItems_MagicResources_MagicResources_idx",
                table: "GameItems_MagicResources",
                column: "MagicResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_Gases_Gases_idx",
                table: "Gases",
                column: "CountAsId");

            migrationBuilder.CreateIndex(
                name: "FK_Gases_Liquids_idx",
                table: "Gases",
                column: "PrecipitateId");

            migrationBuilder.CreateIndex(
                name: "FK_Gases_Tags_Tags_idx",
                table: "Gases_Tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "FK_GroupAIs_GroupAITemplates_idx",
                table: "GroupAIs",
                column: "GroupAITemplateId");

            migrationBuilder.CreateIndex(
                name: "FK_Helpfiles_FutureProgs",
                table: "Helpfiles",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "FK_Helpfiles_ExtraTexts_FutureProgs",
                table: "Helpfiles_ExtraTexts",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "FK_Hooks_Perceivables_Bodies_idx",
                table: "Hooks_Perceivables",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "FK_Hooks_Perceivables_Cells_idx",
                table: "Hooks_Perceivables",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_Hooks_Perceivables_Characters_idx",
                table: "Hooks_Perceivables",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_Hooks_Perceivables_GameItems_idx",
                table: "Hooks_Perceivables",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_Hooks_Perceivables_Hooks_idx",
                table: "Hooks_Perceivables",
                column: "HookId");

            migrationBuilder.CreateIndex(
                name: "FK_Hooks_Perceivables_Shards_idx",
                table: "Hooks_Perceivables",
                column: "ShardId");

            migrationBuilder.CreateIndex(
                name: "FK_Hooks_Perceivables_Zones_idx",
                table: "Hooks_Perceivables",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_Infections_Bodyparts_idx",
                table: "Infections",
                column: "BodypartId");

            migrationBuilder.CreateIndex(
                name: "FK_Infections_Bodies_idx",
                table: "Infections",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "FK_Infections_Wounds_idx",
                table: "Infections",
                column: "WoundId");

            migrationBuilder.CreateIndex(
                name: "FK_ItemGroupForms_ItemGroups_idx",
                table: "ItemGroupForms",
                column: "ItemGroupId");

            migrationBuilder.CreateIndex(
                name: "FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE_idx",
                table: "knowledges",
                column: "CanAcquireProgId");

            migrationBuilder.CreateIndex(
                name: "FK_KNOWLEDGES_FUTUREPROGS_LEARN_idx",
                table: "knowledges",
                column: "CanLearnProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Languages_Accents_idx",
                table: "Languages",
                column: "DefaultLearnerAccentId");

            migrationBuilder.CreateIndex(
                name: "FK_Languages_LanguageDifficultyModels",
                table: "Languages",
                column: "DifficultyModel");

            migrationBuilder.CreateIndex(
                name: "FK_Languages_TraitDefinitions",
                table: "Languages",
                column: "LinkedTraitId");

            migrationBuilder.CreateIndex(
                name: "FK_Laws_FutureProgs_idx",
                table: "Laws",
                column: "LawAppliesProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Laws_LegalAuthority_idx",
                table: "Laws",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_Laws_OffenderClasses_LegalClasses_idx",
                table: "Laws_OffenderClasses",
                column: "LegalClassId");

            migrationBuilder.CreateIndex(
                name: "FK_Laws_VictimClasses_LegalClasses_idx",
                table: "Laws_VictimClasses",
                column: "LegalClassId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_Currencies_idx",
                table: "LegalAuthorities",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_Zones_LegalAuthorities_idx",
                table: "LegalAuthorities_Zones",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalClasses_LegalAuthorities_idx",
                table: "LegalClasses",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalClasses_FutureProgs_idx",
                table: "LegalClasses",
                column: "MembershipProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Limbs_BodyProtos_idx",
                table: "Limbs",
                column: "RootBodyId");

            migrationBuilder.CreateIndex(
                name: "FK_Limbs_BodypartProto_idx",
                table: "Limbs",
                column: "RootBodypartId");

            migrationBuilder.CreateIndex(
                name: "FK_Limbs_BodypartProto_Limbs_idx",
                table: "Limbs_BodypartProto",
                column: "LimbId");

            migrationBuilder.CreateIndex(
                name: "FK_Limbs_SpinalParts_BodypartProtos_idx",
                table: "Limbs_SpinalParts",
                column: "BodypartProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_Liquids_Liquids_CountasAs_idx",
                table: "Liquids",
                column: "CountAsId");

            migrationBuilder.CreateIndex(
                name: "FK_Liquids_Materials_idx",
                table: "Liquids",
                column: "DriedResidueId");

            migrationBuilder.CreateIndex(
                name: "FK_Liquids_Drugs_idx",
                table: "Liquids",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "FK_Liquids_Liquids_idx",
                table: "Liquids",
                column: "SolventId");

            migrationBuilder.CreateIndex(
                name: "FK_Liquids_Tags_Tags_idx",
                table: "Liquids_Tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "FK_LoginIPs_Accounts",
                table: "LoginIPs",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicCapabilities_MagicSchools_idx",
                table: "MagicCapabilities",
                column: "MagicSchoolId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicPowers_MagicSchools_idx",
                table: "MagicPowers",
                column: "MagicSchoolId");

            migrationBuilder.CreateIndex(
                name: "FK_MagicSchools_MagicSchools_idx",
                table: "MagicSchools",
                column: "ParentSchoolId");

            migrationBuilder.CreateIndex(
                name: "Materials_Tags_Tags_idx",
                table: "Materials_Tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "FK_Merchandises_GameItems_idx",
                table: "Merchandises",
                column: "PreferredDisplayContainerId");

            migrationBuilder.CreateIndex(
                name: "FK_Merchandises_Shops_idx",
                table: "Merchandises",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "FK_Merits_Merits_idx",
                table: "Merits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "FK_Merits_ChargenResources_ChargenResources_idx",
                table: "Merits_ChargenResources",
                column: "ChargenResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_MoveSpeeds_BodyPrototype",
                table: "MoveSpeeds",
                column: "BodyProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_Languages_MutualIntelligabilities_Target_idx",
                table: "MutualIntelligabilities",
                column: "TargetLanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_NPCs_Characters_Bodyguard_idx",
                table: "NPCs",
                column: "BodyguardCharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_NPCs_Characters",
                table: "NPCs",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_NPCs_NPCTemplates",
                table: "NPCs",
                columns: new[] { "TemplateId", "TemplateRevnum" });

            migrationBuilder.CreateIndex(
                name: "FK_NPCs_ArtificialIntelligences_NPCs",
                table: "NPCs_ArtificialIntelligences",
                column: "NPCId");

            migrationBuilder.CreateIndex(
                name: "FK_NPCTemplates_EditableItems",
                table: "NPCTemplates",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_NTAI_ArtificalIntelligences",
                table: "NPCTemplates_ArtificalIntelligences",
                column: "AIId");

            migrationBuilder.CreateIndex(
                name: "FK_NTAI_NPCTemplates",
                table: "NPCTemplates_ArtificalIntelligences",
                columns: new[] { "NPCTemplateId", "NPCTemplateRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_Paygrades_Clans",
                table: "Paygrades",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "FK_Paygrades_Currencies",
                table: "Paygrades",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_PerceiverMerits_Bodies_idx",
                table: "PerceiverMerits",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "FK_PerceiverMerits_Characters_idx",
                table: "PerceiverMerits",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_PerceiverMerits_GameItems_idx",
                table: "PerceiverMerits",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: " FK_PerceiverMerits_Merits_idx",
                table: "PerceiverMerits",
                column: "MeritId");

            migrationBuilder.CreateIndex(
                name: "FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels_idx",
                table: "PopulationBloodModels_Bloodtypes",
                column: "PopulationBloodModelId");

            migrationBuilder.CreateIndex(
                name: "FK_ProgSchedules_FutureProgs_idx",
                table: "ProgSchedules",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectActions_ProjectPhases_idx",
                table: "ProjectActions",
                column: "ProjectPhaseId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectLabourImpacts_ProjectLabourRequirements_idx",
                table: "ProjectLabourImpacts",
                column: "ProjectLabourRequirementId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectLabourRequirements_ProjectPhases_idx",
                table: "ProjectLabourRequirements",
                column: "ProjectPhaseId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectMaterialRequirements_ProjectPhases_idx",
                table: "ProjectMaterialRequirements",
                column: "ProjectPhaseId");

            migrationBuilder.CreateIndex(
                name: "FK_ProjectPhases_Projects_idx",
                table: "ProjectPhases",
                columns: new[] { "ProjectId", "ProjectRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_Projects_EditableItems_idx",
                table: "Projects",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_RaceButcheryProfiles_FutureProgs_Can_idx",
                table: "RaceButcheryProfiles",
                column: "CanButcherProgId");

            migrationBuilder.CreateIndex(
                name: "FK_RaceButcheryProfiles_Tags_idx",
                table: "RaceButcheryProfiles",
                column: "RequiredToolTagId");

            migrationBuilder.CreateIndex(
                name: "FK_RaceButcheryProfiles_FutureProgs_Why_idx",
                table: "RaceButcheryProfiles",
                column: "WhyCannotButcherProgId");

            migrationBuilder.CreateIndex(
                name: "FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions_idx",
                table: "RaceButcheryProfiles_BreakdownChecks",
                column: "TraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts_idx",
                table: "RaceButcheryProfiles_ButcheryProducts",
                column: "ButcheryProductId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_AttributeBonusProg",
                table: "Races",
                column: "AttributeBonusProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_AvailabilityProg",
                table: "Races",
                column: "AvailabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_BodyProtos",
                table: "Races",
                column: "BaseBodyId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_Liquids_Blood_idx",
                table: "Races",
                column: "BloodLiquidId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_BloodModels_idx",
                table: "Races",
                column: "BloodModelId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_CorpseModels_idx",
                table: "Races",
                column: "CorpseModelId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_HealthStrategies_idx",
                table: "Races",
                column: "DefaultHealthStrategyId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_Materials_idx",
                table: "Races",
                column: "NaturalArmourMaterialId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_ArmourTypes_idx",
                table: "Races",
                column: "NaturalArmourTypeId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_Races",
                table: "Races",
                column: "ParentRaceId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_RaceButcheryProfiles_idx",
                table: "Races",
                column: "RaceButcheryProfileId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_Liqiuds_Sweat_idx",
                table: "Races",
                column: "SweatLiquidId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_AdditionalBodyparts_BodypartProto",
                table: "Races_AdditionalBodyparts",
                column: "BodypartId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_AdditionalBodyparts_Races",
                table: "Races_AdditionalBodyparts",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "FK_RAC_CharacteristicDefinitions",
                table: "Races_AdditionalCharacteristics",
                column: "CharacteristicDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_Attributes_TraitDefinitions",
                table: "Races_Attributes",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "FK_Races-BreathableGases_Gases_idx",
                table: "Races_BreathableGases",
                column: "GasId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_BreathableLiquids_Liquids_idx",
                table: "Races_BreathableLiquids",
                column: "LiquidId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_ChargenResources_ChargenResources",
                table: "Races_ChargenResources",
                column: "ChargenResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_EdibleMaterials_Materials_idx",
                table: "Races_EdibleMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_WeaponAttacks_BodypartProto_idx",
                table: "Races_WeaponAttacks",
                column: "BodypartId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_WeaponAttacks_WeaponAttacks_idx",
                table: "Races_WeaponAttacks",
                column: "WeaponAttackId");

            migrationBuilder.CreateIndex(
                name: "FK_RandomNameProfiles_NameCulture",
                table: "RandomNameProfiles",
                column: "NameCultureId");

            migrationBuilder.CreateIndex(
                name: "FK_RangedWeaponTypes_TraitDefinitions_Fire_idx",
                table: "RangedWeaponTypes",
                column: "FireTraitId");

            migrationBuilder.CreateIndex(
                name: "FK_RangedWeaponTypes_TraitDefinitions_Operate_idx",
                table: "RangedWeaponTypes",
                column: "OperateTraitId");

            migrationBuilder.CreateIndex(
                name: "FK_Ranks_Clans",
                table: "Ranks",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "FK_Ranks_GameItemProtos",
                table: "Ranks",
                columns: new[] { "InsigniaGameItemId", "InsigniaGameItemRevnum" });

            migrationBuilder.CreateIndex(
                name: "FK_Ranks_Abbreviations_FutureProgs",
                table: "Ranks_Abbreviations",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Ranks_Paygrades_Paygrades",
                table: "Ranks_Paygrades",
                column: "PaygradeId");

            migrationBuilder.CreateIndex(
                name: "FK_Ranks_Titles_FutureProgs",
                table: "Ranks_Titles",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_RegionalClimates_Seasons_Seasons_idx",
                table: "RegionalClimates_Seasons",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "FK_Rooms_Zones",
                table: "Rooms",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_Scripts_Knowledges_idx",
                table: "Scripts",
                column: "KnowledgeId");

            migrationBuilder.CreateIndex(
                name: "FK_Scripts_DesignedLanguages_Languages_idx",
                table: "Scripts_DesignedLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_Seasons_Celestials_idx",
                table: "Seasons",
                column: "CelestialId");

            migrationBuilder.CreateIndex(
                name: "FK_Shards_SkyDescriptionTemplates",
                table: "Shards",
                column: "SkyDescriptionTemplateId");

            migrationBuilder.CreateIndex(
                name: "FK_ShieldTypes_TraitDefinitions_idx",
                table: "ShieldTypes",
                column: "BlockTraitId");

            migrationBuilder.CreateIndex(
                name: "FK_ShieldTypes_ArmourTypes_idx",
                table: "ShieldTypes",
                column: "EffectiveArmourTypeId");

            migrationBuilder.CreateIndex(
                name: "FK_ShopFinancialPeriodResults_FinancialPeriods_idx",
                table: "ShopFinancialPeriodResults",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "FK_ShopFinancialPeriodResults_Shops_idx",
                table: "ShopFinancialPeriodResults",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "FK_Shops_FutureProgs_Can_idx",
                table: "Shops",
                column: "CanShopProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Shops_Currencies_idx",
                table: "Shops",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_Shops_EconomicZonesa_idx",
                table: "Shops",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_Shops_Cells_Stockroom_idx",
                table: "Shops",
                column: "StockroomCellId");

            migrationBuilder.CreateIndex(
                name: "FK_Shops_FutureProgs_WhyCant_idx",
                table: "Shops",
                column: "WhyCannotShopProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Shops_Cells_Workshop_idx",
                table: "Shops",
                column: "WorkshopCellId");

            migrationBuilder.CreateIndex(
                name: "FK_Shops_StoreroomCells_Cells_idx",
                table: "Shops_StoreroomCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_ShopTills_GameItems_idx",
                table: "ShopsTills",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_ShopTransactionRecords_Currencies_idx",
                table: "ShopTransactionRecords",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "FK_ShopTransactionRecords_Shops_idx",
                table: "ShopTransactionRecords",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "FK_Socials_FutureProgs",
                table: "Socials",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg_idx",
                table: "SurgicalProcedurePhases",
                column: "OnPhaseProgId");

            migrationBuilder.CreateIndex(
                name: "FK_SurgicalProcedures_FutureProgs_AbortProg_idx",
                table: "SurgicalProcedures",
                column: "AbortProgId");

            migrationBuilder.CreateIndex(
                name: "FK_SurgicalProcedures_FutureProgs_CompletionProg_idx",
                table: "SurgicalProcedures",
                column: "CompletionProgId");

            migrationBuilder.CreateIndex(
                name: "FK_SurgicalProcedures_Knowledges_idx",
                table: "SurgicalProcedures",
                column: "KnowledgeRequiredId");

            migrationBuilder.CreateIndex(
                name: "FK_SurgicalProcedures_FutureProgs_Usability_idx",
                table: "SurgicalProcedures",
                column: "UsabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg_idx",
                table: "SurgicalProcedures",
                column: "WhyCannotUseProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Tags_Parent_idx",
                table: "Tags",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "FK_Tags_Futureprogs_idx",
                table: "Tags",
                column: "ShouldSeeProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Terrains_WeatherControllers_idx",
                table: "Terrains",
                column: "WeatherControllerId");

            migrationBuilder.CreateIndex(
                name: "FK_Terrains_RangedCovers_RangedCovers_idx",
                table: "Terrains_RangedCovers",
                column: "RangedCoverId");

            migrationBuilder.CreateIndex(
                name: "FK_Timezones_Clocks",
                table: "Timezones",
                column: "ClockId");

            migrationBuilder.CreateIndex(
                name: "FK_TraitDefinitions_AvailabilityProg",
                table: "TraitDefinitions",
                column: "AvailabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_TraitDefinitions_TraitExpression",
                table: "TraitDefinitions",
                column: "ExpressionId");

            migrationBuilder.CreateIndex(
                name: "FK_TraitDefinitions_LearnableProg_idx",
                table: "TraitDefinitions",
                column: "LearnableProgId");

            migrationBuilder.CreateIndex(
                name: "FK_TraitDefinitions_TeachableProg_idx",
                table: "TraitDefinitions",
                column: "TeachableProgId");

            migrationBuilder.CreateIndex(
                name: "FK_TraitDefinitions_ChargenResources_ChargenResources",
                table: "TraitDefinitions_ChargenResources",
                column: "ChargenResourceId");

            migrationBuilder.CreateIndex(
                name: "FK_TraitExpressionParameters_TraitDefinitions",
                table: "TraitExpressionParameters",
                column: "TraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_TraitExpressionParameters_TraitExpression",
                table: "TraitExpressionParameters",
                column: "TraitExpressionId");

            migrationBuilder.CreateIndex(
                name: "FK_Traits_TraitDefinitions",
                table: "Traits",
                column: "TraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_WeaponAttacks_TraitExpression_Damage_idx",
                table: "WeaponAttacks",
                column: "DamageExpressionId");

            migrationBuilder.CreateIndex(
                name: "FK_WeaponAttacks_FutureProgs_idx",
                table: "WeaponAttacks",
                column: "FutureProgId");

            migrationBuilder.CreateIndex(
                name: "FK_WeaponAttacks_TraitExpression_Pain_idx",
                table: "WeaponAttacks",
                column: "PainExpressionId");

            migrationBuilder.CreateIndex(
                name: "FK_WeaponAttacks_TraitExpression_Stun_idx",
                table: "WeaponAttacks",
                column: "StunExpressionId");

            migrationBuilder.CreateIndex(
                name: "FK_WeaponAttacks_WeaponTypes_idx",
                table: "WeaponAttacks",
                column: "WeaponTypeId");

            migrationBuilder.CreateIndex(
                name: "FK_WeaponTypes_TraitDefinitions_Attack_idx",
                table: "WeaponTypes",
                column: "AttackTraitId");

            migrationBuilder.CreateIndex(
                name: "FK_WeaponTypes_TraitDefinitions_Parry_idx",
                table: "WeaponTypes",
                column: "ParryTraitId");

            migrationBuilder.CreateIndex(
                name: "FK_WearableSizeParameterRule_TraitDefinitions",
                table: "WearableSizeParameterRule",
                column: "TraitId");

            migrationBuilder.CreateIndex(
                name: "FK_WeatherControllers_Celestials_idx",
                table: "WeatherControllers",
                column: "CelestialId");

            migrationBuilder.CreateIndex(
                name: "FK_WeatherControllers_Seasons_idx",
                table: "WeatherControllers",
                column: "CurrentSeasonId");

            migrationBuilder.CreateIndex(
                name: "FK_WeatherControllers_WeatherEvents_idx",
                table: "WeatherControllers",
                column: "CurrentWeatherEventId");

            migrationBuilder.CreateIndex(
                name: "FK_WeatherControllers_Clocks_idx",
                table: "WeatherControllers",
                column: "FeedClockId");

            migrationBuilder.CreateIndex(
                name: "FK_WeatherControllers_TimeZones_idx",
                table: "WeatherControllers",
                column: "FeedClockTimeZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_WeatherControllers_RegionalClimates_idx",
                table: "WeatherControllers",
                column: "RegionalClimateId");

            migrationBuilder.CreateIndex(
                name: "FK_WeatherEvents_WeatherEvents_idx",
                table: "WeatherEvents",
                column: "CountsAsId");

            migrationBuilder.CreateIndex(
                name: "FK_WitnessProfiles_IdentityProg_idx",
                table: "WitnessProfiles",
                column: "IdentityKnownProgId");

            migrationBuilder.CreateIndex(
                name: "FK_WitnessProfiles_MultiplierProg_idx",
                table: "WitnessProfiles",
                column: "ReportingMultiplierProgId");

            migrationBuilder.CreateIndex(
                name: "FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities_idx",
                table: "WitnessProfiles_CooperatingAuthorities",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses_idx",
                table: "WitnessProfiles_IgnoredCriminalClasses",
                column: "LegalClassId");

            migrationBuilder.CreateIndex(
                name: "FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses_idx",
                table: "WitnessProfiles_IgnoredVictimClasses",
                column: "LegalClassId");

            migrationBuilder.CreateIndex(
                name: "FK_Wounds_Characters_idx",
                table: "Wounds",
                column: "ActorOriginId");

            migrationBuilder.CreateIndex(
                name: "FK_Wounds_Bodies_idx",
                table: "Wounds",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "FK_Wounds_GameItemOwner_idx",
                table: "Wounds",
                column: "GameItemId");

            migrationBuilder.CreateIndex(
                name: "FK_Wounds_GameItems_idx",
                table: "Wounds",
                column: "LodgedItemId");

            migrationBuilder.CreateIndex(
                name: "FK_Wounds_GameItems_Tool_idx",
                table: "Wounds",
                column: "ToolOriginId");

            migrationBuilder.CreateIndex(
                name: "FK_Writings_Characters_Author_idx",
                table: "Writings",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "FK_Writings_Languages_idx",
                table: "Writings",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_Writings_Scripts_idx",
                table: "Writings",
                column: "ScriptId");

            migrationBuilder.CreateIndex(
                name: "FK_Writings_Characters_TrueAuthor_idx",
                table: "Writings",
                column: "TrueAuthorId");

            migrationBuilder.CreateIndex(
                name: "FK_Zones_Cells",
                table: "Zones",
                column: "DefaultCellId");

            migrationBuilder.CreateIndex(
                name: "FK_Zones_Shards",
                table: "Zones",
                column: "ShardId");

            migrationBuilder.CreateIndex(
                name: "FK_Zones_WeatherControllers_idx",
                table: "Zones",
                column: "WeatherControllerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Languages_Spoken",
                table: "Characters",
                column: "CurrentLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Languages_Written",
                table: "Characters",
                column: "CurrentWritingLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_ActiveProjects",
                table: "Characters",
                column: "CurrentProjectId",
                principalTable: "ActiveProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Cells",
                table: "Characters",
                column: "Location",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Bodies",
                table: "Characters",
                column: "BodyId",
                principalTable: "Bodies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Accents",
                table: "Characters",
                column: "CurrentAccentId",
                principalTable: "Accents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Accents_Accents",
                table: "Characters_Accents",
                column: "AccentId",
                principalTable: "Accents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Accents",
                table: "Languages",
                column: "DefaultLearnerAccentId",
                principalTable: "Accents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterLog_Cells",
                table: "CharacterLog",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveProjectLabours_ActiveProjects",
                table: "ActiveProjectLabours",
                column: "ActiveProjectId",
                principalTable: "ActiveProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveProjectMaterials_ActiveProjects",
                table: "ActiveProjectMaterials",
                column: "ActiveProjectId",
                principalTable: "ActiveProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Rooms_Rooms",
                table: "Areas_Rooms",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BodypartProto_BodyPrototype",
                table: "BodypartProto",
                column: "BodyId",
                principalTable: "BodyProtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hooks_Perceivables_Cells",
                table: "Hooks_Perceivables",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hooks_Perceivables_Zones",
                table: "Hooks_Perceivables",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EconomicZones_FinancialPeriods",
                table: "EconomicZones",
                column: "CurrentFinancialPeriodId",
                principalTable: "FinancialPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CellOverlays_Cells",
                table: "CellOverlays",
                column: "CellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_Rooms",
                table: "Cells",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accents_Languages",
                table: "Accents");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Languages_Spoken",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Languages_Written",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Accounts",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Chargens_Accounts",
                table: "Chargens");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_ActiveProjects",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_CellOverlays_Cells",
                table: "CellOverlays");

            migrationBuilder.DropForeignKey(
                name: "FK_Zones_Cells",
                table: "Zones");

            migrationBuilder.DropForeignKey(
                name: "FK_TraitDefinitions_AvailabilityProg",
                table: "TraitDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TraitDefinitions_LearnableProg",
                table: "TraitDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TraitDefinitions_TeachableProg",
                table: "TraitDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_BodyPrototype_Bodyparts",
                table: "BodyProtos");

            migrationBuilder.DropForeignKey(
                name: "FK_EconomicZones_Currencies",
                table: "EconomicZones");

            migrationBuilder.DropForeignKey(
                name: "FK_EconomicZones_Calendars",
                table: "EconomicZones");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialPeriods_EconomicZones",
                table: "FinancialPeriods");

            migrationBuilder.DropTable(
                name: "AccountNotes");

            migrationBuilder.DropTable(
                name: "Accounts_ChargenResources");

            migrationBuilder.DropTable(
                name: "ActiveProjectLabours");

            migrationBuilder.DropTable(
                name: "ActiveProjectMaterials");

            migrationBuilder.DropTable(
                name: "Allies");

            migrationBuilder.DropTable(
                name: "AmmunitionTypes");

            migrationBuilder.DropTable(
                name: "Appointments_Abbreviations");

            migrationBuilder.DropTable(
                name: "Appointments_Titles");

            migrationBuilder.DropTable(
                name: "Areas_Rooms");

            migrationBuilder.DropTable(
                name: "AutobuilderAreaTemplates");

            migrationBuilder.DropTable(
                name: "AutobuilderRoomTemplates");

            migrationBuilder.DropTable(
                name: "Bans");

            migrationBuilder.DropTable(
                name: "BloodModels_Bloodtypes");

            migrationBuilder.DropTable(
                name: "Bloodtypes_BloodtypeAntigens");

            migrationBuilder.DropTable(
                name: "BoardPosts");

            migrationBuilder.DropTable(
                name: "Bodies_DrugDoses");

            migrationBuilder.DropTable(
                name: "Bodies_GameItems");

            migrationBuilder.DropTable(
                name: "Bodies_Implants");

            migrationBuilder.DropTable(
                name: "Bodies_Prosthetics");

            migrationBuilder.DropTable(
                name: "Bodies_SeveredParts");

            migrationBuilder.DropTable(
                name: "BodypartGroupDescribers_BodypartProtos");

            migrationBuilder.DropTable(
                name: "BodypartGroupDescribers_BodyProtos");

            migrationBuilder.DropTable(
                name: "BodypartGroupDescribers_ShapeCount");

            migrationBuilder.DropTable(
                name: "BodypartInternalInfos");

            migrationBuilder.DropTable(
                name: "BodypartProto_AlignmentHits");

            migrationBuilder.DropTable(
                name: "BodypartProto_BodypartProto_Upstream");

            migrationBuilder.DropTable(
                name: "BodypartProto_OrientationHits");

            migrationBuilder.DropTable(
                name: "bodypartshapecountview");

            migrationBuilder.DropTable(
                name: "BodyProtos_AdditionalBodyparts");

            migrationBuilder.DropTable(
                name: "BoneOrganCoverages");

            migrationBuilder.DropTable(
                name: "ButcheryProductItems");

            migrationBuilder.DropTable(
                name: "ButcheryProducts_BodypartProtos");

            migrationBuilder.DropTable(
                name: "CellOverlays_Exits");

            migrationBuilder.DropTable(
                name: "Cells_ForagableYields");

            migrationBuilder.DropTable(
                name: "Cells_GameItems");

            migrationBuilder.DropTable(
                name: "Cells_MagicResources");

            migrationBuilder.DropTable(
                name: "Cells_RangedCovers");

            migrationBuilder.DropTable(
                name: "Cells_Tags");

            migrationBuilder.DropTable(
                name: "ChannelCommandWords");

            migrationBuilder.DropTable(
                name: "ChannelIgnorers");

            migrationBuilder.DropTable(
                name: "CharacterCombatSettings");

            migrationBuilder.DropTable(
                name: "CharacterIntroTemplates");

            migrationBuilder.DropTable(
                name: "Characteristics");

            migrationBuilder.DropTable(
                name: "CharacterKnowledges");

            migrationBuilder.DropTable(
                name: "CharacterLog");

            migrationBuilder.DropTable(
                name: "Characters_Accents");

            migrationBuilder.DropTable(
                name: "Characters_ChargenRoles");

            migrationBuilder.DropTable(
                name: "Characters_Languages");

            migrationBuilder.DropTable(
                name: "Characters_MagicResources");

            migrationBuilder.DropTable(
                name: "Characters_Scripts");

            migrationBuilder.DropTable(
                name: "ChargenAdvices_ChargenRoles");

            migrationBuilder.DropTable(
                name: "ChargenAdvices_Cultures");

            migrationBuilder.DropTable(
                name: "ChargenAdvices_Ethnicities");

            migrationBuilder.DropTable(
                name: "ChargenAdvices_Races");

            migrationBuilder.DropTable(
                name: "ChargenRoles_Approvers");

            migrationBuilder.DropTable(
                name: "ChargenRoles_ClanMemberships_Appointments");

            migrationBuilder.DropTable(
                name: "ChargenRoles_Costs");

            migrationBuilder.DropTable(
                name: "ChargenRoles_Currencies");

            migrationBuilder.DropTable(
                name: "ChargenRoles_Merits");

            migrationBuilder.DropTable(
                name: "ChargenRoles_Traits");

            migrationBuilder.DropTable(
                name: "Checks");

            migrationBuilder.DropTable(
                name: "CheckTemplateDifficulties");

            migrationBuilder.DropTable(
                name: "ClanMemberships_Appointments");

            migrationBuilder.DropTable(
                name: "ClanMemberships_Backpay");

            migrationBuilder.DropTable(
                name: "Clans_AdministrationCells");

            migrationBuilder.DropTable(
                name: "Clans_TreasuryCells");

            migrationBuilder.DropTable(
                name: "ClimateModels");

            migrationBuilder.DropTable(
                name: "Coins");

            migrationBuilder.DropTable(
                name: "Colours");

            migrationBuilder.DropTable(
                name: "CombatMessages_WeaponAttacks");

            migrationBuilder.DropTable(
                name: "CraftInputs");

            migrationBuilder.DropTable(
                name: "CraftPhases");

            migrationBuilder.DropTable(
                name: "CraftProducts");

            migrationBuilder.DropTable(
                name: "CraftTools");

            migrationBuilder.DropTable(
                name: "Crimes");

            migrationBuilder.DropTable(
                name: "CultureInfos");

            migrationBuilder.DropTable(
                name: "Cultures_ChargenResources");

            migrationBuilder.DropTable(
                name: "CurrencyDescriptionPatternElementSpecialValues");

            migrationBuilder.DropTable(
                name: "CurrencyDivisionAbbreviations");

            migrationBuilder.DropTable(
                name: "DamagePatterns");

            migrationBuilder.DropTable(
                name: "DefaultHooks");

            migrationBuilder.DropTable(
                name: "DisfigurementTemplates");

            migrationBuilder.DropTable(
                name: "Doors");

            migrationBuilder.DropTable(
                name: "Drawings");

            migrationBuilder.DropTable(
                name: "Dream_Phases");

            migrationBuilder.DropTable(
                name: "Dreams_Already_Dreamt");

            migrationBuilder.DropTable(
                name: "Dreams_Characters");

            migrationBuilder.DropTable(
                name: "DrugsIntensities");

            migrationBuilder.DropTable(
                name: "Dubs");

            migrationBuilder.DropTable(
                name: "EconomicZoneRevenues");

            migrationBuilder.DropTable(
                name: "EconomicZoneShopTaxes");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "EnforcementAuthorities_AccusableClasses");

            migrationBuilder.DropTable(
                name: "EnforcementAuthorities_ArrestableClasses");

            migrationBuilder.DropTable(
                name: "EnforcementAuthorities_ParentAuthorities");

            migrationBuilder.DropTable(
                name: "EntityDescriptionPatterns_EntityDescriptions");

            migrationBuilder.DropTable(
                name: "Ethnicities_Characteristics");

            migrationBuilder.DropTable(
                name: "Ethnicities_ChargenResources");

            migrationBuilder.DropTable(
                name: "ExternalClanControls_Appointments");

            migrationBuilder.DropTable(
                name: "ForagableProfiles_Foragables");

            migrationBuilder.DropTable(
                name: "ForagableProfiles_HourlyYieldGains");

            migrationBuilder.DropTable(
                name: "ForagableProfiles_MaximumYields");

            migrationBuilder.DropTable(
                name: "Foragables");

            migrationBuilder.DropTable(
                name: "FutureProgs_Parameters");

            migrationBuilder.DropTable(
                name: "GameItemComponents");

            migrationBuilder.DropTable(
                name: "gameitemeditingview");

            migrationBuilder.DropTable(
                name: "GameItemProtos_DefaultVariables");

            migrationBuilder.DropTable(
                name: "GameItemProtos_GameItemComponentProtos");

            migrationBuilder.DropTable(
                name: "GameItemProtos_OnLoadProgs");

            migrationBuilder.DropTable(
                name: "GameItemProtos_Tags");

            migrationBuilder.DropTable(
                name: "GameItems_MagicResources");

            migrationBuilder.DropTable(
                name: "Gases_Tags");

            migrationBuilder.DropTable(
                name: "Grids");

            migrationBuilder.DropTable(
                name: "GroupAIs");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "HeightWeightModels");

            migrationBuilder.DropTable(
                name: "Helpfiles_ExtraTexts");

            migrationBuilder.DropTable(
                name: "Hooks_Perceivables");

            migrationBuilder.DropTable(
                name: "Improvers");

            migrationBuilder.DropTable(
                name: "Infections");

            migrationBuilder.DropTable(
                name: "ItemGroupForms");

            migrationBuilder.DropTable(
                name: "Laws_OffenderClasses");

            migrationBuilder.DropTable(
                name: "Laws_VictimClasses");

            migrationBuilder.DropTable(
                name: "LegalAuthorities_Zones");

            migrationBuilder.DropTable(
                name: "Limbs_BodypartProto");

            migrationBuilder.DropTable(
                name: "Limbs_SpinalParts");

            migrationBuilder.DropTable(
                name: "Liquids_Tags");

            migrationBuilder.DropTable(
                name: "LoginIPs");

            migrationBuilder.DropTable(
                name: "MagicCapabilities");

            migrationBuilder.DropTable(
                name: "MagicGenerators");

            migrationBuilder.DropTable(
                name: "MagicPowers");

            migrationBuilder.DropTable(
                name: "Materials_Tags");

            migrationBuilder.DropTable(
                name: "Merchandises");

            migrationBuilder.DropTable(
                name: "Merits_ChargenResources");

            migrationBuilder.DropTable(
                name: "MoveSpeeds");

            migrationBuilder.DropTable(
                name: "MutualIntelligabilities");

            migrationBuilder.DropTable(
                name: "NonCardinalExitTemplates");

            migrationBuilder.DropTable(
                name: "NPCs_ArtificialIntelligences");

            migrationBuilder.DropTable(
                name: "NPCTemplates_ArtificalIntelligences");

            migrationBuilder.DropTable(
                name: "PerceiverMerits");

            migrationBuilder.DropTable(
                name: "PopulationBloodModels_Bloodtypes");

            migrationBuilder.DropTable(
                name: "ProgSchedules");

            migrationBuilder.DropTable(
                name: "ProjectActions");

            migrationBuilder.DropTable(
                name: "ProjectLabourImpacts");

            migrationBuilder.DropTable(
                name: "RaceButcheryProfiles_BreakdownChecks");

            migrationBuilder.DropTable(
                name: "RaceButcheryProfiles_BreakdownEmotes");

            migrationBuilder.DropTable(
                name: "RaceButcheryProfiles_ButcheryProducts");

            migrationBuilder.DropTable(
                name: "RaceButcheryProfiles_SkinningEmotes");

            migrationBuilder.DropTable(
                name: "RaceEdibleForagableYields");

            migrationBuilder.DropTable(
                name: "Races_AdditionalBodyparts");

            migrationBuilder.DropTable(
                name: "Races_AdditionalCharacteristics");

            migrationBuilder.DropTable(
                name: "Races_Attributes");

            migrationBuilder.DropTable(
                name: "Races_BreathableGases");

            migrationBuilder.DropTable(
                name: "Races_BreathableLiquids");

            migrationBuilder.DropTable(
                name: "Races_ChargenResources");

            migrationBuilder.DropTable(
                name: "Races_EdibleMaterials");

            migrationBuilder.DropTable(
                name: "Races_WeaponAttacks");

            migrationBuilder.DropTable(
                name: "RandomNameProfiles_DiceExpressions");

            migrationBuilder.DropTable(
                name: "RandomNameProfiles_Elements");

            migrationBuilder.DropTable(
                name: "RangedWeaponTypes");

            migrationBuilder.DropTable(
                name: "Ranks_Abbreviations");

            migrationBuilder.DropTable(
                name: "Ranks_Paygrades");

            migrationBuilder.DropTable(
                name: "Ranks_Titles");

            migrationBuilder.DropTable(
                name: "RegionalClimates_Seasons");

            migrationBuilder.DropTable(
                name: "Scripts_DesignedLanguages");

            migrationBuilder.DropTable(
                name: "Shards_Calendars");

            migrationBuilder.DropTable(
                name: "Shards_Celestials");

            migrationBuilder.DropTable(
                name: "Shards_Clocks");

            migrationBuilder.DropTable(
                name: "ShieldTypes");

            migrationBuilder.DropTable(
                name: "ShopFinancialPeriodResults");

            migrationBuilder.DropTable(
                name: "Shops_StoreroomCells");

            migrationBuilder.DropTable(
                name: "ShopsTills");

            migrationBuilder.DropTable(
                name: "ShopTransactionRecords");

            migrationBuilder.DropTable(
                name: "SkyDescriptionTemplates_Values");

            migrationBuilder.DropTable(
                name: "Socials");

            migrationBuilder.DropTable(
                name: "StackDecorators");

            migrationBuilder.DropTable(
                name: "StaticConfigurations");

            migrationBuilder.DropTable(
                name: "StaticStrings");

            migrationBuilder.DropTable(
                name: "SurgicalProcedurePhases");

            migrationBuilder.DropTable(
                name: "Terrains_RangedCovers");

            migrationBuilder.DropTable(
                name: "TimeZoneInfos");

            migrationBuilder.DropTable(
                name: "TraitDecorators");

            migrationBuilder.DropTable(
                name: "TraitDefinitions_ChargenResources");

            migrationBuilder.DropTable(
                name: "TraitExpressionParameters");

            migrationBuilder.DropTable(
                name: "Traits");

            migrationBuilder.DropTable(
                name: "UnitOfMeasure");

            migrationBuilder.DropTable(
                name: "VariableDefaults");

            migrationBuilder.DropTable(
                name: "VariableDefinitions");

            migrationBuilder.DropTable(
                name: "VariableValues");

            migrationBuilder.DropTable(
                name: "WearableSizes");

            migrationBuilder.DropTable(
                name: "WearProfiles");

            migrationBuilder.DropTable(
                name: "WitnessProfiles_CooperatingAuthorities");

            migrationBuilder.DropTable(
                name: "WitnessProfiles_IgnoredCriminalClasses");

            migrationBuilder.DropTable(
                name: "WitnessProfiles_IgnoredVictimClasses");

            migrationBuilder.DropTable(
                name: "Writings");

            migrationBuilder.DropTable(
                name: "Zones_Timezones");

            migrationBuilder.DropTable(
                name: "ProjectMaterialRequirements");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "BloodtypeAntigens");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropTable(
                name: "BodypartGroupDescribers");

            migrationBuilder.DropTable(
                name: "Exits");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "CharacteristicValues");

            migrationBuilder.DropTable(
                name: "ChargenAdvices");

            migrationBuilder.DropTable(
                name: "ChargenRoles_ClanMemberships");

            migrationBuilder.DropTable(
                name: "CheckTemplates");

            migrationBuilder.DropTable(
                name: "CombatMessages");

            migrationBuilder.DropTable(
                name: "Crafts");

            migrationBuilder.DropTable(
                name: "CurrencyDescriptionPatternElements");

            migrationBuilder.DropTable(
                name: "Locks");

            migrationBuilder.DropTable(
                name: "Dreams");

            migrationBuilder.DropTable(
                name: "EnforcementAuthorities");

            migrationBuilder.DropTable(
                name: "EntityDescriptions");

            migrationBuilder.DropTable(
                name: "CharacteristicProfiles");

            migrationBuilder.DropTable(
                name: "ClanMemberships");

            migrationBuilder.DropTable(
                name: "ExternalClanControls");

            migrationBuilder.DropTable(
                name: "ForagableProfiles");

            migrationBuilder.DropTable(
                name: "GameItemComponentProtos");

            migrationBuilder.DropTable(
                name: "MagicResources");

            migrationBuilder.DropTable(
                name: "GroupAITemplates");

            migrationBuilder.DropTable(
                name: "Helpfiles");

            migrationBuilder.DropTable(
                name: "Hooks");

            migrationBuilder.DropTable(
                name: "Wounds");

            migrationBuilder.DropTable(
                name: "Laws");

            migrationBuilder.DropTable(
                name: "Limbs");

            migrationBuilder.DropTable(
                name: "MagicSchools");

            migrationBuilder.DropTable(
                name: "NPCs");

            migrationBuilder.DropTable(
                name: "ArtificialIntelligences");

            migrationBuilder.DropTable(
                name: "Merits");

            migrationBuilder.DropTable(
                name: "ButcheryProducts");

            migrationBuilder.DropTable(
                name: "Gases");

            migrationBuilder.DropTable(
                name: "WeaponAttacks");

            migrationBuilder.DropTable(
                name: "RandomNameProfiles");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "SurgicalProcedures");

            migrationBuilder.DropTable(
                name: "RangedCovers");

            migrationBuilder.DropTable(
                name: "ChargenResources");

            migrationBuilder.DropTable(
                name: "LegalClasses");

            migrationBuilder.DropTable(
                name: "WitnessProfiles");

            migrationBuilder.DropTable(
                name: "ChargenRoles");

            migrationBuilder.DropTable(
                name: "CurrencyDescriptionPatterns");

            migrationBuilder.DropTable(
                name: "CurrencyDivisions");

            migrationBuilder.DropTable(
                name: "CharacteristicDefinitions");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "GameItems");

            migrationBuilder.DropTable(
                name: "NPCTemplates");

            migrationBuilder.DropTable(
                name: "WeaponTypes");

            migrationBuilder.DropTable(
                name: "LegalAuthorities");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "Paygrades");

            migrationBuilder.DropTable(
                name: "GameItemProtos");

            migrationBuilder.DropTable(
                name: "Clans");

            migrationBuilder.DropTable(
                name: "ItemGroups");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "LanguageDifficultyModels");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "AuthorityGroups");

            migrationBuilder.DropTable(
                name: "ActiveProjects");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Bodies");

            migrationBuilder.DropTable(
                name: "Chargens");

            migrationBuilder.DropTable(
                name: "Cultures");

            migrationBuilder.DropTable(
                name: "Accents");

            migrationBuilder.DropTable(
                name: "ProjectLabourRequirements");

            migrationBuilder.DropTable(
                name: "Scripts");

            migrationBuilder.DropTable(
                name: "Bloodtypes");

            migrationBuilder.DropTable(
                name: "Ethnicities");

            migrationBuilder.DropTable(
                name: "EntityDescriptionPatterns");

            migrationBuilder.DropTable(
                name: "NameCulture");

            migrationBuilder.DropTable(
                name: "ProjectPhases");

            migrationBuilder.DropTable(
                name: "knowledges");

            migrationBuilder.DropTable(
                name: "Races");

            migrationBuilder.DropTable(
                name: "PopulationBloodModels");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Liquids");

            migrationBuilder.DropTable(
                name: "BloodModels");

            migrationBuilder.DropTable(
                name: "CorpseModels");

            migrationBuilder.DropTable(
                name: "HealthStrategies");

            migrationBuilder.DropTable(
                name: "RaceButcheryProfiles");

            migrationBuilder.DropTable(
                name: "Drugs");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "CellOverlays");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "HearingProfiles");

            migrationBuilder.DropTable(
                name: "Terrains");

            migrationBuilder.DropTable(
                name: "CellOverlayPackages");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "EditableItems");

            migrationBuilder.DropTable(
                name: "Shards");

            migrationBuilder.DropTable(
                name: "WeatherControllers");

            migrationBuilder.DropTable(
                name: "SkyDescriptionTemplates");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "WeatherEvents");

            migrationBuilder.DropTable(
                name: "RegionalClimates");

            migrationBuilder.DropTable(
                name: "Celestials");

            migrationBuilder.DropTable(
                name: "FutureProgs");

            migrationBuilder.DropTable(
                name: "BodypartProto");

            migrationBuilder.DropTable(
                name: "ArmourTypes");

            migrationBuilder.DropTable(
                name: "BodyProtos");

            migrationBuilder.DropTable(
                name: "BodypartShape");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "WearableSizeParameterRule");

            migrationBuilder.DropTable(
                name: "TraitDefinitions");

            migrationBuilder.DropTable(
                name: "TraitExpression");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Calendars");

            migrationBuilder.DropTable(
                name: "EconomicZones");

            migrationBuilder.DropTable(
                name: "FinancialPeriods");

            migrationBuilder.DropTable(
                name: "Timezones");

            migrationBuilder.DropTable(
                name: "Clocks");
        }
    }
}
