using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldSunCelestialDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CelestialType",
                table: "Celestials",
                type: "varchar(30)",
                nullable: false,
                defaultValue: "Sun",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldDefaultValue: "OldSun",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CelestialType",
                table: "Celestials",
                type: "varchar(30)",
                nullable: false,
                defaultValue: "OldSun",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldDefaultValue: "Sun",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }
    }
}
