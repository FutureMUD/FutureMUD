using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class EnforcermentAndMisc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Zones",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WritingType",
                table: "Writings",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Writings",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WoundType",
                table: "Wounds",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraInformation",
                table: "Wounds",
                type: "text",
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WitnessProfiles",
                type: "varchar(250)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherRoomAddendum",
                table: "WeatherEvents",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherEventType",
                table: "WeatherEvents",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherDescription",
                table: "WeatherEvents",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeatherEvents",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "WeatherEvents",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeatherControllers",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WearlocProfiles",
                table: "WearProfiles",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WearStringInventory",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'worn on'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "'worn on'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WearAffix",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'on'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "'on'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WearAction3rd",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'puts'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "'puts'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WearAction1st",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'put'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "'put'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "WearProfiles",
                type: "varchar(50)",
                nullable: false,
                defaultValueSql: "'Direct'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldDefaultValueSql: "'Direct'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WearProfiles",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WeightVolumeRatios",
                table: "WearableSizeParameterRule",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TraitVolumeRatios",
                table: "WearableSizeParameterRule",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "HeightLinearRatios",
                table: "WearableSizeParameterRule",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeaponTypes",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeaponAttacks",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "WeaponAttacks",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ValueDefinition",
                table: "VariableValues",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceProperty",
                table: "VariableValues",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Property",
                table: "VariableDefinitions",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultValue",
                table: "VariableDefaults",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Property",
                table: "VariableDefaults",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "System",
                table: "UnitOfMeasure",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryAbbreviation",
                table: "UnitOfMeasure",
                type: "varchar(45)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UnitOfMeasure",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviations",
                table: "UnitOfMeasure",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter",
                table: "TraitExpressionParameters",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TraitExpression",
                type: "varchar(200)",
                nullable: false,
                defaultValueSql: "'Unnamed Expression'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldDefaultValueSql: "'Unnamed Expression'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Expression",
                table: "TraitExpression",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ValueExpression",
                table: "TraitDefinitions",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TraitGroup",
                table: "TraitDefinitions",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TraitDefinitions",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ChargenBlurb",
                table: "TraitDefinitions",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "TraitDefinitions",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "TraitDecorators",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TraitDecorators",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Contents",
                table: "TraitDecorators",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Timezones",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Timezones",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Display",
                table: "TimeZoneInfos",
                type: "varchar(1000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TimeZoneInfos",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TerrainEditorColour",
                table: "Terrains",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'#FFFFFFFF'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'#FFFFFFFF'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TerrainBehaviourMode",
                table: "Terrains",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Terrains",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AtmosphereType",
                table: "Terrains",
                type: "varchar(45)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureName",
                table: "SurgicalProcedures",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureGerund",
                table: "SurgicalProcedures",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureDescriptionEmote",
                table: "SurgicalProcedures",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureBeginEmote",
                table: "SurgicalProcedures",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SurgicalProcedures",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalSchool",
                table: "SurgicalProcedures",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "SurgicalProcedures",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PhaseSpecialEffects",
                table: "SurgicalProcedurePhases",
                type: "varchar(500)",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PhaseEmote",
                table: "SurgicalProcedurePhases",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "InventoryActionPlan",
                table: "SurgicalProcedurePhases",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "StaticStrings",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StaticStrings",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "StaticConfigurations",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SettingName",
                table: "StaticConfigurations",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "StackDecorators",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StackDecorators",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "StackDecorators",
                type: "varchar(1000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "StackDecorators",
                type: "varchar(10000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(10000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "OneTargetEcho",
                table: "Socials",
                type: "varchar(1000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "NoTargetEcho",
                table: "Socials",
                type: "varchar(1000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MultiTargetEcho",
                table: "Socials",
                type: "varchar(1000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DirectionTargetEcho",
                table: "Socials",
                type: "varchar(1000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Socials",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SkyDescriptionTemplates_Values",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SkyDescriptionTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MudDateTime",
                table: "ShopTransactionRecords",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shops",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeRecords",
                table: "Shops",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShieldTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shards",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Seasons",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Seasons",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "UnknownScriptDescription",
                table: "Scripts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Scripts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "KnownScriptDescription",
                table: "Scripts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TemperatureInfo",
                table: "RegionalClimates_Seasons",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RegionalClimates",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Ranks_Titles",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Ranks_Abbreviations",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "RankPath",
                table: "Ranks",
                type: "varchar(50)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ranks",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SpecificAmmunitionGrade",
                table: "RangedWeaponTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RangedWeaponTypes",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DamageBonusExpression",
                table: "RangedWeaponTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AccuracyBonusExpression",
                table: "RangedWeaponTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RangedCovers",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionString",
                table: "RangedCovers",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ActionDescriptionString",
                table: "RangedCovers",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RandomNameProfiles_Elements",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DiceExpression",
                table: "RandomNameProfiles_DiceExpressions",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RandomNameProfiles",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Usage",
                table: "Races_AdditionalCharacteristics",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Usage",
                table: "Races_AdditionalBodyparts",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Races",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MaximumLiftWeightExpression",
                table: "Races",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MaximumDragWeightExpression",
                table: "Races",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "HoldBreathLengthExpression",
                table: "Races",
                type: "varchar(500)",
                nullable: false,
                defaultValueSql: "'120'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldDefaultValueSql: "'120'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "HandednessOptions",
                table: "Races",
                type: "varchar(100)",
                nullable: false,
                defaultValueSql: "'1 3'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldDefaultValueSql: "'1 3'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EatCorpseEmoteText",
                table: "Races",
                type: "varchar(500)",
                nullable: false,
                defaultValueSql: "'@ eat|eats {0}$1'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldDefaultValueSql: "'@ eat|eats {0}$1'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DiceExpression",
                table: "Races",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Races",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CommunicationStrategyType",
                table: "Races",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'humanoid'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'humanoid'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "BreathingVolumeExpression",
                table: "Races",
                type: "varchar(500)",
                nullable: false,
                defaultValueSql: "'7'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldDefaultValueSql: "'7'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AllowedGenders",
                table: "Races",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EatEmote",
                table: "RaceEdibleForagableYields",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'@ eat|eats {0} from the location.'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'@ eat|eats {0} from the location.'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "YieldType",
                table: "RaceEdibleForagableYields",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Emote",
                table: "RaceButcheryProfiles_SkinningEmotes",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "RaceButcheryProfiles_SkinningEmotes",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Emote",
                table: "RaceButcheryProfiles_BreakdownEmotes",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "RaceButcheryProfiles_BreakdownEmotes",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subcageory",
                table: "RaceButcheryProfiles_BreakdownChecks",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RaceButcheryProfiles",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Projects",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Projects",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectPhases",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ProjectMaterialRequirements",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectMaterialRequirements",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectMaterialRequirements",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ProjectMaterialRequirements",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ProjectLabourRequirements",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectLabourRequirements",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectLabourRequirements",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ProjectLabourRequirements",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ProjectLabourImpacts",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectLabourImpacts",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectLabourImpacts",
                type: "varchar(1000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ProjectLabourImpacts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ProjectActions",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectActions",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectActions",
                type: "varchar(1000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ProjectActions",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceTime",
                table: "ProgSchedules",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceDate",
                table: "ProgSchedules",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProgSchedules",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PopulationBloodModels",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Paygrades",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Paygrades",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AddColumn<long>(
                name: "StartPatrolProgId",
                table: "PatrolRoutes",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "NPCTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NPCTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "NPCTemplates",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundVerb",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "OriginOutboundPreface",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "OriginInboundPreface",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "InboundVerb",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationOutboundPreface",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationInboundPreface",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NameCulture",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "NameCulture",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPersonVerb",
                table: "MoveSpeeds",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PresentParticiple",
                table: "MoveSpeeds",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FirstPersonVerb",
                table: "MoveSpeeds",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "MoveSpeeds",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Merits",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Merits",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Merits",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Merchandises",
                type: "varchar(1000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ListDescription",
                table: "Merchandises",
                type: "varchar(500)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ResidueSdesc",
                table: "Materials",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ResidueDesc",
                table: "Materials",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ResidueColour",
                table: "Materials",
                type: "varchar(45)",
                nullable: true,
                defaultValueSql: "'white'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldDefaultValueSql: "'white'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Materials",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialDescription",
                table: "Materials",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SchoolVerb",
                table: "MagicSchools",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SchoolAdjective",
                table: "MagicSchools",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PowerListColour",
                table: "MagicSchools",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicSchools",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "MagicResources",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicResources",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "MagicResources",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ShowHelp",
                table: "MagicPowers",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PowerModel",
                table: "MagicPowers",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicPowers",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "MagicPowers",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Blurb",
                table: "MagicPowers",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "MagicGenerators",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicGenerators",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "MagicGenerators",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicCapabilities",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "MagicCapabilities",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CapabilityModel",
                table: "MagicCapabilities",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "LoginIPs",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Locks",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WetShortDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WetDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "VagueTasteText",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "VagueSmellText",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TasteText",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SmellText",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Liquids",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LongDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DrenchedShortDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DrenchedDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayColour",
                table: "Liquids",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'blue'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'blue'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DampShortDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DampDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AccountUserName",
                table: "LineOfCreditAccountUsers",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AccountOwnerName",
                table: "LineOfCreditAccounts",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Limbs",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LegalClasses",
                type: "varchar(250)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LegalAuthorities",
                type: "varchar(250)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AddColumn<long>(
                name: "EnforcerStowingLocationId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MarshallingLocationId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "PlayersKnowTheirCrimes",
                table: "LegalAuthorities",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");

            migrationBuilder.AddColumn<long>(
                name: "PreparingLocationId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrisonLocationId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Laws",
                type: "varchar(250)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EnforcementStrategy",
                table: "Laws",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "UnknownLanguageDescription",
                table: "Languages",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Languages",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "LanguageDifficultyModels",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LanguageDifficultyModels",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "LanguageDifficultyModels",
                type: "longtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "knowledges",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subtype",
                table: "knowledges",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "knowledges",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LongDescription",
                table: "knowledges",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "knowledges",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ItemGroups",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "ItemGroups",
                type: "varchar(1000)",
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ItemGroupForms",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ItemGroupForms",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Improvers",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Improvers",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Improvers",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Hooks",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Hooks",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Hooks",
                type: "text",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Hooks",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Helpfiles_ExtraTexts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TagLine",
                table: "Helpfiles",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "Helpfiles",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PublicText",
                table: "Helpfiles",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Helpfiles",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LastEditedBy",
                table: "Helpfiles",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "Helpfiles",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Helpfiles",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HeightWeightModels",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "HearingProfiles",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SurveyDescription",
                table: "HearingProfiles",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HearingProfiles",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "HearingProfiles",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "HealthStrategies",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HealthStrategies",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "HealthStrategies",
                type: "text",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupAITemplates",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "GroupAITemplates",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupAIs",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "GroupAIs",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "GroupAIs",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "GridType",
                table: "Grids",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Grids",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "VagueSmellText",
                table: "Gases",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SmellText",
                table: "Gases",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Gases",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayColour",
                table: "Gases",
                type: "varchar(40)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(40)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Gases",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PositionTargetType",
                table: "GameItems",
                type: "varchar(45)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PositionEmote",
                table: "GameItems",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EffectData",
                table: "GameItems",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "VariableValue",
                table: "GameItemProtos_DefaultVariables",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "VariableName",
                table: "GameItemProtos_DefaultVariables",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "GameItemProtos",
                type: "varchar(1000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GameItemProtos",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MorphEmote",
                table: "GameItemProtos",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'$0 $?1|morphs into $1|decays into nothing$.'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'$0 $?1|morphs into $1|decays into nothing$.'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LongDescription",
                table: "GameItemProtos",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "GameItemProtos",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "GameItemProtos",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CustomColour",
                table: "GameItemProtos",
                type: "varchar(45)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "GameItemComponents",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "GameItemComponentProtos",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GameItemComponentProtos",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GameItemComponentProtos",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "GameItemComponentProtos",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ParameterName",
                table: "FutureProgs_Parameters",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "FutureProgs",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FunctionText",
                table: "FutureProgs",
                type: "text",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FunctionName",
                table: "FutureProgs",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FunctionComment",
                table: "FutureProgs",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "FutureProgs",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "QuantityDiceExpression",
                table: "Foragables",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Foragables",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ForagableTypes",
                table: "Foragables",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ForageType",
                table: "ForagableProfiles_MaximumYields",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ForageType",
                table: "ForagableProfiles_HourlyYieldGains",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ForagableProfiles",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MudPeriodStart",
                table: "FinancialPeriods",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MudPeriodEnd",
                table: "FinancialPeriods",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Verb2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Verb1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryKeyword2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryKeyword1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundTarget2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundTarget1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundDescription2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundDescription1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "InboundTarget2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "InboundTarget1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "InboundDescription2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "InboundDescription1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "BlockedLayers",
                table: "Exits",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "''",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ethnicities",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EthnicSubgroup",
                table: "Ethnicities",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EthnicGroup",
                table: "Ethnicities",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ChargenBlurb",
                table: "Ethnicities",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "EntityDescriptions",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "EntityDescriptions",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "EntityDescriptionPatterns",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EnforcementAuthorities",
                type: "varchar(250)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "EmailTemplates",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ReturnAddress",
                table: "EmailTemplates",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "EmailTemplates",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewerComment",
                table: "EditableItems",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "BuilderComment",
                table: "EditableItems",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "EconomicZoneTaxes",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceTime",
                table: "EconomicZones",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EconomicZones",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TargetType",
                table: "Dubs",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LastDescription",
                table: "Dubs",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "Dubs",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "IntroducedName",
                table: "Dubs",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalEffects",
                table: "DrugsIntensities",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Drugs",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Dreams",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DreamerText",
                table: "Dream_Phases",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DreamerCommand",
                table: "Dream_Phases",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Drawings",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Drawings",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Doors",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "DisfigurementTemplates",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "DisfigurementTemplates",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DisfigurementTemplates",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "DisfigurementTemplates",
                type: "varchar(5000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(5000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "DisfigurementTemplates",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PerceivableType",
                table: "DefaultHooks",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DamagePatterns",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CurrencyDivisions",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "CurrencyDivisionAbbreviations",
                type: "varchar(150)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(150)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "NegativePrefix",
                table: "CurrencyDescriptionPatterns",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "CurrencyDescriptionPatternElementSpecialValues",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PluraliseWord",
                table: "CurrencyDescriptionPatternElements",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "CurrencyDescriptionPatternElements",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AlternatePattern",
                table: "CurrencyDescriptionPatternElements",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currencies",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PersonWordNeuter",
                table: "Cultures",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PersonWordMale",
                table: "Cultures",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PersonWordIndeterminate",
                table: "Cultures",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PersonWordFemale",
                table: "Cultures",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cultures",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Cultures",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "CultureInfos",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CultureInfos",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TimeOfReport",
                table: "Crimes",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TimeOfCrime",
                table: "Crimes",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CriminalShortDescription",
                table: "Crimes",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CriminalFullDescription",
                table: "Crimes",
                type: "varchar(1000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CriminalCharacteristics",
                table: "Crimes",
                type: "varchar(500)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(500)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ToolType",
                table: "CraftTools",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CraftTools",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "QualityFormula",
                table: "Crafts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Crafts",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Crafts",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Blurb",
                table: "Crafts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ActiveCraftItemSDesc",
                table: "Crafts",
                type: "varchar(200)",
                nullable: false,
                defaultValueSql: "'a craft in progress'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldDefaultValueSql: "'a craft in progress'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ActionDescription",
                table: "Crafts",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ProductType",
                table: "CraftProducts",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CraftProducts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FailEcho",
                table: "CraftPhases",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Echo",
                table: "CraftPhases",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "InputType",
                table: "CraftInputs",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CraftInputs",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CorpseModels",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CorpseModels",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CorpseModels",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CorpseModels",
                type: "text",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "CombatMessages",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FailureMessage",
                table: "CombatMessages",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Colours",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Fancy",
                table: "Colours",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Coins",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PluralWord",
                table: "Coins",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Coins",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "GeneralForm",
                table: "Coins",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Coins",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Clocks",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ClimateModels",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ClimateModels",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ClimateModels",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PayIntervalReferenceTime",
                table: "Clans",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PayIntervalReferenceDate",
                table: "Clans",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Clans",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Clans",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Clans",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "Clans",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PersonalName",
                table: "ClanMemberships",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "JoinDate",
                table: "ClanMemberships",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CheckTemplates",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CheckTemplates",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CheckMethod",
                table: "CheckTemplates",
                type: "varchar(25)",
                nullable: false,
                defaultValueSql: "'Standard'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldDefaultValueSql: "'Standard'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "StageDefinition",
                table: "ChargenScreenStoryboards",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Chargens",
                type: "varchar(12000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(12000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Chargens",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ChargenRoles",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ChargenBlurb",
                table: "ChargenRoles",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ChargenResources",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TextDisplayedToPlayerOnDeduct",
                table: "ChargenResources",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TextDisplayedToPlayerOnAward",
                table: "ChargenResources",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PluralName",
                table: "ChargenResources",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ChargenResources",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MaximumResourceFormula",
                table: "ChargenResources",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "ChargenResources",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AdviceTitle",
                table: "ChargenAdvices",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AdviceText",
                table: "ChargenAdvices",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ShortTermPlan",
                table: "Characters",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PositionTargetType",
                table: "Characters",
                type: "varchar(45)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PositionEmote",
                table: "Characters",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Outfits",
                table: "Characters",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "NeedsModel",
                table: "Characters",
                type: "varchar(50)",
                nullable: false,
                defaultValueSql: "'NoNeeds'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldDefaultValueSql: "'NoNeeds'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "NameInfo",
                table: "Characters",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LongTermPlan",
                table: "Characters",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "IntroductionMessage",
                table: "Characters",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EffectData",
                table: "Characters",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "BirthdayDate",
                table: "Characters",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Command",
                table: "CharacterLog",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "HowAcquired",
                table: "CharacterKnowledges",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "CharacteristicValues",
                type: "varchar(50)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacteristicValues",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalValue",
                table: "CharacteristicValues",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CharacteristicProfiles",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacteristicProfiles",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CharacteristicProfiles",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CharacteristicProfiles",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "CharacteristicDefinitions",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacteristicDefinitions",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "CharacteristicDefinitions",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'standard'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'standard'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CharacteristicDefinitions",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CharacteristicDefinitions",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacterIntroTemplates",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CharacterIntroTemplates",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacterCombatSettings",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MeleeAttackOrderPreference",
                table: "CharacterCombatSettings",
                type: "varchar(100)",
                nullable: false,
                defaultValueSql: "'0 1 2 3 4'",
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldDefaultValueSql: "'0 1 2 3 4'")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CharacterCombatSettings",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ClassificationsAllowed",
                table: "CharacterCombatSettings",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelName",
                table: "Channels",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelColour",
                table: "Channels",
                type: "char(10)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "char(10)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Word",
                table: "ChannelCommandWords",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ForagableType",
                table: "Cells_ForagableYields",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EffectData",
                table: "Cells",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CellOverlays",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CellName",
                table: "CellOverlays",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CellDescription",
                table: "CellOverlays",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "AtmosphereType",
                table: "CellOverlays",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'gas'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'gas'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CellOverlayPackages",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Celestials",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Calendars",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                table: "Calendars",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "ButcheryProducts",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ButcheryProducts",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ButcheryProductItemscol",
                table: "ButcheryProductItems",
                type: "varchar(45)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Usage",
                table: "BodyProtos_AdditionalBodyparts",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WielderDescriptionSingle",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: false,
                defaultValueSql: "'hand'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldDefaultValueSql: "'hand'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "WielderDescriptionPlural",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: false,
                defaultValueSql: "'hands'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldDefaultValueSql: "'hands'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LegDescriptionSingular",
                table: "BodyProtos",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'leg'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'leg'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LegDescriptionPlural",
                table: "BodyProtos",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'legs'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'legs'")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ConsiderString",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BodypartShape",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BodypartProto",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BodypartProto",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ProximityGroup",
                table: "BodypartInternalInfos",
                type: "varchar(45)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "BodypartGroupDescribers",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DescribedAs",
                table: "BodypartGroupDescribers",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "BodypartGroupDescribers",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Tattoos",
                table: "Bodies",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Bodies",
                type: "varchar(1000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Scars",
                table: "Bodies",
                type: "mediumtext",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Bodies",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "EffectData",
                table: "Bodies",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Boards",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BoardPosts",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "BoardPosts",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Bloodtypes",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BloodtypeAntigens",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BloodModels",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Bans",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "IpMask",
                table: "Bans",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TemplateType",
                table: "AutobuilderRoomTemplates",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AutobuilderRoomTemplates",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "AutobuilderRoomTemplates",
                type: "longtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TemplateType",
                table: "AutobuilderAreaTemplates",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AutobuilderAreaTemplates",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "AutobuilderAreaTemplates",
                type: "longtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AuthorityGroups",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ArtificialIntelligences",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ArtificialIntelligences",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ArtificialIntelligences",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "mediumtext")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ArmourTypes",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ArmourTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Areas",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Appointments_Titles",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Appointments_Abbreviations",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Appointments",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "StunExpression",
                table: "AmmunitionTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SpecificType",
                table: "AmmunitionTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "RangedWeaponTypes",
                table: "AmmunitionTypes",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "PainExpression",
                table: "AmmunitionTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AmmunitionTypes",
                type: "varchar(200)",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "DamageExpression",
                table: "AmmunitionTypes",
                type: "text",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "UnitPreference",
                table: "Accounts",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TimeZoneId",
                table: "Accounts",
                type: "varchar(100)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationCode",
                table: "Accounts",
                type: "varchar(50)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "RecoveryCode",
                table: "Accounts",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Accounts",
                type: "varchar(1000)",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "LastLoginIP",
                table: "Accounts",
                type: "varchar(50)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Accounts",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CultureName",
                table: "Accounts",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AddColumn<ulong>(
                name: "ActLawfully",
                table: "Accounts",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "AccountNotes",
                type: "text",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "AccountNotes",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "VagueSuffix",
                table: "Accents",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Suffix",
                table: "Accents",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accents",
                type: "varchar(50)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Group",
                table: "Accents",
                type: "varchar(50)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Accents",
                type: "varchar(4000)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "LegalAuthoritiyCells",
                columns: table => new
                {
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LegalAuthorityId, x.CellId });
                    table.ForeignKey(
                        name: "FK_LegalAuthoritiesCells_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalAuthoritiesCells_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patrols",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatrolRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PatrolPhase = table.Column<int>(type: "int(11)", nullable: false),
                    LastMajorNodeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NextMajorNodeId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PatrolLeaderId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patrols_Characters",
                        column: x => x.PatrolLeaderId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patrols_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patrols_LastMajorNode",
                        column: x => x.LastMajorNodeId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patrols_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patrols_NextMajorNode",
                        column: x => x.NextMajorNodeId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patrols_PatrolRoutes",
                        column: x => x.PatrolRouteId,
                        principalTable: "PatrolRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatrolMembers",
                columns: table => new
                {
                    PatrolId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.PatrolId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_PatrolMembers_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatrolsMembers_Patrols",
                        column: x => x.PatrolId,
                        principalTable: "Patrols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatrolRoutes_StartPatrolProgId",
                table: "PatrolRoutes",
                column: "StartPatrolProgId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_MarshallingCells_idx",
                table: "LegalAuthorities",
                column: "MarshallingLocationId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_PreparingCells_idx",
                table: "LegalAuthorities",
                column: "PreparingLocationId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_PrisonCells_idx",
                table: "LegalAuthorities",
                column: "PrisonLocationId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_StowingCells_idx",
                table: "LegalAuthorities",
                column: "EnforcerStowingLocationId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthoritiesCells_Cells_idx",
                table: "LegalAuthoritiyCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthoritiesCells_LegalAuthorities_idx",
                table: "LegalAuthoritiyCells",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_PatrolMembers_CharacterId",
                table: "PatrolMembers",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_Patrols_Characters_idx",
                table: "Patrols",
                column: "PatrolLeaderId");

            migrationBuilder.CreateIndex(
                name: "FK_Patrols_LastMajorNode_idx",
                table: "Patrols",
                column: "LastMajorNodeId");

            migrationBuilder.CreateIndex(
                name: "FK_Patrols_LegalAuthorities_idx",
                table: "Patrols",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_Patrols_NextMajorNode_idx",
                table: "Patrols",
                column: "NextMajorNodeId");

            migrationBuilder.CreateIndex(
                name: "FK_Patrols_PatrolRoutes_idx",
                table: "Patrols",
                column: "PatrolRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Patrols_CharacterId",
                table: "Patrols",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_MarshallingCells",
                table: "LegalAuthorities",
                column: "MarshallingLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_PreparingCells",
                table: "LegalAuthorities",
                column: "PreparingLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_PrisonCells",
                table: "LegalAuthorities",
                column: "PrisonLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_StowingCells",
                table: "LegalAuthorities",
                column: "EnforcerStowingLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatrolRoutes_FutureProgs_StartPatrolProgId",
                table: "PatrolRoutes",
                column: "StartPatrolProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_MarshallingCells",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_PreparingCells",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_PrisonCells",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_StowingCells",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_PatrolRoutes_FutureProgs_StartPatrolProgId",
                table: "PatrolRoutes");

            migrationBuilder.DropTable(
                name: "LegalAuthoritiyCells");

            migrationBuilder.DropTable(
                name: "PatrolMembers");

            migrationBuilder.DropTable(
                name: "Patrols");

            migrationBuilder.DropIndex(
                name: "IX_PatrolRoutes_StartPatrolProgId",
                table: "PatrolRoutes");

            migrationBuilder.DropIndex(
                name: "FK_LegalAuthorities_MarshallingCells_idx",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "FK_LegalAuthorities_PreparingCells_idx",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "FK_LegalAuthorities_PrisonCells_idx",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "FK_LegalAuthorities_StowingCells_idx",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "StartPatrolProgId",
                table: "PatrolRoutes");

            migrationBuilder.DropColumn(
                name: "EnforcerStowingLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "MarshallingLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "PlayersKnowTheirCrimes",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "PreparingLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "PrisonLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "ActLawfully",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Zones",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WritingType",
                table: "Writings",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Writings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WoundType",
                table: "Wounds",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraInformation",
                table: "Wounds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WitnessProfiles",
                type: "varchar(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherRoomAddendum",
                table: "WeatherEvents",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherEventType",
                table: "WeatherEvents",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherDescription",
                table: "WeatherEvents",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeatherEvents",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "WeatherEvents",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeatherControllers",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WearlocProfiles",
                table: "WearProfiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WearStringInventory",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'worn on'",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "'worn on'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WearAffix",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'on'",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "'on'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WearAction3rd",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'puts'",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "'puts'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WearAction1st",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'put'",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "'put'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "WearProfiles",
                type: "varchar(50)",
                nullable: false,
                defaultValueSql: "'Direct'",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldDefaultValueSql: "'Direct'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WearProfiles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WearProfiles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WeightVolumeRatios",
                table: "WearableSizeParameterRule",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TraitVolumeRatios",
                table: "WearableSizeParameterRule",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "HeightLinearRatios",
                table: "WearableSizeParameterRule",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeaponTypes",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeaponAttacks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "WeaponAttacks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ValueDefinition",
                table: "VariableValues",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceProperty",
                table: "VariableValues",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Property",
                table: "VariableDefinitions",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultValue",
                table: "VariableDefaults",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Property",
                table: "VariableDefaults",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "System",
                table: "UnitOfMeasure",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryAbbreviation",
                table: "UnitOfMeasure",
                type: "varchar(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UnitOfMeasure",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviations",
                table: "UnitOfMeasure",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Parameter",
                table: "TraitExpressionParameters",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TraitExpression",
                type: "varchar(200)",
                nullable: false,
                defaultValueSql: "'Unnamed Expression'",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldDefaultValueSql: "'Unnamed Expression'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Expression",
                table: "TraitExpression",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ValueExpression",
                table: "TraitDefinitions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TraitGroup",
                table: "TraitDefinitions",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TraitDefinitions",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ChargenBlurb",
                table: "TraitDefinitions",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "TraitDefinitions",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "TraitDecorators",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TraitDecorators",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Contents",
                table: "TraitDecorators",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Timezones",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Timezones",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Display",
                table: "TimeZoneInfos",
                type: "varchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TimeZoneInfos",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TerrainEditorColour",
                table: "Terrains",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'#FFFFFFFF'",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'#FFFFFFFF'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TerrainBehaviourMode",
                table: "Terrains",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Terrains",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AtmosphereType",
                table: "Terrains",
                type: "varchar(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureName",
                table: "SurgicalProcedures",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureGerund",
                table: "SurgicalProcedures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureDescriptionEmote",
                table: "SurgicalProcedures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureBeginEmote",
                table: "SurgicalProcedures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SurgicalProcedures",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalSchool",
                table: "SurgicalProcedures",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "SurgicalProcedures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PhaseSpecialEffects",
                table: "SurgicalProcedurePhases",
                type: "varchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PhaseEmote",
                table: "SurgicalProcedurePhases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "InventoryActionPlan",
                table: "SurgicalProcedurePhases",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "StaticStrings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StaticStrings",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "StaticConfigurations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "SettingName",
                table: "StaticConfigurations",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "StackDecorators",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StackDecorators",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "StackDecorators",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "StackDecorators",
                type: "varchar(10000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "OneTargetEcho",
                table: "Socials",
                type: "varchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "NoTargetEcho",
                table: "Socials",
                type: "varchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "MultiTargetEcho",
                table: "Socials",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DirectionTargetEcho",
                table: "Socials",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Socials",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SkyDescriptionTemplates_Values",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SkyDescriptionTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "MudDateTime",
                table: "ShopTransactionRecords",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shops",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeRecords",
                table: "Shops",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShieldTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shards",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Seasons",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Seasons",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "UnknownScriptDescription",
                table: "Scripts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Scripts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "KnownScriptDescription",
                table: "Scripts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TemperatureInfo",
                table: "RegionalClimates_Seasons",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RegionalClimates",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Ranks_Titles",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Ranks_Abbreviations",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "RankPath",
                table: "Ranks",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ranks",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "SpecificAmmunitionGrade",
                table: "RangedWeaponTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RangedWeaponTypes",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DamageBonusExpression",
                table: "RangedWeaponTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccuracyBonusExpression",
                table: "RangedWeaponTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RangedCovers",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionString",
                table: "RangedCovers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ActionDescriptionString",
                table: "RangedCovers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RandomNameProfiles_Elements",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DiceExpression",
                table: "RandomNameProfiles_DiceExpressions",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RandomNameProfiles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Usage",
                table: "Races_AdditionalCharacteristics",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Usage",
                table: "Races_AdditionalBodyparts",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Races",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "MaximumLiftWeightExpression",
                table: "Races",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "MaximumDragWeightExpression",
                table: "Races",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "HoldBreathLengthExpression",
                table: "Races",
                type: "varchar(500)",
                nullable: false,
                defaultValueSql: "'120'",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldDefaultValueSql: "'120'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "HandednessOptions",
                table: "Races",
                type: "varchar(100)",
                nullable: false,
                defaultValueSql: "'1 3'",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldDefaultValueSql: "'1 3'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EatCorpseEmoteText",
                table: "Races",
                type: "varchar(500)",
                nullable: false,
                defaultValueSql: "'@ eat|eats {0}$1'",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldDefaultValueSql: "'@ eat|eats {0}$1'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DiceExpression",
                table: "Races",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Races",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CommunicationStrategyType",
                table: "Races",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'humanoid'",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'humanoid'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "BreathingVolumeExpression",
                table: "Races",
                type: "varchar(500)",
                nullable: false,
                defaultValueSql: "'7'",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldDefaultValueSql: "'7'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AllowedGenders",
                table: "Races",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EatEmote",
                table: "RaceEdibleForagableYields",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'@ eat|eats {0} from the location.'",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'@ eat|eats {0} from the location.'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "YieldType",
                table: "RaceEdibleForagableYields",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Emote",
                table: "RaceButcheryProfiles_SkinningEmotes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "RaceButcheryProfiles_SkinningEmotes",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Emote",
                table: "RaceButcheryProfiles_BreakdownEmotes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "RaceButcheryProfiles_BreakdownEmotes",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subcageory",
                table: "RaceButcheryProfiles_BreakdownChecks",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RaceButcheryProfiles",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Projects",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Projects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectPhases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ProjectMaterialRequirements",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectMaterialRequirements",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectMaterialRequirements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ProjectMaterialRequirements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ProjectLabourRequirements",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectLabourRequirements",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectLabourRequirements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ProjectLabourRequirements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ProjectLabourImpacts",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectLabourImpacts",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectLabourImpacts",
                type: "varchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ProjectLabourImpacts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ProjectActions",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectActions",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectActions",
                type: "varchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ProjectActions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceTime",
                table: "ProgSchedules",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceDate",
                table: "ProgSchedules",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProgSchedules",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PopulationBloodModels",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Paygrades",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Paygrades",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "NPCTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NPCTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "NPCTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundVerb",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "OriginOutboundPreface",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "OriginInboundPreface",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "InboundVerb",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationOutboundPreface",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationInboundPreface",
                table: "NonCardinalExitTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NameCulture",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "NameCulture",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ThirdPersonVerb",
                table: "MoveSpeeds",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PresentParticiple",
                table: "MoveSpeeds",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FirstPersonVerb",
                table: "MoveSpeeds",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "MoveSpeeds",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Merits",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Merits",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Merits",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Merchandises",
                type: "varchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ListDescription",
                table: "Merchandises",
                type: "varchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ResidueSdesc",
                table: "Materials",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ResidueDesc",
                table: "Materials",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ResidueColour",
                table: "Materials",
                type: "varchar(45)",
                nullable: true,
                defaultValueSql: "'white'",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldDefaultValueSql: "'white'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Materials",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialDescription",
                table: "Materials",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "SchoolVerb",
                table: "MagicSchools",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "SchoolAdjective",
                table: "MagicSchools",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PowerListColour",
                table: "MagicSchools",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicSchools",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "MagicResources",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicResources",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "MagicResources",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ShowHelp",
                table: "MagicPowers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PowerModel",
                table: "MagicPowers",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicPowers",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "MagicPowers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Blurb",
                table: "MagicPowers",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "MagicGenerators",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicGenerators",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "MagicGenerators",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MagicCapabilities",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "MagicCapabilities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CapabilityModel",
                table: "MagicCapabilities",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "LoginIPs",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Locks",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WetShortDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WetDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "VagueTasteText",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "VagueSmellText",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TasteText",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "SmellText",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Liquids",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LongDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DrenchedShortDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DrenchedDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayColour",
                table: "Liquids",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'blue'",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'blue'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DampShortDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DampDescription",
                table: "Liquids",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AccountUserName",
                table: "LineOfCreditAccountUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AccountOwnerName",
                table: "LineOfCreditAccounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Limbs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LegalClasses",
                type: "varchar(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LegalAuthorities",
                type: "varchar(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Laws",
                type: "varchar(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EnforcementStrategy",
                table: "Laws",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "UnknownLanguageDescription",
                table: "Languages",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Languages",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "LanguageDifficultyModels",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LanguageDifficultyModels",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "LanguageDifficultyModels",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "knowledges",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subtype",
                table: "knowledges",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "knowledges",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LongDescription",
                table: "knowledges",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "knowledges",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ItemGroups",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true,
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "ItemGroups",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true,
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ItemGroupForms",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ItemGroupForms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Improvers",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Improvers",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Improvers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Hooks",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Hooks",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Hooks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Hooks",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Helpfiles_ExtraTexts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TagLine",
                table: "Helpfiles",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "Helpfiles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PublicText",
                table: "Helpfiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Helpfiles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LastEditedBy",
                table: "Helpfiles",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "Helpfiles",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Helpfiles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HeightWeightModels",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "HearingProfiles",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "SurveyDescription",
                table: "HearingProfiles",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HearingProfiles",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "HearingProfiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "HealthStrategies",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HealthStrategies",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "HealthStrategies",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupAITemplates",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "GroupAITemplates",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupAIs",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "GroupAIs",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "GroupAIs",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "GridType",
                table: "Grids",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Grids",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "VagueSmellText",
                table: "Gases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "SmellText",
                table: "Gases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Gases",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayColour",
                table: "Gases",
                type: "varchar(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Gases",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PositionTargetType",
                table: "GameItems",
                type: "varchar(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PositionEmote",
                table: "GameItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EffectData",
                table: "GameItems",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "VariableValue",
                table: "GameItemProtos_DefaultVariables",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "VariableName",
                table: "GameItemProtos_DefaultVariables",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "GameItemProtos",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GameItemProtos",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "MorphEmote",
                table: "GameItemProtos",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'$0 $?1|morphs into $1|decays into nothing$.'",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'$0 $?1|morphs into $1|decays into nothing$.'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LongDescription",
                table: "GameItemProtos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "GameItemProtos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "GameItemProtos",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CustomColour",
                table: "GameItemProtos",
                type: "varchar(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "GameItemComponents",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "GameItemComponentProtos",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GameItemComponentProtos",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GameItemComponentProtos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "GameItemComponentProtos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ParameterName",
                table: "FutureProgs_Parameters",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "FutureProgs",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FunctionText",
                table: "FutureProgs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FunctionName",
                table: "FutureProgs",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FunctionComment",
                table: "FutureProgs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "FutureProgs",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "QuantityDiceExpression",
                table: "Foragables",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Foragables",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ForagableTypes",
                table: "Foragables",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ForageType",
                table: "ForagableProfiles_MaximumYields",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ForageType",
                table: "ForagableProfiles_HourlyYieldGains",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ForagableProfiles",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "MudPeriodStart",
                table: "FinancialPeriods",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "MudPeriodEnd",
                table: "FinancialPeriods",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Verb2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Verb1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryKeyword2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryKeyword1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundTarget2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundTarget1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundDescription2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "OutboundDescription1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "InboundTarget2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "InboundTarget1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "InboundDescription2",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "InboundDescription1",
                table: "Exits",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "BlockedLayers",
                table: "Exits",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldDefaultValueSql: "''",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ethnicities",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EthnicSubgroup",
                table: "Ethnicities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EthnicGroup",
                table: "Ethnicities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ChargenBlurb",
                table: "Ethnicities",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "EntityDescriptions",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "EntityDescriptions",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "EntityDescriptionPatterns",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EnforcementAuthorities",
                type: "varchar(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "EmailTemplates",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ReturnAddress",
                table: "EmailTemplates",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "EmailTemplates",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewerComment",
                table: "EditableItems",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "BuilderComment",
                table: "EditableItems",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "EconomicZoneTaxes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceTime",
                table: "EconomicZones",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EconomicZones",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TargetType",
                table: "Dubs",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LastDescription",
                table: "Dubs",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "Dubs",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "IntroducedName",
                table: "Dubs",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalEffects",
                table: "DrugsIntensities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Drugs",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Dreams",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DreamerText",
                table: "Dream_Phases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DreamerCommand",
                table: "Dream_Phases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Drawings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Drawings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Doors",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "DisfigurementTemplates",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "DisfigurementTemplates",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DisfigurementTemplates",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "DisfigurementTemplates",
                type: "varchar(5000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(5000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "DisfigurementTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PerceivableType",
                table: "DefaultHooks",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DamagePatterns",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CurrencyDivisions",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "CurrencyDivisionAbbreviations",
                type: "varchar(150)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "NegativePrefix",
                table: "CurrencyDescriptionPatterns",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "CurrencyDescriptionPatternElementSpecialValues",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PluraliseWord",
                table: "CurrencyDescriptionPatternElements",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "CurrencyDescriptionPatternElements",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AlternatePattern",
                table: "CurrencyDescriptionPatternElements",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currencies",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PersonWordNeuter",
                table: "Cultures",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PersonWordMale",
                table: "Cultures",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PersonWordIndeterminate",
                table: "Cultures",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PersonWordFemale",
                table: "Cultures",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cultures",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Cultures",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "CultureInfos",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CultureInfos",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TimeOfReport",
                table: "Crimes",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TimeOfCrime",
                table: "Crimes",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CriminalShortDescription",
                table: "Crimes",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CriminalFullDescription",
                table: "Crimes",
                type: "varchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CriminalCharacteristics",
                table: "Crimes",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ToolType",
                table: "CraftTools",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CraftTools",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "QualityFormula",
                table: "Crafts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Crafts",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Crafts",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Blurb",
                table: "Crafts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ActiveCraftItemSDesc",
                table: "Crafts",
                type: "varchar(200)",
                nullable: false,
                defaultValueSql: "'a craft in progress'",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldDefaultValueSql: "'a craft in progress'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ActionDescription",
                table: "Crafts",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ProductType",
                table: "CraftProducts",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CraftProducts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FailEcho",
                table: "CraftPhases",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Echo",
                table: "CraftPhases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "InputType",
                table: "CraftInputs",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CraftInputs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CorpseModels",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CorpseModels",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CorpseModels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CorpseModels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "CombatMessages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FailureMessage",
                table: "CombatMessages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Colours",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Fancy",
                table: "Colours",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Coins",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PluralWord",
                table: "Coins",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Coins",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "GeneralForm",
                table: "Coins",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Coins",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Clocks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ClimateModels",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ClimateModels",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ClimateModels",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PayIntervalReferenceTime",
                table: "Clans",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PayIntervalReferenceDate",
                table: "Clans",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Clans",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Clans",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Clans",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "Clans",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PersonalName",
                table: "ClanMemberships",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "JoinDate",
                table: "ClanMemberships",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CheckTemplates",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CheckTemplates",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CheckMethod",
                table: "CheckTemplates",
                type: "varchar(25)",
                nullable: false,
                defaultValueSql: "'Standard'",
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldDefaultValueSql: "'Standard'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "StageDefinition",
                table: "ChargenScreenStoryboards",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Chargens",
                type: "varchar(12000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(12000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Chargens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ChargenRoles",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ChargenBlurb",
                table: "ChargenRoles",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ChargenResources",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TextDisplayedToPlayerOnDeduct",
                table: "ChargenResources",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TextDisplayedToPlayerOnAward",
                table: "ChargenResources",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PluralName",
                table: "ChargenResources",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ChargenResources",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "MaximumResourceFormula",
                table: "ChargenResources",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "ChargenResources",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AdviceTitle",
                table: "ChargenAdvices",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AdviceText",
                table: "ChargenAdvices",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ShortTermPlan",
                table: "Characters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PositionTargetType",
                table: "Characters",
                type: "varchar(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "PositionEmote",
                table: "Characters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Outfits",
                table: "Characters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "NeedsModel",
                table: "Characters",
                type: "varchar(50)",
                nullable: false,
                defaultValueSql: "'NoNeeds'",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldDefaultValueSql: "'NoNeeds'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "NameInfo",
                table: "Characters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LongTermPlan",
                table: "Characters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "IntroductionMessage",
                table: "Characters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EffectData",
                table: "Characters",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "BirthdayDate",
                table: "Characters",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Command",
                table: "CharacterLog",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "HowAcquired",
                table: "CharacterKnowledges",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "CharacteristicValues",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacteristicValues",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalValue",
                table: "CharacteristicValues",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CharacteristicProfiles",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacteristicProfiles",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CharacteristicProfiles",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CharacteristicProfiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "CharacteristicDefinitions",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacteristicDefinitions",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "CharacteristicDefinitions",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'standard'",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'standard'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CharacteristicDefinitions",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CharacteristicDefinitions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacterIntroTemplates",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "CharacterIntroTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CharacterCombatSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "MeleeAttackOrderPreference",
                table: "CharacterCombatSettings",
                type: "varchar(100)",
                nullable: false,
                defaultValueSql: "'0 1 2 3 4'",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldDefaultValueSql: "'0 1 2 3 4'",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CharacterCombatSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ClassificationsAllowed",
                table: "CharacterCombatSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelName",
                table: "Channels",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelColour",
                table: "Channels",
                type: "char(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(10)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Word",
                table: "ChannelCommandWords",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ForagableType",
                table: "Cells_ForagableYields",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "EffectData",
                table: "Cells",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CellOverlays",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CellName",
                table: "CellOverlays",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CellDescription",
                table: "CellOverlays",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "AtmosphereType",
                table: "CellOverlays",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'gas'",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldDefaultValueSql: "'gas'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CellOverlayPackages",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Celestials",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "Calendars",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                table: "Calendars",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "ButcheryProducts",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ButcheryProducts",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ButcheryProductItemscol",
                table: "ButcheryProductItems",
                type: "varchar(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Usage",
                table: "BodyProtos_AdditionalBodyparts",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WielderDescriptionSingle",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: false,
                defaultValueSql: "'hand'",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldDefaultValueSql: "'hand'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WielderDescriptionPlural",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: false,
                defaultValueSql: "'hands'",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldDefaultValueSql: "'hands'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LegDescriptionSingular",
                table: "BodyProtos",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'leg'",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'leg'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LegDescriptionPlural",
                table: "BodyProtos",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'legs'",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'legs'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ConsiderString",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BodypartShape",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BodypartProto",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BodypartProto",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ProximityGroup",
                table: "BodypartInternalInfos",
                type: "varchar(45)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "BodypartGroupDescribers",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "DescribedAs",
                table: "BodypartGroupDescribers",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "BodypartGroupDescribers",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Tattoos",
                table: "Bodies",
                type: "mediumtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Bodies",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Scars",
                table: "Bodies",
                type: "mediumtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Bodies",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "EffectData",
                table: "Bodies",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Boards",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BoardPosts",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "BoardPosts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Bloodtypes",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BloodtypeAntigens",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BloodModels",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Bans",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "IpMask",
                table: "Bans",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TemplateType",
                table: "AutobuilderRoomTemplates",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AutobuilderRoomTemplates",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "AutobuilderRoomTemplates",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TemplateType",
                table: "AutobuilderAreaTemplates",
                type: "varchar(45)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AutobuilderAreaTemplates",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "AutobuilderAreaTemplates",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AuthorityGroups",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ArtificialIntelligences",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ArtificialIntelligences",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ArtificialIntelligences",
                type: "mediumtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "mediumtext",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ArmourTypes",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Definition",
                table: "ArmourTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Areas",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Appointments_Titles",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Appointments_Abbreviations",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Appointments",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "StunExpression",
                table: "AmmunitionTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "SpecificType",
                table: "AmmunitionTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "RangedWeaponTypes",
                table: "AmmunitionTypes",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PainExpression",
                table: "AmmunitionTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AmmunitionTypes",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DamageExpression",
                table: "AmmunitionTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "UnitPreference",
                table: "Accounts",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "TimeZoneId",
                table: "Accounts",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationCode",
                table: "Accounts",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "RecoveryCode",
                table: "Accounts",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Accounts",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true,
                oldCollation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:Collation", "utf8mb4_unicode_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LastLoginIP",
                table: "Accounts",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Accounts",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "CultureName",
                table: "Accounts",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "AccountNotes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "AccountNotes",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "VagueSuffix",
                table: "Accents",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Suffix",
                table: "Accents",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accents",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Group",
                table: "Accents",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Accents",
                type: "varchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("MySql:Collation", "utf8_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8");
        }
    }
}
