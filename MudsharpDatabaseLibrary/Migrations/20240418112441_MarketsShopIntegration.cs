using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MarketsShopIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Zones",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Writings",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Wounds",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WitnessProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeeklyStatistics",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeatherEvents",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeatherControllers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WearProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WearableSizes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WearableSizeParameterRule",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeaponTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeaponAttacks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "UnitOfMeasure",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TraitExpression",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TraitDefinitions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TraitDecorators",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Timezones",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Terrains",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Tags",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "SurgicalProcedures",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "StackDecorators",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "SkyDescriptionTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ShopTransactionRecords",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Shops",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<long>(
                name: "MarketId",
                table: "Shops",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ShieldTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Shards",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "SeederChoices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Seasons",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Scripts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScriptedEvents",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScriptedEventMultipleChoiceQuestions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScriptedEventMultipleChoiceQuestionAnswers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScriptedEventFreeTextQuestions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Rooms",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RegionalClimates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Ranks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RangedWeaponTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RangedCovers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RandomNameProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Races",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RaceButcheryProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertySalesOrders",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertyOwners",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertyLeases",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertyLeaseOrders",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertyKeys",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Properties",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectPhases",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectMaterialRequirements",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectLabourRequirements",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectLabourImpacts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectActions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProgSchedules",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PopulationBloodModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PlayerActivitySnapshots",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PerceiverMerits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Paygrades",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Patrols",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PatrolRoutes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NPCSpawners",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NPCs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NonCardinalExitTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NewPlayerHints",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NameCulture",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MoveSpeeds",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Merits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Merchandises",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<bool>(
                name: "IgnoreMarketPricing",
                table: "Merchandises",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Materials",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Markets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MarketInfluenceTemplates",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MarketInfluences",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MarketCategories",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicSpells",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicSchools",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicResources",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicPowers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicGenerators",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicCapabilities",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Locks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Liquids",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LineOfCreditAccounts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Limbs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LegalClasses",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Laws",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Languages",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LanguageDifficultyModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "knowledges",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "JobListings",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ItemGroups",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ItemGroupForms",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Infections",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Improvers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Hooks_Perceivables",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Hooks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Helpfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "HeightWeightModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "HearingProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "HealthStrategies",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GroupAITemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GroupAIs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Grids",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GPTThreads",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GPTMessages",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Gases",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GameItems",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GameItemComponents",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "FutureProgs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "FinancialPeriods",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Exits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Ethnicities",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EntityDescriptions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EntityDescriptionPatterns",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EnforcementAuthorities",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Elections",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EditableItems",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EconomicZoneTaxes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EconomicZones",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Dubs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Drugs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Dreams",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Drawings",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Doors",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "DamagePatterns",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CurrencyDivisions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CurrencyDescriptionPatterns",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CurrencyDescriptionPatternElements",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Currencies",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Cultures",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Crimes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CraftTools",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CraftProducts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CraftInputs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CorpseModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CombatMessages",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CombatActions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Colours",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Coins",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Clocks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ClimateModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Clans",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CheckTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ChargenScreenStoryboards",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Chargens",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ChargenRoles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ChargenResources",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ChargenAdvices",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Characters",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacterLog",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacterKnowledges",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacteristicValues",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacteristicProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacteristicDefinitions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacterIntroTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacterCombatSettings",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Channels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Cells",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CellOverlays",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Celestials",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Calendars",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ButcheryProducts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ButcheryProductItems",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodyProtos",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartShape",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartProto_OrientationHits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartProto_AlignmentHits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartProto",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartGroupDescribers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Bodies",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Boards",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BoardPosts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Bloodtypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BloodtypeAntigens",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BloodModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Bans",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Banks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BankManagerAuditLogs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BankAccountTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BankAccountTransactions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BankAccounts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "ID",
                table: "AutobuilderRoomTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "ID",
                table: "AutobuilderAreaTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AuthorityGroups",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AuctionHouses",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ArtificialIntelligences",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ArmourTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Areas",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Appointments",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AmmunitionTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ActiveProjects",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ActiveJobs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Accounts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AccountNotes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Accents",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Shops_MarketId",
                table: "Shops",
                column: "MarketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Markets_MarketId",
                table: "Shops",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Markets_MarketId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_MarketId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "IgnoreMarketPricing",
                table: "Merchandises");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Zones",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Writings",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Wounds",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WitnessProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeeklyStatistics",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeatherEvents",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeatherControllers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WearProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WearableSizes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WearableSizeParameterRule",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeaponTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WeaponAttacks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "UnitOfMeasure",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TraitExpression",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TraitDefinitions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TraitDecorators",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Timezones",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Terrains",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Tags",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "SurgicalProcedures",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "StackDecorators",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "SkyDescriptionTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ShopTransactionRecords",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Shops",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ShieldTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Shards",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "SeederChoices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Seasons",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Scripts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScriptedEvents",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScriptedEventMultipleChoiceQuestions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScriptedEventMultipleChoiceQuestionAnswers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScriptedEventFreeTextQuestions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Rooms",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RegionalClimates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Ranks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RangedWeaponTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RangedCovers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RandomNameProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Races",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RaceButcheryProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertySalesOrders",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertyOwners",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertyLeases",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertyLeaseOrders",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PropertyKeys",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Properties",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectPhases",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectMaterialRequirements",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectLabourRequirements",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectLabourImpacts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProjectActions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ProgSchedules",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PopulationBloodModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PlayerActivitySnapshots",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PerceiverMerits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Paygrades",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Patrols",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PatrolRoutes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NPCSpawners",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NPCs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NonCardinalExitTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NewPlayerHints",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "NameCulture",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MoveSpeeds",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Merits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Merchandises",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Materials",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Markets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MarketInfluenceTemplates",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MarketInfluences",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MarketCategories",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicSpells",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicSchools",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicResources",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicPowers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicGenerators",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MagicCapabilities",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Locks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Liquids",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LineOfCreditAccounts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Limbs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LegalClasses",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Laws",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Languages",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LanguageDifficultyModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "knowledges",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "JobListings",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ItemGroups",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ItemGroupForms",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Infections",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Improvers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Hooks_Perceivables",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Hooks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Helpfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "HeightWeightModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "HearingProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "HealthStrategies",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GroupAITemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GroupAIs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Grids",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GPTThreads",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GPTMessages",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Gases",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GameItems",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GameItemComponents",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "FutureProgs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "FinancialPeriods",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Exits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Ethnicities",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EntityDescriptions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EntityDescriptionPatterns",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EnforcementAuthorities",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Elections",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EditableItems",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EconomicZoneTaxes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EconomicZones",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Dubs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Drugs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Dreams",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Drawings",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Doors",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "DamagePatterns",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CurrencyDivisions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CurrencyDescriptionPatterns",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CurrencyDescriptionPatternElements",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Currencies",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Cultures",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Crimes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CraftTools",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CraftProducts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CraftInputs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CorpseModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CombatMessages",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CombatActions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Colours",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Coins",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Clocks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ClimateModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Clans",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CheckTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ChargenScreenStoryboards",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Chargens",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ChargenRoles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ChargenResources",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ChargenAdvices",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Characters",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacterLog",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacterKnowledges",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacteristicValues",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacteristicProfiles",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacteristicDefinitions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacterIntroTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CharacterCombatSettings",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Channels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Cells",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "CellOverlays",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Celestials",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Calendars",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ButcheryProducts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ButcheryProductItems",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodyProtos",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartShape",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartProto_OrientationHits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartProto_AlignmentHits",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartProto",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BodypartGroupDescribers",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Bodies",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Boards",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BoardPosts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Bloodtypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BloodtypeAntigens",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BloodModels",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Bans",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Banks",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BankManagerAuditLogs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BankAccountTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BankAccountTransactions",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BankAccounts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "ID",
                table: "AutobuilderRoomTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "ID",
                table: "AutobuilderAreaTemplates",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AuthorityGroups",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AuctionHouses",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ArtificialIntelligences",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ArmourTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Areas",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Appointments",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AmmunitionTypes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ActiveProjects",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ActiveJobs",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Accounts",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AccountNotes",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Accents",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
