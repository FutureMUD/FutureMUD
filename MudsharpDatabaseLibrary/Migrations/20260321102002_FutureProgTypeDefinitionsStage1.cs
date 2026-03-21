using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class FutureProgTypeDefinitionsStage1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "FK_Races_CharacterCombatSettings_idx",
                table: "Races",
                newName: "IX_Races_DefaultCombatSettingId");

            migrationBuilder.RenameIndex(
                name: "FK_CharacterCombatSettings_PriorityProg_idx",
                table: "CharacterCombatSettings",
                newName: "IX_CharacterCombatSettings_PriorityProgId");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceTypeDefinition",
                table: "VariableValues",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "ValueTypeDefinition",
                table: "VariableValues",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "ContainedTypeDefinition",
                table: "VariableDefinitions",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "OwnerTypeDefinition",
                table: "VariableDefinitions",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "OwnerTypeDefinition",
                table: "VariableDefaults",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "ParameterTypeDefinition",
                table: "FutureProgs_Parameters",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "ReturnTypeDefinition",
                table: "FutureProgs",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.Sql(
                "UPDATE FutureProgs SET ReturnTypeDefinition = CONCAT('v1:', LOWER(HEX(ReturnType)));");

            migrationBuilder.Sql(
                "UPDATE FutureProgs_Parameters SET ParameterTypeDefinition = CONCAT('v1:', LOWER(HEX(ParameterType)));");

            migrationBuilder.Sql(
                "UPDATE VariableDefaults SET OwnerTypeDefinition = CONCAT('v1:', LOWER(HEX(OwnerType)));");

            migrationBuilder.Sql(
                "UPDATE VariableDefinitions SET OwnerTypeDefinition = CONCAT('v1:', LOWER(HEX(OwnerType))), ContainedTypeDefinition = CONCAT('v1:', LOWER(HEX(ContainedType)));");

            migrationBuilder.Sql(
                "UPDATE VariableValues SET ReferenceTypeDefinition = CONCAT('v1:', LOWER(HEX(ReferenceType))), ValueTypeDefinition = CONCAT('v1:', LOWER(HEX(ValueType)));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceTypeDefinition",
                table: "VariableValues");

            migrationBuilder.DropColumn(
                name: "ValueTypeDefinition",
                table: "VariableValues");

            migrationBuilder.DropColumn(
                name: "ContainedTypeDefinition",
                table: "VariableDefinitions");

            migrationBuilder.DropColumn(
                name: "OwnerTypeDefinition",
                table: "VariableDefinitions");

            migrationBuilder.DropColumn(
                name: "OwnerTypeDefinition",
                table: "VariableDefaults");

            migrationBuilder.DropColumn(
                name: "ParameterTypeDefinition",
                table: "FutureProgs_Parameters");

            migrationBuilder.DropColumn(
                name: "ReturnTypeDefinition",
                table: "FutureProgs");

            migrationBuilder.RenameIndex(
                name: "IX_Races_DefaultCombatSettingId",
                table: "Races",
                newName: "FK_Races_CharacterCombatSettings_idx");

            migrationBuilder.RenameIndex(
                name: "IX_CharacterCombatSettings_PriorityProgId",
                table: "CharacterCombatSettings",
                newName: "FK_CharacterCombatSettings_PriorityProg_idx");
        }
    }
}
