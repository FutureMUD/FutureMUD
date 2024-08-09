using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class TrackingNameForBodyProtos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WielderDescriptionSingle",
                table: "BodyProtos",
                type: "varchar(100)",
                nullable: false,
                defaultValueSql: "'hand'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldDefaultValueSql: "'hand'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WielderDescriptionPlural",
                table: "BodyProtos",
                type: "varchar(100)",
                nullable: false,
                defaultValueSql: "'hands'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldDefaultValueSql: "'hands'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BodyProtos",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LegDescriptionSingular",
                table: "BodyProtos",
                type: "varchar(100)",
                nullable: false,
                defaultValueSql: "'leg'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'leg'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LegDescriptionPlural",
                table: "BodyProtos",
                type: "varchar(100)",
                nullable: false,
                defaultValueSql: "'legs'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldDefaultValueSql: "'legs'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "NameForTracking",
                table: "BodyProtos",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameForTracking",
                table: "BodyProtos");

            migrationBuilder.AlterColumn<string>(
                name: "WielderDescriptionSingle",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: false,
                defaultValueSql: "'hand'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldDefaultValueSql: "'hand'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "WielderDescriptionPlural",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: false,
                defaultValueSql: "'hands'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldDefaultValueSql: "'hands'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BodyProtos",
                type: "varchar(4000)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LegDescriptionSingular",
                table: "BodyProtos",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'leg'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldDefaultValueSql: "'leg'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "LegDescriptionPlural",
                table: "BodyProtos",
                type: "varchar(1000)",
                nullable: false,
                defaultValueSql: "'legs'",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldDefaultValueSql: "'legs'",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }
    }
}
