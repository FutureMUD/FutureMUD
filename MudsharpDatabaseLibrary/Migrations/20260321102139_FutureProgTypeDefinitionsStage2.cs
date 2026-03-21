using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class FutureProgTypeDefinitionsStage2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "VariableValues");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "VariableDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "VariableDefaults");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "VariableValues");

            migrationBuilder.DropColumn(
                name: "ValueType",
                table: "VariableValues");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "VariableDefinitions");

            migrationBuilder.DropColumn(
                name: "ContainedType",
                table: "VariableDefinitions");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "VariableDefaults");

            migrationBuilder.DropColumn(
                name: "ParameterType",
                table: "FutureProgs_Parameters");

            migrationBuilder.DropColumn(
                name: "ReturnType",
                table: "FutureProgs");

            migrationBuilder.UpdateData(
                table: "VariableValues",
                keyColumn: "ReferenceTypeDefinition",
                keyValue: null,
                column: "ReferenceTypeDefinition",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceTypeDefinition",
                table: "VariableValues",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.UpdateData(
                table: "VariableDefinitions",
                keyColumn: "OwnerTypeDefinition",
                keyValue: null,
                column: "OwnerTypeDefinition",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerTypeDefinition",
                table: "VariableDefinitions",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.UpdateData(
                table: "VariableDefaults",
                keyColumn: "OwnerTypeDefinition",
                keyValue: null,
                column: "OwnerTypeDefinition",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerTypeDefinition",
                table: "VariableDefaults",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "VariableValues",
                columns: new[] { "ReferenceTypeDefinition", "ReferenceId", "ReferenceProperty" });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "VariableDefinitions",
                columns: new[] { "OwnerTypeDefinition", "Property" });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "VariableDefaults",
                columns: new[] { "OwnerTypeDefinition", "Property" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "VariableValues");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "VariableDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "VariableDefaults");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceTypeDefinition",
                table: "VariableValues",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "ReferenceType",
                table: "VariableValues",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ValueType",
                table: "VariableValues",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerTypeDefinition",
                table: "VariableDefinitions",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "OwnerType",
                table: "VariableDefinitions",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ContainedType",
                table: "VariableDefinitions",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerTypeDefinition",
                table: "VariableDefaults",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "OwnerType",
                table: "VariableDefaults",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ParameterType",
                table: "FutureProgs_Parameters",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReturnType",
                table: "FutureProgs",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql(
                "UPDATE FutureProgs SET ReturnType = CONV(SUBSTRING(ReturnTypeDefinition, 4), 16, 10);");

            migrationBuilder.Sql(
                "UPDATE FutureProgs_Parameters SET ParameterType = CONV(SUBSTRING(ParameterTypeDefinition, 4), 16, 10);");

            migrationBuilder.Sql(
                "UPDATE VariableDefaults SET OwnerType = CONV(SUBSTRING(OwnerTypeDefinition, 4), 16, 10);");

            migrationBuilder.Sql(
                "UPDATE VariableDefinitions SET OwnerType = CONV(SUBSTRING(OwnerTypeDefinition, 4), 16, 10), ContainedType = CONV(SUBSTRING(ContainedTypeDefinition, 4), 16, 10);");

            migrationBuilder.Sql(
                "UPDATE VariableValues SET ReferenceType = CONV(SUBSTRING(ReferenceTypeDefinition, 4), 16, 10), ValueType = CONV(SUBSTRING(ValueTypeDefinition, 4), 16, 10);");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "VariableValues",
                columns: new[] { "ReferenceType", "ReferenceId", "ReferenceProperty" });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "VariableDefinitions",
                columns: new[] { "OwnerType", "Property" });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "VariableDefaults",
                columns: new[] { "OwnerType", "Property" });
        }
    }
}
